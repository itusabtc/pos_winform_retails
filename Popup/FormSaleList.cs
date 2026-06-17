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
            }
            else
            {
                txtFromDate.Text = DateTime.Now.ToString("MM-dd-yyyy");
                txtToDate.Text = DateTime.Now.ToString("MM-dd-yyyy");
            }

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
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
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
                        string orderStatus, string orderStatusString, string orderSource, string paymentStatus)>();
                    foreach (JObject obj in JArray.Parse(responce))
                        list.Add((
                            obj["orderId"].ToString(),     obj["orderDate"].ToString(),
                            obj["name"].ToString(),        obj["phone"].ToString(),
                            obj["products"].ToString(),    obj["subtotal"].ToString(),
                            obj["cash"].ToString(),        obj["charge"].ToString(),
                            obj["orderStatus"].ToString(), obj["orderStatusString"].ToString(),
                            obj["order_source"].ToString(), obj["payment_status"].ToString()
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
                        ctrl.SetOrderUnpaid(it.orderStatus, it.paymentStatus, it.orderSource);
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

            this.parentForm.AddSaleItemToCard(orderId, items, paidAmount, orderSource);
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
            btnDeleteSelected.Visible = isChecked;
        }

        public void UpdateDeleteButtonVisibility()
        {
            bool anySelected = panelTicketsTouch.Content.Controls
                .OfType<UCSaleItem>()
                .Any(c => c.selected);
            btnDeleteSelected.Visible = anySelected;
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
