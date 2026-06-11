using System;
using System.Drawing;
using System.Windows.Forms;

namespace NailsChekin.MyControls
{
    /// <summary>
    /// Numpad keyboard for search/input:
    ///   Row 0: 1  2  3  +
    ///   Row 1: 4  5  6  -
    ///   Row 2: 7  8  9  *
    ///   Row 3: .  0  %  /
    ///   Row 4: <<(×2)   ENTER(×2, yellow)
    /// </summary>
    public sealed class KeyBoardSearch : System.Windows.Forms.UserControl
    {
        // ---- Events ----
        public event EventHandler<KeyTappedEventArgs> KeyTapped;
        public event EventHandler ValueChanged;
        public event EventHandler EnterPressed;

        // ---- State ----
        public string Value      { get; set; } = string.Empty;
        public string DefaultValue { get; set; } = string.Empty;

        // ---- Options ----
        public bool AllowDecimal   { get; set; } = true;
        public int  MaxLength      { get; set; } = 0;       // 0 = unlimited
        public bool SendKeyOnClick { get; set; } = true;
        public Control TargetControl { get; set; }

        // ---- Style ----
        private float _buttonFontSize = 24f;
        public float ButtonFontSize
        {
            get => _buttonFontSize;
            set
            {
                if (Math.Abs(_buttonFontSize - value) < 0.1f) return;
                _buttonFontSize = value;
                ApplyFontToAllButtons();
            }
        }

        public Padding ButtonMargin      { get; set; } = new Padding(8);
        public int     ButtonCornerRadius { get; set; } = 14;

        // Option B – Light clean
        // Number keys: white / light border / dark navy text
        private static readonly Color _numBack   = Color.White;
        private static readonly Color _numBorder = Color.FromArgb(226, 232, 240);  // #E2E8F0
        private static readonly Color _numFore   = Color.FromArgb(30,  41,  59);   // #1E293B

        // Operator keys (+, -, *, /): light blue tint
        private static readonly Color _opBack    = Color.FromArgb(239, 246, 255);  // #EFF6FF
        private static readonly Color _opBorder  = Color.FromArgb(191, 219, 254);  // #BFDBFE
        private static readonly Color _opFore    = Color.FromArgb(29,  78,  216);  // #1D4ED8

        // % and << (backspace): amber / caution
        private static readonly Color _bsBack    = Color.FromArgb(255, 247, 237);  // #FFF7ED
        private static readonly Color _bsBorder  = Color.FromArgb(254, 215, 170);  // #FED7AA
        private static readonly Color _bsFore    = Color.FromArgb(194,  65,  12);  // #C2410C

        // ENTER: green / confirm
        private static readonly Color _enterBack = Color.FromArgb(22,  163,  74);  // #16A34A
        private static readonly Color _enterFore = Color.White;

        private TableLayoutPanel _grid;

        public KeyBoardSearch()
        {
            DoubleBuffered = true;
            BackColor = Color.Transparent;
            Padding   = new Padding(6);
            BuildUI();
        }

        private void BuildUI()
        {
            Controls.Clear();

            _grid = new TableLayoutPanel
            {
                Dock            = DockStyle.Fill,
                BackColor       = Color.Transparent,
                Padding         = new Padding(4),
                ColumnCount     = 4,
                RowCount        = 5,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            for (int c = 0; c < 4; c++)
                _grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));

            // Rows 0-3: equal height so buttons are square (21% × 4 = 84%)
            // Row 4 (<< / ENTER): 76% of normal row height (16%)
            for (int r = 0; r < 4; r++)
                _grid.RowStyles.Add(new RowStyle(SizeType.Percent, 21f));
            _grid.RowStyles.Add(new RowStyle(SizeType.Percent, 16f));

            Controls.Add(_grid);

            string[,] keys = {
                { "1", "2", "3", "+" },
                { "4", "5", "6", "-" },
                { "7", "8", "9", "*" },
                { ".", "0", "%", "/" }
            };

            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                {
                    bool isOperator = c == 3;                        // col 3: +, -, *, /
                    bool isAmber    = r == 3 && c == 2;              // %
                    _grid.Controls.Add(CreateKey(keys[r, c], isOperator, isAmber), c, r);
                }

            // Row 4: backspace (col 0-1) + ENTER (col 2-3)
            var backBtn  = CreateBackspaceKey();
            var enterBtn = CreateEnterKey();

            _grid.Controls.Add(backBtn,  0, 4);
            _grid.SetColumnSpan(backBtn, 2);

            _grid.Controls.Add(enterBtn, 2, 4);
            _grid.SetColumnSpan(enterBtn, 2);
        }

        private ButtonRound CreateKey(string text, bool isOperator = false, bool isAmber = false)
        {
            Color bg     = isAmber ? _bsBack   : isOperator ? _opBack   : _numBack;
            Color border = isAmber ? _bsBorder : isOperator ? _opBorder : _numBorder;
            Color fore   = isAmber ? _bsFore   : isOperator ? _opFore   : _numFore;

            var btn = new ButtonRound
            {
                Text           = text,
                Title          = text,
                Dock           = DockStyle.Fill,
                Margin         = ButtonMargin,
                CornerRadius   = ButtonCornerRadius,
                TitleBackColor = bg,
                BorderColor    = border,
                TitleForeColor = fore,
                Font           = new Font("Segoe UI", _buttonFontSize, FontStyle.Bold)
            };
            btn.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left) HandleKey(text);
            };
            return btn;
        }

        private ButtonRound CreateBackspaceKey()
        {
            var btn = new ButtonRound
            {
                Text           = "<<",
                Title          = "<<",
                Dock           = DockStyle.Fill,
                Margin         = ButtonMargin,
                CornerRadius   = ButtonCornerRadius,
                TitleBackColor = _bsBack,
                BorderColor    = _bsBorder,
                TitleForeColor = _bsFore,
                Font           = new Font("Segoe UI", _buttonFontSize, FontStyle.Bold)
            };
            btn.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left) HandleKey("<<");
            };
            return btn;
        }

        private ButtonRound CreateEnterKey()
        {
            var btn = new ButtonRound
            {
                Text           = "ENTER",
                Title          = "ENTER",
                Dock           = DockStyle.Fill,
                Margin         = ButtonMargin,
                CornerRadius   = ButtonCornerRadius,
                TitleBackColor = _enterBack,
                BorderColor    = _enterBack,
                TitleForeColor = _enterFore,
                Font           = new Font("Segoe UI", _buttonFontSize, FontStyle.Bold)
            };
            btn.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left) HandleEnter();
            };
            return btn;
        }

        // ---- Key logic ----

        private void HandleKey(string key)
        {
            string newVal = Value;

            if (key == "<<")
            {
                if (!string.IsNullOrEmpty(DefaultValue) && string.IsNullOrEmpty(newVal))
                    newVal = DefaultValue;
                if (newVal.Length > 0)
                    newVal = newVal.Substring(0, newVal.Length - 1);
            }
            else if (key == ".")
            {
                if (AllowDecimal && newVal.IndexOf('.') < 0)
                    newVal += ".";
            }
            else
            {
                if (MaxLength <= 0 || newVal.Length < MaxLength)
                    newVal += key;
            }

            if (newVal != Value)
            {
                Value = newVal;
                PushToTarget();
                ValueChanged?.Invoke(this, EventArgs.Empty);
            }

            if (SendKeyOnClick)
                KeyTapped?.Invoke(this, new KeyTappedEventArgs { Key = key, Value = Value });
        }

        private void HandleEnter()
        {
            EnterPressed?.Invoke(this, EventArgs.Empty);
            if (SendKeyOnClick)
                KeyTapped?.Invoke(this, new KeyTappedEventArgs { Key = "ENTER", Value = Value });
        }

        // ---- Public API ----

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

        // ---- Helpers ----

        private void PushToTarget()
        {
            if (TargetControl == null) return;
            TargetControl.Text = Value;
        }

        private void ApplyFontToAllButtons()
        {
            if (_grid == null) return;
            foreach (Control c in _grid.Controls)
            {
                if (c is ButtonRound rb)
                {
                    rb.Font = new Font(rb.Font.FontFamily, _buttonFontSize, rb.Font.Style);
                    rb.Invalidate();
                }
            }
        }
    }
}
