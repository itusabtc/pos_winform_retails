using System;
using System.IO;

namespace NailsChekin.Models.Helper
{
    public class LogHelper
    {
        public static void SaveLOG_Payment(string log, string title)
        {
            try
            {
                string forderLog = "C:\\POSLogs\\RetailPayments\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                string logWriteUrl = forderLog + "RETAIL_PAYMENT_LOG_" + DateTime.Now.ToString("ddMMyyyy") + ".txt";

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
                string forderLog = "C:\\POSLogs\\Crashs-Retail\\";
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

        public static void SaveLOG_Trace(string log, string title)
        {
            try
            {
                string forderLog = "C:\\POSLogs\\PurgeTrace-Retail\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                string logWriteUrl = forderLog + "Trace_" + DateTime.Now.ToString("ddMMyyyy") + ".txt";

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

        public static void SaveLOG_PurgeTrace(string log, string title)
        {
            try
            {
                string forderLog = "C:\\POSLogs\\PurgeTrace-Retail\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                string logWriteUrl = forderLog + "Trace_" + DateTime.Now.ToString("ddMMyyyy") + ".txt";

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
                string forderLog = "C:\\POSLogs\\Sockets-Retail\\";
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

        public static void SaveLOG_Test_Clover(string log, string title)
        {
            try
            {
                string forderLog = "C:\\POSLogs\\Clover-Retail\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                string logWriteUrl = forderLog + "LOG_" + DateTime.Now.ToString("ddMMyyyy") + ".txt";

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
                string forderLog = "C:\\POSLogs\\CodePayW-Retail\\";
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

        public static void SaveLOG_Printer(string log, string title)
        {
            try
            {
                string forderLog = "C:\\POSLogs\\Printer-Retail\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                string logWriteUrl = forderLog + DateTime.Now.ToString("ddMMyyyy") + ".txt";

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

    }
}
