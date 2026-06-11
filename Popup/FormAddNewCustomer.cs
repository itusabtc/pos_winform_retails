using DevExpress.Utils.Menu;
using NailsChekin.Models;
using NailsChekin.MyControls;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NailsChekin.Popup
{
    public partial class FormAddNewCustomer : Form
    {
        Control parentForm;

        public FormAddNewCustomer()
        {
            InitializeComponent();
        }

        public FormAddNewCustomer(FormMain parent, string serachPhone)
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            
            this.parentForm = parent;
            txtNew_Phone.Text = serachPhone;
        }

        private void FormAddNewCustomer_Load(object sender, EventArgs e)
        {
            this.Adjust_Screen();

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

        public void Adjust_Screen()
        {
            //Bất đồng bộ
            this.BeginInvoke(new Action(() =>
            {
                btnClose.Location = new Point(this.Width - btnClose.Width - 10, btnClose.Location.Y);
            }));
        }

        private void FormAddNewCustomer_FormClosed(object sender, FormClosedEventArgs e)
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnFinish_Click(object sender, EventArgs e)
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
            btnFinish.Enabled = false;
            btnFinish.Text = "Waiting...";

            string birthay_day = slNew_BirthdayDay.Text.Trim().Length <= 0 ? "0" : slNew_BirthdayDay.Text.Trim();
            string birthay_month = slNew_BirthdayMonth.Text.Trim().Length <= 0 ? "0" : slNew_BirthdayMonth.Text.Trim();

            string DATA = @"{
                            'id':0, 
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
                CustomMessageBox.Show(response);

                //Enable
                btnFinish.Enabled = true;
                btnFinish.Text = "SAVE";

                return;
            }

            //{"id":140335,"name":"CC DD","firstName":"CC","lastName":"DD","birthday":"","country":null,"email":"","phone":"0909111223","icon":null,"status":1,"address":"","state":"","zipCode":"","city":"","birthday_Day":"0","birthday_Month":"0","referalCode":"","bus_Name":"","bus_Phone":"","bus_ZipCode":"","bus_City":"","aboutUs":"","yourFriend":"","storeId":100018,"totalReward":0.0,"paringCode":""}
            JObject obj = JObject.Parse(response);
            string id = obj["id"].ToString();
            string phone = obj["phone"].ToString();
            string firstName = obj["firstName"].ToString();
            string lastName = obj["lastName"].ToString();

            ((FormMain)this.parentForm).SetCustomerInfo(id, phone, firstName, lastName);
            this.Close();
        }


    }
}
