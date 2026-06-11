using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace NailsChekin.Models.Helper
{
    public enum ReportMode { Physical, Perceived }

    public static class MonitorSizeHelper
    {
        // ===================== Public Models =====================
        public sealed class MonitorInfo
        {
            public string Name { get; set; }
            public string DeviceName { get; set; }     // \\.\DISPLAY1
            public string PnpDeviceId { get; set; }    // DISPLAY\ACR0621\...
            public Rectangle Bounds { get; set; }

            public int WidthCm { get; set; }           // EDID cm (0 nếu không có)
            public int HeightCm { get; set; }          // EDID cm (0 nếu không có)

            public int PixelWidth { get; set; }        // pixel thật từ EnumDisplaySettings
            public int PixelHeight { get; set; }

            public float RawDpiX { get; set; }
            public float RawDpiY { get; set; }

            public float EffectiveDpiX { get; set; }
            public float EffectiveDpiY { get; set; }

            public int ScalePercent { get; set; }      // 100, 125, 150...
            public double DiagonalInch { get; set; }   // kết quả cuối (làm tròn 0.5")
        }

        // ===================== API =====================
        [DllImport("user32.dll")] private static extern uint GetDpiForWindow(IntPtr hWnd);

        [DllImport("shcore.dll")]
        private static extern int GetDpiForMonitor(IntPtr hmonitor, int dpiType, out uint dpiX, out uint dpiY);

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT { public int X; public int Y; }

        private const int MDT_EFFECTIVE_DPI = 0; // gồm Scale
        private const int MDT_RAW_DPI = 2;       // DPI vật lý
        private const uint MONITOR_DEFAULTTONEAREST = 2;

        // EnumDisplayDevices để map DISPLAY1 -> PNP ID
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DISPLAY_DEVICE
        {
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;   // \\.\DISPLAY1
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            public int StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;     // DISPLAY\ACR0621\...
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }

        // EnumDisplaySettings để lấy pixel thật
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DEVMODE
        {
            private const int CCHDEVICENAME = 32;
            private const int CCHFORMNAME = 32;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
            public string dmDeviceName;

            public short dmSpecVersion, dmDriverVersion, dmSize, dmDriverExtra;
            public int dmFields;

            public int dmPositionX, dmPositionY;
            public int dmDisplayOrientation, dmDisplayFixedOutput;

            public short dmColor, dmDuplex, dmYResolution, dmTTOption, dmCollate;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
            public string dmFormName;

            public short dmLogPixels;
            public int dmBitsPerPel, dmPelsWidth, dmPelsHeight;
            public int dmDisplayFlags, dmDisplayFrequency;

            public int dmICMMethod, dmICMIntent, dmMediaType, dmDitherType, dmReserved1, dmReserved2;
            public int dmPanningWidth, dmPanningHeight;
        }

        // ===================== Main: Get Monitor Info =====================
        /// <summary>
        /// Trả về thông tin màn hình đang chứa cửa sổ hwnd.
        /// mode = Physical: inch thật; Perceived: inch quy đổi theo Scale (physical * scale) để phục vụ layout.
        /// </summary>
        public static MonitorInfo GetCurrentMonitorInfo(IntPtr hwnd, ReportMode mode = ReportMode.Physical)
        {
            var screen = Screen.FromHandle(hwnd);

            var info = new MonitorInfo
            {
                Name = "Current Screen",
                Bounds = screen.Bounds,
                DeviceName = screen.DeviceName
            };

            // 1) Lấy PNP Device ID theo \\.\DISPLAY1
            info.PnpDeviceId = GetPnpDeviceIdByDisplayName(screen.DeviceName);

            // 2) Lấy EDID cm theo PNP key (DISPLAY\XXXX)
            var edidMap = GetEdidMapByDisplayKey(); // key: DISPLAY\ACR0621
            string displayKey = ToDisplayKey(info.PnpDeviceId); // DISPLAY\ACR0621

            if (!string.IsNullOrEmpty(displayKey) && edidMap.TryGetValue(displayKey, out var edid))
            {
                info.Name = edid.Name ?? displayKey;
                info.WidthCm = edid.WidthCm;
                info.HeightCm = edid.HeightCm;
            }

            // 3) Pixel thật của monitor (rất quan trọng để tránh sai do DPI virtualization)
            var (pxW, pxH) = GetMonitorPixels(screen.DeviceName);
            info.PixelWidth = pxW;
            info.PixelHeight = pxH;

            // 4) DPI raw/effective của monitor
            var (rawX, rawY) = GetDpiForScreen(screen, useEffective: false);
            var (effX, effY) = GetDpiForScreen(screen, useEffective: true);
            info.RawDpiX = rawX;
            info.RawDpiY = rawY;
            info.EffectiveDpiX = effX;
            info.EffectiveDpiY = effY;

            // 5) Scale theo window (96->100%)
            info.ScalePercent = GetScalePercentForWindow(hwnd);

            // 6) Tính physical inches
            double physicalIn = ComputePhysicalInches(info, screen);

            // 7) Xuất theo mode
            if (mode == ReportMode.Physical)
            {
                info.DiagonalInch = RoundHalf(physicalIn);
            }
            else
            {
                // Perceived = inch quy đổi theo Scale để phục vụ layout (logic bạn đang dùng)
                double scale = info.ScalePercent / 100.0;
                info.DiagonalInch = RoundHalf(physicalIn * scale);

                // Nếu bạn muốn "inch cảm nhận" đúng nghĩa theo effective DPI:
                // info.DiagonalInch = RoundHalf(physicalIn / scale);
            }

            return info;
        }

        public static double GetCurrentMonitorInches(IntPtr hwnd, ReportMode mode = ReportMode.Physical)
            => GetCurrentMonitorInfo(hwnd, mode).DiagonalInch;

        // ===================== Your Is_MiniScreen Logic =====================
        /// <summary>
        /// Logic bạn yêu cầu:
        /// - Physical >= 24  => false
        /// - Nếu Laptop (physical nhỏ) nhưng Scale=100 và Res=1920x1080 => false
        /// - Còn lại => true
        /// </summary>
        public static bool IsMiniScreen(IntPtr hwnd)
        {
            var info = GetCurrentMonitorInfo(hwnd, ReportMode.Physical);

            // Rule 1: Physical >= 24 => NOT mini
            if (IsValidInch(info.DiagonalInch) && info.DiagonalInch >= 24.0)
                return false;

            // Rule 2: laptop-like but 100% + 1920x1080 => NOT mini
            int scale = info.ScalePercent;
            int w = info.PixelWidth;
            int h = info.PixelHeight;

            bool is1080p = (w == 1920 && h == 1080) || (w == 1080 && h == 1920);
            if (scale == 100 && is1080p)
                return false;

            return true;
        }

        // ===================== Compute Inches =====================
        private static double ComputePhysicalInches(MonitorInfo info, Screen screen)
        {
            // A) EDID có => dùng EDID
            if (info.WidthCm > 0 && info.HeightCm > 0)
            {
                return Math.Sqrt(info.WidthCm * info.WidthCm + info.HeightCm * info.HeightCm) / 2.54;
            }

            // B) Fallback: pixel thật / RAW DPI
            float rawX = info.RawDpiX > 0 ? info.RawDpiX : 96;
            float rawY = info.RawDpiY > 0 ? info.RawDpiY : 96;

            int pxW = info.PixelWidth > 0 ? info.PixelWidth : screen.Bounds.Width;
            int pxH = info.PixelHeight > 0 ? info.PixelHeight : screen.Bounds.Height;

            double wIn = pxW / rawX;
            double hIn = pxH / rawY;

            // set cm approximate (chỉ để debug)
            info.WidthCm = info.WidthCm > 0 ? info.WidthCm : (int)Math.Round(wIn * 2.54);
            info.HeightCm = info.HeightCm > 0 ? info.HeightCm : (int)Math.Round(hIn * 2.54);

            return Math.Sqrt(wIn * wIn + hIn * hIn);
        }

        // ===================== DPI helpers =====================
        private static (float dpiX, float dpiY) GetDpiForScreen(Screen s, bool useEffective)
        {
            try
            {
                var pt = new POINT { X = s.Bounds.Left + 1, Y = s.Bounds.Top + 1 };
                IntPtr hMon = MonitorFromPoint(pt, MONITOR_DEFAULTTONEAREST);
                int type = useEffective ? MDT_EFFECTIVE_DPI : MDT_RAW_DPI;

                if (GetDpiForMonitor(hMon, type, out uint dx, out uint dy) == 0)
                    return (dx, dy);
            }
            catch { }

            using (var g = Graphics.FromHwnd(IntPtr.Zero))
                return (g.DpiX, g.DpiY);
        }

        private static int GetScalePercentForWindow(IntPtr hwnd)
        {
            try
            {
                uint dpi = GetDpiForWindow(hwnd); // 96 * scale
                return (int)Math.Round(dpi * 100.0 / 96.0);
            }
            catch
            {
                return 100;
            }
        }

        // ===================== Pixel real settings =====================
        private static (int pxW, int pxH) GetMonitorPixels(string deviceName)
        {
            var dm = new DEVMODE();
            dm.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));

            if (EnumDisplaySettings(deviceName, -1, ref dm)) // -1 current settings
                return (dm.dmPelsWidth, dm.dmPelsHeight);

            return (0, 0);
        }

        // ===================== EDID Map by DISPLAY\XXXX =====================
        private sealed class EdidItem
        {
            public string Name;
            public int WidthCm;
            public int HeightCm;
        }

        /// <summary>
        /// key: DISPLAY\ACR0621  -> EDID item (name + cm)
        /// </summary>
        private static Dictionary<string, EdidItem> GetEdidMapByDisplayKey()
        {
            var map = new Dictionary<string, EdidItem>(StringComparer.OrdinalIgnoreCase);
            var friendly = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                // Friendly name
                using (var q1 = new ManagementObjectSearcher(@"root\WMI",
                    "SELECT InstanceName, UserFriendlyName FROM WmiMonitorID"))
                {
                    foreach (ManagementObject mo in q1.Get())
                    {
                        string inst = mo["InstanceName"] as string ?? "";
                        string name = DecodeUShortArray(mo["UserFriendlyName"] as ushort[])?.Trim();

                        string key = ToDisplayKey(inst); // DISPLAY\XXXX
                        if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(name))
                            friendly[key] = name;
                    }
                }

                // Sizes cm
                using (var q2 = new ManagementObjectSearcher(@"root\WMI",
                    "SELECT InstanceName, MaxHorizontalImageSize, MaxVerticalImageSize FROM WmiMonitorBasicDisplayParams"))
                {
                    foreach (ManagementObject mo in q2.Get())
                    {
                        string inst = mo["InstanceName"] as string ?? "";
                        int wcm = SafeInt(mo["MaxHorizontalImageSize"]);
                        int hcm = SafeInt(mo["MaxVerticalImageSize"]);

                        string key = ToDisplayKey(inst); // DISPLAY\XXXX
                        if (string.IsNullOrEmpty(key)) continue;

                        string name = friendly.ContainsKey(key) ? friendly[key] : key;

                        map[key] = new EdidItem { Name = name, WidthCm = wcm, HeightCm = hcm };
                    }
                }
            }
            catch { /* driver/WMI có thể hạn chế */ }

            return map;
        }

        // ===================== DISPLAY1 -> PNP ID =====================
        /// <summary>
        /// Input: \\.\DISPLAY1
        /// Output: DISPLAY\ACR0621\5&10A...
        /// </summary>
        private static string GetPnpDeviceIdByDisplayName(string displayName)
        {
            try
            {
                var dd = new DISPLAY_DEVICE();
                dd.cb = Marshal.SizeOf(dd);

                // Enum adapters
                for (uint id = 0; EnumDisplayDevices(null, id, ref dd, 0); id++)
                {
                    if (!string.Equals(dd.DeviceName, displayName, StringComparison.OrdinalIgnoreCase))
                        continue;

                    // Enum monitors of this adapter
                    var mon = new DISPLAY_DEVICE();
                    mon.cb = Marshal.SizeOf(mon);
                    for (uint j = 0; EnumDisplayDevices(dd.DeviceName, j, ref mon, 0); j++)
                    {
                        if (!string.IsNullOrEmpty(mon.DeviceID) &&
                            mon.DeviceID.StartsWith("DISPLAY\\", StringComparison.OrdinalIgnoreCase))
                        {
                            return mon.DeviceID; // DISPLAY\XXXX\...
                        }
                    }
                }
            }
            catch { }

            return null;
        }

        // ===================== Key normalize =====================
        /// <summary>
        /// Convert:
        /// - "DISPLAY\\ACR0621\\..." => "DISPLAY\\ACR0621"
        /// - "ACR0621" => "ACR0621" (không dùng)
        /// - WMI InstanceName: "DISPLAY\\ACR0621\\..._0" => "DISPLAY\\ACR0621"
        /// </summary>
        private static string ToDisplayKey(string instanceOrPnpId)
        {
            if (string.IsNullOrEmpty(instanceOrPnpId)) return null;

            // WMI InstanceName thường có dạng: DISPLAY\ACR0621\..._0
            // PNP DeviceID: DISPLAY\ACR0621\...
            var parts = instanceOrPnpId.Split('\\');
            if (parts.Length >= 2 && parts[0].Equals("DISPLAY", StringComparison.OrdinalIgnoreCase))
            {
                return parts[0] + "\\" + parts[1];
            }

            // fallback: nếu dạng "DISPLAY\\ACR0621" đã ok
            if (instanceOrPnpId.StartsWith("DISPLAY\\", StringComparison.OrdinalIgnoreCase))
            {
                var p = instanceOrPnpId.Split('\\');
                if (p.Length >= 2) return p[0] + "\\" + p[1];
            }

            return null;
        }

        private static string DecodeUShortArray(ushort[] arr)
        {
            if (arr == null) return null;
            var sb = new StringBuilder();
            foreach (ushort u in arr)
            {
                if (u == 0) break;
                sb.Append((char)u);
            }
            return sb.ToString();
        }

        private static int SafeInt(object o) => (o == null) ? 0 : Convert.ToInt32(o);

        private static bool IsValidInch(double inch)
            => inch >= 7.0 && inch <= 80.0 && !double.IsNaN(inch) && !double.IsInfinity(inch);

        private static double RoundHalf(double v)
            => Math.Round(v * 2, MidpointRounding.AwayFromZero) / 2.0;
    }
}
