using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketSharp;

namespace NailsChekin.Models.Helper
{
    /// <summary>
    /// Helper quản lý WebSocketSharp an toàn cho WinForms:
    /// - Tất cả Connect / Send / Disconnect đều serialize bằng SemaphoreSlim
    /// - Không bao giờ Connect() lại trên socket đã từng connect
    /// - Reconnect luôn Close + Null socket cũ rồi tạo socket mới
    /// - Timeout connect sẽ invalidate socket cũ để tránh OnOpen muộn update sai trạng thái
    /// - Compatible C# 7.3
    /// </summary>
    public sealed class CodePayHelper : IDisposable
    {
        private readonly Control _ui;
        private readonly Func<string> _getUrl;
        private readonly Func<WebSocket> _getWs;
        private readonly Action<WebSocket> _setWs;

        private readonly Action<string> _uiStatus;
        private readonly Action<string> _onMessage;
        private readonly Action<bool> _setConnectedFlag;
        private readonly Action<string, string> _log;

        private readonly SemaphoreSlim _gate = new SemaphoreSlim(1, 1);

        private int _wsGeneration = 0;
        private CancellationTokenSource _reconnectCts;
        private int _reconnectLoopStarted = 0;
        private volatile bool _manualDisconnect = false;
        private bool _disposed = false;

        public int ConnectTimeoutMs { get; set; } = 5000;
        public int ConnectRetries { get; set; } = 2;
        public int SendReconnectRetries { get; set; } = 1;

        public CodePayHelper(
            Control uiInvokeTarget,
            Func<string> getServerUrl,
            Func<WebSocket> getSocket,
            Action<WebSocket> setSocket,
            Action<string> updateStatus,
            Action<string> onMessage,
            Action<bool> setConnectedFlag,
            Action<string, string> log)
        {
            _ui = uiInvokeTarget;
            _getUrl = getServerUrl ?? throw new ArgumentNullException(nameof(getServerUrl));
            _getWs = getSocket ?? throw new ArgumentNullException(nameof(getSocket));
            _setWs = setSocket ?? throw new ArgumentNullException(nameof(setSocket));

            _uiStatus = updateStatus ?? delegate { };
            _onMessage = onMessage ?? delegate { };
            _setConnectedFlag = setConnectedFlag ?? delegate { };
            _log = log ?? delegate { };
        }

        public bool IsConnected
        {
            get
            {
                try
                {
                    var ws = _getWs();
                    return ws != null && ws.ReadyState == WebSocketState.Open;
                }
                catch
                {
                    return false;
                }
            }
        }

        public async Task<bool> WarmUpConnectAsync()
        {
            await _gate.WaitAsync().ConfigureAwait(false);

            try
            {
                _manualDisconnect = false;
                return await EnsureConnectedCoreUnsafeAsync(ConnectRetries, false).ConfigureAwait(false);
            }
            finally
            {
                _gate.Release();
            }
        }

        public async Task DisconnectAsync()
        {
            _manualDisconnect = true;
            StopReconnectLoop();

            await _gate.WaitAsync().ConfigureAwait(false);

            try
            {
                SafeCloseAndNullUnsafe(true);
            }
            finally
            {
                _gate.Release();
            }
        }

        /// <summary>
        /// Send payment:
        /// - Ensure connected
        /// - Send
        /// - Nếu send fail thì close socket cũ, reconnect, send lại
        /// Trả "" nếu OK, hoặc error text nếu fail.
        /// </summary>
        public async Task<string> SendPaymentAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return "Error: empty message.";

            await _gate.WaitAsync().ConfigureAwait(false);

            try
            {
                _manualDisconnect = false;

                bool ok = await EnsureConnectedCoreUnsafeAsync(ConnectRetries, true).ConfigureAwait(false);
                if (!ok)
                    return "Error: Connection to the CodePay device failed. Kindly restart the device and try again";

                if (TrySendUnsafe(message))
                    return "";

                // Send fail: ép đóng socket cũ trước khi reconnect.
                SafeCloseAndNullUnsafe(false);

                for (int i = 0; i < SendReconnectRetries; i++)
                {
                    ok = await EnsureConnectedCoreUnsafeAsync(ConnectRetries, true).ConfigureAwait(false);
                    if (!ok)
                        break;

                    if (TrySendUnsafe(message))
                        return "";

                    SafeCloseAndNullUnsafe(false);
                }

                return "Error: Payment terminal is not responding. Please try again.";
            }
            finally
            {
                _gate.Release();
            }
        }

        private async Task<bool> EnsureConnectedCoreUnsafeAsync(int retries, bool showErrorOnlyOnLastAttempt)
        {
            if (_disposed)
                return false;

            if (IsConnected)
                return true;

            string url = _getUrl();

            if (string.IsNullOrWhiteSpace(url))
            {
                UiStatus("CodePay URL is empty.");
                _setConnectedFlag(false);
                return false;
            }

            int max = Math.Max(1, retries);

            for (int attempt = 1; attempt <= max; attempt++)
            {
                if (IsConnected)
                    return true;

                bool showError = (!showErrorOnlyOnLastAttempt) || (attempt == max);

                bool ok = await ConnectOnceUnsafeAsync(url, ConnectTimeoutMs, showError).ConfigureAwait(false);
                if (ok)
                    return true;

                await Task.Delay(250).ConfigureAwait(false);
            }

            return false;
        }

        private async Task<bool> ConnectOnceUnsafeAsync(string url, int timeoutMs, bool showErrorIfFail)
        {
            // Không reuse socket cũ.
            // Muốn connect lại thì invalidate/close/null socket cũ trước, rồi tạo WebSocket mới.
            SafeCloseAndNullUnsafe(false);

            // Tạo generation mới cho socket mới.
            int gen = Interlocked.Increment(ref _wsGeneration);
            WebSocket ws = null;

            try
            {
                ws = new WebSocket(url);

                ws.OnOpen += (sender, e) =>
                {
                    if (gen != _wsGeneration)
                        return;

                    _setConnectedFlag(true);
                    UiStatus("Payment terminal server connected");
                    _log("WebSocket opened. URL=" + url, "CodePay");

                    StopReconnectLoop();
                };

                ws.OnClose += (sender, e) =>
                {
                    if (gen != _wsGeneration)
                        return;

                    _setConnectedFlag(false);
                    UiStatus("Payment terminal server not connected");
                    _log("WebSocket closed. Code=" + e.Code + ", Reason=" + e.Reason, "CodePay");

                    if (!_manualDisconnect && !_disposed)
                        StartReconnectLoop();
                };

                ws.OnError += (sender, e) =>
                {
                    if (gen != _wsGeneration)
                        return;

                    _setConnectedFlag(false);

                    if (showErrorIfFail)
                        UiStatus("Payment terminal error: " + e.Message);

                    _log(e.Message, "CodePay OnError");
                };

                ws.OnMessage += (sender, e) =>
                {
                    if (gen != _wsGeneration)
                        return;

                    UiMessage(e.Data);
                };

                _setWs(ws);

                // Capture ws để closure không bị ảnh hưởng nếu _clientWebSocket bị đổi.
                // Instance này chỉ được Connect() đúng 1 lần.
                WebSocket capturedWs = ws;

                Task connectTask = Task.Run(() =>
                {
                    try
                    {
                        capturedWs.Connect();
                    }
                    catch (Exception ex)
                    {
                        if (gen == _wsGeneration)
                        {
                            _setConnectedFlag(false);

                            if (showErrorIfFail)
                                UiStatus("Error connecting to WebSocket server: " + ex.Message);
                        }

                        _log(ex.Message, "Connect Exception");
                    }
                });

                Task timeoutTask = Task.Delay(timeoutMs);
                Task done = await Task.WhenAny(connectTask, timeoutTask).ConfigureAwait(false);

                if (done != connectTask)
                {
                    if (gen == _wsGeneration)
                    {
                        // Quan trọng:
                        // Vô hiệu hóa callback của socket timeout.
                        // Nếu Connect() mở trễ sau timeout, OnOpen cũ sẽ bị ignore.
                        Interlocked.Increment(ref _wsGeneration);

                        _setConnectedFlag(false);

                        if (showErrorIfFail)
                            UiStatus("Unable to connect to the CodePay device. Connection timed out");

                        _log("Connection timed out. URL=" + url, "ConnectOnceUnsafeAsync");

                        if (object.ReferenceEquals(_getWs(), ws))
                            _setWs(null);

                        try
                        {
                            ws.CloseAsync();
                        }
                        catch
                        {
                            try { ws.Close(); } catch { }
                        }
                    }

                    return false;
                }

                var current = _getWs();

                if (!object.ReferenceEquals(current, ws))
                    return false;

                bool connected = ws.ReadyState == WebSocketState.Open;

                if (!connected)
                {
                    _setConnectedFlag(false);

                    if (object.ReferenceEquals(_getWs(), ws))
                        _setWs(null);

                    try
                    {
                        ws.CloseAsync();
                    }
                    catch
                    {
                        try { ws.Close(); } catch { }
                    }
                }

                return connected;
            }
            catch (Exception ex)
            {
                _setConnectedFlag(false);

                if (showErrorIfFail)
                    UiStatus("Error connecting to WebSocket server: " + ex.Message);

                _log(ex.Message, "ConnectOnceUnsafeAsync Exception");

                if (object.ReferenceEquals(_getWs(), ws))
                    _setWs(null);

                try
                {
                    if (ws != null)
                        ws.CloseAsync();
                }
                catch
                {
                    try
                    {
                        if (ws != null)
                            ws.Close();
                    }
                    catch { }
                }

                return false;
            }
        }

        private void StartReconnectLoop()
        {
            if (_manualDisconnect || _disposed)
                return;

            if (Interlocked.Exchange(ref _reconnectLoopStarted, 1) == 1)
                return;

            StopReconnectLoopOnly();

            _reconnectCts = new CancellationTokenSource();
            CancellationToken token = _reconnectCts.Token;

            Task.Run(async () =>
            {
                int delayMs = 1000;

                try
                {
                    while (!token.IsCancellationRequested && !_manualDisconnect && !_disposed)
                    {
                        try
                        {
                            await Task.Delay(delayMs, token).ConfigureAwait(false);

                            await _gate.WaitAsync(token).ConfigureAwait(false);

                            try
                            {
                                if (IsConnected)
                                    return;

                                await EnsureConnectedCoreUnsafeAsync(1, false).ConfigureAwait(false);

                                if (IsConnected)
                                    return;
                            }
                            finally
                            {
                                _gate.Release();
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            return;
                        }
                        catch (Exception ex)
                        {
                            _log(ex.Message, "StartReconnectLoop Exception");
                        }

                        delayMs = Math.Min(delayMs * 2, 15000);
                    }
                }
                finally
                {
                    Interlocked.Exchange(ref _reconnectLoopStarted, 0);
                }
            }, token);
        }

        private void StopReconnectLoop()
        {
            StopReconnectLoopOnly();
            Interlocked.Exchange(ref _reconnectLoopStarted, 0);
        }

        private void StopReconnectLoopOnly()
        {
            try
            {
                if (_reconnectCts != null)
                {
                    _reconnectCts.Cancel();
                    _reconnectCts.Dispose();
                    _reconnectCts = null;
                }
            }
            catch { }
        }

        private bool TrySendUnsafe(string message)
        {
            try
            {
                var ws = _getWs();

                if (ws == null || ws.ReadyState != WebSocketState.Open)
                    return false;

                ws.Send(message);

                _log(message, "Send Data JSON");
                return true;
            }
            catch (Exception ex)
            {
                _log(ex.Message, "Send Exception");
                _setConnectedFlag(false);
                return false;
            }
        }

        private void SafeCloseAndNullUnsafe(bool updateUi)
        {
            WebSocket ws = null;

            try
            {
                ws = _getWs();
            }
            catch { }

            // Quan trọng:
            // Invalidate callback của socket cũ trước khi Close().
            // Nếu Close() bắn OnClose/OnError thì handler cũ sẽ tự ignore.
            Interlocked.Increment(ref _wsGeneration);

            // Cắt reference trước để code khác không reuse socket cũ nữa.
            try
            {
                _setWs(null);
            }
            catch { }

            try
            {
                _setConnectedFlag(false);
            }
            catch { }

            if (ws != null)
            {
                try
                {
                    ws.CloseAsync();
                }
                catch
                {
                    try { ws.Close(); } catch { }
                }
            }

            if (updateUi)
                UiStatus("Disconnected from payment terminal");
        }

        private void UiStatus(string text)
        {
            try
            {
                if (_ui != null && !_ui.IsDisposed && _ui.IsHandleCreated && _ui.InvokeRequired)
                    _ui.BeginInvoke(new Action(() => _uiStatus(text)));
                else
                    _uiStatus(text);
            }
            catch { }
        }

        private void UiMessage(string data)
        {
            try
            {
                if (_ui != null && !_ui.IsDisposed && _ui.IsHandleCreated && _ui.InvokeRequired)
                    _ui.BeginInvoke(new Action(() => _onMessage(data)));
                else
                    _onMessage(data);
            }
            catch { }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _manualDisconnect = true;

            StopReconnectLoop();

            try
            {
                _gate.Wait();

                try
                {
                    SafeCloseAndNullUnsafe(false);
                }
                finally
                {
                    _gate.Release();
                }
            }
            catch { }

            try { _gate.Dispose(); } catch { }
        }
    }
}