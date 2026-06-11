using CloverExamplePOS;
using com.clover.remotepay.transport;
using NailsChekin.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NailsChekin.Models.Helper;
using NailsChekin.Popup;
using NailsChekin.Models;

namespace NailsChekin.Popup
{
    public partial class FormCloverConnectSetting : OverlayForm
    {
        CloverDeviceConfiguration selectedConfig;

        const String APPLICATION_ID = "com.clover.CloverExamplePOS:3.0.2";

        CloverDeviceConfiguration USBConfig = new USBCloverDeviceConfiguration("__deviceID__", APPLICATION_ID, false, 1);

        WebSocketCloverDeviceConfiguration WebSocketConfig = new WebSocketCloverDeviceConfiguration("wss://192.168.1.2:12345/remote_pay", APPLICATION_ID, false, 10, "Nails Solutions POS", "POS-3", Properties.Settings.Default.pairingAuthToken, null, null, null); // set the 3 delegates in the ctor

        PairingDeviceConfiguration.OnPairingCodeHandler pairingCodeHandler = null;
        PairingDeviceConfiguration.OnPairingSuccessHandler pairingSuccessHandler = null;
        PairingDeviceConfiguration.OnPairingStateHandler pairingStateHandler = null;

        public FormCloverConnectSetting(FormSetting tocover, PairingDeviceConfiguration.OnPairingCodeHandler pairingHandler, PairingDeviceConfiguration.OnPairingSuccessHandler successHandler, PairingDeviceConfiguration.OnPairingStateHandler stateHandler) : base(tocover)
        {
            pairingCodeHandler = pairingHandler;
            pairingSuccessHandler = successHandler;
            pairingStateHandler = stateHandler;
            WebSocketConfig.OnPairingCode = pairingHandler;
            WebSocketConfig.OnPairingSuccess = successHandler;
            WebSocketConfig.OnPairingState = stateHandler;
            NailsChekin.Properties.Settings.Default.Reload();
            WebSocketConfig.endpoint = NailsChekin.Properties.Settings.Default.lastWSEndpoint;
            if (WebSocketConfig.endpoint == null || "".Equals(WebSocketConfig.endpoint))
            {
                WebSocketConfig.endpoint = "wss://192.168.1.2:12345/remote_pay"; // just a default...
            }
            WebSocketConfig.pairingAuthToken = NailsChekin.Properties.Settings.Default.pairingAuthToken;

            InitializeComponent();
        }

        public FormCloverConnectSetting(FormSetting tocover) : base(tocover)
        {
            Properties.Settings.Default.Reload();
            WebSocketConfig.endpoint = Properties.Settings.Default.lastWSEndpoint;
            if (WebSocketConfig.endpoint == null || "".Equals(WebSocketConfig.endpoint))
            {
                WebSocketConfig.endpoint = "wss://192.168.1.2:12345/remote_pay"; // just a default...
            }

            Properties.Settings.Default.pairingAuthToken = "";  //Reset Pair token khi mở form setting
            Properties.Settings.Default.Save();

            WebSocketConfig.pairingAuthToken = Properties.Settings.Default.pairingAuthToken;

            InitializeComponent();
        }

        private void FormCloverConnectSetting_Load(object sender, EventArgs e)
        {
            // populate the combo box with connection types
            List<ConfigWrapper> dataSource = new List<ConfigWrapper>();

            dataSource.Add(new ConfigWrapper("Clover Connector USB", USBConfig));
            dataSource.Add(new ConfigWrapper("Network Pay Display", WebSocketConfig));

            ConnectionType.DataSource = dataSource;
            ConnectionType.DisplayMember = "Description";
            ConnectionType.ValueMember = "Config";

            //Setting
            if (!Constants.clover_connection_type.Contains("USB"))
                ConnectionType.SelectedValue = WebSocketConfig;
            else
                ConnectionType.SelectedValue = USBConfig;

            ConnectionType.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        protected class ConfigWrapper
        {
            public string Description { get; set; }
            public CloverDeviceConfiguration Config { get; set; }

            public ConfigWrapper(string description, CloverDeviceConfiguration config)
            {
                Description = description;
                Config = config;
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            selectedConfig = ((CloverDeviceConfiguration)ConnectionType.SelectedValue);
            //((FormMain)this.Owner).selectedConfig = selectedConfig;

            if (selectedConfig is WebSocketCloverDeviceConfiguration)
            {
                InitWebSocket();
            }
            else //USB Connect
            {
                ((FormSetting)this.Owner).SwitchCloverMode(selectedConfig);
                Close();
            }
        }

        private void InitWebSocket()
        {
            InputForm iform = new InputForm(this);
            iform.Title = "WebSocket Host Configuration";
            iform.Label = "Enter Device Endpoint(ex: wss://192.168.1.10:12345/remote_pay)";
            iform.Value = ((WebSocketCloverDeviceConfiguration)WebSocketConfig).endpoint;
            iform.FormClosed += WSForm_Closed;
            iform.Show();
        }

        private void WSForm_Closed(object sender, EventArgs e)
        {
            if (((InputForm)sender).Status == DialogResult.OK)
            {
                string endpoint = ((InputForm)sender).Value;
                if (endpoint.Length > 0)
                {
                    Properties.Settings.Default.lastWSEndpoint = endpoint;
                    Properties.Settings.Default.Save();

                    string pairingAuthToken = NailsChekin.Properties.Settings.Default.pairingAuthToken ?? "";

                    // ✅ Fix: dùng "as" tránh crash nếu Owner không phải FormMain
                    if (pairingAuthToken.Trim().Length <= 0)
                    {
                        var formMain = this.Owner as FormMain;
                        if (formMain != null && formMain.CurrentPairingToken?.Trim().Length > 0)
                            pairingAuthToken = formMain.CurrentPairingToken;
                    }

                    selectedConfig = new WebSocketCloverDeviceConfiguration(
                        endpoint, APPLICATION_ID, false, 10,
                        "Nails Solutions POS", "POS-3",
                        Properties.Settings.Default.pairingAuthToken,
                        pairingCodeHandler, pairingSuccessHandler, pairingStateHandler);
                }

                ((FormSetting)this.Owner).SwitchCloverMode(selectedConfig);
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
