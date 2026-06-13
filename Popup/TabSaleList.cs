using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using NailsChekin.Models.Helper;
using DevExpress.Utils.Menu;
using Newtonsoft.Json.Linq;
using NailsChekin.Models;
using NailsChekin.UserControl;
using System.Text.RegularExpressions;
using System.Threading;

namespace NailsChekin.Popup
{
    public partial class TabSaleList : DevExpress.XtraEditors.XtraUserControl
    {
        FormMain parentForm = null;
        public string status = "";

        public TabSaleList()
        {
            InitializeComponent();
        }

        public TabSaleList(FormMain frm, string status)
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();

            this.BackColor = ColorHelper.DefaultBackgoundColor;
            this.parentForm = frm;

            //Init Data
            DXPopupMenu popupPaidBy = new DXPopupMenu();
            popupPaidBy.Items.Add(new DXMenuItem() { Caption = "ALL" });
            popupPaidBy.Items.Add(new DXMenuItem() { Caption = "CASH" });
            popupPaidBy.Items.Add(new DXMenuItem() { Caption = "CREDIT" });
            ddlPaidBySearch.DropDownControl = popupPaidBy;
            foreach (DXMenuItem item in popupPaidBy.Items)
                item.Click += item_paidby_Click;

            if (this.status.Equals("0"))
            {
                txtFromDate.Text = DateTime.Now.ToString("MM/01/yyyy");
                txtToDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
            }
            else
            {
                txtFromDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
                txtToDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
            }
        }

        private void TabSaleList_Load(object sender, EventArgs e)
        {
            this.Adjust_Screen();
           
        }

        private void item_paidby_Click(object sender, EventArgs e)
        {
            ddlPaidBySearch.Text = ((DXMenuItem)sender).Caption;
        }

        private void Adjust_Screen()
        {
            
        }

        private void panelTicketsTouch_SizeChanged(object sender, EventArgs e)
        {
            foreach (Control c in this.panelTicketsTouch.Content.Controls)
            {
                c.Width = this.panelTicketsTouch.Width - 5;
            }
        }

        public void SendSearch(bool reload_update = false)
        {
            // Clear() không dispose control cũ => leak handle sau mỗi lần search
            var oldControls = panelTicketsTouch.Content.Controls.OfType<UCSaleItem>().ToList();
            panelTicketsTouch.Content.Controls.Clear();
            foreach (var old in oldControls) old.Dispose();

            //Search Item
            string DATA = @"{
                            'daysAgo': " + (this.status.Equals("0") ? "31" : "0") + @", 
                            'orderStatus':'" + this.status + @"',
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

            string responce = Utilitys.CALL_API("Order/ordersHistory", DATA, "POST", true);
            if (responce.StartsWith("Error"))
            {
                MessageBox.Show(responce);
                return;
            }

            JArray jArray = JArray.Parse(responce);
            int locationY = 42;
            foreach (JObject obj in jArray)
            {
                string orderId = obj["orderId"].ToString();
                string orderDate = obj["orderDate"].ToString();
                string name = obj["name"].ToString();
                string phone = obj["phone"].ToString();
                string products = obj["products"].ToString();
                string amount = obj["subtotal"].ToString();
                string cash = obj["cash"].ToString();
                string charge = obj["charge"].ToString();
                string orderStatus = obj["orderStatus"].ToString();
                string orderStatusString = obj["orderStatusString"].ToString();
                string orderSource = obj["order_source"].ToString();
                string paymentStatus = obj["payment_status"].ToString();

                UCSaleItem control = new NailsChekin.UserControl.UCSaleItem(orderId, orderDate, name, phone, products, amount, cash, charge, this.status, orderStatusString);
                if (this.status.Equals("0"))
                    control.SetOrderUnpaid(orderStatus, paymentStatus, orderSource);

                control.Width = panelTicketsTouch.Width - 15;
                control.Location = new Point(0, locationY); locationY += control.Height + 2;
                panelTicketsTouch.Content.Controls.Add(control);
            }
        }


        public async Task LoadDataAsync(CancellationToken ct)
        {
            this.SendSearch();
        }

        private readonly SemaphoreSlim _gate = new SemaphoreSlim(1, 1); // chống double-click
        public async Task EnsureLoadedAsync(CancellationToken ct)
        {
            await _gate.WaitAsync(ct);
            try
            {
                if (ct.IsCancellationRequested) return;

                // GỌI HÀM VẼ UI (hàm này đang thêm control trực tiếp vào this.Controls)
                await LoadDataAsync(ct);
            }
            finally
            {
                _gate.Release();
            }
        }

    }
}
