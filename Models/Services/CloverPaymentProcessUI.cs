using NailsChekin.Popup;
using com.clover.remotepay.sdk;
using com.clover.remotepay.transport;
using NailsChekin.Popup;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NailsChekin.Models.Services
{
    public sealed class CloverPaymentProcessUI
    {
        private readonly Control _invoker;                    // form chính để BeginInvoke
        private readonly Func<InputOption, EventHandler> _getHandler;
        private readonly Action _cancelDevice;                // hủy/abort giao dịch trực tiếp trên máy (ResetDevice)

        private FormCloverProcessing _popup;
        private bool _active;                                  // <-- cờ phiên thanh toán
        private const string DynTag = "CLOVER_DYN";

        public CloverPaymentProcessUI(Control invoker, Func<InputOption, EventHandler> getHandler, Action cancelDevice = null)
        {
            _invoker = invoker ?? throw new ArgumentNullException(nameof(invoker));
            _getHandler = getHandler ?? throw new ArgumentNullException(nameof(getHandler));
            _cancelDevice = cancelDevice;
        }

        public void StartPayment(IWin32Window owner)
        {
            _invoker.BeginInvoke((Action)(() =>
            {
                if (_active) return;
                _active = true;

                if (_popup == null || _popup.IsDisposed)
                {
                    _popup = new FormCloverProcessing();
                    _popup.CancelFallback = _cancelDevice;   // X khi chưa có nút Cancel -> ResetDevice
                    _popup.FormClosed += (_, __) => { _popup = null; _active = false; };
                    // Căn giữa ngay khi form hiển thị xong (đảm bảo kích thước đã tính)
                    _popup.Shown += (_, __) => CenterPopup(owner);
                }

                ClearDynamicButtonsInternal();
                _popup.StatusHost.Text = "Đang kết nối Clover…";

                if (owner is Form f) _popup.Show(f); else _popup.Show();

                // Căn giữa lần nữa (phòng khi bạn muốn thấy hiệu lực ngay)
                CenterPopup(owner);
                _popup.BringToFront();
            }));
        }

        public void StopPayment()
        {
            _invoker.BeginInvoke((Action)(() =>
            {
                _active = false;
                if (_popup != null && !_popup.IsDisposed) _popup.Close();
                _popup = null;
            }));
        }

        public void HandleDeviceActivityStart(CloverDeviceEvent deviceEvent)
        {
            _invoker.BeginInvoke((Action)(() =>
            {
                // KHÔNG mở popup nếu chưa StartPayment()
                if (!_active || _popup == null || _popup.IsDisposed) return;

                var panel = _popup.ButtonsHost;

                // xóa nút động cũ (giữ nút gốc)
                var toRemove = new List<Control>();
                foreach (Control c in panel.Controls)
                    if (c.Tag as string == DynTag) toRemove.Add(c);
                foreach (var c in toRemove) panel.Controls.Remove(c);

                // thêm nút động
                if (deviceEvent?.InputOptions != null)
                {
                    panel.SuspendLayout();
                    foreach (var io in deviceEvent.InputOptions)
                    {
                        string text = io.description?.Trim() ?? string.Empty;
                        string lower = text.ToLowerInvariant();

                        var btn = new Button
                        {
                            Text = text.ToUpper(),
                            FlatStyle = FlatStyle.Flat,
                            ForeColor = Color.White,
                            // TẮT autosize để tự đặt kích thước
                            AutoSize = false,
                            // Kích thước gợi ý (to, dễ bấm)
                            Size = new Size(250, 72),
                            MinimumSize = new Size(180, 60),
                            // Font to, đậm
                            Font = new Font("Segoe UI", 18f, FontStyle.Bold),
                            // Khoảng cách giữa các nút & khoảng đệm trong nút
                            Margin = new Padding(12, 8, 12, 16),
                            Padding = new Padding(18, 12, 18, 12),
                            TextAlign = ContentAlignment.MiddleCenter,
                            Tag = DynTag
                        };
                        btn.FlatAppearance.BorderSize = 0;         // viền phẳng
                        btn.UseCompatibleTextRendering = true;     // render mượt hơn với font lớn

                        if (lower == "no receipt" || lower == "cancel" || lower == "delete")
                            btn.BackColor = Color.Red;
                        else if (lower == "print")
                            btn.BackColor = Color.Green;
                        else
                            btn.BackColor = Color.Orange;

                        btn.Click += _getHandler(io);
                        panel.Controls.Add(btn);
                    }
                    panel.ResumeLayout();
                }

                // message + partial authorization
                string msg = deviceEvent?.Message ?? string.Empty;
                if (msg.IndexOf("partial authorization", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var m = Regex.Match(msg, @"partial authorization for\s*\$?\s*([0-9]+(?:\.[0-9]{1,2})?)",RegexOptions.IgnoreCase);
                    string amount = m.Success ? m.Groups[1].Value : "…";
                    msg += Environment.NewLine + $"Thẻ của khách hàng chỉ có ${amount}. Bạn hỏi khách sẽ trả phần thiếu bằng thẻ khác hay tiền mặt?";
                }

                _popup.StatusHost.Text = msg;
            }));
        }

        public void SetStatus(string message)
        {
            _invoker.BeginInvoke((Action)(() =>
            {
                if (_active && _popup != null && !_popup.IsDisposed)
                    _popup.StatusHost.Text = message ?? string.Empty;
            }));
        }

        private void ClearDynamicButtonsInternal()
        {
            if (_popup == null || _popup.IsDisposed) return;
            var panel = _popup.ButtonsHost;
            var toRemove = new List<Control>();
            foreach (Control c in panel.Controls)
                if (c.Tag as string == DynTag) toRemove.Add(c);
            foreach (var c in toRemove) panel.Controls.Remove(c);
        }

        private void CenterPopup(IWin32Window owner)
        {
            if (_popup == null || _popup.IsDisposed) return;

            _popup.StartPosition = FormStartPosition.Manual;

            Rectangle wa;
            if (owner is Control c && c.Visible)
                wa = Screen.FromControl(c).WorkingArea;      // màn hình chứa form chính
            else
                wa = Screen.FromPoint(Cursor.Position).WorkingArea; // fallback

            var x = wa.Left + (wa.Width - _popup.Width) / 2;
            var y = wa.Top + (wa.Height - _popup.Height) / 2;
            _popup.Location = new Point(x, y);
        }

        public void Dispose() => StopPayment();

    }

}
