using System;
using System.Drawing;
using System.Windows.Forms;
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
        public string combine_id = "";   // nhóm combine open sale; "" = đơn lẻ
        public double cash_amount = 0;    // dùng để cộng tổng khi in lại receipt combine
        public double charge_amount = 0;

        public UCSaleItem()
        {
            InitializeComponent();
        }

        public UCSaleItem(string id, string date, string name, string phone, string products, string amount, string cash, string charge, string status, string orderStatusString)
        {
            InitializeComponent();

            lbID.Text = id;
            lbDate.Text = DateTime.TryParse(date, out DateTime dt) ? dt.ToString("MM/dd/yyyy h:mm tt") : date;
            lbCustomer.Text = name + (string.IsNullOrEmpty(phone) ? "" : (Environment.NewLine + phone));
            lbProduct.Text = products;
            lbAmount.Text = "$" + amount;
            lbCash.Text = "$" + cash;
            lbCharge.Text = "$" + charge;
            lbStatus.Text = orderStatusString;
            if (!this.status.Equals("0") && (orderStatusString.Equals("Canceled") || orderStatusString.Equals("Returned")))
                lbStatus.ForeColor = System.Drawing.Color.Red;

            this.id = id;
            this.status = status;
            this.phone = phone;
            this.amount = amount;
            this.cash_amount = Utilitys.getTotalAmount(cash);
            this.charge_amount = Utilitys.getTotalAmount(charge);

            Models.Helper.UIHelper.StyleSelectCheckbox(chkSelected);

            if (!this.status.Equals("0"))   // màn hình lịch sử (history)
            {
                svgDelete.Visible = false;

                // svgPaymentNow tái sử dụng làm nút Refund: chỉ hiện cho đơn đã thanh toán bằng THẺ (charge > 0)
                bool showRefund = orderStatusString.Equals("Paid") && Utilitys.getTotalAmount(charge) > 0;
                if (showRefund)
                {
                    svgPaymentNow.SvgImage = GetRefundIcon();
                    svgPaymentNow.Tag = "REFUND";
                    svgPaymentNow.Visible = true;
                }
                else
                {
                    svgPaymentNow.Visible = false;
                }
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

        // Đánh dấu đơn thuộc combine (tô màu nền). Thanh toán sẽ theo nhóm, không trả lẻ.
        public void SetCombineId(string combineId)
        {
            this.combine_id = combineId ?? "";
            // Tô màu cho cả màn hình SALE OPEN lẫn SALE LIST (lịch sử) để nhận biết đơn thuộc combine
            if (!string.IsNullOrEmpty(this.combine_id))
                tbHeader.BackColor = System.Drawing.Color.FromArgb(255, 244, 214); // vàng nhạt = thuộc combine
        }

        private void svgPaymentNow_Click(object sender, EventArgs e)
        {
            // Màn hình lịch sử: svgPaymentNow đóng vai trò nút Refund (Tag = "REFUND")
            if ("REFUND".Equals(svgPaymentNow.Tag as string))
            {
                OpenRefund();
                return;
            }

            // Màn hình SALE OPEN
            if (!this.status.Equals("0"))
            {
                MessageBox.Show("Order Status Incorrect !!!");
                return;
            }

            var formSale = Models.Helper.UIHelper.GetParentForm<FormSaleList>(this);
            if (!string.IsNullOrEmpty(this.combine_id))
                formSale?.PayCombine(this.combine_id);   // đơn trong combine -> trả theo nhóm
            else
                formSale?.SaleOpen_Payment(this.id);     // đơn lẻ
        }

        private void svgPrinter_Click(object sender, EventArgs e)
        {
            // Đơn thuộc combine -> hỏi in receipt tổng (combine) hay receipt lẻ của riêng đơn này
            if (!string.IsNullOrEmpty(this.combine_id))
            {
                var ans = CustomMessageBox.Show(
                    "This order belongs to a combined ticket." + Environment.NewLine + Environment.NewLine +
                    "[Yes]  = Print the COMBINED receipt" + Environment.NewLine +
                    "[No]   = Print only THIS receipt",
                    "Print Combine", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (ans == DialogResult.Cancel) return;
                if (ans == DialogResult.Yes)
                {
                    Models.Helper.UIHelper.GetParentForm<FormSaleList>(this)?.PrintCombine(this.combine_id);
                    return;
                }
                // ans == No -> in receipt lẻ bình thường (rơi xuống dưới)
            }

            // Trạng thái (Returned/Paid...) do SQL ftTikectPrinter trả về theo orderStatus
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
            FormComment frm = new FormComment(this, "", "Delete Commnet", this.save_note, amount, "delete_order");
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

            Models.Helper.UIHelper.GetParentForm<FormSaleList>(this)?.SendSearch(true);
        }

        // Icon Refund (SVG) dùng chung cho mọi dòng -> parse 1 lần
        private static DevExpress.Utils.Svg.SvgImage _refundIcon;
        private static DevExpress.Utils.Svg.SvgImage GetRefundIcon()
        {
            if (_refundIcon == null)
            {
                string svg = "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24'>"
                    + "<path fill='#039C23' d='M12.5 8c-2.65 0-5.05.99-6.9 2.6L2 7v9h9l-3.62-3.62c1.39-1.16 3.16-1.88 5.12-1.88 3.54 0 6.55 2.31 7.6 5.5l2.37-.78C21.08 11.03 17.15 8 12.5 8z'/>"
                    + "</svg>";
                using (var ms = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(svg)))
                    _refundIcon = DevExpress.Utils.Svg.SvgImage.FromStream(ms);
            }
            return _refundIcon;
        }

        private void OpenRefund() 
        {
            FormRefund frm = new FormRefund(this, this.id);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog(this);
            bool didRefund = frm.refunded;
            frm.Dispose();

            // Reload lại list để cập nhật trạng thái sau khi refund
            if (didRefund)
                Models.Helper.UIHelper.GetParentForm<FormSaleList>(this)?.SendSearch();
        }

        private void svgRefund_Click(object sender, EventArgs e)
        {
            FormRefund frm = new FormRefund(this, this.id);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog(this);
            bool didRefund = frm.refunded;
            frm.Dispose();

            // Reload lại list để cập nhật trạng thái sau khi refund
            if (didRefund)
                Models.Helper.UIHelper.GetParentForm<FormSaleList>(this)?.SendSearch();
        }

        private void chkSelected_CheckedChanged(object sender, EventArgs e)
        {
            this.selected = chkSelected.Checked;
            Models.Helper.UIHelper.UpdateSelectCheckboxGlyph(chkSelected);
            Models.Helper.UIHelper.GetParentForm<FormSaleList>(this)?.UpdateDeleteButtonVisibility();
        }

        public void SetSelected(bool selected)
        {
            this.selected = selected;
            chkSelected.Checked = selected;
        }

    }

}
