using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace NailsChekin.Models.Helper
{
    public class DateTimeHelper
    {
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

        public static string Get_Local_DateTime()
        {
            if (Constants.system_mode == SYSTEM_MODE.TEST)  //Tiệm test VN lấy giờ 
            {
                // Specify the desired time zone (e.g., "Eastern Standard Time", "Pacific Standard Time", etc.)
                string timeZoneId = "Central Standard Time"; // Change this to your desired time zone

                // Get the current UTC time
                DateTime utcNow = DateTime.UtcNow;

                // Get the timezone info
                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

                // Convert UTC time to local time in the specified timezone
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZone);
                return localTime.ToString("yyyy-MM-dd HH:mm:ss");
            }

            //GET CURENT COMPUTER DATETIME !!!
            DateTime current_date = DateTime.Now;
            return current_date.ToString("yyyy-MM-dd HH:mm:ss");
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

        public static string format_time_am_pm(string time)
        {
            if (string.IsNullOrEmpty(time))
                return "";

            try
            {
                DateTime now = DateTime.Parse(time);

                // Định dạng giờ:phút + AM/PM
                string timeOnly = now.ToString("hh:mm tt");
                return timeOnly;
            }
            catch {
                return time;
            }
        }

        // Các pattern chấp nhận: "10-16-2025 8:46PM", "10-16-2025 08:46 PM", ...
        private static readonly string[] _formats = new[]
        {
            "MM-dd-yyyy h:mmtt",
            "MM-dd-yyyy hh:mmtt",
            "MM-dd-yyyy h:mm tt",
            "MM-dd-yyyy hh:mm tt"
        };

        /// <summary>
        /// Parse chuỗi dạng "MM-dd-yyyy h:mmtt" (AM/PM) thành DateTime (local time).
        /// Ném FormatException nếu sai định dạng.
        /// </summary>
        public static DateTime ParseUsAmPm(string input)
        {
            if (TryParseUsAmPm(input, out var dt))
                return dt;

            throw new FormatException($"Invalid date/time: \"{input}\". Expect formats like MM-dd-yyyy h:mmtt (e.g., 10-16-2025 8:46PM).");
        }

        /// <summary>
        /// TryParse an toàn. Trả về false nếu không hợp lệ.
        /// </summary>
        public static bool TryParseUsAmPm(string input, out DateTime result)
        {
            return DateTime.TryParseExact(
                input?.Trim(),
                _formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces, // cho phép khoảng trắng lặt vặt
                out result
            );
        }

    }
}
