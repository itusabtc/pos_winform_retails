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
using NailsChekin.Popup;

namespace NailsChekin.UserControl
{
    public partial class UCCustomerSearchResult : DevExpress.XtraEditors.XtraUserControl
    {
        public string id = "";
        public string phone = "";
        public string firstname = "";
        public string lastname = "";
        public string birthday = "";
        public string address = "";
        FormConfirmSelectCustomerResult parent;

        public bool selected = false;

        public UCCustomerSearchResult()
        {
            InitializeComponent();
        }

        public UCCustomerSearchResult(FormConfirmSelectCustomerResult parent, string id, string phone, string firstname, string lastname, string birthday = "", string address = "")
        {
            InitializeComponent();
            this.parent = parent;

            this.id = id;
            this.phone = phone;
            this.firstname = firstname;
            this.lastname = lastname;
            
            lbPhone.Text = phone;
            lbFirstname.Text = firstname;
            lbLastname.Text = lastname;
            lbBirthday.Text = birthday;
            lbAddress.Text = address;
        }

        private void svgSelect_Click(object sender, EventArgs e)
        {
            parent.SetCustomerInfo(id, phone, firstname, lastname);
        }

        private void svgInfo_Click(object sender, EventArgs e)
        {
            //FormCustomerSignIn frm = new FormCustomerSignIn(this, this.id, "SignUp");
            //frm.StartPosition = FormStartPosition.CenterScreen;
            //frm.ShowDialog(this);
            //frm.Dispose();
        }

        public void UpdateNewInfo(string phone, string firstname, string lastname)
        {
            this.phone = phone;
            this.firstname = firstname;
            this.lastname = lastname;

            lbPhone.Text = phone;
            lbFirstname.Text = firstname;
            lbLastname.Text = lastname;
        }

    }

}
