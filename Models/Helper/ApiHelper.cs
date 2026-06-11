using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace NailsChekin.Models.Helper
{
    public static class ApiHelper
    {
        //public static string restfull_url = "http://localhost:56774//api/";      //LOCAL
        //public static string restfull_url = "http://95.217.32.253:8085/api/";      //TEST
        public static string restfull_url = "http://95.217.32.253:8088/api/";    //MAIN

        public static string CALL_API_OLD(string endpoint, string DATA, string method = "POST", bool request_authen = false)
        {
            string URL = restfull_url + endpoint;
            DATA = FormatDATA(DATA);

            // NOTE: nếu bạn call HTTPS trên .NET Framework thì nên set 1 lần lúc start app.
            // Giữ nguyên ở đây theo yêu cầu của bạn.
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            int retry = 2;

            // ✅ NEW: nếu gặp lỗi keep-alive bị server đóng thì retry sau sẽ tắt KeepAlive
            bool disableKeepAliveOnNextTry = false;

            while (retry-- > 0)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Method = method.ToUpper();
                request.ContentType = "application/json";

                request.ServicePoint.Expect100Continue = false;
                request.ProtocolVersion = HttpVersion.Version11;
                request.Timeout = 10000;
                request.ReadWriteTimeout = 100000;
                request.ServicePoint.ConnectionLimit = 1000;

                // FOR TEST
                request.UserAgent = "Mozilla/5.0 (POS Client)";

                // ✅ NEW: tự động tắt keep-alive ở lần retry tiếp theo nếu phát hiện lỗi keep-alive
                request.KeepAlive = !disableKeepAliveOnNextTry;

                // ✅ NEW: tránh reuse connection "già" quá lâu (hay gây lỗi keep-alive closed bởi LB)
                request.ServicePoint.ConnectionLeaseTimeout = 60_000; // recycle connection mỗi 60s
                request.ServicePoint.MaxIdleTime = 30_000;            // idle 30s thì bỏ

                // ✅ (khuyến nghị) nếu không cần proxy hệ thống
                // request.Proxy = null;

                // if (request_authen)
                // {
                //     string token = NailsChekin.Models.Helper.ConfigLocalHelper.GetStoreConfig("accessToken", "");
                //     request.Headers.Add("Authorization", "Bearer " + token);
                // }

                byte[] postDataBytes = null;
                if (method == "POST" || method == "PUT")
                {
                    // giữ behavior cũ: json nháy đơn -> nháy kép
                    DATA = Regex.Replace(DATA, "'", "\"");
                    postDataBytes = Encoding.UTF8.GetBytes(DATA);
                    request.ContentLength = postDataBytes.Length;

                    try
                    {
                        using (Stream webStream = request.GetRequestStream())
                        {
                            webStream.Write(postDataBytes, 0, postDataBytes.Length);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.SaveLOG_Crash(
                            "Error writing request stream: " + ex.Message +
                            "\nURL: " + URL +
                            "\nEND POINT: " + endpoint +
                            "\nMETHOD: " + request.Method +
                            "\nKeepAlive: " + request.KeepAlive +
                            "\nDATA: " + DATA,
                            "CALL_API Stream Error"
                        );
                        return "Error: Cannot send data to API - " + ex.Message;
                    }
                }

                try
                {
                    using (WebResponse webResponse = request.GetResponse())
                    using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
                    using (StreamReader responseReader = new StreamReader(webStream))
                    {
                        string response = responseReader.ReadToEnd();

                        if (response.StartsWith("\"")) response = response.Substring(1);
                        if (response.EndsWith("\"")) response = response.Substring(0, response.Length - 1);

                        return response;
                    }
                }
                catch (WebException webEx) when (retry > 0 && (
                    webEx.Status == WebExceptionStatus.ReceiveFailure ||
                    webEx.Status == WebExceptionStatus.Timeout ||
                    webEx.Status == WebExceptionStatus.ConnectFailure ||
                    webEx.Status == WebExceptionStatus.ConnectionClosed))
                {
                    // ✅ NEW: log rõ loại lỗi + inner exception
                    string inner = webEx.InnerException?.ToString() ?? "";
                    string msg = webEx.Message ?? "";

                    // ✅ NEW: detect lỗi "keep-alive expected..." => retry sau tắt KeepAlive
                    if (msg.IndexOf("expected to be kept alive", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        msg.IndexOf("underlying connection was closed", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        inner.IndexOf("expected to be kept alive", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        inner.IndexOf("underlying connection was closed", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        disableKeepAliveOnNextTry = true;
                    }

                    if (webEx.InnerException is SocketException socketEx)
                    {
                        LogHelper.SaveLOG_Crash(
                            $"Retryable WebException: {webEx.Status}\n" +
                            $"SocketException: {socketEx.SocketErrorCode} - {socketEx.Message}\n" +
                            $"URL: {URL}\nENDPOINT: {endpoint}\nMETHOD: {request.Method}\nKeepAlive: {request.KeepAlive}\n" +
                            $"WillDisableKeepAliveNextTry: {disableKeepAliveOnNextTry}\nDATA: {DATA}",
                            "CALL_API Retry SocketError"
                        );
                    }
                    else
                    {
                        LogHelper.SaveLOG_Crash(
                            $"Retryable WebException: {webEx.Status}\n" +
                            $"Message: {webEx.Message}\nInner: {inner}\n" +
                            $"URL: {URL}\nENDPOINT: {endpoint}\nMETHOD: {request.Method}\nKeepAlive: {request.KeepAlive}\n" +
                            $"WillDisableKeepAliveNextTry: {disableKeepAliveOnNextTry}\nDATA: {DATA}",
                            "CALL_API Retry WebException"
                        );
                    }

                    Thread.Sleep(1000);
                    continue;
                }
                catch (WebException webEx)
                {
                    string text = "";
                    int statusCode = 0;

                    try
                    {
                        using (WebResponse response = webEx.Response)
                        {
                            if (response is HttpWebResponse httpResponse)
                                statusCode = (int)httpResponse.StatusCode;

                            using (Stream dataStream = response?.GetResponseStream())
                            using (var reader = new StreamReader(dataStream ?? Stream.Null))
                            {
                                text = reader.ReadToEnd();
                            }
                        }
                    }
                    catch (Exception readEx)
                    {
                        LogHelper.SaveLOG_Crash(
                            "Error reading error-response: " + readEx.Message +
                            "\nWebException: " + webEx.Message +
                            "\nURL: " + URL +
                            "\nENDPOINT: " + endpoint +
                            "\nMETHOD: " + request.Method +
                            "\nKeepAlive: " + request.KeepAlive +
                            "\nDATA: " + DATA,
                            "CALL_API WebException ReadErrorBody"
                        );
                    }

                    // ✅ NEW: log đầy đủ luôn, vì đây là chỗ bạn đang chỉ thấy "An error has occurred."
                    LogHelper.SaveLOG_Crash(
                        $"WebException (non-retry): {webEx.Status}\n" +
                        $"HTTP StatusCode: {statusCode}\n" +
                        $"Message: {webEx.Message}\n" +
                        $"Inner: {webEx.InnerException}\n" +
                        $"URL: {URL}\nENDPOINT: {endpoint}\nMETHOD: {request.Method}\nKeepAlive: {request.KeepAlive}\n" +
                        $"ResponseBody: {text}\n" +
                        $"DATA: {DATA}",
                        "CALL_API WebException"
                    );

                    // giữ logic trả error cũ của bạn
                    if (!string.IsNullOrEmpty(text))
                    {
                        if (text.ToUpper().StartsWith("ERROR"))
                            return text;

                        if (text.Contains("message") && IsValidJson(text))
                            return "Error: " + JObject.Parse(text)["message"]?.ToString();

                        if (text.Contains("Message") && IsValidJson(text))
                            return "Error: " + JObject.Parse(text)["Message"]?.ToString();

                        return "Error: " + text;
                    }

                    return "Error: " + webEx.Message;
                }
                catch (Exception e)
                {
                    LogHelper.SaveLOG_Crash(
                        "Message: " + e.Message +
                        "\nURL: " + URL +
                        "\nENDPOINT: " + endpoint +
                        "\nMETHOD: " + request.Method +
                        "\nKeepAlive: " + request.KeepAlive +
                        "\nDATA: " + DATA,
                        "CALL_API Exception"
                    );
                    return "Error: API Exception - " + e.Message;
                }
            }

            return "Error: Failed after retries.";
        }

        public static string CALL_API(string endpoint, string DATA, string method = "POST", bool request_authen = false)
        {
            string URL = restfull_url + endpoint;
            DATA = FormatDATA(DATA);

            ApiPolicy policy = GetPolicy(endpoint);

            // tổng số attempt = policy.Retries (vd 3 = attempt 0..2)
            int attempts = Math.Max(1, policy.Retries);

            bool disableKeepAliveOnNextTry = false;

            // request id để trace log
            string requestId = Guid.NewGuid().ToString("N");

            // seed cho jitter (ổn định cho 1 call)
            int seedHash = (requestId + "|" + (endpoint ?? "")).GetHashCode();

            for (int attempt = 0; attempt < attempts; attempt++)
            {
                int delayMs = ComputeBackoffMs(policy, attempt, seedHash);
                if (delayMs > 0) Thread.Sleep(delayMs);

                HttpWebRequest request = null;

                try
                {
                    request = (HttpWebRequest)WebRequest.Create(URL);
                    request.Method = (method ?? "POST").ToUpperInvariant();
                    request.ContentType = "application/json";
                    request.ProtocolVersion = HttpVersion.Version11;

                    request.Timeout = policy.TimeoutMs;
                    request.ReadWriteTimeout = policy.ReadWriteTimeoutMs;

                    request.ServicePoint.ConnectionLimit = 1000;
                    request.UserAgent = "Mozilla/5.0 (POS Client)";

                    // ✅ KeepAlive policy:
                    // - nếu đã detect lỗi half-closed => tắt keep-alive lần sau
                    request.KeepAlive = !(policy.ForceConnectionClose || disableKeepAliveOnNextTry);

                    // tránh reuse connection "già"
                    request.ServicePoint.ConnectionLeaseTimeout = 60_000;
                    request.ServicePoint.MaxIdleTime = 30_000;

                    // request.Proxy = null; // nếu không cần proxy hệ thống

                    // if (request_authen)
                    // {
                    //     string token = NailsChekin.Models.Helper.ConfigLocalHelper.GetStoreConfig("accessToken", "");
                    //     request.Headers.Add("Authorization", "Bearer " + token);
                    // }

                    request.Headers["X-Request-Id"] = requestId;
                    request.Headers["X-Retry-Attempt"] = attempt.ToString();

                    // ----------------------------
                    // WRITE BODY (POST/PUT)
                    // ----------------------------
                    if (request.Method == "POST" || request.Method == "PUT")
                    {
                        DATA = Regex.Replace(DATA ?? "", "'", "\"");
                        byte[] postDataBytes = Encoding.UTF8.GetBytes(DATA);
                        request.ContentLength = postDataBytes.Length;

                        try
                        {
                            using (Stream webStream = request.GetRequestStream())
                            {
                                webStream.Write(postDataBytes, 0, postDataBytes.Length);
                            }
                        }
                        catch (WebException wex) when (
                            policy.RetryOnStreamTimeout &&
                            attempt < attempts - 1 &&
                            IsRetryableStatus(wex.Status))
                        {
                            // ✅ stream timeout / connect failure trước khi vào IIS => retry
                            disableKeepAliveOnNextTry = true;

                            string inner = wex.InnerException?.ToString() ?? "";
                            LogHelper.SaveLOG_Crash(
                                $"Retryable STREAM WebException: {wex.Status}\n" +
                                $"Message: {wex.Message}\nInner: {inner}\n" +
                                $"RequestId: {requestId}\nAttempt: {attempt + 1}/{attempts}\n" +
                                $"URL: {URL}\nENDPOINT: {endpoint}\nMETHOD: {request.Method}\n" +
                                $"KeepAlive: {request.KeepAlive}\nForceClose: {policy.ForceConnectionClose}\n" +
                                $"WillDisableKeepAliveNextTry: {disableKeepAliveOnNextTry}\n" +
                                $"Timeout(ms): {request.Timeout} ReadWriteTimeout(ms): {request.ReadWriteTimeout}\n" +
                                $"BackoffNext(ms): {ComputeBackoffMs(policy, attempt + 1, seedHash)}\n" +
                                $"DATA: {DATA}",
                                "CALL_API Stream Retry WebException"
                            );

                            try { request.Abort(); } catch { }
                            continue;
                        }
                        catch (Exception ex) when (
                            policy.RetryOnStreamTimeout &&
                            attempt < attempts - 1 &&
                            LooksLikeTimeout(ex))
                        {
                            disableKeepAliveOnNextTry = true;

                            LogHelper.SaveLOG_Crash(
                                $"Retryable STREAM Exception: {ex.Message}\n" +
                                $"RequestId: {requestId}\nAttempt: {attempt + 1}/{attempts}\n" +
                                $"URL: {URL}\nENDPOINT: {endpoint}\nMETHOD: {request.Method}\n" +
                                $"KeepAlive: {request.KeepAlive}\nForceClose: {policy.ForceConnectionClose}\n" +
                                $"WillDisableKeepAliveNextTry: {disableKeepAliveOnNextTry}\n" +
                                $"Timeout(ms): {request.Timeout} ReadWriteTimeout(ms): {request.ReadWriteTimeout}\n" +
                                $"BackoffNext(ms): {ComputeBackoffMs(policy, attempt + 1, seedHash)}\n" +
                                $"DATA: {DATA}",
                                "CALL_API Stream Retry Exception"
                            );

                            try { request.Abort(); } catch { }
                            continue;
                        }
                    }
                    else
                    {
                        request.ContentLength = 0;
                    }

                    // ----------------------------
                    // GET RESPONSE
                    // ----------------------------
                    using (WebResponse webResponse = request.GetResponse())
                    using (Stream webStream = webResponse.GetResponseStream() ?? Stream.Null)
                    using (StreamReader responseReader = new StreamReader(webStream))
                    {
                        string response = responseReader.ReadToEnd();

                        if (response.StartsWith("\"")) response = response.Substring(1);
                        if (response.EndsWith("\"")) response = response.Substring(0, response.Length - 1);

                        return response;
                    }
                }
                catch (WebException webEx) when (attempt < attempts - 1 && IsRetryableStatus(webEx.Status))
                {
                    // detect keep-alive / half-closed => retry sau tắt keepalive
                    string inner = webEx.InnerException?.ToString() ?? "";
                    string msg = webEx.Message ?? "";

                    if (msg.IndexOf("expected to be kept alive", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        msg.IndexOf("underlying connection was closed", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        inner.IndexOf("expected to be kept alive", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        inner.IndexOf("underlying connection was closed", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        disableKeepAliveOnNextTry = true;
                    }

                    LogHelper.SaveLOG_Crash(
                        $"Retryable WebException: {webEx.Status}\n" +
                        $"Message: {webEx.Message}\nInner: {inner}\n" +
                        $"RequestId: {requestId}\nAttempt: {attempt + 1}/{attempts}\n" +
                        $"URL: {URL}\nENDPOINT: {endpoint}\nMETHOD: {(request?.Method ?? method)}\n" +
                        $"KeepAlive: {(request?.KeepAlive ?? false)}\nForceClose: {policy.ForceConnectionClose}\n" +
                        $"WillDisableKeepAliveNextTry: {disableKeepAliveOnNextTry}\n" +
                        $"BackoffNext(ms): {ComputeBackoffMs(policy, attempt + 1, seedHash)}\n" +
                        $"DATA: {DATA}",
                        "CALL_API Retry WebException"
                    );

                    try { request?.Abort(); } catch { }
                    continue;
                }
                catch (WebException webEx)
                {
                    // non-retry or last attempt => đọc response body
                    string text = "";
                    int statusCode = 0;

                    try
                    {
                        using (WebResponse response = webEx.Response)
                        {
                            if (response is HttpWebResponse httpResponse)
                                statusCode = (int)httpResponse.StatusCode;

                            using (Stream dataStream = response?.GetResponseStream())
                            using (var reader = new StreamReader(dataStream ?? Stream.Null))
                            {
                                text = reader.ReadToEnd();
                            }
                        }
                    }
                    catch (Exception readEx)
                    {
                        LogHelper.SaveLOG_Crash(
                            "Error reading error-response: " + readEx.Message +
                            "\nWebException: " + webEx.Message +
                            "\nRequestId: " + requestId +
                            "\nURL: " + URL +
                            "\nENDPOINT: " + endpoint +
                            "\nMETHOD: " + (request?.Method ?? method) +
                            "\nKeepAlive: " + (request?.KeepAlive ?? false) +
                            "\nDATA: " + DATA,
                            "CALL_API WebException ReadErrorBody"
                        );
                    }

                    LogHelper.SaveLOG_Crash(
                        $"WebException (final): {webEx.Status}\n" +
                        $"HTTP StatusCode: {statusCode}\n" +
                        $"Message: {webEx.Message}\n" +
                        $"Inner: {webEx.InnerException}\n" +
                        $"RequestId: {requestId}\n" +
                        $"URL: {URL}\nENDPOINT: {endpoint}\nMETHOD: {(request?.Method ?? method)}\n" +
                        $"KeepAlive: {(request?.KeepAlive ?? false)}\nForceClose: {policy.ForceConnectionClose}\n" +
                        $"TimeoutMs: {policy.TimeoutMs} ReadWriteTimeoutMs: {policy.ReadWriteTimeoutMs}\n" +
                        $"ResponseBody: {text}\n" +
                        $"DATA: {DATA}",
                        "CALL_API WebException Final"
                    );

                    if (!string.IsNullOrEmpty(text))
                    {
                        if (text.ToUpper().StartsWith("ERROR"))
                            return text;

                        if (text.Contains("message") && IsValidJson(text))
                            return "Error: " + JObject.Parse(text)["message"]?.ToString();

                        if (text.Contains("Message") && IsValidJson(text))
                            return "Error: " + JObject.Parse(text)["Message"]?.ToString();

                        return "Error: " + text;
                    }

                    return "Error: " + webEx.Message;
                }
                catch (Exception e)
                {
                    LogHelper.SaveLOG_Crash(
                        "Message: " + e.Message +
                        "\nRequestId: " + requestId +
                        "\nURL: " + URL +
                        "\nENDPOINT: " + endpoint +
                        "\nMETHOD: " + (request?.Method ?? method) +
                        "\nKeepAlive: " + (request?.KeepAlive ?? false) +
                        "\nPolicy: " + $"TimeoutMs={policy.TimeoutMs}, ReadWriteTimeoutMs={policy.ReadWriteTimeoutMs}, Retries={policy.Retries}, ForceClose={policy.ForceConnectionClose}" +
                        "\nDATA: " + DATA,
                        "CALL_API Exception"
                    );
                    return "Error: API Exception - " + e.Message;
                }
                finally
                {
                    try { request?.Abort(); } catch { }
                }
            }

            return "Error: Failed after retries.";
        }

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

        private static string FormatDATA(string DATA)
        {
            if (!string.IsNullOrEmpty(DATA) && Utilitys.IsJsonObjectOrArray(DATA) == "JObject")
            {
                JObject jObject = JObject.Parse(DATA);
                if (!HasKey(jObject, "api_key"))
                    jObject["api_key"] = Constants.pos_api_sceret_key;
                if (!HasKey(jObject, "store_id"))
                    jObject["store_id"] = Constants.pos_store_id;

                return jObject.ToString(Newtonsoft.Json.Formatting.None);
            }

            return DATA;
        }

        static bool HasKey(JObject jObject, string key)
        {
            return jObject.ContainsKey(key);
        }


        // ✅ Endpoint policy: timeout/keepalive/retry/backoff theo từng API
        private sealed class ApiPolicy
        {
            public int TimeoutMs { get; set; } = 15_000;          // overall connect/handshake timeout
            public int ReadWriteTimeoutMs { get; set; } = 120_000; // stream read/write timeout
            public int Retries { get; set; } = 2;                 // tổng số lần thử (bao gồm lần đầu)
            public bool ForceConnectionClose { get; set; } = false; // Connection: close
            public int BackoffBaseMs { get; set; } = 500;         // base delay
            public int BackoffMaxMs { get; set; } = 3_000;        // max delay
            public bool UseJitter { get; set; } = true;
            public bool RetryOnStreamTimeout { get; set; } = true;
        }

        private static ApiPolicy GetPolicy(string endpoint)
        {
            endpoint = (endpoint ?? "").Trim();

            // ✅ CHECKOUT: quan trọng, payload lớn, dễ dính half-closed => close connection + timeout cao + retry nhiều hơn
            if (endpoint.Equals("Order/checkout", StringComparison.OrdinalIgnoreCase))
            {
                return new ApiPolicy
                {
                    TimeoutMs = 60_000,
                    ReadWriteTimeoutMs = 180_000,
                    Retries = 3,                  // 1 lần đầu + 2 retry
                    ForceConnectionClose = true,  // Connection: close
                    BackoffBaseMs = 600,
                    BackoffMaxMs = 4_000,
                    UseJitter = true,
                    RetryOnStreamTimeout = true
                };
            }

            // ✅ Ví dụ: các endpoint query nhẹ (search, list...) — timeout thấp hơn, keep-alive OK
            // if (endpoint.StartsWith("Order/", StringComparison.OrdinalIgnoreCase)) { ... }

            return new ApiPolicy(); // default
        }

        private static bool IsRetryableStatus(WebExceptionStatus status)
        {
            return status == WebExceptionStatus.ReceiveFailure
                || status == WebExceptionStatus.Timeout
                || status == WebExceptionStatus.ConnectFailure
                || status == WebExceptionStatus.ConnectionClosed
                || status == WebExceptionStatus.NameResolutionFailure
                || status == WebExceptionStatus.ProxyNameResolutionFailure;
        }

        private static int ComputeBackoffMs(ApiPolicy policy, int attemptIndex, int seedHash)
        {
            // attemptIndex: 0 (lần đầu), 1 (retry 1), 2 (retry 2)...
            // exponential backoff: base * 2^(attemptIndex-1) (chỉ tính từ retry)
            if (attemptIndex <= 0) return 0;

            long delay = (long)policy.BackoffBaseMs * (1L << Math.Min(attemptIndex - 1, 10));
            if (delay > policy.BackoffMaxMs) delay = policy.BackoffMaxMs;

            if (!policy.UseJitter) return (int)delay;

            // jitter đơn giản, deterministic theo seedHash + attemptIndex
            unchecked
            {
                int x = seedHash ^ (attemptIndex * 1103515245);
                x = (x << 13) ^ x;
                int jitter = (int)(Math.Abs(x) % Math.Max(1, (int)delay));
                // randomize trong khoảng 50%..100% delay
                int min = (int)(delay / 2);
                return min + jitter;
            }
        }

        private static bool LooksLikeTimeout(Exception ex)
        {
            if (ex == null) return false;
            string msg = ex.Message ?? "";
            return msg.IndexOf("timed out", StringComparison.OrdinalIgnoreCase) >= 0
                || msg.IndexOf("timeout", StringComparison.OrdinalIgnoreCase) >= 0;
        }

    }


}
