using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NailsChekin.Models.Helper
{
    /// <summary>
    /// Check version + tự update cho bản cài bằng Inno Setup.
    ///
    /// Cách hoạt động:
    ///  1. Server host 1 file manifest JSON (Constants.update_manifest_url):
    ///     { "version": "1.0.2.0", "setup_url": "http://.../RetailsPOS_Setup_1.0.2.exe", "notes": "..." }
    ///  2. App so version manifest với version Assembly hiện tại.
    ///  3. Nếu mới hơn: tải setup exe về %TEMP%, chạy với cờ silent của Inno
    ///     (/SILENT /SP- /CLOSEAPPLICATIONS) rồi thoát app để Inno ghi đè file.
    ///
    /// Yêu cầu phía Inno script (.iss):
    ///     AppId cố định (không đổi giữa các version) + CloseApplications=yes
    /// </summary>
    public static class UpdateHelper
    {
        public class UpdateInfo
        {
            public Version LatestVersion;
            public string SetupUrl = "";
            public string Notes = "";
        }

        public static Version GetLocalVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        }

        public static string GetLocalVersionString()
        {
            var v = GetLocalVersion();
            return v.Major + "." + v.Minor + "." + v.Build;
        }

        // Cho phép override URL manifest qua config local (key: update_manifest_url)
        private static string GetManifestUrl()
        {
            try { return Utilitys.GetConfig("update_manifest_url", Constants.update_manifest_url); }
            catch { return Constants.update_manifest_url; }
        }

        /// <summary>Tải manifest và parse. Trả về null nếu lỗi mạng/parse (caller tự quyết im lặng hay báo).</summary>
        public static async Task<UpdateInfo> CheckForUpdateAsync()
        {
            try
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
                using (var wc = new WebClient())
                {
                    wc.Encoding = Encoding.UTF8;
                    wc.Headers[HttpRequestHeader.CacheControl] = "no-cache";
                    // ?t= chống cache CDN/proxy trả manifest cũ
                    string url = GetManifestUrl() + "?t=" + DateTime.Now.Ticks;
                    string json = await wc.DownloadStringTaskAsync(url).ConfigureAwait(false);

                    var j = JObject.Parse(json);
                    return new UpdateInfo
                    {
                        LatestVersion = Version.Parse(j["version"].ToString()),
                        SetupUrl = j["setup_url"]?.ToString() ?? "",
                        Notes = j["notes"]?.ToString() ?? ""
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_Crash(ex.Message, "UpdateHelper CheckForUpdateAsync");
                return null;
            }
        }

        public static bool IsNewer(UpdateInfo info)
        {
            return info != null && info.LatestVersion != null && info.LatestVersion > GetLocalVersion();
        }

        /// <summary>
        /// Tải setup exe về %TEMP% và chạy silent. Trả về "" nếu OK (app sẽ tự thoát), ngược lại là message lỗi.
        /// </summary>
        public static async Task<string> DownloadAndInstallAsync(UpdateInfo info)
        {
            if (info == null || string.IsNullOrEmpty(info.SetupUrl))
                return "Error: setup_url missing in update manifest.";

            string tempFile = Path.Combine(Path.GetTempPath(), "RetailsPOS_Setup_" + info.LatestVersion + ".exe");

            try
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
                using (var wc = new WebClient())
                    await wc.DownloadFileTaskAsync(new Uri(info.SetupUrl), tempFile).ConfigureAwait(false);

                // Sanity check: file setup Inno thật luôn > 100KB, tránh chạy nhầm trang lỗi HTML
                if (!File.Exists(tempFile) || new FileInfo(tempFile).Length < 100 * 1024)
                    return "Error: downloaded setup file is invalid.";

                var psi = new ProcessStartInfo
                {
                    FileName = tempFile,
                    // /SILENT: chỉ hiện progress; /SP-: bỏ prompt đầu; /CLOSEAPPLICATIONS: Inno tự đóng app đang chạy
                    Arguments = "/SILENT /SP- /CLOSEAPPLICATIONS /FORCECLOSEAPPLICATIONS /NORESTART",
                    UseShellExecute = true
                };
                Process.Start(psi)?.Dispose();
                return "";
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_Crash(ex.ToString(), "UpdateHelper DownloadAndInstallAsync");
                return "Error: " + ex.Message;
            }
        }

        /// <summary>
        /// Hỏi user và cài. Trả về true nếu user đồng ý và setup đã được khởi chạy (app sẽ exit).
        /// Gọi trên UI thread.
        /// </summary>
        public static async Task<bool> PromptAndInstallAsync(IWin32Window owner, UpdateInfo info)
        {
            string msg = "New version " + info.LatestVersion + " is available (current: " + GetLocalVersion() + ").";
            if (!string.IsNullOrEmpty(info.Notes))
                msg += Environment.NewLine + Environment.NewLine + info.Notes;
            msg += Environment.NewLine + Environment.NewLine + "Update now? POS will restart.";

            var dr = MessageBox.Show(owner, msg, "POS Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr != DialogResult.Yes)
                return false;

            string err = await DownloadAndInstallAsync(info);
            if (!string.IsNullOrEmpty(err))
            {
                MessageBox.Show(owner, err, "POS Update", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            Application.Exit(); // nhả file exe cho Inno ghi đè
            return true;
        }

        /// <summary>
        /// Check thủ công từ nút bấm: báo cả khi đã là bản mới nhất / lỗi mạng.
        /// </summary>
        public static async Task ManualCheckAsync(Form owner)
        {
            var info = await CheckForUpdateAsync();
            if (owner == null || owner.IsDisposed) return;

            if (info == null)
            {
                MessageBox.Show(owner, "Cannot check for updates. Please check your internet connection.",
                    "POS Update", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!IsNewer(info))
            {
                MessageBox.Show(owner, "You are running the latest version (" + GetLocalVersion() + ").",
                    "POS Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            await PromptAndInstallAsync(owner, info);
        }

        // ===== Auto check khi mở app: im lặng nếu không có bản mới / lỗi mạng =====
        private static bool _autoChecked;

        public static async void AutoCheckOnStartup(Form owner)
        {
            if (_autoChecked) return;  // chỉ check 1 lần mỗi phiên chạy
            _autoChecked = true;

            try
            {
                var info = await CheckForUpdateAsync();
                if (!IsNewer(info)) return;
                if (owner == null || owner.IsDisposed || !owner.IsHandleCreated) return;

                owner.BeginInvoke(new Action(async () =>
                {
                    try { await PromptAndInstallAsync(owner, info); } catch { }
                }));
            }
            catch { /* auto check im lặng */ }
        }
    }
}
