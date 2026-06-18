using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using NailsChekin.Models;
using NailsChekin.Models.Helper;
using NailsChekin.MyControls;
using NailsChekin.UserControl;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NailsChekin.Popup
{
    public partial class FormSaleList : Form
    {
        FormMain parentForm = null;
        public FormMain ParentMain => parentForm;   // cho FormRefund truy cập thiết bị (Clover/P5) để hoàn tiền
        public string status = "";
        private bool _isSearching = false;

        public FormSaleList()
        {
            InitializeComponent();
        }

        public FormSaleList(FormMain parent, string status)
        {
            InitializeComponent();

            this.BackColor = ColorHelper.DefaultBackgoundColor;
            panelLayout_Content.BorderColor = ColorHelper.DefaultBorderColor;

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();

            UIHelper.EnableDeepDoubleBuffer(this);
            typeof(Panel).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(panelTicketsTouch, true, null);

            this.parentForm = parent;
            this.status = status;
            txtSearchCustomer.Text = "";

            if (status.Equals("0"))
                lbTitle.Text = "SALE OPEN";
        }

        private void FormSaleList_Load(object sender, EventArgs e)
        {
            this.Adjust_Screen();

            UIHelper.StyleSelectCheckbox(chkSelectAll);

            DXPopupMenu popupPaidBy = new DXPopupMenu();
            popupPaidBy.Items.Add(new DXMenuItem() { Caption = "ALL" });
            popupPaidBy.Items.Add(new DXMenuItem() { Caption = "CASH" });
            popupPaidBy.Items.Add(new DXMenuItem() { Caption = "CREDIT" });
            ddlPaidBySearch.DropDownControl = popupPaidBy;
            foreach (DXMenuItem item in popupPaidBy.Items)
                item.Click += item_paidby_Click;

            if (this.status.Equals("0"))
            {
                txtFromDate.Text = DateTime.Now.ToString("MM-01-yyyy");
                txtToDate.Text = DateTime.Now.ToString("MM-dd-yyyy");

                SetupCombineButtons();   // chỉ SALE OPEN mới có combine
            }
            else
            {
                txtFromDate.Text = DateTime.Now.ToString("MM-dd-yyyy");
                txtToDate.Text = DateTime.Now.ToString("MM-dd-yyyy");

                btnDeleteSelected.Width += 100;
                btnDeleteSelected.Left = panelControl.Width - btnDeleteSelected.Width - 30;
                btnDeleteSelected.TitleFontSize = 20f;
            }

        }

        // ===== Combine open sale =====
        private MyControls.ButtonRound btnCombineSelected;
        private MyControls.ButtonRound btnRemoveCombine;

        private void SetupCombineButtons()
        {
            // Chia vùng nút bulk (btnDeleteSelected) thành 3: DELETE | COMBINE | REMOVE
            int y = btnDeleteSelected.Location.Y;
            int h = btnDeleteSelected.Height;
            int startX = btnDeleteSelected.Location.X;
            int totalW = panelControl.Width - btnDeleteSelected.Width - 6;
            int gap = 6;
            int w = (totalW - gap * 2) / 3;

            btnDeleteSelected.Title = "DELETE";
            btnDeleteSelected.TitleFontSize = 14F;

            btnCombineSelected = NewBulkButton("COMBINE", Color.FromArgb(20, 113, 148), startX + w + gap, y, w, h);
            btnCombineSelected.Click += btnCombineSelected_Click;

            btnRemoveCombine = NewBulkButton("REMOVE", Color.FromArgb(150, 90, 20), startX + (w + gap) * 2, y, w, h);
            btnRemoveCombine.Click += btnRemoveCombine_Click;

            panelControl.Controls.Add(btnCombineSelected);
            panelControl.Controls.Add(btnRemoveCombine);
            btnCombineSelected.BringToFront();
            btnRemoveCombine.BringToFront();
        }

        private MyControls.ButtonRound NewBulkButton(string title, Color color, int x, int y, int w, int h)
        {
            return new MyControls.ButtonRound
            {
                BackColor = Color.Transparent,
                Title = title,
                TitleBackColor = color,
                BorderColor = color,
                TitleForeColor = Color.White,
                Font = new Font("Microsoft Sans Serif", 14F),
                TitleFontSize = 14F,
                Location = new Point(x, y),
                Size = new Size(w, h),
                MinimumSize = new Size(60, 36),
                Cursor = Cursors.Hand,
                Visible = false
            };
        }

        private void item_paidby_Click(object sender, EventArgs e)
        {
            ddlPaidBySearch.Text = ((DXMenuItem)sender).Caption;
        }

        public void Adjust_Screen()
        {
            //Bất đồng bộ
            this.BeginInvoke(new Action(() =>
            {
                panelLayout_Content.Width = this.Width - 10;
                panelLayout_Content.Height = this.Height - panelLayout_Header.Height - 5;

                panelControl.Width = panelLayout_Content.Width - txtSearchReceipt.Right - 20;

                panel_Content.Height = panelLayout_Content.Height - btnFindNow.Bottom - 25;
                btnClose.Location = new Point(this.Width - btnClose.Width - 10, btnClose.Location.Y);
            }));
        }

        private void FormSaleList_Shown(object sender, EventArgs e)
        {
            this.SendSearch();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            // Sau khi 1 popup con (vd FormRefund) đóng, RoundPanel có thể bị mất bo tròn -> vẽ lại
            panelLayout_Content?.ReapplyRegion();
        }

        private void btnFindNow_Click(object sender, EventArgs e)
        {
            this.SendSearch();
        }

        public async void SendSearch(bool reload_update = false)
        {
            if (_isSearching) return;
            _isSearching = true;
            btnFindNow.Enabled = false;

            var content = panelTicketsTouch.Content;
            var oldControls = content.Controls.OfType<UCSaleItem>().ToList();
            content.Controls.Clear();
            foreach (var old in oldControls) old.Dispose();

            var lbWaiting = new Label
            {
                Text = "Searching... Please wait",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 26f, FontStyle.Bold),
                ForeColor = Color.Gray,
                BackColor = Color.Transparent
            };
            panelTicketsTouch.Controls.Add(lbWaiting);
            lbWaiting.BringToFront();

            // Capture input values trên UI thread trước khi đi async
            string currentStatus = this.status;
            string DATA = @"{
                            'daysAgo': " + (currentStatus.Equals("0") ? "31" : "0") + @",
                            'orderStatus':'" + currentStatus + @"',
                            'fromDate':'" + txtFromDate.Text + @"',
                            'toDate':'" + txtToDate.Text + @"',
                            'paidBy':'" + ddlPaidBySearch.Text + @"',
                            'orderId':'" + Regex.Replace(txtSearchReceipt.Text, "\r", "") + @"',
                            'customer':'" + Regex.Replace(txtSearchCustomer.Text, "\r", "") + @"',
                            'product':'',
                            'searchString':'',
                            'pageIndex':0,
                            'pageSize':200
                        }";

            try
            {
                // Gọi API trên background thread — không block UI
                string responce = await Task.Run(() => Utilitys.CALL_API("Order/ordersHistory", DATA, "POST", true));
                if (this.IsDisposed) return;

                if (responce.StartsWith("Error"))
                {
                    CustomMessageBox.Show(responce);
                    return;
                }

                // Parse JSON trên background thread
                var items = await Task.Run(() =>
                {
                    var list = new List<(string orderId, string orderDate, string name, string phone,
                        string products, string amount, string cash, string charge,
                        string orderStatus, string orderStatusString, string orderSource, string paymentStatus,
                        string combineId)>();
                    foreach (JObject obj in JArray.Parse(responce))
                        list.Add((
                            obj["orderId"].ToString(),     obj["orderDate"].ToString(),
                            obj["name"].ToString(),        obj["phone"].ToString(),
                            obj["products"].ToString(),    obj["subtotal"].ToString(),
                            obj["cash"].ToString(),        obj["charge"].ToString(),
                            obj["orderStatus"].ToString(), obj["orderStatusString"].ToString(),
                            obj["order_source"].ToString(), obj["payment_status"].ToString(),
                            obj["combine_id"]?.ToString() ?? ""
                        ));
                    return list;
                });
                if (this.IsDisposed) return;

                panelTicketsTouch.Controls.Remove(lbWaiting);
                lbWaiting.Dispose();
                lbWaiting = null;

                // Tạo tất cả controls + tính vị trí trước, không add vào panel trong vòng lặp
                int itemWidth = panelTicketsTouch.Width - 5;
                int locationY = 5;
                var controls = new UCSaleItem[items.Count];
                for (int i = 0; i < items.Count; i++)
                {
                    var it = items[i];
                    var ctrl = new UCSaleItem(it.orderId, it.orderDate, it.name, it.phone,
                        it.products, it.amount, it.cash, it.charge, currentStatus, it.orderStatusString);
                    if (currentStatus.Equals("0"))
                    {
                        ctrl.SetOrderUnpaid(it.orderStatus, it.paymentStatus, it.orderSource);
                    }

                    ctrl.SetCombineId(it.combineId);
                    ctrl.Width = itemWidth;
                    ctrl.Location = new Point(0, locationY);
                    locationY += ctrl.Height + 2;
                    controls[i] = ctrl;
                }

                // Add tất cả 1 lần duy nhất — 1 layout pass thay vì N lần
                content.SuspendLayout();
                try   { content.Controls.AddRange(controls); }
                finally { content.ResumeLayout(); }
            }
            finally
            {
                if (!this.IsDisposed)
                {
                    _isSearching = false;
                    btnFindNow.Enabled = true;
                    if (lbWaiting != null && !lbWaiting.IsDisposed)
                    {
                        panelTicketsTouch.Controls.Remove(lbWaiting);
                        lbWaiting.Dispose();
                    }
                }
            }
        }

        public async void SaleOpen_Payment(string orderId)
        {
            // Gọi API trên background thread — không block UI
            string responce = await Task.Run(() => Utilitys.CALL_API("Order/" + orderId, "", "GET", true));
            if (this.IsDisposed) return;

            if (responce.StartsWith("Error"))
            {
                CustomMessageBox.Show(responce);
                return;
            }

            // Parse 1 lần duy nhất
            var jObj = JObject.Parse(responce);
            string orderSource = jObj["orderSource"]?.ToString() ?? "POS";
            string paidAmount  = jObj["paidAmount"]?.ToString()  ?? "0";
            string items       = jObj["items"]?.ToString()       ?? "[]";

            string customerId = jObj["customerId"]?.ToString() ?? "0";
            string customerName = jObj["customerName"]?.ToString() ?? "GUEST";
            string customerPhone = jObj["customerPhone"]?.ToString() ?? "";
            this.parentForm.SetCustomerInfo(customerId, customerPhone, customerName, "");

            // Đơn lẻ đã thu 1 phần: nạp lại full payment cũ từ jPayment server trả ngay trong response
            // (CreateUpdateOrder REPLACE -> phải gửi lại đủ list khi complete, nếu không sẽ mất payment cũ).
            double paid = Utilitys.getTotalAmount(paidAmount);
            var priorPayments = paid > 0
                ? CartHelper.ParsePaymentJson(jObj["paymentJson"]?.ToString())
                : null;

            this.parentForm.AddSaleItemToCard(orderId, items, paidAmount, orderSource, priorPayments);
            this.Close(); // dùng Close() để trigger FormClosed cleanup đúng cách
        }

        private void txtFromDate_Click(object sender, EventArgs e)
        {
            FormSelectDate frm = new FormSelectDate(this, "txtFromDate");
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog(this);
            frm.Dispose();
        }

        private void txtToDate_Click(object sender, EventArgs e)
        {
            FormSelectDate frm = new FormSelectDate(this, "txtToDate");
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog(this);
            frm.Dispose();
        }

        public void setDate(string controlId, string value)
        {
            TextEdit dateControl = (TextEdit)this.Controls.Find(controlId, true)[0];
            dateControl.Text = value;
        }

        private void ddlPaidBySearch_Click(object sender, EventArgs e)
        {
            ddlPaidBySearch.ShowDropDown();
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = chkSelectAll.Checked;
            UIHelper.UpdateSelectCheckboxGlyph(chkSelectAll);
            foreach (UCSaleItem ctr in panelTicketsTouch.Content.Controls.OfType<UCSaleItem>())
                ctr.SetSelected(isChecked);
            UpdateDeleteButtonVisibility();
        }

        public void UpdateDeleteButtonVisibility()
        {
            var sel = panelTicketsTouch.Content.Controls.OfType<UCSaleItem>().Where(c => c.selected).ToList();
            btnDeleteSelected.Visible = sel.Count > 0;

            if (btnCombineSelected != null)   // chỉ SALE OPEN mới có combine
            {
                btnCombineSelected.Visible = sel.Count >= 2;
                btnRemoveCombine.Visible = sel.Any(c => !string.IsNullOrEmpty(c.combine_id));
            }
        }

        private async void btnCombineSelected_Click(object sender, EventArgs e)
        {
            var sel = panelTicketsTouch.Content.Controls.OfType<UCSaleItem>().Where(c => c.selected).ToList();

            if (sel.Count < 2)
            {
                CustomMessageBox.Show("Please select at least 2 orders to combine !!!");
                return;
            }

            string posIds = string.Join(",", sel.Where(c => c.order_source == "POS").Select(c => c.id));
            string webIds = string.Join(",", sel.Where(c => c.order_source != "POS").Select(c => c.id));

            string DATA = @"{ 'posOrderIds': '" + posIds + @"', 'webOrderIds': '" + webIds + @"', 'combineId': '' }";
            string res = await Task.Run(() => Utilitys.CALL_API("Order/setCombine", DATA, "POST", true));
            if (this.IsDisposed) return;

            if (string.IsNullOrEmpty(res) || res.ToUpper().StartsWith("ERROR"))
            {
                CustomMessageBox.Show("Combine error: " + Environment.NewLine + res);
                return;
            }

            // res = combine_id mới -> nạp tất cả đơn đã chọn vào cart và mở màn thanh toán ngay
            string combineId = res.Trim();
            var members = sel
                .Select(c => (id: c.id, source: string.IsNullOrEmpty(c.order_source) ? "POS" : c.order_source))
                .ToList();

            this.parentForm.LoadCombineToCart(combineId, members);
            this.Close();
        }

        private async void btnRemoveCombine_Click(object sender, EventArgs e)
        {
            var sel = panelTicketsTouch.Content.Controls.OfType<UCSaleItem>()
                .Where(c => c.selected && !string.IsNullOrEmpty(c.combine_id)).ToList();
            string posIds = string.Join(",", sel.Where(c => c.order_source == "POS").Select(c => c.id));
            string webIds = string.Join(",", sel.Where(c => c.order_source != "POS").Select(c => c.id));

            if (string.IsNullOrEmpty(posIds) && string.IsNullOrEmpty(webIds))
            {
                CustomMessageBox.Show("Please select combined orders to remove !!!");
                return;
            }

            string DATA = @"{ 'posOrderIds': '" + posIds + @"', 'webOrderIds': '" + webIds + @"' }";
            string res = await Task.Run(() => Utilitys.CALL_API("Order/removeCombine", DATA, "POST", true));
            if (this.IsDisposed) return;

            if (res.ToUpper().StartsWith("ERROR"))
            {
                CustomMessageBox.Show("Remove combine error: " + Environment.NewLine + res);
                return;
            }

            this.SendSearch();
        }

        // Thanh toán cả combine: nạp item của tất cả đơn trong nhóm (cả POS lẫn WEB) vào cart rồi để cashier thanh toán
        public void PayCombine(string combineId)
        {
            var members = panelTicketsTouch.Content.Controls.OfType<UCSaleItem>()
                .Where(c => c.combine_id == combineId)
                .Select(c => (id: c.id, source: string.IsNullOrEmpty(c.order_source) ? "POS" : c.order_source))
                .ToList();
            if (members.Count == 0) return;

            this.parentForm.LoadCombineToCart(combineId, members);
            this.Close();
        }

        // In 1 receipt TỔNG cho cả combine: cộng cash/charge của tất cả đơn trong nhóm (change = 0 khi in lại).
        // Nếu combine trả thẻ, ftTikectPrinterCombine trả payment_info thẻ -> PrintDirectTicket tự dùng form chữ ký.
        public void PrintCombine(string combineId)
        {
            var members = panelTicketsTouch.Content.Controls.OfType<UCSaleItem>()
                .Where(c => c.combine_id == combineId).ToList();
            if (members.Count == 0)
            {
                CustomMessageBox.Show("No combined orders found to print !!!");
                return;
            }

            double cash = members.Sum(c => c.cash_amount);
            double charge = members.Sum(c => c.charge_amount);

            string json = Models.MainReport.CombineReceipt_PrinterData(combineId, cash, charge, 0);
            if (string.IsNullOrEmpty(json) || json.ToUpper().StartsWith("ERROR"))
            {
                CustomMessageBox.Show("Print combine error: " + Environment.NewLine + json);
                return;
            }

            Models.Helper.PrinterLocalHelper.PrintDirectTicket("COMBINE", json);
        }

        private async void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            // Lấy ids bằng LINQ — không dùng string += trong loop
            string ids = string.Join(",",
                panelTicketsTouch.Content.Controls
                    .OfType<UCSaleItem>()
                    .Where(c => c.selected)
                    .Select(c => c.id));

            if (string.IsNullOrEmpty(ids))
            {
                CustomMessageBox.Show("Please Select Order !!!");
                return;
            }

            btnDeleteSelected.Enabled = false;
            btnDeleteSelected.Text = "WAITING ...";

            string DATA = @"{
                                'orderIds': '" + ids + @"',
                                'localDate': '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + @"',
                                'comment': 'DELETE MULTI'
                            }";

            // Gọi API trên background thread — không block UI
            string responce = await Task.Run(() => Utilitys.CALL_API("Order/deleteMulti", DATA, "POST", true));
            if (this.IsDisposed) return;

            btnDeleteSelected.Enabled = true;
            btnDeleteSelected.Text = "DELETE SELECTED";

            if (responce.ToUpper().StartsWith("ERROR"))
            {
                CustomMessageBox.Show("Process Order Error: " + Environment.NewLine + responce);
                return;
            }

            this.SendSearch();
        }

        private void FormSaleList_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.parentForm.Show_Tab_Default();

            //Xử lý không bị cảm giác giật do xài Dispose() ngay nếu đóng thẳng
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000); // 3 giây

                try
                {
                    Core.ClearAndDisposeV2(panelTicketsTouch.Content);
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
