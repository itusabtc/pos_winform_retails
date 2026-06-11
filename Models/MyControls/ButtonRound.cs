namespace NailsChekin.MyControls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    public class ButtonRound : UserControl
    {
        [Category("Behavior"), DefaultValue(false),
        Description("Bật chế độ dùng cho FooterTemplateBar: không ellipsis và không tự toggle Selected.")]
        public bool FooterMode { get; set; } = false;

        private bool _hover, _pressed, _selected;
        private string _title = "Round Button";
        private float _titleFontSize = 20f;

        public ButtonRound()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            Cursor = Cursors.Hand;
            Height = 48;
            MinimumSize = new Size(120, 36);

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.StandardClick  |   // chuyển dblclick -> click chuẩn
                     ControlStyles.StandardDoubleClick |
                     ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.Transparent; // có thể đổi ở Designer
        }

        // ====== Properties ======
        [Category("Appearance"), Description("Nội dung hiển thị")]
        public string Title { get => _title; set { _title = value; Invalidate(); } }

        [Category("Appearance"), Description("Khi true, border dùng SelectedBorderColor và dày hơn 1 chút")]
        public bool Selected { get => _selected; set { _selected = value; Invalidate(); } }

        [Category("Appearance"), DefaultValue(12)]
        public int CornerRadius { get; set; } = 12;

        [Category("Layout"), Description("Padding cho vùng chữ, không ảnh hưởng nền")]
        public Padding ButtonPadding { get; set; } = new Padding(16, 10, 16, 10);

        //[Category("Appearance"), Description("Cỡ chữ")]
        //public float TitleFontSize { get => _titleFontSize; set { _titleFontSize = Math.Max(6f, value); Invalidate(); } }

        // === 1) Property: báo Designer repaint ngay, có DefaultValue để serialize ===
        [Category("Appearance")]
        [Description("Cỡ chữ hiển thị của Title (thiết kế + runtime).")]
        [DefaultValue(20f)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public float TitleFontSize
        {
            get { return _titleFontSize; }
            set
            {
                var v = Math.Max(6f, value);
                if (Math.Abs(_titleFontSize - v) < 0.01f) return;
                _titleFontSize = v;
                Invalidate();   // cập nhật ngay cả trong design mode
            }
        }
        // === 2) Khi đổi Font trong Designer, tự cập nhật TitleFontSize ===
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            float v = Math.Max(6f, Font.Size);
            if (Math.Abs(_titleFontSize - v) > 0.01f)
            {
                _titleFontSize = v;
                Invalidate();  // để design surface cập nhật ngay
            }
        }

        // --- Bộ màu theo trạng thái (dùng khi TitleBackColor = Empty) ---
        [Category("Appearance"), DefaultValue(typeof(Color), "White")]
        public Color NormalBackColor { get; set; } = Color.White;

        [Category("Appearance"), DefaultValue(typeof(Color), "245, 248, 255")]
        public Color HoverBackColor { get; set; } = Color.FromArgb(245, 248, 255);

        [Category("Appearance"), DefaultValue(typeof(Color), "235, 242, 255")]
        public Color PressBackColor { get; set; } = Color.FromArgb(235, 242, 255);

        // --- Màu nền cố định (ưu tiên nếu set). Cho phép Transparent trong Designer ---
        private Color _titleBackColor = Color.Empty; // Empty = dùng Normal/Hover/Press
        [Category("Appearance"), Description("Màu nền fill kín; đặt Transparent để nền trong suốt; để Empty để dùng màu theo trạng thái.")]
        public Color TitleBackColor
        {
            get => _titleBackColor;
            set { _titleBackColor = value; Invalidate(); }
        }
        public bool ShouldSerializeTitleBackColor() => !_titleBackColor.IsEmpty;
        public void ResetTitleBackColor() { TitleBackColor = Color.Empty; }

        [Category("Appearance"), DefaultValue(typeof(Color), "White")]
        public Color TitleForeColor { get; set; } = Color.White;

        // --- Border ---
        private Color _borderColor = Color.FromArgb(200, 200, 200);
        [Category("Appearance"), DefaultValue(typeof(Color), "200, 200, 200")]
        public Color BorderColor { get => _borderColor; set { _borderColor = value; Invalidate(); } }

        private Color _selectedBorderColor = Color.DodgerBlue;
        [Category("Appearance"), DefaultValue(typeof(Color), "DodgerBlue")]
        public Color SelectedBorderColor { get => _selectedBorderColor; set { _selectedBorderColor = value; Invalidate(); } }

        private float _borderWidth = 1f;
        [Category("Appearance"), DefaultValue(1f)]
        public float BorderWidth { get => _borderWidth; set { _borderWidth = Math.Max(0f, value); Invalidate(); } }

        private bool _borderVisible = true;
        [Category("Appearance"), DefaultValue(true)]
        public bool BorderVisible { get => _borderVisible; set { _borderVisible = value; Invalidate(); } }

        // ====== Events ======
        public event EventHandler CardClick;
        public bool ClickLocked { get; set; } = false; // footer bật/tắt

        protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _hover = false; _pressed = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            //if (!Enabled || ClickLocked) return;

            _pressed = true;
            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            //if (!Enabled || ClickLocked) return;

            base.OnMouseUp(e);
            if (ClientRectangle.Contains(e.Location))
            {
                Selected = !Selected;
                CardClick?.Invoke(this, EventArgs.Empty);
            }
            _pressed = false;
            Invalidate();
        }


        // ==== Disabled overlay ====
        private bool _showDisabledOverlay = true;
        [Category("Appearance"), Description("Hiển thị lớp phủ khi control bị Disable"), DefaultValue(true)]
        public bool ShowDisabledOverlay { get => _showDisabledOverlay; set { _showDisabledOverlay = value; Invalidate(); } }

        private Color _disabledOverlayColor = Color.FromArgb(110, 0, 0, 0); // đen  ~43% opacity
        [Category("Appearance"), Description("Màu lớp phủ khi Disable (có alpha).")]
        public Color DisabledOverlayColor { get => _disabledOverlayColor; set { _disabledOverlayColor = value; Invalidate(); } }

        private string _disabledOverlayText = "Processing...";
        [Category("Appearance"), Description("Chữ hiển thị trên overlay khi Disable.")]
        public string DisabledOverlayText { get => _disabledOverlayText; set { _disabledOverlayText = value; Invalidate(); } }

        private Font _disabledOverlayFont = null; // null = dùng Font của control (bold)
        [Category("Appearance"), Description("Font chữ overlay khi Disable (để null dùng Font hiện tại).")]
        public Font DisabledOverlayFont { get => _disabledOverlayFont; set { _disabledOverlayFont = value; Invalidate(); } }

        private Color _disabledOverlayForeColor = Color.White;
        [Category("Appearance"), Description("Màu chữ overlay khi Disable.")]
        public Color DisabledOverlayForeColor { get => _disabledOverlayForeColor; set { _disabledOverlayForeColor = value; Invalidate(); } }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            foreach (Control c in Controls) c.Enabled = this.Enabled;

            
            Invalidate();
        }

        // ====== Painting ======
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Nếu BackColor trong suốt hoặc TitleBackColor = Transparent -> copy nền parent
            bool transparentFill = (!_titleBackColor.IsEmpty && _titleBackColor.A == 0);
            if ((BackColor.A == 0 || transparentFill) && Parent != null)
            {
                var g = e.Graphics;
                var s = g.Save();
                g.TranslateTransform(-Left, -Top);
                var pe = new PaintEventArgs(g, Parent.ClientRectangle);
                InvokePaintBackground(Parent, pe);
                g.Restore(s);
                return;
            }
            base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.Half;

            // 1) Toạ độ float + canh tâm pixel cho nét lẻ
            float penW = Math.Max(1f, _borderWidth);
            bool odd = (Math.Round(penW) % 2) == 1;
            float half = odd ? 0.5f : 0f;

            RectangleF avail = new RectangleF(1f, 1f,
                Math.Max(0.1f, Width - 2f),
                Math.Max(0.1f, Height - 2f));

            RectangleF fillRect = avail;
            RectangleF borderRect = new RectangleF(
                avail.X + half, avail.Y + half,
                Math.Max(0.1f, avail.Width - half * 2f),
                Math.Max(0.1f, avail.Height - half * 2f)
            );

            // 2) Chọn màu nền (hỗ trợ Transparent)
            bool customSet = ShouldSerializeTitleBackColor();
            bool transparentFill = customSet && _titleBackColor.A == 0;
            Color fill = customSet
                ? _titleBackColor
                : (_pressed ? PressBackColor : _hover ? HoverBackColor : NormalBackColor);

            using (var pathFill = RoundedRectF(fillRect, CornerRadius))
            {
                // Fill (bỏ qua khi Transparent)
                if (!transparentFill)
                {
                    using (var br = new SolidBrush(fill))
                        g.FillPath(br, pathFill);

                    // 2') Niêm mép: kẻ 1 stroke màu fill để che viền bán trong suốt ở rìa
                    using (var penSeal = new Pen(fill, 1f))
                    {
                        penSeal.Alignment = PenAlignment.Inset;
                        g.DrawPath(penSeal, pathFill);
                    }
                }

                // 3) Border thật (nằm trong, sắc nét)
                if (_borderVisible && penW > 0f)
                {
                    using (var pathBorder = RoundedRectF(borderRect, CornerRadius))
                    using (var pen = new Pen(Selected ? SelectedBorderColor : BorderColor,
                                             Selected ? Math.Max(1.5f, penW) : penW))
                    {
                        pen.Alignment = PenAlignment.Inset;
                        g.DrawPath(pen, pathBorder);
                    }
                }
            }

            // Text căn giữa
            //var textRect = Rectangle.Truncate(new RectangleF(
            //    fillRect.X + ButtonPadding.Left, fillRect.Y + ButtonPadding.Top,
            //    fillRect.Width - ButtonPadding.Horizontal, fillRect.Height - ButtonPadding.Vertical));

            // KHÔNG ellipsis khi ở FooterMode
            var flags = TextFormatFlags.EndEllipsis | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding;

            if (FooterMode) // footer thì không có "..."
                flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPadding;   

            //using (var fTitle = new Font(Font.FontFamily, _titleFontSize, FontStyle.Bold))
            //    TextRenderer.DrawText(g, _title ?? string.Empty, fTitle, textRect, TitleForeColor, flags);

            // vùng trong: trừ 1px viền mỗi cạnh rồi trừ padding
            var inner = Rectangle.Inflate(ClientRectangle, -1, -1);
            var textRect = new Rectangle(
                inner.X + ButtonPadding.Left,
                inner.Y + ButtonPadding.Top,
                Math.Max(0, inner.Width - ButtonPadding.Horizontal),
                Math.Max(0, inner.Height - ButtonPadding.Vertical)
            );

            using (var fTitle = new Font(Font.FontFamily, _titleFontSize, FontStyle.Bold))
                TextRenderer.DrawText(e.Graphics, _title ?? string.Empty, fTitle, textRect, TitleForeColor, flags);

            // ----- Overlay khi Disable -----
            if (!Enabled && _showDisabledOverlay)
            {
                // Vẽ một lớp phủ bo góc trùng với fillRect
                using (var pathOverlay = RoundedRectF(fillRect, CornerRadius))
                using (var brOverlay = new SolidBrush(_disabledOverlayColor))
                {
                    g.FillPath(brOverlay, pathOverlay);

                    // Viền mỏng cùng màu để niêm mép overlay (tránh viền răng cưa)
                    using (var penSeal = new Pen(_disabledOverlayColor, 1f))
                    {
                        penSeal.Alignment = PenAlignment.Inset;
                        g.DrawPath(penSeal, pathOverlay);
                    }
                }

                // Vẽ dòng chữ "Processing..." (hoặc tuỳ chỉnh)
                //if (!string.IsNullOrWhiteSpace(_disabledOverlayText))
                //{
                //    var overlayRect = Rectangle.Truncate(fillRect);
                //    var flagsOverlay = TextFormatFlags.HorizontalCenter |
                //                       TextFormatFlags.VerticalCenter |
                //                       TextFormatFlags.EndEllipsis |
                //                       TextFormatFlags.NoPadding;

                //    using (var fontOverlay = _disabledOverlayFont ?? new Font(Font, FontStyle.Bold))
                //    {
                //        TextRenderer.DrawText(g, _disabledOverlayText, fontOverlay,
                //                              overlayRect, _disabledOverlayForeColor, flagsOverlay);
                //    }
                //}
            }

        }

        // ===== Helpers =====
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

        // Giả lập click từ code
        [Browsable(false)]
        public void PerformClick()
        {
            //if (!Enabled) return;

            // giữ đúng thứ tự như OnMouseUp
            Selected = !Selected;           // toggle chọn
            CardClick?.Invoke(this, EventArgs.Empty); // bắn event custom
            OnClick(EventArgs.Empty);       // bắn event Click chuẩn (nếu ai lắng nghe)
            _pressed = false;
            Invalidate();
        }
    }



}
