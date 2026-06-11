namespace NailsChekin.MyControls
{
    using NailsChekin.Models.Helper;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Globalization;
    using System.Windows.Forms;

    #region Contracts & Models
    public enum HeaderCommand
    {
        Title, BackOffice, BuySupply, Reports, OffCreditDevice, MenuServices, Minimize, Close, SocketStatus, Custom, Clock, ItemLookUp
    }

    public enum HeaderItemKind { Button, Separator, Status, Spacer }

    public enum HeaderStatusState { Normal, Ok, Warning, Error, Info }

    public sealed class HeaderMenuItemDef
    {
        public string Text { get; set; }
        public HeaderCommand Command { get; set; } = HeaderCommand.Custom;
        public Image Icon { get; set; }
        public string Tooltip { get; set; }
    }

    public sealed class HeaderItemDef
    {
        public HeaderItemKind Kind { get; set; } = HeaderItemKind.Button;
        public string Text { get; set; }
        public HeaderCommand Command { get; set; } = HeaderCommand.Custom;
        public string Tooltip { get; set; }
        public Image Icon { get; set; }
        public bool ShowChevron { get; set; }
        public List<HeaderMenuItemDef> MenuItems { get; set; }

        // overrides
        public Color? ForeColor { get; set; }
        public Color? HoverBackColor { get; set; }
        public int? IconSize { get; set; }
        public Padding? ItemPadding { get; set; }

        // separator
        public int? SeparatorWidth { get; set; }

        // status
        public HeaderStatusState? StatusState { get; set; }
        public Color? StatusBackColor { get; set; }
        public Color? StatusForeColor { get; set; }
        public int? StatusCornerRadius { get; set; }

        // spacer
        public int? SpacerWidth { get; set; }
    }

    public sealed class HeaderItemClickedEventArgs : EventArgs
    {
        public HeaderCommand Command { get; internal set; }
        public string Text { get; internal set; }
        public Control SenderControl { get; internal set; }
    }
    public sealed class HeaderMenuClickedEventArgs : EventArgs
    {
        public HeaderCommand OwnerCommand { get; internal set; }
        public HeaderCommand ItemCommand { get; internal set; }
        public string Text { get; internal set; }
    }
    #endregion

    #region Root & Flow
    internal sealed class HeaderRootPanel : Panel
    {
        public HeaderRootPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.SupportsTransparentBackColor, true);
            DoubleBuffered = true;
            ResizeRedraw = true;
            BackColor = Color.Transparent; // blend với nền phía sau
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Nếu trong suốt, vẽ lại nền parent để hòa trộn
            if (BackColor.A == 0 && Parent != null)
            {
                Graphics g = e.Graphics;
                GraphicsState s = g.Save();
                g.TranslateTransform(-Left, -Top);
                using (PaintEventArgs pe = new PaintEventArgs(g, new Rectangle(Point.Empty, Parent.Size)))
                {
                    InvokePaintBackground(Parent, pe);
                    InvokePaint(Parent, pe);
                }
                g.Restore(s);
                return;
            }
            base.OnPaintBackground(e);
        }
    }

    internal sealed class HeaderFlowPanel : FlowLayoutPanel
    {
        public HeaderFlowPanel()
        {
            DoubleBuffered = true; ResizeRedraw = true;
            WrapContents = false; AutoScroll = false;
            FlowDirection = FlowDirection.LeftToRight;
            BackColor = Color.Transparent;
            Margin = Padding.Empty; Padding = new Padding(0);
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }
    }
    #endregion

    #region HeaderButton
    public sealed class HeaderButton : Control
    {
        private bool _hover;
        public Size MinAutoSize { get; set; } = new Size(24, 24);

        public HeaderButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.SupportsTransparentBackColor, true);

            DoubleBuffered = true;
            Cursor = Cursors.Hand;
            ForeColor = Color.White;
            BackColor = Color.Transparent;

            Padding = new Padding(10, 0, 10, 0);
            Height = 28;
            AutoSize = true;
        }

        public Image Icon { get; set; }
        public int IconSize { get; set; } = 16;
        public bool ShowChevron { get; set; } = false;
        public Color HoverBackColor { get; set; } = Color.FromArgb(40, 255, 255, 255);
        public int CornerRadius { get; set; } = 12;

        protected override void OnTextChanged(EventArgs e) { base.OnTextChanged(e); Invalidate(); PerformLayout(); }
        protected override void OnFontChanged(EventArgs e) { base.OnFontChanged(e); Invalidate(); PerformLayout(); }

        public override Size GetPreferredSize(Size proposedSize)
        {
            int textW = string.IsNullOrEmpty(Text) ? 0 :
                TextRenderer.MeasureText(Text, Font, new Size(int.MaxValue, int.MaxValue),
                                         TextFormatFlags.SingleLine | TextFormatFlags.NoPadding).Width;

            int w = Padding.Horizontal + (Icon != null ? IconSize + 6 : 0) + textW + (ShowChevron ? 12 : 0);
            int contentH = Math.Max(Icon != null ? IconSize : 0, Font.Height);
            int h = Math.Max(contentH + Padding.Vertical, 24);

            w = Math.Max(w, MinAutoSize.Width);
            h = Math.Max(h, MinAutoSize.Height);

            return new Size(w, h);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (BackColor.A == 0 && Parent != null)
            {
                Graphics g = e.Graphics; var s = g.Save();
                g.TranslateTransform(-Left, -Top);
                using (PaintEventArgs pe = new PaintEventArgs(g, Parent.ClientRectangle))
                {
                    InvokePaintBackground(Parent, pe);
                }
                g.Restore(s);
                return;
            }
            base.OnPaintBackground(e);
        }

        protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.Half;

            Rectangle rect = ClientRectangle; rect.Inflate(-1, -1);

            if (_hover)
            {
                using (GraphicsPath path = Rounded(rect, CornerRadius))
                using (SolidBrush br = new SolidBrush(HoverBackColor))
                    g.FillPath(br, path);
            }

            int x = rect.Left + Padding.Left;
            int y = rect.Top + (rect.Height - IconSize) / 2;

            if (Icon != null)
            {
                Rectangle dest = new Rectangle(x, y, IconSize, IconSize);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(Icon, dest);
                x += IconSize + 6;
            }

            TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine |
                                    TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis;
            Rectangle textRect = new Rectangle(x, rect.Top, rect.Width - x - Padding.Right - (ShowChevron ? 12 : 0), rect.Height);
            TextRenderer.DrawText(g, Text ?? string.Empty, Font, textRect, ForeColor, flags);

            if (ShowChevron)
            {
                int w = 6, h = 4;
                int cx = rect.Right - Padding.Right - 6;
                int cy = rect.Top + rect.Height / 2 + 1;
                using (GraphicsPath p = new GraphicsPath())
                {
                    p.AddPolygon(new[] {
                        new Point(cx - w/2, cy - h/2),
                        new Point(cx + w/2, cy - h/2),
                        new Point(cx,      cy + h/2)
                    });
                    using (SolidBrush br = new SolidBrush(ForeColor))
                        g.FillPath(br, p);
                }
            }
        }

        private static GraphicsPath Rounded(Rectangle r, int radius)
        {
            if (radius <= 0) { GraphicsPath gp = new GraphicsPath(); gp.AddRectangle(r); gp.CloseFigure(); return gp; }
            int d = radius * 2;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
    #endregion

    #region Separator & StatusBadge
    public sealed class HeaderSeparator : Control
    {
        public HeaderSeparator()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
            DoubleBuffered = true;
            BackColor = Color.Transparent;
            Width = 1; Height = 24; // 1px line
            ForeColor = Color.FromArgb(190, 210, 220, 230); // dễ thấy trên nền sáng/gradient
            Margin = new Padding(12, 0, 12, 0);
        }
        public int LineWidth { get; set; } = 1;
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.None;
            int x = Width / 2;
            using (Pen pen = new Pen(ForeColor, LineWidth))
                g.DrawLine(pen, x, 2, x, Height - 2);
        }
    }

    public sealed class HeaderStatusBadge : Control
    {
        private HeaderStatusState _state = HeaderStatusState.Info;
        public HeaderStatusBadge()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
            DoubleBuffered = true;
            BackColor = Color.Transparent;
            ForeColor = Color.White;
            Padding = new Padding(0, 2, 0, 2);
            Height = 24;
            Font = new Font("Segoe UI", 16.5f, FontStyle.Bold);
            AutoSize = true;
            MinimumSize = new Size(24, 20);
        }

        public HeaderStatusState State { get => _state; set { _state = value; Invalidate(); } }
        public int CornerRadius { get; set; } = 12;
        public Color? OverrideBackColor { get; set; }
        public Color? OverrideForeColor { get; set; }

        public void Set(HeaderStatusState state, string text) { State = state; Text = text; Invalidate(); }

        public override Size GetPreferredSize(Size proposedSize)
        {
            int textW = string.IsNullOrEmpty(Text) ? 0 :
                TextRenderer.MeasureText(Text, Font, new Size(int.MaxValue, int.MaxValue),
                                         TextFormatFlags.SingleLine | TextFormatFlags.NoPadding).Width;
            int w = Padding.Horizontal + textW;
            int h = Math.Max(Font.Height + Padding.Vertical, 20);
            return new Size(w, h);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (BackColor.A == 0 && Parent != null)
            {
                Graphics g = e.Graphics; var s = g.Save();
                g.TranslateTransform(-Left, -Top);
                using (PaintEventArgs pe = new PaintEventArgs(g, Parent.ClientRectangle))
                {
                    InvokePaintBackground(Parent, pe);
                }
                g.Restore(s);
                return;
            }
            base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.Half;

            Rectangle rect = ClientRectangle; rect.Inflate(-1, -1);
            Color back = OverrideBackColor ?? StateColor(State);
            Color fore = OverrideForeColor ?? Color.White;

            using (GraphicsPath path = Rounded(rect, CornerRadius))
            using (SolidBrush br = new SolidBrush(back))
                g.FillPath(br, path);

            var flags = TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine |
            TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis;

            // Nâng chữ lên 1px để canh hàng với các nút
            var textRect = new Rectangle(rect.X, rect.Y - 1, rect.Width, rect.Height);
            TextRenderer.DrawText(g, Text ?? string.Empty, Font, textRect, fore, flags);
        }

        private static Color StateColor(HeaderStatusState s)
        {
            switch (s)
            {
                case HeaderStatusState.Ok: return Color.FromArgb(28, 156, 91);
                case HeaderStatusState.Warning: return Color.FromArgb(230, 145, 56);
                case HeaderStatusState.Error: return Color.FromArgb(202, 62, 71);
                case HeaderStatusState.Info: return Color.FromArgb(186, 222, 247); // very light blue (pill)
                default: return Color.FromArgb(120, 120, 120);
            }
        }

        private static GraphicsPath Rounded(Rectangle r, int radius)
        {
            if (radius <= 0) { GraphicsPath gp = new GraphicsPath(); gp.AddRectangle(r); gp.CloseFigure(); return gp; }
            int d = radius * 2;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            // Tính lại kích thước dựa trên flags giống lúc vẽ
            var flags = TextFormatFlags.SingleLine | TextFormatFlags.NoPadding;
            // dùng TextRenderer.MeasureText với flags nhất quán
            Size sz = TextRenderer.MeasureText(Text ?? string.Empty, Font, new Size(int.MaxValue, int.MaxValue), flags);
            // cộng padding
            int w = Padding.Horizontal + sz.Width;
            int h = Math.Max(Font.Height + Padding.Vertical, 20);
            // gán Size để FlowLayoutPanel reflow
            this.Size = new Size(w, h);
            this.Invalidate();
            this.Parent?.PerformLayout();
        }
    }
    #endregion

    public sealed class HeaderTemplateBar
    {
        public event EventHandler<HeaderItemClickedEventArgs> ItemClicked;
        public event EventHandler<HeaderMenuClickedEventArgs> MenuItemClicked;

        private readonly Control _host;
        private HeaderRootPanel _root;
        private HeaderFlowPanel _left, _right;
        private ToolTip _tip;

        private readonly Dictionary<Control, HeaderItemDef> _defMap = new Dictionary<Control, HeaderItemDef>();
        private readonly Dictionary<HeaderCommand, HeaderStatusBadge> _statusMap = new Dictionary<HeaderCommand, HeaderStatusBadge>();

        private List<HeaderItemDef> _pendingLeft = new List<HeaderItemDef>();
        private List<HeaderItemDef> _pendingRight = new List<HeaderItemDef>();
        private bool _initialized;
        private Timer _clockTimer;

        // Style
        public int BarHeight { get; set; } = 42; // cao hơn để nhìn cân
        public float DefaultFontSize { get; set; } = 16.5f;
        public int DefaultIconSize { get; set; } = 16;
        public int ItemSpacing { get; set; } = 12;
        public Color DefaultForeColor { get; set; } = ControlPaint.Dark(Color.FromArgb(120, 205, 236), 0.3f);   //Đậm hơn màu logo 30%
        public Color DefaultHoverBack { get; set; } = Color.FromArgb(40, 255, 255, 255);

        private Padding _leftPad = new Padding(8, 8, 8, 8);
        private Padding _rightPad = new Padding(8, 3, 8, 7); // TOP nhỏ hơn -> hàng phải “cao” hơn

        public Padding LeftPadding
        {
            get { return (_left != null) ? _left.Padding : _leftPad; }
            set { _leftPad = value; OnUI(() => { if (_left != null) _left.Padding = value; _RelayoutHeights(); }); }
        }

        public Padding RightPadding
        {
            get { return (_right != null) ? _right.Padding : _rightPad; }
            set { _rightPad = value; OnUI(() => { if (_right != null) _right.Padding = value; _RelayoutHeights(); }); }
        }

        private void _RelayoutHeights()
        {
            if (_left != null)
            {
                foreach (Control c in _left.Controls)
                {
                    if (c is HeaderButton) c.Height = Math.Max(BarHeight - _left.Padding.Vertical, 24);
                    else if (c is HeaderStatusBadge) c.Height = Math.Max(BarHeight - _left.Padding.Vertical, 22);
                    else if (c is HeaderSeparator) c.Height = Math.Max(BarHeight - _left.Padding.Vertical, 24);
                }
            }
            if (_right != null)
            {
                foreach (Control c in _right.Controls)
                {
                    if (c is HeaderButton) c.Height = Math.Max(BarHeight - _right.Padding.Vertical, 24);
                    else if (c is HeaderStatusBadge) c.Height = Math.Max(BarHeight - _right.Padding.Vertical, 22);
                    else if (c is HeaderSeparator) c.Height = Math.Max(BarHeight - _right.Padding.Vertical, 24);
                }
            }
            _root?.Invalidate();
        }

        public HeaderTemplateBar(Control host)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));
            AttachToHostSafe();
        }

        // ===== Public API =====
        public void SetLeftItems(IEnumerable<HeaderItemDef> items)
        {
            _pendingLeft = items != null ? new List<HeaderItemDef>(items) : new List<HeaderItemDef>();
            OnUI(delegate { if (_initialized) BuildSide(_left, _pendingLeft); });
        }
        public void SetRightItems(IEnumerable<HeaderItemDef> items)
        {
            _pendingRight = items != null ? new List<HeaderItemDef>(items) : new List<HeaderItemDef>();
            OnUI(delegate { if (_initialized) BuildSide(_right, _pendingRight); });
        }

        public void UpdateStatus(HeaderCommand cmd, HeaderStatusState state, string text)
        {
            OnUI(delegate
            {
                HeaderStatusBadge b;
                if (_statusMap.TryGetValue(cmd, out b)) b.Set(state, text);
            });
        }

        public void Clear()
        {
            OnUI(delegate
            {
                if (_left != null) _left.Controls.Clear();
                if (_right != null) _right.Controls.Clear();
                _defMap.Clear();
                _statusMap.Clear();
            });
            _pendingLeft.Clear();
            _pendingRight.Clear();
        }

        public void ForceRender()
        {
            OnUI(delegate
            {
                if (!_initialized)
                {
                    if (!_host.IsHandleCreated) _host.CreateControl();
                    InitNow();
                }
                if (_pendingLeft != null) BuildSide(_left, _pendingLeft);
                if (_pendingRight != null) BuildSide(_right, _pendingRight);
                if (_root != null) { _root.BringToFront(); _root.Refresh(); }
            });
        }

        // Preset: header mảnh chỉ bên phải
        public void ApplySlimRightPreset()
        {
            SetLeftItems(new HeaderItemDef[0]);
            SetRightItems(new[]
            {
                //new HeaderItemDef {
                //    Kind=HeaderItemKind.Status, Text="Socket: Connected !".ToUpper(), Command=HeaderCommand.SocketStatus,
                //    StatusState=HeaderStatusState.Info,
                //    StatusBackColor=Color.FromArgb(200,220,236,250),
                //    StatusForeColor=DefaultForeColor, // Color.FromArgb(30,60,90),
                //    StatusCornerRadius=12
                //},

                // CLOCK (đứng trước OffCreditDevice)
                new HeaderItemDef
                {
                    Kind = HeaderItemKind.Status,
                    Command = HeaderCommand.Clock,
                    Text = DateTime.Now.ToString("hh:mm:ss tt" + (LayoutHelper.mini_screen ? "" : " dddd MM/dd" ), new CultureInfo("en-US")).ToUpperInvariant(),
                    StatusState = HeaderStatusState.Info,
                    StatusBackColor = Color.FromArgb(200, 220, 236, 250),
                    StatusForeColor = DefaultForeColor,
                    StatusCornerRadius = 12
                },
                new HeaderItemDef { Kind = HeaderItemKind.Separator },

                //new HeaderItemDef { Kind=HeaderItemKind.Button, Text="Printer Status".ToUpper(),    Command=HeaderCommand.Custom },
                new HeaderItemDef { Kind=HeaderItemKind.Button, Text="CREDIT OFF", Command=HeaderCommand.OffCreditDevice },
                new HeaderItemDef { Kind=HeaderItemKind.Separator },

                //new HeaderItemDef { Kind=HeaderItemKind.Button, Text="MENU SERVICES",           Command=HeaderCommand.MenuServices },
                //new HeaderItemDef { Kind=HeaderItemKind.Separator },

                new HeaderItemDef { Kind=HeaderItemKind.Button, Text="PRODUCT SEARCH",           Command=HeaderCommand.ItemLookUp },
                new HeaderItemDef { Kind=HeaderItemKind.Separator },

                new HeaderItemDef { Kind=HeaderItemKind.Button, Text="REPORTS",           Command=HeaderCommand.Reports },
                new HeaderItemDef { Kind=HeaderItemKind.Separator },
                new HeaderItemDef { Kind=HeaderItemKind.Button, Text="BACK OFFICE",           Command=HeaderCommand.BackOffice },
                new HeaderItemDef { Kind=HeaderItemKind.Separator },
                new HeaderItemDef { Kind=HeaderItemKind.Button, Text="BUY SUPPLY",           Command=HeaderCommand.BuySupply },
                new HeaderItemDef { Kind=HeaderItemKind.Separator },
                new HeaderItemDef { Kind=HeaderItemKind.Button, Text="MINIMIZE",          Command=HeaderCommand.Minimize },
                new HeaderItemDef { Kind=HeaderItemKind.Button, Text="CLOSE",             Command=HeaderCommand.Close },
            });
            ForceRender();
        }

        // ===== Internal =====
        private void AttachToHostSafe()
        {
            if (_host.IsDisposed) return;
            if (_host.IsHandleCreated && !_host.InvokeRequired) { InitNow(); return; }

            if (_host.IsHandleCreated) _host.BeginInvoke((Action)InitNow);
            else _host.HandleCreated += delegate { _host.BeginInvoke((Action)InitNow); };
        }

        private void InitNow()
        {
            if (_host.IsDisposed || _initialized) return;

            _root = new HeaderRootPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent, // để blend
                Height = BarHeight,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            _left = new HeaderFlowPanel { Dock = DockStyle.Left, Padding = _leftPad };
            _right = new HeaderFlowPanel { Dock = DockStyle.Right, Padding = _rightPad };
            _root.Controls.Add(_right);
            _root.Controls.Add(_left);
            _host.Controls.Add(_root);

            _tip = new ToolTip { ShowAlways = true, AutomaticDelay = 250 };
            _root.SizeChanged += delegate { _root.Height = BarHeight; };

            _initialized = true;
            if (_pendingLeft != null) BuildSide(_left, _pendingLeft);
            if (_pendingRight != null) BuildSide(_right, _pendingRight);

            //Add Timer Clock
            _clockTimer = new Timer { Interval = 1000 }; // 1s
            _clockTimer.Tick += (s, e) =>
            {
                // hh:mm tt -> 12-giờ + AM/PM; ToUpper cho “AM/PM” rõ ràng
                var nowText = DateTime.Now.ToString("hh:mm:ss tt" + (LayoutHelper.mini_screen ? "" : " dddd MM/dd"), new CultureInfo("en-US")).ToUpperInvariant();

                // Nếu đã có badge Clock thì cập nhật
                HeaderStatusBadge badge;
                if (_statusMap.TryGetValue(HeaderCommand.Clock, out badge) && badge != null)
                {
                    if (!ReferenceEquals(badge.Text, nowText))
                    {
                        badge.Text = nowText;
                        badge.Size = badge.GetPreferredSize(Size.Empty); // ép co lại ngay

                        // khiến layout tự giãn nếu cần (badge đang AutoSize)
                        badge.PerformLayout();
                        badge.Invalidate();
                    }
                }
            };
            _clockTimer.Start();

        }

        private void BuildSide(FlowLayoutPanel flow, IEnumerable<HeaderItemDef> items)
        {
            if (flow == null) return;

            List<HeaderItemDef> list = items != null ? new List<HeaderItemDef>(items) : new List<HeaderItemDef>();
            flow.SuspendLayout();
            foreach (Control c in flow.Controls) c.Dispose();
            flow.Controls.Clear();

            foreach (HeaderItemDef it in list)
            {
                Control ctrl = null;

                switch (it.Kind)
                {
                    case HeaderItemKind.Button:
                        {
                            HeaderButton btn = new HeaderButton
                            {
                                Text = it.Text,
                                Icon = it.Icon,
                                ShowChevron = it.ShowChevron || (it.MenuItems != null && it.MenuItems.Count > 0),
                                ForeColor = it.ForeColor ?? DefaultForeColor,
                                HoverBackColor = it.HoverBackColor ?? DefaultHoverBack,
                                IconSize = it.IconSize ?? DefaultIconSize,
                                Padding = it.ItemPadding ?? new Padding(10, 0, 10, 0),
                                Margin = new Padding(ItemSpacing / 2, 0, ItemSpacing / 2, 0),
                                Font = new Font("Segoe UI", DefaultFontSize, FontStyle.Bold),
                                Height = Math.Max(BarHeight - flow.Padding.Vertical, 24),
                            };
                            if (!string.IsNullOrEmpty(it.Tooltip)) _tip.SetToolTip(btn, it.Tooltip);

                            btn.Click += delegate
                            {
                                if (it.MenuItems != null && it.MenuItems.Count > 0) ShowMenu(btn, it);
                                else if (ItemClicked != null)
                                {
                                    ItemClicked(this, new HeaderItemClickedEventArgs
                                    {
                                        Command = it.Command,
                                        Text = it.Text,
                                        SenderControl = btn
                                    });
                                }
                            };
                            ctrl = btn;
                            break;
                        }

                    case HeaderItemKind.Separator:
                        {
                            ctrl = new HeaderSeparator
                            {
                                Width = Math.Max(1, it.SeparatorWidth ?? 1),
                                Height = Math.Max(BarHeight - flow.Padding.Vertical, 24),
                                Margin = new Padding(12, 0, 12, 0),
                                Font = new Font("Segoe UI", DefaultFontSize, FontStyle.Regular),
                                ForeColor = it.ForeColor ?? DefaultForeColor,
                            };
                            break;
                        }

                    case HeaderItemKind.Status:
                        {
                            var targetH = Math.Max(BarHeight - flow.Padding.Vertical, 24);

                            var badge = new HeaderStatusBadge
                            {
                                Text = it.Text,
                                State = it.StatusState ?? HeaderStatusState.Info,
                                OverrideBackColor = it.StatusBackColor,
                                OverrideForeColor = it.StatusForeColor,
                                CornerRadius = Math.Max(8, it.StatusCornerRadius ?? 12),
                                Font = new Font("Segoe UI", DefaultFontSize, FontStyle.Bold),
                                ForeColor = it.ForeColor ?? DefaultForeColor,
                            };

                            // Ép cao bằng nút để canh giữa thật sự
                            badge.MinimumSize = new Size(0, targetH);
                            badge.Height = targetH;
                            badge.Margin = new Padding(ItemSpacing / 2, 0, ItemSpacing / 2, 0);

                            badge.Click += (s, e) => ItemClicked?.Invoke(this, new HeaderItemClickedEventArgs
                            {
                                Command = it.Command,
                                Text = it.Text,
                                SenderControl = badge
                            });

                            _statusMap[it.Command] = badge;
                            ctrl = badge;
                            break;
                        }

                    case HeaderItemKind.Spacer:
                        ctrl = new Panel { Width = Math.Max(0, it.SpacerWidth ?? 10), Height = 1, Margin = new Padding(0) };
                        break;
                }

                if (ctrl != null)
                {
                    flow.Controls.Add(ctrl);
                    _defMap[ctrl] = it;
                }
            }

            flow.ResumeLayout(true);
            if (_root != null) _root.Height = BarHeight;
        }

        private void ShowMenu(HeaderButton owner, HeaderItemDef def)
        {
            ContextMenuStrip cms = new ContextMenuStrip { ShowImageMargin = true, Renderer = new ToolStripProfessionalRenderer() };
            if (def.MenuItems != null)
            {
                foreach (HeaderMenuItemDef m in def.MenuItems)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem(m.Text, m.Icon);
                    if (!string.IsNullOrEmpty(m.Tooltip)) item.ToolTipText = m.Tooltip;
                    HeaderMenuItemDef mLocal = m;
                    item.Click += delegate
                    {
                        if (MenuItemClicked != null)
                        {
                            MenuItemClicked(this, new HeaderMenuClickedEventArgs
                            {
                                OwnerCommand = def.Command,
                                ItemCommand = mLocal.Command,
                                Text = mLocal.Text
                            });
                        }
                    };
                    cms.Items.Add(item);
                }
            }
            Point pt = owner.PointToScreen(new Point(0, owner.Height));
            cms.Closed += delegate { cms.Dispose(); };
            cms.Show(pt);
        }

        // Helper chạy và trả về kết quả đúng thread UI
        private T OnUI<T>(Func<T> fn)
        {
            if (_host.IsDisposed) return default(T);
            if (_host.IsHandleCreated)
            {
                if (_host.InvokeRequired) return (T)_host.Invoke(fn);
                return fn();
            }
            // Handle chưa tạo: cứ chạy (map pending vẫn ổn)
            return fn();
        }

        private void OnUI(Action a)
        {
            if (_host.IsDisposed) return;

            if (_host.IsHandleCreated)
            {
                if (_host.InvokeRequired) _host.BeginInvoke(a); else a();
            }
            else
            {
                _host.HandleCreated += delegate { if (!_host.IsDisposed) _host.BeginInvoke(a); };
            }
        }

        public void SetItemText(HeaderCommand cmd, string newText)
        {
            OnUI(() =>
            {
                bool any = false;
                foreach (var kv in new List<KeyValuePair<Control, HeaderItemDef>>(_defMap))
                {
                    var def = kv.Value;
                    if (def.Kind == HeaderItemKind.Button && def.Command == cmd)
                    {
                        def.Text = newText;
                        var btn = kv.Key as HeaderButton;
                        if (btn != null) { btn.Text = newText; btn.PerformLayout(); btn.Invalidate(); }
                        any = true;
                    }
                }

                // Nếu là badge (Status) muốn đổi text, dùng UpdateStatus; nhưng hỗ trợ nhẹ ở đây luôn
                if (!any)
                {
                    HeaderStatusBadge badge;
                    if (_statusMap.TryGetValue(cmd, out badge))
                    {
                        badge.Text = newText;
                        badge.Invalidate();
                    }
                }

                if (_root != null) { _root.PerformLayout(); _root.Invalidate(); }
            });
        }

        // Lấy text của item theo Command (ưu tiên Button, nếu không có sẽ trả về Status nếu trùng Command)
        public string GetItemText(HeaderCommand cmd)
        {
            return OnUI<string>(() => GetItemTextCore(cmd));
        }

        // Bản Try... an toàn hơn, không cần null-check ở chỗ gọi
        public bool TryGetItemText(HeaderCommand cmd, out string text)
        {
            string t = OnUI<string>(() => GetItemTextCore(cmd));
            text = t;
            return t != null;
        }

        // Core logic: tìm trong map định nghĩa & control
        private string GetItemTextCore(HeaderCommand cmd)
        {
            // Duyệt các control đã build
            foreach (var kv in _defMap)
            {
                var def = kv.Value;
                if (def.Command != cmd) continue;

                if (def.Kind == HeaderItemKind.Button)
                {
                    var btn = kv.Key as HeaderButton;
                    return (btn != null ? btn.Text : def.Text);
                }
                if (def.Kind == HeaderItemKind.Status)
                {
                    var badge = kv.Key as HeaderStatusBadge;
                    return (badge != null ? badge.Text : def.Text);
                }
            }

            // Phòng trường hợp Status được lưu ở _statusMap
            HeaderStatusBadge b;
            if (_statusMap.TryGetValue(cmd, out b) && b != null)
                return b.Text;

            return null; // không thấy
        }


    }
}
