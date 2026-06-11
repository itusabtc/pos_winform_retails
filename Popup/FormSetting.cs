using DevExpress.Utils.Menu;
using NailsChekin.Models;
using System;
using System.Windows.Forms;
using System.Deployment.Application;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using CloverExamplePOS;
using com.clover.remotepay.transport;
using NailsChekin.Models.Helper;
using System.Drawing;
using System.Diagnostics;

namespace NailsChekin.Popup
{
    public partial class FormSetting : Form
    {
        FormMain parentForm = null;

        public FormSetting()
        {
            // Trong FormSetting constructor hoặc designer:
            this.AutoScaleMode = AutoScaleMode.Dpi; 

            InitializeComponent();
        }

        public FormSetting(FormMain parent)
        {
            InitializeComponent();
            this.BackColor = ColorHelper.DefaultBackgoundColor;

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();

            this.parentForm = parent;
        }

        private async void FormSetting_Load(object sender, EventArgs e)
        {
            this.Adjust_Screen();

            // 1) Setup menus — nhanh, UI thread OK
            SetupDropDownMenus();

            // 2) Load local config — đọc file local, nhanh, UI thread OK
            LoadLocalConfig();

            // 3) Enumerate printer list — query OS/spooler, có thể chậm → background thread
            await LoadPrinterListAsync();
            if (this.IsDisposed) return;

            // 4) Gọi API server — blocking nếu để sync → async hoàn toàn, form vẽ xong trước
            await UpdateSettingFromAPIAsync();
        }

        private void SetupDropDownMenus()
        {
            DXPopupMenu popupCreditCardDevice = new DXPopupMenu();
            popupCreditCardDevice.Items.Add(new DXMenuItem() { Caption = "CODE PAY" });
            popupCreditCardDevice.Items.Add(new DXMenuItem() { Caption = "CLOVER" });
            ddCreditCardDevice.DropDownControl = popupCreditCardDevice;
            foreach (DXMenuItem item in popupCreditCardDevice.Items)
                item.Click += item_creditCardDevice_setting_Click;

            DXPopupMenu popupConnection = new DXPopupMenu();
            popupConnection.Items.Add(new DXMenuItem() { Caption = "Clover Connector USB" });
            popupConnection.Items.Add(new DXMenuItem() { Caption = "Network Pay Display" });
            txtConnecttionType.DropDownControl = popupConnection;
            foreach (DXMenuItem item in popupConnection.Items)
                item.Click += item_connection_setting_Click;

            DXPopupMenu popupConnectionCodePay = new DXPopupMenu();
            popupConnectionCodePay.Items.Add(new DXMenuItem() { Caption = "WLAN/LAN" });
            popupConnectionCodePay.Items.Add(new DXMenuItem() { Caption = "WIFI/INTERNET" });
            txtConnecttionType_CodePay.DropDownControl = popupConnectionCodePay;
            foreach (DXMenuItem item in popupConnectionCodePay.Items)
                item.Click += item_connection_codepay_setting_Click;

            DXPopupMenu popupUsingSystemCredit = new DXPopupMenu();
            popupUsingSystemCredit.Items.Add(new DXMenuItem() { Caption = "ON" });
            popupUsingSystemCredit.Items.Add(new DXMenuItem() { Caption = "OFF" });
            ddUsingSystemCredit.DropDownControl = popupUsingSystemCredit;
            foreach (DXMenuItem item in popupUsingSystemCredit.Items)
                item.Click += item_usingSystemCredit_setting_Click;

            ddlSurChargeUnit.Properties.Items.Add("%");
            ddlSurChargeUnit.Properties.Items.Add("$");
            ddlSurChargeUnit.Text = "%";
        }

        // Enumerate OS printer list trên background thread — tránh block UI
        private async Task LoadPrinterListAsync()
        {
            var printers = await Task.Run(() =>
            {
                var list = new System.Collections.Generic.List<string>();
                foreach (string p in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
                    list.Add(p);
                return list;
            });
            if (this.IsDisposed) return;

            DXPopupMenu popupPrinterList = new DXPopupMenu();
            popupPrinterList.Items.Add(new DXMenuItem() { Caption = "" });
            foreach (string printer in printers)
                popupPrinterList.Items.Add(new DXMenuItem() { Caption = printer });
            ddPrinterList.DropDownControl = popupPrinterList;
            foreach (DXMenuItem item in popupPrinterList.Items)
                item.Click += item_printerList_setting_Click;
        }

        // Đọc config local (file) — nhanh, UI thread OK
        private void LoadLocalConfig()
        {
            // Credit Device
            ddCreditCardDevice.Text = Utilitys.GetConfig("credit_card_device", "CODE PAY");
            if (ddCreditCardDevice.Text.Equals("CODE PAY"))
            {
                gbCodepaySetting.Visible = true;
                gbCloverSetting.Visible  = false;
            }
            else
            {
                gbCodepaySetting.Visible = false;
                gbCloverSetting.Visible  = true;
            }

            chkTipsOn.Checked      = Utilitys.GetConfig("chkTipsOn",      Constants.chkTipsOn);
            chkTipsOff.Checked     = Utilitys.GetConfig("chkTipsOff",     Constants.chkTipsOff);
            chkSigOnPaper.Checked  = Utilitys.GetConfig("chkSigOnPaper",  Constants.chkSigOnPaper);
            chkSigOnScreen.Checked = Utilitys.GetConfig("chkSigOnScreen", Constants.chkSigOnScreen);

            txtConnecttionType.Text         = Utilitys.GetConfig("clover_connection_type",       Constants.clover_connection_type);
            txtCloverIPAddress.Text         = Utilitys.GetConfig("clover_ip_address",            Constants.clover_ip_address);
            txtConnecttionType_CodePay.Text = Utilitys.GetConfig("codepay_connection_type",      Constants.codepay_connection_type_defaul);
            txtCodePayIPAddress.Text        = Utilitys.GetConfig("codepay_ip_address",           Constants.codepay_ip_address_defaul);
            txtCodePayAppId.Text            = Utilitys.GetConfig("codepay_app_id",               Constants.codepay_app_id_default);
            txtCodePayMerchantOrderNo.Text  = Utilitys.GetConfig("codepay_merchant_order_no",    Constants.codepay_merchant_order_no_default);

            // TAX Setting
            chkTaxOn.Checked   = Utilitys.GetConfig("chkTaxOn",  Constants.chkTaxOn);
            chkTaxOff.Checked  = Utilitys.GetConfig("chkTaxOff", Constants.chkTaxOff);
            txtTaxPercent.Text = Utilitys.GetConfig("tax_percent", Constants.tax_percent);

            // Receipt Print Setting
            chkReceiptCusCheckin.Checked    = Utilitys.GetConfig("chkReceiptCusCheckin",    false);
            chkShowPopupConfirmBill.Checked = Utilitys.GetConfig("chkShowPopupConfirmBill", false);

            // Pairing Code
            txtPairingCode.Text = Utilitys.GetConfig("pairing_code", "");

            ddUsingSystemCredit.Text = Utilitys.GetConfig("using_system_credit", Constants.using_system_credit);
            ddPrinterList.Text       = Utilitys.GetConfig("printer_name",        Constants.printer_name);

            // SURCHARGE Setting
            chkSurChargeOn.Checked      = Utilitys.GetConfig("chkSurChargeOn",      Constants.chkSurChargeOn);
            chkSurChargeOff.Checked     = Utilitys.GetConfig("chkSurChargeOff",     Constants.chkSurChargeOff);
            ddlSurChargeUnit.Text       = Utilitys.GetConfig("surCharge_unit",       Constants.surCharge_unit);
            txtSurCharge_percent.Text   = Utilitys.GetConfig("surCharge_percent",    Constants.surCharge_percent);
            txtSurCharge_minAmount.Text = Utilitys.GetConfig("surCharge_minAmount",  Constants.surCharge_minAmount);
        }

        // Gọi API trên background thread — không block UI thread
        private async Task UpdateSettingFromAPIAsync()
        {
            string responce = await Task.Run(() => Utilitys.CALL_API("Store/getStoreSetting", "", "GET", true));
            if (this.IsDisposed) return;

            if (!responce.StartsWith("Error"))
            {
                // Parse JSON 1 lần duy nhất trên background thread
                var (creditOn, taxOn, taxPct, footer) = await Task.Run(() =>
                {
                    var jObj = JObject.Parse(responce);
                    return (
                        jObj["credit_setting_on"]?.ToString() ?? "0",
                        jObj["tax_setting_on"]?.ToString()    ?? "0",
                        jObj["tax_percent"]?.ToString()       ?? "0",
                        jObj["receipt_footer"]?.ToString()    ?? ""
                    );
                });
                if (this.IsDisposed) return;

                Constants.using_system_credit = creditOn.Equals("1") ? "ON" : "OFF";
                Constants.tax_percent         = taxPct;
                Constants.chkTaxOn            = taxOn.Equals("1");
                Constants.chkTaxOff           = !taxOn.Equals("1");
                Constants.receiptFooter       = footer;
            }

            // Cập nhật UI với giá trị mới từ server
            chkTaxOn.Checked      = Constants.chkTaxOn;
            chkTaxOff.Checked     = Constants.chkTaxOff;
            txtTaxPercent.Text    = Constants.tax_percent;
            txtReceiptFooter.Text = Constants.receiptFooter;
        }

        private void Adjust_Screen()
        {
            this.BeginInvoke(new Action(() =>
            {
                btnClose.Location = new Point(this.Width - btnClose.Width - 10, btnClose.Location.Y);
                panelContent.Height = this.Height - lbTitle.Bottom - 20;
                panelContent.Width  = this.Width - 20;
                panelControls.Left  = (panelContent.Width  - panelControls.Width)  / 2;
                panelControls.Top   = (panelContent.Height - panelControls.Height) / 2;
            }));
        }

        private void item_creditCardDevice_setting_Click(object sender, EventArgs e)
        {
            ddCreditCardDevice.Text = ((DXMenuItem)sender).Caption;
            if (ddCreditCardDevice.Text.Equals("CODE PAY"))
            {
                gbCodepaySetting.Visible = true;
                gbCloverSetting.Visible = false;
            }
            else
            {
                gbCodepaySetting.Visible = false;
                gbCloverSetting.Visible = true;
            }
        }

        private void item_printerList_setting_Click(object sender, EventArgs e)
        {
            ddPrinterList.Text = ((DXMenuItem)sender).Caption;
        }

        private void item_connection_setting_Click(object sender, EventArgs e)
        {
            txtConnecttionType.Text = ((DXMenuItem)sender).Caption;
        }

        private void item_connection_codepay_setting_Click(object sender, EventArgs e)
        {
            txtConnecttionType_CodePay.Text = ((DXMenuItem)sender).Caption;
        }

        private void item_usingSystemCredit_setting_Click(object sender, EventArgs e)
        {
            ddUsingSystemCredit.Text = ((DXMenuItem)sender).Caption;
        }

        private async void btnFinish_Click(object sender, EventArgs e)
        {
            // Lưu vào Constants
            Constants.credit_card_device      = ddCreditCardDevice.Text;
            Constants.chkTipsOn               = chkTipsOn.Checked;
            Constants.chkTipsOff              = chkTipsOff.Checked;
            Constants.chkSigOnPaper           = chkSigOnPaper.Checked;
            Constants.chkSigOnScreen          = chkSigOnScreen.Checked;
            Constants.clover_connection_type  = txtConnecttionType.Text;
            Constants.clover_ip_address       = txtCloverIPAddress.Text;
            Constants.codepay_connection_type = txtConnecttionType_CodePay.Text;
            Constants.codepay_ip_address      = txtCodePayIPAddress.Text;
            Constants.codepay_app_id          = txtCodePayAppId.Text;
            Constants.codepay_merchant_order_no = txtCodePayMerchantOrderNo.Text;
            Constants.using_system_credit     = ddUsingSystemCredit.Text;
            Constants.chkTaxOn                = chkTaxOn.Checked;
            Constants.chkTaxOff               = chkTaxOff.Checked;
            Constants.tax_percent             = txtTaxPercent.Text;
            Constants.chkReceiptCusCheckin    = chkReceiptCusCheckin.Checked;
            Constants.chkShowPopupConfirmBill = chkShowPopupConfirmBill.Checked;
            Constants.receiptFooter           = txtReceiptFooter.Text;
            Constants.pairing_code            = txtPairingCode.Text;
            Constants.printer_name            = ddPrinterList.Text;
            Constants.chkSurChargeOn          = chkSurChargeOn.Checked;
            Constants.chkSurChargeOff         = chkSurChargeOff.Checked;
            Constants.surCharge_percent       = txtSurCharge_percent.Text;
            Constants.surCharge_minAmount     = txtSurCharge_minAmount.Text;
            Constants.surCharge_unit          = ddlSurChargeUnit.Text;

            // Tạo thư mục config nếu chưa có
            Utilitys.CreateForderConfig();

            // Build config string bằng StringBuilder — tránh string += trong loop
            var sb = new StringBuilder();
            sb.AppendLine("credit_card_device: "       + Constants.credit_card_device);
            sb.AppendLine("fullmenu_column_default: "  + Constants.fullmenu_column_default);
            sb.AppendLine("appt_column_default: "      + Constants.appt_column_default);
            sb.AppendLine("chkTipsOn: "                + Constants.chkTipsOn);
            sb.AppendLine("chkTipsOff: "               + Constants.chkTipsOff);
            sb.AppendLine("chkSigOnPaper: "            + Constants.chkSigOnPaper);
            sb.AppendLine("chkSigOnScreen: "           + Constants.chkSigOnScreen);
            sb.AppendLine("clover_connection_type: "   + Constants.clover_connection_type);
            sb.AppendLine("clover_ip_address: "        + Constants.clover_ip_address);
            sb.AppendLine("codepay_connection_type: "  + Constants.codepay_connection_type);
            sb.AppendLine("codepay_ip_address: "       + Constants.codepay_ip_address);
            sb.AppendLine("codepay_app_id: "           + Constants.codepay_app_id);
            sb.AppendLine("codepay_merchant_order_no: "+ Constants.codepay_merchant_order_no);
            sb.AppendLine("web_print_acrobatURL: "     + Constants.web_print_acrobatURL);
            sb.AppendLine("web_print_domain: "         + Constants.web_print_domain);
            sb.AppendLine("web_print_filePath: "       + Constants.web_print_filePath);
            sb.AppendLine("turn_system_cloumn_show: "  + Constants.turn_system_cloumn_show);
            sb.AppendLine("checkin_option: "           + Constants.checkin_option);
            sb.AppendLine("inservice_setting: "        + Constants.inservice_setting);
            sb.AppendLine("appt_version_setting: "     + Constants.appt_version_setting);
            sb.AppendLine("quickmenu_option: "         + Constants.quickmenu_option);
            sb.AppendLine("using_system_credit: "      + Constants.using_system_credit);
            sb.AppendLine("chkTaxOn: "                 + Constants.chkTaxOn);
            sb.AppendLine("chkTaxOff: "                + Constants.chkTaxOff);
            sb.AppendLine("tax_percent: "              + Constants.tax_percent);
            sb.AppendLine("chkSurChargeOn: "           + Constants.chkSurChargeOn);
            sb.AppendLine("chkSurChargeOff: "          + Constants.chkSurChargeOff);
            sb.AppendLine("surCharge_percent: "        + Constants.surCharge_percent);
            sb.AppendLine("surCharge_minAmount: "      + Constants.surCharge_minAmount);
            sb.AppendLine("surCharge_unit: "           + Constants.surCharge_unit);
            sb.AppendLine("chkReceiptCusCheckin: "     + Constants.chkReceiptCusCheckin);
            sb.AppendLine("chkShowPopupConfirmBill: "  + Constants.chkShowPopupConfirmBill);
            sb.AppendLine("receiptFooter: "            + Constants.receiptFooter);
            sb.AppendLine("pairing_code: "             + Constants.pairing_code);
            sb.AppendLine("printer_name: "             + Constants.printer_name);

            Utilitys.SaveAllConfig(sb.ToString());

            // Gửi setting lên server trên background thread — không block UI
            string jData = "{ "
                + " 'isDefault':'1',"
                + " 'using_system_credit':'" + (Constants.using_system_credit.Equals("OFF") ? "0" : "1") + "',"
                + " 'taxValue':'" + (Constants.chkTaxOn ? Constants.tax_percent : "0") + "',"
                + " 'receiptFooter':'" + Constants.receiptFooter + "'"
                + " }";

            await Task.Run(() => Utilitys.CALL_API("Store/updateStoreSetting", jData, "POST", true));
            if (this.IsDisposed) return;

            // Đóng form trước — tránh FormSetting bị đơ trong khi chờ device connect
            this.parentForm.isCodePaySocketConnect = false;
            this.parentForm.setTileNavPane1_Default();
            this.parentForm.InitConfig();
            this.Close();

            // re_connect: true → disconnect old Clover/_codePay trước, rồi init lại
            // Fire-and-forget có kiểm soát: Close() đã xong, parentForm vẫn còn sống
            _ = this.parentForm.InitializeCreditDeviceConnector_Async(re_connect: true);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.parentForm.setTileNavPane1_Default();
            this.Close();
        }

        #region Store full-menu in local

        private void btnUpdateLocalMenu_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Check Update Version

        private void InstallUpdateSyncWithInfo()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                bool updateIsAvailable;

                try
                {
                    updateIsAvailable = CheckForUpdateAvailable();
                    //MessageBox.Show("updateIsAvailable: " + updateIsAvailable);
                }
                catch (DeploymentDownloadException dde)
                {
                    MessageBox.Show("The new version of the application cannot be downloaded at this time. \n\nPlease check your network connection, or try again later. Error: " + dde.Message);
                    return;
                }
                catch (InvalidDeploymentException ide)
                {
                    MessageBox.Show("Cannot check for a new version of the application. The ClickOnce deployment is corrupt. Please redeploy the application and try again. Error: " + ide.Message);
                    return;
                }
                catch (InvalidOperationException ioe)
                {
                    MessageBox.Show("This application cannot be updated. It is likely not a ClickOnce application. Error: " + ioe.Message);
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Exception: " + ex.Message);
                    return;
                }

                if (updateIsAvailable)
                {
                    try
                    {
                        Uri updateLocation = ApplicationDeployment.CurrentDeployment.UpdateLocation;

                        System.Net.WebClient webClient = new System.Net.WebClient();
                        webClient.Encoding = Encoding.UTF8;
                        string manifestFile = webClient.DownloadString(updateLocation);

                        //MessageBox.Show("manifestFile: " + manifestFile);

                        //We have some garbage info from the file header, presumably because the file is a .application and not .xml
                        //Just start from the start of the first tag
                        int startOfXml = manifestFile.IndexOfAny(new[] { '<' });
                        manifestFile = manifestFile.Substring(startOfXml);

                        InstallApplication("http://95.217.32.253:8899/TestUpdate/TestUpdate.application");

                        //if (update_success)
                        //{
                        //ad.Update();
                        //MessageBox.Show("The application has been upgraded, and will now restart.");
                        //Application.Restart();
                        //}

                    }
                    catch (DeploymentDownloadException dde)
                    {
                        MessageBox.Show("Cannot install the latest version of the application. \n\nPlease check your network connection, or try again later. Error: " + dde);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("No new version !!!");
                    return;
                }

            }
        }


        InPlaceHostingManager iphm = null;

        public void InstallApplication(string deployManifestUriStr)
        {
            try
            {
                Uri deploymentUri = new Uri(deployManifestUriStr);
                iphm = new InPlaceHostingManager(deploymentUri, false);
            }
            catch (UriFormatException uriEx)
            {
                MessageBox.Show("Cannot update the application: " +
                    "The deployment manifest URL supplied is not a valid URL. " +
                    "Error: " + uriEx.Message);
                return;
            }
            catch (PlatformNotSupportedException platformEx)
            {
                MessageBox.Show("Cannot update the application: " +
                    "This program requires Windows XP or higher. " +
                    "Error: " + platformEx.Message);
                return;
            }
            catch (ArgumentException argumentEx)
            {
                MessageBox.Show("Cannot update the application: " +
                    "The deployment manifest URL supplied is not a valid URL. " +
                    "Error: " + argumentEx.Message);
                return;
            }

            iphm.GetManifestCompleted += new EventHandler<GetManifestCompletedEventArgs>(iphm_GetManifestCompleted);
            iphm.GetManifestAsync();
        }

        void iphm_GetManifestCompleted(object sender, GetManifestCompletedEventArgs e)
        {
            // Check for an error.
            if (e.Error != null)
            {
                // Cancel download and install.
                MessageBox.Show("Could not download manifest. Error: " + e.Error.Message);
                return;
            }

            // bool isFullTrust = CheckForFullTrust(e.ApplicationManifest);

            // Verify this application can be installed.
            try
            {
                // the true parameter allows InPlaceHostingManager
                // to grant the permissions requested in the applicaiton manifest.
                iphm.AssertApplicationRequirements(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while verifying the application. " +
                    "Error: " + ex.Message);
                return;
            }

            // Use the information from GetManifestCompleted() to confirm 
            // that the user wants to proceed.
            string appInfo = "Application Name: " + e.ProductName;
            appInfo += "\nVersion: " + e.Version;
            appInfo += "\nSupport/Help Requests: " + (e.SupportUri != null ? e.SupportUri.ToString() : "N/A");
            appInfo += "\n\nConfirmed that this application can run with its requested permissions.";
            // if (isFullTrust)
            // appInfo += "\n\nThis application requires full trust in order to run.";
            appInfo += "\n\nDo you want to update now?";

            DialogResult dr = MessageBox.Show(appInfo, "New version available to update",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dr != System.Windows.Forms.DialogResult.Yes)
            {
                return;
            }

            // Download the deployment manifest. 
            iphm.DownloadProgressChanged += new EventHandler<DownloadProgressChangedEventArgs>(iphm_DownloadProgressChanged);
            iphm.DownloadApplicationCompleted += new EventHandler<DownloadApplicationCompletedEventArgs>(iphm_DownloadApplicationCompleted);

            try
            {
                // Usually this shouldn't throw an exception unless AssertApplicationRequirements() failed, 
                // or you did not call that method before calling this one.
                iphm.DownloadApplicationAsync();
                //MessageBox.Show("DownloadApplicationAsync Complete !!!");

                //Application.Restart();
            }
            catch (Exception downloadEx)
            {
                MessageBox.Show("Cannot initiate download of application. Error: " + downloadEx.Message);
                return;
            }
        }

        void iphm_DownloadApplicationCompleted(object sender, DownloadApplicationCompletedEventArgs e)
        {
            // Check for an error.
            if (e.Error != null)
            {
                // Cancel download and install.
                MessageBox.Show("Could not download and install application. Error: " + e.Error.Message);
                return;
            }

            // Inform the user that their application is ready for use. 
            DialogResult dr = MessageBox.Show("The application has been upgraded, and will now restart.", "Application updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (dr == DialogResult.OK)
            {
                //Restart app when finish
                Application.Restart();
            }
        }

        void iphm_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // you can show percentage of task completed using e.ProgressPercentage

            string text = "Check New Version " + Environment.NewLine + "Downloaded: " + e.ProgressPercentage + "%";
            //btnCheckNewVersion.Text = text;
        }

        private bool CheckForUpdateAvailable()
        {
            Uri updateLocation = ApplicationDeployment.CurrentDeployment.UpdateLocation;

            //Used to use the Clickonce API but we've uncovered a pretty serious bug which results in a COMException and the loss of ability
            //to check for updates. So until this is fixed, we're resorting to a very lo-fi way of checking for an update.

            System.Net.WebClient webClient = new System.Net.WebClient();
            webClient.Encoding = Encoding.UTF8;
            string manifestFile = webClient.DownloadString(updateLocation);

            //We have some garbage info from the file header, presumably because the file is a .application and not .xml
            //Just start from the start of the first tag
            int startOfXml = manifestFile.IndexOfAny(new[] { '<' });
            manifestFile = manifestFile.Substring(startOfXml);

            Version version;

            XmlDocument doc = new XmlDocument();

            //build the xml from the manifest
            doc.LoadXml(manifestFile);

            XmlNodeList nodesList = doc.GetElementsByTagName("assemblyIdentity");
            if (nodesList == null || nodesList.Count <= 0)
            {
                throw new XmlException("Could not read the xml manifest file, which is required to check if an update is available.");
            }

            XmlNode theNode = nodesList[0];
            version = new Version(theNode.Attributes["version"].Value);

            if (version > ApplicationDeployment.CurrentDeployment.CurrentVersion)
            {
                // update application
                return true;
            }
            return false;
        }



        #endregion

        private void btnPairCloverDevice_Click(object sender, EventArgs e)
        {
            NailsChekin.Properties.Settings.Default.pairingAuthToken = "";
            NailsChekin.Properties.Settings.Default.selectedConfig = "WS";
            NailsChekin.Properties.Settings.Default.Save();

            new FormCloverConnectSetting(this, OnPairingCode, OnPairingSuccess, OnPairingState).Show();
        }

        AlertForm pairingForm;
        public void OnPairingCode(string pairingCode)
        {
            Invoke(new Action(() =>
            {
                pairingForm?.Dispose();
                pairingForm = new AlertForm(this);
                pairingForm.Title = "Pairing Code";
                pairingForm.Label = "Enter this code on the Clover Mini: " + pairingCode;
                pairingForm.Show();

                LogHelper.SaveLOG_Payment("OnPairingCode", "Enter this code on the Clover Mini: " + pairingCode);
            }
            ));
        }

        public void OnPairingSuccess(string pairingAuthToken)
        {
            NailsChekin.Properties.Settings.Default.pairingAuthToken = pairingAuthToken;
            NailsChekin.Properties.Settings.Default.selectedConfig = "WS";
            NailsChekin.Properties.Settings.Default.Save();
            Invoke(new Action(() => pairingForm?.Dispose()));

            LogHelper.SaveLOG_Payment("OnPairingSuccess", pairingAuthToken);
        }

        public void OnPairingState(string state, string message)
        {
            LogHelper.SaveLOG_Payment("OnPairingState", state + " MSG: " + message);
            if (state == "AUTHENTICATING")
            {
                Invoke(new Action(() =>
                {
                    pairingForm?.Dispose();
                    pairingForm = new AlertForm(this);
                    pairingForm.Title = "Pairing Security Pin";
                    pairingForm.Label = message;
                    pairingForm.Show();
                }
                ));
            }
        }

        public void SwitchCloverMode(CloverDeviceConfiguration newConfig)
        {

            //Begin Pair lại nếu xài network pay
            // clear token để force pair
            Properties.Settings.Default.pairingAuthToken = "";
            Properties.Settings.Default.Save();

        }

        private void FormSetting_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Xử lý không bị cảm giác giật do xài Dispose() ngay nếu đóng thẳng
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000); // 3 giây

                try
                {
                    Core.ClearAndDisposeV2(panelContent);
                    Core.ClearMemory();

                    if (!this.IsDisposed)
                    {
                        this.Invoke((Action)(() => this.Dispose()));
                    }
                }
                catch { /* form đã dispose rồi thì thôi */ }
            });
        }

        private static bool _isRestarting;
        private void btnLogout_Click(object sender, EventArgs e)
        {
            // 1) Lưu config
            //Save To File
            string configs = "";
            configs += "store_code: " + ConfigLocalHelper.GetStoreConfig("store_code", "") + "\n";
            configs += "store_id: \n";
            configs += "store_name: \n";
            configs += "timezone: \n";
            configs += "accessToken: \n";
            configs += "current_login: 0\n";

            Utilitys.SaveAllStoreConfig(configs);

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
