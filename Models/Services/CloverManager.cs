using System;
using System.Threading;
using System.Windows.Forms;
using com.clover.remotepay.sdk;
using com.clover.remotepay.transport;
using com.clover.sdk.v3.payments;
using NailsChekin.Models.Helper;
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

        // Giao dịch đang chờ để retry sau khi reconnect
        private (long amountCents, string externalId, bool cardNotPresent, int retries)? _pendingSale;

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

        private void EnsureCore()
        {
            if (Service != null) return;

            Service = new CloverConnectorService(_ui);
            Process = new CloverPaymentProcessUI(_owner, io => (s, a) => Service.InvokeInputOption(io));
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

            // bridge: Listener -> Cart
            Listener.SaleResponse += Cart.HandleSaleResponse;
            Listener.VoidPaymentResponse += Cart.HandleVoidPaymentResponse;
            Listener.RefundPaymentResponse += Cart.HandleRefundPaymentResponse;
            Listener.TipAdded += Cart.HandleTipAdded;

            // bridge: Cart -> Manager
            Cart.SaleSucceeded += HandleSaleSucceeded;
            Cart.SaleFailed += reason => SaleFailed?.Invoke(reason);
            Cart.VoidSucceeded += HandleVoidSucceeded;
            Cart.VoidFailed += reason => VoidFailed?.Invoke(reason);
            Cart.RefundSucceeded += HandleRefundSucceeded;
            Cart.RefundFailed += reason => RefundFailed?.Invoke(reason);
            Cart.TipUpdated += cents => TipUpdated?.Invoke(cents);
            Cart.PaymentFinished += ok => { PaymentFinished?.Invoke(ok); try { Process.StopPayment(); } catch { } };

            // bubble pairing ra Manager
            Service.PairingCodeReceived += c => PairingCodeReceived?.Invoke(c);
            Service.PairingSuccess += t => PairingSuccess?.Invoke(t);
            Service.PairingStateChanged += (s, m) => PairingStateChanged?.Invoke(s, m);

            // ---- Gắn thêm event theo dõi kết nối để auto-reconnect/keepalive ----
            Listener.DeviceReady += () =>
            {
                _isConnected = true;
                _deviceReady = true;
                _reconnectAttempt = 0;
                UI(() => { _setCloverStatus?.Invoke(true); _status?.Invoke("Clover ready."); });
                StartKeepAlive();

                // Retry sale nếu đang treo
                var p = _pendingSale;
                _pendingSale = null;
                if (p.HasValue)
                {
                    _status?.Invoke("Retrying pending sale…");
                    try { Service?.StartSale(p.Value.amountCents, p.Value.externalId, p.Value.cardNotPresent); }
                    catch
                    {
                        if (p.Value.retries > 0)
                        {
                            // xếp lại để thử thêm 1 lần nữa
                            _pendingSale = (p.Value.amountCents, p.Value.externalId, p.Value.cardNotPresent, p.Value.retries - 1);
                            ScheduleReconnect(immediate: true);
                        }
                    }
                }
            };

            Listener.DeviceDisconnected += () =>
            {
                _isConnected = false;
                _deviceReady = false;
                UI(() => { _setCloverStatus?.Invoke(false); _status?.Invoke("Clover USB disconnected."); });
                ScheduleReconnect();
            };

            Listener.DeviceError += (code, msg) =>
            {
                _isConnected = false;
                _deviceReady = false;
                UI(() => { _setCloverStatus?.Invoke(false); _status?.Invoke($"Clover error {code}: {msg}"); });
                ScheduleReconnect();
            };
        }

        // Khi user bấm Pair (ép hiện mã pairing)
        public void BeginPairing(string endpoint)
        {
            EnsureCore();

            var cfg = new WebSocketCloverDeviceConfiguration(
                endpoint, "com.clover.CloverExamplePOS:3.0.2", false, 5, "Nails Solutions POS", "POS-3", "",
                Service.HandlePairingCode, Service.HandlePairingSuccess, Service.HandlePairingState
            );

            Service.ApplyConfigurationAndConnect(cfg, Listener);
        }

        public void Connect(string endpoint, string pairingToken)
        {
            _lastEndpoint = endpoint;
            _lastPairingToken = pairingToken;

            Cleanup(); // bảo đảm sạch trước khi connect

            Service = new CloverConnectorService(_ui);

            // Process UI trước để listener có chỗ cập nhật
            Process = new CloverPaymentProcessUI(_owner, io => (s, a) => Service.InvokeInputOption(io));

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

            // bridge: Listener -> Cart
            Listener.SaleResponse += Cart.HandleSaleResponse;
            Listener.VoidPaymentResponse += Cart.HandleVoidPaymentResponse;
            Listener.RefundPaymentResponse += Cart.HandleRefundPaymentResponse;
            Listener.TipAdded += Cart.HandleTipAdded;

            // bridge: Cart -> Manager
            Cart.SaleSucceeded += HandleSaleSucceeded;
            Cart.SaleFailed += reason => SaleFailed?.Invoke(reason);
            Cart.VoidSucceeded += HandleVoidSucceeded;
            Cart.VoidFailed += reason => VoidFailed?.Invoke(reason);
            Cart.RefundSucceeded += HandleRefundSucceeded;
            Cart.RefundFailed += reason => RefundFailed?.Invoke(reason);
            Cart.TipUpdated += cents => TipUpdated?.Invoke(cents);
            Cart.PaymentFinished += ok => { PaymentFinished?.Invoke(ok); try { Process.StopPayment(); } catch { } };

            // bubble pairing
            Service.PairingCodeReceived += c => PairingCodeReceived?.Invoke(c);
            Service.PairingSuccess += t => PairingSuccess?.Invoke(t);
            Service.PairingStateChanged += (s, m) => PairingStateChanged?.Invoke(s, m);

            // theo dõi kết nối
            // ---- Gắn thêm event theo dõi kết nối để auto-reconnect/keepalive ----
            Listener.DeviceReady += () =>
            {
                _isConnected = true;
                _deviceReady = true;
                _reconnectAttempt = 0;
                UI(() => { _setCloverStatus?.Invoke(true); _status?.Invoke("Clover ready."); });
                StartKeepAlive();

                // Retry sale nếu đang treo
                var p = _pendingSale;
                _pendingSale = null;
                if (p.HasValue)
                {
                    _status?.Invoke("Retrying pending sale…");
                    try { Service?.StartSale(p.Value.amountCents, p.Value.externalId, p.Value.cardNotPresent); }
                    catch
                    {
                        if (p.Value.retries > 0)
                        {
                            // xếp lại để thử thêm 1 lần nữa
                            _pendingSale = (p.Value.amountCents, p.Value.externalId, p.Value.cardNotPresent, p.Value.retries - 1);
                            ScheduleReconnect(immediate: true);
                        }
                    }
                }
            };

            Listener.DeviceDisconnected += () =>
            {
                _isConnected = false;
                _deviceReady = false;
                UI(() => { _setCloverStatus?.Invoke(false); _status?.Invoke("Clover disconnected."); });
                ScheduleReconnect();
            };

            Listener.DeviceError += (code, msg) =>
            {
                _isConnected = false;
                _deviceReady = false;
                UI(() => { _setCloverStatus?.Invoke(false); _status?.Invoke($"Clover error {code}: {msg}"); });
                ScheduleReconnect();
            };

            // thực sự connect (LAN/WebSocket)
            Service.ConnectWebSocket(endpoint, pairingToken, Listener);
            if (Service.Connector == null)
                throw new InvalidOperationException("Clover connector is NULL after Connect.");

            _isConnected = true;
            _reconnectAttempt = 0;
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
            try { Service?.Connector?.ResetDevice(); } catch { }
        }

        public void Disconnect()
        {
            _isConnected = false;
            StopKeepAlive();
            try { Service?.Connector?.RemoveCloverConnectorListener(Listener); } catch { }
            try { Service?.Disconnect(); } catch { }
        }

        /// <summary>Full reset: tháo event, disconnect + dispose, đóng popup.</summary>
        public void Cleanup()
        {
            try
            {
                _isConnected = false;
                StopKeepAlive();
                _reconnectTimer?.Dispose(); _reconnectTimer = null;

                if (Listener != null && Cart != null)
                {
                    Listener.SaleResponse -= Cart.HandleSaleResponse;
                    Listener.VoidPaymentResponse -= Cart.HandleVoidPaymentResponse;
                    Listener.RefundPaymentResponse -= Cart.HandleRefundPaymentResponse;
                    Listener.TipAdded -= Cart.HandleTipAdded;

                    // gỡ event theo dõi kết nối
                    Listener.DeviceDisconnected -= OnListenerDeviceDisconnectedProxy;
                    Listener.DeviceError -= OnListenerDeviceErrorProxy;
                    Listener.DeviceReady -= OnListenerDeviceReadyProxy;
                }

                if (Cart != null)
                {
                    Cart.SaleSucceeded -= HandleSaleSucceeded;
                    Cart.SaleFailed -= (Action<string>)(reason => SaleFailed?.Invoke(reason));
                    Cart.VoidSucceeded -= HandleVoidSucceeded;
                    Cart.VoidFailed -= (Action<string>)(reason => VoidFailed?.Invoke(reason));
                    Cart.RefundSucceeded -= HandleRefundSucceeded;
                    Cart.RefundFailed -= (Action<string>)(reason => RefundFailed?.Invoke(reason));
                    Cart.TipUpdated -= (Action<long>)(c => TipUpdated?.Invoke(c));
                    Cart.PaymentFinished -= (Action<bool>)(ok => { PaymentFinished?.Invoke(ok); try { Process?.StopPayment(); } catch { } });
                }

                if (Service != null)
                {
                    Service.PairingCodeReceived -= (Action<string>)(c => PairingCodeReceived?.Invoke(c));
                    Service.PairingSuccess -= (Action<string>)(t => PairingSuccess?.Invoke(t));
                    Service.PairingStateChanged -= (Action<string, string>)((s, m) => PairingStateChanged?.Invoke(s, m));
                }

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
            // Nếu chưa sẵn sàng thì xếp hàng + ép reconnect ngay
            if (!_deviceReady || Service?.Connector == null)
            {
                _status?.Invoke("Device not ready. Reconnecting before sale…");
                _pendingSale = (amountCents, externalId, cardNotPresent, retries);
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
                    _pendingSale = (amountCents, externalId, cardNotPresent, retries - 1);
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
            // Nếu chưa sẵn sàng thì xếp hàng + ép reconnect ngay
            if (!_deviceReady || Service?.Connector == null)
            {
                _status?.Invoke("Device not ready. Reconnecting before sale…");
                _pendingSale = (amount, externalId, cardNotPresent, retries);
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
                    _pendingSale = (amount, externalId, cardNotPresent, retries - 1);
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
                    UI(() => _status?.Invoke("Reconnecting Clover…"));

                    // Bước 1: thử fast-path
                    //try { Service?.Reconnect(); } catch { /* qua bước 2 */ }
                    // Chạy các lệnh nặng trên worker
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        try { Service?.Reconnect(); } catch { }
                    }).Wait(1500); // đợi ngắn; không nên Wait vô hạn

                    // Chờ ngắn xem DeviceReady chưa
                    //Thread.Sleep(1500);
                    if (!_isConnected)
                    {
                        try
                        {
                            // Bước 2: rebuild kết nối
                            if (!string.IsNullOrWhiteSpace(_lastEndpoint))
                            {
                                Connect(_lastEndpoint, _lastPairingToken ?? string.Empty);
                            }
                            else
                            {
                                // Nếu đang chạy USB thuần, áp USB config (nếu bạn trả ra được)
                                var usbCfg = BuildUsbConfig();
                                if (usbCfg != null)
                                {
                                    Service.ApplyConfigurationAndConnect(usbCfg, Listener);
                                }
                            }
                        }
                        catch
                        {
                            // sẽ backoff và thử lại
                        }
                    }
                }
                finally
                {
                    _reconnecting = false;
                    if (!_isConnected)
                    {
                        _reconnectAttempt++;
                        ScheduleReconnect(false); // backoff tiếp
                    }
                    else
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

        // Proxy để gỡ event trong Cleanup an toàn (tránh null khác instance)
        private void OnListenerDeviceDisconnectedProxy() { }
        private void OnListenerDeviceErrorProxy(string code, string msg) { }
        private void OnListenerDeviceReadyProxy() { }
    }
}
