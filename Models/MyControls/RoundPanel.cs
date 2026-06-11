namespace NailsChekin.MyControls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    public class RoundPanel : Panel
    {
        // ===== Border =====
        private int _cornerRadius = 12;
        [Category("Appearance"), DefaultValue(12)]
        public int CornerRadius { get => _cornerRadius; set { _cornerRadius = Math.Max(0, value); Invalidate(); UpdateRegionClip(); } }

        //private Color _borderColor = Color.FromArgb(160, 160, 160);
        private Color _borderColor = Color.FromArgb(20, 113, 148);
        [Category("Appearance"), DefaultValue(typeof(Color), "20, 113, 148")]
        public Color BorderColor { get => _borderColor; set { _borderColor = value; Invalidate(); } }

        private float _borderWidth = 1.25f;
        [Category("Appearance"), DefaultValue(1.25f)]
        public float BorderWidth { get => _borderWidth; set { _borderWidth = Math.Max(0f, value); Invalidate(); } }

        private bool _borderVisible = true;
        [Category("Appearance"), DefaultValue(true)]
        public bool BorderVisible { get => _borderVisible; set { _borderVisible = value; Invalidate(); } }

        // ===== Background / Gradient =====
        private bool _gradientEnabled = false;
        [Category("Background"), DefaultValue(false)]
        public bool GradientEnabled { get => _gradientEnabled; set { _gradientEnabled = value; Invalidate(); } }

        private Color _gradientStart = Color.White;
        [Category("Background"), DefaultValue(typeof(Color), "White")]
        public Color GradientStart { get => _gradientStart; set { _gradientStart = value; Invalidate(); } }

        private Color _gradientEnd = Color.White;
        [Category("Background"), DefaultValue(typeof(Color), "White")]
        public Color GradientEnd { get => _gradientEnd; set { _gradientEnd = value; Invalidate(); } }

        private LinearGradientMode _gradientMode = LinearGradientMode.Vertical;
        [Category("Background"), DefaultValue(LinearGradientMode.Vertical)]
        public LinearGradientMode GradientMode { get => _gradientMode; set { _gradientMode = value; Invalidate(); } }

        // ===== Header (vẽ thuần GDI+, không add control) =====
        private bool _showHeader = false; // 👉 mặc định FALSE
        [Category("Header"), DefaultValue(false), Description("Bật/tắt vẽ header. Khi false, không vẽ và không tạo control nào cho header.")]
        public bool ShowHeader { get => _showHeader; set { _showHeader = value; Invalidate(); } }

        private int _headerHeight = 40;
        [Category("Header"), DefaultValue(40)]
        public int HeaderHeight { get => _headerHeight; set { _headerHeight = Math.Max(0, value); Invalidate(); } }

        private string _headerText = "Header";
        [Category("Header"), DefaultValue("Header")]
        public string HeaderText { get => _headerText; set { _headerText = value; Invalidate(); } }

        private Color _headerBackColor = Color.FromArgb(244, 246, 248);
        [Category("Header"), DefaultValue(typeof(Color), "244, 246, 248")]
        public Color HeaderBackColor { get => _headerBackColor; set { _headerBackColor = value; Invalidate(); } }

        private Color _headerForeColor = Color.Black;
        [Category("Header"), DefaultValue(typeof(Color), "Black")]
        public Color HeaderForeColor { get => _headerForeColor; set { _headerForeColor = value; Invalidate(); } }

        private Font _headerFont = new Font("Segoe UI", 10f, FontStyle.Bold);
        [Category("Header")]
        public Font HeaderFont { get => _headerFont; set { _headerFont = value ?? new Font("Segoe UI", 10f, FontStyle.Bold); Invalidate(); } }
        public bool ShouldSerializeHeaderFont() => !_headerFont.Equals(new Font("Segoe UI", 10f, FontStyle.Bold));
        public void ResetHeaderFont() { HeaderFont = new Font("Segoe UI", 10f, FontStyle.Bold); }

        private Padding _headerPadding = new Padding(12, 0, 12, 0);
        [Category("Header"), DefaultValue(typeof(Padding), "12,0,12,0")]
        public Padding HeaderPadding { get => _headerPadding; set { _headerPadding = value; Invalidate(); } }

        private ContentAlignment _headerTextAlign = ContentAlignment.MiddleLeft;
        [Category("Header"), DefaultValue(ContentAlignment.MiddleLeft)]
        public ContentAlignment HeaderTextAlign { get => _headerTextAlign; set { _headerTextAlign = value; Invalidate(); } }

        private bool _showHeaderSeparator = true;
        [Category("Header"), DefaultValue(true)]
        public bool ShowHeaderSeparator { get => _showHeaderSeparator; set { _showHeaderSeparator = value; Invalidate(); } }

        private Color _headerSeparatorColor = Color.FromArgb(190, 190, 190);
        [Category("Header"), DefaultValue(typeof(Color), "190, 190, 190")]
        public Color HeaderSeparatorColor { get => _headerSeparatorColor; set { _headerSeparatorColor = value; Invalidate(); } }

        // ===== Shadow =====
        private int _shadowSize = 0;
        [Category("Appearance"), DefaultValue(0)]
        public int ShadowSize { get => _shadowSize; set { _shadowSize = Math.Max(0, value); Invalidate(); } }

        private Point _shadowOffset = new Point(2, 3);
        [Category("Appearance"), DefaultValue(typeof(Point), "2, 3")]
        public Point ShadowOffset { get => _shadowOffset; set { _shadowOffset = value; Invalidate(); } }

        private Color _shadowColor = Color.FromArgb(90, 0, 0, 0);
        [Category("Appearance")]
        public Color ShadowColor { get => _shadowColor; set { _shadowColor = value; Invalidate(); } }
        public bool ShouldSerializeShadowColor() => _shadowColor != Color.FromArgb(90, 0, 0, 0);
        public void ResetShadowColor() { ShadowColor = Color.FromArgb(90, 0, 0, 0); }

        // ===== Behavior =====
        private bool _clipContent = true;
        [Category("Layout"), DefaultValue(true)]
        public bool ClipContent { get => _clipContent; set { _clipContent = value; UpdateRegionClip(); } }

        public RoundPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            DoubleBuffered = true;
            BackColor = Color.Transparent;   // có thể set Transparent
            Padding = new Padding(8);

            SizeChanged += (s, e) => UpdateRegionClip();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.CompositingQuality = CompositingQuality.HighQuality;

            // Toạ độ float + canh tâm pixel cho nét lẻ
            float penW = Math.Max(1f, _borderWidth);
            bool odd = (Math.Round(penW) % 2) == 1;
            float halfPx = odd ? 0.5f : 0f;

            // Vùng khả dụng bên trong control (chừa 1px)
            RectangleF avail = new RectangleF(1f, 1f,
                                              Math.Max(0.1f, Width - 2f),
                                              Math.Max(0.1f, Height - 2f));

            // Border đi trong avail, dịch 0.5px nếu cần
            RectangleF borderRect = new RectangleF(
                avail.X + halfPx, avail.Y + halfPx,
                Math.Max(0.1f, avail.Width - halfPx * 2f),
                Math.Max(0.1f, avail.Height - halfPx * 2f)
            );

            // Fill trọn avail
            RectangleF fillRect = avail;

            // ---- Shadow ----
            if (_shadowSize > 0)
            {
                for (int i = _shadowSize; i >= 1; i--)
                {
                    int alpha = Math.Max(6, _shadowColor.A * i / (_shadowSize + 2));
                    using (var shPath = RoundedRectF(OffsetRectF(borderRect, _shadowOffset.X, _shadowOffset.Y),
                                                     _cornerRadius + (_shadowSize - i)))
                    using (var shBr = new SolidBrush(Color.FromArgb(alpha, _shadowColor)))
                        g.FillPath(shBr, shPath);
                }
            }

            // ---- Fill (Transparent / Solid / Gradient) ----
            using (var fillPath = RoundedRectF(fillRect, Math.Max(0, _cornerRadius - 1)))
            {
                bool transparent = (BackColor.A == 0);
                if (!transparent)
                {
                    if (_gradientEnabled && (_gradientStart != _gradientEnd))
                    {
                        using (var lg = new LinearGradientBrush(Rectangle.Truncate(fillRect), _gradientStart, _gradientEnd, _gradientMode))
                            g.FillPath(lg, fillPath);
                    }
                    else
                    {
                        using (var bg = new SolidBrush(BackColor))
                            g.FillPath(bg, fillPath);
                    }
                }

                // ---- Header (chỉ vẽ khi bật) ----
                bool drawHeader = _showHeader && _headerHeight > 0 && !string.IsNullOrEmpty(_headerText);
                if (drawHeader)
                {
                    RectangleF hdr = new RectangleF(fillRect.X, fillRect.Y,
                                                    fillRect.Width, Math.Min(_headerHeight, fillRect.Height));

                    // Bo góc trên theo panel; dưới phẳng
                    using (var hdrPath = RoundedRectExF(hdr, Math.Max(0, _cornerRadius - 1), Math.Max(0, _cornerRadius - 1), 0, 0))
                    using (var hdrBrush = new SolidBrush(_headerBackColor))
                        g.FillPath(hdrBrush, hdrPath);

                    // Text
                    Rectangle tr = Rectangle.Truncate(new RectangleF(
                        hdr.X + _headerPadding.Left, hdr.Y + _headerPadding.Top,
                        hdr.Width - _headerPadding.Horizontal, hdr.Height - _headerPadding.Vertical));

                    TextFormatFlags flags = TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding;
                    switch (_headerTextAlign)
                    {
                        case ContentAlignment.MiddleLeft: flags |= TextFormatFlags.Left | TextFormatFlags.VerticalCenter; break;
                        case ContentAlignment.MiddleCenter: flags |= TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter; break;
                        case ContentAlignment.MiddleRight: flags |= TextFormatFlags.Right | TextFormatFlags.VerticalCenter; break;
                        case ContentAlignment.TopLeft: flags |= TextFormatFlags.Left | TextFormatFlags.Top; break;
                        case ContentAlignment.TopCenter: flags |= TextFormatFlags.HorizontalCenter | TextFormatFlags.Top; break;
                        case ContentAlignment.TopRight: flags |= TextFormatFlags.Right | TextFormatFlags.Top; break;
                        case ContentAlignment.BottomLeft: flags |= TextFormatFlags.Left | TextFormatFlags.Bottom; break;
                        case ContentAlignment.BottomCenter: flags |= TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom; break;
                        case ContentAlignment.BottomRight: flags |= TextFormatFlags.Right | TextFormatFlags.Bottom; break;
                    }
                    TextRenderer.DrawText(g, _headerText, _headerFont, tr, _headerForeColor, flags);

                    // Separator chỉ vẽ khi header đang vẽ
                    if (_showHeaderSeparator && tr.Bottom < fillRect.Bottom)
                    {
                        using (var penSep = new Pen(_headerSeparatorColor, 1f))
                        {
                            g.SmoothingMode = SmoothingMode.None;
                            g.DrawLine(penSep, (int)fillRect.Left + 1, tr.Bottom, (int)fillRect.Right - 1, tr.Bottom);
                            g.SmoothingMode = SmoothingMode.AntiAlias;
                        }
                    }
                }

            }

            // ---- Border (sắc) ----
            if (_borderVisible && _borderWidth > 0f)
            {
                using (var borderPath = RoundedRectF(borderRect, _cornerRadius))
                using (var pen = new Pen(_borderColor, penW))
                {
                    pen.Alignment = PenAlignment.Inset;
                    g.DrawPath(pen, borderPath);
                }
            }
        }

        //protected override void OnPaintBackground(PaintEventArgs e)
        //{
        //    // Khi Transparent: vẽ lại nền của parent vào đây để tránh nền đen
        //    if (BackColor.A == 0 && Parent != null)
        //    {
        //        var g = e.Graphics;
        //        var state = g.Save();
        //        g.TranslateTransform(-Left, -Top);
        //        var pe = new PaintEventArgs(g, Parent.ClientRectangle);
        //        InvokePaintBackground(Parent, pe);
        //        g.Restore(state);
        //        return;
        //    }
        //    base.OnPaintBackground(e);
        //}

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Blend nền cha khi Transparent
            if (BackColor.A == 0 && Parent != null)
            {
                var g = e.Graphics;
                var s = g.Save();
                try
                {
                    // Đưa hệ trục về gốc của Parent
                    g.TranslateTransform(-Left, -Top);

                    var pe = new PaintEventArgs(g, Parent.ClientRectangle);

                    // LƯU Ý: gọi trực tiếp vì đây là protected static methods của Control
                    InvokePaintBackground(Parent, pe);
                    InvokePaint(Parent, pe); // vẽ cả Parent.Paint (gradient, ảnh nền, v.v.)
                }
                finally
                {
                    g.Restore(s);
                }
                return; // KHÔNG gọi base để tránh fill xám
            }

            base.OnPaintBackground(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateRegionClip();
        }

        private void UpdateRegionClip()
        {
            if (!_clipContent)
            {
                if (Region != null) { Region.Dispose(); Region = null; }
                return;
            }

            // KHÔNG thu nhỏ Region để khỏi cắt mất nửa viền trái/trên
            using (var gp = RoundedRect(new Rectangle(0, 0, Width, Height), _cornerRadius))
            {
                if (Region != null) Region.Dispose();
                Region = new Region(gp);
            }
            Invalidate();
        }

        // ===== Helpers =====
        private static RectangleF OffsetRectF(RectangleF r, float dx, float dy)
            => new RectangleF(r.X + dx, r.Y + dy, r.Width, r.Height);

        private static GraphicsPath RoundedRectF(RectangleF r, float radius)
        {
            float d = radius * 2f;
            var gp = new GraphicsPath();
            if (radius <= 0f) { gp.AddRectangle(r); gp.CloseFigure(); return gp; }
            gp.AddArc(r.X, r.Y, d, d, 180, 90);
            gp.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            gp.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            gp.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            gp.CloseFigure();
            return gp;
        }

        private static GraphicsPath RoundedRectExF(RectangleF r, float tl, float tr, float br, float bl)
        {
            var gp = new GraphicsPath();
            float dTL = tl * 2f, dTR = tr * 2f, dBR = br * 2f, dBL = bl * 2f;

            if (tl > 0) gp.AddArc(r.X, r.Y, dTL, dTL, 180, 90); else gp.AddLine(r.Left, r.Top, r.Left, r.Top);
            if (tr > 0) gp.AddArc(r.Right - dTR, r.Y, dTR, dTR, 270, 90); else gp.AddLine(r.Right, r.Top, r.Right, r.Top);
            if (br > 0) gp.AddArc(r.Right - dBR, r.Bottom - dBR, dBR, dBR, 0, 90); else gp.AddLine(r.Right, r.Bottom, r.Right, r.Bottom);
            if (bl > 0) gp.AddArc(r.X, r.Bottom - dBL, dBL, dBL, 90, 90); else gp.AddLine(r.Left, r.Bottom, r.Left, r.Bottom);

            gp.CloseFigure();
            return gp;
        }

        // Region dùng int-rect là đủ ổn định
        private static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            int d = radius * 2;
            var gp = new GraphicsPath();
            if (radius <= 0) { gp.AddRectangle(r); gp.CloseFigure(); return gp; }
            gp.AddArc(r.X, r.Y, d, d, 180, 90);
            gp.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            gp.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            gp.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            gp.CloseFigure();
            return gp;
        }
    }


}
