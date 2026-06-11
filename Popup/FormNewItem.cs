using DevExpress.Utils.Menu;
using NailsChekin.Models;
using NailsChekin.MyControls;
using NailsChekin.UserControl;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NailsChekin.Popup
{
    public partial class FormNewItem : Form
    {
        FormMain parentForm = null;
        string barcode = "";

        UCInventoryLine parentInventory = null;
        string item_id = "";

        public FormNewItem()
        {
            InitializeComponent();
        }

        public FormNewItem(FormMain parent, string barcode, bool is_search_sku)
        {
            InitializeComponent();

            this.parentForm = parent;
            this.barcode = barcode;

            if (!is_search_sku)
                txtBarcode.Text = this.barcode;
            else
                txtSKU.Text = this.barcode;

            if (barcode.Trim().Length > 0)
                txtName.Focus();
        }

        public FormNewItem(UCInventoryLine parent, string item_id)
        {
            InitializeComponent();

            this.parentInventory = parent;
            this.item_id = item_id;

            txtSKU.Focus();
        }

        private void FormNewItem_Load(object sender, EventArgs e)
        {
            //this.GetCatalog();

            if (!string.IsNullOrEmpty(item_id))
                this.LoadProductInfo();
        }

        private void LoadProductInfo()
        {
            string responce = Utilitys.CALL_API("Product/search-id?item_id=" + item_id, "", "GET", true);
            if (responce.StartsWith("Error"))
            {
                MessageBox.Show(responce);
                return;
            }

            foreach (JObject obj in JArray.Parse(responce))
            {
                string code = obj["code"].ToString();
                string barcode = obj["barcode"].ToString();
                string name = obj["name"].ToString();
                string category_id = obj["category_id"].ToString();
                string category_name = obj["category_Name"].ToString();
                string sub_category_id = obj["subCategory_Id"] == null ? "" : obj["subCategory_Id"].ToString();
                string sub_category_name = obj["subCategory_Name"] == null ? "" : obj["subCategory_Name"].ToString();
                string price = obj["price"].ToString();
                string cost = obj["cost"].ToString();
                string quantity = obj["quantity"].ToString();

                string profit = obj["profit"].ToString();
                string bonus = obj["bonus"].ToString();

                txtSKU.Text = code;
                txtBarcode.Text = barcode;
                txtName.Text = name;
                txtQty.Text = quantity;
                txtPrice.Text = price;
                txtCost.Text = cost;
                txtProfit.Text = profit;
                txtBonus.Text = bonus;

                txtCatalog.Text = category_name;
                this.catalog_selected_id = category_id;

                txtSubCatalog.Text = sub_category_name;
                this.sub_catalog_selected_id = sub_category_id;

                //ddCatalog.Text = category_name;
                //this.catalogId_selected = category_id;
            }
        }

        //private void GetCatalog()
        //{
        //    string responce = Utilitys.CALL_API("Catalog", "", "GET", true);
        //    if (responce.StartsWith("Error"))
        //    {
        //        MessageBox.Show(responce);
        //        return;
        //    }

        //    DXPopupMenu popupMenu = new DXPopupMenu();
        //    foreach (JObject obj in JArray.Parse(responce))
        //    {
        //        string id = obj["id"].ToString();
        //        string name = obj["name"].ToString();

        //        popupMenu.Items.Add(new DXMenuItem() { Caption = name, Tag = id });
        //    }

        //    ddCatalog.DropDownControl = popupMenu;

        //    foreach (DXMenuItem item in popupMenu.Items)
        //        item.Click += item_Click;
        //}

        private void item_Click(object sender, EventArgs e)
        {
            //ddCatalog.Text = ((DXMenuItem)sender).Caption;
            //this.catalogId_selected = ((DXMenuItem)sender).Tag.ToString();
        }

        private void btnConfirm_NO_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void btnConfirm_YES_Click(object sender, EventArgs e)
        {
            string id = string.IsNullOrEmpty(this.item_id) ? "0" : this.item_id;
            string code = txtSKU.Text.Trim();
            string barcode = txtBarcode.Text.Trim();
            string name = txtName.Text.Trim();
            string qty = string.IsNullOrEmpty(txtQty.Text) ? "0" : txtQty.Text.Trim();
            string cost = string.IsNullOrEmpty(txtCost.Text) ? "0" : txtCost.Text.Trim();
            string price = string.IsNullOrEmpty(txtPrice.Text) ? "0" : txtPrice.Text.Trim();
            string profit = string.IsNullOrEmpty(txtProfit.Text) ? "0" : txtProfit.Text.Trim();
            string bonus = string.IsNullOrEmpty(txtBonus.Text) ? "0" : txtBonus.Text.Trim();

            if (string.IsNullOrEmpty(barcode) && string.IsNullOrEmpty(code))
            {
                CustomMessageBox.Show("Please enter SKU / Barcode !!");
                return;
            }

            if (name.Length <= 0)
            {
                CustomMessageBox.Show("Please enter name !!");
                return;
            }

            if (this.catalog_selected_id.Length <= 0)
            {
                CustomMessageBox.Show("Please select catalog !!");
                return;
            }

            string DATA = @"{
                              'id': " + id + @",
                              'name': '" + name + @"',
                              'code': '" + code + @"',
                              'barcode': '" + barcode + @"',
                              'catalog': '" + this.catalog_selected_id + @"',
                              'subCatalog': '" + this.sub_catalog_selected_id + @"',
                              'quantity': '" + qty + @"',
                              'cost': '" + cost + @"',
                              'price': '" + price + @"',
                              'profit': '" + profit + @"',
                              'bonus': '" + bonus + @"',
                              'mainImage': ''
                            }";

            string responce = Utilitys.CALL_API("Product/addProduct", DATA, "POST", true);
            if (responce.StartsWith("Error"))
            {
                CustomMessageBox.Show(responce);
                return;
            }

            //if (parentForm != null)
            //    parentForm.AddNewItemToCard(responce, barcode);
            //else if (parentInventory != null)
            //    parentInventory.SendSearchItemLockUp();

            this.Dispose();
        }

        private void FormNewItem_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Xử lý không bị cảm giác giật do xài Dispose() ngay nếu đóng thẳng
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000); // 3 giây

                try
                {
                    Core.ClearMemory();

                    if (!this.IsDisposed)
                    {
                        this.Invoke((Action)(() => this.Dispose()));
                    }
                }
                catch { /* form đã dispose rồi thì thôi */ }
            });
        }

        private void ddCatalog_Click(object sender, EventArgs e)
        {
            //ddCatalog.ShowDropDown();            
        }

        private void txtCatalog_Click(object sender, EventArgs e)
        {
            FormSelectCatalog frm = new FormSelectCatalog(this);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.WindowState = FormWindowState.Maximized;
            frm.ShowDialog(this);
            frm.Dispose();
        }

        public string catalog_selected_id = "";
        public string catalog_selected_name = "";
        public void SetCatalogSelected(string id, string name)
        {
            this.catalog_selected_id = id;
            this.catalog_selected_name = name;

            txtCatalog.Text = name;

            if (!string.IsNullOrEmpty(id))
            {
                FormSelectSubCatalog frm = new FormSelectSubCatalog(this, id, true);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.WindowState = FormWindowState.Maximized;
                frm.ShowDialog(this);
                frm.Dispose();
            }
        }

        public string sub_catalog_selected_id = "";
        public string sub_catalog_selected_name = "";
        public void SetSubCatalogSelected(string id, string name)
        {
            this.sub_catalog_selected_id = id;
            this.sub_catalog_selected_name = name;

            txtSubCatalog.Text = name;
            txtQty.Focus();
        }

        private void txtSubCatalog_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(catalog_selected_id))
            {
                CustomMessageBox.Show("Error: Please select catalog !!!");
                return;
            }

            FormSelectSubCatalog frm = new FormSelectSubCatalog(this, catalog_selected_id);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.WindowState = FormWindowState.Maximized;
            frm.ShowDialog(this);
            frm.Dispose();
        }

    }

}
