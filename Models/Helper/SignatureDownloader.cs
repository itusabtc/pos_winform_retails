using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace NailsChekin.Models.Helper
{
    class SignatureDownloader
    {
        public static async Task<byte[]> DownloadBytesLikeBrowserAsync__(string url)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.AllowAutoRedirect = true;
            req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            req.Proxy = null;          // QUAN TRỌNG: tắt proxy
            req.Timeout = 20000;
            req.ReadWriteTimeout = 20000;

            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)";
            req.Referer = "https://mgt.codepay.us/";
            req.Accept = "*/*";

            using (var resp = (HttpWebResponse)await req.GetResponseAsync())
            using (var s = resp.GetResponseStream())
            using (var ms = new MemoryStream())
            {
                await s.CopyToAsync(ms);
                return ms.ToArray();
            }
        }

        public static async Task<byte[]> DownloadBytesLikeBrowserAsync(string url)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.AllowAutoRedirect = true;
            req.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            req.Proxy = null;
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)";
            req.Referer = "https://mgt.codepay.us/";
            req.Accept = "*/*";

            var respTask = req.GetResponseAsync();
            var done = await Task.WhenAny(respTask, Task.Delay(20000));
            if (done != respTask)
            {
                req.Abort(); // cắt cứng
                throw new TimeoutException("Timeout waiting response headers.");
            }

            using (var resp = (HttpWebResponse)await respTask)
            using (var s = resp.GetResponseStream())
            using (var ms = new MemoryStream())
            {
                await s.CopyToAsync(ms);
                return ms.ToArray();
            }
        }
    }
}
