using System;
using System.Drawing;
using System.Windows.Forms;
using NailsChekin.Models;
using NailsChekin.Popup;
using NailsChekin.Models.Helper;

namespace NailsChekin.UserControl
{
    public partial class UCCartItem : DevExpress.XtraEditors.XtraUserControl
    {
        public string cart_order_id = "";
        public string cart_item_id = "";

        public string item_id = "";
        public string item_name = "";

        public string quantity = "1";
        public string price = "0";
        public string discount = "";

        public string isPromotion = "0";
        public string scheme = "";

        public Control parent = null;

        public UCCartItem()
        {
            InitializeComponent();
        }

        public UCCartItem(Control parent, string item_id, string item_name, string price, string quantity = "1", string discount = "0", string isPromotion = "0", string scheme = "")
        {
            InitializeComponent();

            this.parent = parent;
            this.cart_item_id = Utilitys.createRamdomKey();
            this.item_id = item_id;
            this.item_name = item_name;
            this.price = price;
            this.quantity = quantity;
            this.discount = discount;
            this.isPromotion = isPromotion;
            this.scheme = scheme;

            txtProductName.Text = item_name;
            lbQty.Text = quantity;
            txtPrice.Text = "$" + price;
            lbTotal.Text = "$" + Math.Round((double.Parse(price) * double.Parse(quantity)), 2).ToString();

            if (this.isPromotion.Equals("1"))
            {
                svgIncrease.Visible = false;
                svgDecrease.Visible = false;
                lbQty.Enabled = false;
                txtPrice.Enabled = false;
                lbTotal.Enabled = false;
                lbTotal.Text = "$" + price;

                tbLayout.BackColor = Color.Yellow;
            }
        }

        public void increaseQuantity(int change)
        {
            this.quantity = (int.Parse(this.quantity) + change).ToString();

            lbQty.Text = this.quantity;
            this.update_parent_cart_amount(this.isPromotion.Equals("1") ? false : true);
        }

        public void decreaseQuantity(int change)
        {
            if ((int.Parse(this.quantity) - change) >= 1)
                this.quantity = (int.Parse(this.quantity) - change).ToString();
            else
            {
                this.quantity = "0";
                lbQty.Text = this.quantity;

                if (this.parent is FormMain formMain)
                {
                    formMain.RemoveCartItem(this.item_id, this.cart_item_id);
                }
                else if (this.parent is FormAddQuickItem formAddQuickItem)
                {
                    formAddQuickItem.RemoveCartItem(this.item_id, this.cart_item_id);
                }
            }

            lbQty.Text = this.quantity;
            this.update_parent_cart_amount(this.isPromotion.Equals("1") ? false : true);
        }

        public double subTotal()
        {
            return Math.Round(double.Parse(quantity) * double.Parse(this.price), 2);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (this.parent is FormMain formMain)
            {
                formMain.RemoveCartItem(this.item_id, this.cart_item_id);
            }
            else if (this.parent is FormAddQuickItem formAddQuickItem)
            {
                formAddQuickItem.RemoveCartItem(this.item_id, this.cart_item_id);
            }
        }

        private void svgIncrease_Click(object sender, EventArgs e)
        {
            this.increaseQuantity(1);
        }

        private void svgDecrease_Click(object sender, EventArgs e)
        {
            this.decreaseQuantity(1);
        }

        private void txtPrice_TextChanged(object sender, EventArgs e)
        {
            this.price = Utilitys.getTotalAmount(txtPrice.Text).ToString();

            this.update_parent_cart_amount(false);
        }

        private void update_parent_cart_amount(bool check_promotion)
        {
            if (!string.IsNullOrEmpty(this.quantity) && !string.IsNullOrEmpty(this.price))
                lbTotal.Text = "$" + (double.Parse(price) * double.Parse(this.quantity)).ToString();
            else
                lbTotal.Text = "$0.00";

            try
            {
                if (this.parent is FormMain formMain)
                {
                    ((FormMain)parent).UpdatePaymentCartAmount();
                }
                else if (this.parent is FormAddQuickItem formQuickItem)
                {
                    ((FormAddQuickItem)parent).UpdatePaymentCartAmount();
                }

                //FormMain form_parent = UIHelper.GetParentForm<FormMain>(this);
                ////if( check_promotion )
                ////    form_parent.Check_Promotion(this.item_id, this.item_name, this.price, this.quantity);
                ////else
                //form_parent.UpdatePaymentCartAmount();
            }
            catch (Exception ex) { }
        }

        private void txtPrice_Click(object sender, EventArgs e)
        {
            FormKeyboardOnlyNumber frm = new FormKeyboardOnlyNumber(this, this.price, "txtPrice", "ChangePriceLocal");
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog(this);
            frm.Dispose();
        }

        public void UpdatePrice(string new_price)
        {
            this.price = new_price;
            txtPrice.Text = "$" + new_price;
            lbTotal.Text = "$" + Math.Round((double.Parse(price) * double.Parse(quantity)), 2).ToString();
        }

        private void lbQty_Click(object sender, EventArgs e)
        {
            FormKeyboardOnlyNumber frm = new FormKeyboardOnlyNumber(this, this.quantity, "lbQty", "ChangeQtyLocal");
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog(this);
            frm.Dispose();
        }

        public void UpdateQty(string new_qty, bool check_promotion = false)
        {
            this.quantity = new_qty;
            lbQty.Text = new_qty;

            // Cập nhật tổng DÒNG + tổng CART (SUBTOTAL/TAX/TOTAL).
            // Trước đây chỉ set lbTotal mà KHÔNG gọi UpdatePaymentCartAmount ->
            // sửa số lượng bằng cách gõ tay (vd 1 -> 10) thì tổng cart giữ nguyên
            // giá trị cũ -> Save Sale gửi subtotal sai -> receipt in sai tiền.
            // Đi qua update_parent_cart_amount giống increaseQuantity để đồng bộ.
            this.update_parent_cart_amount(this.isPromotion.Equals("1") ? false : true);
        }

        private void txtProductName_Click(object sender, EventArgs e)
        {
            //Show Keyboard
            FormItemLockup frm = new FormItemLockup(this, "ItemChangeName", txtProductName.Text);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog(this);
            frm.Dispose();
        }
        public void UpdateItemName(string new_name)
        {
            this.Name = new_name;
            txtProductName.Text = new_name;

            txtProductName.SelectionStart = txtProductName.TextLength;
            txtProductName.SelectionLength = 0;

            this.RemoveFocus();
        }

        public void UpdatePromotionAmount(string new_amount)
        {
            this.price = new_amount;
            txtPrice.Text = "$" + price;
            lbTotal.Text = "$" + price;
        }

        private void txtProductName_TextChanged(object sender, EventArgs e)
        {
            this.item_name = txtProductName.Text;
        }

        public void RemoveFocus()
        {
            var frm = this.FindForm();
            if (frm != null)
            {
                frm.BeginInvoke(new Action(() =>
                {
                    frm.ActiveControl = null;
                }));
            }
        }

        public void MyDispose()
        {
            try
            {
                this.btnRemove.Click -= this.btnRemove_Click;
                this.svgDecrease.Click -= this.svgDecrease_Click;
                this.svgIncrease.Click -= this.svgIncrease_Click;
                this.lbQty.Click -= this.lbQty_Click;
                this.txtPrice.TextChanged -= this.txtPrice_TextChanged;
                this.txtProductName.Click -= this.txtProductName_Click;
                this.txtProductName.TextChanged -= this.txtProductName_TextChanged;

                this.btnRemove.Dispose();
                this.svgDecrease.Dispose();
                this.svgIncrease.Dispose();
                this.lbQty.Dispose();
                this.txtProductName.Dispose();

                if (this != null && tbLayout != null)
                {
                    while (tbLayout.Controls.Count > 0)
                    {
                        tbLayout.Controls[0].Dispose();
                    }

                    tbLayout.Controls.Clear();
                    tbLayout.Dispose();
                    tbLayout = null;
                }

                GC.SuppressFinalize(this);
            }
            catch (Exception ex)
            {
                Models.Helper.LogHelper.SaveLOG_Crash("Message: " + ex.Message + "\nStack Trace:" + ex.StackTrace + "\nInnerException:" + (ex.InnerException?.Message ?? ""), "My Dispose Exception");
            }
        }

    }

}
