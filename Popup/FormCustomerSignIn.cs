using DevExpress.Utils.Menu;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NailsChekin.Models;
using NailsChekin;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using NailsChekin.UserControl;

namespace NailsChekin.Popup
{
    public partial class FormCustomerSignIn : Form
    {
        FormMain parentForm = null;
        UCCustomerSearchResult parentCustomerCart = null;

        public string selectedService = "";
        public string selectedService_Name = "";

        public string selectedStaff = "";
        public string selectedStaff_Name = "";

        public string appoiment_checkIn_Id = "";

        public string selected_tab = "";

        public string is_appoiment_form = "0";
        public string customer_id = "";

        public FormCustomerSignIn()
        {
            InitializeComponent();
        }

        public FormCustomerSignIn(FormMain parent, string serachPhone, string selected_tab)
        {
            InitializeComponent();

            this.parentForm = parent;
            txtNew_Phone.Text = serachPhone;
            this.selected_tab = selected_tab;

            this.InitForm();
        }

        public FormCustomerSignIn(UCCustomerSearchResult parent, string id, string selected_tab)
        {
            InitializeComponent();

            this.parentCustomerCart = parent;
            this.selected_tab = "SignUp";

            this.InitForm();

            this.customer_id = id;
            this.GetCustomerInfo();
            
        }

        private void GetCustomerInfo()
        {
            var responce = Utilitys.CALL_API("Customer/info?id=" + this.customer_id, "", "GET", true);
            if (responce.StartsWith("Error"))
            {
                MessageBox.Show(responce);
                return;
            }

            //{"id":139025,"name":"CC DD","firstName":"CC","lastName":"DD","birthday":"","country":null,"email":"","phone":"0909110687","icon":null,"status":1,"address":"","state":"","zipCode":"","city":"","birthday_Day":"0","birthday_Month":"0","referalCode":"","bus_Name":"","bus_Phone":"","bus_ZipCode":"","bus_City":"","aboutUs":"","yourFriend":"","storeId":500228,"totalReward":null,"paringCode":null}
            JObject obj = JObject.Parse(responce);
            txtNew_Phone.Text = obj["phone"].ToString();
            txtNew_ReferralCode.Text = obj["phone"].ToString();
            txtNew_FirstName.Text = obj["firstName"].ToString();
            txtNew_LastName.Text = obj["lastName"].ToString();
            txtNew_Email.Text = obj["email"].ToString();

            slNew_BirthdayDay.Text = obj["birthday_Day"].ToString();
            slNew_BirthdayMonth.Text = obj["birthday_Month"].ToString();
            txtNew_TotalReward.Text = obj["totalReward"].ToString();
            txtNew_Address.Text = obj["address"].ToString();
            txtNew_State.Text = obj["state"].ToString();
            txtNew_City.Text = obj["city"].ToString();
            txtNew_Zipcode.Text = obj["zipCode"].ToString();

            tabNewCustomer.Caption = "CUSTOMER INFO";
            btnAddAndCheckIn.Text = "SAVE";

            this.GetCustomerHistory();
        }

        private void GetCustomerHistory()
        {
            var responce = Utilitys.CALL_API("Customer/history?id=" + this.customer_id, "", "GET", true);
            if (responce.StartsWith("Error"))
            {
                MessageBox.Show(responce);
                return;
            }

            //[{"orderId":"148134","orderDate":"2025-06-30T17:12:24","itemName":"Item","price":22.0,"qty":1.0,"discount":0.0,"subtotal":22.0},{"orderId":"148134","orderDate":"2025-06-30T17:12:24","itemName":"Item","price":44.0,"qty":1.0,"discount":0.0,"subtotal":44.0},{"orderId":"148133","orderDate":"2025-06-30T17:11:40","itemName":"Item Test","price":22.0,"qty":1.0,"discount":0.0,"subtotal":22.0},{"orderId":"148133","orderDate":"2025-06-30T17:11:40","itemName":"Item 22","price":33.0,"qty":1.0,"discount":0.0,"subtotal":33.0}]
            JArray jArray = JArray.Parse(responce);
            int locationY = 45;
            foreach (JObject obj in jArray)
            {
                string orderDate = obj["orderDate"].ToString();
                string itemName = obj["itemName"].ToString();
                string price = obj["price"].ToString();
                string qty = obj["qty"].ToString();
                string subtotal = obj["subtotal"].ToString();

                UCCustomerInfoLine cardItem = new NailsChekin.UserControl.UCCustomerInfoLine(orderDate, itemName, price, qty, subtotal);
                cardItem.Width = xtraScrollableControl1.Width - 15;
                cardItem.Location = new Point(0, locationY); locationY += (cardItem.Height + 5);
                xtraScrollableControl1.Controls.Add(cardItem);
            }

        }

        private void InitForm()
        {
            //Setting
            string checkin_option = Utilitys.GetConfig("checkin_option", Constants.checkin_option_default);
            
            if (this.selected_tab.Equals("SignUp"))
            {
                this.tabPane1.SelectedPage = tabNewCustomer;
            }
            else
            {
                this.tabPane1.SelectedPage = tabSignIn;
            }
            
            DXPopupMenu popupDay = new DXPopupMenu();
            for (int i = 1; i <= 31; i++)
                popupDay.Items.Add(new DXMenuItem() { Caption = i.ToString() });
            slNew_BirthdayDay.DropDownControl = popupDay;

            foreach (DXMenuItem item in popupDay.Items)
                item.Click += item_day_Click;

            DXPopupMenu popupMonth = new DXPopupMenu();
            for (int i = 1; i <= 12; i++)
                popupMonth.Items.Add(new DXMenuItem() { Caption = i.ToString() });
            slNew_BirthdayMonth.DropDownControl = popupMonth;

            foreach (DXMenuItem item in popupMonth.Items)
                item.Click += item_month_Click;
        }

        private void item_month_Click(object sender, EventArgs e)
        {
            slNew_BirthdayMonth.Text = ((DXMenuItem)sender).Caption;
        }

        private void item_day_Click(object sender, EventArgs e)
        {
            slNew_BirthdayDay.Text = ((DXMenuItem)sender).Caption;
        }

        private void btnTabSignIn_Click(object sender, EventArgs e)
        {
            this.EnableTab(0);
        }

        private void btnTabNew_Click(object sender, EventArgs e)
        {
            this.EnableTab(1);
        }

        private void EnableTab(int tab_selected)
        {
            //this.current_tab = tab_selected;

            //if (this.current_tab == 0) //Customer signIn
            //{
            //    btnTabSignIn.Appearance.BackColor = Color.White;
            //    btnTabNew.Appearance.BackColor = Color.AntiqueWhite;

            //    pnCustomerSignIn.Visible = true;
            //    pnNewCustomer.Visible = false;

            //    pnCustomerSignIn.Dock = DockStyle.Fill;
            //    pnNewCustomer.Dock = DockStyle.None;
            //}
            //else if (this.current_tab == 1) //New Customer
            //{
            //    btnTabSignIn.Appearance.BackColor = Color.AntiqueWhite;
            //    btnTabNew.Appearance.BackColor = Color.White;

            //    pnCustomerSignIn.Visible = false;
            //    pnNewCustomer.Visible = true;

            //    pnCustomerSignIn.Dock = DockStyle.None;
            //    pnNewCustomer.Dock = DockStyle.Fill;
            //}
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            //Enable Parent Form
            this.parentForm.EnableDisableControl(true);

            this.Dispose();
        }

        private void btnClose2_Click(object sender, EventArgs e)
        {
            //Enable Parent Form
            if (this.parentForm != null)
            {
                this.parentForm.EnableDisableControl(true);
            }
  
            this.Dispose();
        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            if (txtPhone.Text.Trim().Length < 9)
            {
                MessageBox.Show("Phone Not Correct");
                return;
            }

            this.Dispose();
        }
        
        private void btnAddAndCheckIn_Click(object sender, EventArgs e)
        {
            if (txtNew_Phone.Text.Trim().Length < 9)
            {
                MessageBox.Show("Phone Not Correct");
                return;
            }

            if (txtNew_FirstName.Text.Trim().Length <= 0 || txtNew_LastName.Text.Trim().Length <= 0)
            {
                MessageBox.Show("Please Enter FirstName and LastName");
                return;
            }

            //Disable        
            btnAddAndCheckIn.Enabled = false;
            btnAddAndCheckIn.Text = "Waiting...";
            
            string birthay_day = slNew_BirthdayDay.Text.Trim().Length <= 0 ? "0" : slNew_BirthdayDay.Text.Trim();
            string birthay_month = slNew_BirthdayMonth.Text.Trim().Length <= 0 ? "0" : slNew_BirthdayMonth.Text.Trim();

            string DATA = @"{
                            'id':" + ( string.IsNullOrEmpty(this.customer_id) ? "0" : this.customer_id ) + @", 
                            'name':'" + (txtNew_FirstName.Text + " " + txtNew_LastName.Text) + @"',
                            'firstName':'" + txtNew_FirstName.Text + @"',
                            'lastName':'" + txtNew_LastName.Text + @"',
                            'birthday':'',
                            'country':'USA',
                            'email':'" + txtNew_Email.Text + @"',
                            'phone':'" + txtNew_Phone.Text + @"',
                            'icon':'',
                            'status':1,
                            'address':'" + txtNew_Address.Text + @"',
                            'state':'" + txtNew_State.Text + @"',
                            'zipCode':'" + txtNew_Zipcode.Text + @"',
                            'city':'" + txtNew_City.Text + @"',
                            'birthday_Day':'" + birthay_month + @"',
                            'birthday_Month':'" + birthay_day + @"',
                            'referalCode':'" + txtNew_ReferralCode.Text + @"',
                            'bus_Name':'',
                            'bus_Phone':'',
                            'bus_ZipCode':'',
                            'bus_City':'',
                            'aboutUs':'',
                            'yourFriend':'" + txtNew_ReferralCode.Text + @"',
                            'paringCode':'" + Constants.pairing_code + @"'
                        }";

            string response = Utilitys.CALL_API("Customer/createOrUpdate", DATA, "POST", true);
            if (response.StartsWith("Error:"))
            {
                MessageBox.Show(response);

                //Enable
                btnAddAndCheckIn.Enabled = true;
                btnAddAndCheckIn.Text = "Add And CheckIn";

                return;
            }

            if (this.parentForm != null)
            {
                this.parentForm.EnableDisableControl(true);
                //this.parentForm.InitCustomers(); // ==> Add Customer ( using websocket )
            }
            else if (this.parentCustomerCart != null)
            {
                this.parentCustomerCart.UpdateNewInfo(txtNew_Phone.Text, txtNew_FirstName.Text, txtNew_LastName.Text);
            }

            this.Dispose();
        }

        private void txtPhone_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            string phone = txtPhone.Text;

            //if (phone.Length < 10)
            //{
            //    txtFirstname.Text = "";
            //    txtLastName.Text = "";
            //}
            //else
            //{
            //    POSService.MaxViewWebServiceSoapClient service = new POSService.MaxViewWebServiceSoapClient();

            //    string jsonStrResponse = service.AppCheckIn_CustomerCheckExits(txtPhone.Text, 
            //                                    NailsChekin.Models.Constants.pos_store_code, NailsChekin.Models.Constants.pos_sceret_key);

            //    var response = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonStrResponse);
            //    if (response != null)
            //    {
            //        if (response[0].FirstOrDefault().Value.Equals("Success"))
            //        {
            //            string fullName = response[2].FirstOrDefault().Value;
            //            string firstName = response[3].FirstOrDefault().Value;
            //            string lastName = response[4].FirstOrDefault().Value;

            //            if (firstName.Trim().Length > 0)
            //            {
            //                txtFirstname.Text = firstName;
            //                txtLastName.Text = lastName;
            //            }
            //            else if (fullName.Trim().Length > 0)
            //            {
            //                txtFirstname.Text = Regex.Split( fullName, " " )[0];
            //                txtLastName.Text = Regex.Split(fullName, " ")[1];
            //            }
            //        }
            //    }
            //}

        }

    }
}
