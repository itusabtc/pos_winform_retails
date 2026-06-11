using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;          // ISynchronizeInvoke
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using SocketIOClient;

namespace NailsChekin.Models.Helper
{
    public sealed class SocketIoClientHelper : IDisposable
    {
        private readonly SocketIOClient.SocketIO _socket;
        private readonly ISynchronizeInvoke _invoker;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private readonly TimeSpan _connectTimeout;
        private readonly TimeSpan _pingInterval;
        private readonly TimeSpan _pongTimeout;
        private readonly int _maxBackoffSeconds;
        private readonly bool _fallbackToPolling; // dự phòng nếu cần (chỉ đổi được nếu bản lib hỗ trợ Transport)
        private readonly string _path;
        private readonly int _eio;

        private readonly ConcurrentQueue<KeyValuePair<string, object>> _emitQueue =
            new ConcurrentQueue<KeyValuePair<string, object>>();
        private readonly SemaphoreSlim _emitGate = new SemaphoreSlim(1, 1);

        private Task _bgTask;
        private volatile bool _userStopRequested;
        private volatile bool _disposed;

        public bool IsConnected { get { return _socket.Connected; } }
        public string Url { get; private set; }

        // Sự kiện public cho app
        public event Action Connected;
        public event Action Disconnected;
        public event Action<long> ReconnectAttempt;
        public event Action<string> MessageReceived;
        public event Action<Exception> Error;

        /// <summary>
        /// Đăng ký (lại) các kênh custom sau mỗi lần connect thành công.
        /// </summary>
        public Action<SocketIOClient.SocketIO> Resubscribe { get; set; }

        public SocketIoClientHelper(
            string url,
            SocketIOClient.SocketIOOptions options = null,
            ISynchronizeInvoke invoker = null,
            int engineIoVersion = 4,
            string path = "/socket.io/",
            TimeSpan? connectTimeout = null,
            TimeSpan? pingInterval = null,
            TimeSpan? pongTimeout = null,
            int maxBackoffSeconds = 30,
            bool fallbackToPolling = true)
        {
            if (url == null) throw new ArgumentNullException("url");

            Url = url;
            _invoker = invoker;

            _connectTimeout = connectTimeout ?? TimeSpan.FromSeconds(6);
            _pingInterval = pingInterval ?? TimeSpan.FromSeconds(25);
            _pongTimeout = pongTimeout ?? TimeSpan.FromSeconds(12);
            _maxBackoffSeconds = maxBackoffSeconds < 5 ? 5 : maxBackoffSeconds;
            _fallbackToPolling = fallbackToPolling;
            _path = string.IsNullOrEmpty(path) ? "/socket.io/" : path;
            _eio = engineIoVersion;

            if (options == null) options = new SocketIOClient.SocketIOOptions();

            // Đặt option bằng reflection để tương thích mọi bản
            TrySetProp(options, "Path", _path);                       // string
            TrySetProp(options, "Reconnection", false);               // bool (ta tự quản)
            TrySetProp(options, "EIO", _eio);                         // int (có bản không có)
            TrySetTransport(options, "WebSocket");                    // enum TransportProtocol nếu có
            TrySetTimeout(options, "ConnectionTimeout", _connectTimeout);

            _socket = new SocketIOClient.SocketIO(Url, options);

            // Kênh "message" (nếu server có gửi)
            _socket.On("message", delegate (SocketIOClient.SocketIOResponse res)
            {
                string msg = null;
                try { msg = res.GetValue<string>(); } catch { }
                var capture = msg;
                RaiseOnUI(delegate { if (MessageReceived != null) MessageReceived(capture); });
            });
        }

        public async Task StartAsync()
        {
            ThrowIfDisposed();
            if (_bgTask != null) return;

            _userStopRequested = false;
            var linked = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token);
            _bgTask = Task.Run(() => RunLoopAsync(linked.Token), linked.Token);
            await Task.Yield();
        }

        public async Task StopAsync()
        {
            if (_disposed) return;
            _userStopRequested = true;
            try
            {
                _cts.Cancel();
                if (_bgTask != null)
                    await Task.WhenAny(_bgTask, Task.Delay(2000));
            }
            catch { }

            try { await _socket.DisconnectAsync(); } catch { }
        }

        /// <summary>Emit: nếu đang offline sẽ xếp hàng, gửi bù sau khi kết nối lại.</summary>
        public async Task EmitAsync(string eventName, object data)
        {
            ThrowIfDisposed();

            if (IsConnected)
            {
                await SafeEmitInternal(eventName, data);
                return;
            }

            _emitQueue.Enqueue(new KeyValuePair<string, object>(eventName, data));
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            try { _cts.Cancel(); } catch { }
            try { _socket.Dispose(); } catch { }
            _emitGate.Dispose();
        }

        // ================== INTERNALS ==================

        private async Task RunLoopAsync(CancellationToken ct)
        {
            var rnd = new Random();
            int attempt = 0;
            DateTime lastActivityUtc = DateTime.UtcNow;

            Action touch = delegate { lastActivityUtc = DateTime.UtcNow; };

            while (!ct.IsCancellationRequested && !_userStopRequested)
            {
                try
                {
                    attempt++;
                    RaiseOnUI(delegate { if (ReconnectAttempt != null) ReconnectAttempt(attempt); });

                    await ConnectWithTimeoutAsync(ct);
                    touch();

                    RaiseOnUI(delegate { if (Connected != null) Connected(); });

                    // (re)subscribe kênh custom của app
                    try { if (Resubscribe != null) Resubscribe(_socket); } catch (Exception ex) { RaiseOnUI(delegate { if (Error != null) Error(ex); }); }

                    // gửi bù emit còn hàng
                    await DrainEmitQueueAsync(ct);

                    // vòng keep-alive
                    while (IsConnected && !ct.IsCancellationRequested && !_userStopRequested)
                    {
                        // half-open: không có activity quá lâu (pingInterval + pongTimeout)
                        if (DateTime.UtcNow - lastActivityUtc > _pongTimeout + _pingInterval)
                        {
                            RaiseOnUI(delegate { if (Error != null) Error(new TimeoutException("Half-open detected; force reconnect")); });
                            break;
                        }

                        // ping nhẹ (đổi event phù hợp server bạn nếu cần)
                        try
                        {
                            await SafeEmitInternal("__pos_ping__", null);
                        }
                        catch (Exception ex)
                        {
                            RaiseOnUI(delegate { if (Error != null) Error(ex); });
                            break;
                        }

                        await Task.Delay(_pingInterval, ct);
                        touch();
                    }

                    // rơi khỏi vòng trong ⇒ coi như mất kết nối
                    if (!ct.IsCancellationRequested && !_userStopRequested)
                    {
                        RaiseOnUI(delegate { if (Disconnected != null) Disconnected(); });
                        try { await _socket.DisconnectAsync(); } catch { }
                    }

                    // reset số lần attempt khi đã từng vào được trạng thái connected
                    if (IsConnected == false) { /* giữ attempt để backoff */ }
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    RaiseOnUI(delegate { if (Error != null) Error(ex); });
                    RaiseOnUI(delegate { if (Disconnected != null) Disconnected(); });
                }

                if (ct.IsCancellationRequested || _userStopRequested) break;

                // exponential backoff + jitter
                int backoff = attempt <= 1 ? 1 : (int)Math.Pow(2, Math.Min(5, attempt - 1)); // 1,2,4,8,16,32
                if (backoff > _maxBackoffSeconds) backoff = _maxBackoffSeconds;
                int jitterMs = rnd.Next(0, 1000);
                await Task.Delay(TimeSpan.FromSeconds(backoff));
                await Task.Delay(jitterMs);

                // Tùy chọn đổi Transport sang Polling sau vài lần fail (chỉ hoạt động nếu bản lib expose Transport)
                if (_fallbackToPolling && attempt >= 3)
                {
                    TrySetTransport(_socket.Options, "Polling");
                }
            }
        }

        private async Task ConnectWithTimeoutAsync(CancellationToken ct)
        {
            // luôn đảm bảo các option khớp (nếu bản lib có)
            TrySetProp(_socket.Options, "EIO", _eio);
            TrySetProp(_socket.Options, "Path", _path);

            var connectTask = _socket.ConnectAsync(); // hầu hết bản không có overload CT
            var finished = await Task.WhenAny(connectTask, Task.Delay(_connectTimeout, ct));
            if (finished != connectTask)
            {
                try { await _socket.DisconnectAsync(); } catch { }
                throw new TimeoutException("SocketIO connect timeout " + _connectTimeout.TotalSeconds + "s");
            }
            await connectTask; // propagate lỗi nếu có
        }

        private async Task DrainEmitQueueAsync(CancellationToken ct)
        {
            while (!_emitQueue.IsEmpty && IsConnected && !ct.IsCancellationRequested)
            {
                KeyValuePair<string, object> item;
                if (_emitQueue.TryDequeue(out item))
                {
                    try { await SafeEmitInternal(item.Key, item.Value); }
                    catch (Exception ex) { RaiseOnUI(delegate { if (Error != null) Error(ex); }); break; }
                }
                else break;
            }
        }

        private async Task SafeEmitInternal(string eventName, object data)
        {
            await _emitGate.WaitAsync();
            try
            {
                if (data == null) await _socket.EmitAsync(eventName);
                else await _socket.EmitAsync(eventName, data);
            }
            finally { _emitGate.Release(); }
        }

        private void RaiseOnUI(Action a)
        {
            if (a == null) return;
            if (_invoker != null && _invoker.InvokeRequired) _invoker.BeginInvoke(a, null);
            else a();
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException("SocketIoClientHelper");
        }

        // --------- Helpers set option bằng Reflection (an toàn mọi bản) ---------

        private static void TrySetProp(object obj, string name, object value)
        {
            try
            {
                var t = obj.GetType();
                var p = t.GetProperty(name);
                if (p == null || !p.CanWrite) return;

                var pt = p.PropertyType;

                if (value != null && !pt.IsAssignableFrom(value.GetType()))
                {
                    // hỗ trợ TimeSpan <-> int(ms)
                    if (pt == typeof(TimeSpan) && value is TimeSpan ts)
                    {
                        p.SetValue(obj, ts, null); return;
                    }
                    if (pt == typeof(int) && value is TimeSpan ts2)
                    {
                        p.SetValue(obj, (int)ts2.TotalMilliseconds, null); return;
                    }
                    if (pt.IsEnum && value is string s)
                    {
                        var enumVal = Enum.Parse(pt, s, true);
                        p.SetValue(obj, enumVal, null); return;
                    }

                    var converted = Convert.ChangeType(value, pt);
                    p.SetValue(obj, converted, null);
                    return;
                }

                p.SetValue(obj, value, null);
            }
            catch { /* ignore if not supported */ }
        }

        private static void TrySetTransport(object options, string transportName)
        {
            // tìm property Transport (enum) và gán bằng tên "WebSocket"/"Polling"
            try
            {
                var p = options.GetType().GetProperty("Transport");
                if (p == null || !p.CanWrite) return;
                var enumType = p.PropertyType;
                if (!enumType.IsEnum) return;
                var enumVal = Enum.Parse(enumType, transportName, true);
                p.SetValue(options, enumVal, null);
            }
            catch { }
        }

        private static void TrySetTimeout(object options, string propName, TimeSpan timeout)
        {
            try
            {
                var p = options.GetType().GetProperty(propName);
                if (p == null || !p.CanWrite) return;
                var pt = p.PropertyType;

                if (pt == typeof(TimeSpan))
                {
                    p.SetValue(options, timeout, null);
                }
                else if (pt == typeof(int))
                {
                    p.SetValue(options, (int)timeout.TotalMilliseconds, null);
                }
            }
            catch { }
        }
    }
}
