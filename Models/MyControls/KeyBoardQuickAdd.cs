using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace NailsChekin.MyControls
{
    /// <summary>
    /// Quick Add keyboard 4x5:
    /// 1 2 3 +
    /// 4 5 6 -
    /// 7 8 9 *
    /// . 0 % /
    /// <<    ENTER
    /// </summary>
    public sealed class KeyBoardQuickAdd : System.Windows.Forms.UserControl
    {
        public event EventHandler<KeyTappedEventArgs> KeyTapped;
        public event EventHandler ValueChanged;
        public event EventHandler EnterPressed;

        private TableLayoutPanel _grid;

        public string Value { get; private set; } = string.Empty;

        public string DefaultValue { get; set; } = string.Empty;

        public bool AllowDecimal { get; set; } = true;

        public int MaxLength { get; set; } = 0; // 0 = unlimited

        public bool SendKeyOnClick { get; set; } = true;

        public Control TargetControl { get; set; }

        private float _buttonFontSize = 22f;
        public float ButtonFontSize
        {
            get { return _buttonFontSize; }
            set
            {
                if (Math.Abs(_buttonFontSize - value) < 0.1f) return;
                _buttonFontSize = value;
                ApplyStyleToAllButtons();
            }
        }

        // Khoảng cách button: left, top, right, bottom
        // Top/bottom tăng lên để các button không bị sát chiều dọc.
        public Padding ButtonMargin { get; set; } = new Padding(4, 5, 4, 5);

        public int ButtonCornerRadius { get; set; } = 0;

        // Nền tổng thể keyboard/panel
        public Color KeyboardBackColor { get; set; } = Color.FromArgb(229, 241, 249);

        // Button thường nền trắng
        public Color NormalButtonColor { get; set; } = Color.White;

        public Color NormalBorderColor { get; set; } = Color.FromArgb(215, 225, 232);

        public Color NormalTextColor { get; set; } = Color.FromArgb(15, 15, 15);

        public Color EnterButtonColor { get; set; } = Color.FromArgb(230, 235, 0);

        public Color EnterTextColor { get; set; } = Color.Black;

        public Color BackButtonColor { get; set; } = Color.White;

        public Color BackTextColor { get; set; } = Color.FromArgb(15, 15, 15);

        public KeyBoardQuickAdd()
        {
            DoubleBuffered = true;
            BackColor = KeyboardBackColor;
            MinimumSize = new Size(220, 230);

            BuildUI();
        }

        private void BuildUI()
        {
            Controls.Clear();

            _grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = KeyboardBackColor,
                Padding = new Padding(0),
                ColumnCount = 4,
                RowCount = 5,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                GrowStyle = TableLayoutPanelGrowStyle.FixedSize
            };

            _grid.ColumnStyles.Clear();
            _grid.RowStyles.Clear();

            for (int c = 0; c < 4; c++)
            {
                _grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            }

            for (int r = 0; r < 5; r++)
            {
                _grid.RowStyles.Add(new RowStyle(SizeType.Percent, 20f));
            }

            Controls.Add(_grid);

            AddKey("1", 0, 0);
            AddKey("2", 1, 0);
            AddKey("3", 2, 0);
            AddKey("+", 3, 0);

            AddKey("4", 0, 1);
            AddKey("5", 1, 1);
            AddKey("6", 2, 1);
            AddKey("-", 3, 1);

            AddKey("7", 0, 2);
            AddKey("8", 1, 2);
            AddKey("9", 2, 2);
            AddKey("*", 3, 2);

            AddKey(".", 0, 3);
            AddKey("0", 1, 3);
            AddKey("%", 2, 3);
            AddKey("/", 3, 3);

            var backBtn = CreateKey("<<", KeyButtonType.Back);
            _grid.Controls.Add(backBtn, 0, 4);
            _grid.SetColumnSpan(backBtn, 2);

            var enterBtn = CreateKey("ENTER", KeyButtonType.Enter);
            _grid.Controls.Add(enterBtn, 2, 4);
            _grid.SetColumnSpan(enterBtn, 2);
        }

        private void AddKey(string text, int col, int row)
        {
            _grid.Controls.Add(CreateKey(text, KeyButtonType.Normal), col, row);
        }

        private ButtonRound CreateKey(string text, KeyButtonType type)
        {
            Color backColor;
            Color borderColor;
            Color textColor;
            FontStyle fontStyle;

            if (type == KeyButtonType.Enter)
            {
                backColor = EnterButtonColor;
                borderColor = EnterButtonColor;
                textColor = EnterTextColor;
                fontStyle = FontStyle.Bold;
            }
            else if (type == KeyButtonType.Back)
            {
                backColor = BackButtonColor;
                borderColor = NormalBorderColor;
                textColor = BackTextColor;
                fontStyle = FontStyle.Bold;
            }
            else
            {
                backColor = NormalButtonColor;
                borderColor = NormalBorderColor;
                textColor = NormalTextColor;
                fontStyle = FontStyle.Bold;
            }

            var btn = new ButtonRound
            {
                Text = text,
                Title = text,

                Dock = DockStyle.Fill,
                Margin = ButtonMargin,

                CornerRadius = ButtonCornerRadius,
                TitleBackColor = backColor,
                BorderColor = borderColor,

                ForeColor = textColor,
                Font = new Font("Segoe UI", ButtonFontSize, fontStyle),
                TabStop = false
            };

            ApplyButtonRoundTextColor(btn, textColor);

            btn.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    var key = ((ButtonRound)s).Text;
                    HandleKey(key);
                }
            };

            return btn;
        }

        private void HandleKey(string key)
        {
            if (string.IsNullOrEmpty(key)) return;

            if (key == "ENTER")
            {
                EnterPressed?.Invoke(this, EventArgs.Empty);

                if (SendKeyOnClick)
                {
                    KeyTapped?.Invoke(this, new KeyTappedEventArgs
                    {
                        Key = key,
                        Value = Value
                    });
                }

                return;
            }

            string newVal = Value;

            if ((key == "<<" || key == ".") &&
                !string.IsNullOrEmpty(DefaultValue) &&
                string.IsNullOrEmpty(Value))
            {
                newVal = DefaultValue;
            }

            if (key == "<<")
            {
                if (newVal.Length > 0)
                    newVal = newVal.Substring(0, newVal.Length - 1);
            }
            else if (key == ".")
            {
                if (AllowDecimal && !newVal.Contains("."))
                    newVal += ".";
            }
            else if (IsNumberKey(key))
            {
                if (MaxLength <= 0 || newVal.Length < MaxLength)
                    newVal += key;
            }
            else if (IsOperatorKey(key))
            {
                // + - * / %
                // Không cộng trực tiếp vào Value.
                // Form ngoài xử lý qua KeyTapped.
            }

            if (newVal != Value)
            {
                Value = newVal;
                PushToTarget();
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }

            if (SendKeyOnClick)
            {
                KeyTapped?.Invoke(this, new KeyTappedEventArgs
                {
                    Key = key,
                    Value = Value
                });
            }
        }

        private bool IsNumberKey(string key)
        {
            return key.Length == 1 && key[0] >= '0' && key[0] <= '9';
        }

        private bool IsOperatorKey(string key)
        {
            return key == "+" || key == "-" || key == "*" || key == "/" || key == "%";
        }

        private void PushToTarget()
        {
            if (TargetControl == null) return;
            TargetControl.Text = Value;
        }

        public void Clear()
        {
            if (Value.Length == 0) return;

            Value = string.Empty;
            PushToTarget();
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Backspace()
        {
            if (Value.Length == 0) return;

            Value = Value.Substring(0, Value.Length - 1);
            PushToTarget();
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetValue(string text)
        {
            Value = text ?? string.Empty;
            PushToTarget();
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetButtonText(string key, string newText)
        {
            if (_grid == null) return;

            foreach (Control c in _grid.Controls)
            {
                var btn = c as ButtonRound;
                if (btn == null) continue;

                if (string.Equals(btn.Text, key, StringComparison.Ordinal))
                {
                    btn.Text = newText;
                    btn.Title = newText;
                    btn.Invalidate();
                    return;
                }
            }
        }

        /// <summary>
        /// Set nền keyboard giống form/panel bên ngoài.
        /// Button số vẫn giữ nền trắng.
        /// </summary>
        public void SetKeyboardBackColor(Color color)
        {
            KeyboardBackColor = color;

            BackColor = color;

            if (_grid != null)
                _grid.BackColor = color;

            ApplyStyleToAllButtons();
            Invalidate();
        }

        /// <summary>
        /// Set khoảng cách ngang/dọc giữa các button.
        /// Ví dụ: SetButtonSpacing(4, 5)
        /// </summary>
        public void SetButtonSpacing(int horizontal, int vertical)
        {
            ButtonMargin = new Padding(horizontal, vertical, horizontal, vertical);

            if (_grid == null) return;

            foreach (Control c in _grid.Controls)
            {
                c.Margin = ButtonMargin;
            }

            _grid.PerformLayout();
            Invalidate();
        }

        private void ApplyStyleToAllButtons()
        {
            if (_grid == null) return;

            foreach (Control c in _grid.Controls)
            {
                var btn = c as ButtonRound;
                if (btn == null) continue;

                bool isEnter = btn.Text == "ENTER";
                bool isBack = btn.Text == "<<";

                Color backColor = isEnter
                    ? EnterButtonColor
                    : isBack
                        ? BackButtonColor
                        : NormalButtonColor;

                Color borderColor = isEnter
                    ? EnterButtonColor
                    : NormalBorderColor;

                Color textColor = isEnter
                    ? EnterTextColor
                    : isBack
                        ? BackTextColor
                        : NormalTextColor;

                FontStyle fontStyle = FontStyle.Bold;

                btn.Margin = ButtonMargin;
                btn.TitleBackColor = backColor;
                btn.BorderColor = borderColor;
                btn.ForeColor = textColor;
                btn.Title = btn.Text;
                btn.Font = new Font("Segoe UI", ButtonFontSize, fontStyle);

                ApplyButtonRoundTextColor(btn, textColor);

                btn.Invalidate();
            }

            _grid.PerformLayout();
        }

        /// <summary>
        /// ButtonRound custom mỗi project có thể dùng tên property màu chữ khác nhau.
        /// Set nhiều property phổ biến để tránh lỗi chữ không hiện.
        /// </summary>
        private void ApplyButtonRoundTextColor(ButtonRound btn, Color color)
        {
            if (btn == null) return;

            btn.ForeColor = color;

            SetColorPropertyIfExists(btn, "TitleColor", color);
            SetColorPropertyIfExists(btn, "TitleForeColor", color);
            SetColorPropertyIfExists(btn, "ButtonTextColor", color);
            SetColorPropertyIfExists(btn, "TextColor", color);
            SetColorPropertyIfExists(btn, "TextForeColor", color);
            SetColorPropertyIfExists(btn, "TitleTextColor", color);
            SetColorPropertyIfExists(btn, "TitleFontColor", color);
            SetColorPropertyIfExists(btn, "FontColor", color);

            btn.Invalidate();
        }

        private void SetColorPropertyIfExists(object obj, string propertyName, Color value)
        {
            if (obj == null) return;

            var prop = obj.GetType().GetProperty(
                propertyName,
                BindingFlags.Public | BindingFlags.Instance
            );

            if (prop == null) return;
            if (!prop.CanWrite) return;
            if (prop.PropertyType != typeof(Color)) return;

            prop.SetValue(obj, value, null);
        }

        private enum KeyButtonType
        {
            Normal,
            Back,
            Enter
        }
    }
}