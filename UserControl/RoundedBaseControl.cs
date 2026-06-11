using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace NailsChekin.UserControl
{
    /// <summary>
    /// Base UserControl bo góc mượt:
    /// - Fill nền bo góc ở OnPaintBackground (để control con Transparent “ăn” nền).
    /// - Vẽ viền ngoài & khung trong (tùy chọn) ở OnPaint.
    /// - Có thể bật Region clip nếu thực sự muốn cắt hình theo bo góc (mặc định tắt).
    /// </summary>
    public class RoundedBaseControl : System.Windows.Forms.UserControl
    {
        // ===== Card options =====
        private int _cornerRadius = 14;
        private Color _fillColor = Color.Orange;
        private Color _borderColor = Color.FromArgb(240, 240, 240);
        private float _borderThickness = 0f;
        private bool _borderMatchParent = true;

        [Category("Card"), DefaultValue(14)]
        public int CornerRadius
        {
            get => _cornerRadius;
            set { if (_cornerRadius != value) { _cornerRadius = value; Invalidate(); } }
        }

        [Category("Card")]
        public Color FillColor
        {
            get => _fillColor;
            set { if (_fillColor != value) { _fillColor = value; Invalidate(); } }
        }

        [Category("Card")]
        public Color BorderColor
        {
            get => _borderColor;
            set { if (_borderColor != value) { _borderColor = value; Invalidate(); } }
        }

        [Category("Card"), DefaultValue(0f)] // 0 = không vẽ viền
        public float BorderThickness
        {
            get => _borderThickness;
            set { if (Math.Abs(_borderThickness - value) > float.Epsilon) { _borderThickness = value; Invalidate(); } }
        }

        [Category("Card"), DefaultValue(true)]
        public bool BorderMatchParent
        {
            get => _borderMatchParent;
            set { if (_borderMatchParent != value) { _borderMatchParent = value; Invalidate(); } }
        }

        // ===== Inner border options =====
        private bool _showInnerBorder = false;
        private Padding _innerPadding = new Padding(8, 28, 8, 34);
        private int _innerCornerRadius = 10;
        private Color _innerBorderColor = Color.White;
        private float _innerBorderThickness = 1.5f;

        [Category("Card/InnerBorder"), DefaultValue(false)]
        public bool ShowInnerBorder
        {
            get => _showInnerBorder;
            set { if (_showInnerBorder != value) { _showInnerBorder = value; Invalidate(); } }
        }

        [Category("Card/InnerBorder"), DefaultValue(typeof(Padding), "8,28,8,34")]
        public Padding InnerPadding
        {
            get => _innerPadding;
            set { if (_innerPadding != value) { _innerPadding = value; Invalidate(); } }
        }

        [Category("Card/InnerBorder"), DefaultValue(10)]
        public int InnerCornerRadius
        {
            get => _innerCornerRadius;
            set { if (_innerCornerRadius != value) { _innerCornerRadius = value; Invalidate(); } }
        }

        [Category("Card/InnerBorder")]
        public Color InnerBorderColor
        {
            get => _innerBorderColor;
            set { if (_innerBorderColor != value) { _innerBorderColor = value; Invalidate(); } }
        }

        [Category("Card/InnerBorder"), DefaultValue(1.5f)]
        public float InnerBorderThickness
        {
            get => _innerBorderThickness;
            set { if (Math.Abs(_innerBorderThickness - value) > float.Epsilon) { _innerBorderThickness = value; Invalidate(); } }
        }

        // ===== Optional Region clip (1-bit mask – mặc định KHÔNG dùng) =====
        private bool _useRegionClip = false;

        [Category("Card/Advanced"), DefaultValue(false)]
        public bool UseRegionClip
        {
            get => _useRegionClip;
            set { if (_useRegionClip != value) { _useRegionClip = value; UpdateRegion(); } }
        }

        public RoundedBaseControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);
            BackColor = SystemColors.Control;
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (Parent != null) BackColor = Parent.BackColor;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateRegion();
        }

        private void UpdateRegion()
        {
            if (UseRegionClip && Width > 0 && Height > 0)
            {
                using (GraphicsPath gp = RoundedRectF(new RectangleF(0, 0, Width, Height), CornerRadius))
                    Region = new Region(gp);
            }
            else
            {
                Region = null;
            }
        }

        /// <summary>Vùng vẽ card (đã chừa mép cho viền).</summary>
        protected virtual RectangleF GetCardRect()
        {
            float t = Math.Max(0f, BorderThickness);
            return new RectangleF(
                t > 0f ? t : 1f,
                t > 0f ? t : 1f,
                Width - (t > 0f ? 2 * t : 2f),
                Height - (t > 0f ? 2 * t : 2f));
        }

        // ===== Paint =====

        // Fill nền Parent + nền card bo góc (để control con Transparent nhìn đúng nền)
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (Parent != null)
            {
                Graphics g = e.Graphics;
                GraphicsState st = g.Save();
                g.TranslateTransform(-Left, -Top);
                PaintEventArgs pe = new PaintEventArgs(g, Parent.DisplayRectangle);
                InvokePaintBackground(Parent, pe);
                InvokePaint(Parent, pe);
                g.Restore(st);
            }
            else
            {
                base.OnPaintBackground(e);
            }

            Graphics g2 = e.Graphics;
            g2.SmoothingMode = SmoothingMode.AntiAlias;
            g2.CompositingQuality = CompositingQuality.HighQuality;
            g2.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g2.PixelOffsetMode = PixelOffsetMode.Default; // fill dùng Default

            using (GraphicsPath path = RoundedRectF(GetCardRect(), CornerRadius))
            using (SolidBrush br = new SolidBrush(FillColor))
                g2.FillPath(br, path);
        }

        // Vẽ viền & inner border
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // 1) Outer border
            if (BorderThickness > 0f)
            {
                g.PixelOffsetMode = PixelOffsetMode.Half;
                RectangleF rf = new RectangleF(BorderThickness / 2f, BorderThickness / 2f,
                                               Width - BorderThickness, Height - BorderThickness);

                Color borderCol = (BorderMatchParent && Parent != null) ? Parent.BackColor : BorderColor;

                using (GraphicsPath path = RoundedRectF(rf, CornerRadius))
                using (Pen pen = new Pen(borderCol, BorderThickness)
                { Alignment = PenAlignment.Inset, LineJoin = LineJoin.Round })
                    g.DrawPath(pen, path);
            }

            // 2) Inner border (optional)
            if (ShowInnerBorder && InnerBorderThickness > 0f)
            {
                g.PixelOffsetMode = PixelOffsetMode.Half;

                RectangleF card = GetCardRect();
                RectangleF ibox = new RectangleF(
                    card.X + InnerPadding.Left,
                    card.Y + InnerPadding.Top,
                    card.Width - (InnerPadding.Left + InnerPadding.Right),
                    card.Height - (InnerPadding.Top + InnerPadding.Bottom));

                if (ibox.Width > 0 && ibox.Height > 0)
                {
                    using (GraphicsPath gpInner = RoundedRectF(ibox, InnerCornerRadius))
                    using (Pen p2 = new Pen(InnerBorderColor, InnerBorderThickness)
                    { Alignment = PenAlignment.Inset, LineJoin = LineJoin.Round })
                        g.DrawPath(p2, gpInner);
                }
            }

            // 3) Cho lớp con vẽ thêm
            PaintOverlay(g);
        }

        /// <summary>Hook để lớp kế thừa vẽ thêm overlay (text/icon) sau khi card đã vẽ xong.</summary>
        protected virtual void PaintOverlay(Graphics g) { }

        // Helper bo góc float
        protected static GraphicsPath RoundedRectF(RectangleF r, int radius)
        {
            float d = radius * 2f;
            GraphicsPath p = new GraphicsPath();
            if (radius <= 0) { p.AddRectangle(r); p.CloseFigure(); return p; }
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }
    }
}
