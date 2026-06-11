using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NailsChekin.Models.Helper
{
    public static class RoundedButtonsUI
    {
        // ===== Double buffer =====
        public static void EnableDoubleBuffer(Control c)
        {
            typeof(Control).GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(c, true, null);

            typeof(Control).GetMethod("SetStyle",
                BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(c, new object[] {
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw, true });

            typeof(Control).GetMethod("UpdateStyles",
                BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(c, null);
        }

        // ===== Pause redraw/layout trong lúc update =====
        const int WM_SETREDRAW = 0x000B;
        [DllImport("user32.dll")] static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        public static void UpdatePanelFast(Control panel, Action updates)
        {
            panel.SuspendLayout();
            SendMessage(panel.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
            try { updates?.Invoke(); }
            finally
            {
                SendMessage(panel.Handle, WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
                panel.ResumeLayout(true);
                panel.Invalidate(true);
            }
        }

        // ===== Helper bo góc: chỉ vẽ VIỀN, giữ nguyên text/font/skin; có cache path =====
        private sealed class StyleState
        {
            public GraphicsPath Path;
            public int W, H, Radius;
            public float BorderWidth;
            public Color? BorderOverride;
            public Color? OriginalFore;
            public EventHandler OnResize;
            public PaintEventHandler OnPaint;
            public void Dispose() { Path?.Dispose(); Path = null; }
        }

        public static void Apply(SimpleButton btn, int radius, float borderWidth = 2f, Color? borderColor = null)
        {
            if (btn.Tag is StyleState old)
            {
                btn.Resize -= old.OnResize;
                btn.Paint -= old.OnPaint;
                old.Dispose();
                btn.Tag = null;
            }

            // bỏ viền vuông, giữ skin
            btn.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;

            var st = new StyleState { Radius = radius, BorderWidth = borderWidth, BorderOverride = borderColor };

            // Cache màu chữ gốc 1 lần và ép cho Appearance
            st.OriginalFore = btn.ForeColor;
            btn.Appearance.ForeColor = st.OriginalFore.Value;
            btn.Appearance.Options.UseForeColor = true; // QUAN TRỌNG: không cho skin ghi đè

            st.OnResize = (s, e) =>
            {
                st.Path?.Dispose();
                st.W = btn.Width; st.H = btn.Height;
                if (st.W <= 1 || st.H <= 1) return;

                var rect = new Rectangle(0, 0, st.W - 1, st.H - 1);
                st.Path = GetRoundPath(rect, st.Radius);
                btn.Region = new Region(st.Path);
                btn.Invalidate();
            };

            st.OnPaint = (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                if (st.Path == null || st.W != btn.Width || st.H != btn.Height) st.OnResize(s, e);
                if (st.Path == null) return;

                // đảm bảo mỗi lần vẽ text vẫn dùng màu gốc
                btn.Appearance.ForeColor = System.Drawing.Color.White; // st.OriginalFore ?? btn.ForeColor;
                btn.Appearance.Options.UseForeColor = true;

                Color border = st.BorderOverride
                               ?? (!btn.Appearance.BorderColor.IsEmpty ? btn.Appearance.BorderColor : btn.BackColor);

                using (var pen = new Pen(border, st.BorderWidth))
                    e.Graphics.DrawPath(pen, st.Path);

                // không vẽ nền/text → DevExpress tự vẽ đúng skin + font + ForeColor đã ép
            };

            btn.Resize += st.OnResize;
            btn.Paint += st.OnPaint;
            btn.Tag = st;
            st.OnResize(btn, EventArgs.Empty);
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

        // ======= API 1 dòng để dùng: =======
        public static void ShowPanelRoundedButtons(Control panel, int radius, float borderWidth = 2f, Color? borderColor = null, Action extraUpdates = null)
        {
            EnableDoubleBuffer(panel);
            UpdatePanelFast(panel, () =>
            {
                ApplyAll(panel, radius, borderWidth, borderColor); // bo góc + viền cho mọi SimpleButton
                extraUpdates?.Invoke();                            // chỗ này tuỳ bạn cập nhật thêm dữ liệu/UI
            });
            panel.Visible = true; // nếu panel là overlay, bạn có thể tự set ngoài hàm
        }
    }
}
