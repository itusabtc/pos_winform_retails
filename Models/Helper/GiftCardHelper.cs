using NailsChekin.UserControl;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace NailsChekin.Models.Helper
{
    class GiftCardHelper
    {
        public static string CheckOut(string jItems, string pay_amount, string meta_data)
        {
            try
            {
                string jData = "{";
                jData += "'pay_amount':'" + pay_amount + "',";
                jData += "'jItems':" + jItems + ",";
                jData += "'meta_data':" + meta_data + ",";
                jData += "'local_time':'" + DateTimeHelper.Get_Local_DateTime() + "' ";
                jData += "}";

                var response = ApiHelper.CALL_API("GiftCard/checkout", jData);
                if (!Utilitys.IsValidJson(response))
                {
                    return "Error: " + response; //Error Message
                }
                return response;
            }
            catch (Exception exx)
            {
                return "Error: " + exx.Message;
            }
        }

        public static string Info(string gift_card_no, string customer_phone)
        {
            try
            {
                string jData = "{";
                jData += "'gift_card_no':'" + gift_card_no + "',";
                jData += "'customer_phone':" + customer_phone + ",";
                jData += "'local_time':'" + DateTimeHelper.Get_Local_DateTime() + "' ";
                jData += "}";

                var response = ApiHelper.CALL_API("GiftCard/info", jData);
                if (!Utilitys.IsValidJson(response))
                {
                    return "Error: " + response; //Error Message
                }
                return response;
            }
            catch (Exception exx)
            {
                return "Error: " + exx.Message;
            }
        }

        public static string PrintData(string gift_card_token)
        {
            try
            {
                string jData = "{";
                jData += "'gift_card_token':'" + gift_card_token + "',";
                jData += "'local_time':'" + DateTimeHelper.Get_Local_DateTime() + "' ";
                jData += "}";

                var response = ApiHelper.CALL_API("GiftCard/print-data", jData);
                if (!Utilitys.IsValidJson(response))
                {
                    return "Error: " + response; //Error Message
                }
                return response;
            }
            catch (Exception exx)
            {
                return "Error: " + exx.Message;
            }
        }

        public static void RenderPaymentCart(Control panel, string jItems)
        {
            panel.Controls.Clear();
            //if (Utilitys.IsValidJson(jItems))
            //{
            //    int locationY = 5;
            //    foreach (JObject item in JArray.Parse(jItems))
            //    {
            //        string id = "";
            //        string card_no = item.GetValue("card_no").ToString();
            //        string value = item.GetValue("value").ToString();
            //        string balance = "";
            //        string expiry_type = item.GetValue("expiry_type").ToString();
            //        string expiry = item.GetValue("expiry").ToString();
            //        string cusPhone = item.GetValue("cusPhone").ToString();

            //        UCViewGiftCardItem control = new UCViewGiftCardItem(id, card_no, value, balance, expiry_type, expiry, cusPhone);

            //        control.Width = panel.Width;
            //        control.Location = new Point(0, locationY);
            //        locationY += control.Height + 5;
            //        panel.Controls.Add(control);
            //    }
            //}
        }

    }
}
