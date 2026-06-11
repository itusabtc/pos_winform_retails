using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using WiseShare;

namespace EcrHost_Trans_Demo
{

    public partial class Form1 : Form
    {
        [DllImport(@"WiseSdk_EcrHost_Payment_X86.dll", EntryPoint = "ECR_Init", CallingConvention = CallingConvention.StdCall)]
        extern static UInt32 ECR_Init(int dwConnectionType);

        [DllImport(@"WiseSdk_EcrHost_Payment_X86.dll", EntryPoint = "ECRLAN_WaitPairing", CallingConvention = CallingConvention.StdCall)]
        extern static UInt32 ECRLAN_WaitPairing(ref ST_ECR_LAN_PAIRING_CALLBACK pstPairingCallback);

        [DllImport(@"WiseSdk_EcrHost_Payment_X86.dll", EntryPoint = "ECRLAN_StopWaitPairing", CallingConvention = CallingConvention.StdCall)]
        extern static UInt32 ECRLAN_StopWaitPairing();

        [DllImport(@"WiseSdk_EcrHost_Payment_X86.dll", EntryPoint = "ECRLAN_Connect", CallingConvention = CallingConvention.StdCall)]
        extern static UInt32 ECRLAN_Connect(ref ST_TERMINAL_INFO pstTerminalInfo, int dwWaitingSeconds, ref ST_ECR_CONNECTION_CALLBACK pstConnectionCallback);

        [DllImport(@"WiseSdk_EcrHost_Payment_X86.dll", EntryPoint = "ECRLAN_Disconnect", CallingConvention = CallingConvention.StdCall)]
        extern static UInt32 ECRLAN_Disconnect();

        [DllImport(@"WiseSdk_EcrHost_Payment_X86.dll", EntryPoint = "ECRLAN_isConnected", CallingConvention = CallingConvention.StdCall)]
        extern static int ECRLAN_isConnected(); // return 1 connnect

        [DllImport(@"WiseSdk_EcrHost_Payment_X86.dll", EntryPoint = "ECRUSB_Connect", CallingConvention = CallingConvention.StdCall)]
        extern static UInt32 ECRUSB_Connect(ref ST_ECR_CONNECTION_CALLBACK pstConnectionCallback);

        [DllImport(@"WiseSdk_EcrHost_Payment_X86.dll", EntryPoint = "ECRUSB_Disconnect", CallingConvention = CallingConvention.StdCall)]
        extern static UInt32 ECRUSB_Disconnect();

        [DllImport(@"WiseSdk_EcrHost_Payment_X86.dll", EntryPoint = "ECRUSB_isConnected", CallingConvention = CallingConvention.StdCall)]
        extern static int ECRUSB_isConnected();

        [DllImport(@"WiseSdk_EcrHost_Payment_X86.dll", EntryPoint = "ECRLAN_AcceptPairing", CallingConvention = CallingConvention.StdCall)]
        extern static UInt32 ECRLAN_AcceptPairing(ref ST_TERMINAL_INFO pstTerminalInfo);

        [DllImport(@"WiseSdk_EcrHost_Payment_X86.dll", EntryPoint = "ECRLAN_RejectPairing", CallingConvention = CallingConvention.StdCall)]
        extern static UInt32 ECRLAN_RejectPairing(ref ST_TERMINAL_INFO pstTerminalInfo);

        [DllImport(@"WiseSdk_EcrHost_Payment_X86.dll", EntryPoint = "ECRLAN_GetPairList", CallingConvention = CallingConvention.StdCall)]
        extern static UInt32 ECRLAN_GetPairList(IntPtr pstTerminalInfo, ref int pdwNum);

        [DllImport(@"WiseSdk_EcrHost_Payment_X86.dll", EntryPoint = "ECRLAN_DeletePairList", CallingConvention = CallingConvention.StdCall)]
        extern static UInt32 ECRLAN_DeletePairList(ref ST_TERMINAL_INFO pstTerminalInfo, int dwFlag);

        [DllImport(@"WiseSdk_EcrHost_Payment_X86.dll", EntryPoint = "ECRLAN_Unpair", CallingConvention = CallingConvention.StdCall)]
        extern static UInt32 ECRLAN_Unpair(ref ST_TERMINAL_INFO pstTerminalInfo);

        [DllImport(@"WiseSdk_EcrHost_Payment_X86.dll", EntryPoint = "ECR_DoTransaction", CallingConvention = CallingConvention.StdCall)]
        extern static UInt32 ECR_DoTransaction(string pszRequestMessage, int dwWaitingSeconds, ref ST_ECR_TRANS_CALLBACK pstTransCallback);

        [DllImport(@"WiseSdk_EcrHost_Payment_X86.dll", EntryPoint = "ECR_CancelTransaction", CallingConvention = CallingConvention.StdCall)]
        extern static UInt32 ECR_CancelTransaction(string pszRequestMessage, int dwWaitingSeconds);

        [DllImport(@"WiseSdk_EcrHost_Payment_X86.dll", EntryPoint = "ECR_QueryTransaction", CallingConvention = CallingConvention.StdCall)]
        extern static UInt32 ECR_QueryTransaction(string pszRequestMessage, int dwWaitingSeconds, ref ST_ECR_TRANS_CALLBACK pstTransCallback);


        // C Header file structure
        [StructLayout(LayoutKind.Sequential)]
        public struct ST_TERMINAL_INFO
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
            public string szTerminalMacAddr;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 65)]
            public string szTerminalName;
            public byte bTerminalNameLen;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 65)]
            public string szAliasName;
            public byte bAliasNameLen;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
            public string szTerminalIp;
            public byte bTerminalIpLen;

            public UInt32 dwPort;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
            public string szTerminalSn;
            public byte bTerminalSnLen;
        }

        // C header file callback function
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void onRequestPairing(ref ST_TERMINAL_INFO pstTerminalInfo);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void onRequestUnpairing(ref ST_TERMINAL_INFO pstTerminalInfo);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void onWsConnected();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void onWsDisconnected();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void onWsError(int dwErrCode, string pszErrMsg);


        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void onTransSuccess(string pszResponse);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void onTransError(int dwErrCode, string pszErrMsg);


        public struct ST_ECR_LAN_PAIRING_CALLBACK
        {
            public onRequestPairing RequestPair;
            public onRequestUnpairing RequestUnpair;
        }


        public struct ST_ECR_CONNECTION_CALLBACK
        {
            public onWsConnected WsConnected;
            public onWsDisconnected WsDisconnected;
            public onWsError WsError;
        }

        public struct ST_ECR_TRANS_CALLBACK
        {
            public onTransSuccess TransSuccess;
            public onTransError TransError;
        }

        enum ConnectState
        {
            UNPAIRED = 0,
            PAIRED,
            CONNECT
        }
        private const int TRUE = 1;
        private const int FALSE = 0;
        private const int CONNECTION_TYPE_USB = 1;
        private const int CONNECTION_TYPE_LAN = 2;
        static Form1 Ins = null;
        int _connectionType = CONNECTION_TYPE_LAN;
        ST_ECR_LAN_PAIRING_CALLBACK _lanPairCallback = new ST_ECR_LAN_PAIRING_CALLBACK();
        static ST_ECR_CONNECTION_CALLBACK _ecrConnectCallback = new ST_ECR_CONNECTION_CALLBACK();
        ST_ECR_TRANS_CALLBACK _transCallback = new ST_ECR_TRANS_CALLBACK();
        static ST_TERMINAL_INFO _curTerminalInfo;
        static List<ST_TERMINAL_INFO> _lstStTerminalInfo = new List<ST_TERMINAL_INFO>();

        public Form1()
        {
            InitializeComponent();
            Ins = this;
            _connectionType = CONNECTION_TYPE_LAN;
            UInt32 ret = ECR_Init(_connectionType);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _lstStTerminalInfo = GetPairArray();

            if (_lstStTerminalInfo.Count <= 0)
            {
                Logger.Info("Pairlist is empty!", Form1.Ins.tbLog);
                SetConnectStateCtrl(ConnectState.UNPAIRED);
            }
            else
            {
                for (int i = 0; i < _lstStTerminalInfo.Count; i++)
                {
                    Logger.Info($"Pairlist{i + 1}: {_lstStTerminalInfo[i].szTerminalName}  {_lstStTerminalInfo[i].szTerminalIp}", Form1.Ins.tbLog);
                }
                SetConnectStateCtrl(ConnectState.PAIRED);
            }
        }


        // C# Callback implementation
        static void RequestPairing_Event(ref ST_TERMINAL_INFO pstTerminalInfo)
        {
            string strTerminalName = pstTerminalInfo.szTerminalName;
            Logger.Info($"RequestPairing_Event:{pstTerminalInfo.szTerminalIp}:{pstTerminalInfo.dwPort}", Form1.Ins.tbLog);
            DialogResult result = DialogResult.None;// = MessageBox.Show($"Are you pair the {pstTerminalInfo.szTerminalName} device?", "Tips", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            Form1.Ins.Invoke(new MethodInvoker(delegate ()
            {
                result = MessageBox.Show($"The device \"{strTerminalName}\" is requesting to pair. Accept or not?", "Tips", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }));


            UInt32 ret = ERR_ECR.ERR_ECR_SUCCESS;
            if (result == DialogResult.OK)
            {
                ret = ECRLAN_AcceptPairing(ref pstTerminalInfo);
                ret = ConnectWsServer();
            }
            else
            {
                ret = ECRLAN_RejectPairing(ref pstTerminalInfo);
            }
        }

        static void RequestUnpairing_Event(ref ST_TERMINAL_INFO pstTerminalInfo)
        {
            Logger.Info($"RequestUnpairing_Event:{pstTerminalInfo.szTerminalName}", Form1.Ins.tbLog);

            string strTerminalName = pstTerminalInfo.szTerminalName;
            Form1.Ins.Invoke(new MethodInvoker(delegate ()
            {
                MessageBox.Show($"Register unpair the {strTerminalName} device");
            }));

            // Set control connect status
            _lstStTerminalInfo = GetPairArray();
            if (_lstStTerminalInfo.Count <= 0)
                SetConnectStateCtrl(ConnectState.UNPAIRED);
            else
                SetConnectStateCtrl(ConnectState.PAIRED);

            // Disconnect
            System.Threading.Thread.Sleep(100);
            ECRLAN_Disconnect();
        }

        static void WsConnected_Event()
        {
            Logger.Info("The Wi-Fi connection is successful.", Form1.Ins.tbLog);
            SetConnectStateCtrl(ConnectState.CONNECT);
        }

        static void WsDisconnected_Event()
        {
            Logger.Info("The Wi-Fi disconnection is successful.", Form1.Ins.tbLog);

            _lstStTerminalInfo = GetPairArray();
            if (_lstStTerminalInfo.Count <= 0)
                SetConnectStateCtrl(ConnectState.UNPAIRED);
            else
                SetConnectStateCtrl(ConnectState.PAIRED);

        }

        static void WsError_Event(int dwErrCode, string pszErrMsg)
        {
            Logger.Info($"WsError_Event dwErrCode:{dwErrCode.ToString("X2")}!", Form1.Ins.tbLog);
        }

        static void UsbConnected_Event()
        {
            Logger.Info("The USB connection is successful.", Form1.Ins.tbLog);
        }

        static void UsbDisconnected_Event()
        {
            Logger.Info("The USB disconnection is successful.", Form1.Ins.tbLog);
        }

        static void UsbError_Event(int dwErrCode, string pszErrMsg)
        {
            Logger.Info($"UsbError_Event dwErrCode:{dwErrCode.ToString("X2")}!", Form1.Ins.tbLog);
        }


        static void TransSuccess_Event(string pszResponse)
        {
            Logger.Info($"TransSuccess_Event success:{pszResponse}", Form1.Ins.tbLog);
        }

        static void TransError_Event(int dwErrCode, string pszErrMsg)
        {
            Logger.Info($"TransError_Event dwErrCode:{dwErrCode.ToString("X2")}, {pszErrMsg}!", Form1.Ins.tbLog);
        }

        static UInt32 ConnectWsServer()
        {
            if (TRUE == ECRLAN_isConnected())
                return ERR_ECR.ERR_ECR_SUCCESS;

            _lstStTerminalInfo = GetPairArray();

            if (_lstStTerminalInfo.Count <= 0)
            {
                Logger.Info("Pairlist is empty!", Form1.Ins.tbLog);
                return ERR_ECR.ERR_ECR_SUCCESS;
            }

            _ecrConnectCallback.WsConnected = new onWsConnected(WsConnected_Event);
            _ecrConnectCallback.WsDisconnected = new onWsDisconnected(WsDisconnected_Event);
            _ecrConnectCallback.WsError = new onWsError(WsError_Event);

            _curTerminalInfo = _lstStTerminalInfo[0];
            UInt32 ret = ECRLAN_Connect(ref _curTerminalInfo, 60, ref _ecrConnectCallback);
            return ret;
        }

        static UInt32 ConnectUsb()
        {
            if (TRUE == ECRUSB_isConnected())
            {
                return ERR_ECR.ERR_ECR_SUCCESS;
            }

            _ecrConnectCallback.WsConnected = new onWsConnected(UsbConnected_Event);
            _ecrConnectCallback.WsDisconnected = new onWsDisconnected(UsbDisconnected_Event);
            _ecrConnectCallback.WsError = new onWsError(UsbError_Event);

            UInt32 ret = ECRUSB_Connect(ref _ecrConnectCallback);

            return ret;
        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            UInt32 ret = ECR_Init(CONNECTION_TYPE_LAN);

            _lanPairCallback.RequestPair = new onRequestPairing(RequestPairing_Event);
            _lanPairCallback.RequestUnpair = new onRequestUnpairing(RequestUnpairing_Event);
            ret = ECRLAN_WaitPairing(ref _lanPairCallback);
            if (ret == ERR_ECR.ERR_ECR_SUCCESS)
            {
                Logger.Info("Start mdns service, Start websocket service", Form1.Ins.tbLog);
            }
            else if (ERR_ECR.ERR_ECR_WAIT_PAIRING == ret)
            {
                Logger.Info("ECRLAN_WaitPairing is already running", Form1.Ins.tbLog);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            UInt32 ret = ERR_ECR.ERR_ECR_SUCCESS;
            if (_connectionType == CONNECTION_TYPE_LAN)
                ret = ConnectWsServer();
            else
                ret = ConnectUsb();

            if (ret != ERR_ECR.ERR_ECR_SUCCESS)
            {
                Logger.Info($"btnConnect_Click Errorcode:{ret.ToString("X2")}", Form1.Ins.tbLog);
            }
        }

        private void Disconnect_Click(object sender, EventArgs e)
        {
            UInt32 ret = ERR_ECR.ERR_ECR_SUCCESS;
            if (_connectionType == CONNECTION_TYPE_LAN)
                ret = ECRLAN_Disconnect();
            else
                ret = ECRUSB_Disconnect();

            if (ret != ERR_ECR.ERR_ECR_SUCCESS && e != null)
            {
                Logger.Info($"Disconnect_Click Errorcode:{ret.ToString("X2")}", Form1.Ins.tbLog);
            }
        }

        private void btnUnpair_Click(object sender, EventArgs e)
        {
            _lstStTerminalInfo = GetPairArray();

            if (_lstStTerminalInfo.Count <= 0)
            {
                Logger.Info("Pairlist is empty!", Form1.Ins.tbLog);
                return;
            }

            // Unpair
            _curTerminalInfo = _lstStTerminalInfo[0];
            ECRLAN_Unpair(ref _curTerminalInfo);

            // Set control connect status
            _lstStTerminalInfo = GetPairArray();
            if (_lstStTerminalInfo.Count <= 0)
                SetConnectStateCtrl(ConnectState.UNPAIRED);
            else
                SetConnectStateCtrl(ConnectState.PAIRED);

            // Disconnect
            System.Threading.Thread.Sleep(100);
            ECRLAN_Disconnect();
        }


        private void radUsb_CheckedChanged(object sender, EventArgs e)
        {
            Disconnect_Click(null, null);
            _connectionType = CONNECTION_TYPE_USB;
            UInt32 ret = ECR_Init(CONNECTION_TYPE_USB);
            ChangeCtrlVisible(CONNECTION_TYPE_USB);
        }

        private void radWifi_CheckedChanged(object sender, EventArgs e)
        {
            Disconnect_Click(null, null);
            _connectionType = CONNECTION_TYPE_LAN;
            UInt32 ret = ECR_Init(CONNECTION_TYPE_LAN);
            ChangeCtrlVisible(CONNECTION_TYPE_LAN);
        }


        private void ChangeCtrlVisible(int dwConnectionType)
        {
            if (CONNECTION_TYPE_USB == dwConnectionType)
            {
                btnStart.Visible = false;
                btnUnpair.Visible = false;
                btnConnectState.Visible = false;
            }
            else
            {
                btnStart.Visible = true;
                btnUnpair.Visible = true;

                // Set control connect status
                _lstStTerminalInfo = GetPairArray();
                if (_lstStTerminalInfo.Count <= 0)
                    SetConnectStateCtrl(ConnectState.UNPAIRED);
                else
                    SetConnectStateCtrl(ConnectState.PAIRED);
            }
        }

        private void btnSale_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbOrderNumber.Text))
            {
                MessageBox.Show("Merchant Order No cannot be empty！");
                return;
            }
            if (string.IsNullOrEmpty(tbAmount.Text))
            {
                MessageBox.Show("Amount cannot be empty！");
                return;
            }
            if (string.IsNullOrEmpty(tbAppId.Text))
            {
                MessageBox.Show("App Id cannot be empty！");
                return;
            }
            
            // sale
            _transCallback.TransSuccess = new onTransSuccess(TransSuccess_Event);
            _transCallback.TransError = new onTransError(TransError_Event);

            // Construct json sale data
            PaymentRequestParams reqParams = new PaymentRequestParams();
            reqParams.app_id = tbAppId.Text;
            reqParams.topic = Constants.PAYMENT_TOPIC;
            reqParams.request_id = "111111";
            //reqParams.voice_data = new PaymentRequestParams.VoiceData();
            //reqParams.voice_data.content = "CodePay Register Received a new order";
            //reqParams.voice_data.content_locale = "en-US";
            reqParams.biz_data = new PaymentRequestParams.BizData();
            reqParams.biz_data.merchant_order_no = tbOrderNumber.Text;// "123" + DateTime.Now.ToString("yyyyMMddHHmmss");
            reqParams.biz_data.order_amount = tbAmount.Text;//"11";
            //reqParams.biz_data.on_screen_tip = false;
            //reqParams.biz_data.print_receipt = 0;
            reqParams.biz_data.pay_scenario = "SWIPE_CARD";
            reqParams.biz_data.trans_type = Constants.TRANS_TYPE_SALE;

            string json = JsonConvert.SerializeObject(reqParams, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            UInt32 ret = ECR_DoTransaction(json, 60, ref _transCallback);
            if (ret == ERR_ECR.ERR_ECR_SUCCESS)
            {
                Logger.Info($"Sale:{json}", Form1.Ins.tbLog);
            }
            else
            {
                Logger.Info($"Sale ErrorCode:{ret.ToString("X2")}", Form1.Ins.tbLog);
            }
        }

        private void btnRefund_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbOrderNumber.Text))
            {
                MessageBox.Show("Merchant Order No cannot be empty！");
                return;
            }
            if (string.IsNullOrEmpty(tbAmount.Text))
            {
                MessageBox.Show("Amount cannot be empty！");
                return;
            }
            if (string.IsNullOrEmpty(tbAppId.Text))
            {
                MessageBox.Show("App Id cannot be empty！");
                return;
            }

            // refund
            _transCallback.TransSuccess = new onTransSuccess(TransSuccess_Event);
            _transCallback.TransError = new onTransError(TransError_Event);

            // Construct json refund data
            PaymentRequestParams reqParams = new PaymentRequestParams();
            reqParams.orig_merchant_order_no = tbOrderNumber.Text;
            reqParams.order_amount = tbAmount.Text;
            reqParams.app_id = tbAppId.Text;
            reqParams.topic = Constants.PAYMENT_TOPIC;
            reqParams.request_id = "111111";
            //reqParams.voice_data = new PaymentRequestParams.VoiceData();
            //reqParams.voice_data.content = "CodePay Register Received a new order";
            //reqParams.voice_data.content_locale = "en-US";
            reqParams.biz_data = new PaymentRequestParams.BizData();
            reqParams.biz_data.merchant_order_no = tbOrderNumber.Text;//"123" + DateTime.Now.ToString("yyyyMMddHHmmss");
            reqParams.biz_data.order_amount = tbAmount.Text;//"11";
            //reqParams.biz_data.on_screen_tip = false;
            //reqParams.biz_data.print_receipt = 0;
            reqParams.biz_data.pay_scenario = "SWIPE_CARD";
            reqParams.biz_data.trans_type = Constants.TRANS_TYPE_REFUND;

            string json = JsonConvert.SerializeObject(reqParams, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            UInt32 ret = ECR_DoTransaction(json, 60, ref _transCallback);
            if (ret == ERR_ECR.ERR_ECR_SUCCESS)
            {
                Logger.Info($"Refund:{json}", Form1.Ins.tbLog);
            }
            else
            {
                Logger.Info($"Refund ErrorCode:{ret.ToString("X2")}", Form1.Ins.tbLog);
            }
        }

        private void btnQueryTransaction_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbOrderNumber.Text))
            {
                MessageBox.Show("Merchant Order No cannot be empty！");
                return;
            }
            if (string.IsNullOrEmpty(tbAppId.Text))
            {
                MessageBox.Show("App Id cannot be empty！");
                return;
            }

            // query
            //_transCallback = new ST_ECR_TRANS_CALLBACK();
            _transCallback.TransSuccess = new onTransSuccess(TransSuccess_Event);
            _transCallback.TransError = new onTransError(TransError_Event);

            // Construct json query data
            PaymentRequestParams reqParams = new PaymentRequestParams();
            reqParams.app_id = tbAppId.Text;
            reqParams.topic = Constants.QUERY_TOPIC;
            reqParams.request_id = "111111";

            reqParams.biz_data = new PaymentRequestParams.BizData();
            reqParams.biz_data.merchant_order_no = tbOrderNumber.Text;
            //reqParams.biz_data.orig_merchant_order_no = reqParams.biz_data.merchant_order_no;

            JsonSerializerSettings jsetting = new JsonSerializerSettings();
            jsetting.DefaultValueHandling = DefaultValueHandling.Ignore;
            jsetting.NullValueHandling = NullValueHandling.Ignore;
            string json = JsonConvert.SerializeObject(reqParams, jsetting);
            UInt32 ret = ECR_QueryTransaction(json, 60, ref _transCallback);
            if (ret == ERR_ECR.ERR_ECR_SUCCESS)
            {
                Logger.Info($"Query:{json}", Form1.Ins.tbLog);
            }
            else
            {
                Logger.Info($"Query ErrorCode:{ret.ToString("X2")}", Form1.Ins.tbLog);
            }
        }

        private void btnCancelTransaction_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbOrderNumber.Text))
            {
                MessageBox.Show("Merchant Order No cannot be empty！");
                return;
            }
            if (string.IsNullOrEmpty(tbAppId.Text))
            {
                MessageBox.Show("App Id cannot be empty！");
                return;
            }

            // 构造json格式交易数据json
            PaymentRequestParams reqParams = new PaymentRequestParams();
            reqParams.app_id = tbAppId.Text;
            reqParams.topic = Constants.CLOSE_TOPIC;
            reqParams.request_id = "111111";
            reqParams.biz_data = new PaymentRequestParams.BizData();
            reqParams.biz_data.merchant_order_no = tbOrderNumber.Text; //"123" + DateTime.Now.ToString("yyyyMMddHHmmss");
            //reqParams.biz_data.orig_merchant_order_no = tbOrderNumber.Text;
            //reqParams.biz_data.confirm_on_terminal = false;
            //reqParams.biz_data.trans_type = Constants.TRANS_TYPE_VOID;

            JsonSerializerSettings jsetting = new JsonSerializerSettings();
            jsetting.DefaultValueHandling = DefaultValueHandling.Ignore;
            jsetting.NullValueHandling = NullValueHandling.Ignore;
            string json = JsonConvert.SerializeObject(reqParams, jsetting);
            UInt32 ret = ECR_CancelTransaction(json, 60);
            if (ret == ERR_ECR.ERR_ECR_SUCCESS)
            {
                Logger.Info($"Cancel:{json}", Form1.Ins.tbLog);
            }
            else
            {
                Logger.Info($"Cancel ErrorCode:{ret.ToString("X2")}", Form1.Ins.tbLog);
            }
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            tbLog.Clear();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            ECRLAN_StopWaitPairing();
            ECRLAN_Disconnect();
        }

        private static List<ST_TERMINAL_INFO> GetPairArray()
        {
            ST_TERMINAL_INFO[] curTerminalInfo = new ST_TERMINAL_INFO[20];
            int curNum = 20;
            IntPtr ptrTerminals = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(ST_TERMINAL_INFO)) * 20);
            UInt32 ret = ECRLAN_GetPairList(ptrTerminals, ref curNum);
            for (int i = 0; i < curNum; i++)
            {
                IntPtr ptr = (IntPtr)((UInt32)ptrTerminals + i * Marshal.SizeOf(typeof(ST_TERMINAL_INFO)));
                curTerminalInfo[i] = (ST_TERMINAL_INFO)Marshal.PtrToStructure(ptr, typeof(ST_TERMINAL_INFO));
            }
            Marshal.FreeHGlobal(ptrTerminals);

            List<ST_TERMINAL_INFO> lstStTerminalInfo = new List<ST_TERMINAL_INFO>();
            for (int i = 0; i < curNum; i++)
            {
                lstStTerminalInfo.Add(curTerminalInfo[i]);
            }
            return lstStTerminalInfo;
        }

        private static void SetConnectStateCtrl(ConnectState connectState)
        {
            if (connectState == ConnectState.UNPAIRED)
            {
                Form1.Ins.btnConnectState.Invoke(new MethodInvoker(delegate ()
                {
                    Form1.Ins.btnConnectState.Visible = false;
                }));
            }
            else if (connectState == ConnectState.PAIRED)
            {
                Form1.Ins.btnConnectState.Invoke(new MethodInvoker(delegate ()
                {
                    Form1.Ins.btnConnectState.Text = Form1._lstStTerminalInfo[0].szTerminalName;
                    Form1.Ins.btnConnectState.BackColor = Color.Red;
                    Form1.Ins.btnConnectState.Visible = true;
                }));
            }
            else if (connectState == ConnectState.CONNECT)
            {
                Form1.Ins.btnConnectState.Invoke(new MethodInvoker(delegate ()
                {
                    Form1.Ins.btnConnectState.Text = Form1._lstStTerminalInfo[0].szTerminalName;
                    Form1.Ins.btnConnectState.BackColor = Color.LimeGreen;
                    Form1.Ins.btnConnectState.Visible = true;
                }));
            }
        }


    }
}
