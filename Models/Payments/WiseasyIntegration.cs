using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NailsChekin.Models.Payments
{
    public class WiseasyIntegration
    {
        static async Task Main()
        {
            // Thông tin tài khoản Wiseasy của bạn
            string merchantId = "YourMerchantId";
            string apiKey = "YourApiKey";
            string apiUrl = "https://api.wiseasy.com.vn";

            // Tạo một đơn hàng mới
            var order = new
            {
                amount = 10000,  // Số tiền thanh toán (ví dụ: 10000 VND)
                orderId = Guid.NewGuid().ToString(),  // Mã đơn hàng duy nhất
                description = "Mô tả đơn hàng"
            };

            // Tạo chuỗi JSON từ đơn hàng
            string jsonOrder = Newtonsoft.Json.JsonConvert.SerializeObject(order);

            // Tạo chữ ký cho đơn hàng
            string signature = GenerateSignature(apiKey, jsonOrder);

            // Gửi yêu cầu thanh toán đến Wiseasy API
            string response = await MakePaymentRequest(apiUrl, merchantId, signature, jsonOrder);

            // Xử lý phản hồi từ Wiseasy API
            Console.WriteLine(response);
        }

        static string GenerateSignature(string apiKey, string data)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA256(Encoding.UTF8.GetBytes(apiKey)))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return Convert.ToBase64String(hash);
            }
        }

        static async Task<string> MakePaymentRequest(string apiUrl, string merchantId, string signature, string jsonOrder)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Merchant-Id", merchantId);
                httpClient.DefaultRequestHeaders.Add("Signature", signature);

                var content = new StringContent(jsonOrder, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{apiUrl}/payment", content);

                return await response.Content.ReadAsStringAsync();
            }
        }

    }
}
