using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using NailsChekin.Models.Helper;
using NailsChekin.UserControl;
using NailsChekin.MyControls;

namespace NailsChekin.Models.Reports
{
    public partial class TicketReceipt : DevExpress.XtraReports.UI.XtraReport
    {
        string orderId = "";
        public TicketReceipt()
        {
            InitializeComponent();
        }

        public TicketReceipt(string orderId, string responce)
        {
            InitializeComponent();
            this.orderId = orderId;

            if (string.IsNullOrEmpty(responce))
            {
                responce = MainReport.Receipt_PrinterData(this.orderId);
            }

            if (responce.Trim().Length > 0)
            {
                this.DrawReceipt(responce);
            }
        }

        private void DrawReceipt(string responce)
        {
            try
            {
                JObject jData = JObject.Parse(responce);

                JObject storeInfo = JObject.Parse(jData["storeInfo"].ToString());
                JObject ticketInfo = JObject.Parse(jData["ticketInfo"].ToString());
                JArray items = JArray.Parse(jData["items"].ToString());
                JObject totals = JObject.Parse(jData["totals"].ToString());
                string youEarn = jData["youEarn"].ToString();
                string youSave = jData["youSave"].ToString();
                JObject footerInfo = JObject.Parse(jData["footerInfo"].ToString());

                //storeInfo
                vendorName.Text = storeInfo["name"].ToString().ToUpper();
                vendorAddress.Text = storeInfo["address"].ToString();
                vendorFullAddress.Text = storeInfo["city"].ToString();
                vendorPhone.Text = storeInfo["phone"].ToString();

                dateLabel.Text = ticketInfo["date"].ToString();
                customerLable.Text = "CUSTOMER: " + ticketInfo["customer"].ToString();
                ticketStatusLable.Text = ticketInfo["print_status"].ToString().ToUpper();

                ticketIdLable.Text = "*******************************" + Environment.NewLine + "#" + this.orderId + Environment.NewLine + "*******************************";

                // Item đã refund -> tô đỏ + amount có dấu '-' (dùng chung cho receipt thường & return)
                double refundedSubtotal;
                int number_items = ReceiptRenderHelper.FillItems(detailTable, items, out refundedSubtotal);

                total_service_lable.Text = number_items.ToString();

                string t_title = "SUBTOTAL";
                string t_text = "$" + totals["SUBTOTAL"].ToString();

                string[] data_title = new string[] { "SUBTOTAL:", "DISCOUNT:", "REWARD:", "TAX:", "TOTAL:", "CASH RECEIVED:", "CHARGE:", "CHANGED:" };
                string[] data_value = new string[] { totals["SUBTOTAL"].ToString(), totals["DISCOUNT"].ToString(), totals["REWARD"].ToString(), totals["TAX"].ToString(), totals["TOTAL"].ToString(), totals["CASH"].ToString(), totals["CHARGE"].ToString(), totals["CHANGED"].ToString() };
                for (int i = 0; i < data_title.Length; i++)
                {
                    if (data_value[i].Trim().Length > 0 && i >= 1 )
                    {
                        t_title += Environment.NewLine + data_title[i];
                        t_text += Environment.NewLine + "$" + data_value[i];
                    }
                }

                total_title.Text = t_title;
                total_lable.Text = t_text;

                // Dòng tổng tiền đã hoàn (đỏ) ngay dưới các ô tiền
                ReceiptRenderHelper.AddReturnTotalRow(xrTable2, refundedSubtotal, totals);

                total_you_earn_points_today.Text = "You Earn " + youEarn + " points today";
                total_items_sold.Text = "Items Sold: " + number_items;

                //string footer_text = "THANK YOU FOR SHOPPING AT";
                //footer_text += Environment.NewLine + "" + storeInfo["name"].ToString().ToUpper();
                //footer_text += Environment.NewLine + "PLEASE COME AGAIN";
                //xr_footer_thanks.Text = footer_text;

                if (!string.IsNullOrEmpty(footerInfo["COMMENT"].ToString()))
                {
                    string footer_comment = "***************************************";

                    string comment = footerInfo["COMMENT"].ToString();
                    string[] lines = Regex.Split(comment, "&#x0D;");
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(lines[i].Trim()))
                        {
                            footer_comment += Environment.NewLine + lines[i].Trim();
                        }
                    }

                    footer_comment += Environment.NewLine + "***************************************";
                    xr_footer_comment_node.Text = footer_comment;
                }
                else
                {
                    xr_footer_comment_node.Visible = false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_Printer(ex.Message + Environment.NewLine + ex.StackTrace, "DrawReceipt TicketReceipt Exception - responce: " + responce);
                CustomMessageBox.Show("Print Error: " + ex.Message);
            }
        }

    }

    // Đổ danh sách item vào bảng chi tiết của receipt.
    // Item có "refunded":1 -> tô màu ĐỎ cả dòng + amount hiện dấu '-' (receipt RETURN khi in lại đơn đã hoàn).
    // Item không có cờ refunded (vd receipt bán hàng / receipt lúc refund) -> in bình thường.
    // Tách riêng để dễ nâng cấp mẫu Receipt Return sau này.
    internal static class ReceiptRenderHelper
    {
        // Đổ item vào bảng chi tiết. Trả về tổng số lượng; out refundedSubtotal = tổng LINE amount (pre-tax) của các item đã hoàn.
        public static int FillItems(XRTable detailTable, JArray items, out double refundedSubtotal)
        {
            refundedSubtotal = 0;
            int number_items = 0;
            int stt = 0;
            // Màu gốc của dòng item -> dùng để RESET cho dòng không refund (InsertRowBelow clone cả màu đỏ của dòng trước)
            System.Drawing.Color defaultColor = detailTable.Rows[0].Cells[0].ForeColor;
            foreach (JObject obj in items)
            {
                string item = obj["item"] == null ? "" : obj["item"].ToString();
                string qty = obj["qty"] == null ? "0" : obj["qty"].ToString();
                string price = obj["price"] == null ? "" : obj["price"].ToString();
                string amount = obj["amount"] == null ? "0" : obj["amount"].ToString();
                bool refunded = obj["refunded"] != null && obj["refunded"].ToString().Trim() == "1";

                double q;
                if (double.TryParse(qty, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out q))
                    number_items += (int)Math.Round(q, 0);

                // Cộng theo LINE amount đang in (không dùng refund_amount vì quick item trùng id/tên dễ bị ghi đè)
                if (refunded)
                    refundedSubtotal += ParseNum(amount);

                XRTableRow row;
                if (stt == 0)
                {
                    row = detailTable.Rows[0];
                }
                else
                {
                    detailTable.InsertRowBelow(null);
                    row = detailTable.Rows[stt];
                }

                row.Cells[0].Text = qty + " @ " + price;
                row.Cells[1].Text = item;
                row.Cells[2].Text = (refunded ? "-$" : "$") + amount;

                // LUÔN set màu (đỏ nếu refunded, ngược lại trả về màu gốc) -> tránh dòng clone bị dính đỏ
                System.Drawing.Color color = refunded ? System.Drawing.Color.Red : defaultColor;
                row.Cells[0].ForeColor = color;
                row.Cells[1].ForeColor = color;
                row.Cells[2].ForeColor = color;

                stt++;
            }
            return number_items;
        }

        // Thêm 1 dòng "TOTAL AMOUNT RETURN" màu đỏ dưới bảng totals.
        // returnTotal = tổng tiền hàng đã hoàn + thuế phân bổ (theo tỉ lệ tax/subtotal của đơn).
        public static void AddReturnTotalRow(XRTable totalsTable, double refundedSubtotal, JObject totals)
        {
            if (totalsTable == null || refundedSubtotal <= 0) return;

            double ordSub = ParseNum(totals != null && totals["SUBTOTAL"] != null ? totals["SUBTOTAL"].ToString() : "");
            double ordTax = ParseNum(totals != null && totals["TAX"] != null ? totals["TAX"].ToString() : "");
            double rate = ordSub > 0 ? ordTax / ordSub : 0;
            double returnTotal = Math.Round(refundedSubtotal * (1 + rate), 2);
            if (returnTotal <= 0) return;

            var row = new XRTableRow();
            row.Weight = 1D;

            var titleCell = new XRTableCell();
            titleCell.Text = "TOTAL AMOUNT RETURN";
            titleCell.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            titleCell.ForeColor = System.Drawing.Color.Red;
            titleCell.Multiline = true;
            titleCell.Weight = 1.4093352534058872D;   // khớp total_title

            var valueCell = new XRTableCell();
            valueCell.Text = "-$" + returnTotal.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
            valueCell.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            valueCell.ForeColor = System.Drawing.Color.Red;
            valueCell.Multiline = true;
            valueCell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopRight;
            valueCell.Weight = 0.59066474659411283D;  // khớp total_lable

            row.Cells.AddRange(new XRTableCell[] { titleCell, valueCell });
            totalsTable.Rows.Add(row);
        }

        // Parse chuỗi tiền ("$77", "5.64", "") -> double
        private static double ParseNum(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0;
            s = s.Replace("$", "").Replace(",", "").Trim();
            double d;
            return double.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out d) ? d : 0;
        }
    }
}
