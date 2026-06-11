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
using NailsChekin.Popup;
using NailsChekin.Models;
using NailsChekin.MyControls;

namespace NailsChekin.UserControl
{
    public partial class UCSaleItem : DevExpress.XtraEditors.XtraUserControl
    {
        public string id = "";
        public string status = "";
        public string phone = "";
        public string amount = "0";
        public string order_source = "POS";
        public string order_status = "";
        public string payment_status = "";
        public bool selected = false;

        public UCSaleItem()
        {
            InitializeComponent();
        }

        public UCSaleItem(string id, string date, string name, string phone, string products, string amount, string cash, string charge, string status, string orderStatusString)
        {
            InitializeComponent();

            lbID.Text = id;
            lbDate.Text = DateTime.TryParse(date, out DateTime dt)
                ? dt.ToString("MM/dd/yyyy h:mm tt")
                : date;
            lbCustomer.Text = name + (string.IsNullOrEmpty(phone) ? "" : (Environment.NewLine + phone));
            lbProduct.Text = products;
            lbAmount.Text = "$" + amount;
            lbCash.Text = "$" + cash;
            lbCharge.Text = "$" + charge;
            lbStatus.Text = orderStatusString;
            if ( !this.status.Equals("0") && ( orderStatusString.Equals("Canceled") || orderStatusString.Equals("Returned")) )
                lbStatus.ForeColor = System.Drawing.Color.Red;

            this.id = id;
            this.status = status;
            this.phone = phone;
            this.amount = amount;

            chkSelected.AutoSize = false;
            chkSelected.Appearance = System.Windows.Forms.Appearance.Button;
            chkSelected.FlatStyle = FlatStyle.Flat;
            chkSelected.Text = "✓";
            chkSelected.Font = new Font("Segoe UI", 16f, FontStyle.Bold);
            chkSelected.TextAlign = ContentAlignment.MiddleCenter;
            chkSelected.FlatAppearance.CheckedBackColor = Color.FromArgb(41, 182, 246);
            chkSelected.FlatAppearance.BorderSize = 1;
            chkSelected.FlatAppearance.BorderColor = Color.LightGray;

            if (this.status.Equals("0"))
            {
                //lbID.ForeColor = System.Drawing.Color.Red;
                //lbCash.ForeColor = System.Drawing.Color.Red;
                //lbStatus.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                svgPaymentNow.Visible = false;
                svgDelete.Visible = false;
            }
        }

        public void SetOrderUnpaid(string orderStatus, string paymentStatus, string orderSource)
        {
            this.order_source = orderSource;
            this.order_status = orderStatus;
            this.payment_status = paymentStatus;

            if (status.Equals("0") && !order_source.Equals("POS"))
            {
                lbID.ForeColor = System.Drawing.Color.Red;
                lbCustomer.ForeColor = System.Drawing.Color.Red;
                lbDate.ForeColor = System.Drawing.Color.Red;
                lbProduct.ForeColor = System.Drawing.Color.Red;
                lbAmount.ForeColor = System.Drawing.Color.Red;
                lbCash.ForeColor = System.Drawing.Color.Red;
                lbStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void svgPaymentNow_Click(object sender, EventArgs e)
        {
            if (!this.status.Equals("0"))
            {
                MessageBox.Show("Order Status Incorrect !!!");
                return;
            }

            FormSaleList form_parent = Models.Helper.UIHelper.GetParentForm<FormSaleList>(this);
            form_parent.SaleOpen_Payment(this.id);

        }

        private void svgPrinter_Click(object sender, EventArgs e)
        {
            Models.Helper.PrinterLocalHelper.PrintDirectTicket(this.id, "");
        }

        private void svgEmailSms_Click(object sender, EventArgs e)
        {
            FormEmailSms frm = new FormEmailSms(this.id, this.phone);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog(this);
            frm.Dispose();
        }

        public string save_note = "";
        private void svgDelete_Click(object sender, EventArgs e)
        {
            FormComment frm = new FormComment(this, "", "Delete Commnet", this.save_note, amount,"delete_order");
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog(this);
            frm.Dispose();
        }

        public void DeleteOrder(string comment)
        {
            if (this.status.Equals("1"))
            {
                CustomMessageBox.Show("Order Status Not Correct !!!");
                return;
            }

            this.save_note = comment;
            string DATA = @"{
                                'orderId': " + this.id + @",
                                'localDate': '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + @"',
                                'comment': '" + comment + @"'
                            }";

            string responce = Utilitys.CALL_API("Order/delete", DATA, "POST", true);
            if (responce.ToUpper().StartsWith("ERROR")) //Fail
            {
                CustomMessageBox.Show("Process Order Error: " + Environment.NewLine + responce);
                this.Dispose();
                return;
            }

            Control form_parent = (FormSaleList)this.Parent.Parent.Parent.Parent;
            ((FormSaleList)form_parent).SendSearch(true);
        }

        private void chkSelected_CheckedChanged(object sender, EventArgs e)
        {
            this.selected = chkSelected.Checked;
            Models.Helper.UIHelper.GetParentForm<FormSaleList>(this)?.UpdateDeleteButtonVisibility();
        }

        public void SetSelected(bool selected)
        {
            this.selected = selected;
            chkSelected.Checked = selected;
        }

    }

}
