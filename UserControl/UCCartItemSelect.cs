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
using NailsChekin.Models;

namespace NailsChekin.UserControl
{
    public partial class UCCartItemSelect : DevExpress.XtraEditors.XtraUserControl
    {
        public string cart_order_id = "";
        public string cart_item_id = "";

        public string item_id = "";
        public string item_name = "";

        public string quantity = "1";
        public string price = "0";
        public string discount = "";

        public bool selected = false;

        public Control parent = null;

        public UCCartItemSelect()
        {
            InitializeComponent();
        }

        public UCCartItemSelect(Control parent, string item_id, string item_name, string price, string quantity = "1", string discount = "0")
        {
            InitializeComponent();

            this.parent = parent;
            this.cart_item_id = Utilitys.createRamdomKey();
            this.item_id = item_id;
            this.item_name = item_name;
            this.price = price;
            this.quantity = quantity;
            this.discount = discount;

            txtProductName.Text = item_name;
            lbQty.Text = quantity;
            txtPrice.Text = "$" + price;
            lbTotal.Text = "$" + Math.Round((double.Parse(price) * double.Parse(quantity)), 2).ToString();

        }

        private void svgSelect_Click(object sender, EventArgs e)
        {
            if (this.selected)
                this.selected = false;
            else
                this.selected = true;

            this.SetColor();
        }

        private void SetColor()
        {
            if (this.selected)
                this.tbLayout.BackColor = System.Drawing.Color.Orange;
            else
                this.tbLayout.BackColor = System.Drawing.Color.FloralWhite;
        }

    }
}
