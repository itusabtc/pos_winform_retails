using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace NailsChekin.Popup
{
    public partial class FormCloverProcessing : Form
    {
        // Expose control đã kéo thả sẵn trong Designer
        public FlowLayoutPanel ButtonsHost => UIStateButtonPanel;   // tên panel bạn đã đặt
        public Label StatusHost => DeviceCurrentStatus;             // tên label bạn đã đặt

        // Hủy giao dịch trực tiếp trên máy Clover (ResetDevice) khi không tìm thấy nút Cancel
        // động — ví dụ lúc đang "Connecting/Processing" máy chưa gửi InputOptions nên panel
        // chưa có nút Cancel. Nếu null thì giữ hành vi cũ (chỉ đóng popup).
        public Action CancelFallback;

        public FormCloverProcessing()
        {
            InitializeComponent();

            // Tùy chọn: căn chỉnh cho panel
            //UIStateButtonPanel.WrapContents = true;
            //UIStateButtonPanel.AutoScroll = true;
            UIStateButtonPanel.Padding = new Padding(5);
        }

        private void svgImageBox1_Click(object sender, System.EventArgs e)
        {
            // Thử kích hoạt đúng nút Cancel trong ButtonsHost
            if (TryClickCancelFrom(ButtonsHost))
                return;

            // Không tìm thấy nút Cancel (đang Connecting/Processing) -> ép hủy giao dịch trên máy Clover.
            try { CancelFallback?.Invoke(); } catch { /* hủy là best-effort, không chặn việc đóng form */ }

            this.Dispose();
        }

        private bool TryClickCancelFrom(FlowLayoutPanel host)
        {
            foreach (Control c in host.Controls)
            {
                if (!IsCancelControl(c)) continue;
                if (InvokeClick(c)) return true;
            }
            return false;
        }

        private bool IsCancelControl(Control c)
        {
            // 1) Nếu là IButtonControl và DialogResult = Cancel
            if (c is IButtonControl ib && ib.DialogResult == DialogResult.Cancel) return true;

            // 2) Dựa vào Text/Name/Tag chứa "cancel"
            string txt = (c.Text ?? "").Trim().ToLowerInvariant();
            string name = (c.Name ?? "").Trim().ToLowerInvariant();
            string tag = (c.Tag?.ToString() ?? "").Trim().ToLowerInvariant();

            if (txt == "cancel" || txt == "&cancel") return true;
            if (name.Contains("cancel")) return true;
            if (tag == "cancel") return true;

            return false;
        }

        private bool InvokeClick(Control c)
        {
            try
            {
                // Chuẩn WinForms Button
                if (c is Button btn)
                {
                    btn.PerformClick();
                    return true;
                }

                // DevExpress / custom control cũng hay có PerformClick()
                var performClick = c.GetType().GetMethod(
                    "PerformClick",
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic);

                if (performClick != null)
                {
                    performClick.Invoke(c, null);
                    return true;
                }

                // Thử gọi trực tiếp OnClick(EventArgs)
                var onClick = c.GetType().GetMethod(
                    "OnClick",
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.NonPublic);

                if (onClick != null)
                {
                    onClick.Invoke(c, new object[] { EventArgs.Empty });
                    return true;
                }

                // Cuối cùng: Control.InvokeOnClick(...)
                var invokeOnClick = typeof(Control).GetMethod(
                    "InvokeOnClick",
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.NonPublic);

                if (invokeOnClick != null)
                {
                    invokeOnClick.Invoke(c, new object[] { c, EventArgs.Empty });
                    return true;
                }
            }
            catch
            {
                // nuốt lỗi để không làm đứng app
            }
            return false;
        }


    }
}
