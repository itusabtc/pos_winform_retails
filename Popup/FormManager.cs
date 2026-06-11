using NailsChekin.Models;
using NailsChekin.Models.Helper;
using NailsChekin.MyControls;
using System;
using System.Windows.Forms;

namespace NailsChekin.Popup
{
    public partial class FormManager : Form
    {
        Control parentForm;

        string redirect_url = "";
        string pincode_type = "2"; //Default User OR Staff
        string member_id = "";

        string pin_enter = "";
        string jData = "";

        public FormManager()
        {
            InitializeComponent();
        }

        public FormManager(Control parent, string pincode_type, string member_id, string redirect_url)
        {
            InitializeComponent();

            this.parentForm = parent;
            this.pincode_type = pincode_type;
            this.member_id = member_id;
            this.redirect_url = redirect_url;
        }

        public FormManager(Control parent, string pincode_type, string member_id, string redirect_url, string jData, string title = "")
        {
            InitializeComponent();

            this.parentForm = parent;
            this.pincode_type = pincode_type;
            this.member_id = member_id;
            this.redirect_url = redirect_url;
            this.jData = jData;

            if (!string.IsNullOrEmpty(title))
                lbTitle.Text = title.ToUpper();
            else
                lbTitle.Text = "ENTER OFFICE PASSWORD";
        }


        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (txtInput.Text.Trim().Length < 4)
            {
                CustomMessageBox.Show("Manager PIN Code Not Correct");
                return;
            }

            btnConfirm.Title = "WAITING...";
            btnConfirm.Enabled = false;

            //Check PIN CODE
            //POSService.MaxViewWebServiceSoapClient service = new POSService.MaxViewWebServiceSoapClient();
            //string jsonStrResponse = service.AppCheckIn_CheckPINCode(txtInput.Text, this.pincode_type, this.member_id, NailsChekin.Models.Constants.pos_store_code, NailsChekin.Models.Constants.pos_sceret_key);

            //var response = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonStrResponse);
            //if (response != null)
            //{
            //    btnConfirm.Text = "Confirm";
            //    btnConfirm.Enabled = true;

            //    if (response[0].FirstOrDefault().Value.Equals("Fail"))
            //    {
            //        string msg = response[1].FirstOrDefault().Value;
            //        MessageBox.Show(msg);
            //        return;
            //    }
            //}

            ////Process if PINCode Suceess
            //if (this.redirect_url.Equals("CloseOutReport"))
            //{
            //    service = new POSService.MaxViewWebServiceSoapClient();
            //    jsonStrResponse = service.AppCheckIn_CloseOutReport(txtInput.Text, NailsChekin.Models.Constants.pos_store_code, NailsChekin.Models.Constants.pos_sceret_key);

            //    response = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(jsonStrResponse);
            //    if (response != null)
            //    {
            //        btnConfirm.Text = "Confirm";
            //        btnConfirm.Enabled = true;

            //        if (response[0].FirstOrDefault().Value.Equals("Fail"))
            //        {
            //            string msg = response[1].FirstOrDefault().Value;
            //            MessageBox.Show(msg);
            //        }
            //        else if (response[0].FirstOrDefault().Value.Equals("Success"))
            //        {


            //            //this.parentForm.EnableDisableControl(true);
            //            this.Dispose();
            //        }
            //    }
            //}

            //else if (this.redirect_url.Equals("ViewDetailCustomer"))
            //{
            //    //((UCCustomer)this.parentForm).Show_Detail_Info();
            //    this.Dispose();
            //}


            if (this.redirect_url.Equals("OpenCashDrawer"))
            {
                string error = PrinterLocalHelper.OpenCashDraweFromPrinter(Constants.printer_name);
                if (error.Trim().Length > 0)
                    CustomMessageBox.Show(error);
            }

            this.Close();

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }


    }
}
