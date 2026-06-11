using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace NailsChekin.Models.Implements
{
    public class KineticScrollPanel : Panel
    {
        // ====== (các hằng/interop WM_GESTURE như bản trước) ======
        const int WM_GESTURE = 0x0119;
        const int WM_GESTURENOTIFY = 0x011A;
        const uint GID_PAN = 4;
        const uint GF_BEGIN = 0x00000001;
        const uint GF_INERTIA = 0x00000002;
        const uint GF_END = 0x00000004;
        const uint GC_PAN = 0x00000001;
        const uint GC_PAN_WITH_SINGLE_FINGER_VERTICALLY = 0x00000002;
        const uint GC_PAN_WITH_SINGLE_FINGER_HORIZONTALLY = 0x00000004;
        const uint GC_PAN_WITH_INERTIA = 0x00000010;

        [StructLayout(LayoutKind.Sequential)] struct POINTS { public short x, y; }
        [StructLayout(LayoutKind.Sequential)]
        struct GESTUREINFO
        {
            public uint cbSize, dwFlags, dwID; public IntPtr hwndTarget; public POINTS ptsLocation;
            public uint dwInstanceID, dwSequenceID; public ulong ullArguments; public uint cbExtraArgs;
        }
        [StructLayout(LayoutKind.Sequential)]
        struct GESTURECONFIG { public uint dwID, dwWant, dwBlock; }

        [DllImport("user32.dll")] static extern bool GetGestureInfo(IntPtr hGestureInfo, ref GESTUREINFO pGestureInfo);
        [DllImport("user32.dll")] static extern bool CloseGestureInfoHandle(IntPtr hGestureInfo);
        [DllImport("user32.dll")]
        static extern bool SetGestureConfig(IntPtr hWnd, uint dwReserved, uint cIDs,
            [In] GESTURECONFIG[] pGestureConfig, uint cbSize);

        // ====== Config ======
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Panel Content { get; } = new Panel { Location = new Point(0, 0) };
        public bool EnableVertical { get; set; } = true;
        public bool EnableHorizontal { get; set; } = false;
        public bool HardStopEdges { get; set; } = true;
        public float Friction { get; set; } = 0.92f;

        public bool AutoRefreshLayoutBounds { get; set; } = true;

        // ====== State ======
        bool draggingMouse; Point lastMouse;
        Point lastPan; float velX, velY;
        readonly Timer inertiaTimer = new Timer { Interval = 16 }; // ~60fps

        int minX, maxX, minY, maxY;

        // auto-refresh scheduling
        bool _recalcScheduled;
        int _updateNesting;

        public KineticScrollPanel()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);

            Controls.Add(Content);

            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                return;

            inertiaTimer.Tick += (s, e) => {
                bool stopX = Math.Abs(velX) < 0.1f || !EnableHorizontal;
                bool stopY = Math.Abs(velY) < 0.1f || !EnableVertical;

                int dx = stopX ? 0 : (int)velX;
                int dy = stopY ? 0 : (int)velY;

                MoveContent(dx, dy);

                if (HardStopEdges)
                {
                    if ((Content.Left == maxX && velX > 0) || (Content.Left == minX && velX < 0)) velX = 0;
                    if ((Content.Top == maxY && velY > 0) || (Content.Top == minY && velY < 0)) velY = 0;
                }

                velX *= Friction;
                velY *= Friction;

                if ((stopX || velX == 0) && (stopY || velY == 0))
                    inertiaTimer.Stop();
            };

            // ==== AUTO REFRESH HOOKS ====
            Content.ControlAdded += (s, e) => { WireChild(e.Control, true); ScheduleRecalc(); };
            Content.ControlRemoved += (s, e) => { WireChild(e.Control, false); ScheduleRecalc(); };
            Content.SizeChanged += (s, e) => ScheduleRecalc();
            Content.Layout += (s, e) => ScheduleRecalc(); // thay đổi bố cục con

            // nếu đã có con từ trước:
            foreach (Control c in Content.Controls) WireChild(c, true);
        }

        // Cho phép gom nhiều thay đổi:
        public void BeginUpdate() { _updateNesting++; }
        public void EndUpdate() { if (_updateNesting > 0) _updateNesting--; if (_updateNesting == 0) ScheduleRecalc(); }

        void WireChild(Control c, bool wire)
        {
            if (c == null) return;
            if (wire)
            {
                c.SizeChanged += ChildChanged;
                c.LocationChanged += ChildChanged;
                c.VisibleChanged += ChildChanged;
            }
            else
            {
                c.SizeChanged -= ChildChanged;
                c.LocationChanged -= ChildChanged;
                c.VisibleChanged -= ChildChanged;
            }
        }
        void ChildChanged(object s, EventArgs e) => ScheduleRecalc();

        //void ScheduleRecalc()
        //{
        //    if (!AutoRefreshLayoutBounds) return;
        //    if (_updateNesting > 0) return;
        //    if (_recalcScheduled || IsDisposed) return;

        //    _recalcScheduled = true;
        //    BeginInvoke((Action)(() =>
        //    {
        //        if (IsDisposed) return;
        //        _recalcScheduled = false;
        //        RefreshLayoutBounds(); // thực hiện tính toán
        //    }));
        //}

        void ScheduleRecalc()
        {
            if (!AutoRefreshLayoutBounds) return;
            if (_updateNesting > 0) return;
            if (_recalcScheduled || IsDisposed) return;

            _recalcScheduled = true;

            if (!IsHandleCreated)
            {
                // Nếu chưa có handle, chờ đến khi tạo xong mới chạy
                HandleCreated += (s, e) =>
                {
                    if (IsDisposed) return;
                    _recalcScheduled = false;
                    RefreshLayoutBounds();
                };
                return;
            }

            BeginInvoke((Action)(() =>
            {
                if (IsDisposed) return;
                _recalcScheduled = false;
                RefreshLayoutBounds(); // thực hiện tính toán
            }));
        }

        // ==== Bounds/extent tính từ con thay vì dựa vào AutoSize ====
        public void RefreshLayoutBounds()
        {
            // Tính kích thước cần thiết của Content theo union bounds các child
            Rectangle ext = Rectangle.Empty;
            foreach (Control c in Content.Controls)
            {
                if (!c.Visible) continue;
                ext = ext == Rectangle.Empty ? c.Bounds : Rectangle.Union(ext, c.Bounds);
            }
            if (ext == Rectangle.Empty) ext = new Rectangle(0, 0, 0, 0);

            // Đặt size Content đủ chứa hết con
            Size need = new Size(Math.Max(0, ext.Right), Math.Max(0, ext.Bottom));
            if (Content.Size != need)
                Content.Size = need;

            RecalcBounds(); // cập nhật min/max và clamp vị trí
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            var cfg = new[] {
            new GESTURECONFIG {
                dwID = GID_PAN,
                dwWant = GC_PAN |
                         (EnableVertical   ? GC_PAN_WITH_SINGLE_FINGER_VERTICALLY   : 0) |
                         (EnableHorizontal ? GC_PAN_WITH_SINGLE_FINGER_HORIZONTALLY : 0) |
                         GC_PAN_WITH_INERTIA,
                dwBlock = 0
            }
        };
            SetGestureConfig(this.Handle, 0, (uint)cfg.Length, cfg, (uint)Marshal.SizeOf(typeof(GESTURECONFIG)));
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            RecalcBounds();
        }

        void RecalcBounds()
        {
            maxX = 0; maxY = 0;
            minX = Math.Min(0, Width - Content.Width);
            minY = Math.Min(0, Height - Content.Height);
            Clamp();
        }

        void Clamp()
        {
            if (Content.Left > maxX) Content.Left = maxX;
            if (Content.Left < minX) Content.Left = minX;
            if (Content.Top > maxY) Content.Top = maxY;
            if (Content.Top < minY) Content.Top = minY;
        }

        Point LimitDelta(int dx, int dy)
        {
            int lx = Content.Left, ly = Content.Top;
            int nx = lx + dx, ny = ly + dy;
            if (!EnableHorizontal) dx = 0;
            if (!EnableVertical) dy = 0;
            if (nx > maxX) dx = maxX - lx;
            if (nx < minX) dx = minX - lx;
            if (ny > maxY) dy = maxY - ly;
            if (ny < minY) dy = minY - ly;
            return new Point(dx, dy);
        }

        void MoveContent(int dx, int dy)
        {
            if (HardStopEdges)
            {
                var lim = LimitDelta(dx, dy);
                dx = lim.X; dy = lim.Y;

                if (dx == 0 && dy == 0)
                {
                    if (Content.Left == maxX || Content.Left == minX) velX = 0;
                    if (Content.Top == maxY || Content.Top == minY) velY = 0;
                    return;
                }
            }

            var p = Content.Location; p.Offset(dx, dy);
            Content.Location = p;

            if (!HardStopEdges) Clamp();
            Invalidate();
        }

        // ==== Mouse drag fallback ====
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            inertiaTimer.Stop();
            draggingMouse = true;
            lastMouse = e.Location;
            Capture = true;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!draggingMouse) return;
            int rawDx = e.X - lastMouse.X;
            int rawDy = e.Y - lastMouse.Y;
            var lim = HardStopEdges ? LimitDelta(rawDx, rawDy) : new Point(rawDx, rawDy);
            MoveContent(lim.X, lim.Y);
            velX = lim.X; velY = lim.Y;
            lastMouse = e.Location;
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (!draggingMouse) return;
            draggingMouse = false; Capture = false;
            if (Math.Abs(velX) > 0.5f || Math.Abs(velY) > 0.5f) inertiaTimer.Start();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (!EnableVertical) return;
            inertiaTimer.Stop();
            int dy = e.Delta / 2;
            MoveContent(0, dy);
        }

        // ==== Touch pan (WM_GESTURE) ====
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_GESTURE)
            {
                var gi = new GESTUREINFO { cbSize = (uint)Marshal.SizeOf(typeof(GESTUREINFO)) };
                if (GetGestureInfo(m.LParam, ref gi))
                {
                    if (gi.dwID == GID_PAN)
                    {
                        var ptScr = new Point(gi.ptsLocation.x, gi.ptsLocation.y);
                        var pt = PointToClient(ptScr);

                        if ((gi.dwFlags & GF_BEGIN) == GF_BEGIN)
                        {
                            inertiaTimer.Stop();
                            lastPan = pt;
                            velX = velY = 0;
                        }
                        else
                        {
                            int rawDx = EnableHorizontal ? (pt.X - lastPan.X) : 0;
                            int rawDy = EnableVertical ? (pt.Y - lastPan.Y) : 0;

                            var lim = HardStopEdges ? LimitDelta(rawDx, rawDy) : new Point(rawDx, rawDy);
                            MoveContent(lim.X, lim.Y);
                            velX = lim.X; velY = lim.Y;

                            lastPan = pt;
                        }
                        CloseGestureInfoHandle(m.LParam); m.Result = IntPtr.Zero; return;
                    }
                }
                CloseGestureInfoHandle(m.LParam);
            }
            base.WndProc(ref m);
        }

        // ==== API tiện ích ====
        public void ScrollTo(int x, int y)
        {
            inertiaTimer.Stop();
            Content.Location = new Point(
                Math.Max(minX, Math.Min(maxX, x)),
                Math.Max(minY, Math.Min(maxY, y))
            );
            Invalidate();
        }
        public void ScrollBy(int dx, int dy) => MoveContent(dx, dy);
        public Rectangle Viewport => new Rectangle(-Content.Left, -Content.Top, Width, Height);
    }

}
