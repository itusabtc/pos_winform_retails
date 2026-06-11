using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace NailsChekin.Models.Helper
{
    class EmplHelper
    {
        public static void Init_Empl_Select(Control panel, Control parent)
        {
            try
            {
                //Remove ALL
                NailsChekin.Models.Core.ClearAndDisposeV2(panel);

                string jData = Get_Empl_CheckInOut();
                if (jData.Length >= 20)
                {
                    int locationX = 10;
                    int locationY = 10;

                    JArray jArray = JArray.Parse(jData);
                    int num_coloumn = 3;

                    int count = 0;
                    foreach (JObject item in jArray)
                    {
                        string staffId = item["id"].ToString();
                        string staffName = item["first_name"].ToString();
                        string clockIn = item["clockin"].ToString();
                        string clockInNum = item["stt"].ToString();
                        string clockInTime = item["login_time"].ToString();
                        string pinCode = "";

                        string imgURL = item["avatar"].ToString();
                        if (item["avatar"].ToString().Trim().Length <= 0)
                            imgURL = "NA";

                        var staff = new NailsChekin.UserControl.UCItemCheckInOut("empl", staffId, staffName, imgURL, pinCode, clockIn, clockInNum, clockInTime)
                        {
                            Title = staffName.ToUpper(),
                            Subtitle = clockIn.Equals("1") ? "CHECKED IN" : "NOT CHECKED IN",
                            Description = string.IsNullOrEmpty(clockInTime) ? "" : ("TIME: " + DateTimeHelper.format_time_am_pm(clockInTime)),
                            IconFrameStyle = UserControl.ItemBaseControl.IconFrame.Circle,
                            CardBaseColor = clockIn.Equals("1") ? ColorHelper.Danger : ColorHelper.Warning
                        };

                        staff.Location = new Point(locationX, locationY);
                        staff.Width = ((panel.Width - ((num_coloumn) * 10)) / num_coloumn);
                        staff.GroupRoot = parent;
                        panel.Controls.Add(staff);

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
                LogHelper.SaveLOG_Crash("Message: " + ex.Message + "\nStack Trace:" + ex.StackTrace, "Init_Empl_CheckIn_Out Exception");
            }
        }


        public static void Init_Empl_CheckIn_Out(Control panel)
        {
            try
            {
                //Remove ALL
                NailsChekin.Models.Core.ClearAndDisposeV2(panel);

                string jData = Get_Empl_CheckInOut();
                if (jData.Length >= 20)
                {
                    int locationX = 10;
                    int locationY = 10;

                    JArray jArray = JArray.Parse(jData);
                    int num_coloumn = 3;

                    int count = 0;
                    foreach (JObject item in jArray)
                    {
                        string staffId = item["id"].ToString();
                        string staffName = item["first_name"].ToString();
                        string clockIn = item["clockin"].ToString();
                        string clockInNum = item["stt"].ToString();
                        string clockInTime = item["login_time"].ToString();
                        string pinCode = item["pincode"].ToString();

                        string imgURL = item["avatar"].ToString();
                        if (item["avatar"].ToString().Trim().Length <= 0)
                            imgURL = "NA";

                        //Control staff = new NailsChekin.UserControl.UCItemCheckInOut("empl", staffId, staffName, imgURL, pinCode, clockIn, clockInNum, clockInTime);
                        //staff.Location = new Point(locationX, locationY);
                        //panel.Controls.Add(staff);

                        //string subTitle = "";
                        //if (clockInTime.Trim().Length > 0)
                        //{
                        //    subTitle = "IN: " + DateTimeHelper.format_time_am_pm(clockInTime);
                        //    subTitle += Environment.NewLine + "OUT: ";
                        //    subTitle += Environment.NewLine + "TOTAL HOUR: ";
                        //}

                        var staff = new NailsChekin.UserControl.UCItemCheckInOut("empl", staffId, staffName, imgURL, pinCode, clockIn, clockInNum, clockInTime)
                        {
                            Title = staffName.ToUpper(),
                            Subtitle = clockIn.Equals("1") ? "CLOCK OUT" : "CLOCK IN",
                            Description = string.IsNullOrEmpty(clockInTime) ? "" : ("TIME: " + DateTimeHelper.format_time_am_pm(clockInTime)),
                            IconFrameStyle = UserControl.ItemBaseControl.IconFrame.Circle,
                            CardBaseColor = clockIn.Equals("1") ? ColorHelper.Danger : ColorHelper.Warning
                        };

                        staff.Location = new Point(locationX, locationY);
                        staff.Width = ((panel.Width - ((num_coloumn) * 10)) / num_coloumn);
                        panel.Controls.Add(staff);
                        
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
                LogHelper.SaveLOG_Crash("Message: " + ex.Message + "\nStack Trace:" + ex.StackTrace, "Init_Empl_CheckIn_Out Exception");
            }
        }

        public static string Get_Empl_CheckInOut()
        {
            //GET ALL, NO TURN
            string jData = "{";
            jData += "'date':'" + DateTimeHelper.Get_Local_DateTime() + "', ";
            jData += "}";

            string response = ApiHelper.CALL_API("User/list-checkinout", jData);
            if (Utilitys.IsValidJson(response))
                return response.ToString();

            return "";
        }

        public static void Init_Empl_History_CheckIn_Out(Control panel)
        {
            //try
            //{
            //    //Remove ALL
            //    NailsChekin.Models.Core.ClearAndDisposeV2(panel);

            //    string jData = Get_Empl_History();
            //    if (jData.Length >= 20)
            //    {
            //        int locationX = 5;
            //        int locationY = 5;

            //        JArray jArray = JArray.Parse(jData);

            //        int count = 0;
            //        foreach (JObject item in jArray)
            //        {
            //            string name = item["empl_name"].ToString();
            //            string clockInTime = item["clockIn"].ToString();
            //            string clockOutTime = item["clockOut"].ToString();
            //            string totalTime = item["total_time"].ToString();

            //            Control staff = new NailsChekin.UserControl.UCViewHistoryItem(name, clockInTime, clockOutTime, totalTime);
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
            //    LogHelper.SaveLOG_Crash("Message: " + ex.Message + "\nStack Trace:" + ex.StackTrace, "Init_Empl_History_CheckIn_Out Exception");
            //}
        }

        public static string Get_Empl_History()
        {
            //GET ALL, NO TURN
            string jData = "{";
            jData += "'date':'" + DateTimeHelper.Get_Local_DateTime() + "', ";
            jData += "}";

            string response = ApiHelper.CALL_API("User/empl-times", jData);
            if (Utilitys.IsValidJson(response))
                return response.ToString();

            return "";
        }

    }
}
