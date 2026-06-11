using NailsChekin.Models;
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
    public partial class FormEmailSms : Form
    {
        public string orderId = "";
        public string phone = "";

        public FormEmailSms()
        {
            InitializeComponent();
        }

        public FormEmailSms(string orderId, string phone)
        {
            InitializeComponent();

            this.orderId = orderId;
            this.phone = phone;
            txtPhone.Text = phone;
        }

        private void btnSendEmail_Click(object sender, EventArgs e)
        {
            if (!txtEmail.Text.Contains("@"))
            {
                MessageBox.Show("Please enter email address");
                return;
            }

            btnSendEmail.Enabled = false;
            btnSendSms.Enabled = false;

            this.SendNow("Email");

        }

        private void btnSendSms_Click(object sender, EventArgs e)
        {
            if (txtPhone.Text.Trim().Length <= 9)
            {
                MessageBox.Show("Please enter phone number correct");
                return;
            }

            btnSendEmail.Enabled = false;
            btnSendSms.Enabled = false;

            this.SendNow("SMS");

        }

        private void SendNow(string type)
        {
            string DATA = @"{
                                'id': " + this.orderId + @",
                                'sendDate': '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + @"',
                                'type': '" + type + @"',
                                'email': '" + txtEmail.Text + @"',
                                'phone': '" + txtPhone.Text + @"'  
                            }";

            string responce = Utilitys.CALL_API("Order/sendNow", DATA, "POST", true);
            if (responce.ToUpper().StartsWith("ERROR")) //Fail
            {
                MessageBox.Show("Send Error: " + Environment.NewLine + responce);
                btnSendEmail.Enabled = true;
                btnSendSms.Enabled = true;
                return;
            }

            this.Dispose();
        }

    }
}
