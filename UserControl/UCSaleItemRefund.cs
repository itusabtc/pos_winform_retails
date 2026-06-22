using System;
using System.Drawing;
using System.Windows.Forms;

namespace NailsChekin.UserControl
{
    public partial class UCSaleItemRefund : DevExpress.XtraEditors.XtraUserControl
    {
        public string detail_id = "";   // pk_seq của dòng (vw_AllOrderItems.detail_id) -> refund đúng dòng
        public string item_id = "";
        public string item_name = "";

        public double quantity = 1;
        public double price = 0;
        public double discount = 0;

        // Số tiền refund cho dòng này (line subtotal lấy từ API)
        public double amount = 0;

        // Item đã được refund trước đó -> không cho chọn lại
        public bool already_refunded = false;

        public bool selected = false;

        public Control parent = null;

        public UCSaleItemRefund()
        {
            InitializeComponent();
        }

        public UCSaleItemRefund(Control parent, string detail_id, string item_id, string item_name,
            double price, double quantity, double discount, double amount, bool alreadyRefunded)
        {
            InitializeComponent();

            this.parent = parent;
            this.detail_id = detail_id;
            this.item_id = item_id;
            this.item_name = item_name;
            this.price = price;
            this.quantity = quantity;
            this.discount = discount;
            this.amount = amount;
            this.already_refunded = alreadyRefunded;

            lbName.Text = item_name;
            lbQty.Text = quantity.ToString("0.##");
            lbPrice.Text = "$" + price.ToString("0.00");
            lbDiscount.Text = "$" + discount.ToString("0.00");
            lbTotal.Text = "$" + amount.ToString("0.00");

            // Checkbox chọn/không chọn kiểu nút (☐ xám khi chưa chọn, ✓ trắng nền xanh khi chọn)
            NailsChekin.Models.Helper.UIHelper.StyleSelectCheckbox(chkSelected);

            if (alreadyRefunded)
            {
                lbName.Text = item_name + "   (Refunded)";
                chkSelected.Enabled = false;
                tbLayout.BackColor = Color.Gainsboro;
                lbName.ForeColor = lbQty.ForeColor = lbPrice.ForeColor =
                    lbDiscount.ForeColor = lbTotal.ForeColor = Color.Gray;
            }
            else
            {
                // Cho phép bấm vào cả dòng để chọn (touch-friendly), không chỉ checkbox
                tbLayout.Click += Row_Click;
                lbName.Click += Row_Click;
                lbQty.Click += Row_Click;
                lbPrice.Click += Row_Click;
                lbDiscount.Click += Row_Click;
                lbTotal.Click += Row_Click;
            }
        }

        private void Row_Click(object sender, EventArgs e)
        {
            if (already_refunded) return;
            chkSelected.Checked = !chkSelected.Checked;
        }

        private void chkSelected_CheckedChanged(object sender, EventArgs e)
        {
            this.selected = chkSelected.Checked;
            NailsChekin.Models.Helper.UIHelper.UpdateSelectCheckboxGlyph(chkSelected);
            this.SetColor();
        }

        public void SetSelected(bool value)
        {
            if (already_refunded) return;
            chkSelected.Checked = value; // trigger CheckedChanged -> set selected + màu
        }

        private void SetColor()
        {
            if (already_refunded) return;
            this.tbLayout.BackColor = this.selected ? Color.WhiteSmoke : Color.FloralWhite;
        }
    }
}
