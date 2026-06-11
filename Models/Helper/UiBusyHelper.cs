using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NailsChekin.Models.Helper
{
    public static class UiBusyHelper
    {
        const int WM_SETREDRAW = 0x000B;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        /// Khóa vẽ + layout (cả cây con nếu muốn) trong lúc build lại UI, rồi bật lại 1 lần.
        public static void WithRedrawLockedDeep(Control root, Action build, bool lockChildren = true)
        {
            if (root == null || build == null) return;
            if (!root.IsHandleCreated) root.CreateControl();

            var list = new List<Control> { root };
            if (lockChildren) CollectDescendants(root, list);

            var autoScrollSaved = new Dictionary<ScrollableControl, bool>();

            // --- KHÓA ---
            foreach (var c in list)
            {
                if (c == null || c.IsDisposed) continue;
                try
                {
                    c.SuspendLayout();
                    if (!c.IsHandleCreated) c.CreateControl();
                    if (c.IsHandleCreated) SendMessage(c.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);

                    var sc = c as ScrollableControl;
                    if (sc != null && !sc.IsDisposed)
                    {
                        autoScrollSaved[sc] = sc.AutoScroll;
                        sc.AutoScroll = false;
                    }
                }
                catch { /* swallow: control có thể bị dispose trong lúc build */ }
            }

            try
            {
                build(); // Clear/Add/Dispose... diễn ra ở đây
            }
            finally
            {
                // --- MỞ KHÓA (đi ngược) ---
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    var c = list[i];
                    if (c == null || c.IsDisposed) continue;

                    try
                    {
                        if (c.IsHandleCreated) SendMessage(c.Handle, WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
                        c.ResumeLayout(performLayout: true);
                        c.Invalidate(true);
                        c.Update();

                        var sc = c as ScrollableControl;
                        bool v;
                        if (sc != null && !sc.IsDisposed && autoScrollSaved.TryGetValue(sc, out v))
                            sc.AutoScroll = v;
                    }
                    catch { /* bỏ qua control vừa bị dispose */ }
                }
            }
        }

        private static void CollectDescendants(Control parent, List<Control> bag)
        {
            foreach (Control ch in parent.Controls)
            {
                bag.Add(ch);
                if (ch.HasChildren) CollectDescendants(ch, bag);
            }
        }

        /// Build 1 control mới (cùng kiểu với oldContent) OFF-SCREEN,
        /// build nội dung vào đó, rồi thay thế oldContent trong container.
        /// Trả về control mới để bạn giữ tham chiếu.
        public static T HotSwapRebuild<T>(Control container, T oldContent, Action<T> buildIntoNew) where T : Control
        {
            if (container == null || oldContent == null || buildIntoNew == null) return oldContent;

            // 1) Tạo instance mới cùng kiểu
            var t = oldContent.GetType();
            var newContent = (T)Activator.CreateInstance(t);

            // copy thuộc tính bố cục cơ bản
            newContent.Bounds = oldContent.Bounds;
            newContent.Dock = oldContent.Dock;
            newContent.Margin = oldContent.Margin;
            newContent.Padding = oldContent.Padding;
            newContent.BackColor = oldContent.BackColor;
            newContent.Font = oldContent.Font;

            // C# 7.0: dùng "as" thay cho pattern matching
            var so = oldContent as ScrollableControl;
            var sn = newContent as ScrollableControl;
            if (so != null && sn != null)
                sn.AutoScroll = so.AutoScroll;

            // double-buffer để mượt
            EnableTrueDoubleBuffer(newContent, recursive: true);

            // 2) Build OFF-SCREEN
            newContent.SuspendLayout();
            buildIntoNew(newContent);
            newContent.ResumeLayout(false);

            // 3) Thay thế 1 phát
            container.SuspendLayout();
            int idx = 0;
            try { idx = container.Controls.GetChildIndex(oldContent); } catch { idx = container.Controls.Count; }

            container.Controls.Add(newContent);
            container.Controls.SetChildIndex(newContent, idx);
            newContent.Visible = true;

            container.Controls.Remove(oldContent);
            oldContent.Dispose();

            container.ResumeLayout(true);

            return newContent;
        }
        /// <summary>
        /// Rebuild UI cho 1 container mà KHÔNG flicker:
        /// phủ snapshot hiện tại lên, rebuild bên dưới, rồi tháo snapshot.
        /// </summary>
        public static void RebuildNoFlicker(Control root, Action rebuild, bool lockChildren = true)
        {
            if (root == null || rebuild == null) return;
            if (!root.IsHandleCreated) root.CreateControl();

            // 1) Chụp snapshot
            var size = root.ClientSize;
            if (size.Width <= 0 || size.Height <= 0) { rebuild(); return; }

            using (var bmp = new Bitmap(size.Width, size.Height))
            {
                // DrawToBitmap đôi khi cần OnShown rồi mới “bắt” được đầy đủ
                root.DrawToBitmap(bmp, new Rectangle(Point.Empty, size));

                // 2) Phủ overlay (snapshot)
                var overlay = new PictureBox
                {
                    Dock = DockStyle.Fill,
                    Margin = Padding.Empty,
                    Image = (Image)bmp.Clone(),
                    SizeMode = PictureBoxSizeMode.StretchImage
                };

                root.SuspendLayout();
                root.Controls.Add(overlay);
                overlay.BringToFront();
                root.ResumeLayout(false);

                try
                {
                    // 3) Rebuild ở dưới, có khóa redraw/layout sâu
                    WithRedrawLockedDeep(root, () => rebuild(), lockChildren);
                }
                finally
                {
                    // 4) Tháo overlay
                    root.SuspendLayout();
                    if (!overlay.IsDisposed)
                    {
                        root.Controls.Remove(overlay);
                        overlay.Image?.Dispose();
                        overlay.Dispose();
                    }
                    root.ResumeLayout(true);
                    root.Invalidate(true);
                    root.Update();
                }
            }
        }

        /// <summary>
        /// Bật double-buffer thực sự để giảm nhấp nháy lâu dài (gọi 1 lần sau InitializeComponent).
        /// </summary>
        public static void EnableTrueDoubleBuffer(Control c, bool recursive = false)
        {
            if (c == null) return;

            var t = c.GetType();

            var piDB = t.GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            piDB?.SetValue(c, true, null);

            var miSetStyle = t.GetMethod("SetStyle",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            miSetStyle?.Invoke(c, new object[] {
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true });

            var miUpdate = t.GetMethod("UpdateStyles",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            miUpdate?.Invoke(c, null);

            if (recursive)
                foreach (Control child in c.Controls)
                    EnableTrueDoubleBuffer(child, true);
        }

        /// Phủ overlay trong suốt chặn thao tác, KHÔNG đổi màu/Enabled, không redraw nặng
        public static async Task RunWithOverlayAsync(Control root, Func<Task> action, Color? overlayColor = null, bool showWaitCursor = true)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));
            if (action == null) throw new ArgumentNullException(nameof(action));

            var overlay = new BusyOnlyOverlay
            {
                Dock = DockStyle.Fill,
                OverlayColor = overlayColor ?? Color.FromArgb(0, 0, 0, 0),
                Cursor = showWaitCursor ? Cursors.WaitCursor : Cursors.Default,
                Margin = Padding.Empty
            };

            root.SuspendLayout();
            root.Controls.Add(overlay);
            overlay.BringToFront();
            root.ResumeLayout(false);

            bool prevWait = root.UseWaitCursor;
            if (showWaitCursor) SetWaitCursorRecursive(root, true);

            try { await action().ConfigureAwait(true); }
            finally
            {
                if (showWaitCursor) SetWaitCursorRecursive(root, prevWait);
                root.SuspendLayout();
                if (!overlay.IsDisposed)
                {
                    root.Controls.Remove(overlay);
                    overlay.Dispose();
                }
                root.ResumeLayout(false);
            }
        }

        public static async Task RunWithClickShieldAsync(Control root, Func<Task> action, int? blockMs = null)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));
            if (action == null) throw new ArgumentNullException(nameof(action));

            var overlay = new BusyOnlyOverlay
            {
                OverlayColor = Color.FromArgb(0, 0, 0, 0), // trong suốt tuyệt đối
                Cursor = Cursors.Default,
            };

            root.SuspendLayout();
            root.Controls.Add(overlay);
            overlay.BringToFront();
            root.ResumeLayout(false);

            try
            {
                int ms = blockMs ?? SystemInformation.DoubleClickTime + 50;
                await Task.WhenAll(action(), Task.Delay(ms)).ConfigureAwait(true);
            }
            finally
            {
                root.SuspendLayout();
                if (!overlay.IsDisposed)
                {
                    root.Controls.Remove(overlay);
                    overlay.Dispose();
                }
                root.ResumeLayout(false);
                root.Invalidate(true);
                root.Update();
            }
        }

        private static void SetWaitCursorRecursive(Control root, bool on)
        {
            root.UseWaitCursor = on;
            foreach (Control c in root.Controls) SetWaitCursorRecursive(c, on);
        }

        /// <summary>
        /// Hiển thị overlay Loading (icon + text) trong khi chạy action, sau đó gỡ ra.
        /// - dimBackground: phủ lớp nền mờ nhẹ.
        /// - showWaitCursor: đổi con trỏ sang chờ.
        /// - spinnerGif: nếu có (Properties.Resources.spinner_32…), sẽ dùng GIF; nếu null dùng ProgressBar marquee.
        /// </summary>
        public static async Task ShowLoadingWhileAsyncBK(
            Control root,
            Func<Task> action,
            string text = "Loading…",
            bool dimBackground = false,
            bool showWaitCursor = true,
            Image spinnerGif = null)
        {
            if (root == null) throw new ArgumentNullException(nameof(root));
            if (action == null) throw new ArgumentNullException(nameof(action));

            if (!root.IsHandleCreated) root.CreateControl();

            // --- Tạo overlay ---
            var overlay = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = dimBackground ? Color.FromArgb(32, 0, 0, 0) : Color.Transparent,
                Margin = Padding.Empty
            };
            overlay.SetBounds(0, 0, root.ClientSize.Width, root.ClientSize.Height);
            overlay.TabStop = true; // nuốt focus
            overlay.Visible = true;

            // Hộp trung tâm chứa spinner + text
            var box = new Panel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(16, 12, 16, 12),
                BackColor = Color.Transparent // Color.FromArgb(20, 0, 0, 0) // rất nhạt; nếu muốn trong suốt hoàn toàn, để Transparent
            };

            Control spinnerCtrl;
            if (spinnerGif != null)
            {
                spinnerCtrl = new PictureBox
                {
                    Image = spinnerGif,
                    SizeMode = PictureBoxSizeMode.AutoSize,
                    Margin = new Padding(0, 0, 8, 0)
                };
            }
            else
            {
                var pb = new ProgressBar
                {
                    Style = ProgressBarStyle.Marquee,
                    MarqueeAnimationSpeed = 25,
                    Size = new Size(110, 16),
                    Margin = new Padding(0, 0, 8, 0)
                };
                spinnerCtrl = pb;
            }

            var inner = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent
            };
            inner.Controls.Add(spinnerCtrl);

            if (spinnerGif == null)
            {
                var lbl = new Label
                {
                    AutoSize = true,
                    Text = text ?? "",
                    Font = new Font("Segoe UI", 10f, FontStyle.Regular),
                    ForeColor = Color.FromArgb(50, 50, 50)
                };
                inner.Controls.Add(lbl);
            }

            box.Controls.Add(inner);

            // center box
            void CenterBox()
            {
                box.Left = Math.Max(0, (overlay.ClientSize.Width - box.Width) / 2);
                box.Top = Math.Max(0, (overlay.ClientSize.Height - box.Height) / 2);
            }
            overlay.Controls.Add(box);
            overlay.SizeChanged += (s, e) => CenterBox();

            // chặn mọi tương tác phía sau
            overlay.MouseDown += (s, e) => overlay.Focus();
            overlay.KeyDown += (s, e) => { /* swallow */ };

            // --- Gắn overlay ---
            root.SuspendLayout();
            root.Controls.Add(overlay);
            overlay.BringToFront();
            root.ResumeLayout(false);

            CenterBox();

            bool oldWait = root.UseWaitCursor;
            if (showWaitCursor) SetWaitCursorRecursive(root, true);

            try
            {
                // Cho overlay có cơ hội render trước khi chạy action nặng
                overlay.Update();
                await Task.Yield();

                // Chạy action (có thể chứa await tải dữ liệu)
                await action().ConfigureAwait(true);
            }
            finally
            {
                if (showWaitCursor) SetWaitCursorRecursive(root, oldWait);
                if (!overlay.IsDisposed)
                {
                    root.Controls.Remove(overlay);
                    overlay.Dispose();
                }
            }
        }

        public static async Task ShowLoadingWhileAsync(
            Control root,
            Func<Task> action,
            string text = "Loading…",
            bool dimBackground = false,
            bool showWaitCursor = true,
            Image spinnerGif = null,
            bool onlySpinnerGifIfProvided = false) // ⬅️ thêm tham số mới
        {
            if (root == null) throw new ArgumentNullException(nameof(root));
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (!root.IsHandleCreated) root.CreateControl();

            // Nếu đang ở chế độ "chỉ gif" và có gif → ép tắt dim/text
            bool effectiveDim = dimBackground && !(onlySpinnerGifIfProvided && spinnerGif != null);
            string effectiveText = (onlySpinnerGifIfProvided && spinnerGif != null) ? "" : (text ?? "");

            // --- Overlay ---
            var overlay = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = effectiveDim ? Color.FromArgb(32, 0, 0, 0) : Color.Transparent,
                Margin = Padding.Empty,
                TabStop = true,   // nuốt focus/phím
                Visible = true
            };
            overlay.SetBounds(0, 0, root.ClientSize.Width, root.ClientSize.Height);

            // Hộp trung tâm
            var box = new Panel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(16, 12, 16, 12),
                BackColor = Color.Transparent
            };

            Control spinnerCtrl;
            PictureBox pbGif = null;
            System.Windows.Forms.Timer gifTimer = null;

            if (spinnerGif != null)
            {
                pbGif = new PictureBox
                {
                    Image = spinnerGif,                      // dùng image gốc, không clone Bitmap
                    SizeMode = PictureBoxSizeMode.AutoSize,
                    Margin = new Padding(0, 0, effectiveText.Length > 0 ? 8 : 0, 0),
                    BackColor = Color.Transparent
                };
                spinnerCtrl = pbGif;

                // Ép animate gif (ổn định trong overlay trong suốt)
                //if (ImageAnimator.CanAnimate(spinnerGif))  ==> xài ảnh gift mới cần hiệu ứng GIFT hơi xấu
                //{
                //    gifTimer = new System.Windows.Forms.Timer { Interval = 10 }; // ~30fps
                //    gifTimer.Tick += (s, e) =>
                //    {
                //        try
                //        {
                //            ImageAnimator.UpdateFrames(spinnerGif);
                //            if (!pbGif.IsDisposed) pbGif.Invalidate();
                //        }
                //        catch { /* ignore */ }
                //    };
                //    ImageAnimator.Animate(spinnerGif, null);
                //    gifTimer.Start();
                //}
            }
            else
            {
                // fallback: progress bar + text
                var pb = new ProgressBar
                {
                    Style = ProgressBarStyle.Marquee,
                    MarqueeAnimationSpeed = 25,
                    Size = new Size(110, 16),
                    Margin = new Padding(0, 0, 8, 0)
                };
                spinnerCtrl = pb;
            }

            var inner = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent
            };
            inner.Controls.Add(spinnerCtrl);

            if (spinnerGif == null || effectiveText.Length > 0)   // không vẽ text khi "only gif"
            {
                var lbl = new Label
                {
                    AutoSize = true,
                    Text = effectiveText,
                    Font = new Font("Segoe UI", 10f, FontStyle.Regular),
                    ForeColor = Color.FromArgb(50, 50, 50),
                    Margin = new Padding(0, 1, 0, 0)
                };
                inner.Controls.Add(lbl);
            }

            box.Controls.Add(inner);

            void CenterBox()
            {
                box.Left = Math.Max(0, (overlay.ClientSize.Width - box.Width) / 2);
                box.Top = Math.Max(0, (overlay.ClientSize.Height - box.Height) / 2);
            }
            overlay.Controls.Add(box);
            overlay.SizeChanged += (s, e) => CenterBox();

            // chặn tương tác phía sau
            overlay.MouseDown += (s, e) => overlay.Focus();
            overlay.KeyDown += (s, e) => { /* swallow */ };

            // --- Gắn overlay ---
            root.SuspendLayout();
            root.Controls.Add(overlay);
            overlay.BringToFront();
            root.ResumeLayout(false);
            CenterBox();

            bool oldWait = root.UseWaitCursor;
            if (showWaitCursor) SetWaitCursorRecursive(root, true);

            try
            {
                // Cho overlay có cơ hội render trước khi chạy action nặng
                overlay.Update();
                await Task.Yield();

                await action().ConfigureAwait(true);
            }
            finally
            {
                // dừng gif
                try
                {
                    if (gifTimer != null)
                    {
                        gifTimer.Stop();
                        gifTimer.Dispose();
                    }
                    if (spinnerGif != null && ImageAnimator.CanAnimate(spinnerGif))
                        ImageAnimator.StopAnimate(spinnerGif, null);
                }
                catch { /* ignore */ }

                if (showWaitCursor) SetWaitCursorRecursive(root, oldWait);
                if (!overlay.IsDisposed)
                {
                    root.Controls.Remove(overlay);
                    overlay.Dispose();
                }
            }
        }

        // ===== Bản sync (nếu cần) =====
        public static void ShowLoadingWhile(
            Control host,
            Action action,
            string text = "Loading…",
            bool dimBackground = false,
            bool showWaitCursor = true,
            Image spinnerGif = null)
        {
            ShowLoadingWhileAsync(
                host,
                () => { action?.Invoke(); return Task.CompletedTask; },
                text, dimBackground, showWaitCursor, spinnerGif).GetAwaiter().GetResult();
        }

        // ===== Helpers =====
        private static async Task RunOnUIAsync(Control c, Action a)
        {
            var tcs = new TaskCompletionSource<bool>();
            if (c.IsHandleCreated)
            {
                if (c.InvokeRequired) c.BeginInvoke(new Action(() => { a(); tcs.TrySetResult(true); }));
                else { a(); tcs.TrySetResult(true); }
            }
            else
            {
                EventHandler h = null;
                h = (s, e) =>
                {
                    try { if (!c.IsDisposed) c.BeginInvoke(new Action(() => { a(); tcs.TrySetResult(true); })); }
                    finally { c.HandleCreated -= h; }
                };
                c.HandleCreated += h;
                c.CreateControl();
            }
            await tcs.Task.ConfigureAwait(true);
        }

        private static void ForceRepaintUp(Control c)
        {
            while (c != null && !c.IsDisposed)
            {
                c.Invalidate(true);
                c.Update();
                c.Refresh();
                c = c.Parent;
            }
        }

        /// Overlay thực sự trong suốt, không vẽ lại nền parent (tránh flicker)
        // Hợp nhất: BusyOverlay dùng được cho cả code cũ (OverlayColor) và code mới (Caption/Spinner/ShowDim)
        private sealed class BusyOverlay : Panel
        {
            // ==== API cũ (giữ nguyên) ====
            /// <summary>Màu phủ trực tiếp; nếu trong suốt sẽ không fill.</summary>
            public Color OverlayColor { get; set; } = Color.Transparent;

            // ==== API mới (bổ sung) ====
            /// <summary>Vẽ nền mờ đen (nếu OverlayColor vẫn trong suốt).</summary>
            public bool ShowDim { get; set; } = false;

            /// <summary>Ảnh spinner (gif/png). Nếu null sẽ vẽ spinner tĩnh bằng DrawArc.</summary>
            public Image Spinner { get; set; } = null;

            /// <summary>Chuỗi hiển thị dưới spinner.</summary>
            public string Caption { get; set; } = "Loading…";

            public BusyOverlay()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.UserPaint |
                         ControlStyles.SupportsTransparentBackColor, true);

                BackColor = Color.Transparent;   // không phá trong suốt nền dưới
                Dock = DockStyle.Fill;
                TabStop = true;                  // giữ focus để chặn phím
                Cursor = Cursors.WaitCursor;
            }

            // Mẹo: WS_EX_TRANSPARENT → không xoá nền, cho “nhìn xuyên” xuống dưới
            protected override CreateParams CreateParams
            {
                get { var cp = base.CreateParams; cp.ExStyle |= 0x00000020; return cp; } // WS_EX_TRANSPARENT
            }

            // Tránh fill nền xám
            protected override void OnPaintBackground(PaintEventArgs e) { /* NO-OP */ }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;

                // 1) Lớp phủ: ưu tiên OverlayColor, nếu trong suốt mà ShowDim=true thì mờ đen
                if (OverlayColor.A > 0)
                {
                    using (var br = new SolidBrush(OverlayColor))
                        g.FillRectangle(br, ClientRectangle);
                }
                else if (ShowDim)
                {
                    using (var br = new SolidBrush(Color.FromArgb(96, 0, 0, 0))) // ~38% mờ
                        g.FillRectangle(br, ClientRectangle);
                }

                // 2) Spinner + Caption (tùy chọn)
                int cx = Width / 2, cy = Height / 2;

                if (Spinner != null)
                {
                    int w = Spinner.Width, h = Spinner.Height;
                    g.DrawImage(Spinner, cx - w / 2, cy - h - 8, w, h);
                }
                else
                {
                    // spinner tĩnh siêu nhẹ
                    int r = Math.Max(16, Math.Min(Width, Height) / 10);
                    using (var p = new Pen(Color.White, 3f))
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                        g.DrawArc(p, cx - r, cy - r - 10, 2 * r, 2 * r, 30, 300);
                    }
                }

                if (!string.IsNullOrEmpty(Caption))
                {
                    var sz = TextRenderer.MeasureText(Caption, Font);
                    var rect = new Rectangle(cx - sz.Width / 2, cy + 4, sz.Width, sz.Height);
                    TextRenderer.DrawText(g, Caption, Font, rect, Color.White,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
            }

            // Nuốt mọi tương tác để chặn click/phím trong lúc loading
            protected override void OnMouseDown(MouseEventArgs e) { Focus(); }
            protected override void OnMouseMove(MouseEventArgs e) { }
            protected override void OnMouseUp(MouseEventArgs e) { }
            protected override bool ProcessCmdKey(ref Message msg, Keys keyData) => true;
        }

        //Không Icon, Text chỉ hiện 1 lớp phủ bên trên
        sealed class BusyOnlyOverlay : Control
        {
            public Color OverlayColor { get; set; } = Color.FromArgb(0, 0, 0, 0);

            public BusyOnlyOverlay()
            {
                SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
                SetStyle(ControlStyles.Opaque, true);   // tự xử lý nền, tránh artefact
                TabStop = true;
                Dock = DockStyle.Fill;
                Cursor = Cursors.Default;
                BackColor = Color.Black;                // không dùng tới (OnPaintBackground đã override)
                Margin = Padding.Empty;
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
                // không gọi base để tránh “đen góc”
                if (OverlayColor.A > 0)
                {
                    using (var b = new SolidBrush(OverlayColor))
                        e.Graphics.FillRectangle(b, ClientRectangle);
                }
                // A==0: zero-paint (không vẽ gì)
            }
            protected override void OnPaint(PaintEventArgs e) { /* no-op */ }

            protected override void WndProc(ref Message m)
            {
                const int WM_MOUSEACTIVATE = 0x21;
                const int MA_NOACTIVATEANDEAT = 4;
                if (m.Msg == WM_MOUSEACTIVATE)
                {
                    m.Result = (IntPtr)MA_NOACTIVATEANDEAT; // nuốt chuột, không kích hoạt control dưới
                    return;
                }
                base.WndProc(ref m);
            }

            public static void ShowClickShield(Control root, int? durationMs = null)
            {
                if (root == null || root.IsDisposed) return;

                const string ShieldTag = "__ClickShield__";
                var exists = root.Controls.OfType<BusyOnlyOverlay>().FirstOrDefault(o => Equals(o.Tag, ShieldTag));
                if (exists != null) return;

                var overlay = new BusyOnlyOverlay
                {
                    OverlayColor = Color.FromArgb(0, 0, 0, 0),
                    Cursor = Cursors.Default,
                    Margin = Padding.Empty,
                    TabStop = true,
                    Tag = ShieldTag
                };

                int ms = durationMs ?? SystemInformation.DoubleClickTime + 50;

                root.SuspendLayout();
                root.Controls.Add(overlay);
                overlay.BringToFront();
                root.ResumeLayout(false);

                var t = new System.Windows.Forms.Timer { Interval = ms };
                t.Tick += (_, __) =>
                {
                    t.Stop(); t.Dispose();
                    if (!overlay.IsDisposed)
                    {
                        root.SuspendLayout();
                        root.Controls.Remove(overlay);
                        overlay.Dispose();
                        root.ResumeLayout(false);
                    }
                };
                t.Start();
            }
        }

    }

}
