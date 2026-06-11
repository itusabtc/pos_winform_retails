using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NailsChekin.MyControls
{
    /// <summary>
    /// Label tự co chữ tối đa 2 dòng, dư sẽ hiện "…".
    /// - Đặt AutoSize = false để control tính đúng trong khung.
    /// - Chỉnh MinFontSize nếu muốn nhỏ hơn/lớn hơn.
    /// - Căn trái/giữa/phải theo TextAlign như Label thường.
    /// </summary>
    public class TwoLineAutoShrinkLabel : Label
    {
        // --- Cấu hình ---
        private float _minPixelHeight = 12f; // tối thiểu 12px như yêu cầu
        private int _maxLines = 2;
        private bool _autoShrink = true;

        // Giữ để tương thích: nếu bạn vẫn muốn set theo point thì có property bên dưới
        private float _minFontSizePoints = 6f; // chỉ dùng khi UseMinInPixels = false
        public bool UseMinInPixels { get; set; } = true; // mặc định dùng pixel

        private Font _renderFont;

        public TwoLineAutoShrinkLabel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw, true);

            AutoSize = false;
            _renderFont = (Font)Font.Clone();
        }

        /// <summary>Chiều cao chữ tối thiểu tính theo pixel (mặc định 12px)</summary>
        [Category("Behavior")]
        [Description("Chiều cao chữ tối thiểu tính theo pixel. Mặc định 12.")]
        public float MinPixelHeight
        {
            get => _minPixelHeight;
            set { _minPixelHeight = Math.Max(8f, value); Refit(); }
        }

        /// <summary>Số dòng tối đa (mặc định 2)</summary>
        [Category("Behavior")]
        public int MaxLines
        {
            get => _maxLines;
            set { _maxLines = Math.Max(1, value); Refit(); }
        }

        /// <summary>Tự co chữ khi tràn</summary>
        [Category("Behavior")]
        public bool AutoShrink
        {
            get => _autoShrink;
            set { _autoShrink = value; Refit(); }
        }

        /// <summary>(Tùy chọn) Cỡ chữ nhỏ nhất theo point nếu không dùng pixel</summary>
        [Category("Behavior")]
        [Description("Chỉ dùng khi UseMinInPixels = false.")]
        public float MinFontSizePoints
        {
            get => _minFontSizePoints;
            set { _minFontSizePoints = Math.Max(6f, value); Refit(); }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _renderFont?.Dispose();
            _renderFont = (Font)Font.Clone();
            Refit();
        }
        protected override void OnTextChanged(EventArgs e) { base.OnTextChanged(e); Refit(); }
        protected override void OnSizeChanged(EventArgs e) { base.OnSizeChanged(e); Refit(); }

        private float PixelsToPoints(float px, float dpiY) => px * 72f / dpiY;

        private void Refit()
        {
            if (!AutoShrink || string.IsNullOrEmpty(Text) || Width <= 4 || Height <= 4)
            {
                Invalidate();
                return;
            }

            var rect = GetTextRect();
            if (rect.Width <= 4 || rect.Height <= 4) { Invalidate(); return; }

            float bestPt = Font.Size;

            var flags = TextFormatFlags.WordBreak |
                        TextFormatFlags.EndEllipsis |
                        TextFormatFlags.TextBoxControl |
                        TextFormatFlags.NoPadding;

            using (var g = CreateGraphics())
            {
                // Tính min theo point dựa trên yêu cầu 12px
                float minPt = UseMinInPixels
                    ? PixelsToPoints(_minPixelHeight, g.DpiY)
                    : _minFontSizePoints;

                // Giới hạn minPt không quá nhỏ
                minPt = Math.Max(6f, minPt);

                // Giảm dần kích thước để vừa tối đa 2 dòng và chiều cao control
                for (float tryPt = Math.Min(Font.Size, bestPt); tryPt >= minPt; tryPt -= 0.5f)
                {
                    using (var tf = new Font(Font.FontFamily, tryPt, Font.Style, Font.Unit))
                    {
                        Size measured = TextRenderer.MeasureText(
                            g, Text, tf, new Size(rect.Width, int.MaxValue), flags);

                        int lineH = TextRenderer.MeasureText(
                            g, "Ay", tf, Size.Empty, TextFormatFlags.NoPadding | TextFormatFlags.TextBoxControl).Height;

                        if (measured.Height <= Math.Min(rect.Height, _maxLines * lineH))
                        {
                            bestPt = tryPt;
                            break;
                        }
                    }
                }
            }

            _renderFont?.Dispose();
            _renderFont = new Font(Font.FontFamily, bestPt, Font.Style, Font.Unit);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Nền
            e.Graphics.Clear(BackColor);

            // Nếu rỗng thì thôi
            if (string.IsNullOrEmpty(Text) || _renderFont == null)
                return;

            // Vùng vẽ (trừ Padding)
            Rectangle rect = GetTextRect();
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            // ===== FLAGS =====
            // Dùng để ĐO: khít sát để tính đúng wrap/height
            TextFormatFlags flagsMeasure = TextFormatFlags.WordBreak
                                         | TextFormatFlags.EndEllipsis
                                         | TextFormatFlags.TextBoxControl
                                         | TextFormatFlags.NoPadding;

            // Dùng để VẼ: không NoPadding để tránh cắt; thêm GlyphOverhangPadding cho anti-alias/overhang
            TextFormatFlags flagsDraw = TextFormatFlags.WordBreak
                                      | TextFormatFlags.EndEllipsis
                                      | TextFormatFlags.TextBoxControl
                                      | TextFormatFlags.GlyphOverhangPadding;

            // Căn ngang theo TextAlign
            switch (TextAlign)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.BottomLeft: flagsDraw |= TextFormatFlags.Left; break;
                case ContentAlignment.TopCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.BottomCenter: flagsDraw |= TextFormatFlags.HorizontalCenter; break;
                default: flagsDraw |= TextFormatFlags.Right; break;
            }

            // ===== ĐO LINE HEIGHT & CHIỀU CAO THỰC =====
            int lineH = TextRenderer.MeasureText(
                e.Graphics, "Ay", _renderFont, Size.Empty,
                TextFormatFlags.NoPadding | TextFormatFlags.TextBoxControl).Height;

            int maxBlockH = Math.Min(rect.Height, _maxLines * lineH);

            // Đo chiều cao thực khi wrap theo width (không giới hạn height)
            Size measured = TextRenderer.MeasureText(
                e.Graphics, Text, _renderFont, new Size(rect.Width, int.MaxValue), flagsMeasure);

            int drawH = Math.Min(measured.Height, maxBlockH);

            // ===== FUDGE tránh bị cắt trên/dưới và chừa chỗ ellipsis bên phải =====
            int fudgeY = (DeviceDpi >= 144) ? 2 : 1;  // DPI cao nới thêm
            int fudgeRight = 1;

            Rectangle drawRect = new Rectangle(
                rect.Left,
                rect.Top,
                Math.Max(0, rect.Width - fudgeRight),
                Math.Min(rect.Height, drawH + fudgeY)
            );

            // ===== CĂN DỌC (Top/Middle/Bottom) =====
            bool isMiddle = TextAlign == ContentAlignment.MiddleLeft
                         || TextAlign == ContentAlignment.MiddleCenter
                         || TextAlign == ContentAlignment.MiddleRight;

            bool isBottom = TextAlign == ContentAlignment.BottomLeft
                         || TextAlign == ContentAlignment.BottomCenter
                         || TextAlign == ContentAlignment.BottomRight;

            if (isMiddle)
                drawRect.Y = rect.Top + (rect.Height - drawRect.Height) / 2;
            else if (isBottom)
                drawRect.Y = rect.Bottom - drawRect.Height;
            // Top* thì giữ nguyên

            // ===== VẼ =====
            TextRenderer.DrawText(e.Graphics, Text, _renderFont, drawRect, ForeColor, flagsDraw);
        }

        private Rectangle GetTextRect() =>
            new Rectangle(Padding.Left, Padding.Top,
                          Math.Max(0, Width - Padding.Horizontal),
                          Math.Max(0, Height - Padding.Vertical));

        protected override void Dispose(bool disposing)
        {
            if (disposing) _renderFont?.Dispose();
            base.Dispose(disposing);
        }
    }
}
