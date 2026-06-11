using System;
using System.Drawing;
using System.Windows.Forms;

namespace NailsChekin.MyControls
{
    public class BusyOverlay : Control
    {
        private readonly PictureBox _pic;

        public BusyOverlay(Image icon, int iconSize = 96, int shadeAlpha = 0)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.Transparent;
            TabStop = false;

            _pic = new PictureBox
            {
                Image = icon,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(iconSize, iconSize),
                BackColor = Color.Transparent
            };
            Controls.Add(_pic);
            Resize += (s, e) => CenterIcon();
            CenterIcon();

            ShadeAlpha = shadeAlpha; // 0 = hoàn toàn trong suốt (chỉ hiện icon)
        }

        public int ShadeAlpha { get; set; }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT -> vẽ "trong suốt" không che nền
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (ShadeAlpha > 0)
            {
                using (var br = new SolidBrush(Color.FromArgb(ShadeAlpha, 0, 0, 0)))
                    e.Graphics.FillRectangle(br, ClientRectangle); // nếu muốn phủ mờ rất nhẹ
            }
        }

        private void CenterIcon()
        {
            if (_pic == null) return;
            _pic.Location = new Point((Width - _pic.Width) / 2, (Height - _pic.Height) / 2);
            _pic.BringToFront();
        }

        // Chặn click rơi xuống control phía dưới
        protected override void OnMouseDown(MouseEventArgs e) { /* swallow */ }
        protected override void OnMouseUp(MouseEventArgs e) { /* swallow */ }
        protected override void OnClick(EventArgs e) { /* swallow */ }

        public static BusyOverlay ShowOver(Control parent, Image icon, int iconSize = 96, int shadeAlpha = 0)
        {
            var ov = new BusyOverlay(icon, iconSize, shadeAlpha) { Dock = DockStyle.Fill };
            parent.Controls.Add(ov);
            ov.BringToFront();
            parent.Refresh(); // đảm bảo render ngay
            return ov;
        }

        public void CloseOverlay()
        {
            if (Parent != null) Parent.Controls.Remove(this);
            Dispose();
        }
    }
}
