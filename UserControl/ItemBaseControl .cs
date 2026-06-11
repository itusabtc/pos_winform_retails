using NailsChekin.Models.Helper;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NailsChekin.UserControl
{
    public class ItemBaseControl : System.Windows.Forms.UserControl
    {
        // ======= State chung =======
        protected bool _hover, _pressed, _selected;
        protected string _title = "Title";
        protected string _subtitle = "Subtitle";
        protected string _description = "Description";
        protected Image _icon;
        protected Rectangle _iconRect;
        protected bool _iconOwnedByControl;

        // ======= Thuộc tính tái dùng =======
        [Category("Card/Data")] public string Title { get => _title; set { _title = value ?? ""; Invalidate(); } }
        [Category("Card/Data")] public string Subtitle { get => _subtitle; set { _subtitle = value ?? ""; Invalidate(); } }
        [Category("Card/Data")] public string Description { get => _description; set { _description = value ?? ""; Invalidate(); } }

        [Category("Card/Data")]
        public Image Icon
        {
            get => _icon;
            set { _iconOwnedByControl = false; _icon = value; Invalidate(); }
        }

        [Category("Card/Data")] public bool Selected { get => _selected; set { _selected = value; Invalidate(); } }

        [Category("Card/Layout")] public int CornerRadius { get; set; } = 12;
        [Category("Card/Layout")] public Padding CardPadding { get; set; } = new Padding(12, 10, 12, 10);
        [Category("Card/Layout")] public int IconSize { get; set; } = 64;
        [Category("Card/Layout")] public int IconTextGap { get; set; } = 10;
        [Category("Card/Layout")] public bool AlignTextToIcon { get; set; } = false;
        [Category("Card/Layout")] public int TextTopPadding { get; set; } = 0;

        [Category("Card/Icon")] public Image PlaceholderIcon { get; set; }
        public enum IconFrame { Original, Circle, Rounded }
        [Category("Card/Icon")] public IconFrame IconFrameStyle { get; set; } = IconFrame.Original;
        [Category("Card/Icon")] public int IconCornerRadius { get; set; } = 12;
        [Category("Card/Icon")] public Color IconFrameBorderColor { get; set; } = Color.Transparent;
        [Category("Card/Icon")] public float IconFrameBorderThickness { get; set; } = 1f;
        [Category("Card/Icon")] public Color IconFrameBackground { get; set; } = Color.Transparent;

        [Category("Card/Icon")] public bool UseInitialAsPlaceholder { get; set; } = true;
        [Category("Card/Icon")] public bool InitialAutoColor { get; set; } = true;
        [Category("Card/Icon")] public Color InitialBgColor { get; set; } = Color.FromArgb(240, 240, 240);
        [Category("Card/Icon")] public Color InitialTextColor { get; set; } = Color.FromArgb(40, 40, 40);
        [Category("Card/Icon")] public bool InitialUseSubtitle { get; set; } = false;   // false = lấy từ Title (mặc định); true = lấy từ Subtitle
        [Category("Card/Icon")] public bool ShowIcon { get; set; } = true;

        [Category("Card/Text")] public float TitleSize { get; set; } = LayoutHelper.mini_screen ? 16f : 18f;
        [Category("Card/Text")] public float SubtitleSize { get; set; } = LayoutHelper.mini_screen ? 16f : 18f;
        [Category("Card/Text")] public float DescSize { get; set; } = LayoutHelper.mini_screen ? 10f : 12f;
        [Category("Card/Text")] public int LineSpacing { get; set; } = 2;

        [Category("Card/Text")] public Color TitleColor { get; set; } = Color.White;
        [Category("Card/Text")] public Color SubtitleColor { get; set; } = Color.FromArgb(230, 255, 255, 255);
        [Category("Card/Text")] public Color DescriptionColor { get; set; } = Color.FromArgb(210, 255, 255, 255);

        // ======= Click hành vi + sự kiện =======
        [Category("Card/Behavior")] public bool ToggleSelectedOnClick { get; set; } = false;
        [Category("Card/Behavior")] public string BusyText { get; set; } = "Processing…";
        [Category("Card/Behavior")] public int AutoRevertDescMs { get; set; } = 0;

        public event EventHandler CardClick;
        public event EventHandler ImageClick;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public EventHandler CardClickInline { get; set; }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public EventHandler ImageClickInline { get; set; }

        // Cho phép gán tác vụ async khi click; trả về Description mới
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public Func<ItemBaseControl, Task<string>> CardActionAsync { get; set; }

        protected virtual void OnCardClick()
        {
            CardClickInline?.Invoke(this, EventArgs.Empty);
            CardClick?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnImageClick()
        {
            ImageClickInline?.Invoke(this, EventArgs.Empty);
            ImageClick?.Invoke(this, EventArgs.Empty);
        }

        private bool _isBusy;
        protected async void HandleCardClickAsync()
        {
            if (_isBusy) return;
            _isBusy = true;

            var oldDesc = _description;

            if (!string.IsNullOrEmpty(BusyText))
            {
                _description = BusyText;
                Invalidate();
            }

            string newDesc = null;
            try
            {
                if (CardActionAsync != null)
                    newDesc = await CardActionAsync(this).ConfigureAwait(true);

                OnCardClick();
            }
            finally
            {
                //if (!string.IsNullOrEmpty(newDesc))
                //    _description = newDesc;
                //else
                //    _description = oldDesc;

                _description = oldDesc;
                _isBusy = false;
                Invalidate();

                //if (AutoRevertDescMs > 0 && !string.IsNullOrEmpty(newDesc))
                //{
                //    var revert = oldDesc;
                //    var t = new System.Windows.Forms.Timer { Interval = AutoRevertDescMs };
                //    t.Tick += (s, e) =>
                //    {
                //        t.Stop(); t.Dispose();
                //        if (!_isBusy) { _description = revert; Invalidate(); }
                //    };
                //    t.Start();
                //}
            }
        }

        // ======= Async icon loader dùng chung =======
        private static readonly HttpClient _http = new HttpClient();
        private static readonly ConcurrentDictionary<string, Image> _iconCache =
            new ConcurrentDictionary<string, Image>(StringComparer.OrdinalIgnoreCase);
        private CancellationTokenSource _iconCts;

        public async Task LoadIconAsync(string pathOrUrl, Image placeholder = null,
                                        CancellationToken externalToken = default(CancellationToken))
        {
            var ph = placeholder ?? PlaceholderIcon;
            if (ph != null) SetIconFromControlOwned(CloneSafe(ph));
            if (string.IsNullOrWhiteSpace(pathOrUrl)) return;

            _iconCts?.Cancel(); _iconCts?.Dispose();
            _iconCts = new CancellationTokenSource();

            using (var linked = CancellationTokenSource.CreateLinkedTokenSource(_iconCts.Token, externalToken))
            {
                var ct = linked.Token;
                try
                {
                    if (_iconCache.TryGetValue(pathOrUrl, out var cached))
                    {
                        SetIconFromControlOwned(CloneSafe(cached));
                        return;
                    }

                    Image loaded;
                    if (IsHttpUrl(pathOrUrl))
                    {
                        var bytes = await _http.GetByteArrayAsync(pathOrUrl).ConfigureAwait(false);
                        ct.ThrowIfCancellationRequested();
                        loaded = await Task.Run(() => LoadBitmapFromBytes(bytes), ct).ConfigureAwait(false);
                    }
                    else
                    {
                        loaded = await Task.Run(() => LoadBitmapFromFile(pathOrUrl), ct).ConfigureAwait(false);
                    }

                    _iconCache[pathOrUrl] = loaded;
                    ct.ThrowIfCancellationRequested();
                    SetIconFromControlOwned(CloneSafe(loaded));
                }
                catch { /* ignore; giữ placeholder */ }
            }
        }

        public Task LoadIconAsync(byte[] rawBytes, Image placeholder = null,
                                  CancellationToken externalToken = default(CancellationToken))
        {
            var ph = placeholder ?? PlaceholderIcon;
            if (ph != null) SetIconFromControlOwned(CloneSafe(ph));

            _iconCts?.Cancel(); _iconCts?.Dispose();
            _iconCts = new CancellationTokenSource();
            var ct = _iconCts.Token;

            return Task.Run(() =>
            {
                var bmp = LoadBitmapFromBytes(rawBytes);
                ct.ThrowIfCancellationRequested();
                if (!IsDisposed)
                    BeginInvoke(new Action(() => SetIconFromControlOwned(CloneSafe(bmp))));
            }, externalToken);
        }

        // ======= Mouse state chung =======
        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hover = true; Invalidate(); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hover = false; _pressed = false; Invalidate(); }
        protected override void OnMouseDown(MouseEventArgs e) { base.OnMouseDown(e); if (e.Button == MouseButtons.Left) { _pressed = true; Invalidate(); } }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (!ClientRectangle.Contains(e.Location)) { _pressed = false; Invalidate(); return; }

            if (_iconRect.Contains(e.Location) && (Icon != null || PlaceholderIcon != null || UseInitialAsPlaceholder))
            {
                //OnImageClick();
                if (ToggleSelectedOnClick) _selected = !_selected;
                HandleCardClickAsync();
            }
            else
            {
                if (ToggleSelectedOnClick) _selected = !_selected;
                HandleCardClickAsync();
            }

            _pressed = false;
            Invalidate();
        }

        // ======= Helpers dùng chung =======
        protected static GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            int d = radius * 2; var p = new GraphicsPath();
            if (radius <= 0) { p.AddRectangle(r); p.CloseFigure(); return p; }
            p.AddArc(r.Left, r.Top, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Top, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure(); return p;
        }
        protected static Rectangle GetContainedRect(Image img, Rectangle box)
        {
            float rW = (float)box.Width / img.Width, rH = (float)box.Height / img.Height;
            float scale = Math.Min(rW, rH);
            int w = (int)Math.Round(img.Width * scale), h = (int)Math.Round(img.Height * scale);
            int x = box.X + (box.Width - w) / 2, y = box.Y + (box.Height - h) / 2;
            return new Rectangle(x, y, w, h);
        }
        protected static int MeasureLine(Graphics g, Font font)
        {
            var sz = TextRenderer.MeasureText(g, "Ag", font, Size.Empty,
                TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
            return sz.Height;
        }
        protected static Font FitFont(Graphics g, FontFamily fam, FontStyle style, string text, Rectangle box, float initPx)
        {
            float size = initPx;
            for (int i = 0; i < 8; i++)
            {
                using (var f = new Font(fam, size, style, GraphicsUnit.Pixel))
                {
                    var sz = TextRenderer.MeasureText(g, text, f, Size.Empty,
                        TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
                    if (sz.Width <= box.Width * 0.75f && sz.Height <= box.Height * 0.75f)
                        return new Font(fam, size, style, GraphicsUnit.Pixel);
                }
                size *= 0.9f;
            }
            return new Font(fam, Math.Max(8f, size), style, GraphicsUnit.Pixel);
        }

        protected void DrawIconWithFrame(Graphics g, Image img, Rectangle box)
        {
            if (img == null || box.Width <= 0 || box.Height <= 0) return;

            if (IconFrameStyle == IconFrame.Original)
            {
                g.DrawImage(img, GetContainedRect(img, box));
                return;
            }

            using (var path = new GraphicsPath())
            {
                if (IconFrameStyle == IconFrame.Circle) path.AddEllipse(box);
                else path.AddPath(RoundedRect(box, Math.Max(0, IconCornerRadius)), false);

                GraphicsState st = g.Save();
                try
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    if (IconFrameBackground.A > 0)
                        using (var bg = new SolidBrush(IconFrameBackground)) g.FillPath(bg, path);

                    g.SetClip(path, CombineMode.Replace);
                    g.DrawImage(img, GetContainedRect(img, box));
                }
                finally { g.Restore(st); }

                if (IconFrameBorderThickness > 0f && IconFrameBorderColor.A > 0)
                    using (var pen = new Pen(IconFrameBorderColor, IconFrameBorderThickness))
                        g.DrawPath(pen, path);
            }
        }

        protected void DrawInitialAvatar(Graphics g, Rectangle box)
        {
            //string letter = GetFirstLetter(Title);
            // >>> chọn chuỗi làm nguồn lấy chữ cái &màu
            string seed = InitialUseSubtitle
                ? (string.IsNullOrWhiteSpace(Subtitle) ? Title : Subtitle)   // fallback sang Title nếu Subtitle rỗng
                : (string.IsNullOrWhiteSpace(Title) ? Subtitle : Title);     // fallback sang Subtitle nếu Title rỗng

            string letter = GetFirstLetter(seed);
            Color bg = InitialAutoColor ? ColorFromString(Title) : InitialBgColor;

            using (var path = new GraphicsPath())
            {
                if (IconFrameStyle == IconFrame.Circle) path.AddEllipse(box);
                else if (IconFrameStyle == IconFrame.Rounded) path.AddPath(RoundedRect(box, IconCornerRadius), false);
                else path.AddRectangle(box);

                using (var b = new SolidBrush(bg)) g.FillPath(b, path);
                if (IconFrameBorderThickness > 0f && IconFrameBorderColor.A > 0)
                    using (var pen = new Pen(IconFrameBorderColor, IconFrameBorderThickness))
                        g.DrawPath(pen, path);

                using (var f = FitFont(g, this.Font.FontFamily, FontStyle.Bold, letter, box, box.Height * 0.55f))
                {
                    var flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                                TextFormatFlags.SingleLine | TextFormatFlags.NoPadding;
                    TextRenderer.DrawText(g, letter, f, box, InitialTextColor, flags);
                }
            }
        }

        protected bool DrawAvatar(Graphics g, Rectangle box)
        {
            if (!ShowIcon) return false;          // ⬅️ thêm dòng này

            if (Icon != null) { DrawIconWithFrame(g, Icon, box); return true; }
            if (UseInitialAsPlaceholder) { DrawInitialAvatar(g, box); return true; }
            if (PlaceholderIcon != null) { DrawIconWithFrame(g, PlaceholderIcon, box); return true; }
            return false;
        }

        protected static string GetFirstLetter(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "?";
            foreach (var c in s.Trim()) if (char.IsLetterOrDigit(c)) return char.ToUpperInvariant(c).ToString();
            return "?";
        }
        protected static Color ColorFromString(string s)
        {
            Color[] pal = {
                Color.FromArgb(0xEF,0x9A,0x9A), Color.FromArgb(0xFF,0xCC,0x80),
                Color.FromArgb(0xFF,0xF5,0x7F), Color.FromArgb(0xA5,0xD6,0xA7),
                Color.FromArgb(0x80,0xDE,0xEA), Color.FromArgb(0x90,0xCA,0xF9),
                Color.FromArgb(0xCE,0x93,0xD8)
            };
            if (string.IsNullOrEmpty(s)) return pal[0];
            unchecked { int h = 23; foreach (char c in s) h = h * 31 + c; return pal[Math.Abs(h) % pal.Length]; }
        }

        protected static bool IsHttpUrl(string s)
        {
            return Uri.TryCreate(s, UriKind.Absolute, out var uri) &&
                   (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }
        protected static Bitmap LoadBitmapFromFile(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var ms = new MemoryStream())
            {
                fs.CopyTo(ms); ms.Position = 0;
                using (var tmp = (Bitmap)Image.FromStream(ms, true, true))
                    return new Bitmap(tmp);
            }
        }
        protected static Bitmap LoadBitmapFromBytes(byte[] data)
        {
            using (var ms = new MemoryStream(data, false))
            using (var tmp = (Bitmap)Image.FromStream(ms, true, true))
                return new Bitmap(tmp);
        }
        protected static Image CloneSafe(Image img) => img == null ? null : new Bitmap(img);
        protected void SetIconFromControlOwned(Image img)
        {
            if (_iconOwnedByControl && _icon != null) { try { _icon.Dispose(); } catch { } }
            _icon = img; _iconOwnedByControl = true;
            if (!IsDisposed && IsHandleCreated) Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _iconCts?.Cancel(); _iconCts?.Dispose();
                if (_iconOwnedByControl && _icon != null) { try { _icon.Dispose(); } catch { } _icon = null; }
            }
            base.Dispose(disposing);
        }
    }
}
