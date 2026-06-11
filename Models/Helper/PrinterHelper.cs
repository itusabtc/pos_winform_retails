using NailsChekin.MyControls;
using DevExpress.XtraReports.UI;
using System;
using System.Drawing.Printing;
using System.IO;
using System.Management;
using System.Printing;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;

namespace NailsChekin.Models
{
    public static class PrinterHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)] public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)] public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)] public string pDataType;
        }

        [DllImport("winspool.Drv", SetLastError = true)] public static extern bool OpenPrinter(string pPrinterName, out IntPtr phPrinter, IntPtr pDefault);
        [DllImport("winspool.Drv", SetLastError = true)] public static extern bool ClosePrinter(IntPtr hPrinter);
        [DllImport("winspool.Drv", SetLastError = true)] public static extern bool StartDocPrinter(IntPtr hPrinter, int level, ref DOCINFOA di);
        [DllImport("winspool.Drv", SetLastError = true)] public static extern bool EndDocPrinter(IntPtr hPrinter);
        [DllImport("winspool.Drv", SetLastError = true)] public static extern bool StartPagePrinter(IntPtr hPrinter);
        [DllImport("winspool.Drv", SetLastError = true)] public static extern bool EndPagePrinter(IntPtr hPrinter);
        [DllImport("winspool.Drv", SetLastError = true)] public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

        // =========================================================
        // FIX #4: Thêm lệnh ESC @ để reset buffer nội bộ của máy in
        // POS-80C có thể còn lệnh ESC/POS dở từ session trước
        // ESC @ = 0x1B 0x40 = Initialize Printer (xóa sạch buffer lệnh)
        // =========================================================
        public static void ResetPrinterEscPos(string printerName)
        {
            if (string.IsNullOrWhiteSpace(printerName)) return;

            // ESC @ : Initialize Printer — xóa hoàn toàn buffer lệnh của POS-80C
            // Phải gửi trước mọi lệnh in để tránh rác từ session trước
            byte[] resetCmd = new byte[] { 0x1B, 0x40 };

            IntPtr hPrinter = IntPtr.Zero;
            IntPtr pBytes = IntPtr.Zero;
            try
            {
                if (!OpenPrinter(printerName.Normalize(), out hPrinter, IntPtr.Zero))
                    return;

                DOCINFOA di = new DOCINFOA { pDocName = "ResetPrinter", pDataType = "RAW" };
                if (!StartDocPrinter(hPrinter, 1, ref di)) return;
                if (!StartPagePrinter(hPrinter)) { EndDocPrinter(hPrinter); return; }

                pBytes = Marshal.AllocHGlobal(resetCmd.Length);
                Marshal.Copy(resetCmd, 0, pBytes, resetCmd.Length);
                WritePrinter(hPrinter, pBytes, resetCmd.Length, out int _);

                EndPagePrinter(hPrinter);
                EndDocPrinter(hPrinter);
            }
            catch { /* best-effort */ }
            finally
            {
                if (pBytes != IntPtr.Zero) Marshal.FreeHGlobal(pBytes);
                if (hPrinter != IntPtr.Zero) ClosePrinter(hPrinter);
            }
        }

        public static void PrintReport(XtraReport report, string printerName, bool openCashDrawer = false)
        {
            var tool = new ReportPrintTool(report);
            tool.Print(printerName);
            if (openCashDrawer) SafeOpenDrawer(printerName);
        }

        public static void OpenDrawer(string printerName)
        {
            byte[] openDrawerCommand = new byte[] { 0x1B, 0x70, 0x00, 0x19, 0xFA };
            IntPtr hPrinter;
            DOCINFOA di = new DOCINFOA { pDocName = "OpenCashDrawer", pDataType = "RAW" };

            if (OpenPrinter(printerName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                if (StartDocPrinter(hPrinter, 1, ref di))
                {
                    if (StartPagePrinter(hPrinter))
                    {
                        IntPtr pBytes = Marshal.AllocHGlobal(openDrawerCommand.Length);
                        Marshal.Copy(openDrawerCommand, 0, pBytes, openDrawerCommand.Length);
                        WritePrinter(hPrinter, pBytes, openDrawerCommand.Length, out int _);
                        Marshal.FreeHGlobal(pBytes);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
        }

        public static void SafeOpenDrawer(string printerName)
        {
            byte[] openDrawerCommand = new byte[] { 0x1B, 0x70, 0x00, 0x19, 0xFA };
            IntPtr hPrinter = IntPtr.Zero;
            IntPtr pBytes = IntPtr.Zero;

            try
            {
                if (OpenPrinter(printerName.Normalize(), out hPrinter, IntPtr.Zero))
                {
                    DOCINFOA di = new DOCINFOA { pDocName = "OpenCashDrawer", pDataType = "RAW" };
                    if (StartDocPrinter(hPrinter, 1, ref di))
                    {
                        if (StartPagePrinter(hPrinter))
                        {
                            pBytes = Marshal.AllocHGlobal(openDrawerCommand.Length);
                            Marshal.Copy(openDrawerCommand, 0, pBytes, openDrawerCommand.Length);
                            WritePrinter(hPrinter, pBytes, openDrawerCommand.Length, out int _);
                        }
                    }
                }
            }
            finally
            {
                if (pBytes != IntPtr.Zero) Marshal.FreeHGlobal(pBytes);
                if (hPrinter != IntPtr.Zero)
                {
                    EndPagePrinter(hPrinter);
                    EndDocPrinter(hPrinter);
                    ClosePrinter(hPrinter);
                }
            }
        }

        public static void ClearPrintQueue(string printerName)
        {
            try
            {
                using (PrintServer ps = new PrintServer())
                {
                    PrintQueue pq = ps.GetPrintQueue(printerName);
                    pq.Purge();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ClearPrintQueue error: " + ex.Message);
            }
        }

        public static void ClearAllPrintersQueue()
        {
            try
            {
                using (PrintServer ps = new PrintServer())
                {
                    foreach (PrintQueue pq in ps.GetPrintQueues())
                    {
                        try { pq.Purge(); }
                        catch (Exception ex)
                        {
                            CustomMessageBox.Show($"Error clearing {pq.FullName}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Clear all print queues failed: " + ex.Message);
            }
        }

        private static bool IsAdmin()
        {
            using (var id = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(id);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        // =========================================================
        // FIX #1 + #3: Sửa PurgePrinterOnStartup
        // Lỗi cũ: Chờ Spooler CHẠY → Spooler đã dispatch job sang máy in rồi mới purge
        // Fix:    Dừng Spooler TRƯỚC, xóa file, khởi động lại Spooler sạch
        //         Sau đó mới dùng WMI cleanup các job còn lại (nếu có)
        // =========================================================
        public static int PurgePrinterOnStartup(string needle, bool allowContainsMatch = true,
                                                int retries = 6, int delayMs = 1500)
        {
            if (string.IsNullOrWhiteSpace(needle)) return 0;

            // BƯỚC 1: Dừng Spooler và xóa spool file TRƯỚC KHI nó kịp dispatch job
            // Đây là bước quan trọng nhất — không được chờ Spooler running
            HardPurgeSpooler_Silent();

            // BƯỚC 2: Sau khi Spooler đã restart sạch, đợi printer xuất hiện
            for (int i = 0; i < retries; i++)
            {
                if (AnyPrinterMatch(needle, allowContainsMatch)) break;
                Thread.Sleep(delayMs);
            }

            // BƯỚC 3: Cleanup bổ sung bằng WMI cho các job lỡ xuất hiện sau restart
            int deleted = PurgeJobsByPrinterNameContains_WMI(needle, allowContainsMatch);

            // BƯỚC 4: Gửi ESC @ để reset buffer nội bộ của máy in
            // Xử lý trường hợp máy in còn lệnh dở từ session trước
            try
            {
                ResetPrinterEscPos(needle);
            }
            catch { /* best-effort */ }

            return deleted;
        }

        private static bool AnyPrinterMatch(string needle, bool contains)
        {
            try
            {
                using (var s = new ManagementObjectSearcher("SELECT Name FROM Win32_Printer"))
                {
                    foreach (ManagementObject p in s.Get())
                    {
                        var name = p["Name"]?.ToString() ?? "";
                        bool match = string.Equals(name, needle, StringComparison.OrdinalIgnoreCase)
                                     || (contains && name.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0);
                        if (match) { p.Dispose(); return true; }
                        p.Dispose();
                    }
                }
            }
            catch { }
            return false;
        }

        private static int PurgeJobsByPrinterNameContains_WMI(string needle, bool contains)
        {
            if (string.IsNullOrWhiteSpace(needle)) return 0;

            int deleted = 0;
            string esc = needle.Replace("\\", "\\\\").Replace("'", "''");
            string like = contains ? $"%{esc}%,%" : $"{esc},%";
            string query = $"SELECT * FROM Win32_PrintJob WHERE Name LIKE '{like}'";

            try
            {
                using (var s = new ManagementObjectSearcher(query))
                using (var jobs = s.Get())
                {
                    foreach (ManagementObject job in jobs)
                    {
                        try
                        {
                            var name = (job["Name"]?.ToString() ?? "").Trim();
                            int lastComma = name.LastIndexOf(',');
                            string printerName = (lastComma > 0 ? name.Substring(0, lastComma) : name).Trim();

                            bool match = string.Equals(printerName, needle, StringComparison.OrdinalIgnoreCase)
                                         || (contains && printerName.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0);
                            if (!match) continue;

                            object ret = job.InvokeMethod("Delete", null);
                            int code = 0;
                            try { code = Convert.ToInt32(ret); } catch { }
                            if (code == 0) deleted++;
                        }
                        catch { }
                        finally { job.Dispose(); }
                    }
                }
            }
            catch { }

            return deleted;
        }

        private static void HardPurgeSpooler()
        {
            try
            {
                using (var sc = new ServiceController("Spooler"))
                {
                    if (sc.Status != ServiceControllerStatus.Stopped &&
                        sc.Status != ServiceControllerStatus.StopPending)
                        sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(15));
                }

                string dir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                    @"System32\spool\PRINTERS");

                if (Directory.Exists(dir))
                {
                    foreach (var f in Directory.GetFiles(dir))
                    {
                        try { File.Delete(f); } catch { }
                    }
                }

                using (var sc2 = new ServiceController("Spooler"))
                {
                    sc2.Start();
                    sc2.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(15));
                }
            }
            catch { }
        }

        public static int PurgeAllPrintJobs_WMI(int retries = 3, int delayMs = 800)
        {
            int deletedTotal = 0;

            for (int attempt = 0; attempt < retries; attempt++)
            {
                int deleted = 0;
                try
                {
                    using (var s = new ManagementObjectSearcher("SELECT * FROM Win32_PrintJob"))
                    using (var jobs = s.Get())
                    {
                        bool any = false;
                        foreach (ManagementObject job in jobs)
                        {
                            any = true;
                            try
                            {
                                object ret = job.InvokeMethod("Delete", null);
                                int code = 0;
                                try { code = Convert.ToInt32(ret); } catch { }
                                if (code == 0) deleted++;
                            }
                            catch { }
                            finally { job.Dispose(); }
                        }

                        if (!any) break;
                    }
                }
                catch { }

                deletedTotal += deleted;
                if (deleted == 0) break;

                Thread.Sleep(delayMs);
            }

            return deletedTotal;
        }

        // =========================================================
        // FIX #2: Sửa HardPurgeSpooler_Silent - lỗi capture file list trước khi stop
        // Lỗi cũ:
        //   1. Lấy danh sách file → Spooler đang StartPending → files có thể rỗng → return oan
        //   2. Stop() trên Spooler đang StartPending → ném exception → catch nuốt im
        //      → code tiếp tục xóa file trong khi Spooler vẫn chạy → file bị lock → xóa thất bại
        // Fix:
        //   1. Dừng Spooler TRƯỚC bằng TryStopSpooler() xử lý mọi trạng thái
        //   2. Liệt kê file SAU KHI Spooler đã dừng (không bị lock)
        //   3. Không cần check file tồn tại trước khi stop — stop luôn để đảm bảo
        // =========================================================
        public static void HardPurgeSpooler_Silent()
        {
            try
            {
                string dir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                    @"System32\spool\PRINTERS");

                if (!Directory.Exists(dir)) return;

                // BƯỚC 1: Dừng Spooler trước — không check file trước nữa
                // Vì Spooler có thể đang StartPending, phải handle đủ các trạng thái
                bool stopped = TryStopSpoolerRobust(TimeSpan.FromSeconds(20));

                // BƯỚC 2: Liệt kê file SAU KHI Spooler đã dừng
                // Lúc này file không bị lock → xóa thành công
                string[] files;
                try { files = Directory.GetFiles(dir); }
                catch { files = new string[0]; }

                // Xóa tất cả spool file (.SPL, .SHD)
                foreach (var f in files)
                {
                    try { File.Delete(f); } catch { /* file bị lock bởi process khác — bỏ qua */ }
                }

                // BƯỚC 3: Khởi động lại Spooler sạch
                TryStartSpoolerRobust(TimeSpan.FromSeconds(20));
            }
            catch { /* toàn bộ luồng purge thất bại — im lặng, không crash app */ }
        }

        // =========================================================
        // Helper: Dừng Spooler xử lý đủ các trạng thái (Running, StartPending, StopPending...)
        // Trả về true nếu Stopped thành công, false nếu hết timeout
        // =========================================================
        private static bool TryStopSpoolerRobust(TimeSpan timeout)
        {
            var deadline = DateTime.UtcNow + timeout;

            // Thử tối đa 3 lần vì service có thể transition trạng thái
            for (int attempt = 0; attempt < 3; attempt++)
            {
                try
                {
                    using (var sc = new ServiceController("Spooler"))
                    {
                        // Refresh để lấy trạng thái thực tế
                        sc.Refresh();
                        var status = sc.Status;

                        if (status == ServiceControllerStatus.Stopped)
                            return true; // Đã stopped rồi

                        if (status == ServiceControllerStatus.StopPending)
                        {
                            // Đang trên đường dừng — chờ nó dừng hẳn
                            var remaining = deadline - DateTime.UtcNow;
                            if (remaining <= TimeSpan.Zero) return false;
                            sc.WaitForStatus(ServiceControllerStatus.Stopped, remaining);
                            return true;
                        }

                        if (status == ServiceControllerStatus.StartPending)
                        {
                            // Đang khởi động — đợi nó Running rồi mới Stop được
                            var remaining = deadline - DateTime.UtcNow;
                            if (remaining <= TimeSpan.Zero) return false;
                            TimeSpan waitStart = remaining < TimeSpan.FromSeconds(5) ? remaining : TimeSpan.FromSeconds(5);
                            try { sc.WaitForStatus(ServiceControllerStatus.Running, waitStart); }
                            catch { }
                            // Refresh lại rồi thử Stop trong vòng lặp kế tiếp
                            Thread.Sleep(200);
                            continue;
                        }

                        if (status == ServiceControllerStatus.Running ||
                            status == ServiceControllerStatus.Paused ||
                            status == ServiceControllerStatus.ContinuePending ||
                            status == ServiceControllerStatus.PausePending)
                        {
                            sc.Stop();
                            var remaining = deadline - DateTime.UtcNow;
                            if (remaining <= TimeSpan.Zero) return false;
                            sc.WaitForStatus(ServiceControllerStatus.Stopped, remaining);
                            return true;
                        }
                    }
                }
                catch
                {
                    // WaitForStatus hoặc Stop ném exception — thử lại sau 500ms
                    Thread.Sleep(500);
                }
            }

            return false;
        }

        // =========================================================
        // Helper: Khởi động lại Spooler xử lý đủ các trạng thái
        // =========================================================
        private static bool TryStartSpoolerRobust(TimeSpan timeout)
        {
            var deadline = DateTime.UtcNow + timeout;

            for (int attempt = 0; attempt < 3; attempt++)
            {
                try
                {
                    using (var sc = new ServiceController("Spooler"))
                    {
                        sc.Refresh();
                        var status = sc.Status;

                        if (status == ServiceControllerStatus.Running)
                            return true;

                        if (status == ServiceControllerStatus.StartPending)
                        {
                            var remaining = deadline - DateTime.UtcNow;
                            if (remaining <= TimeSpan.Zero) return false;
                            sc.WaitForStatus(ServiceControllerStatus.Running, remaining);
                            return true;
                        }

                        if (status == ServiceControllerStatus.Stopped)
                        {
                            sc.Start();
                            var remaining = deadline - DateTime.UtcNow;
                            if (remaining <= TimeSpan.Zero) return false;
                            sc.WaitForStatus(ServiceControllerStatus.Running, remaining);
                            return true;
                        }
                    }
                }
                catch
                {
                    Thread.Sleep(500);
                }
            }

            return false;
        }

        public static void SafePurgePOSQueuesOnExit()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT Name, JobStatus FROM Win32_PrintJob"))
                {
                    foreach (ManagementObject job in searcher.Get())
                    {
                        try
                        {
                            var full = (job["Name"]?.ToString() ?? "");
                            var printerName = full.Split(',')[0].Trim();
                            if (printerName.IndexOf("POS", StringComparison.OrdinalIgnoreCase) < 0)
                                continue;

                            var js = (job["JobStatus"]?.ToString() ?? "").ToLowerInvariant();
                            if (js.Contains("printing") || js.Contains("spooling"))
                                continue;

                            job.InvokeMethod("Delete", null);
                        }
                        catch { }
                        finally { job.Dispose(); }
                    }
                }
            }
            catch { }
        }

        public static bool ShouldPurgePrinter()
        {
            if (!string.IsNullOrWhiteSpace(Constants.printer_name))
                return true;
            try
            {
                return PrinterSettings.InstalledPrinters != null &&
                       PrinterSettings.InstalledPrinters.Count > 0;
            }
            catch { return false; }
        }

        public static bool HasSpoolFiles()
        {
            try
            {
                string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                                          @"System32\spool\PRINTERS");
                return Directory.Exists(dir) && Directory.GetFiles(dir).Length > 0;
            }
            catch { return false; }
        }
    }
}
