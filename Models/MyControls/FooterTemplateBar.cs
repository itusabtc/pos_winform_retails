namespace NailsChekin.MyControls
{
    using NailsChekin.Models.Helper;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    #region Contracts
    public enum MainCommand
    {
        Dashboard, Turn, CheckIn, Customer, BackOffice, Payment, GiftCard,
        ApptBook, Adjust, CloseOut, CashDrawer, BuySupply, ItemEntry, ItemLookUp, Inventory, Sales, OpenSale
    }

    public sealed class MenuItemDef
    {
        public string Text { get; set; }
        public MainCommand Command { get; set; }
        public Color? BackColor { get; set; }
        public Color? ForeColor { get; set; }
        public int? CornerRadius { get; set; }
    }

    public sealed class MenuItemClickedEventArgs : EventArgs
    {
        public MainCommand Command { get; internal set; }
        public string Text { get; internal set; }
        public ButtonRound Button { get; internal set; }
    }
    #endregion

    #region Internal flow container (chống flicker + pan/scroll)
    internal class NoFlickerFlow : FlowLayoutPanel
    {
        // --- Hide scrollbars
        [DllImport("user32.dll")] private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);
        private const int SB_HORZ = 0, SB_VERT = 1;

        // --- Touch
        private const int WM_TOUCH = 0x0240;
        [DllImport("user32.dll")] private static extern bool RegisterTouchWindow(IntPtr hWnd, uint ulFlags);
        [DllImport("user32.dll")] private static extern bool GetTouchInputInfo(IntPtr hTouchInput, int cInputs, [In, Out] TOUCHINPUT[] pInputs, int cbSize);
        [DllImport("user32.dll")] private static extern void CloseTouchInputHandle(IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct TOUCHINPUT
        {
            public int x, y; public IntPtr hSource; public int dwID, dwFlags, dwMask, dwTime;
            public IntPtr dwExtraInfo; public int cxContact, cyContact;
        }
        private const int TOUCHEVENTF_DOWN = 0x0001;
        private const int TOUCHEVENTF_UP = 0x0002;
        private static int T2P(int v) { return v / 100; }
        private static int LOWORD(IntPtr p) { return (int)((long)p & 0xFFFF); }

        public bool HideScrollbars { get; set; } = true;

        // --- Mouse pan state
        private bool _dragging;
        private Point _last;

        // --- Touch pan state
        private int? _touchId;
        private Point _lastTouch;
        private TOUCHINPUT[] _touchBuf;

        // --- Suppress click per-control ngay sau khi pan + cooldown start-pan
        private Control _suppressClickFor = null;
        private int _suppressUntilTick = 0;
        private int _panCooldownUntilTick = 0;

        // --- Chống double-wire
        private readonly HashSet<Control> _panningWired = new HashSet<Control>();

        public NoFlickerFlow()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            WrapContents = false;
            AutoScroll = true;
            FlowDirection = FlowDirection.LeftToRight;
            Padding = new Padding(12, 6, 12, 6);
            Margin = Padding.Empty;
            BackColor = Color.Transparent;

            SetStyle(ControlStyles.Selectable | ControlStyles.UserMouse, true);
            Cursor = Cursors.Hand;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (IsHandleCreated)
            {
                try { RegisterTouchWindow(Handle, 0); } catch { /* ignore */ }
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_TOUCH)
            {
                HandleTouchFromThis(m.LParam);
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);

            if (HideScrollbars && IsHandleCreated)
            {
                ShowScrollBar(Handle, SB_HORZ, false);
                ShowScrollBar(Handle, SB_VERT, false);
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (HideScrollbars && IsHandleCreated)
            {
                ShowScrollBar(Handle, SB_HORZ, false);
                ShowScrollBar(Handle, SB_VERT, false);
            }
        }

        // ===== Mouse panning trên panel
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _dragging = true;
            _last = e.Location;
            Capture = true;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!_dragging) return;
            PanBy(e.X - _last.X);
            _last = e.Location;
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _dragging = false;
            Capture = false;
        }

        // Wheel → cuộn ngang
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            int step = 40 * Math.Sign(e.Delta);
            PanBy(step);
        }

        // Không auto-scroll khi child focus
        protected override Point ScrollToControl(Control activeControl)
        {
            return DisplayRectangle.Location;
        }

        private void PanBy(int dx)
        {
            int curX = -AutoScrollPosition.X;
            int newX = Math.Max(0, curX - dx); // kéo phải → nội dung đi theo
            AutoScrollPosition = new Point(newX, -AutoScrollPosition.Y);
        }

        // ===== Touch từ panel
        private void HandleTouchFromThis(IntPtr lParam)
        {
            int count = LOWORD(lParam);
            if (_touchBuf == null || _touchBuf.Length < count)
                _touchBuf = new TOUCHINPUT[count];

            bool ok = GetTouchInputInfo(lParam, count, _touchBuf, Marshal.SizeOf(typeof(TOUCHINPUT)));
            if (!ok) { CloseTouchInputHandle(lParam); return; }

            for (int i = 0; i < count; i++)
            {
                var ti = _touchBuf[i];
                var pt = PointToClient(new Point(T2P(ti.x), T2P(ti.y)));

                bool isDown = (ti.dwFlags & TOUCHEVENTF_DOWN) == TOUCHEVENTF_DOWN;
                bool isUp = (ti.dwFlags & TOUCHEVENTF_UP) == TOUCHEVENTF_UP;
                bool isMove = !isDown && !isUp;

                if (isDown && _touchId == null)
                {
                    _touchId = ti.dwID;
                    _lastTouch = pt;
                }
                else if (isMove && _touchId == ti.dwID)
                {
                    PanBy(pt.X - _lastTouch.X);
                    _lastTouch = pt;
                }
                else if (isUp && _touchId == ti.dwID)
                {
                    _touchId = null;
                    // Kết thúc một cử chỉ pan trên nền panel → chặn start-pan mới rất ngắn
                    _panCooldownUntilTick = Environment.TickCount + 140;
                }
            }

            CloseTouchInputHandle(lParam);
        }

        // ===== API cho child controls: pan bằng chuột
        public void StartPanFromChild(Control child, MouseEventArgs e)
        {
            _dragging = true;
            _last = PointToClient(child.PointToScreen(e.Location));
            Capture = true;
        }
        public void DragFromChild(Control child, MouseEventArgs e)
        {
            if (!_dragging) return;
            var cur = PointToClient(child.PointToScreen(e.Location));
            PanBy(cur.X - _last.X);
            _last = cur;
        }
        public void EndPanFromChild(Control child)
        {
            if (_dragging)
            {
                _dragging = false;
                Capture = false;
                // suppress click đúng 1 lần cho CHÍNH control vừa pan
                _suppressClickFor = child;
                _suppressUntilTick = Environment.TickCount + 140;   // thời gian nuốt 1 click
                _panCooldownUntilTick = Environment.TickCount + 140; // tránh khởi động pan mới ngay
            }
        }

        // Nuốt click 1 lần cho đúng control
        public bool ConsumeSuppressClick(Control c)
        {
            if (c != null &&
                ReferenceEquals(_suppressClickFor, c) &&
                Environment.TickCount < _suppressUntilTick)
            {
                _suppressClickFor = null; // consume-once
                return true;
            }
            return false;
        }

        // Gắn panning cho child (không đăng ký WM_TOUCH trên child để click vẫn hoạt động)
        public void EnableChildPanning(Control c)
        {
            if (_panningWired.Contains(c)) return;
            _panningWired.Add(c);
            c.Disposed += (s, e) => _panningWired.Remove(c);

            bool started = false;
            Rectangle dragBox = Rectangle.Empty;

            c.MouseDown += (sender, e) =>
            {
                started = false;
                Size sz = SystemInformation.DragSize; // DPI-aware threshold
                dragBox = new Rectangle(e.X - sz.Width / 2, e.Y - sz.Height / 2, sz.Width, sz.Height);
            };

            c.MouseMove += (sender, e) =>
            {
                if (Environment.TickCount < _panCooldownUntilTick) return;

                if (!started)
                {
                    if (!dragBox.Contains(e.Location))
                    {
                        StartPanFromChild(c, e);
                        c.Capture = true;
                        started = true;
                    }
                    return;
                }
                DragFromChild(c, e);
            };

            EventHandler finish = (sender, e) =>
            {
                if (!started) return;
                EndPanFromChild(c);
                c.Capture = false;
                started = false;
            };

            c.MouseUp += (sender, e) => finish(sender, e);
            c.MouseCaptureChanged += finish;
            c.Leave += finish;
        }
    }
    #endregion

    /// <summary>
    /// Thanh nút template cho footer/header – dàn nút ButtonRound linh hoạt theo chiều ngang.
    /// </summary>
    public class FooterTemplateBar
    {
        public enum ButtonWidthMode { Equal, ByText, ByTextFill }

        public event EventHandler<MenuItemClickedEventArgs> ItemClicked;

        private readonly NoFlickerFlow _host;
        private readonly Dictionary<ButtonRound, MenuItemDef> _map = new Dictionary<ButtonRound, MenuItemDef>();
        private readonly Dictionary<ButtonRound, int> _intrinsicWidth = new Dictionary<ButtonRound, int>();
        private bool _layoutScheduled;

        // ---- Style chung
        public int ButtonHeight { get; set; } = 48;
        public int MinButtonWidth { get; set; } = 120;
        public float FontSize { get; set; } = 12f;
        public int Spacing { get; set; } = 12;
        public int DefaultCornerRadius { get; set; } = 18;

        // Màu bình thường
        public Color DefaultBackColor { get; set; } = Color.FromArgb(86, 136, 195);
        public Color DefaultForeColor { get; set; } = Color.White;
        public Color DefaultBorderColor { get; set; } = Color.FromArgb(60, 110, 170);

        // Màu khi chọn
        public Color SelectedBackColor { get; set; } = Color.FromArgb(255, 152, 0);
        public Color SelectedForeColor { get; set; } = Color.White;
        public Color SelectedBorderColor { get; set; } = Color.FromArgb(230, 120, 0);

        // Phân bổ chiều ngang
        public ButtonWidthMode WidthMode { get; set; } = ButtonWidthMode.Equal;
        public bool AllowOverflow { get; set; } = true;

        private int _sideInset = 16;
        public int SideInset
        {
            get { return _sideInset; }
            set { _sideInset = Math.Max(0, value); _host.Padding = new Padding(_sideInset, 6, _sideInset, 6); ScheduleLayout(); }
        }

        public Control Host { get { return _host; } }
        public MainCommand? DefaultSelectedCommand { get; set; } = MainCommand.Payment;

        public FooterTemplateBar(Control host)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));
            _host = new NoFlickerFlow { Dock = DockStyle.Fill, Padding = new Padding(_sideInset, 6, _sideInset, 6) };
            host.Controls.Add(_host);

            // Tự động bật panning cho ButtonRound vừa add
            _host.ControlAdded += (s, e) =>
            {
                if (e.Control is ButtonRound)
                    _host.EnableChildPanning(e.Control);
                ScheduleLayout();
            };

            _host.SizeChanged += (s, e) => ScheduleLayout();
            _host.ControlRemoved += (s, e) => ScheduleLayout();
        }

        // ===== Build items
        public void SetItems(IEnumerable<MenuItemDef> items)
        {
            _host.SuspendLayout();
            _host.Controls.Clear();
            _map.Clear();
            _intrinsicWidth.Clear();

            foreach (var it in items)
            {
                var btn = CreateButton(it);
                _host.Controls.Add(btn);
                _map[btn] = it;
            }

            _host.ResumeLayout(true);
            ScheduleLayout();

            // Chọn mặc định
            ButtonRound target =
                _map.FirstOrDefault(kv => kv.Value.Command == (DefaultSelectedCommand ?? _map.Values.First().Command)).Key
                ?? _host.Controls.OfType<ButtonRound>().FirstOrDefault();

            if (target != null) SelectButton(target);
        }

        public void SelectCommand(MainCommand cmd)
        {
            foreach (var kv in _map)
            {
                bool isSel = (kv.Value.Command == cmd);
                kv.Key.Selected = isSel;
                ApplyVisual(kv.Key, kv.Value, isSel);
            }
            _currentSelected = cmd;
        }

        public void PerformClick(MainCommand cmd)
        {
            if (_lockActive || _eatClicks)
            {
                SetPending(cmd, true);
                return;
            }
            var kv = _map.FirstOrDefault(x => x.Value.Command == cmd);
            if (kv.Key == null) return;

            DisableAllExcept(cmd, true);
            _host.Update();
            SelectButton(kv.Key);

            ItemClicked?.Invoke(this, new MenuItemClickedEventArgs
            {
                Command = cmd,
                Text = kv.Value.Text,
                Button = kv.Key
            });
        }

        // ===== Private
        private ButtonRound CreateButton(MenuItemDef it)
        {
            var btn = new ButtonRound
            {
                FooterMode = true,                  // ⬅️ chỉ áp dụng khi dùng cho footer
                Title = it.Text,
                TitleFontSize = FontSize,
                CornerRadius = it.CornerRadius ?? DefaultCornerRadius,
                Height = ButtonHeight,
                TitleBackColor = it.BackColor ?? DefaultBackColor,
                TitleForeColor = it.ForeColor ?? DefaultForeColor,
                BorderColor = DefaultBorderColor,
                BorderWidth = 1f,
                ButtonPadding = LayoutHelper.mini_screen ? new Padding(0, 10, 0, 10) : new Padding(0, 10, 0, 10),
                Margin = new Padding(Spacing / 2, 6, Spacing / 2, 6)
            };

            using (var f = new Font("Segoe UI", FontSize, FontStyle.Bold))
            {
                // Đo có tính overhang + thêm 2 bên 1px (Windows dùng nội bộ)
                var flags = TextFormatFlags.SingleLine
                          | TextFormatFlags.NoPrefix
                          | TextFormatFlags.GlyphOverhangPadding
                          | TextFormatFlags.LeftAndRightPadding;

                var sz = TextRenderer.MeasureText(it.Text ?? string.Empty, f, Size.Empty, flags);

                const int SAFETY = 4; // buffer chống “cắt mép” do làm tròn/bo góc
                int extra = btn.ButtonPadding.Left + btn.ButtonPadding.Right + SAFETY;

                btn.Width = Math.Max(MinButtonWidth, sz.Width + extra);
                _intrinsicWidth[btn] = btn.Width;
            }

            // Gắn pan cho chính nút
            _host.EnableChildPanning(btn);

            // Click: nuốt đúng 1 lần ngay sau khi pan của nút này
            btn.CardClick += (s, e) =>
            {
                if (_host.ConsumeSuppressClick(btn)) return;
                if (_eatClicks || !btn.Enabled) return;

                SelectButton(btn);
                var def = _map[btn];
                ItemClicked?.Invoke(this, new MenuItemClickedEventArgs
                {
                    Command = def.Command,
                    Text = def.Text,
                    Button = btn
                });
            };

            return btn;
        }

        private void SelectButton(ButtonRound btn)
        {
            foreach (ButtonRound b in _host.Controls.OfType<ButtonRound>())
            {
                bool isSel = (b == btn);
                b.Selected = isSel;
                var def = _map[b];
                ApplyVisual(b, def, isSel);
                if (isSel) _currentSelected = def.Command;
            }
        }

        private void ApplyVisual(ButtonRound b, MenuItemDef def, bool isSelected)
        {
            if (isSelected)
            {
                b.TitleBackColor = SelectedBackColor;
                b.TitleForeColor = SelectedForeColor;
                b.BorderColor = SelectedBorderColor;
            }
            else
            {
                b.TitleBackColor = def.BackColor ?? DefaultBackColor;
                b.TitleForeColor = def.ForeColor ?? DefaultForeColor;
                b.BorderColor = DefaultBorderColor;
            }
            b.Invalidate();
        }

        private void ScheduleLayout()
        {
            if (_layoutScheduled) return;
            _layoutScheduled = true;
            _host.BeginInvoke((Action)(() => { _layoutScheduled = false; LayoutHorizontally(); }));
        }

        // đảm bảo scrollbar ngang (dù ẩn) hoạt động đúng
        private void EnsureHorizontalScroll(int contentWidth)
        {
            bool overflow = contentWidth > _host.ClientSize.Width;
            _host.WrapContents = false;
            _host.AutoScroll = overflow;
            _host.AutoScrollMinSize = overflow
                ? new Size(Math.Max(contentWidth, _host.ClientSize.Width + 1), 0)
                : Size.Empty;
        }

        private void LayoutHorizontally()
        {
            var btns = _host.Controls.OfType<ButtonRound>().ToList();
            int n = btns.Count;
            if (n == 0) { EnsureHorizontalScroll(_host.Padding.Left + _host.Padding.Right); return; }

            foreach (var b in btns)
            {
                b.Height = ButtonHeight;
                b.Margin = new Padding(Spacing / 2, 6, Spacing / 2, 6);
            }

            int leftPad = _host.Padding.Left;
            int rightPad = _host.Padding.Right;
            int clientW = Math.Max(0, _host.ClientSize.Width - leftPad - rightPad);
            int totalGap = Spacing * Math.Max(0, n - 1);
            int avail = Math.Max(0, clientW - totalGap);

            //if (WidthMode == ButtonWidthMode.Equal)
            //{
            //    int widthPer = (n > 0 ? avail / n : 0);
            //    bool overflow = false;

            //    if (widthPer < MinButtonWidth)
            //    {
            //        if (AllowOverflow) { widthPer = MinButtonWidth; overflow = true; }
            //        else { widthPer = Math.Max(1, widthPer); }
            //    }

            //    int remainder = Math.Max(0, avail - widthPer * n);
            //    for (int i = 0; i < n; i++)
            //        btns[i].Width = widthPer + (remainder-- > 0 ? 1 : 0);

            //    int sumW = btns.Sum(x => x.Width) + totalGap;
            //    int contentWidth = leftPad + sumW + rightPad;
            //    if (!overflow) overflow = sumW > clientW;

            //    if (overflow) EnsureHorizontalScroll(contentWidth);
            //    else { _host.AutoScroll = false; _host.AutoScrollMinSize = Size.Empty; }

            //    _host.PerformLayout();
            //    return;
            //}

            // --- Equal: chia đều ---
            if (WidthMode == ButtonWidthMode.Equal)
            {
                // nếu đủ chỗ thì chia đều như cũ
                int widthPer = (n > 0 ? avail / n : 0);
                int minPer = Math.Max(1, Math.Min(Math.Max(MinButtonWidth, 1), widthPer));

                // TÍNH tổng width theo text (intrinsic)
                int sumIntrinsic = btns.Sum(b => _intrinsicWidth.ContainsKey(b) ? _intrinsicWidth[b] : Math.Max(MinButtonWidth, b.Width));
                int contentIntrinsic = leftPad + sumIntrinsic + totalGap + rightPad;

                if (contentIntrinsic > _host.ClientSize.Width)
                {
                    // >>> TRÀN: dùng width theo text, bật scroll (KHÔNG cắt chữ)
                    foreach (var b in btns)
                        b.Width = _intrinsicWidth.ContainsKey(b) ? _intrinsicWidth[b] : Math.Max(MinButtonWidth, b.Width);

                    EnsureHorizontalScroll(contentIntrinsic);
                    _host.WrapContents = false;
                    _host.PerformLayout();
                    return;
                }
                else
                {
                    // >>> ĐỦ CHỖ: chia đều bình thường
                    bool overflow = false;
                    if (widthPer < MinButtonWidth) { widthPer = MinButtonWidth; overflow = true; }

                    int remainder = Math.Max(0, avail - widthPer * n);
                    for (int i = 0; i < n; i++)
                        btns[i].Width = widthPer + (remainder-- > 0 ? 1 : 0);

                    int sumW = btns.Sum(x => x.Width) + totalGap;
                    int contentWidth = leftPad + sumW + rightPad;
                    if (!overflow) overflow = sumW > clientW;

                    if (overflow) EnsureHorizontalScroll(contentWidth);
                    else { _host.AutoScroll = false; _host.AutoScrollMinSize = Size.Empty; }

                    _host.WrapContents = false;
                    _host.PerformLayout();
                    return;
                }
            }

            int sumBase = btns.Sum(b => _intrinsicWidth.ContainsKey(b) ? _intrinsicWidth[b] : Math.Max(MinButtonWidth, b.Width));

            if (sumBase <= avail)
            {
                if (WidthMode == ButtonWidthMode.ByText)
                {
                    foreach (var b in btns) b.Width = _intrinsicWidth[b];
                }
                else // ByTextFill
                {
                    int extra = avail - sumBase;
                    double total = Math.Max(1, sumBase);
                    int remainder2 = extra;

                    foreach (var b in btns)
                    {
                        int baseW = _intrinsicWidth[b];
                        int add = (int)Math.Floor(extra * (baseW / total));
                        b.Width = baseW + add;
                        remainder2 -= add;
                    }
                    int i = 0;
                    while (remainder2-- > 0) { btns[i++ % n].Width += 1; }
                }

                _host.AutoScroll = false;
                _host.AutoScrollMinSize = Size.Empty;
                _host.WrapContents = false;
                _host.PerformLayout();
                return;
            }
            else
            {
                if (AllowOverflow)
                {
                    foreach (var b in btns) b.Width = _intrinsicWidth[b];
                    int sumW = btns.Sum(x => x.Width) + totalGap;
                    int contentWidth = leftPad + sumW + rightPad;
                    EnsureHorizontalScroll(contentWidth);
                    _host.PerformLayout();
                    return;
                }
                else
                {
                    int widthPer = Math.Max(MinButtonWidth, avail / n);
                    for (int i = 0; i < n; i++) btns[i].Width = widthPer;

                    _host.AutoScroll = false;
                    _host.AutoScrollMinSize = Size.Empty;
                    _host.WrapContents = false;
                    _host.PerformLayout();
                    return;
                }
            }
        }

        // ====== Lock/Unlock while loading ======
        public bool AutoLockOnClick { get; set; } = true;
        private MainCommand? _currentSelected = null;

        private readonly Dictionary<ButtonRound, bool> _enabledSnapshot = new Dictionary<ButtonRound, bool>();
        private bool _lockActive = false;
        private bool _eatClicks = false;

        public void DisableAllExcept(MainCommand current, bool disableCurrentToo = true)
        {
            if (_lockActive) return;
            _lockActive = true;
            _eatClicks = true;

            foreach (var kv in _map)
            {
                var btn = kv.Key;
                _enabledSnapshot[btn] = btn.Enabled;

                btn.ClickLocked = true;
                btn.Enabled = true;
                btn.UseWaitCursor = true;
            }

            _host.UseWaitCursor = true;
            _host.Cursor = Cursors.WaitCursor;
        }

        public void RestoreEnabled()
        {
            if (!_lockActive) return;

            foreach (var kv in _enabledSnapshot)
            {
                kv.Key.ClickLocked = false;
                kv.Key.Enabled = kv.Value;
                kv.Key.UseWaitCursor = false;
            }
            _enabledSnapshot.Clear();

            _host.UseWaitCursor = false;
            _host.Cursor = Cursors.Default;

            _eatClicks = false;
            _lockActive = false;
        }

        // ===== Pending next command =====
        private MainCommand? _pendingCommand;

        public bool HasPending { get { return _pendingCommand.HasValue; } }
        public void SetPending(MainCommand cmd, bool selectVisually = true)
        {
            _pendingCommand = cmd;
            if (selectVisually) SelectCommand(cmd);
        }
        public MainCommand? ConsumePending()
        {
            MainCommand? tmp = _pendingCommand;
            _pendingCommand = null;
            return tmp;
        }
        public MainCommand? PeekPending() { return _pendingCommand; }
    }
}
