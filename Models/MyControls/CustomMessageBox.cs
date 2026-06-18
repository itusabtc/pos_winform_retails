using System.Drawing;
using System.Windows.Forms;

namespace NailsChekin.MyControls
{
    public static class CustomMessageBox
    {
        public static DialogResult Show(string message, string title = "Error", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.Error)
        {
            using (Form form = new Form())
            using (Label label = new Label())
            using (PictureBox pictureBox = new PictureBox())
            {
                // Form
                form.Text = title;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.ClientSize = new Size(600, 200); // sẽ tăng tự động nếu text dài

                // Icon
                pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox.Location = new Point(24, 32);
                pictureBox.Size = new Size(56, 56);

                switch (icon)
                {
                    case MessageBoxIcon.Error:
                        pictureBox.Image = SystemIcons.Error.ToBitmap();
                        break;
                    case MessageBoxIcon.Warning:
                        pictureBox.Image = SystemIcons.Warning.ToBitmap();
                        break;
                    case MessageBoxIcon.Information:
                        pictureBox.Image = SystemIcons.Information.ToBitmap();
                        break;
                    case MessageBoxIcon.Question:
                        pictureBox.Image = SystemIcons.Question.ToBitmap();
                        break;
                }

                // Label
                label.Text = message;
                label.Font = new Font("Segoe UI", 32, FontStyle.Regular); // to gấp 2 lần (16 -> 32) cho dễ đọc
                label.AutoSize = true;
                label.MaximumSize = new Size(820, 0); // wrap text nếu dài
                label.Location = new Point(95, 28);

                form.Controls.Add(label);
                form.Controls.Add(pictureBox);

                // --- Căn giữa với icon nếu chỉ 1 dòng ---
                form.Shown += (s, e) =>
                {
                    // nếu text thấp (chỉ 1 dòng) thì căn giữa icon
                    if (label.Height <= label.Font.Height * 2)
                    {
                        int iconMid = pictureBox.Top + pictureBox.Height / 2;
                        int textMid = label.Top + label.Height / 2;
                        int offset = iconMid - textMid;
                        label.Top += offset; // dịch label xuống/ lên
                    }
                };

                // Sau khi add label: tính kích thước form theo nội dung, CHỪA khoảng cách rõ ràng với hàng nút
                int contentWidth = System.Math.Max(680, label.Location.X + label.Width + 40);
                int contentHeight = label.Location.Y + label.Height + 55 /*gap nội dung-nút*/ + 70 /*chiều cao nút*/ + 25 /*đáy*/;
                if (contentHeight < 240) contentHeight = 240;
                form.ClientSize = new Size(contentWidth, contentHeight);

                // Buttons — bố trí động, căn giữa hàng nút và tự nới rộng form nếu cần
                // (trước đây YesNoCancel dùng 3 nút 200px nên tràn ra ngoài form 600px)
                string[] btnTexts;
                DialogResult[] btnResults;
                switch (buttons)
                {
                    case MessageBoxButtons.OKCancel:
                        btnTexts = new[] { "OK", "Cancel" };
                        btnResults = new[] { DialogResult.OK, DialogResult.Cancel };
                        break;
                    case MessageBoxButtons.YesNo:
                        btnTexts = new[] { "Yes", "No" };
                        btnResults = new[] { DialogResult.Yes, DialogResult.No };
                        break;
                    case MessageBoxButtons.YesNoCancel:
                        btnTexts = new[] { "Yes", "No", "Cancel" };
                        btnResults = new[] { DialogResult.Yes, DialogResult.No, DialogResult.Cancel };
                        break;
                    default: // OK
                        btnTexts = new[] { "OK" };
                        btnResults = new[] { DialogResult.OK };
                        break;
                }

                int btnHeight = 70, btnGap = 24;
                int btnWidth = btnTexts.Length >= 3 ? 190 : 220;
                int totalBtnWidth = btnTexts.Length * btnWidth + (btnTexts.Length - 1) * btnGap;

                // Nới rộng form nếu hàng nút rộng hơn vùng client hiện tại
                int neededWidth = totalBtnWidth + 40;
                if (form.ClientSize.Width < neededWidth)
                    form.ClientSize = new Size(neededWidth, form.ClientSize.Height);

                int bottomY = form.ClientSize.Height - btnHeight - 25;
                int startX = (form.ClientSize.Width - totalBtnWidth) / 2;

                for (int i = 0; i < btnTexts.Length; i++)
                {
                    Button btn = new Button();
                    btn.Text = btnTexts[i];
                    btn.Font = new Font("Segoe UI", 26, FontStyle.Bold);
                    btn.Size = new Size(btnWidth, btnHeight);
                    btn.Location = new Point(startX + i * (btnWidth + btnGap), bottomY);
                    btn.DialogResult = btnResults[i];
                    form.Controls.Add(btn);

                    if (form.AcceptButton == null) form.AcceptButton = btn;
                    if (btnResults[i] == DialogResult.Cancel || btnResults[i] == DialogResult.No)
                        form.CancelButton = btn;
                }

                return form.ShowDialog();
            }
        }
    }
}
