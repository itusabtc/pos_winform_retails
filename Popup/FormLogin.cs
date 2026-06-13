using Newtonsoft.Json;
using NailsChekin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using NailsChekin.MyControls;
using NailsChekin.Models.Helper;

namespace NailsChekin.Popup
{
    public partial class FormLogin : Form
    {
        FormMain parentForm = null;
        string current_login = "";

        public FormLogin()
        {
            InitializeComponent();
        }

        public FormLogin(FormMain parent)
        {
            InitializeComponent();
            this.BackColor = ColorHelper.DefaultBackgoundColor;
            this.parentForm = parent;
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            //Read Config
            txtStoreCode.Text = Utilitys.GetStoreConfig("store_code", "");
            lbStoreName.Text = Utilitys.GetStoreConfig("store_name", "");
            lbStoreTimeZone.Text = Utilitys.GetStoreConfig("timezone", "0");

            current_login = Utilitys.GetStoreConfig("current_login", "");
            if (current_login.Equals("1"))
                btnAction.Text = "Logout";
            else
                btnAction.Text = "Login";

        }

        private static bool _isRestarting;
        private void btnAction_Click(object sender, EventArgs e)
        {
            if (txtStoreCode.Text.Trim().Length <= 0)
            {
                MessageBox.Show("Please check username");
                return;
            }

            if (txtPinCode.Text.Trim().Length <= 0)
            {
                MessageBox.Show("Please check password");
                return;
            }

            btnAction.Enabled = false;

            if (current_login.Equals("1")) //Logout
            {
                //Logout

                //Save To File
                string configs = "";

                configs += "store_code: " + txtStoreCode.Text + "\n";
                configs += "store_id: \n";
                configs += "store_name: \n";
                configs += "timezone: \n";
                configs += "accessToken: \n";
                configs += "current_login: 0\n";

                Utilitys.SaveAllStoreConfig(configs);

                this.parentForm.EnableDisableControl(false);
                this.parentForm.setTileNavPane1_Default();

                current_login = "0";
                btnAction.Text = "Login";
                btnAction.Enabled = true;
            }
            else //Login
            {
                string DATA = "{'userName':'" + txtStoreCode.Text + "','password':'" + txtPinCode.Text + "'}";

                var response = Utilitys.CALL_API("Login", DATA, "POST");
                if (response != null)
                {
                    btnAction.Enabled = true;

                    if (response.StartsWith("Error"))
                    {
                        CustomMessageBox.Show(response);
                        return;
                    }
                    else 
                    {
                        string storeId = JObject.Parse(response)["storeId"].ToString();
                        string storeName = JObject.Parse(response)["storeName"].ToString();
                        string accessToken = JObject.Parse(response)["accessToken"].ToString();
                        string timezone = "-5";

                        //Check And Create Forder
                        Utilitys.CreateForderConfig();

                        //Save To File
                        string configs = "";

                        configs += "store_code: " + txtStoreCode.Text + "\n";
                        configs += "store_id: " + storeId + "\n";
                        configs += "store_name: " + storeName + "\n";
                        configs += "timezone: " + timezone + "\n";
                        configs += "accessToken: " + accessToken + "\n";
                        configs += "current_login: 1\n";

                        Utilitys.SaveAllStoreConfig(configs);

                        Constants.pos_store_code = txtStoreCode.Text;
                        Constants.pos_store_id = storeId;
                        Constants.pos_timezone = int.Parse(timezone);

                        //this.parentForm.InitFormData(true);
                        //this.parentForm.EnableDisableControl(true);
                        //this.parentForm.setTileNavPane1_Default();
                        this.Dispose();

                        // 2) Bỏ các confirm FormClosing khi đang restart (nếu bạn có)
                        _isRestarting = true;

                        // 3) Restart chính app (giữ nguyên command-line args)
                        //Application.Restart();
                        // Sau dòng này Windows Forms sẽ cố gắng đóng toàn bộ message loop và mở lại tiến trình

                        // giữ args nếu bạn cần
                        var args = Environment.GetCommandLineArgs();
                        var argLine = string.Join(" ", args, 1, args.Length - 1);

                        // thêm cờ restart để instance mới biết chờ mutex
                        var startArgs = "--restart " + argLine;

                        try
                        {
                            using (Process.Start(Application.ExecutablePath, startArgs)) { }
                        }
                        catch { /* log nếu cần */ }

                        // Thoát app hiện tại (mutex sẽ được release ở ApplicationExit)
                        Application.Exit();
                    }
                }
            }
        }
        
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();

            // Thoát app hiện tại (mutex sẽ được release ở ApplicationExit)
            Application.Exit();
        }

    }
}
