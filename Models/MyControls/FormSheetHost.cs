using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;

namespace NailsChekin.MyControls
{
    public static class FormSheetHost
    {
        // Hiển thị form như một sheet phủ lên host; trả về DialogResult khi form đóng
        public static Task<DialogResult> ShowAsync(Control host, Form form, bool dimBackground = false)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));
            if (form == null) throw new ArgumentNullException(nameof(form));

            var tcs = new TaskCompletionSource<DialogResult>();

            if (!host.IsHandleCreated) host.CreateControl();

            // overlay để chặn tương tác host (không nháy)
            var overlay = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = dimBackground ? Color.FromArgb(32, 0, 0, 0) : Color.Transparent,
                Margin = Padding.Empty
            };
            overlay.Click += (s, e) => overlay.Focus();
            overlay.TabStop = true;

            // Chuẩn bị form để embed
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.ControlBox = false;
            form.ShowIcon = false;
            form.ShowInTaskbar = false;
            form.StartPosition = FormStartPosition.Manual;
            form.Dock = DockStyle.Fill;
            form.KeyPreview = true; // cho ESC, v.v.

            // double-buffer cho form và host (giảm flicker)
            EnableTrueDoubleBuffer(host, true);
            EnableTrueDoubleBuffer(form, true);

            // Cleanup khi form đóng
            void Cleanup()
            {
                if (!host.IsDisposed)
                {
                    if (host.Controls.Contains(form)) host.Controls.Remove(form);
                    if (host.Controls.Contains(overlay)) host.Controls.Remove(overlay);
                }
                form.Dispose();
                overlay.Dispose();
            }

            form.FormClosed += (s, e) =>
            {
                var dr = form.DialogResult;
                Cleanup();
                tcs.TrySetResult(dr);
            };

            host.SuspendLayout();
            host.Controls.Add(overlay);
            overlay.BringToFront();
            host.Controls.Add(form);
            form.BringToFront();
            host.ResumeLayout(false);

            form.Show();     // hiển thị ngay như sheet
            form.Focus();

            return tcs.Task;
        }

        // Bản sync (nếu bạn chưa muốn async/await)
        public static DialogResult Show(Control host, Form form, bool dimBackground = false)
            => ShowAsync(host, form, dimBackground).GetAwaiter().GetResult();

        // bật double-buffer bằng reflection (an toàn với mọi control)
        public static void EnableTrueDoubleBuffer(Control c, bool recursive = false)
        {
            if (c == null) return;
            var t = c.GetType();

            var piDB = t.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            piDB?.SetValue(c, true, null);

            var miSet = t.GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic);
            miSet?.Invoke(c, new object[] {
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true });

            var miUpd = t.GetMethod("UpdateStyles", BindingFlags.Instance | BindingFlags.NonPublic);
            miUpd?.Invoke(c, null);

            if (recursive)
                foreach (Control ch in c.Controls) EnableTrueDoubleBuffer(ch, true);
        }
    }
}
