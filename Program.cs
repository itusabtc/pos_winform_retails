using NailsChekin.Models.Helper;
using NailsChekin.Models;
using NailsChekin.Models.Helper;
using NailsChekin.UserControl;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using static NailsChekin.Models.Constants;
using NailsChekin.MyControls;

namespace NailsChekin
{
    static class Program
    {
        private static Mutex _single;

        private static readonly Stopwatch _sw = Stopwatch.StartNew();
        private static void TraceStep(string name)
        {
            //ONLY TEST !!!!
            try
            {
                //var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "AntPay", "startup.log");
                //System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
                //System.IO.File.AppendAllText(path, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} | {_sw.ElapsedMilliseconds,7} ms | {name}\r\n");

                //LogHelper.SaveLOG_Trace($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} | {_sw.ElapsedMilliseconds,7} ms | {name}\r\n", "TraceStep");
            }
            catch { }
        }

        private static void PurgeTrace(string msg)
        {
            try
            {
                //var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "AntPay", "Logs");
                //System.IO.Directory.CreateDirectory(dir);

                //var path = System.IO.Path.Combine(dir, "purge_startup.log");
                //System.IO.File.AppendAllText(path, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} | {msg}\r\n");

                LogHelper.SaveLOG_Trace($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} | {msg}\r\n", "PurgeTrace");
            }
            catch { }
        }


        [STAThread]
        static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();

            //AutoStartHelper.InitializeFromArgs(args);

            TraceStep("Main() begin");

            bool isDevRun = IsDevRun();

            TraceStep("LoadAllConfigToSystem begin");
            ConfigLocalHelper.LoadAllConfigToSystem();
            TraceStep("LoadAllConfigToSystem done");

            bool registerPurgeTask = args.Any(a => a.Equals("--registerPurgeTask", StringComparison.OrdinalIgnoreCase));
            bool purgeOnly = args.Any(a => a.Equals("--purgeOnly", StringComparison.OrdinalIgnoreCase));

            PurgeTrace("Cmd=" + Environment.CommandLine);
            PurgeTrace("registerPurgeTask=" + registerPurgeTask);
            PurgeTrace("purgeOnly=" + purgeOnly);

            // =========================================================
            // 1) Register purge task mode
            // =========================================================
            if (registerPurgeTask)
            {
                try
                {
                    AutoStartHelper.RegisterPurgeTaskOnly("Ant Pay Retail", Application.ExecutablePath);
                    PurgeTrace("RegisterPurgeTaskOnly: OK");
                }
                catch (Exception ex)
                {
                    LogHelper.SaveLOG_PurgeTrace(ex.ToString(), "RegisterPurgeTaskOnly ERROR");
                    CustomMessageBox.Show(
                        "Cannot register automatic startup printer cleanup.\r\n\r\n" + ex.Message,
                        "Ant Pay Retail",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                return;
            }

            // =========================================================
            // 2) Purge-only mode
            // =========================================================
            if (!isDevRun && purgeOnly)
            {
                RunPurgeOnlyMode();
                return;
            }

            // =========================================================
            // 3) Normal UI mode
            // =========================================================
            if (!AcquireSingleInstanceMutex())
                return;

            TraceStep("AcquireSingleInstanceMutex done");

            SetupFramework();

            if (!isDevRun)
                ConfigCarshApp();

            // Tạo task UI thường sớm, không phụ thuộc internet.
            // Lưu ý: EnsureOptionB_Tasks nên chỉ tạo task UI thường,
            // KHÔNG tạo purge task nữa.
            if (!isDevRun)
            {
                try
                {
                    AutoStartHelper.EnsureOptionB_Tasks("Ant Pay Retail", Application.ExecutablePath);
                }
                catch (Exception ex)
                {
                    LogHelper.SaveLOG_PurgeTrace(ex.ToString(), "EnsureOptionB_Tasks Exception");
                }
            }

            // Có internet mới vào app thường
            if (!Utilitys.IsInternetAvailable())
            {
                TraceStep("IsInternetAvailable = false");
                CustomMessageBox.Show("Internet is not available.");
                return;
            }

            TraceStep("DoStartupTasks begin");
            // Fallback purge nhẹ trong app thường
            DoStartupTasks(isDevRun);

            // Chỉ xin quyền tạo purge task khi thực sự có dùng printer
            if (!isDevRun)
            {
                TryEnsurePurgeTaskAfterConfig();
            }

            TraceStep("Before Application.Run");

            if (isDevRun)
                Run_TestMode();
            else
                Run_RealTime();
        }

        [STAThread]
        static void MainBK()
        {
            TraceStep("Main() begin");
            bool isDevRun = IsDevRun();

            bool purgeOnly = Environment.CommandLine.IndexOf("--purgeOnly", StringComparison.OrdinalIgnoreCase) >= 0;

            // ✅ purge-only mode (task elevated) - không mở UI
            if (!isDevRun && purgeOnly)
            {
                var sw = Stopwatch.StartNew();
                PurgeTrace("=== PurgeOnly START ===");
                PurgeTrace("Cmd=" + Environment.CommandLine);

                try
                {
                    // check elevated
                    bool isAdmin = false;
                    try
                    {
                        var wp = new System.Security.Principal.WindowsPrincipal(
                                     System.Security.Principal.WindowsIdentity.GetCurrent());
                        isAdmin = wp.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
                    }
                    catch { }
                    PurgeTrace("IsAdmin=" + isAdmin);

                    //ConfigLocalHelper.LoadAllConfigToSystem();
                    PurgeTrace("printer_name=" + (Constants.printer_name ?? ""));

                    if (!string.IsNullOrWhiteSpace(Constants.printer_name))
                    {
                        int del = PrinterHelper.PurgeAllPrintJobs_WMI(2, 500);
                        PurgeTrace("WMI deleted=" + del);

                        bool hasSpool = false;
                        try { hasSpool = PrinterHelper.HasSpoolFiles(); } catch { }
                        PurgeTrace("HasSpoolFiles=" + hasSpool);

                        if (del > 0 || hasSpool)
                        {
                            var sw2 = Stopwatch.StartNew();
                            PrinterHelper.HardPurgeSpooler_Silent();
                            PurgeTrace("HardPurge done in " + sw2.ElapsedMilliseconds + "ms");
                        }
                        else
                        {
                            PurgeTrace("HardPurge skipped");
                        }
                    }
                    else
                    {
                        PurgeTrace("Skip purge (printer_name empty)");
                    }
                }
                catch (Exception ex)
                {
                    PurgeTrace("ERROR: " + ex);
                }
                finally
                {
                    PurgeTrace("=== PurgeOnly END | " + sw.ElapsedMilliseconds + "ms ===");
                }
                return;
            }

            // ✅ App UI mode
            if (!AcquireSingleInstanceMutex()) return;
            TraceStep("AcquireSingleInstanceMutex done");

            SetupFramework();

            if (!isDevRun) ConfigCarshApp();

            // ✅ tạo tasks sớm (không phụ thuộc internet)
            if (!isDevRun)
            {
                try { AutoStartHelper.EnsureOptionB_Tasks("Ant Pay Retail", Application.ExecutablePath); }
                catch (Exception ex) { LogHelper.SaveLOG_Crash(ex.ToString(), "EnsureOptionB_Tasks Exception"); }
            }

            // Sau đó mới check internet / load config
            TraceStep("IsInternetAvailable begin");
            if (!Utilitys.IsInternetAvailable())
            {
                TraceStep("IsInternetAvailable = false");
                CustomMessageBox.Show("Internet is not available.");
                return;
            }
            TraceStep("IsInternetAvailable done");

            //TraceStep("LoadAllConfigToSystem begin");
            //ConfigLocalHelper.LoadAllConfigToSystem();
            //TraceStep("LoadAllConfigToSystem done");

            TraceStep("Before Application.Run");
            if (isDevRun) Run_TestMode();
            else Run_RealTime();
        }

        private static void RunPurgeOnlyMode()
        {
            var sw = Stopwatch.StartNew();
            PurgeTrace("=== PurgeOnly START ===");

            try
            {
                bool isAdmin = false;
                try
                {
                    var wp = new System.Security.Principal.WindowsPrincipal(
                        System.Security.Principal.WindowsIdentity.GetCurrent());
                    isAdmin = wp.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
                }
                catch { }

                PurgeTrace("IsAdmin=" + isAdmin);

                if (!isAdmin)
                {
                    PurgeTrace("PurgeOnly running without admin rights.");
                }

                if (string.IsNullOrWhiteSpace(Constants.printer_name))
                {
                    PurgeTrace("Skip purge (printer_name empty)");
                    return;
                }

                int del = PrinterHelper.PurgeAllPrintJobs_WMI(2, 500);
                PurgeTrace("WMI deleted=" + del);

                bool hasSpoolBefore = false;
                try { hasSpoolBefore = PrinterHelper.HasSpoolFiles(); } catch { }
                PurgeTrace("HasSpoolFiles(before)=" + hasSpoolBefore);

                // Mạnh tay để giảm khả năng sót queue/spool lỗi
                PrinterHelper.HardPurgeSpooler_Silent();

                bool hasSpoolAfter = false;
                try { hasSpoolAfter = PrinterHelper.HasSpoolFiles(); } catch { }
                PurgeTrace("HasSpoolFiles(after)=" + hasSpoolAfter);
            }
            catch (Exception ex)
            {
                PurgeTrace("ERROR: " + ex);
            }
            finally
            {
                PurgeTrace("=== PurgeOnly END | " + sw.ElapsedMilliseconds + "ms ===");
            }
        }

        private static void TryEnsurePurgeTaskAfterConfig()
        {
            try
            {
                //if (string.IsNullOrWhiteSpace(Constants.printer_name))
                //{
                //    PurgeTrace("TryEnsurePurgeTaskAfterConfig: skip because printer_name empty");
                //    return;
                //}

                const string purgeTaskName = "Ant Pay Retail - Purge";
                if (AutoStartHelper.HasTask(purgeTaskName))
                {
                    PurgeTrace("TryEnsurePurgeTaskAfterConfig: purge task already exists");
                    return;
                }

                PurgeTrace("TryEnsurePurgeTaskAfterConfig: purge task missing, requesting elevation");
                AutoStartHelper.RequestRegisterPurgeTask();
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_PurgeTrace(ex.ToString(), "TryEnsurePurgeTaskAfterConfig Exception");
            }
        }

        private static void DoStartupTasks(bool isDevRun)
        {
            PurgeTrace("DoStartupTasks | isDevRun=" + isDevRun + " | printer_name=" + (Constants.printer_name ?? ""));

            if (isDevRun)
                return;

            if (string.IsNullOrWhiteSpace(Constants.printer_name))
                return;

            try
            {
                PurgeTrace("DoStartupTasks: PurgePrinterOnStartup running");

                // Ưu tiên purge theo đúng tên printer đã cấu hình
                PrinterHelper.PurgePrinterOnStartup(
                    Constants.printer_name,
                    allowContainsMatch: true,
                    retries: 6,
                    delayMs: 1500);
            }
            catch (Exception ex)
            {
                PurgeTrace("DoStartupTasks Exception: " + ex);
            }
        }

        private static bool AcquireSingleInstanceMutex()
        {
            try
            {
                bool createdNew;
                _single = new Mutex(true, @"Local\AntPayRetail_SingleInstance", out createdNew);

                if (!createdNew)
                {
                    bool isRestart = Environment.CommandLine.IndexOf("--restart", StringComparison.OrdinalIgnoreCase) >= 0;
                    if (!isRestart)
                    {
                        // Không nên MessageBox lúc startup (dễ làm tưởng app không chạy)
                        return false;
                    }

                    try
                    {
                        if (!_single.WaitOne(TimeSpan.FromSeconds(10)))
                            return false;
                    }
                    catch (AbandonedMutexException) { }
                }

                Application.ApplicationExit += (s, e) =>
                {
                    try { _single?.ReleaseMutex(); } catch { }
                    try { _single?.Dispose(); } catch { }
                };

                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                // fallback lần cuối: tên không prefix
                try
                {
                    bool createdNew;
                    _single = new Mutex(true, "AntPay_SingleInstance", out createdNew);
                    return createdNew;
                }
                catch
                {
                    LogHelper.SaveLOG_Crash(ex.ToString(), "AcquireSingleInstanceMutex Unauthorized");
                    return true; // đừng chặn startup vì mutex
                }
            }
        }

        private static void SetupFramework()
        {
            //ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            //ServicePointManager.Expect100Continue = false;
            //ServicePointManager.SetTcpKeepAlive(false, int.MaxValue, 0);

            // 1) TLS: Chỉ cần nếu bạn gọi HTTPS.
            // Nếu API của bạn vẫn là http://... thì phần này không ảnh hưởng.
            // Khuyến nghị: chỉ TLS 1.2 (đừng |= để khỏi giữ TLS cũ).
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            // 2) Tránh 100-Continue gây chậm/đứt ở một số server/proxy
            ServicePointManager.Expect100Continue = false;

            // 3) Tăng giới hạn kết nối đồng thời (mặc định thường rất thấp)
            ServicePointManager.DefaultConnectionLimit = 500;

            // 4) TCP KeepAlive:
            // - Không liên quan trực tiếp tới lỗi "keep-alive closed by server" của HTTP pooling.
            // - Chỉ bật nếu bạn có kết nối idle lâu qua NAT/VPN/Router hay bị drop.
            // Nếu không chắc, để false (mặc định) hoặc bỏ hẳn.
            //ServicePointManager.SetTcpKeepAlive(true, 120_000, 10_000); // 2 phút, 10s

            // 5) UI settings
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = new Font("Segoe UI", 16f);
        }

        private static bool IsDevRun()
        {
            var exe = Application.ExecutablePath;
            var p = exe.Replace('/', '\\').ToLowerInvariant();
            return Debugger.IsAttached
                   || p.Contains(@"\bin\debug\")
                   || p.Contains(@"\bin\release\");
        }

        #region Config Enviroment

        private static void Run_TestMode()
        {
            Constants.system_mode = SYSTEM_MODE.TEST;
            ConfigCarshApp();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
            //Application.Run(new Popup.FormTest());
        }

        private static void Run_RealTime()
        {
            Constants.system_mode = SYSTEM_MODE.REAL;
            ConfigCarshApp();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());

            //var wi = WindowsIdentity.GetCurrent();
            //var wp = new WindowsPrincipal(wi);

            //bool runAsAdmin = wp.IsInRole(WindowsBuiltInRole.Administrator);
            //if (!runAsAdmin)
            //{
            //    // It is not possible to launch a ClickOnce app as administrator directly,
            //    // so instead we launch the app as administrator in a new process.
            //    var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);

            //    // The following properties run the new process as administrator
            //    processInfo.UseShellExecute = true;
            //    processInfo.Verb = "runas";

            //    // Start the new process
            //    try
            //    {
            //        Process.Start(processInfo);
            //    }
            //    catch (Exception)
            //    {
            //        // The user did not allow the application to run as administrator
            //        MessageBox.Show("Sorry, but I don't seem to be able to start " +
            //           "this program with administrator rights!");
            //    }

            //    // Shut down the current process
            //    Application.Exit();
            //}
            //else
            //{
            //    //We are running as administrator
            //    Application.EnableVisualStyles();
            //    Application.SetCompatibleTextRenderingDefault(false);
            //    Application.Run(new FormMain());
            //}
        }

        #endregion

        #region Config Crash APP

        private static void ConfigCarshApp()
        {
            // Set the unhandled exception mode to force all Windows Forms errors to go through
            // our handler.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            //NO UI
            //Application.ThreadException += (sender, args) =>
            //{
            //    Models.Utilitys.SaveLOG_Crash(args.Exception.ToString(), "Thread Exception Big Error");
            //};

            // Add the event handler for handling UI thread exceptions to the event.  ==> UnhandledExceptionMode.CatchException
            Application.ThreadException += new ThreadExceptionEventHandler(Form1_UIThreadException);


            // Add the event handler for handling non-UI thread exceptions to the event.  ==> UnhandledExceptionMode.ThrowException
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        // Handle the UI exceptions by showing a dialog box, and asking the user whether
        // or not they wish to abort execution.
        private static void Form1_UIThreadException(object sender, System.Threading.ThreadExceptionEventArgs t)
        {
            Models.Utilitys.SaveLOG_Crash(t.Exception.ToString(), "Thread Exception Big Error");

            DialogResult result = DialogResult.Cancel;
            try
            {
                result = ShowThreadExceptionDialog("Windows Forms Error", t.Exception);
            }
            catch
            {
                try
                {
                    MessageBox.Show("Fatal Windows Forms Error",
                        "Fatal Windows Forms Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Exit();
                }
            }

            // Exits the program when the user clicks Abort.
            if (result == DialogResult.Abort)
                Application.Exit();
        }

        // Handle the UI exceptions by showing a dialog box, and asking the user whether
        // or not they wish to abort execution.
        // NOTE: This exception cannot be kept from terminating the application - it can only
        // log the event, and inform the user about it.
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;

                string errorMsg = "An application error occurred. Please contact the adminstrator " +
                    "with the following information:\n\n";

                // Since we can't prevent the app from terminating, log this to the event log.
                if (!EventLog.SourceExists("ThreadException"))
                {
                    EventLog.CreateEventSource("ThreadException", "Application");
                }

                // Create an EventLog instance and assign its source.
                EventLog myLog = new EventLog();
                myLog.Source = "ThreadException";
                myLog.WriteEntry(errorMsg + ex.Message + "\n\nStack Trace:\n" + ex.StackTrace);

                //Local Log
                Models.Utilitys.SaveLOG_Crash(ex.Message, "Unhandled Exception Big Error");
            }
            catch (Exception exc)
            {
                try
                {
                    MessageBox.Show("Fatal Non-UI Error",
                        "Fatal Non-UI Error. Could not write the error to the event log. Reason: "
                        + exc.Message, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Exit();
                }
            }
        }

        // Creates the error message and displays it.
        private static DialogResult ShowThreadExceptionDialog(string title, Exception e)
        {
            string errorMsg = "An application error occurred. Please contact the adminstrator " +
                "with the following information:\n\n";
            errorMsg = errorMsg + e.Message + "\n\nStack Trace:\n" + e.StackTrace;
            return MessageBox.Show(errorMsg, title, MessageBoxButtons.AbortRetryIgnore,
                MessageBoxIcon.Stop);
        }

        #endregion

    }
}
