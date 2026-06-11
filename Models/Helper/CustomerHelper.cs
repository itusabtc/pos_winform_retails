using NailsChekin.UserControl;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NailsChekin.Models.Helper
{
    public class CustomerHelper
    {
        public static void Draw_Customers_ByTab(Control panel, string customer_selected, string ticket_selected, string tab_name, ref int number_waiting, ref int number_inservice, ref int number_payment)
        {
            //try
            //{
            //    //Remove ALL
            //    NailsChekin.Models.Core.ClearAndDisposeV2(panel);
               
            //    int stt = 0;
            //    int locationX = 0;
            //    int locationY = 5;
            //    int num_coloumn = LayoutHelper.number_coloumn_customer;

            //    string jData = Get_CustomerJSON("");
            //    if (!Utilitys.IsValidJson(jData))
            //        return;

            //    JArray jArray = JArray.Parse(jData);
            //    List<JToken> results = new List<JToken>();

            //    number_waiting = jArray.Where(obj => (string)obj["type"] == "waiting_list").ToList().Count;
            //    number_inservice = jArray.Where(obj => (string)obj["type"] == "in_service").ToList().Count;
            //    number_payment = jArray.Where(obj => (string)obj["type"] == "pending_payment").ToList().Count;

            //    if (tab_name.ToUpper().Equals("PENDING PAYMENT"))
            //        results = jArray.Where(obj => (string)obj["type"] == "pending_payment").ToList();
            //    else if (tab_name.ToUpper().Equals("IN SERVICE"))
            //        results = jArray.Where(obj => (string)obj["type"] == "in_service").ToList();
            //    else //DEFAULT TAB WAITING
            //        results = jArray.Where(obj => (string)obj["type"] == "waiting_list").ToList();

            //    foreach (JObject item in results)
            //    {
            //        string type = item["type"].ToString();
            //        string cusId = item["cusId"].ToString();
            //        string name = item["name"].ToString();
            //        string phone = item["phone"].ToString();
            //        string ranking = item["isVIP"].Equals("1") ? "VIP" : "Silver";
            //        string discount = item["discount"].ToString();

            //        string staffId = item["staffId"].ToString();
            //        string serviceId = item["serviceId"].ToString();
            //        string serviceName = item["serviceName"].ToString();
            //        string serviceAmount = item["serviceAmount"].ToString();
            //        string paid = item["paid"] == null ? "0" : item["paid"].ToString();
            //        string staffName = item["staffName"].ToString().ToUpper();

            //        string checkInNum = item["checkInNum"].ToString();
            //        string customer_checkIn = item["customer_checkIn"].ToString();

            //        string isCombine = item["isCombine"].ToString();
            //        string combine_token = item["combine_token"].ToString();

            //        int num_sale = int.Parse(item["num_sale"].ToString());
            //        string box_title = (num_sale > 0 ? "Frequent Customer" : "New Customer");
            //        if (type.Equals("pending_payment"))
            //            box_title = "PENDING PAYMENT";
            //        else if (type.Equals("in_service"))
            //            box_title = "IN SERVICE";

            //        string ticketId = item["ticketId"].ToString();
            //        string appoinmentId = item["appoinmentId"].ToString();
            //        string count_appt_current = item["count_appt_current"] == null ? "0" : item["count_appt_current"].ToString();

            //        string imgURL = "/ClientWeb/FileUpload/" + item["avatar"].ToString();
            //        if (item["avatar"].ToString().Trim().Length <= 0)
            //            imgURL = "pos_files/male.png";

            //        string signInTime = item["local_time"].ToString();
            //        string timer = "";
            //        bool start_countdount = false;
            //        if (type.Equals("in_service"))  //Count Down timer
            //        {
            //            string is_start = item["is_start"] == null ? "0" : item["is_start"].ToString();
            //            string start_time = item["start_time"] == null ? "" : item["start_time"].ToString();
            //            string total_minute = item["total_minute"] == null ? "30" : item["total_minute"].ToString();
            //            if (is_start.Equals("1") && !string.IsNullOrEmpty(start_time))  //Finizaline mới chạy Countdown
            //            {
            //                DateTime time_end = DateTime.Parse(start_time).AddMinutes(int.Parse(total_minute));

            //                int remain_second = (int)(time_end - DateTime.Now).TotalSeconds;
            //                if (remain_second > 0)
            //                {
            //                    string minute_display = "";
            //                    string second_display = "";

            //                    int _minutes = remain_second / 60;
            //                    int _second = remain_second - _minutes * 60;

            //                    minute_display = (_minutes < 10 ? "0" : "") + _minutes.ToString();
            //                    second_display = (_second < 10 ? "0" : "") + _second.ToString();

            //                    timer = (minute_display + ":" + second_display);
            //                    start_countdount = true;
            //                }
            //            }
            //            else
            //            {
            //                //int remain_second = int.Parse(item["remain_second"].ToString());
            //                //string minute_display = "";
            //                //string second_display = "";

            //                //int _minutes = remain_second / 60;
            //                //int _second = remain_second - _minutes * 60;

            //                //minute_display = (_minutes < 10 ? "0" : "") + _minutes.ToString();
            //                //second_display = (_second < 10 ? "0" : "") + _second.ToString();

            //                //timer = (minute_display + ":" + second_display);
            //            }
            //        }
            //        else if (type.Contains("waiting") && int.Parse(count_appt_current) >= 1)  //Nếu check in chọn YES nhưng không có trong appt book thì không đổi màu
            //        {
            //            if ((!appoinmentId.Equals("0") && item["haveAppt"].ToString().Equals("1")) || customer_checkIn.Equals("1"))
            //            {
            //                timer = DateTimeHelper.get_format_time(item["appoinmentTime"].ToString());
            //            }
            //        }

            //        //Add Control
            //        var customer = new UCCustomer(cusId, name, phone, checkInNum, ranking, discount, staffName, serviceName, serviceAmount, signInTime, timer,
            //                                    box_title.ToUpper(), "", "", ticketId, appoinmentId, customer_checkIn, isCombine, combine_token, start_countdount);
            //        customer.Width = LayoutHelper.coloumn_customer_width;
            //        customer.Location = new Point(locationX, locationY);
            //        if (type.Equals("pending_payment") || type.Equals("in_service"))
            //        {
            //            //To Done / Cancel
            //            customer.staffId = staffId;
            //            customer.serviceId = serviceId;

            //            if (type.Equals("pending_payment"))
            //                customer.SetPaided(staffName, serviceAmount, paid);
            //        }
            //        else
            //        {
            //            customer.SetTicketCheckInId(item.GetValue("ticket_checkin_id") == null ? "0" : item.GetValue("ticket_checkin_id").ToString());
            //        }

            //        panel.Controls.Add(customer);
            //        if (stt > 0 && (stt + 1) % num_coloumn == 0)
            //        {
            //            locationY += (customer.Height + 5);
            //            locationX = 0;
            //        }
            //        else
            //        {
            //            locationX += (customer.Width + 5);
            //        }

            //        stt++;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.SaveLOG_Crash("Message: " + ex.Message + "\nStack Trace:" + ex.StackTrace, "Init_Customers Exception");
            //}
        }

        public static string Get_CustomerJSON(string type)
        {
            if (type.Equals("WAITING LIST"))
                type = "waiting_list";
            else if (type.Equals("WAITING LIST NO APPT"))
                type = "waiting_list_no_appt";
            else if (type.Equals("WAITING LIST YES APPT"))
                type = "waiting_list_yes_appt";
            else if (type.Equals("WAITING LIST APPT"))
                type = "waiting_list_appt";
            else if (type.Equals("IN SERVICE"))
                type = "in_service";
            else if (type.Equals("PENDING PAYMENT"))
                type = "pending_payment";
            else if (type.Equals("IN SERVICE NEW"))
                type = "in_service_new";
            else if (type.Equals("PENDING PAYMENT NEW"))
                type = "pending_payment_new";
            else if (type.Equals("PENDING PAYMENT COMBINE"))
                type = "pending_payment_with_combine";
            else if (type.Equals("INSERVICE AND PENDING PAYMENT"))
                type = "inservice_and_pending_payment";
            else
                type = "all";

            string jData = "{";
            jData += "'date':'" + DateTimeHelper.Get_Local_DateTime() + "', ";
            jData += "'type':'" + type + "' ";
            jData += "}";

            string response = ApiHelper.CALL_API("Customer/list", jData);
            if (Utilitys.IsValidJson(response))
                return response;

            return "";
        }

        public static void Init_Customer_History_CheckIn_Out(Control panel, int control_width)
        {
            //try
            //{
            //    //Remove ALL
            //    NailsChekin.Models.Core.ClearAndDisposeV2(panel);

            //    string jData = Get_Customer_History();
            //    if (jData.Length >= 20)
            //    {
            //        int locationX = 5;
            //        int locationY = 5;

            //        JArray jArray = JArray.Parse(jData);

            //        int count = 0;
            //        foreach (JObject item in jArray)
            //        {
            //            string phone = item["phone"].ToString();
            //            string name = item["name"].ToString();
            //            string type = item["status"].ToString();
            //            string clockInTime = item["signInDate"].ToString();

            //            Control control = new UCViewCustomerHistoryItem(phone, name, type, clockInTime);
            //            control.Width = control_width - 10;
            //            control.Location = new Point(locationX, locationY);
            //            panel.Controls.Add(control);

            //            locationY += (control.Height + 10);
            //            count++;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.SaveLOG_Crash("Message: " + ex.Message + "\nStack Trace:" + ex.StackTrace, "Init_Empl_History_CheckIn_Out Exception");
            //}
        }

        public static string Get_Customer_History()
        {
            string jData = "{";
            jData += "'date':'" + DateTimeHelper.Get_Local_DateTime() + "', ";
            jData += "}";

            string response = ApiHelper.CALL_API("Customer/signin-list", jData);
            if (Utilitys.IsValidJson(response))
                return response.ToString();

            return "";
        }

        public static void Search_Customer_List(Control panel, int control_width, string search_name, string seach_phone)
        {
            //try
            //{
            //    string jData = Get_Customer_Search(search_name, seach_phone);
            //    if (jData.Length >= 20)
            //    {
            //        int locationX = 5;
            //        int locationY = 5;

            //        JArray jArray = JArray.Parse(jData);

            //        int count = 0;
            //        foreach (JObject item in jArray)
            //        {
            //            string id = item["id"].ToString();
            //            string name = item["name"].ToString();
            //            string phone = item["phone"].ToString();
            //            string point = item["point"].ToString();
            //            string serviceName = item["serviceName"].ToString();
            //            string staffName = item["staffName"].ToString();
            //            string lastTime = item["lastTime"].ToString();

            //            Control control = new UCSearchCustomerItem(id, name, phone, point, serviceName, staffName, lastTime);
            //            control.Width = control_width; 
            //            control.Location = new Point(locationX, locationY);
            //            panel.Controls.Add(control);

            //            locationY += (control.Height + 10);
            //            count++;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.SaveLOG_Crash("Message: " + ex.Message + "\nStack Trace:" + ex.StackTrace, "Init_Empl_History_CheckIn_Out Exception");
            //}
        }

        public static string Get_Customer_Search(string name, string phone)
        {
            string jData = "{";
            jData += "'name':'" + name + "', ";
            jData += "'phone':'" + phone + "', ";
            jData += "}";

            string response = ApiHelper.CALL_API("Customer/search-list", jData);
            if (Utilitys.IsValidJson(response))
                return response.ToString();

            return "";
        }

        public static string Get_Customer_Search_Info(string id)
        {
            string jData = "{";
            jData += "'id':'" + id + "', ";
            jData += "}";

            string response = ApiHelper.CALL_API("Customer/search-info", jData);
            if (Utilitys.IsValidJson(response))
                return response.ToString();

            return "";
        }

        public static string Get_Customer_Service_History(string customerId, string from_date, string to_date, string nails, string ticket_id, string only_service_history = "0")
        {
            string jData = "{";
            jData += "'customer_id':'" + customerId + "', ";
            jData += "'from_date':'" + from_date + "', ";
            jData += "'to_date':'" + to_date + "', ";
            jData += "'nails':'" + nails + "', ";
            jData += "'ticket_id':'" + ticket_id + "', ";
            jData += "'only_service_history':'" + only_service_history + "' ";
            jData += "}";

            string response = ApiHelper.CALL_API("Customer/history", jData);
            if (Utilitys.IsValidJson(response))
                return response;

            return "";

        }



    }
}
