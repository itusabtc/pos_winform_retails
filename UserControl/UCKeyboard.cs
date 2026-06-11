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
using System.Text.RegularExpressions;

namespace NailsChekin.UserControl
{
    public partial class UCKeyboard : DevExpress.XtraEditors.XtraUserControl
    {
        Control parentForm;

        string redirect_url = "";
        string control_id = "";
        string current_text = "";

        bool show_paynow = false;
        bool is_item_lockup = false;

        public UCKeyboard()
        {
            InitializeComponent();
        }

        public UCKeyboard(Control parent, string control_id, string current_text = "", string redirect_url = "")
        {
            InitializeComponent();

            this.parentForm = parent;
            this.current_text = current_text;
            this.control_id = control_id;
            this.redirect_url = redirect_url;

            btnNumber0.Click += keyboard_Click;
            btnNumber1.Click += keyboard_Click;
            btnNumber2.Click += keyboard_Click;
            btnNumber3.Click += keyboard_Click;
            btnNumber4.Click += keyboard_Click;
            btnNumber5.Click += keyboard_Click;
            btnNumber6.Click += keyboard_Click;
            btnNumber7.Click += keyboard_Click;
            btnNumber8.Click += keyboard_Click;
            btnNumber9.Click += keyboard_Click;
            btnNumberX.Click += keyboard_Click;
            btnNumberDot.Click += keyboard_Click;
            btnNumberClear.Click += keyboard_Click;
            btnNumberC.Click += keyboard_Click;
        }

        void keyboard_Click(object sender, EventArgs e)
        {
            SimpleButton button = sender as SimpleButton;
           
            switch (button.Text)
            {
                case "0":
                    current_text += button.Text;
                    break;
                case "1":
                    current_text += button.Text;
                    break;
                case "2":
                    current_text += button.Text;
                    break;
                case "3":
                    current_text += button.Text;
                    break;
                case "4":
                    current_text += button.Text;
                    break;
                case "5":
                    current_text += button.Text;
                    break;
                case "6":
                    current_text += button.Text;
                    break;
                case "7":
                    current_text += button.Text;
                    break;
                case "8":
                    current_text += button.Text;
                    break;
                case "9":
                    current_text += button.Text;
                    break;
                case ".":
                    current_text += button.Text;
                    break;
                case "*":
                    current_text += button.Text;
                    break;
                case "C":
                    current_text = "";
                    break;
                case "<<":
                    current_text = current_text.Length > 1 ? (current_text.Substring(0, current_text.Length - 1)) : "";
                    break;
            }

            txtBarcode.Text = current_text;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            //this.SearchItemsByBarcode(this.current_text);

            //Quick Payment , not search !!!!
            this.AddQuickPaymentItems(this.current_text);

        }

        private void panelPayNow_MouseClick(object sender, MouseEventArgs e)
        {
            //this.Visible = false;
            //if (parentForm is FormMain)
            //{
            //    ((FormMain)parentForm).PaymentCart_ShowPayNow();
            //}
        }

        private void lbPayNow_Click(object sender, EventArgs e)
        {
            //if (lbPayNow.Text.Equals("Add To Cart"))
            //{
            //    ((FormMain)parentForm).AddLockupItemToCard();
            //}
            //else if (parentForm is FormMain)
            //{
            //    ((FormMain)parentForm).PaymentCart_ShowPayNow();
            //}
        }

        public void SetFocus()
        {
            txtBarcode.Text = "";
            txtBarcode.Enabled = true;
            txtBarcode.Focus();
        }

        public void ShowScanBarcode()
        {
            txtBarcode.Visible = true;
            txtBarcode.Enabled = true;
            txtBarcode.Focus();

            this.current_text = "";
            txtBarcode.Text = "";
        }

        public void HideScanBarcode()
        {
            //txtBarcode.Visible = false;
            //txtBarcode.Enabled = false;

            this.current_text = "";
            txtBarcode.Text = "";
        }

        public void ShowPayNow(bool show_paynow, bool is_item_lockup)
        {
            this.Height = 520;

            txtBarcode.Visible = true;
            panelPayNow_Keyboard.Visible = true;

            this.show_paynow = show_paynow;
            panelPayNow_QuickPayment.Visible = this.show_paynow;
            panelPayNow_QuickPayment.Height = 90;
            panelPayNow_QuickPayment.Dock = DockStyle.Bottom;

            this.is_item_lockup = is_item_lockup;

            if (this.show_paynow)
                lbPayNow.Text = "Pay Now";
            else if (is_item_lockup)
                lbPayNow.Text = "Add To Cart";
        }

        public void ShowAddToCart()
        {
            this.Height = 90;

            txtBarcode.Visible = false;
            panelPayNow_Keyboard.Visible = false;

            panelPayNow_QuickPayment.Visible = true;
            lbPayNow.Text = "Add To Cart";
            
            this.Dock = DockStyle.Bottom;
            panelPayNow_QuickPayment.Dock = DockStyle.Fill;
        }

        public void ResetDefault()
        {
            this.Height = 520;
            this.Dock = DockStyle.Bottom;

            txtBarcode.Visible = true; txtBarcode.Focus();
            panelPayNow_Keyboard.Visible = true;
            panelPayNow_Keyboard.Height = 375;
            panelPayNow_Keyboard.Location = new Point(44, 44);

            panelPayNow_QuickPayment.Visible = this.show_paynow;
            lbPayNow.Text = "Pay Now";
            panelPayNow_QuickPayment.Height = 90;
            panelPayNow_QuickPayment.Dock = DockStyle.Bottom;

            this.current_text = "";
            txtBarcode.Text = "";
        }

        private void txtBarcode_TextChanged(object sender, EventArgs e)
        {
            //if (txtBarcode.Text.StartsWith("Scan Barcode"))
            //    return;

            //if (txtBarcode.Text.Trim().Length >= 4)
            //{
            //    this.SearchItemsByBarcode(txtBarcode.Text);
            //}
        }

        public void SearchItemsByBarcode(string search, bool is_search_barcode = false)
        {
            string responce = null;
            if (is_search_barcode)
            {
                responce = Utilitys.CALL_API("Product/search-barcode?barcode=" + Regex.Replace(search, "\r", ""), "", "GET", true);
            }
            else
            {
                //Search Item
                string DATA = @"{
                                'category_id':null, 
                                'category_slugname':null, 
                                'searchString':'" + Regex.Replace(search, "\r", "") + @"',
                                'pageIndex':0, 
                                'pageSize':50
                            }";

                responce = Utilitys.CALL_API("Product/search", DATA, "POST", true);
            }

            if (parentForm is FormMain)
            {
                if (responce.StartsWith("Error"))
                {
                    MessageBox.Show(responce);
                    return;
                }

                //((FormMain)parentForm).AddScanItemToCard(responce, Regex.Replace(search, "\r", ""), is_search_barcode);
                //this.current_text = "";             
            }
        }

        private void AddQuickPaymentItems(string quickSyntax)
        {
            //if(string.IsNullOrEmpty(quickSyntax))
            //{
            //    MessageBox.Show("Erorr: Please check quick payment item");
            //    return;
            //}

            //if(!quickSyntax.Contains("*"))
            //{
            //    string quantity = "1";
            //    string price = quickSyntax;

            //    ((FormMain)parentForm).AddQucikItemToCard(quantity, price, this.current_text);
            //}
            //else
            //{
            //    int index = quickSyntax.IndexOf("*");
            //    string quantity = quickSyntax.Substring(0, index);
            //    string price = quickSyntax.Substring(index + 1, quickSyntax.Length - (index + 1));

            //    ((FormMain)parentForm).AddQucikItemToCard(quantity, price, this.current_text);
            //}

            this.current_text = "";
            txtBarcode.Text = "";
            txtBarcode.Enabled = true;
            txtBarcode.Focus();
        }

    }

}
