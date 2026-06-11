using NailsChekin.Models.Helper;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace NailsChekin.MyControls
{
    #region KeyBoardTemplateBar (dùng ButtonRound)
    public class KeyTappedEventArgs : EventArgs
    {
        public string Key { get; internal set; }
        public string Value { get; internal set; }
    }

    /// <summary>Mini keypad 3×4: 1..9, "." , "0" , "&lt;&lt;"</summary>
    public sealed class KeyBoardTemplateBar : System.Windows.Forms.UserControl
    {
        // Sự kiện
        public event EventHandler<KeyTappedEventArgs> KeyTapped;
        public event EventHandler ValueChanged;

        // Giá trị hiện tại
        public string Value { get; set; } = string.Empty;

        // Giá trị của mặc định
        public string DefaultValue { get; set; } = string.Empty;

        // Tùy chọn
        public bool AllowDecimal { get; set; } = true;
        public int MaxLength { get; set; } = 0;   // 0 = không giới hạn
        public bool SendKeyOnClick { get; set; } = true;

        // Nếu set, sẽ tự đẩy Value vào text của control này
        public Control TargetControl { get; set; }

        // Style
        private float _buttonFontSize = 24f;
        public float ButtonFontSize
        {
            get { return _buttonFontSize; }
            set
            {
                if (Math.Abs(_buttonFontSize - value) < 0.1f) return;
                _buttonFontSize = value;
                ApplyFontToAllButtons();   // cập nhật ngay các nút đang hiển thị
            }
        }

        public Padding ButtonMargin { get; set; } = new Padding(10);
        public int ButtonCornerRadius { get; set; } = 14;
        public Color ButtonColor { get; set; } = Color.FromArgb(233, 139, 57);
        public Color ButtonTextColor { get; set; } = Color.White;

        private TableLayoutPanel _grid;

        public KeyBoardTemplateBar()
        {
            DoubleBuffered = true;
            BackColor = Color.Transparent;
            BuildUI();
        }

        private void BuildUI()
        {
            Controls.Clear();

            _grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(6),
                ColumnCount = 3,
                RowCount = 4,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };
            for (int c = 0; c < 3; c++) _grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333f));
            for (int r = 0; r < 4; r++) _grid.RowStyles.Add(new RowStyle(SizeType.Percent, 25f));
            Controls.Add(_grid);

            string[,] keys = {
                { "1","2","3" },
                { "4","5","6" },
                { "7","8","9" },
                { ".","0","<<" }
            };

            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 3; c++)
                    _grid.Controls.Add(CreateKey(keys[r, c]), c, r);
        }

        private ButtonRound CreateKey(string text)
        {
            var btn = new ButtonRound
            {
                Text = text,
                Dock = DockStyle.Fill,
                Margin = ButtonMargin,
                CornerRadius = ButtonCornerRadius,
                TitleBackColor = ColorHelper.Warning,
                BorderColor = ColorHelper.Warning,
                Title = text,
                Font = new Font("Segoe UI", ButtonFontSize, FontStyle.Bold)        
            };
            //btn.Click += OnKeyClicked;

            // Ưu tiên xử lý trên MouseDown để không bị trễ do double-click
            btn.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left) HandleKey(((ButtonRound)s).Text);
            };

            // (Tuỳ chọn) vẫn gắn Click để tương thích khi trigger bằng bàn phím/space
            //btn.Click += (s, e) => HandleKey(((ButtonRound)s).Text);

            return btn;
        }

        private void HandleKey(string key)
        {
            string newVal = Value;

            if ((key == "<<" || key == ".") && !string.IsNullOrEmpty(DefaultValue) && string.IsNullOrEmpty(Value))
                newVal = DefaultValue;

            if (key == "<<")
            {
                if (newVal.Length > 0) newVal = newVal.Substring(0, newVal.Length - 1);
            }
            else if (key == ".")
            {
                if (AllowDecimal && newVal.IndexOf('.') < 0) newVal += ".";
            }
            else // 0..9
            {
                if (MaxLength <= 0 || newVal.Length < MaxLength) newVal += key;
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

        private void OnKeyClicked(object sender, EventArgs e)
        {
            var key = ((ButtonRound)sender).Text ?? "";
            string newVal = Value;
            if ( (key == "<<" || key == "." ) && !string.IsNullOrEmpty(DefaultValue) && string.IsNullOrEmpty(Value) )  //Nếu mặc định có value ngoài truyển vào
                newVal = DefaultValue;

            if (key == "<<")
            {
                if (newVal.Length > 0) newVal = newVal.Substring(0, newVal.Length - 1);
            }
            else if (key == ".")
            {
                if (AllowDecimal && newVal.IndexOf('.') < 0)
                    newVal += ".";
            }
            else // 0..9
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

        // API tiện ích
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

        private void PushToTarget()
        {
            if (TargetControl == null) return;
            // Có thể là TextBox, MaskedTextBox, Label, Button… -> đều có .Text
            TargetControl.Text = Value;
        }
        private void ApplyFontToAllButtons()
        {
            if (_grid == null) return;
            foreach (Control c in _grid.Controls)
            {
                var rb = c as ButtonRound;
                if (rb == null) continue;
                var f = rb.Font;
                rb.Font = new Font(f.FontFamily, _buttonFontSize, f.Style);
                rb.Invalidate();
            }
        }
        public bool SetKeyFontSize(string key, float size)
        {
            bool changed = false;
            if (_grid == null) return false;

            foreach (Control c in _grid.Controls)
            {
                var rb = c as ButtonRound;
                if (rb != null && string.Equals(rb.Text, key, StringComparison.Ordinal))
                {
                    var f = rb.Font;
                    rb.Font = new Font(f.FontFamily, size, f.Style);
                    rb.Invalidate();
                    changed = true;
                }
            }
            return changed;
        }

        // Blend nền khi control trong suốt
        //protected override void OnPaintBackground(PaintEventArgs e)
        //{
        //    if (BackColor.A == 0 && Parent != null)
        //    {
        //        var g = e.Graphics; var s = g.Save();
        //        g.TranslateTransform(-Left, -Top);
        //        using (var pe = new PaintEventArgs(g, Parent.ClientRectangle))
        //        {
        //            this.InvokePaintBackground(Parent, pe);
        //            this.InvokePaint(Parent, pe);
        //        }
        //        g.Restore(s);
        //        return;
        //    }
        //    base.OnPaintBackground(e);
        //}
    }
    #endregion
}
