using NailsChekin.Models.Helper;
using NailsChekin.UserControl;
using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NailsChekin.Popup
{
    public partial class FormItemLockup : Form
    {
        Control parent;
        public string redirect; 

        public FormItemLockup()
        {
            InitializeComponent();
        }

        public FormItemLockup(Control parent, string redirect, string default_search = "")
        {
            InitializeComponent();
            float scale = LayoutHelper.mini_screen ? 1.3f : 1.8f;
            ScaleAllControls(this, scale);
            // Single-line TextBox ignores Height — only font drives it, so scale font explicitly
            txtSearchKey.Font = new Font(txtSearchKey.Font.FontFamily, txtSearchKey.Font.Size * scale, txtSearchKey.Font.Style);

            this.parent = parent;
            this.redirect = redirect;

            //BAR CODE Tạo Timer để reset buffer nếu ngừng quét quá lâu
            timerBarcodeReset = new System.Windows.Forms.Timer();
            timerBarcodeReset.Interval = 200; // 200ms: nếu ngừng nhập quá 200ms thì coi như kết thúc quét
            timerBarcodeReset.Tick += TimerBarcodeReset_Tick;

            this.KeyPreview = true;
            this.KeyPress += MainForm_KeyPress;

            ApplyKeyboardTheme();

            foreach (Button button in this.Controls.OfType<Button>())
            {
                button.Click += Button_Click;
            }

            if (this.redirect.Equals("ItemLockUp"))
            {
                lbTitle.Text = "PRODUCT SEARCH";
            }
            else if (this.redirect.Equals("CustomerLockUp"))
            {
                lbTitle.Text = "CUSTOMER SEARCH";
            }
            else if (this.redirect.Equals("InventoryLookUp"))
            {
                lbTitle.Text = "PRODUCT INVENTORY SEARCH";
            }
            else if (this.redirect.Equals("ItemChangeName"))
            {
                lbTitle.Text = "ITEM NAME";
            }

            if (!string.IsNullOrEmpty(default_search))
            {
                txtSearchKey.Text = default_search;
                typingBuffer.Append(default_search);
            }

        }

        private void ApplyKeyboardTheme()
        {
            // Form background: blue-400 — matches FormKeyboardOnlyNumber
            this.BackColor = Color.FromArgb(96, 165, 250);

            // Palette (mirrors KeyBoardSearch)
            var numBack    = Color.White;
            var numBorder  = Color.FromArgb(226, 232, 240);  // #E2E8F0
            var numFore    = Color.FromArgb(30,  41,  59);   // #1E293B
            var modBack    = Color.FromArgb(239, 246, 255);  // #EFF6FF – modifier keys (Tab/Shift/Ctrl/Alt)
            var modBorder  = Color.FromArgb(191, 219, 254);  // #BFDBFE
            var modFore    = Color.FromArgb(29,  78,  216);  // #1D4ED8
            var bsBack     = Color.FromArgb(255, 247, 237);  // #FFF7ED – backspace
            var bsBorder   = Color.FromArgb(254, 215, 170);  // #FED7AA
            var bsFore     = Color.FromArgb(194,  65,  12);  // #C2410C
            var enterBack  = Color.FromArgb(22,  163,  74);  // #16A34A – enter
            var enterHover = Color.FromArgb(21,  128,  61);
            var hoverColor = Color.FromArgb(59,  130, 246);  // blue-500 for regular key hover

            // Scale font to match button size (ScaleAllControls scales size but not font)
            float fontScale = NailsChekin.Models.Helper.LayoutHelper.mini_screen ? 1.3f : 1.8f;

            foreach (Button btn in this.Controls.OfType<Button>())
            {
                btn.Text = btn.Text.TrimEnd();

                // Fix missing TextAlign on multi-line keys (e.g. button6 "^/6" was MiddleCenter
                // while all other top-row keys are TopLeft — caused "6" to appear too low)
                if (btn.Text.Contains('\n'))
                    btn.TextAlign = ContentAlignment.TopLeft;

                btn.Font = new Font(btn.Font.FontFamily, btn.Font.Size * fontScale, btn.Font.Style);

                string txt  = (btn.Text ?? "").Trim().ToUpperInvariant();
                string name = (btn.Name ?? "").ToUpperInvariant();

                if (txt == "ENTER" || name.Contains("ENTER"))
                {
                    btn.BackColor = enterBack; btn.ForeColor = Color.White;
                    btn.FlatAppearance.MouseOverBackColor = enterHover;
                    btn.FlatAppearance.MouseDownBackColor = enterHover;
                    btn.FlatAppearance.BorderColor = enterBack;
                }
                else if (txt.Contains("BACK") || txt.Contains("SPACE") || name.Contains("BACK"))
                {
                    btn.BackColor = bsBack; btn.ForeColor = bsFore;
                    btn.FlatAppearance.MouseOverBackColor = bsBorder;
                    btn.FlatAppearance.MouseDownBackColor = bsBorder;
                    btn.FlatAppearance.BorderColor = bsBorder;
                }
                else
                {
                    btn.BackColor = numBack; btn.ForeColor = numFore;
                    btn.FlatAppearance.MouseOverBackColor = hoverColor;
                    btn.FlatAppearance.MouseDownBackColor = hoverColor;
                    btn.FlatAppearance.BorderColor = numBorder;
                }
                btn.FlatAppearance.BorderSize = 1;
            }

            // CheckBoxes: chkEnter → green, rest → modifier blue (Tab/Shift/Ctrl/Alt/NumLock)
            foreach (CheckBox cb in this.Controls.OfType<CheckBox>())
            {
                cb.Font = new Font(cb.Font.FontFamily, cb.Font.Size * fontScale, cb.Font.Style);

                if ((cb.Name ?? "").ToUpperInvariant() == "CHKENTER")
                {
                    cb.BackColor = enterBack; cb.ForeColor = Color.White;
                    cb.FlatAppearance.MouseOverBackColor = enterHover;
                    cb.FlatAppearance.MouseDownBackColor = enterHover;
                    cb.FlatAppearance.BorderColor = enterBack;
                }
                else
                {
                    cb.BackColor = modBack; cb.ForeColor = modFore;
                    cb.FlatAppearance.MouseOverBackColor = hoverColor;
                    cb.FlatAppearance.MouseDownBackColor = hoverColor;
                    cb.FlatAppearance.BorderColor = modBorder;
                }
                cb.FlatAppearance.BorderSize = 1;
            }

            // Title label: dark text on blue background
            lbTitle.ForeColor = Color.FromArgb(12, 74, 110);  // sky-950
            lbTitle.BackColor = Color.Transparent;

            // Search textbox
            txtSearchKey.BackColor = Color.White;
            txtSearchKey.ForeColor = Color.FromArgb(30, 41, 59);
        }

        private void FormItemLockup_Load(object sender, EventArgs e)
        {
            // Override StartPosition set by caller — must be Manual so our Location sticks
            this.StartPosition = FormStartPosition.Manual;
            PositionBottomFull();
        }

        private void PositionBottomFull()
        {
            var screen = Screen.FromControl(this.Owner ?? this);
            var workArea = screen.Bounds; // full screen (not WorkingArea) to reach absolute bottom
            int targetWidth = workArea.Width;

            // Stretch all controls' X/Width proportionally to fill screen width
            if (this.Width > 0 && targetWidth != this.Width)
            {
                float ratio = (float)targetWidth / this.Width;
                StretchHorizontalRecursive(this, ratio);
                // Single-line TextBox ignores Width-driven height — scale font too
                txtSearchKey.Font = new Font(txtSearchKey.Font.FontFamily, txtSearchKey.Font.Size * ratio, txtSearchKey.Font.Style);
                this.ClientSize = new Size(targetWidth, this.ClientSize.Height);
            }

            // Align left edge to screen and bottom edge to work area bottom
            this.Left = workArea.Left;
            this.Top  = workArea.Bottom - this.Height;
        }

        private static void StretchHorizontalRecursive(Control parent, float ratio)
        {
            foreach (Control ctrl in parent.Controls)
            {
                ctrl.Left  = (int)Math.Round(ctrl.Left  * ratio);
                ctrl.Width = (int)Math.Round(ctrl.Width * ratio);
                if (ctrl.Controls.Count > 0)
                    StretchHorizontalRecursive(ctrl, ratio);
            }
        }

        private void chkEnter_Click(object sender, EventArgs e)
        {
            scanBuffer.Clear();
            typingBuffer.Clear();
            this.SearchNow();
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            string key = Convert.ToString(btn.Tag);
            if (string.IsNullOrWhiteSpace(key))
                key = GetButtonInputText(btn);

            if (string.IsNullOrWhiteSpace(key))
                return;

            string keyUpper = key.ToUpperInvariant();

            if (keyUpper == "BACKSPACE")
            {
                RemoveLastTypingChar();
                return;
            }

            if (keyUpper == "ENTER")
            {
                ExecuteTypingSearch();
                return;
            }

            if (keyUpper == "SPACE")
            {
                AppendTypingText(" ");
                return;
            }

            if (keyUpper == "TAB" ||
                keyUpper == "CAPSLOCK" ||
                keyUpper == "SHIFT" ||
                keyUpper == "CTRL" ||
                keyUpper == "ALT" ||
                keyUpper == "NUMLOCK")
            {
                return;
            }

            AppendTypingText(key);
        }

        private void SyncTypingBufferToTextBox()
        {
            txtSearchKey.Text = typingBuffer.ToString();
            txtSearchKey.SelectionStart = txtSearchKey.Text.Length;
            txtSearchKey.SelectionLength = 0;
            txtSearchKey.Focus();
        }

        private void AppendTypingText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            typingBuffer.Append(text);
            SyncTypingBufferToTextBox();
        }

        private void RemoveLastTypingChar()
        {
            if (typingBuffer.Length <= 0)
                return;

            typingBuffer.Remove(typingBuffer.Length - 1, 1);
            SyncTypingBufferToTextBox();
        }

        private void ExecuteTypingSearch()
        {
            string keyword = typingBuffer.ToString().Trim();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                scanBuffer.Clear();
                typingBuffer.Clear();
                SearchNow();
            }
        }

        private string GetButtonInputText(Button btn)
        {
            if (btn == null)
                return "";

            string raw = (btn.Text ?? "").Replace("\r", "");

            // 1) nhận diện riêng phím Space
            // nhiều bàn phím ảo có nút space chỉ toàn khoảng trắng hoặc không có text rõ ràng
            if (string.IsNullOrEmpty(raw) || raw.Trim().Length == 0)
            {
                string btnName = (btn.Name ?? "").ToUpperInvariant();

                if (btnName.Contains("SPACE"))
                    return " ";

                return "";
            }

            string upperRaw = raw.Trim().ToUpperInvariant();
            string btnNameUpper = (btn.Name ?? "").ToUpperInvariant();

            // 2) các phím chức năng phổ biến
            if (upperRaw == "SPACE" || btnNameUpper.Contains("SPACE"))
                return " ";

            if (upperRaw == "ENTER" || btnNameUpper.Contains("ENTER"))
                return "ENTER";

            if (upperRaw == "BACKSPACE" || upperRaw == "BACK SPACE" || btnNameUpper.Contains("BACK"))
                return "BACKSPACE";

            if (upperRaw == "TAB" || btnNameUpper.Contains("TAB"))
                return "TAB";

            if (upperRaw == "CAPS LOCK" || btnNameUpper.Contains("CAPS"))
                return "CAPSLOCK";

            if (upperRaw == "SHIFT" || btnNameUpper.Contains("SHIFT"))
                return "SHIFT";

            if (upperRaw == "CTRL" || btnNameUpper.Contains("CTRL"))
                return "CTRL";

            if (upperRaw == "ALT" || btnNameUpper.Contains("ALT"))
                return "ALT";

            if (upperRaw == "NUM LOCK" || btnNameUpper.Contains("NUMLOCK"))
                return "NUMLOCK";

            string[] parts = raw.Split('\n');

            // 3) ưu tiên dòng cuối không rỗng
            for (int i = parts.Length - 1; i >= 0; i--)
            {
                string line = parts[i].Trim();
                if (!string.IsNullOrEmpty(line))
                    return line;
            }

            return "";
        }

        #region Process Barcode

        private System.Windows.Forms.Timer timerBarcodeReset;
        private readonly StringBuilder typingBuffer = new StringBuilder();
        private readonly StringBuilder scanBuffer = new StringBuilder();

        private DateTime scanFirstCharTime = DateTime.MinValue;
        private DateTime scanLastCharTime = DateTime.MinValue;

        private const int ScanIdleTimeoutMs = 80;     // scanner ngưng 80ms coi như kết thúc
        private const int MaxScanCharGapMs = 30;      // tốc độ rất nhanh mới coi là scan
        private const int MinScanLength = 6;          // barcode tối thiểu
        private bool _enterHandledByCommandKey = false;

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!CanHandleKeyboard())
                return;

            // Enter/Backspace đã xử lý ở ProcessCmdKey
            if (_enterHandledByCommandKey)
            {
                _enterHandledByCommandKey = false;
                e.Handled = true;
                return;
            }

            if (char.IsControl(e.KeyChar))
                return;

            char ch = e.KeyChar;

            if (scanBuffer.Length == 0)
                scanFirstCharTime = DateTime.Now;

            scanBuffer.Append(ch);
            scanLastCharTime = DateTime.Now;

            timerBarcodeReset.Stop();
            timerBarcodeReset.Interval = ScanIdleTimeoutMs;
            timerBarcodeReset.Start();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!CanHandleKeyboard())
                return base.ProcessCmdKey(ref msg, keyData);

            if (keyData == Keys.Enter)
            {
                _enterHandledByCommandKey = true;

                string value = scanBuffer.ToString().Trim();
                scanBuffer.Clear();

                string valueTyping = typingBuffer.ToString().Trim();
                typingBuffer.Clear();

                if (!string.IsNullOrWhiteSpace(value) || !string.IsNullOrWhiteSpace(valueTyping))
                {
                    txtSearchKey.Text = string.IsNullOrEmpty(value) ? valueTyping : value;
                    this.SearchNow();
                }

                return true;
            }

            if (keyData == Keys.Space)
            {
                if (scanBuffer.Length == 0)
                    scanFirstCharTime = DateTime.Now;

                scanBuffer.Append(' ');
                scanLastCharTime = DateTime.Now;

                typingBuffer.Append(' ');
                txtSearchKey.Text = typingBuffer.ToString();
                txtSearchKey.SelectionStart = txtSearchKey.Text.Length;

                timerBarcodeReset.Stop();
                timerBarcodeReset.Interval = ScanIdleTimeoutMs;
                timerBarcodeReset.Start();

                return true;
            }

            if (keyData == Keys.Back)
            {
                ResetScanBuffer();

                if (typingBuffer.Length > 0)
                {
                    typingBuffer.Remove(typingBuffer.Length - 1, 1);
                    txtSearchKey.Text = typingBuffer.ToString();
                    txtSearchKey.SelectionStart = txtSearchKey.Text.Length;
                }

                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ResetScanBuffer()
        {
            scanBuffer.Clear();
            scanFirstCharTime = DateTime.MinValue;
            scanLastCharTime = DateTime.MinValue;
        }

        private bool IsLikelyScan(string text, DateTime firstTime, DateTime lastTime)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            text = text.Trim();

            if (text.Length < MinScanLength)
                return false;

            if (firstTime == DateTime.MinValue || lastTime == DateTime.MinValue)
                return false;

            double totalMs = (lastTime - firstTime).TotalMilliseconds;
            if (totalMs < 0)
                return false;

            if (text.Length <= 1)
                return false;

            double avgGap = totalMs / (text.Length - 1);

            return avgGap <= MaxScanCharGapMs;
        }

        private void TimerBarcodeReset_Tick(object sender, EventArgs e)
        {
            timerBarcodeReset.Stop();

            if (!CanHandleKeyboard())
            {
                ResetScanBuffer();
                return;
            }

            if (scanBuffer.Length == 0)
                return;

            if ((DateTime.Now - scanLastCharTime).TotalMilliseconds < ScanIdleTimeoutMs)
            {
                timerBarcodeReset.Start();
                return;
            }

            string inputText = scanBuffer.ToString().Trim();
            bool isScan = IsLikelyScan(inputText, scanFirstCharTime, scanLastCharTime);

            if (isScan)
            {
                this.SearchNow();
                typingBuffer.Clear();
                txtSearchKey.Clear();
            }
            else
            {
                typingBuffer.Append(inputText);
                txtSearchKey.Text = typingBuffer.ToString();
                txtSearchKey.SelectionStart = txtSearchKey.Text.Length;
            }

            ResetScanBuffer();
        }

        private bool CanHandleKeyboard()
        {
            return this.Visible
                   && !this.IsDisposed
                   && this.IsHandleCreated
                   && this.ContainsFocus;
        }

        #endregion Process Barcode

        private void svgClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void SearchNow()
        {
            string searchText   = txtSearchKey.Text.Trim();
            string redirectCopy = this.redirect;
            Control parentCopy  = this.parent;

            // Đóng FormItemLockup TRƯỚC — tránh bị chồng form khi popup mới ShowDialog
            this.Close();

            // BeginInvoke: post action vào message queue SAU khi form đã fully closed
            // Đảm bảo FormItemLockup biến mất hoàn toàn trước khi popup mới xuất hiện
            parentCopy.BeginInvoke(new Action(() =>
            {
                switch (redirectCopy)
                {
                    case "ItemLockUp":
                        ((FormMain)parentCopy).SendSearchItemLockUp(searchText);
                        break;
                    case "CustomerLockUp":
                        ((FormMain)parentCopy).SendSearchCustomerLockUp(searchText);
                        break;
                    case "InventoryLookUp":
                        ((FormMain)parentCopy).ShowInventoryForm(searchText);
                        break;
                    case "ItemChangeName":
                        ((UCCartItem)parentCopy).UpdateItemName(searchText);
                        break;
                }
            }));
        }

        private void FormItemLockup_FormClosing(object sender, FormClosingEventArgs e)
        {
            try { timerBarcodeReset?.Stop(); timerBarcodeReset?.Dispose(); } catch { }
        }

        private void FormItemLockup_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Xử lý không bị cảm giác giật do xài Dispose() ngay nếu đóng thẳng
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000); // 3 giây
                try
                {
                    if (!this.IsDisposed)
                    {
                        this.Invoke((Action)(() => this.Dispose()));
                    }
                }
                catch { /* form đã dispose rồi thì thôi */ }
            });
        }

        /// <summary>
        /// Scale toàn bộ form (ClientSize) và tất cả controls con (Location + Size) theo hệ số.
        /// Gọi sau InitializeComponent() để tránh sửa tay ~80 controls trong Designer.
        /// </summary>
        private static void ScaleAllControls(Form form, float factor)
        {
            form.ClientSize = new Size(
                (int)(form.ClientSize.Width  * factor),
                (int)(form.ClientSize.Height * factor)
            );

            foreach (Control ctrl in form.Controls)
                ScaleControlRecursive(ctrl, factor);
        }

        private static void ScaleControlRecursive(Control ctrl, float factor)
        {
            ctrl.Location = new Point(
                (int)(ctrl.Location.X * factor),
                (int)(ctrl.Location.Y * factor)
            );
            ctrl.Size = new Size(
                (int)(ctrl.Size.Width  * factor),
                (int)(ctrl.Size.Height * factor)
            );

            foreach (Control child in ctrl.Controls)
                ScaleControlRecursive(child, factor);
        }

    }

}
