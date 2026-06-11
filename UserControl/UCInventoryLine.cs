using System;
using System.Drawing.Printing;
using System.Net;
using System.Windows.Forms;
using NailsChekin.Models;
using NailsChekin.Popup;

namespace NailsChekin.UserControl
{
    public partial class UCInventoryLine : DevExpress.XtraEditors.XtraUserControl
    {
        public string id = "";
        public bool enable_update_qty = false;

        public UCInventoryLine()
        {
            InitializeComponent();
        }

        public UCInventoryLine(string id, string barcode, string name, string catalog, string price, string qty)
        {
            InitializeComponent();

            lbID.Text = id;
            lbBarcode.Text = barcode;
            lbName.Text = name;
            lbCatalog.Text = catalog;         
            lbPrice.Text = "$" + price;
            txtQty.Text = qty;
           
            this.id = id;
        }

        private void txtQty_Click(object sender, EventArgs e)
        {
            txtQty.ReadOnly = false;
            if (txtQty.Text.Trim().Equals("0"))
            {
                txtQty.Text = "";
            }
            else
            {
                double inventory = -1;
                if (!double.TryParse(txtQty.Text, out inventory))
                {
                    txtQty.Text = "";
                }
            }

            enable_update_qty = true;
        }

        private void txtQty_MouseLeave(object sender, EventArgs e)
        {
            txtQty.ReadOnly = true;
            enable_update_qty = false;
        }

        private void txtQty_TextChanged(object sender, EventArgs e)
        {
            if (enable_update_qty)
            {
                //Update Qty
                string qty = txtQty.Text.Trim();
                if (!string.IsNullOrEmpty(qty))
                {
                    double inventory = -1;
                    if (double.TryParse(qty, out inventory))
                    {
                        string DATA = @"{
                              'id': " + this.id + @",
                              'quantity': '" + qty + @"'
                            }";

                        string responce = Utilitys.CALL_API("Product/updateInventory", DATA, "POST", true);
                        //if (responce.StartsWith("Error"))
                        //{
                        //    MessageBox.Show(responce);
                        //    return;
                        //}
                    }
                }
            }

        }

        private void svgEdit_Click(object sender, EventArgs e)
        {
            FormNewItem frm = new FormNewItem(this, this.id);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog(this);
            frm.Dispose();
        }

        public void SendSearchItemLockUp()
        {
            //((FormInventoryAdjust)this.Parent.Parent.Parent.Parent).SendSearchItemLockUp("", true);

            ((FormInventoryAdjust)this.Parent.Parent.Parent.Parent).ResetSendSearchItemLockUp(lbBarcode.Text, true);
        }

    }

}
