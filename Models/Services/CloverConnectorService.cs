using System;
using System.Threading;
using System.Windows.Forms;
using com.clover.remotepay.sdk;
using com.clover.remotepay.transport;
using com.clover.sdk.v3.payments;
using NailsChekin.Models.Helper;

namespace NailsChekin.Models.Services
{
    /// <summary>
    /// Quản lý vòng đời kết nối Clover + phát sự kiện Pairing.
    /// Dùng kèm CloverListenerHelper (implements ICloverConnectorListener) để nhận các device events.
    /// </summary>
    public sealed class CloverConnectorService : IDisposable
    {
        private readonly SynchronizationContext _ui;

        const String APPLICATION_ID = "com.clover.CloverExamplePOS:3.0.2";
        CloverDeviceConfiguration USBConfig = new USBCloverDeviceConfiguration("__deviceID__", APPLICATION_ID, false, 1);
        WebSocketCloverDeviceConfiguration WebSocketConfig = new WebSocketCloverDeviceConfiguration("wss://192.168.1.2:12345/remote_pay", APPLICATION_ID, false, 1, "Nails Solutions POS", "POS-3", NailsChekin.Properties.Settings.Default.pairingAuthToken, null, null, null); // set the 3 delegates in the ctor

        public ICloverConnector Connector { get; private set; }
        public CloverDeviceConfiguration SelectedConfig { get; private set; }
        public CloverListenerHelper Listener { get; private set; }
        public bool IsConnected { get; private set; }

        // Lưu lại thông số để Reconnect
        public string Endpoint { get; private set; }
        public string ApplicationId { get; private set; }
        public string PosName { get; private set; }
        public string PosId { get; private set; }
        public string PairingAuthToken { get; private set; }

        // ===== Pairing events (moved từ FormMain) =====
        public event Action<string> PairingCodeReceived;          // 6-digit code hiển thị cho user
        public event Action<string> PairingSuccess;               // nhận pairing token
        public event Action<string, string> PairingStateChanged;  // state + message

        public CloverConnectorService(SynchronizationContext ui)
        {
            _ui = ui ?? new WindowsFormsSynchronizationContext();
        }

        private void PostUI(Action a) => _ui.Post(_ => a(), null);

        // ===== Private handlers cho pairing =====
        public void HandlePairingCode(string code)
            => PostUI(() => PairingCodeReceived?.Invoke(code));

        public void HandlePairingSuccess(string token)
        {
            PairingAuthToken = token;
            PostUI(() => PairingSuccess?.Invoke(token));
        }

        public void HandlePairingState(string state, string message)
            => PostUI(() => PairingStateChanged?.Invoke(state, message));

        /// <summary>
        /// Kết nối qua WebSocket LAN (đúng style WebSocketCloverDeviceConfiguration + Factory).
        /// </summary>
        public void ConnectWebSocket(string endpoint, string pairingAuthToken, CloverListenerHelper listener ) // helper đã AddCloverConnectorListener(this) bên trong       
        {
            // Lưu cấu hình để có thể Reconnect
            Endpoint = endpoint;
            ApplicationId = APPLICATION_ID;
            PosName = "Nails Solutions POS";
            PosId = "POS-3";
            PairingAuthToken = pairingAuthToken;
            Listener = listener;

            SelectedConfig = this.GetSelectedConfig(PairingAuthToken);

            // Dọn kết nối cũ (nếu có)
            try
            {
                if (Connector != null && Listener != null)
                    Connector.RemoveCloverConnectorListener(Listener);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Clover Error: " + ex.Message);
            }

            try { Connector?.Dispose(); } catch { }

            // Tạo connector + mở kết nối + gắn listener
            Connector = CloverConnectorFactory.createICloverConnector(SelectedConfig);
            Connector.InitializeConnection();
            if (Listener != null) Connector.AddCloverConnectorListener(Listener);

            IsConnected = true;
        }

        private CloverDeviceConfiguration GetSelectedConfig(string CurrentPairingToken)
        {
            CloverDeviceConfiguration selectedConfig = null;
            if (Constants.clover_connection_type.Contains("USB"))
            {
                selectedConfig = USBConfig;
            }
            else
            {
                //Lấy thông tin lần kết nối gần nhất
                string endpoint = NailsChekin.Properties.Settings.Default.lastWSEndpoint == null ? "" : NailsChekin.Properties.Settings.Default.lastWSEndpoint;
                string pairingAuthToken = NailsChekin.Properties.Settings.Default.pairingAuthToken == null ? "" : NailsChekin.Properties.Settings.Default.pairingAuthToken;
                if (pairingAuthToken.Trim().Length <= 0 && CurrentPairingToken.Trim().Length > 0)
                    pairingAuthToken = CurrentPairingToken;

                if (endpoint.Trim().Length > 0 && pairingAuthToken.Trim().Length > 0)
                {
                    WebSocketConfig.endpoint = endpoint;
                    WebSocketConfig.OnPairingCode = HandlePairingCode;
                    WebSocketConfig.OnPairingSuccess = HandlePairingSuccess;
                    WebSocketConfig.OnPairingState = HandlePairingState;
                    WebSocketConfig.pairingAuthToken = pairingAuthToken;

                    selectedConfig = new WebSocketCloverDeviceConfiguration(endpoint, APPLICATION_ID, false, 5, "Nails Solutions POS", "POS-3", pairingAuthToken,
                                                          HandlePairingCode, HandlePairingSuccess, HandlePairingState);
                }
            }

            return selectedConfig;
        }

        // <summary>
        /// Áp dụng CloverDeviceConfiguration mới (USB/WebSocket…), tạo connector mới và mở kết nối.
        /// Tự gỡ listener & dispose connector cũ.
        /// </summary>
        public void ApplyConfigurationAndConnect(CloverDeviceConfiguration newConfig, ICloverConnectorListener newListener = null)
        {
            if (newConfig == null) throw new ArgumentNullException(nameof(newConfig));

            // gỡ & dọn cũ
            try { if (Connector != null && Listener != null) Connector.RemoveCloverConnectorListener(Listener); } catch { }
            try { Connector?.Dispose(); } catch { }

            // set lại SelectedConfig
            SelectedConfig = newConfig;
          
            // Tạo connector + mở kết nối + gắn listener
            Connector = CloverConnectorFactory.createICloverConnector(SelectedConfig);
            Connector.InitializeConnection();
            if (Listener != null) Connector.AddCloverConnectorListener(Listener);

            IsConnected = true;
        }

        /// <summary>Khởi tạo lại dùng SelectedConfig hiện tại (không đổi cấu hình).</summary>
        public void ReconnectWithSelectedConfig() => ApplyConfigurationAndConnect(SelectedConfig, Listener);

        /// <summary>
        /// Reconnect với cấu hình đã dùng lần trước.
        /// </summary>
        public void Reconnect()
        {
            if (SelectedConfig == null) return;

            try { if (Connector != null && Listener != null) Connector.RemoveCloverConnectorListener(Listener); } catch { }
            try { Connector?.Dispose(); } catch { }

            Connector = CloverConnectorFactory.createICloverConnector(SelectedConfig);
            Connector.InitializeConnection();
            if (Listener != null) Connector.AddCloverConnectorListener(Listener);

            IsConnected = true;
        }

        /// <summary>
        /// Ngắt kết nối & giải phóng tài nguyên.
        /// </summary>
        public void Disconnect()
        {
            try { if (Connector != null && Listener != null) Connector.RemoveCloverConnectorListener(Listener); } catch { }
            try { Connector?.Dispose(); } catch { }

            Connector = null;
            IsConnected = false;
        }

        // ===================== LỆNH THƯỜNG DÙNG =====================

        /// <summary>
        /// Bắt đầu thanh toán (amount tính bằng cents).
        /// </summary>
        public void StartSale(long amountCents, string externalId = null, bool cardNotPresent = true)
        {
            var req = new SaleRequest
            {
                Amount = amountCents,
                ExternalId = externalId ?? GenerateExternalId(),
                CardNotPresent = cardNotPresent
                // Bổ sung thêm các option khác nếu bạn cần: DisableCashBack, TipAmount, etc.
            };
            Connector?.Sale(req);
        }

        // Overload đầy đủ – thay logic từ Pay() cũ
        public void StartSale( string orderId, long taxAmount, long tipAmount, long amount, bool repairMode, bool tipsOn )
        {
            var req = new SaleRequest();

            // ExternalId tối đa 32 ký tự
            int rndLen = (orderId?.Length ?? 0) <= 19 ? 10 : Math.Max(1, 32 - (orderId?.Length ?? 0) - 3);
            req.ExternalId = $"{orderId}__{ExternalIDUtil.GenerateRandomString(rndLen)}";

            // Số tiền
            req.Amount = amount - tipAmount;
            req.Type = PayIntent.TransactionType.PAYMENT;

            // Cho phép mọi phương thức nhập thẻ + CNP
            req.CardEntryMethods = 36623;
            req.CardNotPresent = true;

            // Tip logic
            //if (tipsOn && !(orderId?.StartsWith("GIFT") == true) && !(orderId?.StartsWith("APPT") == true))
            //{
            //    if (tipAmount > 0)
            //    {
            //        // Tip nhập từ POS: cộng vào tổng
            //        req.TipMode = com.clover.remotepay.sdk.TipMode.NO_TIP;
            //        req.TipAmount = 0;
            //        req.Amount = amount + tipAmount;
            //    }
            //    else
            //    {
            //        // Cho tip trên máy Clover trước khi thanh toán
            //        req.TipMode = com.clover.remotepay.sdk.TipMode.ON_SCREEN_BEFORE_PAYMENT;
            //        // req.TipSuggestions = ... // nếu bạn có preset
            //    }
            //}
            //else
            //{
                req.TipMode = com.clover.remotepay.sdk.TipMode.NO_TIP;
                req.TipAmount = 0;
                req.Amount = amount + tipAmount;
            //}

            req.TaxAmount = taxAmount;

            // Một số cờ tuỳ chọn (để nguyên nếu SDK của bạn hỗ trợ)
            req.DisableCashback = null;
            req.DisableRestartTransactionOnFail = null;
            req.DisablePrinting = false;
            req.DisableReceiptSelection = false;
            req.DisableDuplicateChecking = false;

            // Ký & confirm
            req.SignatureThreshold = 0;
            req.SignatureEntryLocation = null;
            req.AutoAcceptSignature = true;
            req.AutoAcceptPaymentConfirmations = true;

            // Offline options (tuỳ nhu cầu)
            req.AllowOfflinePayment = null;
            req.ApproveOfflinePaymentWithoutPrompt = null;
            req.ForceOfflinePayment = null;

            Connector?.Sale(req);
        }

        public void VoidPayment(string paymentId, string orderId)
        {
            var req = new VoidPaymentRequest { PaymentId = paymentId, OrderId = orderId };
            Connector?.VoidPayment(req);
        }

        public void RefundPayment(string paymentId, long amountCents)
        {
            var req = new RefundPaymentRequest { PaymentId = paymentId, Amount = amountCents };
            Connector?.RefundPayment(req);
        }

        public void AcceptSignature(VerifySignatureRequest req) => Connector?.AcceptSignature(req);
        public void RejectSignature(VerifySignatureRequest req) => Connector?.RejectSignature(req);
        // Một số version: AcceptPayment(ConfirmPaymentRequest)
        // Version khác:   AcceptPayment(Payment)
        // Ta thử lần lượt bằng reflection.
        public void AcceptPayment(ConfirmPaymentRequest req)
        {
            try
            {
                if (Connector == null || req == null) return;

                var t = Connector.GetType();

                // 1) Thử bản mới: AcceptPayment(ConfirmPaymentRequest)
                var m1 = t.GetMethod("AcceptPayment", new[] { typeof(ConfirmPaymentRequest) });
                if (m1 != null) { m1.Invoke(Connector, new object[] { req }); return; }

                // 2) Fallback bản cũ: AcceptPayment(Payment)
                var m2 = t.GetMethod("AcceptPayment", new[] { typeof(Payment) });
                if (m2 != null && req.Payment != null) { m2.Invoke(Connector, new object[] { req.Payment }); }
            }
            catch { /* optional: log */ }
        }

        public void RejectPayment(ConfirmPaymentRequest req)
        {
            try
            {
                if (Connector == null || req == null) return;

                var t = Connector.GetType();

                // 1) Thử bản mới: RejectPayment(ConfirmPaymentRequest)
                var m1 = t.GetMethod("RejectPayment", new[] { typeof(ConfirmPaymentRequest) });
                if (m1 != null) { m1.Invoke(Connector, new object[] { req }); return; }

                // 2) Fallback bản cũ: RejectPayment(Payment)
                var m2 = t.GetMethod("RejectPayment", new[] { typeof(Payment) });
                if (m2 != null && req.Payment != null) { m2.Invoke(Connector, new object[] { req.Payment }); }
            }
            catch { /* optional: log */ }
        }
        public void InvokeInputOption(InputOption io) => Connector?.InvokeInputOption(io);

        public void DisplayReceiptOptions(DisplayPaymentReceiptOptionsRequest req)
            => Connector?.DisplayPaymentReceiptOptions(req);

        private static string GenerateExternalId()
            => $"NS-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}".Substring(0, 24);

        public void Dispose() => Disconnect();

    }

}
