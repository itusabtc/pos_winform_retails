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
                pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
                pictureBox.Location = new Point(20, 30);
                pictureBox.Size = new Size(48, 48);

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
                label.Font = new Font("Segoe UI", 16, FontStyle.Regular);
                label.AutoSize = true;
                label.MaximumSize = new Size(500, 0); // wrap text nếu dài
                label.Location = new Point(80, 20);

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

                // Sau khi add label, tính chiều cao form theo nội dung
                int contentHeight = label.Height + 100;
                if (contentHeight < 200) contentHeight = 200;
                form.ClientSize = new Size(600, contentHeight);

                // Buttons
                int btnWidth = 200, btnHeight = 60;
                int bottomY = form.ClientSize.Height - btnHeight - 20;
                DialogResult result = DialogResult.None;

                void AddButton(string text, DialogResult dr, int offsetX)
                {
                    Button btn = new Button();
                    btn.Text = text;
                    btn.Font = new Font("Segoe UI", 20, FontStyle.Bold);
                    btn.Size = new Size(btnWidth, btnHeight);
                    btn.Location = new Point(offsetX, bottomY);
                    btn.DialogResult = dr;
                    form.Controls.Add(btn);

                    if (form.AcceptButton == null) form.AcceptButton = btn;
                    if (form.CancelButton == null && (dr == DialogResult.Cancel || dr == DialogResult.No))
                        form.CancelButton = btn;
                }

                switch (buttons)
                {
                    case MessageBoxButtons.OK:
                        AddButton("OK", DialogResult.OK, (form.ClientSize.Width - btnWidth) / 2);
                        break;
                    case MessageBoxButtons.OKCancel:
                        AddButton("OK", DialogResult.OK, form.ClientSize.Width / 2 - btnWidth - 10);
                        AddButton("Cancel", DialogResult.Cancel, form.ClientSize.Width / 2 + 10);
                        break;
                    case MessageBoxButtons.YesNo:
                        AddButton("Yes", DialogResult.Yes, form.ClientSize.Width / 2 - btnWidth - 10);
                        AddButton("No", DialogResult.No, form.ClientSize.Width / 2 + 10);
                        break;
                    case MessageBoxButtons.YesNoCancel:
                        AddButton("Yes", DialogResult.Yes, form.ClientSize.Width / 2 - btnWidth - 120);
                        AddButton("No", DialogResult.No, form.ClientSize.Width / 2 - btnWidth / 2);
                        AddButton("Cancel", DialogResult.Cancel, form.ClientSize.Width / 2 + btnWidth + 20);
                        break;
                }

                // Show dialog
                result = form.ShowDialog();
                return result;
            }
        }
    }
}
