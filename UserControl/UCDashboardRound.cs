using NailsChekin.Models.Helper;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace NailsChekin.UserControl
{
    class UCDashboardRound : ItemBaseControl
    {
        // Theme cho card
        [Category("Card/Theme")] public Color CardBaseColor { get; set; } = Color.Orange;
        [Category("Card/Theme")] public Color CardHoverColor { get; set; } = ColorHelper.Question; // Color.FromArgb(245, 248, 255);
        [Category("Card/Theme")] public Color CardPressColor { get; set; } = Color.FromArgb(235, 242, 255);
        [Category("Card/Theme")] public Color CardBorderColor { get; set; } = Color.FromArgb(210, 210, 210);
        [Category("Card/Theme")] public Color CardSelBorderColor { get; set; } = Color.DodgerBlue;

        public UCDashboardRound(string amount, string name, string color)
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            Cursor = Cursors.Hand;
            Height = 125;
            MinimumSize = new Size(150, 125);

            Title = "$" + amount;
            Subtitle = name.ToUpper();
            ShowIcon = false;
            TitleSize = 22; SubtitleSize = LayoutHelper.mini_screen ? 16 : 18;

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.Half;

            // Nền & viền
            var rect = ClientRectangle; rect.Inflate(-2, -2);
            Color baseBg = _pressed ? CardPressColor : (_hover ? CardHoverColor : CardBaseColor);
            using (var path = RoundedRect(rect, CornerRadius))
            using (var bk = new SolidBrush(baseBg))
            using (var pen = new Pen(Selected ? CardSelBorderColor : CardBorderColor, Selected ? 2f : 1f))
            {
                g.FillPath(bk, path);
                g.DrawPath(pen, path);
            }

            int padL = CardPadding.Left, padT = CardPadding.Top, padR = CardPadding.Right, padB = 5; // padB = CardPadding.Bottom;
            int iconBox = 52;
            int x = rect.Left + padL;
            int y = rect.Top + padT;
            int w = rect.Width - padL - padR;
            int h = rect.Height - padT - padB;

            // Icon
            _iconRect = Rectangle.Empty;
            int textLeft = x;
            if (ShowIcon)
            {
                _iconRect = new Rectangle(x, (y + (h - iconBox) / 2) + 10, iconBox, iconBox);
                bool drew = DrawAvatar(g, _iconRect);         // đã check ShowIcon bên trong rồi, nhưng giữ cho rõ ràng
                if (drew) textLeft = _iconRect.Right + IconTextGap;
            }

            // Text
            using (var fTitle = new Font(Font.FontFamily, TitleSize, FontStyle.Bold))
            using (var fSub = new Font(Font.FontFamily, SubtitleSize, FontStyle.Bold))
            {
                int availW = w - (textLeft - x);
                int curY = y + TextTopPadding;

                // === Title (giữ nguyên cách vẽ của bạn) ===
                int hTitleLn = MeasureLine(g, fTitle);
                TextRenderer.DrawText(g, _title ?? "", fTitle,
                    new Rectangle(textLeft, curY, (w - x), hTitleLn),
                    TitleColor,
                    TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
                curY += hTitleLn + LineSpacing;

                int hSubLn = MeasureLine(g, fSub);
                TextRenderer.DrawText(g, _subtitle ?? "", fSub,
                    new Rectangle(textLeft, curY, (w - x), hSubLn),
                    SubtitleColor,
                    TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // UCDashboardRound
            // 
            this.Name = "UCDashboardRound";
            this.Size = new System.Drawing.Size(366, 125);
            this.ResumeLayout(false);

        }

    }
}
