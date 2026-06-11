using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace NailsChekin.Models.Helper
{
    class DxRoundedButtonHelper
    {
        // Bộ nhớ đệm + handlers để tháo lắp gọn gàng
        private sealed class StyleState
        {
            public GraphicsPath Path;
            public int W, H, Radius;
            public float BorderWidth;
            public Color? BorderColorOverride;

            public EventHandler OnResize;
            public PaintEventHandler OnPaint;

            public void Dispose()
            {
                Path?.Dispose();
                Path = null;
            }
        }

        public static void Apply(SimpleButton btn, int radius, float borderWidth = 2f, Color? borderColor = null)
        {
            // Tháo style cũ nếu đã gắn
            if (btn.Tag is StyleState old)
            {
                btn.Resize -= old.OnResize;
                btn.Paint -= old.OnPaint;
                old.Dispose();
                btn.Tag = null;
            }

            // Giữ skin hiện tại; chỉ bỏ viền vuông của DevExpress
            btn.ButtonStyle = BorderStyles.NoBorder;
            // KHÔNG can thiệp BackColor/ForeColor/Text → DevExpress tự vẽ như cũ

            var state = new StyleState
            {
                Radius = radius,
                BorderWidth = borderWidth,
                BorderColorOverride = borderColor
            };

            state.OnResize = (s, e) =>
            {
                state.Path?.Dispose();
                state.W = btn.Width;
                state.H = btn.Height;

                if (state.W <= 1 || state.H <= 1) return;

                var rect = new Rectangle(0, 0, state.W - 1, state.H - 1);
                state.Path = GetRoundPath(rect, state.Radius);

                // Gán Region một lần theo kích thước mới
                btn.Region = new Region(state.Path);
                btn.Invalidate();
            };

            state.OnPaint = (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                // Phòng khi Paint chạy trước Resize
                if (state.Path == null || state.W != btn.Width || state.H != btn.Height)
                    state.OnResize(s, e);

                if (state.Path == null) return;

                // MÀU VIỀN: ưu tiên màu truyền vào → Appearance.BorderColor → BackColor
                Color border =
                    state.BorderColorOverride
                    ?? (!btn.Appearance.BorderColor.IsEmpty ? btn.Appearance.BorderColor : btn.BackColor);

                using (var pen = new Pen(border, state.BorderWidth))
                    e.Graphics.DrawPath(pen, state.Path);

                // Không vẽ nền/text: để DevExpress vẽ theo skin → text/Font giữ nguyên
            };

            btn.Resize += state.OnResize;
            btn.Paint += state.OnPaint;
            btn.Tag = state;

            // Khởi tạo ngay lần đầu
            state.OnResize(btn, EventArgs.Empty);
        }

        public static void ApplyAll(Control root, int radius, float borderWidth = 2f, Color? borderColor = null)
        {
            foreach (Control c in root.Controls)
            {
                if (c is SimpleButton sb)
                    Apply(sb, radius, borderWidth, borderColor);

                if (c.HasChildren)
                    ApplyAll(c, radius, borderWidth, borderColor);
            }
        }

        // Util tạo path bo góc
        private static GraphicsPath GetRoundPath(Rectangle r, int radius)
        {
            int d = Math.Max(0, radius) * 2;
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }
    }
}
