using NailsChekin.Models;
using NailsChekin.Models.Helper;
using NailsChekin.MyControls;
using NailsChekin.Popup;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NailsChekin.UserControl
{
    [DefaultEvent("CardClick")]
    public class UCItemCheckInOut : ItemBaseControl
    {
        // Theme cho card
        [Category("Card/Theme")] public Color CardBaseColor { get; set; } = Color.Orange;
        [Category("Card/Theme")] public Color CardHoverColor { get; set; } = ColorHelper.Question; // Color.FromArgb(245, 248, 255);
        [Category("Card/Theme")] public Color CardPressColor { get; set; } = Color.FromArgb(235, 242, 255);
        [Category("Card/Theme")] public Color CardBorderColor { get; set; } = Color.FromArgb(210, 210, 210);
        [Category("Card/Theme")] public Color CardSelBorderColor { get; set; } = Color.DodgerBlue;

        public bool allow_edit_turn = false;
        public string turnNum = "";
        public int max_turnNum = 0;

        public UCItemCheckInOut()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            Cursor = Cursors.Hand;
            Height = 100;
            MinimumSize = new Size(285, 136);

            // 👉 Toàn bộ xử lý khi click nằm TRONG control này:
            CardActionAsync = _ => DoClickToggleAsync();
        }

        public UCItemCheckInOut(string item_type, string id, string name, string image, string pinCode, string clockIn, string clockInNum, string clockInTime)
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            Cursor = Cursors.Hand;
            Height = 120;
            MinimumSize = new Size(200, 120);

            this.item_type = item_type;
            this.id = id;
            this.name = name;
            this.image = image;
            this.pinCode = pinCode;
            this.color = color.Trim().Length <= 0 ? "Orange" : color;
            this.text_color = text_color.Trim().Length <= 0 ? "Black" : text_color;
            this.clockIn = clockIn;
            this.clockInTime = clockInTime;
            this.clockInNum = clockInNum;

            CardActionAsync = _ => DoClickToggleAsync();
        }

        // Business logic nội bộ: gọi API, rồi quyết định Description mới
        private async Task<string> DoClickToggleAsync()
        {
            if (this.allow_edit_turn)
            {
                //FormEditTurnDetail frm = new FormEditTurnDetail(this, this.id, this.name, this.turnNum, this.clockInNum, this.max_turnNum.ToString());
                //frm.StartPosition = FormStartPosition.CenterScreen;
                //frm.ShowDialog(this);
                //frm.Dispose();

                return "";
            }

            // TODO: call API clock in/out ở đây
            //await Task.Yield(); // demo
            var root = GroupRoot ?? this.Parent ?? this.FindForm();
            await UiBusyHelper.RunWithOverlayAsync(root, async () =>
            {
                if (this.item_type.Equals("empl"))
                    EmplClickAction();
                else
                    NailsClickAction();
            }, overlayColor: (Color?)null, showWaitCursor: true);

            //// ví dụ: toggle giữa 2 trạng thái
            //string cur = (Description ?? "").ToUpperInvariant();
            //string next = cur.Contains("CLOCK IN") ? "CLOCK OUT" : "CLOCK IN";

            //// Nếu bạn có ThemeHelper: đổi màu theo trạng thái
            //ApplyStatusTheme(next);

            //return next; // base sẽ set Description = next và Invalidate()
            return "";
        }

        // (tuỳ chọn) việc muốn chạy SAU khi CardActionAsync xong:
        protected override void OnCardClick()
        {
            // ví dụ: log, hiệu ứng, …
            //base.OnCardClick(); // gọi nếu vẫn muốn cho bên ngoài subscribe được event
        }

        // đặt trong class UCItemCheckInOutRound
        public void ApplyStatusTheme(string status)
        {
            // Lấy palette theo trạng thái (IN/OUT/BREAK/PROCESS/…)
            var p = NailsChekin.Models.Helper.ThemeHelper.MapStatus(status);

            // Áp vào control hiện tại
            CardBaseColor = p.Base;
            CardHoverColor = p.Hover;
            CardPressColor = p.Press;
            CardBorderColor = p.Border;
            CardSelBorderColor = p.SelBorder;

            TitleColor = p.Title;
            SubtitleColor = p.Subtitle;
            DescriptionColor = p.Description;

            if (!IsDisposed) Invalidate();
        }

        public void SetColor()
        {
            if (this.clockIn.Equals("1"))
            {
                Subtitle = "CLOCK OUT";
                CardBaseColor = ColorHelper.Danger;
            }
            else
            {
                Subtitle = "CLOCK IN";
                CardBaseColor = ColorHelper.Warning;
            }

            if (!IsDisposed) Invalidate();
        }

        public void SetAllowEditTurn(bool allow, string turnNum, int max_turnNum)
        {
            this.allow_edit_turn = allow;
            this.turnNum = turnNum;
            this.max_turnNum = max_turnNum;
        }

        public void ReloadTurn()
        {
            //Call Parent Reload
            //if (GroupRoot is FormEditTurn)
            //{
            //    ((FormEditTurn)GroupRoot).InitStaffs();
            //    return;
            //}
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

            int padL = CardPadding.Left, padT = CardPadding.Top, padR = CardPadding.Right, padB = CardPadding.Bottom;
            int iconBox = IconSize;
            int x = rect.Left + padL;
            int y = rect.Top + padT;
            int w = rect.Width - padL - padR;
            int h = rect.Height - padT - padB;

            // Icon
            _iconRect = Rectangle.Empty;
            int textLeft = x;

            _iconRect = new Rectangle(x, (y + (h - iconBox) / 2) + 10, iconBox, iconBox);
            bool drew = DrawAvatar(g, _iconRect);
            if (drew) textLeft = _iconRect.Right + IconTextGap;

            // Text
            using (var fTitle = new Font(Font.FontFamily, TitleSize, FontStyle.Bold))
            using (var fSub = new Font(Font.FontFamily, SubtitleSize, FontStyle.Regular))
            using (var fDesc = new Font(Font.FontFamily, DescSize, FontStyle.Regular))
            {
                int availW = w - (textLeft - x);
                int curY = y + TextTopPadding;

                if (_iconRect != Rectangle.Empty && AlignTextToIcon)
                {
                    int hTitle = MeasureLine(g, fTitle);
                    // Subtitle multi-line đo theo width
                    var subFlags = TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl | TextFormatFlags.NoPadding;
                    Size subSize = TextRenderer.MeasureText(g, _subtitle ?? "", fSub, new Size(availW, int.MaxValue), subFlags);
                    int blockH = hTitle + LineSpacing + subSize.Height;
                    int targetTop = _iconRect.Top + Math.Max(0, (iconBox - blockH) / 2);
                    curY = Math.Max(y + TextTopPadding, targetTop);
                }

                // === Title (giữ nguyên cách vẽ của bạn) ===
                int hTitleLn = MeasureLine(g, fTitle);
                TextRenderer.DrawText(g, _title ?? "", fTitle,
                    new Rectangle(10, 5, (w-x), hTitleLn),
                    TitleColor,
                    TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
                int titleBottom = hTitleLn + LineSpacing; // curY + hTitleLn + LineSpacing;

                // === Description: neo phải & sát bottom (giữ như bạn đang dùng) ===
                int rightPad = 2, bottomPad = 2;
                var flagsDesc = TextFormatFlags.Right | TextFormatFlags.WordBreak |
                                TextFormatFlags.TextBoxControl | TextFormatFlags.NoPadding;

                Size descSize = TextRenderer.MeasureText(g, _description ?? "", fDesc,
                    new Size(availW - rightPad, int.MaxValue), flagsDesc);
                int descTop = Math.Max((y + h) - descSize.Height - bottomPad, titleBottom);
                var descRect = new Rectangle(textLeft, descTop, availW - rightPad, Math.Max(0, (y + h) - descTop));

                // === SUBTITLE: tối đa 3 dòng, căn GIỮA DỌC, NGANG CĂN TRÁI (mặc định) ===
                var flagsSub = TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl |
                               TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis; // không HorizontalCenter → mặc định căn trái

                // đo kích thước hiện tại
                Size subMeasured = TextRenderer.MeasureText(g, _subtitle ?? "", fSub, new Size(availW, int.MaxValue), flagsSub);

                // giới hạn tối đa 3 dòng + vừa khoảng trống giữa Title và Description
                int lineHSub = MeasureLine(g, fSub);
                int gapTop = titleBottom;
                int gapBottom = descTop - LineSpacing;
                int gapHeight = Math.Max(0, gapBottom - gapTop);
                int subMaxH = Math.Min(lineHSub * 3, gapHeight);

                // (tuỳ chọn) co chữ nhẹ nếu vượt quá 3 dòng/không đủ chỗ
                float subPx = fSub.Size;
                float minPx = Math.Max(8f, subPx - 6f);
                while (subMeasured.Height > subMaxH && subPx > minPx)
                {
                    subPx -= 0.5f;
                    using (var fTmp = new Font(fSub.FontFamily, subPx, fSub.Style, fSub.Unit))
                        subMeasured = TextRenderer.MeasureText(g, _subtitle ?? "", fTmp, new Size(availW, int.MaxValue), flagsSub);
                }
                // dùng font cuối cùng (tạo 1 lần để vẽ)
                using (var fSubDraw = new Font(fSub.FontFamily, subPx, fSub.Style, fSub.Unit))
                {
                    int subDrawH = Math.Min(subMeasured.Height, subMaxH);
                    int subTop = gapTop + Math.Max(0, (gapHeight - subDrawH) / 2); // ← căn giữa theo CHIỀU DỌC
                   
                    //Fix căn giữa ICON
                    subTop = (_iconRect.Top + (_iconRect.Height / 2)) - subDrawH + 10;

                    TextRenderer.DrawText(g, _subtitle ?? "", fSubDraw,
                        new Rectangle(textLeft, subTop, availW, subDrawH),
                        SubtitleColor, flagsSub);
                }

                // === Vẽ Description sau cùng ===
                TextRenderer.DrawText(g, _description ?? "", fDesc, descRect, DescriptionColor, flagsDesc);

            }
        }


        // My Business
        public Control GroupRoot { get; set; }   // gán từ form cha
        public string item_type = "";

        public string id = "";
        public string name = "";
        public string pinCode = "";
        public string image = "";
        public string color = "";
        public string text_color = "";
        public string specializeIns = "";
        public string clockIn = "";
        public string clockInTime = "";
        public string clockInNum = "";

        private void NailsClickAction()
        {
            //if (GroupRoot is FormSelectStaff)
            //{
            //    ((FormSelectStaff)GroupRoot).SetSelectedStaff(id, name);
            //    return;
            //}

            ////CheckIn - OUT
            //if (this.pinCode.Trim().Length < 4)
            //{
            //    CustomMessageBox.Show("PIN Code Not Correct");
            //    return;
            //}

            //if (clockIn.Equals("0"))
            //{
            //    //Đổi màu trước để đỡ cảm giác đợi, fail thì nhả lại
            //    this.clockIn = "1";
            //    this.SetColor();
               
            //    string jData = "{";
            //    jData += "'pinCode':'" + this.pinCode + "', ";
            //    jData += "'current_local_time':'" + DateTimeHelper.Get_Local_DateTime() + "', ";
            //    jData += "'from_source':'POS' ";
            //    jData += "}";

            //    string response = ApiHelper.CALL_API("Staff/check-in", jData);
            //    if (response.ToUpper().StartsWith("ERROR"))
            //    {
            //        this.clockIn = "0";
            //        this.SetColor();

            //        CustomMessageBox.Show(response);
            //    }
            //    else
            //    {
            //        //Reload History
            //        //((TabCheckIn)this.Parent.Parent).ReloadHistory("nails");
            //    }
            //}
            //else  //Clock out
            //{
            //    this.clockIn = "0";
            //    this.SetColor();

            //    string jData = "{";
            //    jData += "'pinCode':'" + this.pinCode + "', ";
            //    jData += "'version':'" + Constants.version_id + "', ";
            //    jData += "'current_local_time':'" + DateTimeHelper.Get_Local_DateTime() + "', ";
            //    jData += "'from_source':'POS' ";
            //    jData += "}";

            //    string response = ApiHelper.CALL_API("Staff/check-out", jData);
            //    if (!Utilitys.IsValidJson(response))
            //    {
            //        this.clockIn = "1";
            //        this.SetColor();

            //        CustomMessageBox.Show(response);
            //    }
            //    else
            //    {
            //        Task.Run(() =>
            //        {
            //            try
            //            {
            //                jData = "{";
            //                jData += "'pinCode':'" + this.pinCode + "', ";
            //                jData += "'date':'" + DateTime.Parse(DateTimeHelper.Get_Local_DateTime()).ToString("yyyy-MM-dd") + "' ";
            //                jData += "}";

            //                response = ApiHelper.CALL_API("Report/clockOut-data-print", jData);
            //                if (Utilitys.IsValidJson(response))
            //                {
            //                    JArray jDetails = JArray.Parse(response);
            //                    foreach (var item in jDetails)
            //                    {
            //                        //PrinterLocalHelper.PrintDirectDailySaleForNails(item.ToString());
            //                    }
            //                }
            //            }
            //            catch { }
            //        });
            //    }
            //}
        }

        private void EmplClickAction()
        {
            //if (GroupRoot is FormSelectStaff)
            //{
            //    ((FormSelectStaff)GroupRoot).SetSelectedStaff(id, name, true);
            //    return;
            //}

            //if (this.pinCode.Trim().Length < 4)
            //{
            //    CustomMessageBox.Show("PIN Code Not Correct");
            //    return;
            //}

            //if (clockIn.Equals("0"))
            //{
            //    //Đổi màu trước để đỡ cảm giác đợi, fail thì nhả lại
            //    this.clockIn = "1";
            //    this.SetColor();

            //    string jData = "{";
            //    jData += "'pinCode':'" + this.pinCode + "', ";
            //    jData += "'current_local_time':'" + DateTimeHelper.Get_Local_DateTime() + "', ";
            //    jData += "'from_source':'POS' ";
            //    jData += "}";

            //    string response = ApiHelper.CALL_API("User/check-in", jData);
            //    if (response.ToUpper().StartsWith("ERROR"))
            //    {
            //        this.clockIn = "0";
            //        this.SetColor();

            //        CustomMessageBox.Show(response);
            //    }
            //    else
            //    {
            //        //Reload History
            //        ((TabCheckIn)this.Parent.Parent).ReloadHistory("empl");
            //    }
            //}
            //else  //Clock out
            //{
            //    this.clockIn = "0";
            //    this.SetColor();

            //    string jData = "{";
            //    jData += "'pinCode':'" + this.pinCode + "', ";
            //    jData += "'current_local_time':'" + DateTimeHelper.Get_Local_DateTime() + "', ";
            //    jData += "'from_source':'POS' ";
            //    jData += "}";

            //    string response = ApiHelper.CALL_API("User/check-out", jData);
            //    if (response.ToUpper().StartsWith("ERROR"))
            //    {
            //        this.clockIn = "1";
            //        this.SetColor();

            //        CustomMessageBox.Show(response);
            //    }
            //    else
            //    {
            //        //Reload History
            //        ((TabCheckIn)this.Parent.Parent).ReloadHistory("empl");

            //        Task.Run(() =>
            //        {
            //            try
            //            {
            //                jData = "{";
            //                jData += "'pinCode':'" + this.pinCode + "', ";
            //                jData += "'date':'" + DateTime.Parse(DateTimeHelper.Get_Local_DateTime()).ToString("yyyy-MM-dd") + "' ";
            //                jData += "}";

            //                response = ApiHelper.CALL_API("Report/empl-clockOut-data-print", jData);
            //                if (Utilitys.IsValidJson(response))
            //                {
            //                    PrinterLocalHelper.PrintDirectEmplCheckOut(response);                                
            //                }
            //            }
            //            catch { }
            //        });

            //    }
            //}
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // UCItemCheckInOutRound
            // 
            this.Name = "UCItemCheckInOutRound";
            this.Size = new System.Drawing.Size(200, 120);
            this.ResumeLayout(false);

        }
    }
}
