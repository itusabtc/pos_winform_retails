using NailsChekin.Models.Implements;
using NailsChekin.Models.ListModel;
using NailsChekin.UserControl;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NailsChekin.Models.Helper
{
    public class OrderHelper
    {
        public static void Add_TicketItems_ToPaymentCard(string ticketId, bool isInService, bool isCombine, KineticScrollPanel panel, ref string meta)
        {
            //string jData = "{";
            //jData += "'ticket_id':'" + ticketId + "', ";
            //jData += "'is_combine':'" + (isCombine ? "1" : "0") + "' ";
            //jData += "}";

            //string response = ApiHelper.CALL_API("Order/info", jData);
            //if (Utilitys.IsValidJson(response))
            //{
            //    JObject jResponce = JObject.Parse(response);
            //    meta = jResponce["meta"].ToString();

            //    JArray jsonData = JArray.Parse(jResponce["info"].ToString());
            //    foreach (JObject item in jsonData)
            //    {
            //        string staff_id = item["staffId"].ToString();
            //        string staff_name = item["staffName"].ToString();
            //        string service_id = item["id"].ToString();
            //        string service_name = item["name"].ToString();
            //        string service_price = item["price"].ToString();
            //        string service_priceCC = item["price"].ToString();
            //        string quantity = item["quantity"].ToString();
            //        string service_length = item["service_length"] == null ? "0" : item["service_length"].ToString();
            //        string color = item["color"] == null ? "" : item["color"].ToString();
            //        string discount = item["discount"].ToString();
            //        string discount_percent_owner = item["discount_percent_owner"] == null ? "100" : item["discount_percent_owner"].ToString();
            //        string tip = item["tip"].ToString();
            //        string isQuickMenu = item["isQuickMenu"].ToString();
            //        string catalog = item["catalog"].ToString();
            //        string cashTip = item["cashTip"].ToString();
            //        string saleId = item["ticketId"].ToString();

            //        string cart_id = item["cart_id"] == null ? "" : item["cart_id"].ToString();
            //        string staffDone = item["staffDone"] == null ? "0" : item["staffDone"].ToString();
            //        string is_start = item["is_start"] == null ? "0" : item["is_start"].ToString();
            //        string start_time = item["start_time"] == null ? "" : item["start_time"].ToString();

            //        string add_on_fee = item["add_on_fee"] == null ? "0" : item["add_on_fee"].ToString();
            //        string ticket_repair_id = item["ticket_repair_id"] == null ? "0" : item["ticket_repair_id"].ToString();
            //        string original_cart_item_id = item["original_cart_item_id"] == null ? "0" : item["original_cart_item_id"].ToString();
            //        bool isRepair = !ticket_repair_id.Equals("0") && ticket_repair_id.Trim().Length > 0 && !original_cart_item_id.Equals("0");

            //        bool add_item_to_cart = true;
            //        if (isQuickMenu.Equals("1") || service_name.ToUpper().Equals("ANY"))
            //        {
            //            if (!Core.ALLOW_ADD_QUICKMENU_TO_CART() && (string.IsNullOrEmpty(staff_id) || staff_id.Equals("0") || staff_name.ToUpper().StartsWith("ANY") ) )  //Nếu không có thợ thì không add line quick menu
            //            {
            //                add_item_to_cart = false;
            //            }
            //        }

            //        //Add Item TO Cart
            //        if (add_item_to_cart)
            //        {
            //            bool flag = checkItemInCard(staff_id, service_id, ticketId, panel);
            //            if (!flag)
            //            {
            //                UCCartItem cardItem = new NailsChekin.UserControl.UCCartItem(cart_id, staff_id, staff_name, service_id, service_name, service_price, service_priceCC, service_length, quantity, color, color, false, (isQuickMenu.Equals("1") ? true : false));
            //                cardItem.setDiscount(discount);
            //                cardItem.setDiscountPercentOwner(discount_percent_owner);
            //                cardItem.setCatalog(catalog);
            //                cardItem.setTip(tip);
            //                cardItem.setCashTip(cashTip);
            //                cardItem.setTicketId(isCombine ? saleId : ticketId);
            //                cardItem.SetInService(isInService, staffDone, is_start, start_time);
            //                cardItem.SetColor(color);

            //                if (isRepair)
            //                    cardItem.SetRepairInfo(ticket_repair_id, add_on_fee, original_cart_item_id);

            //                cardItem.Width = panel.Width - 8;
            //                panel.Content.Controls.Add(cardItem);
            //            }
            //        }
            //    }
            //}
        }

        public static bool checkItemInCard(string staff_id, string service_id, string ticket_id, Control panel)
        {
            //foreach (UCCartItem control in panel.Controls.OfType<UCCartItem>())
            //{
            //    if (control.staff_id.Equals(staff_id) && control.service_id.Equals(service_id) && control.ticketId.Equals(ticket_id))
            //    {
            //        return true;
            //    }
            //}

            return false;
        }

        public static double Get_Cash_Amount(List<PaymentModel> paymentList)
        {
            double total_cash_amount = 0;
            for (int i = 0; i < paymentList.Count; i++)
            {
                if (paymentList[i].type.ToUpper().Equals("CASH"))
                {
                    total_cash_amount += paymentList[i].amount;
                }
            }
            return Math.Round(total_cash_amount, 2);
        }

        //Hàm này tính Cash Discount FULL có ban đầu
        public static double Get_Cash_Discount(double amt_due, double tender)
        {
            double cash_discount = 0;
            double cash_discount_percent = Core.CASH_DISCOUNT_PERCENT();

            if (tender <= amt_due)
            {
                cash_discount = tender * cash_discount_percent / 100.0;
            }
            else //Discount trên số tiền ticket thôi
            {
                cash_discount = amt_due * cash_discount_percent / 100.0;
            }

            return Math.Round(cash_discount, 2);
        }

        public static double Get_Cash_Discount_Service(double total_amount, double amt_due, double tender)
        {
            double cash_discount = 0;
            double cash_discount_percent = Core.CASH_DISCOUNT_PERCENT();

            cash_discount = total_amount * cash_discount_percent / 100.0;
            return Math.Round(cash_discount, 2);
        }

        public static double Get_Cash_Discount_Product(double total_product_amount, double amt_due, double tender)
        {
            double cash_discount_product = 0;
            double cash_discount_product_percent = Core.CASH_DISCOUNT_PRODUCT_PERCENT();

            cash_discount_product = total_product_amount * cash_discount_product_percent / 100.0;
            return Math.Round(cash_discount_product, 2);
        }

        public static double Get_Cash_Received(List<PaymentModel> paymentList)
        {
            double total_cash_received = 0;
            for (int i = 0; i < paymentList.Count; i++)
            {
                if (paymentList[i].type.ToUpper().Equals("CASH"))
                {
                    total_cash_received += paymentList[i].cash_received;
                }
            }
            return Math.Round(total_cash_received, 2);
        }

        public static double Get_Cash_AmtDue(List<PaymentModel> paymentList)
        {
            double cash_amtDue = 0;
            for (int i = 0; i < paymentList.Count; i++)
            {
                if (paymentList[i].type.ToUpper().Equals("CASH"))
                {
                    cash_amtDue += paymentList[i].amt_due;
                }
            }
            return Math.Round(cash_amtDue, 2);
        }

        public static double Get_ChangeDue(List<PaymentModel> paymentList)
        {
            double change_due = 0;
            for (int i = 0; i < paymentList.Count; i++)
            {
                if (paymentList[i].type.ToUpper().Equals("CASH"))
                {
                    double cash_request = paymentList[i].amount + paymentList[i].cash_discount;
                    change_due += paymentList[i].cash_received - cash_request;
                }
            }
            return Math.Round(change_due, 2);
        }

        public static List<PaymentModel> Check_Payment_Method(List<PaymentModel> paymentList, string method, double amount)
        {
            //Check nếu chưa có thì add vô
            bool exits = false;
            for (int i = 0; i < paymentList.Count; i++)
            {
                if (paymentList[i].type.ToUpper().Equals(method.ToUpper()))
                {
                    exits = true;
                    break;
                }
            }

            if (!exits)
            {
                paymentList.Add(new PaymentModel(method.ToUpper(), amount));
            }

            return paymentList;
        }

        public static double Get_Service_Amount(Control panel, double amt_due, double tender)
        {
            //Tính tỉ lệ dựa trên Tender ( số tiền bấm thanh toán ) => xử lý payment nhiều lần
            //Tender đang là total cart bao gồm cả service và product 

            double total_service_amount = 0;
            double total_product_amount = 0;
            double total_amount = 0;
            //foreach (UCCartItem control in panel.Controls.OfType<UCCartItem>())
            //{
            //    total_product_amount += double.Parse(string.IsNullOrEmpty(control.price) ? "0" : control.price);

            //    total_amount += double.Parse(string.IsNullOrEmpty(control.price) ? "0" : control.price);
            //}

            //double tile = Math.Round( total_service_amount / total_amount, 3);
            //return Math.Round((tender * tile), 2);

            total_service_amount = total_amount - Get_Product_Amount(panel, amt_due, tender); //Ưu tiên service amount phải khớp số lẻ 100%
            return Math.Round(total_service_amount, 2);
        }

        public static double Get_Product_Amount(Control panel, double amt_due, double tender)
        {
            //Tính tỉ lệ dựa trên Tender ( số tiền bấm thanh toán ) => xử lý payment nhiều lần
            //Tender đang là total cart bao gồm cả service và product 

            double total_product_amount = 0;
            double total_amount = 0;
            //foreach (UCCartItem control in panel.Controls.OfType<UCCartItem>())
            //{
            //    //if (control.IsItemProduct())
            //    //{
            //    //    total_product_amount += double.Parse(string.IsNullOrEmpty(control.price) ? "0" : control.price);
            //    //}

            //    total_amount += double.Parse(string.IsNullOrEmpty(control.price) ? "0" : control.price);
            //}

            double tile = Math.Round(total_product_amount / total_amount, 3);
            return Math.Round((tender * tile), 2);
        }

        public static double Get_Product_Amount_OLD(Control panel, double amt_due, double tender)
        {
            double total_amount = 0;
            //foreach (UCCartItem control in panel.Controls.OfType<UCCartItem>())
            //{
            //    //if (control.IsItemProduct())
            //    //{
            //    //    total_amount += double.Parse(string.IsNullOrEmpty(control.price) ? "0" : control.price);
            //    //}
            //}
            return Math.Round(total_amount, 2);
        }


    }
}
