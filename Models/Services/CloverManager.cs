using System;
using System.Threading;
using System.Windows.Forms;
using com.clover.remotepay.sdk;
using com.clover.remotepay.transport;
using com.clover.sdk.v3.payments;
using NailsChekin.Models.Helper;

namespace NailsChekin.Models.Services
{
    public sealed class CloverManager : IDisposable
    {
        // --- UI deps ---
        private readonly Control _owner;
        private readonly SynchronizationContext _ui;
        private readonly Action<string> _status;
        private readonly Action<bool> _enableDisableClover;
        private readonly Action<bool> _setCloverStatus;
        private readonly Action<bool> _setWaitingFlag;
        private readonly Action<bool> _setConfirmPrint;
        private readonly Func<string> _getToken;
        private readonly Action<string> _log;
        private readonly Action<VerifySignatureRequest> _showSignatureForm;

        // --- core objects ---
        public CloverConnectorService Service { get; private set; }
        public CloverListenerHelper Listener { get; private set; }
        public CartHelper Cart { get; private set; }
        public CloverPaymentProcessUI Process { get; private set; }

        // --- re-emit events (Form chỉ subscribe vào Manager) ---
        public event Action<string> PairingCodeReceived;
        public event Action<string> PairingSuccess;
        public event Action<string, string> PairingStateChanged;

        public event Action<Payment, SaleResponse> SaleSucceeded;
        public event Action<string> SaleFailed;
        public event Action<Payment, VoidPaymentResponse> VoidSucceeded;
        public event Action<string> VoidFailed;
        public event Action<Payment, RefundPaymentResponse> RefundSucceeded;
        public event Action<string> RefundFailed;
        public event Action<long> TipUpdated;          // cents
        public event Action<bool> PaymentFinished;     // đóng popup

        // CloverManager.cs
        public event Action<string> PairingNeeded;   // báo cho UI biết lý do chưa connect

        // --- KeepAlive & Reconnect state ---
        private System.Threading.Timer _keepAliveTimer;
        private System.Threading.Timer _reconnectTimer;
        private volatile bool _isConnected;
        private volatile bool _reconnecting;
        private volatile bool _deviceReady;  // đã OnDeviceReady
        private string _lastEndpoint;
        private string _lastPairingToken;
        private int _reconnectAttempt;

        // Tham số điều chỉnh
        private const int KEEPALIVE_MS = 45_000;              // 45s
        private const int RECONNECT_FIRST_DELAY_MS = 1_500;   // 1.5s
        private const int RECONNECT_MAX_DELAY_MS = 30_000;    // 30s
        private const int RECONNECT_READY_GRACE_MS = 4_000;   // sau mỗi lần connect, chờ tối đa 4s cho OnDeviceReady trước khi coi là fail

        // Khi teardown (Disconnect/Cleanup) -> dừng hẳn vòng reconnect, tránh spin trên instance đã dispose.
        private volatile bool _shuttingDown = false;

        // Giao dịch đang chờ để retry sau khi reconnect.
        // Lưu ĐỦ tham số để khi retry không bị mất orderId/tax/tip (tránh sai ticket/sai tiền).
        private sealed class PendingSale
        {
            public bool IsFull;        // true = overload đầy đủ (orderId/tax/tip…)

            // overload đơn giản
            public long AmountCents;
            public string ExternalId;
            public bool CardNotPresent;

            // overload đầy đủ
            public string OrderId;
            public long TaxAmount;
            public long TipAmount;
            public long Amount;
            public bool RepairMode;
            public bool TipsOn;

            public int Retries;
        }
        private PendingSale _pendingSale;

        // Flag device đang reset (tránh gửi Sale trong lúc reset chưa xong)
        private volatile bool _isResetting = false;
        private System.Threading.Timer _resetTimeoutTimer;
        private const int RESET_TIMEOUT_MS = 8_000;   // SDK không phản hồi reset trong 8s thì tự nhả cờ
        private const int RESET_WAIT_MS = 10_000;     // Sale chờ reset tối đa 10s

        public CloverManager(
            Control owner,
            SynchronizationContext ui,
            Action<string> updateStatus,
            Action<bool> enableDisableClover,
            Action<bool> setCloverStatus,
            Action<bool> setWaitingFlag,
            Action<bool> setConfirmPrint,
            Func<string> getToken,
            Action<string> log,
            Action<VerifySignatureRequest> showSignatureForm
        )
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            _ui = ui ?? new WindowsFormsSynchronizationContext();
            _status = updateStatus ?? (_ => { });
            _enableDisableClover = enableDisableClover ?? (_ => { });
            _setCloverStatus = setCloverStatus ?? (_ => { });
            _setWaitingFlag = setWaitingFlag ?? (_ => { });
            _setConfirmPrint = setConfirmPrint ?? (_ => { });
            _getToken = getToken ?? (() => string.Empty);
            _log = log ?? (_ => { });
            _showSignatureForm = showSignatureForm ?? (_ => { });
        }

        // -------------------- Public API --------------------

        public bool ConnectNetworkIfPaired(string endpoint, string pairingToken)
        {
            _lastEndpoint = endpoint;
            _lastPairingToken = pairingToken;

            if (!Constants.clover_connection_type.Contains("USB"))
            {
                endpoint = ConfigLocalHelper.GetConfig("clover_ip_address", "");
                if (string.IsNullOrWhiteSpace(endpoint))
                {
                    PairingNeeded?.Invoke("Chưa cấu hình Endpoint LAN (wss://IP:port/remote_pay).");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(pairingToken))
                {
                    UI(() => { _setCloverStatus?.Invoke(false); _status?.Invoke("Đang ghép nối Clover…"); });
                    BeginPairing(endpoint);
                    return false; // chưa connected, chỉ mới bắt đầu Pairing
                }
            }

            // Đã có đủ thông tin -> thử connect
            try
            {
                Connect(endpoint, pairingToken);
                UI_SYNC(() => { _setCloverStatus?.Invoke(true); _status?.Invoke("Clover connected."); });
                return true;
            }
            catch (Exception ex)
            {
                _setCloverStatus?.Invoke(false);
                _status?.Invoke("Clover connect failed: " + ex.Message);
                PairingNeeded?.Invoke("Kết nối thất bại. Vui lòng kiểm tra IP/Token hoặc Pair lại.");
                return false;
            }
        }

        // Tạo Service/Process/Listener/Cart + nối toàn bộ event. Dùng chung cho EnsureCore & Connect.
        private void BuildCore()
        {
            _shuttingDown = false; // build mới -> cho phép reconnect lại

            Service = new CloverConnectorService(_ui);

            // Process UI trước để listener có chỗ cập nhật.
            // Tham số cancelDevice: nút X trên FormCloverProcessing khi chưa có nút Cancel động -> ResetDevice.
            Process = new CloverPaymentProcessUI(_owner, io => (s, a) => Service.InvokeInputOption(io), () => ResetDevice("Cancel from X close"));

            Listener = new CloverListenerHelper(
                updateStatus: _status,
                enableDisableClover: _enableDisableClover,
                setCloverStatus: _setCloverStatus,
                getCurrentToken: _getToken,
                deviceActivityStartUI: ev => Process.HandleDeviceActivityStart(ev),
                invokeInputOption: io => Service.InvokeInputOption(io),
                displayReceiptOptions: req => Service.DisplayReceiptOptions(req),
                showSignatureForm: req => _showSignatureForm(req),
                reconnect: null // () => Service.Reconnect()
            );

            Cart = new CartHelper(Service, _status, _setWaitingFlag, _setConfirmPrint, _getToken, _log);

            WireListenerAndCart();
        }

        private void EnsureCore()
        {
            if (Service != null) return;
            BuildCore();
        }

        // Nối toàn bộ event (đặt 1 chỗ để không bị lệch giữa các nhánh tạo kết nối).
        // Dùng named handler để Cleanup có thể gỡ đúng bằng UnwireListenerAndCart().
        private void WireListenerAndCart()
        {
            // bridge: Listener -> Cart
            Listener.SaleResponse += Cart.HandleSaleResponse;
            Listener.VoidPaymentResponse += Cart.HandleVoidPaymentResponse;
            Listener.RefundPaymentResponse += Cart.HandleRefundPaymentResponse;
            Listener.TipAdded += Cart.HandleTipAdded;

            // bridge: Cart -> Manager
            Cart.SaleSucceeded += HandleSaleSucceeded;
            Cart.SaleFailed += OnCartSaleFailed;
            Cart.VoidSucceeded += HandleVoidSucceeded;
            Cart.VoidFailed += OnCartVoidFailed;
            Cart.RefundSucceeded += HandleRefundSucceeded;
            Cart.RefundFailed += OnCartRefundFailed;
            Cart.TipUpdated += OnCartTipUpdated;
            Cart.PaymentFinished += OnCartPaymentFinished;

            // bubble pairing ra Manager
            Service.PairingCodeReceived += OnServicePairingCode;
            Service.PairingSuccess += OnServicePairingSuccess;
            Service.PairingStateChanged += OnServicePairingState;

            // theo dõi kết nối để auto-reconnect/keepalive + xử lý state kẹt
            Listener.DeviceReady += OnListenerDeviceReady;
            Listener.DeviceDisconnected += OnListenerDeviceDisconnected;
            Listener.DeviceError += OnListenerDeviceError;
            Listener.InvalidStateTransition += OnListenerInvalidStateTransition;
            Listener.ResetDeviceCompleted += OnListenerResetDeviceCompleted;
        }

        private void UnwireListenerAndCart()
        {
            if (Listener != null && Cart != null)
            {
                Listener.SaleResponse -= Cart.HandleSaleResponse;
                Listener.VoidPaymentResponse -= Cart.HandleVoidPaymentResponse;
                Listener.RefundPaymentResponse -= Cart.HandleRefundPaymentResponse;
                Listener.TipAdded -= Cart.HandleTipAdded;
            }

            if (Cart != null)
            {
                Cart.SaleSucceeded -= HandleSaleSucceeded;
                Cart.SaleFailed -= OnCartSaleFailed;
                Cart.VoidSucceeded -= HandleVoidSucceeded;
                Cart.VoidFailed -= OnCartVoidFailed;
                Cart.RefundSucceeded -= HandleRefundSucceeded;
                Cart.RefundFailed -= OnCartRefundFailed;
                Cart.TipUpdated -= OnCartTipUpdated;
                Cart.PaymentFinished -= OnCartPaymentFinished;
            }

            if (Service != null)
            {
                Service.PairingCodeReceived -= OnServicePairingCode;
                Service.PairingSuccess -= OnServicePairingSuccess;
                Service.PairingStateChanged -= OnServicePairingState;
            }

            if (Listener != null)
            {
                Listener.DeviceReady -= OnListenerDeviceReady;
                Listener.DeviceDisconnected -= OnListenerDeviceDisconnected;
                Listener.DeviceError -= OnListenerDeviceError;
                Listener.InvalidStateTransition -= OnListenerInvalidStateTransition;
                Listener.ResetDeviceCompleted -= OnListenerResetDeviceCompleted;
            }
        }

        // ---------- named bridge handlers (Cart/Service -> Manager) ----------
        private void OnCartSaleFailed(string reason) => SaleFailed?.Invoke(reason);
        private void OnCartVoidFailed(string reason) => VoidFailed?.Invoke(reason);
        private void OnCartRefundFailed(string reason) => RefundFailed?.Invoke(reason);
        private void OnCartTipUpdated(long cents) => TipUpdated?.Invoke(cents);
        private void OnCartPaymentFinished(bool ok)
        {
            PaymentFinished?.Invoke(ok);
            try { Process?.StopPayment(); } catch { }
        }
        private void OnServicePairingCode(string c) => PairingCodeReceived?.Invoke(c);
        private void OnServicePairingSuccess(string t) => PairingSuccess?.Invoke(t);
        private void OnServicePairingState(string s, string m) => PairingStateChanged?.Invoke(s, m);

        // ---------- named connection handlers (Listener -> Manager) ----------
        private void OnListenerDeviceReady()
        {
            _isConnected = true;
            _deviceReady = true;
            _reconnectAttempt = 0;
            UI(() => { _setCloverStatus?.Invoke(true); _status?.Invoke("Clover ready."); });
            StartKeepAlive();

            // Retry sale nếu đang treo — dùng đúng overload đã lưu để không mất orderId/tax/tip
            var p = _pendingSale;
            _pendingSale = null;
            if (p != null)
            {
                _status?.Invoke("Retrying pending sale…");
                try
                {
                    if (p.IsFull)
                        Service?.StartSale(p.OrderId, p.TaxAmount, p.TipAmount, p.Amount, p.RepairMode, p.TipsOn);
                    else
                        Service?.StartSale(p.AmountCents, p.ExternalId, p.CardNotPresent);
                }
                catch
                {
                    if (p.Retries > 0)
                    {
                        // xếp lại để thử thêm 1 lần nữa
                        p.Retries--;
                        _pendingSale = p;
                        ScheduleReconnect(immediate: true);
                    }
                }
            }
        }

        private void OnListenerDeviceDisconnected()
        {
            _isConnected = false;
            _deviceReady = false;
            UI(() => { _setCloverStatus?.Invoke(false); _status?.Invoke("Clover disconnected."); });
            ScheduleReconnect();
        }

        private void OnListenerDeviceError(string code, string msg)
        {
            _isConnected = false;
            _deviceReady = false;
            UI(() => { _setCloverStatus?.Invoke(false); _status?.Invoke($"Clover error {code}: {msg}"); });
            ScheduleReconnect();
        }

        private void OnListenerInvalidStateTransition(string requested, string current)
        {
            // Device kẹt giữa phiên (vd ADD_TIP_BEFORE_PAYMENT) → reset để về idle
            _status?.Invoke($"InvalidStateTransition: {requested} <- {current}. Đang reset device…");
            BeginResetDevice();

            // Giải phóng UI để popup/cờ không bị treo nếu SDK chỉ phát notification này mà không có SaleResponse
            UI(() => _setWaitingFlag?.Invoke(false));
            try { Process?.StopPayment(); } catch { }
        }

        private void OnListenerResetDeviceCompleted(bool success)
        {
            _isResetting = false;
            _resetTimeoutTimer?.Dispose(); _resetTimeoutTimer = null;
            _status?.Invoke(success ? "Device reset xong, sẵn sàng thanh toán." : "Reset device thất bại.");
        }

        // Khi user bấm Pair (ép hiện mã pairing)
        public void BeginPairing(string endpoint)
        {
            EnsureCore();

            var cfg = new WebSocketCloverDeviceConfiguration(
                endpoint, "com.clover.CloverExamplePOS:3.0.2", false, 5, "Nails Solutions POS", "POS-RETAIL", "",
                Service.HandlePairingCode, Service.HandlePairingSuccess, Service.HandlePairingState
            );

            Service.ApplyConfigurationAndConnect(cfg, Listener);
        }

        public void Connect(string endpoint, string pairingToken)
        {
            _lastEndpoint = endpoint;
            _lastPairingToken = pairingToken;

            Cleanup(); // bảo đảm sạch trước khi connect

            BuildCore();

            // thực sự connect (LAN/WebSocket)
            Service.ConnectWebSocket(endpoint, pairingToken, Listener);
            if (Service.Connector == null)
                throw new InvalidOperationException("Clover connector is NULL after Connect.");

            _isConnected = true;
            // KHÔNG reset _reconnectAttempt ở đây: transport up != device ready. Chỉ reset trong OnListenerDeviceReady,
            // nếu không backoff sẽ luôn bị kéo về 1.5s -> hammer khi máy báo lỗi ngay sau connect.
            StartKeepAlive();
        }

        public void SwitchCloverMode(string endpoint, string pairingToken, CloverDeviceConfiguration newConfig)
        {
            Cleanup(); // bảo đảm sạch trước khi connect

            if (Service == null || Listener == null || Process == null || Cart == null)
                Connect(endpoint, pairingToken);
            else
                Service.ApplyConfigurationAndConnect(newConfig, Listener);
        }

        public void Reconnect()
        {
            if (Service == null) throw new InvalidOperationException("Service not created. Call Connect first.");
            Service.Reconnect();
        }

        public void ResetDevice(string reason = "")
        {
            _status?.Invoke("Force reset Clover " + reason);
            BeginResetDevice();
        }

        /// <summary>Máy đã sẵn sàng nhận lệnh (đã OnDeviceReady và connector còn sống).</summary>
        public bool IsReady => _deviceReady && Service?.Connector != null;

        /// <summary>
        /// Gửi 1 lệnh hỏi trạng thái thiết bị (heartbeat tức thì). Nếu máy còn sống thì OnRetrieveDeviceStatusResponse
        /// bắn về; nếu đang rớt thì lỗi sẽ kích ScheduleReconnect. Dùng để "đánh thức"/kiểm tra trước khi báo lỗi.
        /// </summary>
        public void PingDeviceStatus()
        {
            try { Service?.Connector?.RetrieveDeviceStatus(new RetrieveDeviceStatusRequest()); } catch { }
        }

        // Gửi lệnh ResetDevice và bật flag _isResetting.
        // Nếu RESET_TIMEOUT_MS không nhận được ResetDeviceResponse thì tự tắt flag (tránh bị treo mãi).
        private void BeginResetDevice()
        {
            _isResetting = true;
            try { Service?.Connector?.ResetDevice(); } catch { _isResetting = false; return; }

            _resetTimeoutTimer?.Dispose();
            _resetTimeoutTimer = new System.Threading.Timer(_ =>
            {
                _isResetting = false;
                _status?.Invoke("Reset device timeout — tiếp tục bình thường.");
            }, null, RESET_TIMEOUT_MS, Timeout.Infinite);
        }

        public void Disconnect()
        {
            _shuttingDown = true;
            _isConnected = false;
            _deviceReady = false;
            StopKeepAlive();
            _reconnectTimer?.Dispose(); _reconnectTimer = null;
            try { Service?.Connector?.RemoveCloverConnectorListener(Listener); } catch { }
            try { Service?.Disconnect(); } catch { }
        }

        /// <summary>Full reset: tháo event, disconnect + dispose, đóng popup.</summary>
        public void Cleanup()
        {
            try
            {
                _shuttingDown = true;
                _isConnected = false;
                _deviceReady = false;
                StopKeepAlive();
                _reconnectTimer?.Dispose(); _reconnectTimer = null;

                // dọn trạng thái reset để không kẹt cờ ở instance kế tiếp
                _isResetting = false;
                _resetTimeoutTimer?.Dispose(); _resetTimeoutTimer = null;
                _pendingSale = null;

                // gỡ toàn bộ event đã nối (named handler nên -= khớp đúng)
                UnwireListenerAndCart();

                // gỡ listener khỏi SDK connector (đây mới là cái chặn double-callback thật sự)
                try { Service?.Connector?.RemoveCloverConnectorListener(Listener); } catch { }
                try { Service?.Dispose(); } catch { }

                try { Process?.StopPayment(); } catch { }
                try { Process?.Dispose(); } catch { }

                Service = null; Listener = null; Cart = null; Process = null;
            }
            catch { /* ignore */ }
        }

        // ---------- tiện ích gửi lệnh ----------
        public void StartSale(long amountCents, string externalId = null, bool cardNotPresent = true)
            => Service?.StartSale(amountCents, externalId, cardNotPresent);

        public void StartSale(string orderId, long taxAmount, long tipAmount, long amount, bool repairMode, bool tipsOn)
            => Service?.StartSale(orderId, taxAmount, tipAmount, amount, repairMode, tipsOn);

        // Public API mới: gọi thay vì StartSale(...) cũ khi bạn muốn an toàn kết nối
        public void StartSaleWithAutoRetry(long amountCents, string externalId = null, bool cardNotPresent = true, int retries = 1)
        {
            // Device đang reset → chờ reset xong rồi tự retry
            if (_isResetting)
            {
                _status?.Invoke("Device đang reset. Tự động thử lại sau khi reset xong…");
                WaitResetThenSale(() => StartSaleWithAutoRetry(amountCents, externalId, cardNotPresent, retries));
                return;
            }

            // Nếu chưa sẵn sàng thì xếp hàng + ép reconnect ngay
            if (!_deviceReady || Service?.Connector == null)
            {
                _status?.Invoke("Device not ready. Reconnecting before sale…");
                _pendingSale = new PendingSale
                {
                    IsFull = false,
                    AmountCents = amountCents,
                    ExternalId = externalId,
                    CardNotPresent = cardNotPresent,
                    Retries = retries
                };
                ScheduleReconnect(immediate: true);
                return;
            }

            try
            {
                Service?.StartSale(amountCents, externalId, cardNotPresent);
            }
            catch
            {
                // Nếu văng do transport, tạo pending và reconnect
                if (retries > 0)
                {
                    _status?.Invoke("Transport error. Reconnecting to retry sale…");
                    _pendingSale = new PendingSale
                    {
                        IsFull = false,
                        AmountCents = amountCents,
                        ExternalId = externalId,
                        CardNotPresent = cardNotPresent,
                        Retries = retries - 1
                    };
                    _deviceReady = false;
                    ScheduleReconnect(immediate: true);
                }
                else
                {
                    // Hết lượt retry -> để Cart/Listener báo lỗi như bình thường
                    throw;
                }
            }
        }

        public void StartSaleWithAutoRetry(string orderId, long taxAmount, long tipAmount, long amount, bool repairMode, bool tipsOn,
                string externalId = null, bool cardNotPresent = true, int retries = 1)
        {
            // Device đang reset → chờ reset xong rồi tự retry
            if (_isResetting)
            {
                _status?.Invoke("Device đang reset. Tự động thử lại sau khi reset xong…");
                WaitResetThenSale(() => StartSaleWithAutoRetry(orderId, taxAmount, tipAmount, amount, repairMode, tipsOn, externalId, cardNotPresent, retries));
                return;
            }

            // Nếu chưa sẵn sàng thì xếp hàng + ép reconnect ngay
            if (!_deviceReady || Service?.Connector == null)
            {
                _status?.Invoke("Device not ready. Reconnecting before sale…");
                _pendingSale = new PendingSale
                {
                    IsFull = true,
                    OrderId = orderId,
                    TaxAmount = taxAmount,
                    TipAmount = tipAmount,
                    Amount = amount,
                    RepairMode = repairMode,
                    TipsOn = tipsOn,
                    Retries = retries
                };
                ScheduleReconnect(immediate: true);
                return;
            }

            try
            {
                Service?.StartSale(orderId, taxAmount, tipAmount, amount, repairMode, tipsOn);
            }
            catch
            {
                // Nếu văng do transport, tạo pending và reconnect
                if (retries > 0)
                {
                    _status?.Invoke("Transport error. Reconnecting to retry sale…");
                    _pendingSale = new PendingSale
                    {
                        IsFull = true,
                        OrderId = orderId,
                        TaxAmount = taxAmount,
                        TipAmount = tipAmount,
                        Amount = amount,
                        RepairMode = repairMode,
                        TipsOn = tipsOn,
                        Retries = retries - 1
                    };
                    _deviceReady = false;
                    ScheduleReconnect(immediate: true);
                }
                else
                {
                    // Hết lượt retry -> để Cart/Listener báo lỗi như bình thường
                    throw;
                }
            }
        }

        // Chờ _isResetting = false (tối đa RESET_WAIT_MS) rồi gọi lại Sale trên thread pool.
        // Ép nhả cờ sau khi hết thời gian chờ để không lặp vô hạn.
        private void WaitResetThenSale(Action retrySale)
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                int waited = 0;
                while (_isResetting && waited < RESET_WAIT_MS)
                {
                    System.Threading.Thread.Sleep(200);
                    waited += 200;
                }
                _isResetting = false; // chống lặp nếu reset quá hạn
                retrySale();
            });
        }

        public void VoidPayment(string paymentId, string orderId) => Service?.VoidPayment(paymentId, orderId);
        public void RefundPayment(string paymentId, long amountCents) => Service?.RefundPayment(paymentId, amountCents);

        public void Dispose() => Cleanup();

        // ---------- private bridge handlers ----------
        private void HandleSaleSucceeded(Payment p, SaleResponse r) => SaleSucceeded?.Invoke(p, r);
        private void HandleVoidSucceeded(Payment p, VoidPaymentResponse r) => VoidSucceeded?.Invoke(p, r);
        private void HandleRefundSucceeded(Payment p, RefundPaymentResponse r) => RefundSucceeded?.Invoke(p, r);

        // Gọi UI bất đồng bộ
        private void UI(Action act)
        {
            try { _ui?.Post(_ => act(), null); } catch { }
        }

        // Gọi UI đồng bộ (đảm bảo xong rồi mới return)
        private void UI_SYNC(Action act)
        {
            try
            {
                if (_ui == null) { act(); return; }
                if (SynchronizationContext.Current == _ui) act();
                else _ui.Send(_ => act(), null);
            }
            catch { }
        }

        // === KeepAlive & Reconnect ===

        private void StartKeepAlive()
        {
            _keepAliveTimer?.Dispose();
            _keepAliveTimer = new System.Threading.Timer(_ =>
            {
                try
                {
                    Service?.Connector?.RetrieveDeviceStatus(new RetrieveDeviceStatusRequest());
                }
                catch { /* nếu rớt thật, ScheduleReconnect sẽ lo */ }
            }, null, KEEPALIVE_MS, KEEPALIVE_MS);
        }

        private void StopKeepAlive()
        {
            _keepAliveTimer?.Dispose();
            _keepAliveTimer = null;
        }

        private void ScheduleReconnect(bool immediate = false)
        {
            if (_shuttingDown) return;
            if (_reconnecting) return;
            _reconnecting = true;

            int delay = immediate ? 0 : Math.Min(
                RECONNECT_FIRST_DELAY_MS * (int)Math.Pow(2, Math.Max(0, _reconnectAttempt - 1)),
                RECONNECT_MAX_DELAY_MS);

            _reconnectTimer?.Dispose();
            _reconnectTimer = new System.Threading.Timer(_ =>
            {
                try
                {
                    if (_shuttingDown || _deviceReady) return; // đã teardown hoặc đã ready -> thôi

                    UI(() => _status?.Invoke("Reconnecting Clover… (lần " + (_reconnectAttempt + 1) + ")"));

                    // Bước 1: thử fast-path trên worker (không Wait vô hạn)
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        try { Service?.Reconnect(); } catch { }
                    }).Wait(1500); // đợi ngắn

                    // Bước 2: nếu chưa ready thì rebuild kết nối
                    if (!_shuttingDown && !_deviceReady)
                    {
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(_lastEndpoint))
                            {
                                Connect(_lastEndpoint, _lastPairingToken ?? string.Empty);
                            }
                            else
                            {
                                var usbCfg = BuildUsbConfig();
                                if (usbCfg != null)
                                    Service.ApplyConfigurationAndConnect(usbCfg, Listener);
                            }
                        }
                        catch
                        {
                            // sẽ backoff và thử lại
                        }
                    }

                    // Bước 3: chờ OnDeviceReady tối đa RECONNECT_READY_GRACE_MS.
                    // Tránh tear-down máy đang bắt tay (transport up nhưng chưa ready) + giãn nhịp retry (không hammer).
                    int waited = 0;
                    while (!_shuttingDown && !_deviceReady && waited < RECONNECT_READY_GRACE_MS)
                    {
                        System.Threading.Thread.Sleep(200);
                        waited += 200;
                    }
                }
                finally
                {
                    _reconnecting = false;

                    // Chỉ coi là thành công khi máy THẬT SỰ ready (OnDeviceReady), KHÔNG dựa vào _isConnected lạc quan.
                    if (!_shuttingDown && !_deviceReady)
                    {
                        _reconnectAttempt++;
                        ScheduleReconnect(false); // backoff TĂNG dần: 1.5 -> 3 -> 6 -> 12 -> 24 -> 30s
                    }
                    else if (_deviceReady)
                    {
                        _reconnectAttempt = 0;
                    }
                }
            }, null, delay, Timeout.Infinite);
        }

        // TODO: Trả về UsbCloverDeviceConfiguration đúng theo SDK nếu dùng USB MODE.
        private CloverDeviceConfiguration BuildUsbConfig()
        {
            // Ví dụ khung (hãy sửa lại theo version SDK của bạn):
            // return new UsbCloverDeviceConfiguration(
            //     "com.yourcompany.yourpos:3.0.2",
            //     "Nails Solutions POS",
            //     "POS-3",
            //     /* forceBluetooth */ false
            // );
            return null; // nếu không dùng USB thuần, cứ để null
        }
    }
}

