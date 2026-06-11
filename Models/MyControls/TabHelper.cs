// TabHelper.cs  —  phiên bản đơn giản chỉ dùng Open/Close
namespace NailsChekin.MyControls
{
    using System;
    using System.Drawing;
    using System.Reflection;
    using System.Windows.Forms;

    /// <summary>
    /// Quản lý "sheet" đơn giản: tạo 1 RoundPanel theo Bounds và build nội dung.
    /// KHÔNG cache. Mỗi lần Open sẽ tạo sheet mới (bạn có thể CloseTopIfAny trước đó).
    /// </summary>
    public sealed class TabHelper
    {
        private readonly Control _host;

        public TabHelper(Control host)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));
            _host = host;

            EnableTrueDoubleBuffer(_host, recursive: true);
            _host.Resize += (s, e) =>
            {
                // Nếu muốn tự co giãn sheet đang trên cùng theo host.Resize thì mở comment:
                // var top = CurrentSheet;
                // if (top != null && !top.IsDisposed && _autoBounds.HasValue) top.Bounds = _autoBounds.Value();
            };
        }

        /// <summary>Tạo helper mới.</summary>
        public static TabHelper Attach(Control host) { return new TabHelper(host); }

        /// <summary>Sheet hiện tại (đang Visible) nếu có.</summary>
        public RoundPanel CurrentSheet
        {
            get
            {
                for (int i = _host.Controls.Count - 1; i >= 0; i--)
                {
                    var rp = _host.Controls[i] as RoundPanel;
                    if (rp != null && rp.Visible) return rp;
                }
                return null;
            }
        }

        /// <summary>
        /// Mở một sheet mới với Bounds chỉ định và build nội dung bên trong.
        /// Không cache. Bạn tự gọi CloseTopIfAny() nếu muốn chỉ có 1 sheet.
        /// </summary>
        public RoundPanel Open(
            string name,
            Rectangle bounds,
            Action<RoundPanel> build,
            bool transparent = true)
        {
            if (string.IsNullOrWhiteSpace(name)) name = Guid.NewGuid().ToString("N");
            if (build == null) throw new ArgumentNullException(nameof(build));

            RoundPanel result = null;

            RunOnUIBlocking(_host, () =>
            {
                // Tạo sheet mới (ẩn tạm trong lúc build để không bị nháy)
                var sheet = new RoundPanel
                {
                    Name = name,
                    Dock = DockStyle.None,
                    Bounds = bounds,
                    Margin = Padding.Empty,
                    Padding = new Padding(6),
                    BackColor = transparent ? Color.Transparent : Color.FromArgb(12, 0, 0, 0),
                    Visible = false
                };

                _host.SuspendLayout();
                try
                {
                    _host.Controls.Add(sheet);
                    _host.Controls.SetChildIndex(sheet, 0); // lên trên cùng trong host

                    EnableTrueDoubleBuffer(sheet, recursive: true);

                    sheet.SuspendLayout();
                    try
                    {
                        build(sheet); // bạn add UC/dock fill... ở đây
                    }
                    finally
                    {
                        sheet.ResumeLayout(performLayout: true);
                    }

                    sheet.Visible = true;
                    sheet.BringToFront();
                    sheet.PerformLayout();
                    sheet.Refresh();
                }
                finally
                {
                    _host.ResumeLayout(performLayout: true);
                }

                result = sheet;
            });

            return result;
        }

        // 1) Mở sheet RỖNG (không build gì) → để overlay bám vào trước
        public RoundPanel OpenEmpty(string name, Rectangle bounds, bool transparent = true)
        {
            if (string.IsNullOrWhiteSpace(name)) name = Guid.NewGuid().ToString("N");
            RoundPanel result = null;

            // y hệt Open(...) nhưng BỎ phần build
            RunOnUIBlocking(_host, () =>
            {
                var sheet = new RoundPanel
                {
                    Name = name,
                    Dock = DockStyle.None,
                    Bounds = bounds,
                    Margin = Padding.Empty,
                    Padding = new Padding(6),
                    BackColor = transparent ? Color.Transparent : Color.FromArgb(12, 0, 0, 0),
                    Visible = true,
                    Tag = "__SHEET_TAB__"
                };

                _host.SuspendLayout();
                try
                {
                    _host.Controls.Add(sheet);
                    _host.Controls.SetChildIndex(sheet, 0);
                    EnableTrueDoubleBuffer(sheet, recursive: true);
                    sheet.BringToFront();
                    sheet.PerformLayout();
                    sheet.Refresh();
                }
                finally
                {
                    _host.ResumeLayout(true);
                }
                result = sheet;
            });

            return result;
        }

        // 2) Build nội dung vào một sheet đã có (chạy gọn trong UI thread)
        public void BuildInto(RoundPanel sheet, Action<RoundPanel> build)
        {
            if (sheet == null || sheet.IsDisposed || build == null) return;
            sheet.SuspendLayout();
            try { build(sheet); }
            finally { sheet.ResumeLayout(true); sheet.PerformLayout(); sheet.Refresh(); }
        }

        /// <summary>Đóng sheet đang trên cùng (nếu có).</summary>
        public bool CloseTopIfAny()
        {
            RoundPanel top = null;
            RunOnUIBlocking(_host, () => { top = CurrentSheet; });
            if (top == null) return false;

            RunOnUIBlocking(_host, () =>
            {
                _host.SuspendLayout();
                try
                {
                    _host.Controls.Remove(top);
                    top.Dispose();
                }
                finally
                {
                    _host.ResumeLayout(performLayout: true);
                }
            });
            return true;
        }

        /// <summary>Đóng tất cả các sheet.</summary>
        public void CloseAll()
        {
            RunOnUIBlocking(_host, () =>
            {
                _host.SuspendLayout();
                try
                {
                    for (int i = _host.Controls.Count - 1; i >= 0; i--)
                    {
                        var rp = _host.Controls[i] as RoundPanel;
                        if (rp != null)
                        {
                            if (rp.Tag != null && rp.Tag.ToString().Equals("__SHEET_TAB__"))
                            {
                                _host.Controls.RemoveAt(i);
                                rp.Dispose();
                            }
                        }
                    }
                }
                finally
                {
                    _host.ResumeLayout(performLayout: true);
                }
            });
        }

        // ===== Helpers =====

        private static void RunOnUIBlocking(Control c, Action action)
        {
            if (c == null || action == null) return;

            if (c.IsHandleCreated)
            {
                if (c.InvokeRequired) c.Invoke(action);
                else action();
            }
            else
            {
                // Chưa có handle -> tạo handle và chạy sau khi tạo
                EventHandler h = null;
                h = (s, e) =>
                {
                    try
                    {
                        if (!c.IsDisposed)
                        {
                            if (c.InvokeRequired) c.Invoke(action);
                            else action();
                        }
                    }
                    finally { c.HandleCreated -= h; }
                };
                c.HandleCreated += h;
                c.CreateControl();
            }
        }

        /// <summary>Bật double-buffer để đỡ flicker (qua reflection, C#7 friendly).</summary>
        private static void EnableTrueDoubleBuffer(Control c, bool recursive)
        {
            if (c == null || c.IsDisposed) return;

            RunOnUIBlocking(c, () =>
            {
                if (c.IsDisposed) return;

                var t = c.GetType();

                // DoubleBuffered (protected)
                var piDB = t.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
                if (piDB != null) piDB.SetValue(c, true, null);

                // SetStyle (protected)
                var miSet = t.GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic);
                if (miSet != null)
                {
                    var styles =
                        ControlStyles.AllPaintingInWmPaint |
                        ControlStyles.OptimizedDoubleBuffer |
                        ControlStyles.UserPaint;
                    miSet.Invoke(c, new object[] { styles, true });
                }

                // UpdateStyles (protected)
                var miUpd = t.GetMethod("UpdateStyles", BindingFlags.Instance | BindingFlags.NonPublic);
                if (miUpd != null) miUpd.Invoke(c, null);

                if (recursive)
                {
                    foreach (Control ch in c.Controls)
                        EnableTrueDoubleBuffer(ch, true);
                }
            });
        }
    }
}


