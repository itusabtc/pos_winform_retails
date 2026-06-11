using EcrHost_Trans_Demo;
using NailsChekin.Models.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace NailsChekin.Models.Payments
{
    public class P5LibUsbMode
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

        static FormMain Ins = null;

        int _connectionType = CONNECTION_TYPE_LAN;
        //ST_ECR_LAN_PAIRING_CALLBACK _lanPairCallback = new ST_ECR_LAN_PAIRING_CALLBACK();
        static ST_ECR_CONNECTION_CALLBACK _ecrConnectCallback = new ST_ECR_CONNECTION_CALLBACK();
        //ST_ECR_TRANS_CALLBACK _transCallback = new ST_ECR_TRANS_CALLBACK();
        //static ST_TERMINAL_INFO _curTerminalInfo;
        //static List<ST_TERMINAL_INFO> _lstStTerminalInfo = new List<ST_TERMINAL_INFO>();

        public P5LibUsbMode()
        {
            //Ins = this;
            _connectionType = CONNECTION_TYPE_USB;
            UInt32 ret = ECR_Init(CONNECTION_TYPE_USB);
        }

        public P5LibUsbMode(FormMain frmMain)
        {
            Ins = frmMain;
            _connectionType = CONNECTION_TYPE_USB;
            UInt32 ret = ECR_Init(CONNECTION_TYPE_USB);
        }

        public string InitUSBConnect()
        {
            try
            {
                UInt32 ret = ERR_ECR.ERR_ECR_SUCCESS;
                ret = ConnectUsb();

                if (ret != ERR_ECR.ERR_ECR_SUCCESS)
                {
                    //Logger.Info($"btnConnect_Click Errorcode:{ret.ToString("X2")}", Form1.Ins.tbLog);
                    Console.WriteLine($"btnConnect_Click Errorcode:{ret.ToString("X2")}");
                    LogHelper.SaveLOG_CodePay(ret.ToString("X2"), "InitUSBConnect Error");

                    Ins.UpdateCreditDeviceStatus("CodePay USB Connect Error: " + ret.ToString("X2"));
                    return "CodePay USB Connect Error: " + ret.ToString("X2");
                }

                return "OK";
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_CodePay(ex.Message, "InitUSBConnect Exception");
                return "CodePay USB Connect Error: " + ex.Message;
            }   
        }

        public static void ResetToInitialState()
        {
            try
            {
                ECRUSB_Disconnect();
                _ = Task.Delay(500); 

                // Reset callback
                _ecrConnectCallback = new ST_ECR_CONNECTION_CALLBACK
                {
                    WsConnected = new onWsConnected(UsbConnected_Event),
                    WsDisconnected = new onWsDisconnected(UsbDisconnected_Event),
                    WsError = new onWsError(UsbError_Event)
                };
               
                LogHelper.SaveLOG_CodePay("USB callbacks reset", "ResetUsbCallback");

                _isReconnecting = false;
                Ins?.Invoke(new Action(() =>
                {
                    Ins.UpdateCreditDeviceStatus("USB device removed!");
                }));
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_CodePay(ex.Message, "ResetToInitialState Exception");
            }
        }

        static UInt32 ConnectUsb()
        {
            try
            {
                if (TRUE == ECRUSB_isConnected())
                {
                    LogHelper.SaveLOG_CodePay("ECRUSB_isConnected", "ConnectUsb Check Status");
                    return ERR_ECR.ERR_ECR_SUCCESS;
                }

                //Đảm bảo gắt hoàn toàn kết nối cũ
                ECRUSB_Disconnect();
                _ecrConnectCallback = new ST_ECR_CONNECTION_CALLBACK();

                _ecrConnectCallback.WsConnected = new onWsConnected(UsbConnected_Event);
                _ecrConnectCallback.WsDisconnected = new onWsDisconnected(UsbDisconnected_Event);
                _ecrConnectCallback.WsError = new onWsError(UsbError_Event);

                UInt32 ret = ECRUSB_Connect(ref _ecrConnectCallback);
                LogHelper.SaveLOG_CodePay(ret.ToString(), "ConnectUsb RET RESULT");

                if (ret == ERR_ECR.ERR_ECR_SUCCESS)
                {
                    // 👉 Gọi UsbWatcher sau khi kết nối thành công
                    //StartUsbWatcher();
                }

                return ret;
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_CodePay(ex.Message, "ConnectUsb Exception");
                return 0;
            }
        }

        static bool _isReconnecting = false;
        public static async void ReConnectUsb()
        {
            if (_isReconnecting)
            {
                LogHelper.SaveLOG_CodePay("Reconnect in progress...", "ReConnectUsb");
                return;
            }

            _isReconnecting = true;
            const int maxRetries = 3;
            int retry = 0;

            while (retry < maxRetries)
            {
                retry++;
                LogHelper.SaveLOG_CodePay($"Attempt {retry}", "ReConnectUsb");

                try
                {
                    // 👉 Rất quan trọng: gọi lại ECR_Init để re-initialize USB lib
                    UInt32 initRet = ECR_Init(CONNECTION_TYPE_USB);
                   
                    // Ngắt kết nối cũ (nếu còn)
                    ECRUSB_Disconnect();
                    await Task.Delay(800); // Đợi một chút để chắc chắn ngắt kết nối

                    //Reset trước khi khởi tạo lại
                    _ecrConnectCallback.WsConnected = null;
                    _ecrConnectCallback.WsDisconnected = null;
                    _ecrConnectCallback.WsError = null;

                    _ecrConnectCallback = new ST_ECR_CONNECTION_CALLBACK
                    {
                        WsConnected = new onWsConnected(UsbConnected_Event),
                        WsDisconnected = new onWsDisconnected(UsbDisconnected_Event),
                        WsError = new onWsError(UsbError_Event)
                    };

                    UInt32 ret = ECRUSB_Connect(ref _ecrConnectCallback);
                    LogHelper.SaveLOG_CodePay(ret.ToString(), "ReConnectUsb - ECRUSB_Connect RET RESULT");

                    if (ret == ERR_ECR.ERR_ECR_SUCCESS)
                    {
                        //StartUsbWatcher();
                        LogHelper.SaveLOG_CodePay("Reconnected successfully", "ReConnectUsb");
                        Ins?.UpdateCreditDeviceStatus("USB Reconnected successfully");
                        _isReconnecting = false;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.SaveLOG_CodePay(ex.ToString(), "ReConnectUsb Exception");
                }

                await Task.Delay(3000); // Đợi 3 giây trước lần thử tiếp theo
            }

            LogHelper.SaveLOG_CodePay("Reconnect failed after max retries", "ReConnectUsb");
            Ins?.UpdateCreditDeviceStatus("USB Reconnect failed after max retries");
            _isReconnecting = false;
        }

        public static UsbWatcher usbWatcher;
        public static void StartUsbWatcher()
        {
            if (usbWatcher == null)
            {
                usbWatcher = new UsbWatcher(UsbDisconnected_Event);
                usbWatcher.Start();
                Console.WriteLine("UsbWatcher started inside ConnectUsb.");
                LogHelper.SaveLOG_CodePay($"UsbWatcher started inside ConnectUsb.", "StartUsbWatcher");
            }
        }

        static void UsbConnected_Event()
        {
            //Logger.Info("The USB connection is successful.", Form1.Ins.tbLog);
            Console.WriteLine("The USB connection is successful.");
            LogHelper.SaveLOG_CodePay($"The USB connection is successful.", "UsbConnected_Event");

            Ins.UpdateCreditDeviceStatus("The USB connection is successful");
        }

        static void UsbDisconnected_Event()
        {
            //Logger.Info("The USB disconnection is successful.", Form1.Ins.tbLog);
            Console.WriteLine("The USB disconnection is successful.");
            LogHelper.SaveLOG_CodePay($"The USB disconnection is successful.", "UsbDisconnected_Event");

            ECRUSB_Disconnect();
            Ins.Invoke(new Action(() =>
            {
                Ins.UpdateCreditDeviceStatus("The USB disconnection is successful");
            }));

            //Reconnect
            //ReConnectUsb();
        }

        static void UsbError_Event(int dwErrCode, string pszErrMsg)
        {
            //Logger.Info($"UsbError_Event dwErrCode:{dwErrCode.ToString("X2")}!", Form1.Ins.tbLog);
            Console.WriteLine($"UsbError_Event dwErrCode:{dwErrCode.ToString("X2")}!");
            Ins.UpdateCreditDeviceStatus($"UsbError_Event dwErrCode:{dwErrCode.ToString("X2")}!");

            LogHelper.SaveLOG_CodePay("Error code: " + dwErrCode.ToString("X2") + " Message: " + pszErrMsg, "UsbError_Event");
        }

        static void TransSuccess_Event(string pszResponse)
        {
            try
            {
                //Logger.Info($"TransSuccess_Event success:{pszResponse}", Form1.Ins.tbLog);
                Console.WriteLine($"TransSuccess_Event success:{pszResponse}");
                Ins.CodePay_Process_OnMessage(pszResponse);

                LogHelper.SaveLOG_CodePay($"TransSuccess_Event success:{pszResponse}", "P5 TransSuccess_Event");
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_Crash(ex.Message, "P5 TransSuccess_Event Exception");
            }
        }

        static void TransError_Event(int dwErrCode, string pszErrMsg)
        {
            try
            {
                //Logger.Info($"TransError_Event dwErrCode:{dwErrCode.ToString("X2")}, {pszErrMsg}!", Form1.Ins.tbLog);
                Console.WriteLine($"TransError_Event dwErrCode:{dwErrCode.ToString("X2")}, {pszErrMsg}!");
                LogHelper.SaveLOG_CodePay($"TransError_Event dwErrCode:{dwErrCode.ToString("X2")}, {pszErrMsg}!", "P5 TransError_Event");
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_Crash(ex.Message, "P5 TransError_Event Exception");
            }
        }

        public string PayNow(string ticketId, string tip_amount, string amount, string note, bool repair_mode)
        {
            if (FALSE == ECRUSB_isConnected())
            {
                ReConnectUsb();
            }

            try
            {
                Ins._transCallback.TransSuccess = new onTransSuccess(TransSuccess_Event);
                Ins._transCallback.TransError = new onTransError(TransError_Event);

                // Construct json sale data
                PaymentRequestParams reqParams = new PaymentRequestParams();
                reqParams.app_id = Constants.codepay_app_id;
                reqParams.topic = EcrHost_Trans_Demo.Constants.PAYMENT_TOPIC;
                reqParams.request_id = "111111";

                reqParams.biz_data = new PaymentRequestParams.BizData();
                reqParams.biz_data.merchant_order_no = Utilitys.createRamdomKey();
                reqParams.biz_data.order_amount = amount;
                reqParams.biz_data.on_screen_signature = true;

                if (repair_mode)
                {
                    reqParams.biz_data.on_screen_tip = false;
                    reqParams.biz_data.tip_amount = "0";
                }
                else if ((!string.IsNullOrEmpty(tip_amount) && !tip_amount.Equals("0")) || note.StartsWith("CODEPAY_GIFT"))
                {
                    reqParams.biz_data.on_screen_tip = false;
                    reqParams.biz_data.tip_amount = tip_amount;
                }
                else
                {
                    reqParams.biz_data.on_screen_tip = true;
                }

                reqParams.biz_data.pay_scenario = "SWIPE_CARD";
                reqParams.biz_data.trans_type = EcrHost_Trans_Demo.Constants.TRANS_TYPE_SALE;

                string json = JsonConvert.SerializeObject(reqParams, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                UInt32 ret = ECR_DoTransaction(json, 120, ref Ins._transCallback);
                if (ret == ERR_ECR.ERR_ECR_SUCCESS)
                {
                    Console.WriteLine($"Sale:{json}");
                }
                else
                {
                    Console.WriteLine($"Sale ErrorCode:{ret.ToString("X2")}");
                    return "Error: " + ret.ToString("X2");
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }

            return "OK";
        }

        public string Void(string orig_merchant_order_no, string amount)
        {
            try
            {
                // refund
                Ins._transCallback.TransSuccess = new onTransSuccess(TransSuccess_Event);
                Ins._transCallback.TransError = new onTransError(TransError_Event);

                // Construct json refund data
                PaymentRequestParams reqParams = new PaymentRequestParams();
                reqParams.orig_merchant_order_no = orig_merchant_order_no;
                reqParams.order_amount = amount;
                reqParams.app_id = Constants.codepay_app_id;
                reqParams.topic = EcrHost_Trans_Demo.Constants.PAYMENT_TOPIC;
                reqParams.request_id = "111111";
                //reqParams.voice_data = new PaymentRequestParams.VoiceData();
                //reqParams.voice_data.content = "CodePay Register Received a new order";
                //reqParams.voice_data.content_locale = "en-US";
                reqParams.biz_data = new PaymentRequestParams.BizData();
                reqParams.biz_data.merchant_order_no = Utilitys.createRamdomKey();
                reqParams.biz_data.order_amount = amount;
                //reqParams.biz_data.on_screen_tip = false;
                //reqParams.biz_data.print_receipt = 0;
                reqParams.biz_data.pay_scenario = "SWIPE_CARD";
                reqParams.biz_data.trans_type = EcrHost_Trans_Demo.Constants.TRANS_TYPE_VOID;

                string json = JsonConvert.SerializeObject(reqParams, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                UInt32 ret = ECR_DoTransaction(json, 120, ref Ins._transCallback);
                if (ret == ERR_ECR.ERR_ECR_SUCCESS)
                {
                    //Logger.Info($"Refund:{json}", Form1.Ins.tbLog);
                    Console.WriteLine($"Void:{json}");
                }
                else
                {
                    //Logger.Info($"Refund ErrorCode:{ret.ToString("X2")}", Form1.Ins.tbLog);
                    Console.WriteLine($"Void ErrorCode:{ret.ToString("X2")}");
                    return "Error: " + $"Void ErrorCode:{ret.ToString("X2")}";
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }

            return "OK";
        }

        public string Refund(string orig_merchant_order_no, string amount, string tip_amount)
        {
            try
            {
                // refund
                Ins._transCallback.TransSuccess = new onTransSuccess(TransSuccess_Event);
                Ins._transCallback.TransError = new onTransError(TransError_Event);

                // Construct json refund data
                PaymentRequestParams reqParams = new PaymentRequestParams();
                reqParams.orig_merchant_order_no = orig_merchant_order_no;
                reqParams.order_amount = amount;
                reqParams.tip_amount = tip_amount;
                reqParams.app_id = Constants.codepay_app_id;
                reqParams.topic = EcrHost_Trans_Demo.Constants.PAYMENT_TOPIC;
                reqParams.request_id = "111111";
                //reqParams.voice_data = new PaymentRequestParams.VoiceData();
                //reqParams.voice_data.content = "CodePay Register Received a new order";
                //reqParams.voice_data.content_locale = "en-US";
                reqParams.biz_data = new PaymentRequestParams.BizData();
                reqParams.biz_data.merchant_order_no = Utilitys.createRamdomKey();
                reqParams.biz_data.order_amount = amount;
                //reqParams.biz_data.on_screen_tip = false;
                //reqParams.biz_data.print_receipt = 0;
                reqParams.biz_data.pay_scenario = "SWIPE_CARD";
                reqParams.biz_data.trans_type = EcrHost_Trans_Demo.Constants.TRANS_TYPE_REFUND;

                string json = JsonConvert.SerializeObject(reqParams, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                UInt32 ret = ECR_DoTransaction(json, 120, ref Ins._transCallback);
                if (ret == ERR_ECR.ERR_ECR_SUCCESS)
                {
                    //Logger.Info($"Refund:{json}", Form1.Ins.tbLog);
                    Console.WriteLine($"Refund:{json}");
                }
                else
                {
                    //Logger.Info($"Refund ErrorCode:{ret.ToString("X2")}", Form1.Ins.tbLog);
                    Console.WriteLine($"Refund ErrorCode:{ret.ToString("X2")}");
                    return "Error: " + $"Refund ErrorCode:{ret.ToString("X2")}";
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }

            return "OK";
        }

        public string Cancel(string merchant_order_no)
        {
            try
            {
                // 构造json格式交易数据json
                PaymentRequestParams reqParams = new PaymentRequestParams();
                reqParams.app_id = Constants.codepay_app_id;
                reqParams.topic = EcrHost_Trans_Demo.Constants.CLOSE_TOPIC;
                reqParams.request_id = "111111";
                reqParams.biz_data = new PaymentRequestParams.BizData();
                reqParams.biz_data.merchant_order_no = merchant_order_no; //"123" + DateTime.Now.ToString("yyyyMMddHHmmss");
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
                    //Logger.Info($"Cancel:{json}", Form1.Ins.tbLog);
                    Console.WriteLine($"Cancel:{json}");
                }
                else
                {
                    //Logger.Info($"Cancel ErrorCode:{ret.ToString("X2")}", Form1.Ins.tbLog);
                    return "Error: Cancel ErrorCode " + ret.ToString("X2");
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }

            return "OK";
        }

        public string SaleTest()
        {
            //if (string.IsNullOrEmpty(tbOrderNumber.Text))
            //{
            //    MessageBox.Show("Merchant Order No cannot be empty！");
            //    return;
            //}
            //if (string.IsNullOrEmpty(tbAmount.Text))
            //{
            //    MessageBox.Show("Amount cannot be empty！");
            //    return;
            //}
            //if (string.IsNullOrEmpty(tbAppId.Text))
            //{
            //    MessageBox.Show("App Id cannot be empty！");
            //    return;
            //}

            // sale
            Ins._transCallback.TransSuccess = new onTransSuccess(TransSuccess_Event);
            Ins._transCallback.TransError = new onTransError(TransError_Event);

            // Construct json sale data
            PaymentRequestParams reqParams = new PaymentRequestParams();
            reqParams.app_id = "wz6012822ca2f1as78"; //tbAppId.Text;
            reqParams.topic = EcrHost_Trans_Demo.Constants.PAYMENT_TOPIC;
            reqParams.request_id = "111111";
            //reqParams.voice_data = new PaymentRequestParams.VoiceData();
            //reqParams.voice_data.content = "CodePay Register Received a new order";
            //reqParams.voice_data.content_locale = "en-US";
            reqParams.biz_data = new PaymentRequestParams.BizData();
            reqParams.biz_data.merchant_order_no = "123" + DateTime.Now.ToString("yyyyMMddHHmmss");
            reqParams.biz_data.order_amount = "11";
            //reqParams.biz_data.on_screen_tip = false;
            //reqParams.biz_data.print_receipt = 0;
            reqParams.biz_data.pay_scenario = "SWIPE_CARD";
            reqParams.biz_data.trans_type = EcrHost_Trans_Demo.Constants.TRANS_TYPE_SALE;

            string json = JsonConvert.SerializeObject(reqParams, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            UInt32 ret = ECR_DoTransaction(json, 60, ref Ins._transCallback);
            if (ret == ERR_ECR.ERR_ECR_SUCCESS)
            {
                //Logger.Info($"Sale:{json}", Form1.Ins.tbLog);
                Console.WriteLine($"Sale:{json}");
            }
            else
            {
                //Logger.Info($"Sale ErrorCode:{ret.ToString("X2")}", Form1.Ins.tbLog);
                Console.WriteLine($"Sale ErrorCode:{ret.ToString("X2")}");
            }

            return "OK";
        }

        public static void CleanupStaticResources()
        {
            try
            {
                // 1. Ngắt kết nối USB
                ECRUSB_Disconnect();
                _ = Task.Delay(1500);  // Bắt buộc delay cho Windows/Driver giải phóng

                // 2. Dọn delegate trong callback
                //_ecrConnectCallback = new ST_ECR_CONNECTION_CALLBACK
                //{
                //    WsConnected = null,
                //    WsDisconnected = null,
                //    WsError = null
                //};

                _ecrConnectCallback.WsConnected = null;
                _ecrConnectCallback.WsDisconnected = null;
                _ecrConnectCallback.WsError = null;
                _ecrConnectCallback = new ST_ECR_CONNECTION_CALLBACK(); // reset lại sạch

                // 3. Dừng và giải phóng UsbWatcher nếu đang chạy
                if (usbWatcher != null)
                {
                    usbWatcher.Stop();
                    //usbWatcher.Dispose();
                    usbWatcher = null;
                }

                // 4. Reset cờ và các biến khác
                _isReconnecting = false;

                Ins?.Invoke(new Action(() =>
                {
                    Ins.UpdateCreditDeviceStatus("USB device removed.");
                }));
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_CodePay(ex.Message, "CleanupStaticResources Exception");
            }
        }

    }
}
