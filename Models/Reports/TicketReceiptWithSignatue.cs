using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.IO;
using DevExpress.XtraPrinting;
using NailsChekin.UserControl;
using NailsChekin.Models.Helper;
using NailsChekin.MyControls;

namespace NailsChekin.Models.Reports
{
    public partial class TicketReceiptWithSignatue : DevExpress.XtraReports.UI.XtraReport
    {
        string orderId = "";
        public TicketReceiptWithSignatue()
        {
            InitializeComponent();
        }

        public TicketReceiptWithSignatue(string orderId, string responce)
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

                int count = 1;
                int number_items = 0;
                int stt = 0;
                foreach (JObject obj in items)
                {
                    string item = obj["item"].ToString();
                    string qty = obj["qty"].ToString();
                    string price = obj["price"].ToString();
                    string amount = obj["amount"].ToString();
                    number_items += (int)Math.Round(double.Parse(qty), 0);

                    if (stt == 0)
                    {
                        detailTable.Rows[0].Cells["qty"].Text = qty.ToString() + " @ " + price;
                        detailTable.Rows[0].Cells["service"].Text = item;
                        detailTable.Rows[0].Cells["total"].Text = "$" + amount;
                    }
                    else
                    {
                        detailTable.InsertRowBelow(null);
                        detailTable.Rows[stt].Cells[0].Text = qty.ToString() + " @ " + price;
                        detailTable.Rows[stt].Cells[1].Text = item;
                        detailTable.Rows[stt].Cells[2].Text = "$" + amount;
                    }

                    stt++;
                }

                total_service_lable.Text = number_items.ToString();

                string t_title = "SUBTOTAL";
                string t_text = "$" + totals["SUBTOTAL"].ToString();

                string[] data_title = new string[] { "SUBTOTAL:", "DISCOUNT:", "REWARD:", "TAX:", "TOTAL:", "CASH RECEIVED:", "CHARGE:", "CHANGED:" };
                string[] data_value = new string[] { totals["SUBTOTAL"].ToString(), totals["DISCOUNT"].ToString(), totals["REWARD"].ToString(), totals["TAX"].ToString(), totals["TOTAL"].ToString(), totals["CASH"].ToString(), totals["CHARGE"].ToString(), totals["CHANGED"].ToString() };
                for (int i = 0; i < data_title.Length; i++)
                {
                    if (data_value[i].Trim().Length > 0 && i >= 1)
                    {
                        t_title += Environment.NewLine + data_title[i];
                        t_text += Environment.NewLine + "$" + data_value[i];
                    }
                }

                total_title.Text = t_title;
                total_lable.Text = t_text;

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

                //Phần thông tin thanh toán và chữ ký
                string _payment_info = jData["payment_info"] == null ? "" : jData["payment_info"].ToString();
                if (Utilitys.IsValidJson(_payment_info))
                {
                    JObject payment_info = JObject.Parse(_payment_info);

                    xrCC_TransNo.Text = "TRANS#: " + ( payment_info["trans_no"] == null ? "" : payment_info["trans_no"].ToString() );
                    xrCC_Type.Text = payment_info["pay_method_id"] == null ? "" : payment_info["pay_method_id"].ToString().ToUpper();
                    xrCC_Network.Text = this.GetNetWork(payment_info["card_network_type"] == null ? "" : payment_info["card_network_type"].ToString());
                    xrCC_CardNo.Text = payment_info["card_no"] == null ? "" : payment_info["card_no"].ToString();
                    xrCC_AuthCode.Text = payment_info["auth_code"] == null ? "" : payment_info["auth_code"].ToString();

                    string signature_base64 = payment_info["signature_base64"] == null ? "" : payment_info["signature_base64"].ToString();
                    if (!string.IsNullOrEmpty(signature_base64))
                        SetPictureFromBase64(xrPicSignature, signature_base64);
                    else
                        xrPicSignature.Visible = false;
                }
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_Printer(ex.Message + Environment.NewLine + ex.StackTrace, "DrawReceipt TicketReceiptWithSignatue Exception - responce: " + responce);
                CustomMessageBox.Show("Print Error: " + ex.Message);
            }
        }

        private string GetNetWork(string card_network_type)
        {
            if (card_network_type.Equals("1"))
                return "DEBIT";
            if (card_network_type.Equals("3"))
                return "EBT";
            if (card_network_type.Equals("4"))
                return "GIFT CARD";
            //2
            return "CREDIT";
        }

        public void SetPictureFromBase64(XRPictureBox pic, string base64)
        {
            if (string.IsNullOrWhiteSpace(base64))
            {
                pic.Image = null;
                return;
            }

            // Nếu base64 dạng: data:image/png;base64,....
            int comma = base64.IndexOf(',');
            if (comma > 0 && base64.Substring(0, comma).Contains("base64"))
                base64 = base64.Substring(comma + 1);

            byte[] bytes = Convert.FromBase64String(base64);

            using (var ms = new MemoryStream(bytes))
            using (var img = Image.FromStream(ms))
            {
                pic.Image = (Image)img.Clone(); // clone để không phụ thuộc stream
            }

            pic.Sizing = ImageSizeMode.ZoomImage;
        }

    }
}
