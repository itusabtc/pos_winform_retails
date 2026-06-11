using NailsChekin.MyControls;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Windows.Forms;

namespace NailsChekin.Models.Helper
{
    public static class AutoStartHelper
    {
        private const string RUN_KEY = @"Software\Microsoft\Windows\CurrentVersion\Run";

        // Task Scheduler constants
        private const int TASK_TRIGGER_LOGON = 9;
        private const int TASK_ACTION_EXEC = 0;

        private const int TASK_CREATE_OR_UPDATE = 6;
        private const int TASK_LOGON_INTERACTIVE_TOKEN = 3;

        private const int TASK_RUNLEVEL_LUA = 0;      // chạy thường
        private const int TASK_RUNLEVEL_HIGHEST = 1;  // chạy cao nhất

        private static bool IsElevated()
        {
            try
            {
                var wp = new WindowsPrincipal(WindowsIdentity.GetCurrent());
                return wp.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch { return false; }
        }

        public static void EnsureOptionB_Tasks(string baseName, string exePath)
        {
            // 1) Task mở app (không elevated) - tạo được cả khi không admin (đa số máy OK)
            try
            {
                EnsureTask(
                    taskName: baseName,
                    exePath: exePath,
                    arguments: "",                 // mở app bình thường
                    runLevel: TASK_RUNLEVEL_LUA,   // không elevated
                    delayIso8601: "PT8S"           // delay 8s để purge kịp chạy trước
                );
            }
            catch { }

            // 2) Task purge (elevated) - chỉ tạo khi có admin (installer) => hiện không tạo được và đã tách 1 step riêng
            //if (!IsElevated()) return;

            //try
            //{
            //    EnsureTask(
            //        taskName: baseName + " - Purge",
            //        exePath: exePath,
            //        arguments: "--purgeOnly",
            //        runLevel: TASK_RUNLEVEL_HIGHEST,
            //        delayIso8601: "PT0S"
            //    );
            //}
            //catch { }
        }

        private static void EnsureTask(string taskName, string exePath, string arguments, int runLevel, string delayIso8601)
        {
            Type tsType = Type.GetTypeFromProgID("Schedule.Service");
            if (tsType == null) throw new Exception("Task Scheduler service not available.");

            dynamic service = Activator.CreateInstance(tsType);
            service.Connect();

            dynamic root = service.GetFolder("\\");
            dynamic task = service.NewTask(0);

            string userId = WindowsIdentity.GetCurrent().Name;

            task.RegistrationInfo.Description = "AntPay auto tasks (created/updated by app).";

            // Principal
            task.Principal.UserId = userId;
            task.Principal.LogonType = TASK_LOGON_INTERACTIVE_TOKEN;
            task.Principal.RunLevel = runLevel;

            // Trigger: logon
            dynamic trigger = task.Triggers.Create(TASK_TRIGGER_LOGON);
            trigger.UserId = userId;
            trigger.Delay = string.IsNullOrWhiteSpace(delayIso8601) ? "PT0S" : delayIso8601;

            // Action
            dynamic action = task.Actions.Create(TASK_ACTION_EXEC);
            action.Path = exePath;
            action.Arguments = arguments ?? "";
            action.WorkingDirectory = Path.GetDirectoryName(exePath);

            // Settings
            task.Settings.StartWhenAvailable = true;
            task.Settings.DisallowStartIfOnBatteries = false;
            task.Settings.StopIfGoingOnBatteries = false;
            task.Settings.ExecutionTimeLimit = "PT0S";
            task.Settings.MultipleInstances = 0; // ignore new

            root.RegisterTaskDefinition(taskName, task, TASK_CREATE_OR_UPDATE, null, null, TASK_LOGON_INTERACTIVE_TOKEN, null);
        }

        public static void EnsureAutoStart_TaskScheduler(string taskName, string exePath, string arguments)
        {
            if (string.IsNullOrWhiteSpace(taskName)) throw new ArgumentNullException(nameof(taskName));
            if (string.IsNullOrWhiteSpace(exePath)) throw new ArgumentNullException(nameof(exePath));

            // COM: Schedule.Service
            Type tsType = Type.GetTypeFromProgID("Schedule.Service");
            if (tsType == null) throw new Exception("Task Scheduler service not available.");

            dynamic service = Activator.CreateInstance(tsType);
            service.Connect();

            dynamic root = service.GetFolder("\\");
            dynamic task = service.NewTask(0);

            // Info
            task.RegistrationInfo.Description = "Ant Pay AI auto-start at logon (installed by app).";

            // Principal
            string userId = WindowsIdentity.GetCurrent().Name;  // DOMAIN\User or PC\User
            task.Principal.UserId = userId;
            task.Principal.LogonType = TASK_LOGON_INTERACTIVE_TOKEN;
            task.Principal.RunLevel = TASK_RUNLEVEL_HIGHEST;

            // Trigger: Logon
            dynamic trigger = task.Triggers.Create(TASK_TRIGGER_LOGON);
            trigger.UserId = userId;
            trigger.Delay = "PT5S"; // ISO 8601 duration, e.g. PT5S = 5 seconds

            // Action: Exec
            dynamic action = task.Actions.Create(TASK_ACTION_EXEC);
            action.Path = exePath;
            action.Arguments = arguments ?? "";
            action.WorkingDirectory = Path.GetDirectoryName(exePath);

            // Settings
            task.Settings.StartWhenAvailable = true;
            task.Settings.DisallowStartIfOnBatteries = false;
            task.Settings.StopIfGoingOnBatteries = false;
            task.Settings.ExecutionTimeLimit = "PT0S"; // no limit
            task.Settings.MultipleInstances = 0; // ignore new (0 = TASK_INSTANCES_IGNORE_NEW)

            // Register (Create or Update)
            // If bạn muốn task chạy cho mọi user: cần tạo task dưới SYSTEM hoặc group -> phức tạp hơn.
            root.RegisterTaskDefinition(taskName, task, TASK_CREATE_OR_UPDATE, null, null, TASK_LOGON_INTERACTIVE_TOKEN, null);
        }

        public static bool HasTask(string taskName)
        {
            if (string.IsNullOrWhiteSpace(taskName))
                return false;

            try
            {
                Type tsType = Type.GetTypeFromProgID("Schedule.Service");
                if (tsType == null)
                    return false;

                dynamic service = Activator.CreateInstance(tsType);
                service.Connect();

                dynamic root = service.GetFolder("\\");
                dynamic task = null;

                try
                {
                    task = root.GetTask(taskName);
                }
                catch
                {
                    task = null;
                }

                return task != null;
            }
            catch (Exception ex)
            {
                try
                {
                    LogHelper.SaveLOG_Crash(ex.ToString(), "HasTask ERROR");
                }
                catch { }

                return false;
            }
        }

        public static bool RequestRegisterPurgeTask()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = Application.ExecutablePath,
                    Arguments = "--registerPurgeTask",
                    Verb = "runas",
                    UseShellExecute = true,
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                };

                var p = Process.Start(psi);

                try
                {
                    if (p != null)
                    {
                        p.WaitForExit(15000);
                    }
                }
                catch { }

                bool created = HasTask("Ant Pay AI - Purge");

                try
                {
                    LogHelper.SaveLOG_PurgeTrace(
                        "RequestRegisterPurgeTask finished. Created=" + created,
                        "RequestRegisterPurgeTask");
                }
                catch { }

                if (!created)
                {
                    try
                    {
                        CustomMessageBox.Show(
                            "Automatic startup printer cleanup has not been enabled yet.\r\n\r\n" +
                            "Please allow the Windows permission prompt to enable it.",
                            "Ant Pay",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                    catch { }
                }

                return created;
            }
            catch (Exception ex)
            {
                try
                {
                    LogHelper.SaveLOG_Crash(ex.ToString(), "RequestRegisterPurgeTask ERROR");
                }
                catch { }

                try
                {
                    CustomMessageBox.Show(
                        "Automatic startup printer cleanup has not been enabled yet.\r\n\r\n" +
                        "Please allow the Windows permission prompt to enable it.",
                        "Ant Pay",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                catch { }

                return false;
            }
        }

        public static void RegisterPurgeTaskOnly(string baseName, string exePath)
        {
            if (string.IsNullOrWhiteSpace(baseName))
                throw new ArgumentNullException(nameof(baseName));

            if (string.IsNullOrWhiteSpace(exePath))
                throw new ArgumentNullException(nameof(exePath));

            if (!File.Exists(exePath))
                throw new FileNotFoundException("EXE not found.", exePath);

            string taskName = baseName + " - Purge";

            try
            {
                Type tsType = Type.GetTypeFromProgID("Schedule.Service");
                if (tsType == null)
                    throw new Exception("Task Scheduler service not available.");

                dynamic service = Activator.CreateInstance(tsType);
                service.Connect();

                dynamic root = service.GetFolder("\\");
                dynamic task = service.NewTask(0);

                string userId = WindowsIdentity.GetCurrent().Name;

                task.RegistrationInfo.Description = "AntPay purge spooler task (created by app).";

                // Principal
                task.Principal.UserId = userId;
                task.Principal.LogonType = TASK_LOGON_INTERACTIVE_TOKEN;
                task.Principal.RunLevel = TASK_RUNLEVEL_HIGHEST;

                // Trigger: logon
                dynamic trigger = task.Triggers.Create(TASK_TRIGGER_LOGON);
                trigger.UserId = userId;

                // Delay nhẹ để Windows Spooler/printer driver kịp load
                trigger.Delay = "PT3S";

                // Action
                dynamic action = task.Actions.Create(TASK_ACTION_EXEC);
                action.Path = exePath;
                action.Arguments = "--purgeOnly";
                action.WorkingDirectory = Path.GetDirectoryName(exePath);

                // Settings
                task.Settings.StartWhenAvailable = true;
                task.Settings.DisallowStartIfOnBatteries = false;
                task.Settings.StopIfGoingOnBatteries = false;

                // Quan trọng: không để PT3S, vì purge nhiều vòng sẽ bị kill sớm
                task.Settings.ExecutionTimeLimit = "PT1M";

                task.Settings.MultipleInstances = 0; // ignore new
                task.Settings.Hidden = true;

                root.RegisterTaskDefinition(
                    taskName,
                    task,
                    TASK_CREATE_OR_UPDATE,
                    null,
                    null,
                    TASK_LOGON_INTERACTIVE_TOKEN,
                    null
                );

                try
                {
                    LogHelper.SaveLOG_PurgeTrace(
                        $"RegisterPurgeTaskOnly OK | Task={taskName} | User={userId} | Exe={exePath}",
                        "RegisterPurgeTaskOnly");
                }
                catch { }
            }
            catch (Exception ex)
            {
                try
                {
                    LogHelper.SaveLOG_Crash(ex.ToString(), "RegisterPurgeTaskOnly ERROR");
                }
                catch { }

                throw;
            }
        }

        public static bool IsInStartup(string appName)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RUN_KEY, false))
            {
                var val = key?.GetValue(appName) as string;
                return !string.IsNullOrEmpty(val);
            }
        }

        public static void AddToStartup(string appName)
        {
            var exePath = Application.ExecutablePath;
            var value = $"\"{exePath}\""; // nếu cần params: "\"{exePath}\" --minimized"

            using (var key = Registry.CurrentUser.OpenSubKey(RUN_KEY, true))
            {
                if (key == null) throw new Exception("Cannot open HKCU Run key");
                key.SetValue(appName, value);
            }
        }

        public static void RemoveFromStartup(string appName)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RUN_KEY, true))
            {
                key?.DeleteValue(appName, false);
            }
        }
    }
}
