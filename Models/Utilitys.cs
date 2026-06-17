using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using com.clover.sdk.v3.merchant;
using DevExpress.XtraReports.UI;
using NailsChekin.Models.Reports;
using Newtonsoft.Json.Linq;
using PdfiumViewer;

namespace NailsChekin.Models
{
    class Utilitys
    {
        // Random dùng chung cho toàn app. Tránh tạo "new Random()" mỗi lần gọi vì
        // Random được seed theo Environment.TickCount (~15ms) -> gọi liên tiếp trong
        // cùng tick sẽ sinh key TRÙNG (trùng cart_item_id / merchant_order_no).
        // Random không thread-safe nên mọi truy cập phải nằm trong _rngLock.
        private static readonly Random _rng = new Random();
        private static readonly object _rngLock = new object();

        public static string Convert_To_TimeZone(string date, int timezone)
        {
            DateTime current_date = DateTime.Parse(date);

            current_date = current_date.AddHours(timezone);

            //return current_date.ToString("HH:mm");
            return current_date.ToShortTimeString();

        }

        public static string Get_Local_Date_From_UTC(int timezone)
        {
            DateTime current_date = DateTime.UtcNow;

            current_date = current_date.AddHours(timezone);

            return current_date.ToString("yyyy-MM-dd");

        }

        public static string Convert_Date_UTC_To_TimeZone(int timezone)
        {
            DateTime current_date = DateTime.UtcNow;

            current_date = current_date.AddHours(timezone);

            return current_date.ToString("yyyy-MM-dd");
        }

        public static string Convert_To_FormatDate(string date)
        {
            //RETURN FORMAT MM/DD/YYYY

            string[] arr = Regex.Split(date, "-");
            if (date.Contains("/"))
                arr = Regex.Split(date, "/");

            if (arr[0].Length == 2) //Fomrat YYYY-MM-DD 
                return date;

            return arr[1] + "-" + arr[2] + "-" + arr[0];
        }


        public static string RemoveItemId(string selected, string itemId)
        {
            string _selected = selected;

            _selected = _selected.Replace(itemId, "");
            _selected = _selected.Replace(",,", "");
            if (_selected.EndsWith(","))
                _selected = _selected.Substring(0, _selected.Length - 1);

            return _selected;
        }

        public static string Get_Date_Name(string current_date)
        {
            string date_name = "";

            DateTime thisDate = DateTime.Parse(current_date);

            date_name = thisDate.ToString("dddd, MMMM dd, yyyy");

            return date_name;
        }

        public static double getTotalAmount(string text)
        {
            if (text.Trim().Length <= 0)
                return 0;

            double amount = 0;

            if (text.Contains("$"))
                text = text.Substring(1, text.Length - 1);

            double.TryParse(text, out amount);

            return amount;
        }

        public static double getSurcharge(double total_amount, double total_tip)
        {
            //Không bao gồm tip

            bool chkSurChargeOn = Utilitys.GetConfig("chkSurChargeOn", Constants.chkSurChargeOn);
            string surCharge_percent = Utilitys.GetConfig("surCharge_percent", Constants.surCharge_percent);
            string surCharge_minAmount = Utilitys.GetConfig("surCharge_minAmount", Constants.surCharge_minAmount);
            string surCharge_unit = Utilitys.GetConfig("surCharge_unit", Constants.surCharge_unit);

            if (chkSurChargeOn && surCharge_percent.Trim().Length > 0)
            {
                double min_amount = double.Parse(string.IsNullOrEmpty(surCharge_minAmount) ? "0" : surCharge_minAmount);
                if (total_amount - total_tip < min_amount) //Nhỏ hơn số tiền mới cộng Fee
                {
                    if (surCharge_unit.Equals("%"))
                    {
                        double surCharge = (total_amount - total_tip) * (double.Parse(surCharge_percent) / 100.0);
                        return Math.Round(surCharge, 2);
                    }
                    else  //FIX $
                    {
                        double surCharge = double.Parse(surCharge_percent);
                        return Math.Round(surCharge, 2);
                    }
                }
            }

            return 0;
        }

        public static double getSurchargeSplit(double total_amount, double total_tip, double custom_split_amount)
        {
            //SPLIT KHÔNG THU FEE

            //bool chkSurChargeOn = Utilitys.GetConfig("chkSurChargeOn", Constants.chkSurChargeOn);
            //string surCharge_percent = Utilitys.GetConfig("surCharge_percent", Constants.surCharge_percent);
            //string surCharge_minAmount = Utilitys.GetConfig("surCharge_minAmount", Constants.surCharge_minAmount);
            //string surCharge_unit = Utilitys.GetConfig("surCharge_unit", Constants.surCharge_unit);

            //if (chkSurChargeOn && surCharge_percent.Trim().Length > 0)
            //{
            //    double min_amount = double.Parse(string.IsNullOrEmpty(surCharge_minAmount) ? "0" : surCharge_minAmount);
            //    if ( ( (total_amount - total_tip) + custom_split_amount ) < min_amount) //Nhỏ hơn số tiền mới cộng Fee ( tổng tiền chứ không phải tổng credit )
            //    {
            //        if (surCharge_unit.Equals("%"))
            //        {
            //            double surCharge = (total_amount - total_tip) * (double.Parse(surCharge_percent) / 100.0);
            //            return Math.Round(surCharge, 2);
            //        }
            //        else  //FIX $
            //        {
            //            double surCharge = double.Parse(surCharge_percent);
            //            return Math.Round(surCharge, 2);
            //        }
            //    }
            //}

            return 0;
        }

        public static double getSurcharge_Debit(double total_amount, double total_tip)
        {
            //Không bao gồm tip
            //Surcharge UNIT $

            bool chkSurChargeOn = Utilitys.GetConfig("chkSurChargeOn", Constants.chkSurChargeOn);
            string surCharge_debit_percent = Utilitys.GetConfig("surCharge_debit_percent", Constants.surCharge_debit_percent);
            string surCharge_unit = Utilitys.GetConfig("surCharge_unit", Constants.surCharge_unit);

            if (chkSurChargeOn && surCharge_debit_percent.Trim().Length > 0)
            {
                if (surCharge_unit.Equals("%"))
                {
                    double surCharge = (total_amount - total_tip) * (double.Parse(surCharge_debit_percent) / 100.0);
                    return Math.Round(surCharge, 2);
                }
                else  //FIX $
                {
                    double surCharge = double.Parse(surCharge_debit_percent);
                    return Math.Round(surCharge, 2);
                }
            }

            return 0;
        }

        public static double getSurcharge_From_Paided(double clover_paided)
        {
            bool chkSurChargeOn = Utilitys.GetConfig("chkSurChargeOn", Constants.chkSurChargeOn);
            string surCharge_percent = Utilitys.GetConfig("surCharge_percent", Constants.surCharge_percent);
            string surCharge_unit = Utilitys.GetConfig("surCharge_unit", Constants.surCharge_unit);

            if (chkSurChargeOn && surCharge_percent.Trim().Length > 0)
            {
                if (surCharge_unit.Equals("%"))  //Trừ ngược số tiền thu được tính fee trong đó
                {
                    double net_amount = (clover_paided) / (1 + (double.Parse(surCharge_percent) / 100.0));
                    double surCharge = (clover_paided - net_amount) / 100.0;  //Surcharge lưu đơn vị $, clover amount đơn vị nhân 100

                    return Math.Round(surCharge, 2);
                }
                else  //FIX $
                {
                    double surCharge = double.Parse(surCharge_percent);
                    return Math.Round(surCharge, 2);
                }
            }

            return 0;
        }

        public static double getSurcharge_Debit_From_Paided(double clover_paided)
        {
            bool chkSurChargeOn = Utilitys.GetConfig("chkSurChargeOn", Constants.chkSurChargeOn);
            string surCharge_debit_percent = Utilitys.GetConfig("surCharge_debit_percent", Constants.surCharge_debit_percent);
            string surCharge_unit = Utilitys.GetConfig("surCharge_unit", Constants.surCharge_unit);

            if (chkSurChargeOn && surCharge_debit_percent.Trim().Length > 0)
            {
                if (surCharge_unit.Equals("%"))  //Trừ ngược số tiền thu được tính fee trong đó
                {
                    double net_amount = (clover_paided) / (1 + (double.Parse(surCharge_debit_percent) / 100.0));
                    double surCharge = (clover_paided - net_amount) / 100.0;  //Surcharge lưu đơn vị $, clover amount đơn vị nhân 100

                    return Math.Round(surCharge, 2);
                }
                else  //FIX $
                {
                    double surCharge = double.Parse(surCharge_debit_percent);
                    return Math.Round(surCharge, 2);
                }
            }

            return 0;
        }

        public static double getDualPrice(double total_amount, double total_tip)
        {
            //Chưa sử dụng 
            return 0;
        }

        public static string[] GetFastPays(double grand_total)
        {
            //Giống công thức trên web

            //Cost1
            double cost1 = grand_total;

            //Cost2
            var cost2 = Math.Ceiling(cost1);
            var x = 1;
            var n = cost2.ToString().Length - 1;
            if (n == 2) x = 10;
            else if (n == 3) x = 100;

            var u = 10 * x;

            if (cost2 == cost1)
            {
                if (Math.Floor(cost1 / u) < 5 * u)
                {
                    var temp = cost1 % 10;
                    if (temp == 0)
                    {
                        cost2 = cost1 + (10 - temp);
                    }
                    else
                    {
                        if (temp < 5)
                            cost2 = cost1 + (5 - temp);
                        else
                            cost2 = cost1 + (10 - temp);
                    }
                }
                else
                {
                    var temp = cost1 % u;
                    if (temp == 0)
                    {
                        cost2 = cost1 + ((10 * x) - temp);
                    }
                    else
                    {
                        if (temp < (5 * x))
                            cost2 = cost1 + ((5 * x) - temp);
                        else
                            cost2 = cost1 + ((10 * x) - temp);
                    }
                }
            }

            //Cost3
            var cost3 = cost2;
            var a = 20 * x;
            var a1 = 30 * x;
            var temp1 = cost2 % u;
            if (Math.Floor(cost2 / u) < 5 * u)
            {
                if (temp1 == 0)
                {
                    cost3 = cost2 + (10 * x - temp1);
                }
                else
                {
                    if (temp1 < 5 * x)
                        cost3 = cost2 + (5 * x - temp1);
                    else
                        cost3 = cost2 + (10 * x - temp1);
                }
            }
            else
            {
                if (cost3 == 100 * x)
                    cost3 = 200 * x;
                else if (cost3 == 200 * x)
                    cost3 = 500 * x;
                else if (cost3 >= (80 * x))
                {
                    cost3 = cost3 + (10 * x - temp1);
                }
                else if (cost3 / a == 1)
                    cost3 = cost3 + 30 * x;
                else if (cost3 / a1 == 1)
                    cost3 = cost3 + 20 * x;
                else
                {
                    cost3 = cost2 + (10 * x - temp1);
                }
            }

            return new string[] { cost2.ToString(), cost3.ToString() };
        }


        public static string[] GetCashPays(double grand_total)
        {
            //Giống công thức trên web

            //Cost0 = grand_total

            //Cost1
            double cost1 = Math.Round(grand_total);

            //Cost2
            var cost2 = Math.Ceiling(cost1);
            var x = 1;
            var n = cost2.ToString().Length - 1;
            if (n == 2) x = 10;
            else if (n == 3) x = 100;

            var u = 10 * x;

            if (cost2 == cost1)
            {
                if (Math.Floor(cost1 / u) < 5 * u)
                {
                    var temp = cost1 % 10;
                    if (temp == 0)
                    {
                        cost2 = cost1 + (10 - temp);
                    }
                    else
                    {
                        if (temp < 5)
                            cost2 = cost1 + (5 - temp);
                        else
                            cost2 = cost1 + (10 - temp);
                    }
                }
                else
                {
                    var temp = cost1 % u;
                    if (temp == 0)
                    {
                        cost2 = cost1 + ((10 * x) - temp);
                    }
                    else
                    {
                        if (temp < (5 * x))
                            cost2 = cost1 + ((5 * x) - temp);
                        else
                            cost2 = cost1 + ((10 * x) - temp);
                    }
                }
            }

            //Cost3
            var cost3 = cost2;
            var a = 20 * x;
            var a1 = 30 * x;
            var temp1 = cost2 % u;
            if (Math.Floor(cost2 / u) < 5 * u)
            {
                if (temp1 == 0)
                {
                    cost3 = cost2 + (10 * x - temp1);
                }
                else
                {
                    if (temp1 < 5 * x)
                        cost3 = cost2 + (5 * x - temp1);
                    else
                        cost3 = cost2 + (10 * x - temp1);
                }
            }
            else
            {
                if (cost3 == 100 * x)
                    cost3 = 200 * x;
                else if (cost3 == 200 * x)
                    cost3 = 500 * x;
                else if (cost3 >= (80 * x))
                {
                    cost3 = cost3 + (10 * x - temp1);
                }
                else if (cost3 / a == 1)
                    cost3 = cost3 + 30 * x;
                else if (cost3 / a1 == 1)
                    cost3 = cost3 + 20 * x;
                else
                {
                    cost3 = cost2 + (10 * x - temp1);
                }
            }

            return new string[] { cost1.ToString(), cost2.ToString(), cost3.ToString() };
        }

        public static string get_format_hour(int hour, int row)
        {
            string _hour = "";
            string _minute = "";
            if (row == 1)
                _minute = "15";
            else if (row == 2)
                _minute = "30";
            else if (row == 3)
                _minute = "45";
            else
                _minute = "00";

            //AM, PM FORMAT
            if (hour < 12)
                _hour = (hour) + (_minute.Length > 0 ? (":" + _minute) : "") + " AM";
            else if (hour == 12)
                _hour = (hour) + (_minute.Length > 0 ? (":" + _minute) : "") + " PM";
            else
                _hour = ((hour - 12)) + (_minute.Length > 0 ? (":" + _minute) : "") + " PM";

            //24h
            //_hour = ( hour < 10 ? ( "0" + hour ) : hour.ToString() ) + (_minute.Length > 0 ? (":" + _minute) : "");

            return _hour;
        }

        public static string get_format_time(int hour, int minute)
        {
            string _time = "";
            string _minute = minute < 10 ? ("0" + minute) : minute.ToString();

            //AM, PM FORMAT
            if (hour < 12)
                _time = (hour) + (_minute.Length > 0 ? (":" + _minute) : "") + " AM";
            else if (hour == 12)
                _time = (hour) + (_minute.Length > 0 ? (":" + _minute) : "") + " PM";
            else
                _time = ((hour - 12)) + (_minute.Length > 0 ? (":" + _minute) : "") + " PM";

            return _time;
        }

        public static string get_format_time(string time)
        {
            if (time.Length != 5)
                return time;

            string[] arr = Regex.Split(time, ":");
            int hour = int.Parse(arr[0]);
            int minute = int.Parse(arr[1]);

            return get_format_time(hour, minute);
        }

        public static string get_format_number(string number)
        {
            //Format $0.00

            if (number.Trim().Length <= 0 || number.Equals("0"))
                return "$0.00";

            double value = double.Parse(number);
            return value.ToString("C", CultureInfo.CurrentCulture);

        }

        public static List<TipSuggestion> TipSuggestions
        {
            get
            {
                List<TipSuggestion> suggestions = new List<TipSuggestion>();

                // Add selected suggestions to the list of tip suggestions.

                suggestions.Add(new TipSuggestion() { percentage = 5, name = "Thanks" });

                suggestions.Add(new TipSuggestion() { percentage = 10, name = "Nice" });

                suggestions.Add(new TipSuggestion() { percentage = 15, name = "Good Job" });

                suggestions.Add(new TipSuggestion() { percentage = 20, name = "Outstanding!" });

                return suggestions;
            }
        }

        public static void SaveLOG_Payment(string log, string title)
        {
            try
            {
                string forderLog = "C:\\POSLogs\\RetailPayments\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                string logWriteUrl = forderLog + "SERVICE_PAYMENT_LOG_" + DateTime.Now.ToString("ddMMyyyy") + ".txt";

                StreamWriter sw = new StreamWriter(logWriteUrl, true);
                try
                {
                    sw.WriteLine(DateTime.Now.ToString() + "." + title + ": " + log);
                }
                catch
                {
                    sw.WriteLine(DateTime.Now.ToString() + "." + title + " EXCEPTION: " + log);
                }

                sw.Close();
            }
            catch (Exception ex) { }
        }

        public static void SaveLOG_Crash(string log, string title)
        {
            try
            {
                string forderLog = "C:\\POSLogs\\Crashs\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                string logWriteUrl = forderLog + "Crash_LOG_" + DateTime.Now.ToString("ddMMyyyy") + ".txt";

                StreamWriter sw = new StreamWriter(logWriteUrl, true);
                try
                {
                    sw.WriteLine(DateTime.Now.ToString() + "." + title + ": " + log);
                }
                catch
                {
                    sw.WriteLine(DateTime.Now.ToString() + "." + title + " EXCEPTION: " + log);
                }

                sw.Close();
            }
            catch (Exception ex) { }
        }

        public static void SaveLOG_CodePay(string log, string title)
        {
            try
            {
                string forderLog = "C:\\POSLogs\\CodePay\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                string logWriteUrl = forderLog + "CodePay_LOG_" + DateTime.Now.ToString("ddMMyyyy") + ".txt";

                StreamWriter sw = new StreamWriter(logWriteUrl, true);
                try
                {
                    sw.WriteLine(DateTime.Now.ToString() + "." + title + ": " + log);
                }
                catch
                {
                    sw.WriteLine(DateTime.Now.ToString() + "." + title + " EXCEPTION: " + log);
                }

                sw.Close();
            }
            catch (Exception ex) { }
        }

        public static void SaveLOG_Socket(string log, string title)
        {
            try
            {
                string forderLog = "C:\\POSLogs\\Sockets\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                string logWriteUrl = forderLog + "SOCKET_LOG_" + DateTime.Now.ToString("ddMMyyyy") + ".txt";

                StreamWriter sw = new StreamWriter(logWriteUrl, true);
                try
                {
                    sw.WriteLine(DateTime.Now.ToString() + "." + title + ": " + log);
                }
                catch
                {
                    sw.WriteLine(DateTime.Now.ToString() + "." + title + " EXCEPTION: " + log);
                }

                sw.Close();
            }
            catch (Exception ex) { }
        }

        public static string createRamdomKey()
        {
            //string key = Guid.NewGuid().ToString("d");
            //key = Regex.Replace(key, "-", "");

            //if (key.Length > 7)
            //    key = key.Substring(0, 7);

            //return key;

            return createRamdomKeyNumber();
        }

        public static string createRamdomKeyNumber()
        {
            // Chữ số đầu tiên BẮT BUỘC khác 0 (1..9).
            // Nếu để số 0 đứng đầu, khi nhét vào JSON sẽ thành 'id': 0049947 -> số JSON
            // không hợp lệ, API model-binding báo lỗi "Input string '0049947' is not a valid integer".
            // Đồng thời phải giữ ĐÚNG 7 chữ số để API nhận diện đây là temp ticket id (Length >= 7)
            // và tạo order mới thay vì update.
            lock (_rngLock)
            {
                string key = _rng.Next(1, 10).ToString();    // chữ số đầu: 1..9

                for (int i = 0; i < 6; i++)
                    key += _rng.Next(0, 10).ToString();      // 6 chữ số còn lại: 0..9

                return key;                                  // luôn 7 chữ số, không có số 0 đứng đầu
            }
        }


        #region Read / Write Config

        public static void SaveConfig(string key, string value)
        {
            try
            {
                string forderLog = "C:\\POSLogs\\Retails\\Config\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                string logWriteUrl = forderLog + "config.txt";

                string current_text = Utilitys.RedConfig(key);

                if (current_text.Trim().Length <= 0)
                {
                    StreamWriter sw = new StreamWriter(logWriteUrl, true);
                    sw.WriteLine(key + ": " + value);
                    sw.Close();
                }
                else  //Đã có thì update lại
                {
                    string old_content = Utilitys.ReadAllConfig();
                    old_content = Regex.Replace(old_content, current_text, (key + ": " + value));

                    StreamWriter sw = new StreamWriter(logWriteUrl, false);
                    sw.WriteLine(old_content);
                    sw.Close();
                }
            }
            catch (Exception ex) { }
        }

        public static void SaveAllConfig(string configs)
        {
            try
            {
                string forderLog = "C:\\POSLogs\\Retails\\Config\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                string logWriteUrl = forderLog + "config.txt";

                StreamWriter sw = new StreamWriter(logWriteUrl, false);
                sw.WriteLine(configs);
                sw.Close();

            }
            catch (Exception ex) { }
        }

        public static string RedConfig(string key)
        {
            try
            {
                string value = "";

                string forderLog = "C:\\POSLogs\\Retails\\Config\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                string logReadUrl = forderLog + "config.txt";

                using (StreamReader sr = File.OpenText(logReadUrl))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        if (s.StartsWith(key))
                        {
                            value = s.Replace(key + ": ", "");
                            break;
                        }
                    }
                    sr.Close();
                }
                return value;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string ReadAllConfig()
        {
            try
            {
                string value = "";

                string forderLog = "C:\\POSLogs\\Retails\\Config\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                string logReadUrl = forderLog + "config.txt";

                using (StreamReader sr = File.OpenText(logReadUrl))
                {
                    string s = sr.ReadToEnd();
                    sr.Close();
                }
                return value;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string GetConfig(string key, string default_value)
        {
            string value = Utilitys.RedConfig(key);

            if (value.Trim().Length <= 0)
                value = default_value;

            return value;

        }

        public static bool GetConfig(string key, bool default_value)
        {
            string value = Utilitys.RedConfig(key);
            if (value.ToUpper().Equals("ON") || value.ToUpper().Equals("TRUE"))
                return true;
            else if (value.ToUpper().Equals("OFF") || value.ToUpper().Equals("FALSE"))
                return false;

            if (value.Trim().Length <= 0)
                return default_value;

            return true;

        }

        public static string GetStoreConfig(string key, string default_value)
        {
            string value = Utilitys.RedStoreConfig(key);

            if (value.Trim().Length <= 0)
                value = default_value;

            return value;

        }

        public static string RedStoreConfig(string key)
        {
            try
            {
                string value = "";

                string forderLog = "C:\\POSLogs\\Retails\\Config\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                string logReadUrl = forderLog + "store_config.txt";

                using (StreamReader sr = File.OpenText(logReadUrl))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        if (s.StartsWith(key))
                        {
                            value = s.Replace(key + ": ", "");
                            break;
                        }
                    }
                    sr.Close();
                }
                return value;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static void SaveAllStoreConfig(string configs)
        {
            try
            {
                string forderLog = "C:\\POSLogs\\Retails\\Config\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                string logWriteUrl = forderLog + "store_config.txt";

                StreamWriter sw = new StreamWriter(logWriteUrl, false);
                sw.WriteLine(configs);
                sw.Close();

            }
            catch (Exception ex) { }
        }


        public static void CreateForderConfig()
        {
            try
            {
                string forderLog = "C:\\POSLogs\\Retails\\Config\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                string forderPrinter = Constants.web_print_filePath;
                if (!Directory.Exists(forderPrinter))
                    Directory.CreateDirectory(forderPrinter);

            }
            catch (Exception ex) { }
        }

        public static string RedFileText(string filename)
        {
            try
            {
                string value = "";

                string logReadUrl = filename;
                using (StreamReader sr = File.OpenText(logReadUrl))
                {
                    value = sr.ReadToEnd();
                    sr.Close();
                }
                return value;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        #endregion


        #region Utils

        public static System.Drawing.Color GetColor(string hex)
        {
            try
            {
                //hex substring: FBBC05
                string hex1 = hex.Substring(0, 2);
                string hex2 = hex.Substring(2, 2);
                string hex3 = hex.Substring(4, 2);

                int red = int.Parse(hex1, System.Globalization.NumberStyles.HexNumber);
                int green = int.Parse(hex2, System.Globalization.NumberStyles.HexNumber);
                int blue = int.Parse(hex3, System.Globalization.NumberStyles.HexNumber);

                return System.Drawing.Color.FromArgb(red, green, blue);
            }
            catch (Exception ex)
            {
                //Default
                return System.Drawing.Color.FromArgb(0xFB, 0xBC, 0x05);
            }
        }

        #endregion


        #region Call Rest full api

        public static string CALL_API(string endpoint, string DATA, string method = "GET", bool request_authen = false)
        {
            try
            {
                //string URL = "https://api-retails.nailsbeautysupply.com/api/" + endpoint;
                string URL = "http://178.63.64.96:8088/api/" + endpoint;
                //string DATA = @"{""userName"":""RET32132"",""password"":""123456""}";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = method;
                request.ContentType = "application/json";

                if (request_authen)
                {
                    string token = Utilitys.GetStoreConfig("accessToken", "");
                    request.Headers.Add("Authorization", "Bearer " + token);
                }

                if (method.Equals("POST") || method.Equals("PUT"))
                {
                    DATA = Regex.Replace(DATA, "'", "\"");
                    request.ContentLength = DATA.Length;
                    using (Stream webStream = request.GetRequestStream())
                    using (StreamWriter requestWriter = new StreamWriter(webStream, System.Text.Encoding.ASCII))
                    {
                        requestWriter.Write(DATA);
                    }
                }

                try
                {
                    WebResponse webResponse = request.GetResponse();
                    using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
                    using (StreamReader responseReader = new StreamReader(webStream))
                    {
                        string response = responseReader.ReadToEnd();
                        return response;
                    }
                }
                catch (WebException e)
                {
                    // Timeout / mất kết nối / không phân giải được host => e.Response = null
                    if (e.Response == null)
                        return "Error API: " + e.Status + " - " + e.Message;

                    using (WebResponse response = e.Response)
                    using (Stream data = response.GetResponseStream() ?? Stream.Null)
                    using (var reader = new StreamReader(data))
                    {
                        string text = reader.ReadToEnd();
                        if (!string.IsNullOrEmpty(text) && text.Contains("message"))
                        {
                            try { return "Error API: " + JObject.Parse(text)["message"]; } catch { }
                        }

                        return "Error API: " + text;
                    }
                }
                catch (Exception e)
                {
                    return "Error API: 404 Exception - " + e.Message;
                }
            }
            catch (Exception exx)
            {
                return "Error API: " + exx.Message;
            }
        }


        #endregion

        #region Validate JSON

        public static bool IsValidJson(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            try
            {
                JToken.Parse(input);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsJSonObject(string text)
        {
            try
            {
                JObject obj = JObject.Parse(text);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string IsJsonObjectOrArray(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return "Invalid JSON";

            try
            {
                JToken token = JToken.Parse(json);
                if (token is JObject)
                    return "JObject";
                else if (token is JArray)
                    return "JArray";
                else
                    return "Unknown JSON type";
            }
            catch (Exception)
            {
                return "Invalid JSON";
            }
        }

        public static bool HasKey(JToken token, string key)
        {
            return token is JObject obj && obj.ContainsKey(key);
        }

        static bool HasKey(JObject jObject, string key)
        {
            return jObject.ContainsKey(key);
        }

        #endregion

        #region Check Internet

        public static async System.Threading.Tasks.Task<bool> CheckInternet()
        {
            try
            {
                Ping myPing = new Ping();
                //String host = "google.com";
                String host = Constants.hostName;
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool IsInternetAvailable()
        {
            try
            {
                // Ping a well-known server (e.g., Google's public DNS server)
                using (Ping ping = new Ping())
                {
                    PingReply reply = ping.Send("95.217.32.253", 2000); // 2 second timeout
                    bool result = (reply != null && reply.Status == IPStatus.Success);
                    if (result == true)
                    {
                        return result;
                    }
                }

                // Ping a well-known server (e.g., Google's public DNS server)
                using (Ping ping = new Ping())
                {
                    PingReply reply = ping.Send("pos.nailspaofamerica.com", 5000); // 5 second timeout
                    return reply != null && reply.Status == IPStatus.Success;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool CheckIsNumber(string number)
        {
            try
            {
                if (string.IsNullOrEmpty(number))
                    return false;

                double n_check = -1;
                if (double.TryParse(number, out n_check))
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool IsNumber(string text)
        {
            try
            {
                double number = -1;
                if (double.TryParse(text, out number))
                    return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        #endregion

    }
}
