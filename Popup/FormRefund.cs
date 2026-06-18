using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NailsChekin.Models;
using NailsChekin.Models.Helper;
using NailsChekin.MyControls;
using NailsChekin.UserControl;
using Newtonsoft.Json.Linq;

namespace NailsChekin.Popup
{
    public partial class FormRefund : Form
    {
        private readonly Control _parent;
        private readonly string _orderId;

        // Tham chiếu giao dịch gốc để hoàn tiền qua máy (lấy từ GET Order/{id} - hướng B)
        private string _paymentId = "";       // Clover: externalPaymentId | CodePay: trans_no
        private string _cloverOrderId = "";   // Clover: order id | CodePay: merchant_order_no
        private bool _isCredit = false;       // đơn thanh toán thẻ?

        // True khi đã refund thành công ít nhất 1 item -> parent reload lại list khi đóng
        public bool refunded = false;

        public FormRefund()
        {
            InitializeComponent();
        }

        public FormRefund(Control parent, string orderId)
        {
            InitializeComponent();

            this._parent = parent;
            this._orderId = orderId;
            this.lbTitle.Text = "REFUND  -  ORDER #" + orderId;

            // Checkbox "chọn tất cả" trên header (☐ xám khi chưa chọn, ✓ trắng nền xanh khi chọn)
            UIHelper.StyleSelectCheckbox(chkSelected);
            this.chkSelected.CheckedChanged += chkSelectAll_CheckedChanged;
        }

        private async void FormRefund_Load(object sender, EventArgs e)
        {
            this.Adjust_Screen();
            await LoadOrder();
        }

        public void Adjust_Screen()
        {
            //Bất đồng bộ
            this.BeginInvoke(new Action(() =>
            {
                panelLayout_Content.Width = this.Width - 25;
                panelLayout_Content.Height = this.Height - panelLayout_Header.Height - 20;

                btnClose.Location = new Point(this.Width - btnClose.Width - 10, btnClose.Location.Y);
            }));
        }

        private async Task LoadOrder()
        {
            SetBusy(true);
            lbStatus.Text = "Status: Loading...";

            string responce = await Task.Run(() => Utilitys.CALL_API("Order/" + _orderId, "", "GET", true));
            if (this.IsDisposed) return;

            if (IsApiError(responce))
            {
                CustomMessageBox.Show("Load order error: " + Environment.NewLine + responce);
                SetBusy(false);
                return;
            }

            JObject obj;
            try { obj = JObject.Parse(responce); }
            catch { CustomMessageBox.Show("Load order error !!!"); SetBusy(false); return; }

            _paymentId = Str(Get(obj, "paymentId"));
            _cloverOrderId = Str(Get(obj, "cloverOrderId"));
            int methodOfPayment = (int)Num(Get(obj, "methodOfPayment"));
            _isCredit = methodOfPayment == 1
                || !string.IsNullOrEmpty(_paymentId)
                || !string.IsNullOrEmpty(_cloverOrderId);

            string date = Str(Get(obj, "orderDate"));
            lbDate.Text = "Date: " + (DateTime.TryParse(date, out DateTime dt) ? dt.ToString("MM/dd/yyyy h:mm tt") : date);
            lbCustomer.Text = "Customer: " + Str(Get(obj, "customerName"));
            lbTotal.Text = "Total: $" + Money(Get(obj, "orderTotal", "ordertotal"));
            lbPaid.Text = "Paid: $" + Money(Get(obj, "paidAmount"));

            string statusStr = Str(Get(obj, "orderStatusString"));
            lbStatus.Text = "Status: " + statusStr;
            lbStatus.ForeColor = statusStr.Equals("Returned") ? Color.Red : Color.Black;

            // Reset header select-all (không kích hoạt toggle-all)
            chkSelected.CheckedChanged -= chkSelectAll_CheckedChanged;
            chkSelected.Checked = false;
            chkSelected.CheckedChanged += chkSelectAll_CheckedChanged;

            // Dispose row cũ trước khi clear
            var content = panelCartItemsTouch.Content;
            var oldRows = content.Controls.OfType<UCSaleItemRefund>().ToList();
            content.Controls.Clear();
            foreach (var old in oldRows) old.Dispose();

            // Tạo các row + tính vị trí trước, add 1 lần
            JArray items = Get(obj, "items") as JArray ?? new JArray();
            int itemWidth = panelCartItemsTouch.Width - 5;
            int locationY = 0;
            var rows = new List<UCSaleItemRefund>();
            foreach (JToken tok in items)
            {
                JObject it = tok as JObject;
                if (it == null) continue;

                string itemId = Str(Get(it, "itemId"));
                string itemName = Str(Get(it, "itemName"));
                double qty = Num(Get(it, "qty"));
                double price = Num(Get(it, "price"));
                double discount = Num(Get(it, "discount"));
                double sub = Num(Get(it, "subTotal", "subtotal"));
                bool isRefunded = (int)Num(Get(it, "refunded")) == 1;

                var row = new UCSaleItemRefund(this, itemId, itemName, price, qty, discount, sub, isRefunded);
                row.Width = itemWidth;
                row.Location = new Point(0, locationY);
                locationY += row.Height + 2;
                rows.Add(row);
            }

            content.SuspendLayout();
            try { content.Controls.AddRange(rows.ToArray()); }
            finally { content.ResumeLayout(); }

            bool canRefund = statusStr.Equals("Paid") && rows.Any(r => !r.already_refunded);
            SetBusy(false);
            btnRefundAll.Enabled = canRefund;
            btnRefundSelected.Enabled = canRefund;
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = chkSelected.Checked;
            UIHelper.UpdateSelectCheckboxGlyph(chkSelected);
            foreach (var row in panelCartItemsTouch.Content.Controls.OfType<UCSaleItemRefund>())
                row.SetSelected(isChecked);
        }

        private async void btnRefundAll_Click(object sender, EventArgs e)
        {
            if (CustomMessageBox.Show("Refund ALL items of this order?", "Confirm Refund",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            double totalRefund = panelCartItemsTouch.Content.Controls.OfType<UCSaleItemRefund>()
                .Where(r => !r.already_refunded).Sum(r => r.amount);

            SetBusy(true);

            // 1) Hoàn tiền qua máy (đơn thẻ). Trả về số GD hoàn (refund_transactionNo); null = lỗi máy.
            string refundTransNo = await RefundOnDevice(totalRefund);
            if (this.IsDisposed) return;
            if (refundTransNo == null) { await LoadOrder(); return; }

            // 2) Ghi nhận refund toàn bộ order (itemId rỗng), kèm số GD hoàn của máy
            string res = await Task.Run(() => Utilitys.CALL_API("Order/refund", BuildRefundJson("", "", 0, refundTransNo), "POST", true));
            if (this.IsDisposed) return;

            if (IsApiError(res))
            {
                CustomMessageBox.Show("Refund error: " + Environment.NewLine + res);
                await LoadOrder();
                return;
            }

            refunded = true;
            PrintRefundReceipt();
            CustomMessageBox.Show("Refund successful !!!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        /// <summary>
        /// Phát lệnh hoàn tiền qua máy nếu là đơn thẻ.
        /// Trả về refund_transactionNo (số GD hoàn) để ghi DB ("" nếu đơn tiền mặt / không dùng máy);
        /// trả về null nếu lỗi máy (đã hiện thông báo) -> caller dừng, KHÔNG ghi DB.
        /// </summary>
        private async Task<string> RefundOnDevice(double amount)
        {
            if (!_isCredit) return ""; // đơn tiền mặt -> không hoàn qua máy, refund_transactionNo rỗng

            FormMain main = UIHelper.GetParentForm<FormSaleList>(_parent)?.ParentMain;
            if (main == null) return ""; // fallback: chỉ ghi DB

            lbStatus.Text = "Status: Processing refund on device...";
            var res = await main.RefundOnDeviceAsync(_paymentId, _cloverOrderId, amount);
            if (this.IsDisposed) return null;

            if (res.Skipped) return "";
            if (!res.Success)
            {
                CustomMessageBox.Show("Refund device error: " + Environment.NewLine + res.Error);
                return null;
            }
            return res.RefundTransNo ?? "";
        }

        private async void btnRefundSelected_Click(object sender, EventArgs e)
        {
            var rows = panelCartItemsTouch.Content.Controls.OfType<UCSaleItemRefund>()
                .Where(r => r.selected && !r.already_refunded)
                .ToList();

            if (rows.Count == 0)
            {
                CustomMessageBox.Show("Please select item(s) to refund !!!");
                return;
            }

            if (CustomMessageBox.Show("Refund " + rows.Count + " selected item(s)?", "Confirm Refund",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            double totalRefund = rows.Sum(r => r.amount);

            SetBusy(true);

            // 1) Hoàn tiền qua máy 1 lần cho tổng item đã chọn
            string refundTransNo = await RefundOnDevice(totalRefund);
            if (this.IsDisposed) return;
            if (refundTransNo == null) { await LoadOrder(); return; }

            // 2) Ghi nhận từng item, kèm số GD hoàn của máy
            string res = "OK";
            foreach (var row in rows)
            {
                string DATA = BuildRefundJson(row.item_id, row.item_name, row.amount, refundTransNo);
                res = await Task.Run(() => Utilitys.CALL_API("Order/refund", DATA, "POST", true));
                if (this.IsDisposed) return;

                if (IsApiError(res)) break;
                refunded = true;
            }

            if (IsApiError(res))
            {
                CustomMessageBox.Show("Refund error: " + Environment.NewLine + res);
                await LoadOrder();
                return;
            }

            PrintRefundReceipt();
            CustomMessageBox.Show("Refund successful !!!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Reload để cập nhật trạng thái item; nếu đã refund hết -> đóng
            await LoadOrder();
            if (!this.IsDisposed && !btnRefundAll.Enabled)
                this.Close();
        }

        // In lại receipt của order sau khi refund: dùng chung template Sale/Combine.
        // Trạng thái RETURNED do SQL ftTikectPrinter trả về (orderStatus = 3); PrintDirectTicket tự kèm chữ ký nếu là đơn thẻ.
        private void PrintRefundReceipt()
        {
            try
            {
                PrinterLocalHelper.PrintDirectTicket(_orderId, "");
            }
            catch { /* lỗi in không được chặn flow refund */ }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string BuildRefundJson(string itemId, string itemName, double amount, string transactionId)
        {
            // Build bằng JObject để tránh lỗi khi itemName có dấu nháy.
            // transactionId = số GD hoàn của máy (refund_transactionNo) cho đơn thẻ; "" cho đơn tiền mặt.
            var payload = new JObject
            {
                ["orderId"] = long.Parse(_orderId),
                ["itemId"] = itemId,
                ["itemName"] = itemName,
                ["amount"] = amount,
                ["transactionId"] = transactionId ?? ""
            };
            return payload.ToString();
        }

        private void SetBusy(bool busy)
        {
            this.UseWaitCursor = busy;
            if (busy)
            {
                // Caller bật lại nút khi load xong
                btnRefundAll.Enabled = false;
                btnRefundSelected.Enabled = false;
            }
        }

        private static bool IsApiError(string res)
        {
            return string.IsNullOrEmpty(res) || res.ToUpper().StartsWith("ERROR");
        }

        // ── Helpers parse JSON (không phân biệt hoa thường) ───────────────
        private static JToken Get(JObject o, params string[] names)
        {
            if (o == null) return null;
            foreach (var n in names)
            {
                var t = o.GetValue(n, StringComparison.OrdinalIgnoreCase);
                if (t != null && t.Type != JTokenType.Null) return t;
            }
            return null;
        }

        private static string Str(JToken t) => t == null ? "" : t.ToString();

        private static double Num(JToken t)
        {
            if (t == null) return 0;
            return double.TryParse(t.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double d) ? d : 0;
        }

        private static string Money(JToken t) => Num(t).ToString("0.00");

        private void FormRefund_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Parent (UCSaleItem) tự reload list sau khi ShowDialog đóng nếu đã refund

            //Xử lý không bị cảm giác giật do xài Dispose() ngay nếu đóng thẳng
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000); // 3 giây

                try
                {
                    Core.ClearAndDisposeV2(panelLayout_Content);
                    Core.ClearMemory();

                    if (!this.IsDisposed)
                    {
                        this.Invoke((Action)(() => this.Dispose()));
                    }
                }
                catch { /* form đã dispose rồi thì thôi */ }
            });
        }
    }
}
