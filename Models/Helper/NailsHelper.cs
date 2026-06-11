using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace NailsChekin.Models.Helper
{
    public class NailsHelper
    {
        public static void Init_Staff_TakeTurn(Control panel, bool get_all, string skills, ref string staff_data, bool only_clear = false, bool sort_by_clockInNum = true)
        {
            //try
            //{
            //    //Remove ALL
            //    NailsChekin.Models.Core.ClearAndDisposeV2(panel);
            //    //Thread.Sleep(3000); //đợi bộ dọn rác chạy xong !!!
            //    staff_data = "";

            //    string jData = Get_Staff_TakeTurn(get_all, skills);
            //    if (jData.Length >= 20)
            //    {
            //        int locationX = 0;
            //        int locationY = 5;

            //        JArray jArray = JArray.Parse(jData);

            //        bool show_image = jArray.Count >= 30 ? false : true;
            //        int count = 0;
            //        int num_staff = 0;
            //        foreach (JObject item in jArray)
            //        {
            //            string staffCode = item["staffCode"].ToString();
            //            string staffId = item["id"].ToString();
            //            string staffName = item["first_name"].ToString();
            //            string color = item["color"].ToString();
            //            string text_color = item["text_color"].ToString();
            //            string specializeIns = item["specializeIns"].ToString();
            //            string inService = item["inService"].ToString();

            //            string takeCount = item["takeCount"].ToString();
            //            string apptCount = item["apptCount"].ToString();

            //            string taketurnType = item["taketurnType"].ToString();
            //            string nextTurn = (num_staff == 0 ? "1" : "0"); num_staff++;

            //            if (taketurnType.Equals("ByClockIn") || taketurnType.Equals("ByNoClockIn"))
            //                nextTurn = "0";

            //            string imgURL = item["avatar"].ToString();
            //            if (item["avatar"].ToString().Trim().Length <= 0)
            //                imgURL = "NA";

            //            if (!show_image)
            //                imgURL = "NA";

            //            string clockInTime = item["_time"].ToString();
            //            string clockInNum = item["clockInNum"].ToString();
            //            if (!sort_by_clockInNum)
            //                clockInNum = (count + 1).ToString();

            //            if (NailsChekin.Properties.Settings.Default.selected_nails.Equals(staffId))  //Heigline selected
            //                color = Constants.color_nails_selected;

            //            var staff = new NailsChekin.UserControl.UCStaffRound(staffId, staffName, imgURL, color, text_color, specializeIns, clockInNum, clockInTime, takeCount, apptCount, inService, nextTurn, "MainPOS")
            //            {
            //                Title = NailsHelper.Format_ClockIn_Num(clockInNum) + "  " + DateTimeHelper.format_time_am_pm(clockInTime),
            //                TitleSize = 12,
            //                Subtitle = staffName.ToUpper(),
            //                SubtitleSize = 18,
            //                InitialUseSubtitle = true,
            //                Description = specializeIns,
            //                DescSize = 9,
            //                IconFrameStyle = UserControl.ItemBaseControl.IconFrame.Circle,
            //                CardBaseColor = inService.Equals("1") ? ColorHelper.Danger : (nextTurn.Equals("1") ? ColorHelper.Warning : ColorHelper.Question)
            //            };

            //            staff.Width = LayoutHelper.coloumn_nails_width;
            //            staff.Location = new Point(locationX, locationY);
            //            panel.Controls.Add(staff);
            //            if (show_image && !imgURL.EndsWith("NA"))
            //                _ = staff.LoadIconAsync(NailsChekin.Models.Constants.imgURL + imgURL, placeholder: staff.PlaceholderIcon);

            //            if (count > 0 && (count + 1) % LayoutHelper.number_coloumn_nails == 0)
            //            {
            //                locationY += (staff.Height + 5);
            //                locationX = 0;
            //            }
            //            else
            //            {
            //                locationX += (staff.Width + 5);
            //            }

            //            staff_data += staffName + " (" + staffId + ")__";
            //            count++;
            //        }
            //    }

            //    //Store in local
            //    Properties.Settings.Default.jNailsTech = jData;
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.SaveLOG_Crash("Message: " + ex.Message + "\nStack Trace:" + ex.StackTrace, "Init_Staff_TakeTurn_Multi_RightToLeft Exception");
            //}
        }
        public static string Get_Staff_TakeTurn(bool get_all, string skills)
        {
            string jData = "{";
            jData += "'date':'', ";
            jData += "'skills':'" + skills + "', ";
            jData += "'get_all':'" + (get_all ? "1" : "0") + "' ";
            jData += "}";

            string response = ApiHelper.CALL_API("Staff/take-turn", jData);
            if (Utilitys.IsValidJson(response))
                return response.ToString();

            return "";
        }

        public static string Get_Staff_All()
        {
            string jData = "{}";

            string response = ApiHelper.CALL_API("Staff/nails-all", jData);
            if (Utilitys.IsValidJson(response))
                return response.ToString();

            return "";
        }

        public static void Init_Staff_Select(Control panel, Control parent, bool show_all_nails, bool allow_edit_turn = false)
        {
            try
            {
                //Remove ALL
                NailsChekin.Models.Core.ClearAndDisposeV2(panel);

                string jData = show_all_nails ? Get_Staff_All() : Get_Staff_TakeTurn(true, "");
                if (jData.Length >= 20)
                {
                    int locationX = 10;
                    int locationY = 10;

                    JArray jArray = JArray.Parse(jData);
                    int num_coloumn = 7;

                    int count = 0;
                    foreach (JObject item in jArray)
                    {
                        string staffCode = item["staffCode"].ToString();
                        string staffId = item["id"].ToString();
                        string staffName = item["first_name"].ToString();
                        string color = item["color"] == null ? "" : item["color"].ToString();
                        string text_color = item["text_color"] == null ? "" : item["text_color"].ToString();
                        string specializeIns = item["specializeIns"] == null ? "" : item["specializeIns"].ToString();
                        string inService = item["inService"] == null ? "0" : item["inService"].ToString();

                        string takeCount = item["takeCount"].ToString();
                        string apptCount = item["apptCount"].ToString();

                        string turnNum = item["turnNum"] == null ? "0" : item["turnNum"].ToString();
                        string clockInNum = item["clockInNum"] == null ? "0" : item["clockInNum"].ToString();
                        string clockInTime = item["_time"] == null ? "" : item["_time"].ToString();

                        string imgURL = item["avatar"].ToString();
                        if (item["avatar"].ToString().Trim().Length <= 0)
                            imgURL = "NA";

                        var staff = new NailsChekin.UserControl.UCItemCheckInOut("nails", staffId, staffName, imgURL, "", "1", clockInNum, clockInTime)
                        {
                            Title = ( allow_edit_turn ? NailsHelper.Format_ClockIn_Num(turnNum) : NailsHelper.Format_ClockIn_Num(clockInNum) ) + "  " + DateTimeHelper.format_time_am_pm(clockInTime),
                            Subtitle = staffName.ToUpper(),
                            Description = specializeIns,
                            IconFrameStyle = UserControl.ItemBaseControl.IconFrame.Circle,
                            CardBaseColor = ColorHelper.Warning
                        };

                        staff.Location = new Point(locationX, locationY);
                        staff.Width = ((panel.Width - ((num_coloumn) * 10) - 5) / num_coloumn);
                        staff.GroupRoot = parent;
                        staff.SetAllowEditTurn(allow_edit_turn, turnNum, jArray.Count);
                        panel.Controls.Add(staff);
                        if (!imgURL.EndsWith("NA"))
                            _ = staff.LoadIconAsync(NailsChekin.Models.Constants.imgURL + imgURL, placeholder: staff.PlaceholderIcon);

                        if (count > 0 && (count + 1) % num_coloumn == 0)
                        {
                            locationY += (staff.Height + 10);
                            locationX = 10;
                        }
                        else
                        {
                            locationX += (staff.Width + 10);
                        }

                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_Crash("Message: " + ex.Message + "\nStack Trace:" + ex.StackTrace, "Init_Staff_CheckIn_Out Exception");
            }
        }

        public static void Init_Staff_CheckIn_Out(Control panel)
        {
            try
            {
                //Remove ALL
                NailsChekin.Models.Core.ClearAndDisposeV2(panel);

                string jData = Get_Staff_CheckInOut();
                if (jData.Length >= 20)
                {
                    int locationX = 10;
                    int locationY = 10;

                    JArray jArray = JArray.Parse(jData);
                    int num_coloumn = 7;

                    int count = 0;
                    foreach (JObject item in jArray)
                    {
                        string staffId = item["id"].ToString();
                        string staffName = item["nick_name"].ToString();
                        string clockIn = item["clockin"].ToString();
                        string clockInNum = item["stt"].ToString();
                        string clockInTime = item["login_time"].ToString();
                        string pinCode = item["pincode"].ToString();

                        string imgURL = item["avatar"].ToString();
                        if (item["avatar"].ToString().Trim().Length <= 0)
                            imgURL = "NA";

                        var staff = new NailsChekin.UserControl.UCItemCheckInOut("nails", staffId, staffName, imgURL, pinCode, clockIn, clockInNum, clockInTime)
                        {
                            Title = staffName.ToUpper(),
                            Subtitle = clockIn.Equals("1") ? "CLOCK OUT" : "CLOCK IN",
                            Description = string.IsNullOrEmpty(clockInTime) ? "" : ("TIME: " + DateTimeHelper.format_time_am_pm(clockInTime)),
                            IconFrameStyle = UserControl.ItemBaseControl.IconFrame.Circle,
                            CardBaseColor = clockIn.Equals("1") ? ColorHelper.Danger : ColorHelper.Warning
                        };

                        staff.Location = new Point(locationX, locationY);
                        staff.Width = ((panel.Width - ((num_coloumn) * 10 )) / num_coloumn);
                        panel.Controls.Add(staff);
                        if (!imgURL.EndsWith("NA"))
                            _ = staff.LoadIconAsync(NailsChekin.Models.Constants.imgURL + imgURL, placeholder: staff.PlaceholderIcon);

                        if (count > 0 && (count + 1) % num_coloumn == 0)
                        {
                            locationY += (staff.Height + 10);
                            locationX = 10;
                        }
                        else
                        {
                            locationX += (staff.Width + 10);
                        }

                        count++;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_Crash("Message: " + ex.Message + "\nStack Trace:" + ex.StackTrace, "Init_Staff_CheckIn_Out Exception");
            }
        }
        public static string Get_Staff_CheckInOut()
        {
            //GET ALL, NO TURN
            string jData = "{";
            jData += "'date':'" + DateTimeHelper.Get_Local_DateTime() + "', ";
            jData += "}";

            string response = ApiHelper.CALL_API("Staff/list-checkinout", jData);
            if (Utilitys.IsValidJson(response))
                return response.ToString();

            return "";
        }

        public static void Init_Staff_History_CheckIn_Out(Control panel)
        {
            //try
            //{
            //    //Remove ALL
            //    NailsChekin.Models.Core.ClearAndDisposeV2(panel);

            //    string jData = Get_Staff_History();
            //    if (jData.Length >= 20)
            //    {
            //        int locationX = 5;
            //        int locationY = 5;

            //        JArray jArray = JArray.Parse(jData);

            //        int count = 0;
            //        foreach (JObject item in jArray)
            //        {
            //            string staffId = item["id"].ToString();
            //            string staffName = item["nick_name"].ToString();
            //            string type = item["type"].ToString();
            //            string clockInTime = item["login_time"].ToString();

            //            Control staff = new NailsChekin.UserControl.UCViewHistoryItem(staffName, type, clockInTime);
            //            staff.Width = panel.Width - 10;
            //            staff.Location = new Point(locationX, locationY);
            //            panel.Controls.Add(staff);

            //            locationY += (staff.Height + 10);
            //            count++;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.SaveLOG_Crash("Message: " + ex.Message + "\nStack Trace:" + ex.StackTrace, "Init_Staff_History_CheckIn_Out Exception");
            //}
        }
        public static string Get_Staff_History()
        {
            //GET ALL, NO TURN
            string jData = "{";
            jData += "'date':'" + DateTimeHelper.Get_Local_DateTime() + "', ";
            jData += "}";

            string response = ApiHelper.CALL_API("Staff/history-check-in", jData);
            if (Utilitys.IsValidJson(response))
                return response.ToString();

            return "";
        }

        public static void Init_Staff_Test_Scroll(Control panel)
        {
            try
            {
                //Remove ALL
                NailsChekin.Models.Core.ClearAndDisposeV2(panel);

                string jData = Get_Staff_CheckInOut();
                if (jData.Length >= 20)
                {
                    int locationX = 5;
                    int locationY = 45;

                    JArray jArray = JArray.Parse(jData);

                    bool show_image = jArray.Count >= 30 ? false : true;
                    int num_coloumn = 3;

                    int count = 0;
                    foreach (JObject item in jArray)
                    {
                        string staffId = item["id"].ToString();
                        string staffName = item["nick_name"].ToString();
                        string clockIn = item["clockin"].ToString();
                        string clockInNum = item["stt"].ToString();
                        string clockInTime = item["login_time"].ToString();
                        string pinCode = item["pincode"].ToString();

                        string imgURL = item["avatar"].ToString();
                        if (item["avatar"].ToString().Trim().Length <= 0)
                            imgURL = "NA";

                        if (!show_image)
                            imgURL = "NA";

                        Control staff = new NailsChekin.UserControl.UCItemCheckInOut("nails", staffId, staffName, imgURL, pinCode, clockIn, clockInNum, clockInTime);
                        staff.Width = panel.Width / num_coloumn;
                        staff.Location = new Point(locationX, locationY);
                        panel.Controls.Add(staff);

                        if (count > 0 && (count + 1) % num_coloumn == 0)
                        {
                            locationY += (staff.Height + 10);
                            locationX = 5;
                        }
                        else
                        {
                            locationX += (staff.Width + 10);
                        }

                        count++;
                    }

                    int count2 = 0;
                    foreach (JObject item in jArray)
                    {
                        string staffId = item["id"].ToString();
                        string staffName = item["nick_name"].ToString();
                        string clockIn = item["clockin"].ToString();
                        string clockInNum = item["stt"].ToString();
                        string clockInTime = item["login_time"].ToString();
                        string pinCode = item["pincode"].ToString();

                        clockInNum = (int.Parse(clockInNum) + 21).ToString();

                        string imgURL = item["avatar"].ToString();
                        if (item["avatar"].ToString().Trim().Length <= 0)
                            imgURL = "NA";

                        if (!show_image)
                            imgURL = "NA";

                        Control staff = new NailsChekin.UserControl.UCItemCheckInOut("nails", staffId, staffName, imgURL, pinCode, clockIn, clockInNum, clockInTime);
                        staff.Width = panel.Width / num_coloumn;
                        staff.Location = new Point(locationX, locationY);
                        panel.Controls.Add(staff);

                        if (count > 0 && (count + 1) % num_coloumn == 0)
                        {
                            locationY += (staff.Height + 10);
                            locationX = 5;
                        }
                        else
                        {
                            locationX += (staff.Width + 10);
                        }

                        count++;
                        count2++;
                        if (count2 >= 10)
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_Crash("Message: " + ex.Message + "\nStack Trace:" + ex.StackTrace, "Init_Staff_CheckIn_Out Exception");
            }
        }

        public static string Format_ClockIn_Num(string clockInNum)
        {
            string clock_in_num_text = "";
            if (clockInNum.Trim().Length > 0)
            {
                if (int.Parse(clockInNum) == 1)
                    clock_in_num_text = "1ST";
                else if (int.Parse(clockInNum) == 2)
                    clock_in_num_text = "2ND";
                else if (int.Parse(clockInNum) == 3)
                    clock_in_num_text = "3RD";
                else if ((int.Parse(clockInNum) >= 4 && int.Parse(clockInNum) <= 20) || int.Parse(clockInNum) >= 34)
                    clock_in_num_text = clockInNum + "TH";
                else if (clockInNum.EndsWith("1"))
                    clock_in_num_text = clockInNum + "ST";
                else if (clockInNum.EndsWith("2"))
                    clock_in_num_text = clockInNum + "ND";
                else if (clockInNum.EndsWith("3"))
                    clock_in_num_text = clockInNum + "RD";
                else
                    clock_in_num_text = clockInNum + "TH";
            }

            return clock_in_num_text;
        }

    }
}
