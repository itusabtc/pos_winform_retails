using NailsChekin.Models;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace NailsChekin.Popup
{
    public partial class FormIntro : Form
    {
        // PIN
        private string _pin = "";
        private const int PIN_LENGTH = 4;
        private Panel _pinBoxesPanel;
        private Label _lblPinTitle;
        private bool  _pinError = false;
        private string _correctPin = "1234";
        private static readonly Color _red = Color.FromArgb(222, 53, 11);

        // Clock
        private Label _lblDay, _lblDate;
        private Panel _timeRow;
        private System.Windows.Forms.Timer _clockTimer;

        // Background image (cache bản scale để không resize 4K mỗi lần paint)
        private Image  _bgImage;
        private Bitmap _bgScaled;

        // Colors
        private static readonly Color _blue      = Color.FromArgb(26, 115, 232);
        private static readonly Color _blueDark  = Color.FromArgb(13, 71, 161);
        private static readonly Color _bgTop     = Color.FromArgb(205, 226, 248);
        private static readonly Color _bgBottom  = Color.FromArgb(235, 245, 255);
        private static readonly Color _textDark  = Color.FromArgb(23, 43, 77);
        private static readonly Color _textMid   = Color.FromArgb(107, 119, 140);
        private static readonly Color _green     = Color.FromArgb(54, 179, 126);
        private static readonly Color _teal      = Color.FromArgb(0, 184, 217);
        private static readonly Color _cardBg    = Color.White;
        private static readonly Color _padBtnBg  = Color.FromArgb(244, 247, 252);
        private static readonly Color _padBorder = Color.FromArgb(223, 230, 240);

        public event Action<string> LoginSuccess;

        public string CorrectPin
        {
            get => _correctPin;
            set => _correctPin = value;
        }

        // Đọc local config tương tự FormSetting.LoadLocalConfig
        private void LoadLocalConfig()
        {
            try
            {
                Constants.chkPincodeOn  = Utilitys.GetConfig("chkPincodeOn",  false);
                Constants.chkPincodeOff = Utilitys.GetConfig("chkPincodeOff", true);

                // PIN tạm thời cố định "1234" — sau này đọc từ config key "pincode" tại đây
            }
            catch { }
        }

        // Quyết định form khởi động: PIN bật → FormIntro, tắt → FormMain luôn
        public static Form CreateStartupForm()
        {
            bool pincodeOn = false;
            try { pincodeOn = Utilitys.GetConfig("chkPincodeOn", false); } catch { }

            if (pincodeOn)
                return new FormIntro();
            return new FormMain();
        }

        // WS_EX_COMPOSITED — chống flicker cho các panel transparent
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        public FormIntro()
        {
            InitializeComponent();
            LoadLocalConfig();
            try { _bgImage = Properties.Resources.BG_Light; } catch { }
            this.BackColor = _bgTop;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
            this.SuspendLayout();
            BuildLayout();
            this.ResumeLayout(true);
        }

        // ─── Background: ảnh BG_Light scale-to-fill (cache) ───────────────────
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (_bgImage != null && ClientSize.Width > 0 && ClientSize.Height > 0)
            {
                if (_bgScaled == null || _bgScaled.Width != ClientSize.Width || _bgScaled.Height != ClientSize.Height)
                {
                    _bgScaled?.Dispose();
                    _bgScaled = new Bitmap(ClientSize.Width, ClientSize.Height);
                    using (var g = Graphics.FromImage(_bgScaled))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.DrawImage(_bgImage, 0, 0, ClientSize.Width, ClientSize.Height);
                    }
                }
                e.Graphics.DrawImageUnscaled(_bgScaled, 0, 0);
                return;
            }

            // Fallback nếu thiếu resource
            using (var brush = new LinearGradientBrush(
                new PointF(0, 0), new PointF(ClientSize.Width, ClientSize.Height),
                _bgTop, _bgBottom))
            {
                e.Graphics.FillRectangle(brush, ClientRectangle);
            }
        }

        // ─── Layout root ─────────────────────────────────────────────────────
        private void BuildLayout()
        {
            var root = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
                BackColor   = Color.Transparent,
                RowCount    = 3,
                ColumnCount = 1,
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 185));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            this.Controls.Add(root);

            root.Controls.Add(BuildHeader(),    0, 0);
            root.Controls.Add(BuildContent(),   0, 1);
            root.Controls.Add(BuildStatusBar(), 0, 2);

            StartClock();
        }

        // ─── Header: ant logo + "Ant Pay POS" ────────────────────────────────
        private Control BuildHeader()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };

            Image logo = null;
            try { logo = Properties.Resources.ant_load; } catch { }

            panel.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode      = SmoothingMode.AntiAlias;
                g.InterpolationMode  = InterpolationMode.HighQualityBicubic;
                g.TextRenderingHint  = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                var fontMain = new Font("Segoe UI", 48f, FontStyle.Bold);
                var fontTag  = new Font("Segoe UI", 14f, FontStyle.Bold);

                string txt1 = "Ant Pay ";
                string txt2 = "POS";
                string tag  = "FAST  •  SIMPLE  •  SMART";

                var sz1   = g.MeasureString(txt1, fontMain);
                var sz2   = g.MeasureString(txt2, fontMain);
                var szTag = g.MeasureString(tag, fontTag, int.MaxValue,
                    new StringFormat { FormatFlags = StringFormatFlags.MeasureTrailingSpaces });

                // Khối text: dòng chính + tagline ngay dưới
                float textW   = sz1.Width + sz2.Width - 18;
                float mainH   = sz1.Height - 18;          // bỏ bớt leading của font
                float tagGap  = 6;
                float blockH  = mainH + tagGap + szTag.Height;

                // Logo cao ngang cả khối text
                int   logoH   = (int)(blockH + 14);
                int   logoW   = logo != null ? (int)(logoH * (float)logo.Width / logo.Height) : logoH;
                int   logoGap = 24;

                float totalW = logoW + logoGap + textW;
                float startX = (panel.Width  - totalW) / 2f;
                float topY   = (panel.Height - blockH) / 2f;

                if (logo != null)
                    g.DrawImage(logo, startX, topY + (blockH - logoH) / 2f, logoW, logoH);

                // "Ant Pay" đen + "POS" xanh
                float tx = startX + logoW + logoGap;
                float ty = topY - 11;
                using (var b = new SolidBrush(_textDark))
                    g.DrawString(txt1, fontMain, b, tx, ty);
                using (var b = new SolidBrush(_blue))
                    g.DrawString(txt2, fontMain, b, tx + sz1.Width - 18, ty);

                // Tagline giãn đều chữ, căn giữa dưới khối text — vẽ từng ký tự
                float tagY = topY + mainH + tagGap;
                DrawSpacedText(g, tag, fontTag, _textMid, tx + 2, tagY, textW - 12);

                fontMain.Dispose();
                fontTag.Dispose();
            };
            panel.Resize += (s, e) => panel.Invalidate();
            return panel;
        }

        // Vẽ text giãn ký tự đều để khớp targetWidth (letter-spacing kiểu justify)
        private static void DrawSpacedText(Graphics g, string text, Font font, Color color, float x, float y, float targetWidth)
        {
            var widths = new float[text.Length];
            float sumW = 0;
            for (int i = 0; i < text.Length; i++)
            {
                widths[i] = g.MeasureString(text[i].ToString(), font, int.MaxValue,
                    new StringFormat(StringFormat.GenericTypographic)
                    { FormatFlags = StringFormatFlags.MeasureTrailingSpaces }).Width;
                sumW += widths[i];
            }

            float extra = text.Length > 1 ? Math.Max(0, (targetWidth - sumW) / (text.Length - 1)) : 0;
            float cx = x;
            using (var b  = new SolidBrush(color))
            using (var sf = new StringFormat(StringFormat.GenericTypographic))
            {
                for (int i = 0; i < text.Length; i++)
                {
                    g.DrawString(text[i].ToString(), font, b, cx, y, sf);
                    cx += widths[i] + extra;
                }
            }
        }

        // ─── Content: left clock | center card | right info ──────────────────
        private Control BuildContent()
        {
            var table = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
                BackColor   = Color.Transparent,
                RowCount    = 1,
                ColumnCount = 3,
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24));

            table.Controls.Add(BuildClockPanel(), 0, 0);
            table.Controls.Add(BuildMainCard(),   1, 0);
            table.Controls.Add(BuildRightPanel(), 2, 0);
            return table;
        }

        // ─── Left: chỉ có đồng hồ ────────────────────────────────────────────
        private Control BuildClockPanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(50, 14, 10, 0) };

            _lblDay = new Label
            {
                AutoSize  = false, Dock = DockStyle.Top, Height = 56,
                Font      = new Font("Segoe UI", 24f, FontStyle.Regular),
                ForeColor = _textDark, TextAlign = ContentAlignment.BottomLeft,
                BackColor = Color.Transparent,
            };
            _lblDate = new Label
            {
                AutoSize  = false, Dock = DockStyle.Top, Height = 60,
                Font      = new Font("Segoe UI", 26f, FontStyle.Bold),
                ForeColor = _blue, TextAlign = ContentAlignment.BottomLeft,
                BackColor = Color.Transparent,
            };

            // Vẽ cả dòng giờ trong 1 Paint — tránh label chồng nhau che mất AM/PM
            _timeRow = new Panel { AutoSize = false, Dock = DockStyle.Top, Height = 200, BackColor = Color.Transparent };
            _timeRow.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode     = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                var now = DateTime.Now;
                string tm = now.ToString("h:mm");
                string ap = now.ToString("tt");
                string sc = ":" + now.ToString("ss");

                // Auto-scale: thu nhỏ font nếu tổng width vượt quá panel
                float sizeT = 110f, sizeA = 38f, sizeS = 36f;
                using (var sf = new StringFormat(StringFormat.GenericTypographic))
                {
                    using (var fT0 = new Font("Segoe UI", sizeT, FontStyle.Bold))
                    using (var fA0 = new Font("Segoe UI", sizeA, FontStyle.Bold))
                    {
                        float needW = g.MeasureString(tm, fT0, int.MaxValue, sf).Width + 16
                                    + g.MeasureString(ap, fA0, int.MaxValue, sf).Width + 8;
                        if (needW > _timeRow.Width && _timeRow.Width > 50)
                        {
                            float k = _timeRow.Width / needW;
                            sizeT *= k; sizeA *= k; sizeS *= k;
                        }
                    }

                    using (var fT = new Font("Segoe UI", sizeT, FontStyle.Bold))
                    using (var fA = new Font("Segoe UI", sizeA, FontStyle.Bold))
                    using (var fS = new Font("Segoe UI", sizeS, FontStyle.Bold))
                    {
                        var szT = g.MeasureString(tm, fT, int.MaxValue, sf);
                        float ty = (_timeRow.Height - szT.Height) / 2f;

                        using (var b = new SolidBrush(_textDark))
                            g.DrawString(tm, fT, b, -6, ty, sf);

                        float sx = -6 + szT.Width + 16;
                        using (var b = new SolidBrush(_textDark))
                            g.DrawString(ap, fA, b, sx, ty + szT.Height * 0.22f, sf);
                        using (var b = new SolidBrush(_blue))
                            g.DrawString(sc, fS, b, sx, ty + szT.Height * 0.52f, sf);
                    }
                }
            };

            panel.Controls.Add(_timeRow);
            panel.Controls.Add(_lblDate);
            panel.Controls.Add(_lblDay);
            return panel;
        }

        // ─── Center: card lớn chứa Clock In (trái) + PIN pad (phải) ───────────
        private Control BuildMainCard()
        {
            var outer = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };

            var card = new Panel { BackColor = Color.Transparent };
            card.Paint += (s, e) => DrawRoundedCard(e.Graphics, card.ClientRectangle, 22,
                _cardBg, Color.FromArgb(70, 180, 210, 240));

            var inner = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
                BackColor   = Color.Transparent,
                RowCount    = 1,
                ColumnCount = 2,
                Padding     = new Padding(14, 14, 14, 14),
            };
            inner.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));
            inner.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));
            inner.Controls.Add(BuildClockInSection(), 0, 0);
            inner.Controls.Add(BuildPinSection(),     1, 0);
            card.Controls.Add(inner);

            outer.Controls.Add(card);
            outer.Resize += (s, e) =>
            {
                int w = Math.Min(860, outer.Width  - 50);
                int h = Math.Min(580, outer.Height - 30);
                card.SetBounds((outer.Width - w) / 2, (outer.Height - h) / 2, w, h);
            };
            return outer;
        }

        // Cột trái trong card: avatar + Clock In to Start + note
        private Control BuildClockInSection()
        {
            var sec = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };

            sec.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode     = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                int w = sec.Width;

                // Vertical divider bên phải section
                using (var pen = new Pen(Color.FromArgb(235, 238, 244), 1))
                    g.DrawLine(pen, w - 1, 16, w - 1, sec.Height - 16);

                // ── Avatar: vòng tròn nền nhạt + người màu xanh
                int avSize = Math.Min(120, w - 60);
                int avX = (w - avSize) / 2;
                int avY = (int)(sec.Height * 0.08);
                using (var b = new SolidBrush(Color.FromArgb(238, 243, 250)))
                    g.FillEllipse(b, avX, avY, avSize, avSize);
                using (var pen = new Pen(Color.White, 5))
                    g.DrawEllipse(pen, avX + 3, avY + 3, avSize - 6, avSize - 6);

                // người: đầu + thân
                int headD = avSize * 28 / 100;
                int headX = avX + (avSize - headD) / 2;
                int headY = avY + avSize * 22 / 100;
                using (var b = new SolidBrush(_blue))
                {
                    g.FillEllipse(b, headX, headY, headD, headD);
                    int bw = avSize * 52 / 100;
                    int bh = avSize * 30 / 100;
                    int bx = avX + (avSize - bw) / 2;
                    int by = headY + headD + avSize * 4 / 100;
                    using (var path = new GraphicsPath())
                    {
                        path.AddArc(bx, by, bw, bh * 2, 180, 180);
                        path.CloseFigure();
                        g.FillPath(b, path);
                    }
                }

                // ── "Clock In to Start"
                int textY = avY + avSize + (int)(sec.Height * 0.05);
                using (var f = new Font("Segoe UI", 17f, FontStyle.Bold))
                using (var b = new SolidBrush(_textDark))
                using (var sf = new StringFormat { Alignment = StringAlignment.Center })
                    g.DrawString("Clock In to Start", f, b, new RectangleF(0, textY, w, 34), sf);

                // ── Mô tả
                using (var f = new Font("Segoe UI", 10.5f, FontStyle.Regular))
                using (var b = new SolidBrush(_textMid))
                using (var sf = new StringFormat { Alignment = StringAlignment.Center })
                    g.DrawString("Enter your 4-digit PIN to\naccess the POS system.", f, b,
                        new RectangleF(10, textY + 40, w - 20, 48), sf);

                // ── Divider ngang
                int divY = textY + 102;
                using (var pen = new Pen(Color.FromArgb(235, 238, 244), 1))
                    g.DrawLine(pen, 24, divY, w - 26, divY);

                // ── Shield icon + note
                int noteY = divY + 16;
                int shX = 26, shY = noteY + 4, shW = 16, shH = 19;
                using (var path = new GraphicsPath())
                {
                    path.AddLine(shX, shY + 3, shX + shW / 2f, shY);
                    path.AddLine(shX + shW / 2f, shY, shX + shW, shY + 3);
                    path.AddLine(shX + shW, shY + 3, shX + shW, shY + shH * 0.55f);
                    path.AddArc(shX, shY + shH * 0.2f, shW, shH * 0.78f, 0, 180);
                    path.CloseFigure();
                    using (var pen = new Pen(_textMid, 1.6f))
                        g.DrawPath(pen, path);
                }
                using (var f = new Font("Segoe UI", 9.5f, FontStyle.Regular))
                using (var b = new SolidBrush(_textMid))
                    g.DrawString("Employees must clock in\nbefore using the register.", f, b,
                        new RectangleF(shX + shW + 10, noteY, w - shX - shW - 36, 44));
            };
            sec.Resize += (s, e) => sec.Invalidate();
            return sec;
        }

        // Cột phải trong card: title + 4 ô PIN + numpad
        private Control BuildPinSection()
        {
            var sec = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(18, 4, 6, 0) };

            _lblPinTitle = new Label
            {
                Text      = "Enter 4-Digit PIN",
                Font      = new Font("Segoe UI", 14.5f, FontStyle.Bold),
                ForeColor = _textDark,
                AutoSize  = false, Dock = DockStyle.Top, Height = 38,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
            };

            // 4 ô PIN vuông bo góc
            _pinBoxesPanel = new Panel { AutoSize = false, Dock = DockStyle.Top, Height = 72, BackColor = Color.Transparent };
            _pinBoxesPanel.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                int boxW = 58, boxH = 56, gap = 16;
                int totalW = PIN_LENGTH * boxW + (PIN_LENGTH - 1) * gap;
                int startX = (_pinBoxesPanel.Width - totalW) / 2;
                int y = (_pinBoxesPanel.Height - boxH) / 2;

                for (int i = 0; i < PIN_LENGTH; i++)
                {
                    int x = startX + i * (boxW + gap);
                    bool filled = i < _pin.Length;
                    var rect = new Rectangle(x, y, boxW, boxH);

                    Color fillC   = _pinError ? Color.FromArgb(253, 237, 234) : (filled ? Color.FromArgb(234, 242, 253) : Color.White);
                    Color borderC = _pinError ? _red : (filled ? _blue : _padBorder);
                    Color dotC    = _pinError ? _red : (filled ? _blue : Color.FromArgb(190, 205, 225));

                    using (var path = RoundedRect(rect, 12))
                    {
                        using (var b = new SolidBrush(fillC))
                            g.FillPath(b, path);
                        using (var pen = new Pen(borderC, filled || _pinError ? 2f : 1.5f))
                            g.DrawPath(pen, path);
                    }
                    // chấm tròn nhỏ ở giữa
                    int dotD = filled || _pinError ? 14 : 8;
                    using (var b = new SolidBrush(dotC))
                        g.FillEllipse(b, x + (boxW - dotD) / 2, y + (boxH - dotD) / 2, dotD, dotD);
                }
            };

            var numpad = BuildNumpad();

            sec.Controls.Add(numpad);
            sec.Controls.Add(_pinBoxesPanel);
            sec.Controls.Add(_lblPinTitle);
            return sec;
        }

        private Control BuildNumpad()
        {
            var table = new TableLayoutPanel
            {
                Dock        = DockStyle.Fill,
                RowCount    = 4,
                ColumnCount = 3,
                BackColor   = Color.Transparent,
                Padding     = new Padding(0, 8, 0, 4),
            };
            for (int r = 0; r < 4; r++) table.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            for (int c = 0; c < 3; c++) table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));

            string[] labels = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "⌫", "0", "ENTER" };
            for (int i = 0; i < 12; i++)
            {
                int    row = i / 3, col = i % 3;
                string lbl = labels[i];

                if (lbl == "ENTER")
                {
                    var btn = MakePadButton("ENTER", "CLOCK IN", _blue, Color.White, 15f);
                    btn.Click += (s, e) => OnEnterPressed();
                    table.Controls.Add(btn, col, row);
                }
                else if (lbl == "⌫")
                {
                    var btn = MakePadButton("⌫", null, _padBtnBg, _textDark, 18f);
                    btn.Click += (s, e) => OnBackspacePressed();
                    table.Controls.Add(btn, col, row);
                }
                else
                {
                    string digit = lbl;
                    var btn = MakePadButton(digit, null, Color.White, _textDark, 20f);
                    btn.Click += (s, e) => OnDigitPressed(digit);
                    table.Controls.Add(btn, col, row);
                }
            }
            return table;
        }

        private Button MakePadButton(string text, string subText, Color backColor, Color foreColor, float fontSize)
        {
            var btn = new Button
            {
                Text      = text,
                Dock      = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat,
                BackColor = backColor,
                ForeColor = foreColor,
                Font      = new Font("Segoe UI", fontSize, FontStyle.Bold),
                Cursor    = Cursors.Hand,
                Margin    = new Padding(6),
                UseVisualStyleBackColor = false,
            };
            btn.FlatAppearance.BorderSize = 0;

            bool isBlue = backColor == _blue;
            var hoverColor = isBlue ? _blueDark : Color.FromArgb(232, 240, 252);
            bool hovered = false, pressed = false;

            btn.MouseEnter += (s, e) => { hovered = true;  btn.Invalidate(); };
            btn.MouseLeave += (s, e) => { hovered = false; pressed = false; btn.Invalidate(); };
            btn.MouseDown  += (s, e) => { pressed = true;  btn.Invalidate(); };
            btn.MouseUp    += (s, e) => { pressed = false; btn.Invalidate(); };

            btn.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode     = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                var bg = pressed || hovered ? hoverColor : btn.BackColor;
                var rect = new Rectangle(0, 0, btn.Width - 1, btn.Height - 1);
                g.Clear(_cardBg);
                using (var path = RoundedRect(rect, 12))
                {
                    using (var b = new SolidBrush(bg))
                        g.FillPath(b, path);
                    if (!isBlue)
                        using (var pen = new Pen(_padBorder, 1.5f))
                            g.DrawPath(pen, path);
                }

                if (subText == null)
                {
                    using (var b  = new SolidBrush(btn.ForeColor))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                        g.DrawString(btn.Text, btn.Font, b, new RectangleF(0, 0, btn.Width, btn.Height), sf);
                }
                else
                {
                    // 2 dòng: ENTER lớn + CLOCK IN nhỏ
                    using (var b  = new SolidBrush(btn.ForeColor))
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        float mainH = btn.Height * 0.62f;
                        g.DrawString(btn.Text, btn.Font, b, new RectangleF(0, 2, btn.Width, mainH), sf);
                        using (var fSub = new Font("Segoe UI", 8f, FontStyle.Bold))
                            g.DrawString(subText, fSub, b, new RectangleF(0, mainH - 4, btn.Width, btn.Height - mainH), sf);
                    }
                }
            };
            return btn;
        }

        // ─── Right: store + terminal cards trắng có icon ──────────────────────
        private Control BuildRightPanel()
        {
            var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(14, 10, 44, 0) };

            string storeName = "";
            try { storeName = Utilitys.GetStoreConfig("store_name", ""); } catch { }
            if (string.IsNullOrWhiteSpace(storeName))
                storeName = Constants.pos_store_code;

            var storeCard = BuildInfoCard(
                storeName.ToUpper(),
                "Store Location",
                DrawStoreIcon);

            var spacer = new Panel { Dock = DockStyle.Top, Height = 14, BackColor = Color.Transparent };

            var termCard = BuildInfoCard(
                string.IsNullOrEmpty(Constants.codepay_terminal_sn) ? "TERMINAL-01" : Constants.codepay_terminal_sn,
                "Front Register",
                DrawMonitorIcon);

            panel.Controls.Add(termCard);
            panel.Controls.Add(spacer);
            panel.Controls.Add(storeCard);
            return panel;
        }

        private Control BuildInfoCard(string title, string subtitle, Action<Graphics, Rectangle> drawIcon)
        {
            var card = new Panel
            {
                AutoSize  = false, Dock = DockStyle.Top, Height = 84,
                BackColor = Color.Transparent,
            };
            card.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode     = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                DrawRoundedCard(g, card.ClientRectangle, 16, _cardBg, Color.FromArgb(70, 180, 210, 240));

                // icon ô vuông bo góc nền xanh nhạt
                int iconSz = 44;
                int ix = 18, iy = (card.Height - iconSz) / 2;
                var iconRect = new Rectangle(ix, iy, iconSz, iconSz);
                using (var path = RoundedRect(iconRect, 10))
                using (var b = new SolidBrush(Color.FromArgb(232, 241, 252)))
                    g.FillPath(b, path);
                drawIcon(g, iconRect);

                int tx = ix + iconSz + 14;
                using (var f = new Font("Segoe UI", 11.5f, FontStyle.Bold))
                using (var b = new SolidBrush(_textDark))
                    g.DrawString(title, f, b, tx, card.Height / 2f - 22);
                using (var f = new Font("Segoe UI", 9.5f, FontStyle.Regular))
                using (var b = new SolidBrush(_textMid))
                    g.DrawString(subtitle, f, b, tx, card.Height / 2f + 2);
            };
            card.Resize += (s, e) => card.Invalidate();
            return card;
        }

        private static void DrawStoreIcon(Graphics g, Rectangle r)
        {
            // mái hiên + thân cửa hàng
            int cx = r.X + r.Width / 2, cy = r.Y + r.Height / 2;
            using (var pen = new Pen(_blue, 2f) { LineJoin = LineJoin.Round })
            {
                g.DrawRectangle(pen, cx - 10, cy - 2, 20, 11);
                using (var path = new GraphicsPath())
                {
                    path.AddLine(cx - 13, cy - 2, cx - 11, cy - 10);
                    path.AddLine(cx - 11, cy - 10, cx + 11, cy - 10);
                    path.AddLine(cx + 11, cy - 10, cx + 13, cy - 2);
                    path.CloseFigure();
                    g.DrawPath(pen, path);
                }
                g.DrawLine(pen, cx - 3, cy + 9, cx - 3, cy + 3);
                g.DrawLine(pen, cx - 3, cy + 3, cx + 4, cy + 3);
                g.DrawLine(pen, cx + 4, cy + 3, cx + 4, cy + 9);
            }
        }

        private static void DrawMonitorIcon(Graphics g, Rectangle r)
        {
            int cx = r.X + r.Width / 2, cy = r.Y + r.Height / 2;
            using (var pen = new Pen(_blue, 2f) { LineJoin = LineJoin.Round })
            {
                g.DrawRectangle(pen, cx - 12, cy - 9, 24, 15);
                g.DrawLine(pen, cx - 5, cy + 10, cx + 5, cy + 10);
                g.DrawLine(pen, cx, cy + 6, cx, cy + 10);
            }
        }

        // ─── Status bar nổi: card trắng bo góc ───────────────────────────────
        private Control BuildStatusBar()
        {
            var outer = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(50, 8, 50, 18) };

            var bar = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            bar.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode     = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                DrawRoundedCard(g, bar.ClientRectangle, 18, Color.FromArgb(235, 255, 255, 255), Color.FromArgb(60, 180, 210, 240));

                int h = bar.Height;
                int iconSz = 38;
                int iy = (h - iconSz) / 2;

                // 3 status items chia đều phần đầu thanh (chừa chỗ CHECK UPDATE + CLOSE bên phải)
                int sectionW = (bar.Width - 360) / 3;
                DrawStatusItem(g, 30 + sectionW * 0, iy, iconSz, _blue,  DrawWifiGlyph,    "ONLINE",            "System Connected");
                DrawStatusItem(g, 30 + sectionW * 1, iy, iconSz, _green, DrawPrinterGlyph, "PRINTER READY",     "Receipts Enabled");
                DrawStatusItem(g, 30 + sectionW * 2, iy, iconSz, _teal,  DrawDrawerGlyph,  "CASH DRAWER READY", "Drawer Connected");

                // separators
                using (var pen = new Pen(Color.FromArgb(235, 238, 244), 1))
                {
                    g.DrawLine(pen, 30 + sectionW - 25,     h / 4, 30 + sectionW - 25,     h * 3 / 4);
                    g.DrawLine(pen, 30 + sectionW * 2 - 25, h / 4, 30 + sectionW * 2 - 25, h * 3 / 4);
                    g.DrawLine(pen, 30 + sectionW * 3 - 25, h / 4, 30 + sectionW * 3 - 25, h * 3 / 4);
                }

                // CHECK UPDATE: hiện version hiện tại, click để check
                int ux = bar.Width - 340;
                DrawStatusItem(g, ux, iy, iconSz, _blue, DrawUpdateGlyph,
                    "CHECK UPDATE", "Version " + NailsChekin.Models.Helper.UpdateHelper.GetLocalVersionString());

                // CLOSE bên phải
                int gx = bar.Width - 150;
                int gy = h / 2;
                DrawCloseGlyph(g, new Rectangle(gx, gy - 10, 20, 20), _red);
                using (var f = new Font("Segoe UI", 10f, FontStyle.Bold))
                using (var b = new SolidBrush(_red))
                    g.DrawString("CLOSE", f, b, gx + 26, gy - 10);
            };
            bar.Resize += (s, e) => bar.Invalidate();
            // vùng click: CHECK UPDATE + CLOSE
            bar.MouseMove += (s, e) =>
            {
                var hitClose  = new Rectangle(bar.Width - 156, bar.Height / 2 - 16, 116, 32);
                var hitUpdate = new Rectangle(bar.Width - 345, bar.Height / 2 - 20, 185, 40);
                bar.Cursor = (hitClose.Contains(e.Location) || hitUpdate.Contains(e.Location))
                    ? Cursors.Hand : Cursors.Default;
            };
            bar.MouseClick += (s, e) =>
            {
                var hitClose  = new Rectangle(bar.Width - 156, bar.Height / 2 - 16, 116, 32);
                var hitUpdate = new Rectangle(bar.Width - 345, bar.Height / 2 - 20, 185, 40);
                if (hitClose.Contains(e.Location))
                    Application.Exit();
                else if (hitUpdate.Contains(e.Location))
                    CheckUpdateNow();
            };
            outer.Controls.Add(bar);
            return outer;
        }

        private static void DrawStatusItem(Graphics g, int x, int y, int iconSz, Color color,
            Action<Graphics, Rectangle, Color> glyph, string title, string sub)
        {
            // icon tròn màu
            using (var b = new SolidBrush(color))
                g.FillEllipse(b, x, y, iconSz, iconSz);
            glyph(g, new Rectangle(x, y, iconSz, iconSz), Color.White);

            int tx = x + iconSz + 12;
            using (var f = new Font("Segoe UI", 10f, FontStyle.Bold))
            using (var br = new SolidBrush(_textDark))
                g.DrawString(title, f, br, tx, y + 1);
            using (var f = new Font("Segoe UI", 9f, FontStyle.Regular))
            using (var br = new SolidBrush(_textMid))
                g.DrawString(sub, f, br, tx, y + 20);
        }

        private static void DrawWifiGlyph(Graphics g, Rectangle r, Color c)
        {
            int cx = r.X + r.Width / 2, cy = r.Y + r.Height * 62 / 100;
            using (var pen = new Pen(c, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
            {
                g.DrawArc(pen, cx - 11, cy - 11, 22, 22, 215, 110);
                g.DrawArc(pen, cx - 7,  cy - 7,  14, 14, 215, 110);
            }
            using (var b = new SolidBrush(c))
                g.FillEllipse(b, cx - 2, cy - 2, 4, 4);
        }

        private static void DrawPrinterGlyph(Graphics g, Rectangle r, Color c)
        {
            int cx = r.X + r.Width / 2, cy = r.Y + r.Height / 2;
            using (var pen = new Pen(c, 1.8f) { LineJoin = LineJoin.Round })
            {
                g.DrawRectangle(pen, cx - 8, cy - 3, 16, 8);
                g.DrawRectangle(pen, cx - 5, cy - 8, 10, 5);
                g.DrawRectangle(pen, cx - 5, cy + 3, 10, 6);
            }
        }

        private static void DrawDrawerGlyph(Graphics g, Rectangle r, Color c)
        {
            int cx = r.X + r.Width / 2, cy = r.Y + r.Height / 2;
            using (var pen = new Pen(c, 1.8f) { LineJoin = LineJoin.Round })
            {
                g.DrawRectangle(pen, cx - 9, cy - 7, 18, 14);
                g.DrawLine(pen, cx - 9, cy - 1, cx + 9, cy - 1);
                g.DrawLine(pen, cx - 3, cy + 3, cx + 3, cy + 3);
            }
        }

        // Mũi tên vòng tròn (refresh) — icon CHECK UPDATE
        private static void DrawUpdateGlyph(Graphics g, Rectangle r, Color c)
        {
            int cx = r.X + r.Width / 2, cy = r.Y + r.Height / 2;
            int rad = r.Width / 4 + 1;
            using (var pen = new Pen(c, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                g.DrawArc(pen, cx - rad, cy - rad, rad * 2, rad * 2, -50, 280);
            // đầu mũi tên ở cuối cung
            using (var b = new SolidBrush(c))
            {
                var pts = new[]
                {
                    new PointF(cx + rad + 3, cy - rad + 2),
                    new PointF(cx + rad - 5, cy - rad + 4),
                    new PointF(cx + rad + 1, cy - rad + 9),
                };
                g.FillPolygon(b, pts);
            }
        }

        // ─── Check update ────────────────────────────────────────────────────
        private bool _checkingUpdate;
        private async void CheckUpdateNow()
        {
            if (_checkingUpdate) return;
            _checkingUpdate = true;
            try { await NailsChekin.Models.Helper.UpdateHelper.ManualCheckAsync(this); }
            finally { _checkingUpdate = false; }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            // Auto check version mới khi mở app (im lặng nếu không có / lỗi mạng)
            NailsChekin.Models.Helper.UpdateHelper.AutoCheckOnStartup(this);
        }

        private static void DrawCloseGlyph(Graphics g, Rectangle r, Color c)
        {
            int cx = r.X + r.Width / 2, cy = r.Y + r.Height / 2;
            int rad = r.Width / 2 - 1;
            using (var pen = new Pen(c, 1.8f) { StartCap = LineCap.Round, EndCap = LineCap.Round })
            {
                g.DrawEllipse(pen, cx - rad, cy - rad, rad * 2, rad * 2);
                int k = rad / 2 + 1;
                g.DrawLine(pen, cx - k, cy - k, cx + k, cy + k);
                g.DrawLine(pen, cx - k, cy + k, cx + k, cy - k);
            }
        }

        // ─── Clock ───────────────────────────────────────────────────────────
        private void StartClock()
        {
            UpdateClock();
            _clockTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            _clockTimer.Tick += (s, e) => UpdateClock();
            _clockTimer.Start();
        }

        private void UpdateClock()
        {
            var now = DateTime.Now;
            if (_lblDay  != null) _lblDay.Text  = now.ToString("dddd").ToUpper();
            if (_lblDate != null) _lblDate.Text = now.ToString("MMMM d, yyyy").ToUpper();
            _timeRow?.Invalidate();
        }

        // ─── PIN logic ───────────────────────────────────────────────────────
        private void OnDigitPressed(string digit)
        {
            if (_pin.Length >= PIN_LENGTH) return;
            ClearPinError();
            _pin += digit;
            _pinBoxesPanel?.Invalidate();
            if (_pin.Length == PIN_LENGTH)
                ValidatePin();
        }

        private void OnBackspacePressed()
        {
            if (_pin.Length == 0) return;
            _pin = _pin.Substring(0, _pin.Length - 1);
            _pinBoxesPanel?.Invalidate();
        }

        private void OnEnterPressed()
        {
            if (_pin.Length == 0) return;
            ValidatePin();
        }

        private void ValidatePin()
        {
            if (_pin == _correctPin)
            {
                LoginSuccess?.Invoke(_pin);
                OpenMainForm();
            }
            else
            {
                ShowPinError();
            }
        }

        private void OpenMainForm()
        {
            _clockTimer?.Stop();

            var frm = new FormMain();
            // FormIntro là main form của Application.Run — đóng nó khi FormMain đóng để thoát app
            frm.FormClosed += (s, e) => this.Close();
            frm.Show();
            this.Hide();
        }

        private void ShowPinError()
        {
            _pin = "";
            _pinError = true;
            _pinBoxesPanel?.Invalidate();
            if (_lblPinTitle != null)
            {
                _lblPinTitle.Text      = "Incorrect PIN. Please try again.";
                _lblPinTitle.ForeColor = _red;
            }

            var t = new System.Windows.Forms.Timer { Interval = 1600 };
            t.Tick += (s, e) =>
            {
                t.Stop();
                t.Dispose();
                ClearPinError();
            };
            t.Start();
        }

        private void ClearPinError()
        {
            if (!_pinError) return;
            _pinError = false;
            _pinBoxesPanel?.Invalidate();
            if (_lblPinTitle != null)
            {
                _lblPinTitle.Text      = "Enter 4-Digit PIN";
                _lblPinTitle.ForeColor = _textDark;
            }
        }

        // ─── Drawing helpers ─────────────────────────────────────────────────
        private static void DrawRoundedCard(Graphics g, Rectangle rect, int radius, Color fill, Color border)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var r = new Rectangle(rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            using (var path = RoundedRect(r, radius))
            {
                using (var b = new SolidBrush(fill))
                    g.FillPath(b, path);
                using (var pen = new Pen(border, 1.5f))
                    g.DrawPath(pen, path);
            }
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d    = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(bounds.X,         bounds.Y,          d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y,          d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d,   0, 90);
            path.AddArc(bounds.X,         bounds.Bottom - d, d, d,  90, 90);
            path.CloseFigure();
            return path;
        }

        // ─── Cleanup ─────────────────────────────────────────────────────────
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _clockTimer?.Stop();
            _clockTimer?.Dispose();
            _bgScaled?.Dispose();
            _bgScaled = null;
            base.OnFormClosed(e);
        }
    }
}
