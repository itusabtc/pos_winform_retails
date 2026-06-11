using NailsChekin.Models.Helper;
using NailsChekin.Models.ListModel;
using NailsChekin.MyControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NailsChekin.Models.Payments
{
    public class CreditCardLib
    {
        #region Codepay function: PAY / CANCEL / VOID / ADJUST ....

        public static string CODEPAY_PAY_ORDER(string ticketId, string tip_amount, string amount, string note, bool repair_mode)
        {
            try
            {
                string jData = "{";
                jData += "'merchant_no':'" + Constants.codepay_merchant_no + "',";
                jData += "'store_no':'" + Constants.codepay_store_no + "',";
                jData += "'terminal_sn':'" + Constants.codepay_terminal_sn + "',";
                jData += "'app_id':'" + Constants.codepay_app_id + "',";
                jData += "'amount':'" + amount + "',";
                jData += "'note':'" + note + "',";
                jData += "'repair_mode':'" + repair_mode + "',";
                jData += "'api_key':'" + Constants.pos_api_sceret_key + "' ";
                jData += "}";

                var jsonStrResponse = ApiHelper.CALL_API("Payment/codepay-pay-order", jData);
                var response = JObject.Parse(jsonStrResponse);
                if (!response["msg"].ToString().Equals("success"))
                    return "Error: " + response["msg"].ToString();

                return "";
            }
            catch (Exception exx)
            {
                return "Error: " + exx.Message;
            }
        }

        public static string CODEPAY_CANCEL_ORDER(string merchant_order_no)
        {
            try
            {
                string jData = "{";
                jData += "'merchant_no':'" + Constants.codepay_merchant_no + "', ";
                jData += "'store_no':'" + Constants.codepay_store_no + "', ";
                jData += "'terminal_sn':'" + Constants.codepay_terminal_sn + "', ";
                jData += "'merchant_order_no':'" + merchant_order_no + "' ";
                jData += "}";

                Console.WriteLine("CODEPAY_CANCEL_ORDER Send Data JSON: " + jData);
                var jsonStrResponse = ApiHelper.CALL_API("Payment/codepay-cancel-order", jData);
                var response = JObject.Parse(jsonStrResponse);
                if (!response["msg"].ToString().Equals("success"))
                    return "Error: " + response["msg"].ToString();

                return "";
            }
            catch (Exception exx)
            {
                return "Error: " + exx.Message;
            }
        }

        public static string CODEPAY_WLAN_PAY_ORDER(FormMain formMain, string ticketId, string tip_amount, string amount, string note, bool repair_mode)
        {
            try
            {
                //2025-03-18 tiền nhân 100 khi bắn qua
                //amount = (double.Parse(amount) * 100).ToString();
                //if (!string.IsNullOrEmpty(tip_amount))
                //    tip_amount = (double.Parse(tip_amount) * 100).ToString();

                string merchant_order_no = Utilitys.createRamdomKey();

                string jBizdata = "{";
                jBizdata += "'merchant_order_no':'" + merchant_order_no + "', ";
                jBizdata += "'pay_scenario':'SWIPE_CARD', ";
                jBizdata += "'order_amount':'" + amount + "', ";
                jBizdata += "'on_screen_signature':true, ";

                //if (repair_mode)
                //{
                //    jBizdata += "'on_screen_tip':false, ";
                //    jBizdata += "'tip_amount':'0', ";
                //}
                //else if ((!string.IsNullOrEmpty(tip_amount) && !tip_amount.Equals("0")) || note.StartsWith("CODEPAY_GIFT"))
                //{
                //    jBizdata += "'on_screen_tip':false, ";
                //    jBizdata += "'tip_amount':'" + tip_amount + "', ";
                //}
                //else
                //{
                //    jBizdata += "'on_screen_tip':true, ";
                //}

                jBizdata += "'on_screen_tip':false, ";
                jBizdata += "'trans_type':'1' ";
                jBizdata += "}";

                string message = "{";
                message += "'topic':'ecrhub.pay.order', ";
                message += "'app_id':'" + Constants.codepay_app_id + "', ";  //wz6012822ca2f1as78
                message += "'biz_data':" + jBizdata + " ";
                message += "}";

                Console.WriteLine("Send Data JSON: " + message);

                formMain.active_interval_check_wlan_payment = true;
                string result = formMain.SendTextAsync(message).Result;
                //if (!result.StartsWith("Error"))
                //{
                //    // Sau khi chạy hàm chính, hẹn 30s sau bắt đầu chạy hàm kiểm tra ngầm
                //    Task.Run(async () =>
                //    {
                //        await Task.Delay(30000); // Đợi 30 giây
                //        await RunBackgroundCheckP5Payment(formMain, merchant_order_no);
                //    });
                //}
                return result;
            }
            catch (Exception exx)
            {
                return "Error: " + exx.Message;
            }
        }

        public static async Task<string> CODEPAY_WLAN_PAY_ORDERAsync(FormMain formMain, string ticketId, string tip_amount, string amount, string note, bool repair_mode)
        {
            string merchant_order_no = "";

            try
            {
                merchant_order_no = Utilitys.createRamdomKey();

                string jBizdata = "{";
                jBizdata += "'merchant_order_no':'" + merchant_order_no + "', ";
                jBizdata += "'pay_scenario':'SWIPE_CARD', ";
                jBizdata += "'order_amount':'" + amount + "', ";
                jBizdata += "'on_screen_signature':true, ";

                //if (repair_mode)
                //{
                //    jBizdata += "'on_screen_tip':false, ";
                //    jBizdata += "'tip_amount':'0', ";
                //}
                //else if ((!string.IsNullOrEmpty(tip_amount) && !tip_amount.Equals("0")) || note.StartsWith("CODEPAY_GIFT"))
                //{
                //    jBizdata += "'on_screen_tip':false, ";
                //    jBizdata += "'tip_amount':'" + tip_amount + "', ";
                //}
                //else
                //{
                //    jBizdata += "'on_screen_tip':true, ";
                //}

                jBizdata += "'on_screen_tip':false, ";
                jBizdata += "'trans_type':'1' ";
                jBizdata += "}";

                string message = "{";
                message += "'topic':'ecrhub.pay.order', ";
                message += "'app_id':'" + Constants.codepay_app_id + "', ";
                message += "'biz_data':" + jBizdata + " ";
                message += "}";

                Console.WriteLine("Send Data JSON: " + message);

                formMain.active_interval_check_wlan_payment = true;

                string result = await formMain.SendTextAsync(message).ConfigureAwait(false);

                // SendTextAsync trả "" nghĩa là send OK.
                // Chỉ xem là lỗi khi result bắt đầu bằng "Error".
                bool sendOk = !result.StartsWith("Error", StringComparison.OrdinalIgnoreCase);

                // Nếu bắn lệnh qua P5 thành công thì tạo background job query status.
                // Job sẽ check ở mốc 30s, 45s, 60s ... tối đa 180s.
                if (sendOk)
                {
                    StartP5PaymentStatusJob(formMain, merchant_order_no);

                    LogHelper.SaveLOG_Payment(
                        "P5 pay order sent. Start status job. merchant_order_no=" + merchant_order_no + ", result=" + result,
                        "P5-PAYMENT-STATUS-JOB"
                    );
                }

                return result;
            }
            catch (Exception ex)
            {
                // Nếu lỗi ngay lúc gửi lệnh thì cleanup job nếu có.
                if (!string.IsNullOrEmpty(merchant_order_no))
                    CancelP5PaymentStatusJob(merchant_order_no);

                return "Error: " + ex.Message;
            }
        }

        private static async Task RunBackgroundCheckP5Payment(FormMain formMain, string merchant_order_no)
        {
            if (!formMain.active_interval_check_wlan_payment)
                return;

            for (int i = 1; i <= 3; i++)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Đang chạy kiểm tra ngầm lần {i}...");
                // Thực hiện kiểm tra ở đây (giả lập bằng Task.Delay)
                await Task.Delay(1000); // Giả lập mất 1s để kiểm tra
                CODEPAY_WLAN_QUERY_ORDER(formMain, merchant_order_no);

                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Kiểm tra ngầm lần {i} hoàn tất.");
                if (!formMain.active_interval_check_wlan_payment)
                    return;

                if (i < 3)
                    await Task.Delay(30000); // Đợi 30s rồi chạy lần tiếp theo
            }

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Đã hoàn thành 3 lần kiểm tra ngầm.");
        }

        public static string CODEPAY_WLAN_CANCEL_ORDER(FormMain formMain, string merchant_order_no)
        {
            try
            {
                string jBizdata = "{";
                jBizdata += "'merchant_order_no':'" + merchant_order_no + "' ";
                jBizdata += "}";

                string message = "{";
                message += "'request_id':'111111', ";
                message += "'topic':'ecrhub.pay.close', ";
                message += "'app_id':'" + Constants.codepay_app_id + "', ";
                message += "'biz_data':" + jBizdata + " ";
                message += "}";

                Console.WriteLine("Send CANCEL Data JSON: " + message);
                return formMain.SendTextAsync(message).Result;
            }
            catch (Exception exx)
            {
                return "Error: " + exx.Message;
            }
        }

        public static async Task<string> CODEPAY_WLAN_CANCEL_ORDER_ASYNC(FormMain formMain, string merchant_order_no, int timeoutMs = 2 * 60 * 1000, CancellationToken ct = default(CancellationToken))
        {
            if (timeoutMs <= 0) timeoutMs = 2 * 60 * 1000; //2 minute

            try
            {
                // Khi POS chủ động cancel thì hủy luôn job query status.
                // Nếu không hủy, background job vẫn có thể tiếp tục query P5 sau 30s/45s/60s.
                CancelP5PaymentStatusJob(merchant_order_no);
                formMain.active_interval_check_wlan_payment = false;

                var payload = new
                {
                    request_id = Guid.NewGuid().ToString("N"),
                    topic = "ecrhub.pay.close",
                    app_id = Constants.codepay_app_id,
                    biz_data = new { merchant_order_no = merchant_order_no }
                };

                string message = JsonConvert.SerializeObject(payload);
                Console.WriteLine("Send CANCEL Data JSON: " + message);

                // Task gửi thực tế
                Task<string> sendTask = formMain.SendTextAsync(message);

                // Task timeout có hỗ trợ cancel ngoài (ct)
                Task timeoutTask = Task.Delay(timeoutMs, ct);

                Task finished = await Task.WhenAny(sendTask, timeoutTask).ConfigureAwait(false);
                if (finished != sendTask)
                {
                    // Nếu ct bị cancel thì ưu tiên báo cancel
                    if (ct.IsCancellationRequested)
                        return "Error: Cancelled.";

                    return "Error: Timeout (2 minutes).";
                }

                // Lấy kết quả thật (nếu sendTask fault -> throw để vào catch)
                return await sendTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return "Error: Cancelled.";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        public static string CODEPAY_WLAN_VOID_ORDER(FormMain formMain, string merchant_order_no, string amount)
        {
            try
            {
                string jBizdata = "{";
                jBizdata += "'merchant_order_no':'" + Utilitys.createRamdomKey() + "', ";
                jBizdata += "'orig_merchant_order_no':'" + merchant_order_no + "', ";
                jBizdata += "'pay_scenario':'SWIPE_CARD', ";
                jBizdata += "'order_amount':'" + amount + "', ";
                jBizdata += "'trans_type':'2' ";
                jBizdata += "}";

                string message = "{";
                message += "'topic':'ecrhub.pay.order', ";
                message += "'app_id':'" + Constants.codepay_app_id + "', ";
                message += "'biz_data':" + jBizdata + " ";
                message += "}";

                Console.WriteLine("Send VOID Data JSON: " + message);
                return formMain.SendTextAsync(message).Result;
            }
            catch (Exception exx)
            {
                return "Error: " + exx.Message;
            }
        }

        public static async Task<string> CODEPAY_WLAN_VOID_ORDER_ASYNC(FormMain formMain, string merchant_order_no, string amount,
            int timeoutMs = 2 * 60 * 1000,
            CancellationToken ct = default(CancellationToken))
        {
            try
            {
                if (timeoutMs <= 0) timeoutMs = 2 * 60 * 1000;

                var payload = new
                {
                    topic = "ecrhub.pay.order",
                    app_id = Constants.codepay_app_id,
                    biz_data = new
                    {
                        merchant_order_no = Utilitys.createRamdomKey(), // void order id mới
                        orig_merchant_order_no = merchant_order_no,     // order gốc cần void
                        pay_scenario = "SWIPE_CARD",
                        order_amount = amount,
                        trans_type = "2"
                    }
                };

                string message = JsonConvert.SerializeObject(payload);
                Console.WriteLine("Send VOID Data JSON: " + message);

                Task<string> sendTask = formMain.SendTextAsync(message);
                Task timeoutTask = Task.Delay(timeoutMs, ct);

                Task finished = await Task.WhenAny(sendTask, timeoutTask).ConfigureAwait(false);

                if (finished != sendTask)
                {
                    if (ct.IsCancellationRequested) return "Error: Cancelled.";
                    return $"Error: Timeout ({timeoutMs} ms).";
                }

                return await sendTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return "Error: Cancelled.";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        public static async Task<string> CODEPAY_WLAN_REFUND_ORDER_ASYNC(FormMain formMain, string merchant_order_no, string order_amount, string tip_amount,
            int timeoutMs = 2 * 60 * 1000,
            CancellationToken ct = default(CancellationToken))
        {
            try
            {
                if (timeoutMs <= 0) timeoutMs = 2 * 60 * 1000;

                var biz = new Dictionary<string, object>
                {
                    ["merchant_order_no"] = Utilitys.createRamdomKey(),
                    ["orig_merchant_order_no"] = merchant_order_no,
                    ["pay_scenario"] = "SWIPE_CARD",
                    ["order_amount"] = order_amount,
                    ["trans_type"] = "3"
                };

                if (ToCents(tip_amount) > 0)
                    biz["tip_amount"] = tip_amount;   // tip=0 thì bỏ field

                var payload = new
                {
                    topic = "ecrhub.pay.order",
                    app_id = Constants.codepay_app_id,
                    biz_data = biz
                };

                string message = JsonConvert.SerializeObject(payload);
                Console.WriteLine("Send REFUND Data JSON: " + message);

                Task<string> sendTask = formMain.SendTextAsync(message);
                Task timeoutTask = Task.Delay(timeoutMs, ct);

                Task finished = await Task.WhenAny(sendTask, timeoutTask).ConfigureAwait(false);

                if (finished != sendTask)
                {
                    if (ct.IsCancellationRequested) return "Error: Cancelled.";
                    return $"Error: Timeout ({timeoutMs} ms).";
                }

                return await sendTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return "Error: Cancelled.";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        public static string CODEPAY_WLAN_QUERY_ORDER(FormMain formMain, string merchant_order_no)
        {
            try
            {
                string jBizdata = "{";
                jBizdata += "'merchant_order_no':'" + merchant_order_no + "' ";
                jBizdata += "}";

                string message = "{";
                message += "'topic':'ecrhub.pay.query', ";
                message += "'app_id':'" + Constants.codepay_app_id + "', ";
                message += "'biz_data':" + jBizdata + " ";
                message += "}";

                Console.WriteLine("Send QUERY Data JSON: " + message);
                return formMain.SendTextAsync(message).Result;
            }
            catch (Exception exx)
            {
                return "Error: " + exx.Message;
            }
        }

        public static async Task<string> CODEPAY_WLAN_QUERY_ORDER_ASYNC(FormMain formMain, string merchant_order_no, CancellationToken ct = default(CancellationToken))
        {
            try
            {
                string jBizdata = "{";
                jBizdata += "'merchant_order_no':'" + merchant_order_no + "' ";
                jBizdata += "}";

                string message = "{";
                message += "'topic':'ecrhub.pay.query', ";
                message += "'app_id':'" + Constants.codepay_app_id + "', ";
                message += "'biz_data':" + jBizdata + " ";
                message += "}";

                Console.WriteLine("Send QUERY Data JSON: " + message);

                Task<string> sendTask = formMain.SendTextAsync(message);
                Task cancelTask = Task.Delay(Timeout.Infinite, ct);

                Task finished = await Task.WhenAny(sendTask, cancelTask).ConfigureAwait(false);

                if (finished != sendTask)
                    return "Error: Cancelled.";

                return await sendTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return "Error: Cancelled.";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        public static string CODEPAY_USB_PAY_ORDER(FormMain formMain, string ticketId, string tip_amount, string amount, string note, bool repair_mode)
        {
            try
            {
                string responce = formMain.p5UsbLib.PayNow(ticketId, tip_amount, amount, note, repair_mode);
                return responce;
            }
            catch (Exception exx)
            {
                return "Error: " + exx.Message;
            }
        }

        public static string CODEPAY_USB_CANCEL_ORDER(FormMain formMain, string merchant_order_no)
        {
            try
            {
                string responce = formMain.p5UsbLib.Cancel(merchant_order_no);
                return responce;
            }
            catch (Exception exx)
            {
                return "Error: " + exx.Message;
            }
        }

        public static string CODEPAY_USB_VOID_ORDER(FormMain formMain, string orig_merchant_order_no, string amount)
        {
            try
            {
                string responce = formMain.p5UsbLib.Void(orig_merchant_order_no, amount);
                return responce;
            }
            catch (Exception exx)
            {
                return "Error: " + exx.Message;
            }
        }

        public static string CODEPAY_USB_REFUND_ORDER(FormMain formMain, string orig_merchant_order_no, string amount, string tip_amount)
        {
            try
            {
                string responce = formMain.p5UsbLib.Refund(orig_merchant_order_no, amount, tip_amount);
                return responce;
            }
            catch (Exception exx)
            {
                return "Error: " + exx.Message;
            }
        }

        public static string CODEPAY_TIP_ADJUST(string merchant_order_no, string tip_adjustment_amount)
        {
            try
            {
                string jData = "{";
                jData += "'merchant_no':'" + Constants.codepay_merchant_no + "', ";
                jData += "'store_no':'" + Constants.codepay_store_no + "', ";
                jData += "'terminal_sn':'" + Constants.codepay_terminal_sn + "', ";
                jData += "'tip_adjustment_amount':'" + tip_adjustment_amount + "', ";
                jData += "'merchant_order_no':'" + merchant_order_no + "' ";
                jData += "}";

                Console.WriteLine("CODEPAY_TIP_ADJUST Send Data JSON: " + jData);
                var jsonStrResponse = ApiHelper.CALL_API("Payment/codepay-tip-adjust", jData);
                var response = JObject.Parse(jsonStrResponse);
                if (!response["msg"].ToString().Equals("success"))
                    return "Error: " + response["msg"].ToString();

                return "";
            }
            catch (Exception exx)
            {
                return "Error: " + exx.Message;
            }
        }

        public static string CODEPAY_BATCH_CLOSE()
        {
            try
            {
                string jData = "{";
                jData += "'merchant_no':'" + Constants.codepay_merchant_no + "', ";
                jData += "'store_no':'" + Constants.codepay_store_no + "', ";
                jData += "'terminal_sn':'" + Constants.codepay_terminal_sn + "' ";
                jData += "}";

                Console.WriteLine("CODEPAY_BATCH_CLOSE Send Data JSON: " + jData);
                var jsonStrResponse = ApiHelper.CALL_API("Payment/codepay-batch-close", jData);
                var response = JObject.Parse(jsonStrResponse);
                if (!response["msg"].ToString().Equals("success"))
                    return "Error: " + response["msg"].ToString();

                return "";
            }
            catch (Exception exx)
            {
                return "Error: " + exx.Message;
            }
        }

        #endregion Codepay function


        #region CodePay Notify

        public static void CodePay_Process_Notify(string payment_data, FormMain frmMain)
        {
            string merchant_order_no = "";
            try
            {
                //{"trans_end_time":"2024-03-11 03:14:53","charset":"UTF-8","store_no":"4123000007","sign":"Pp0ah9j5WPb69+ZRKY1gToV7t1LWHAAHLLGBii06B3/+dw1WghE21brnXmkj4wsxGnVfNNTCw4InzFA90gZe5csuvhb60lSDMnAtIeTK4HJlf/5opBpJSgn50wkeW6NH72hFYLExljkC0V2fnVWiRxtJ+KRsRVV8pl7a1qfHfE0CF0ipQJiakK7KPnV8OC9FneojwijJx0LMLbID/9pDEK+C365BzVh9eC0vE/Kmjhjtzss5nnb6P0Zw3obpK5zyIjjG1dsNDTsis41CVFlptlyQqARtb6N+bPIOoHy0nVybx6oMzjUuO+QreZ6KaEqZxiKVahPFv/sQH1470K1fNQ==","merchant_order_no":"107d537dc56d4c7Wl2qvm","cashback_amount":"0","order_amount":"12","app_id":"wz6012822ca2f1as78","sign_type":"RSA2","trans_status":1,"price_currency":"USD","terminal_sn":"WPYB002329000082","trans_type":1,"timestamp":"1710126893448","trans_no":"51230000092403110000001","merchant_no":"312300000969","trans_error_msg":"Operator cancellation[[K026]Manual cancelation by operator]","method":"payment.result.notify","format":"JSON","trans_amount":"12","http_request_id":"03110314535828456980","version":"1.0","trans_error_code":"110"}
                if (!string.IsNullOrEmpty(payment_data))
                {
                    JObject jPayment = JObject.Parse(payment_data);

                    string trans_status = jPayment["trans_status"].ToString();
                    if (!trans_status.Equals("2"))
                    {
                        string trans_error_code = jPayment["trans_error_code"] == null ? "405" : jPayment["trans_error_code"].ToString();
                        string trans_error_msg = jPayment["trans_error_msg"] == null ? "Payment Error 404 !" : jPayment["trans_error_msg"].ToString();

                        if (frmMain.frmCreditProcessing == null || frmMain.frmCreditProcessing.IsDisposed)
                        {
                            return;  //Không hiện popup nếu đang không trong luồng thanh toán
                        }

                        frmMain.CodePay_ShowHide_Processing(true, trans_error_msg);
                        return;
                    }

                    string order_amount = jPayment["order_amount"].ToString();
                    string trans_amount = jPayment["trans_amount"].ToString();
                    string trans_no = jPayment["trans_no"].ToString();
                    merchant_order_no = jPayment["merchant_order_no"].ToString();

                    LogHelper.SaveLOG_Payment("codepay_amount: " + trans_amount, "ProcessPayment_CodePay Standa - myPOSOrderId: " + merchant_order_no);
                    if (frmMain.paymentList == null)
                    {
                        frmMain.paymentList = new List<PaymentModel>();
                    }

                    frmMain.uiThread.Send(delegate (object state)
                    {
                        PaymentModel _payment = new PaymentModel("CC", double.Parse(trans_amount));

                        //Responce X 100 lưu giống CLover
                        _payment.responce.Add(new CloverResponce(merchant_order_no, (double.Parse(trans_amount) * 100.0).ToString(), "0", trans_no, frmMain.surcharge_amount, frmMain.surcharge_debit_amount, frmMain.dual_price_amount));

                        if (frmMain.current_clover_token.StartsWith("CODEPAY_GIFT"))
                        {
                            frmMain.paymentList.Add(_payment);
                            frmMain.CHECK_PAYMENT_CORRECT(true);
                        }
                        if (frmMain.current_clover_token.StartsWith("CODEPAY_APPT"))
                        {
                            frmMain.paymentList.Add(_payment);
                            frmMain.CHECK_PAYMENT_CORRECT(true);
                        }
                        else
                        {
                            frmMain.paymentList.Add(_payment);
                            frmMain.CHECK_PAYMENT_CORRECT(true);
                        }
                    }, null);
                }
            }
            catch (Exception ex)
            {
                string response_msg = "Payment Error: " + ex.Message;
                frmMain.CodePay_ShowHide_Processing(true, response_msg);
                LogHelper.SaveLOG_Payment(ex.Message, "ProcessPayment_CodePay Standa - myPOSOrderId: " + merchant_order_no + " Exception");
            }
        }

        public static void CodePay_Process_WLAN_USB_Notify(string payment_data, FormMain frmMain)
        {
            //{"app_id":"wz6012822ca2f1as78","biz_data":{"card_type":"2","confirm_on_terminal":false,"endAmount":"20.00","expires":300,"invokeTransType":"01","is_auto_settlement":false,"limit_length":false,"merchant_order_no":"123456","notify_data":{},"on_screen_signature":false,"on_screen_tip":false,"order_amount":"20.00","pay_method_category":"BANKCARD","pay_scenario":"SWIPE_CARD","print_data":{},"tip_amount":"0.00","token":"","transTypeTextId":2131755692,"trans_status":"1","trans_type":"1","voice_data":{}},"callAppMode":"2","call_app_mode":"2","closeOrder":false,"device_data":{"alias_name":"","device_name":"","ip_address":"","mac_address":"","port":""},"notificationOrder":false,"notify_data":{"$ref":"$.biz\\_data.notify\\_data"},"print_data":{"$ref":"$.biz\\_data.print\\_data"},"response_code":"202","response_msg":"Transaction parameter error[[M003]Invalid Amount]","timestamp":"2024-10-07 05:42:39","topic":"ecrhub.pay.order","voice_data":{"$ref":"$.biz\\_data.voice\\_data"}}
            //{"app_id":"wz6012822ca2f1as78","biz_data":{"card_type":"2","confirm_on_terminal":false,"endAmount":"20.00","expires":300,"invokeTransType":"01","is_auto_settlement":false,"limit_length":false,"merchant_order_no":"123456","notify_data":{},"on_screen_signature":false,"on_screen_tip":false,"order_amount":"20.00","pay_method_category":"BANKCARD","pay_scenario":"SWIPE_CARD","print_data":{},"token":"","transTypeTextId":2131755692,"trans_status":"1","trans_type":"1","voice_data":{}},"callAppMode":"2","call_app_mode":"2","closeOrder":false,"device_data":{"alias_name":"","device_name":"","ip_address":"","mac_address":"","port":""},"notificationOrder":false,"notify_data":{"$ref":"$.biz\\_data.notify\\_data"},"print_data":{"$ref":"$.biz\\_data.print\\_data"},"response_code":"110","response_msg":"Operator cancellation[[K026]Manual cancelation by operator]","timestamp":"2024-10-09 09:28:07","topic":"ecrhub.pay.order","voice_data":{"$ref":"$.biz\\_data.voice\\_data"}}

            //Success
            //{"app_id":"wz6012822ca2f1as78","biz_data":{"card_type":"2","confirm_on_terminal":false,"endAmount":"26.25","expires":300,"invokeTransType":"01","is_auto_settlement":false,"limit_length":false,"merchant_order_no":"b0e9176c0ac14ab84LSiT","notify_data":{},"on_screen_signature":false,"on_screen_tip":false,"order_amount":"26.25","pay_method_category":"BANKCARD","pay_scenario":"SWIPE_CARD","print_data":{},"token":"","transTypeTextId":2131755692,"trans_status":"2","trans_type":"1","voice_data":{}},"callAppMode":"2","call_app_mode":"2","closeOrder":false,"device_data":{"alias_name":"","device_name":"","ip_address":"","mac_address":"","port":""},"notificationOrder":false,"notify_data":{"$ref":"$.biz\\_data.notify\\_data"},"print_data":{"$ref":"$.biz\\_data.print\\_data"},"response_code":"0","response_msg":"SUCCESS","timestamp":"2024-10-09 10:04:40","topic":"ecrhub.pay.order","voice_data":{"$ref":"$.biz\\_data.voice\\_data"}}

            string merchant_order_no = "";
            try
            {
                if (Utilitys.IsValidJson(payment_data))
                {
                    JObject jPayment = JObject.Parse(payment_data);
                    JObject biz_data_for_cancel = null;
                    if (jPayment["biz_data"] != null)
                    {
                        try
                        {
                            biz_data_for_cancel = JObject.Parse(jPayment["biz_data"].ToString());
                            merchant_order_no = biz_data_for_cancel["merchant_order_no"] == null ? "" : biz_data_for_cancel["merchant_order_no"].ToString();
                        }
                        catch { }
                    }

                    string response_code = jPayment["response_code"] == null ? "0" : jPayment["response_code"].ToString();
                    if (!(response_code.Equals("200") || response_code.Equals("0")))
                    {
                        if (!string.IsNullOrEmpty(merchant_order_no))  //Hủy job query order
                            CancelP5PaymentStatusJob(merchant_order_no);

                        if (frmMain.frmCreditProcessing == null || frmMain.frmCreditProcessing.IsDisposed)
                        {
                            frmMain.CartEnableControl();
                            return;  //Không hiện popup nếu đang không trong luồng thanh toán
                        }

                        if (!frmMain.frmCreditProcessing.is_cancel_from_pos)
                        {
                            string response_msg = jPayment["response_msg"] == null ? "Payment Error 404 !" : jPayment["response_msg"].ToString();
                            frmMain.CodePay_ShowHide_Processing(true, response_msg);
                        }

                        return;
                    }

                    JObject biz_data = JObject.Parse(jPayment["biz_data"].ToString());
                    string order_amount = biz_data["order_amount"] == null ? "0" : biz_data["order_amount"].ToString();
                    string tip_amount = biz_data["tip_amount"] == null ? "0" : biz_data["tip_amount"].ToString();
                    string trans_no = biz_data["trans_no"] == null ? "" : biz_data["trans_no"].ToString();
                    merchant_order_no = biz_data["merchant_order_no"] == null ? "" : biz_data["merchant_order_no"].ToString();

                    LogHelper.SaveLOG_Payment("payment_data: " + payment_data, "ProcessPayment_CodePay WLAN_USB - myPOSOrderId: " + merchant_order_no);
                    if (frmMain.paymentList == null)
                    {
                        frmMain.paymentList = new List<PaymentModel>();
                    }

                    //frmMain.uiThread.Send(delegate (object state)
                    frmMain.Invoke((MethodInvoker)delegate
                    {
                        PaymentModel _payment = new PaymentModel("CC", double.Parse(order_amount));

                        //Responce X 100 lưu giống CLover
                        var cloverResponce = new CloverResponce(merchant_order_no, (double.Parse(order_amount) * 100.0).ToString(), (double.Parse(tip_amount) * 100.0).ToString(), trans_no, frmMain.surcharge_amount, frmMain.surcharge_debit_amount, frmMain.dual_price_amount);
                        bool repair_mode = false;
                        if (repair_mode)
                        {
                            Control found = frmMain.Controls.Find("txtRepairAmount", true).FirstOrDefault();
                            double repair_fee = repair_mode ? Utilitys.getTotalAmount(found == null ? "0" : found.Text) : 0;

                            Control found_tip = frmMain.Controls.Find("txtTotalTip", true).FirstOrDefault();
                            double tip_pos_amount = repair_mode ? Utilitys.getTotalAmount(found_tip == null ? "0" : found_tip.Text) : 0;

                            cloverResponce.SetRepairMode(repair_fee, tip_pos_amount);
                        }

                        _payment.responce.Add(cloverResponce);


                        frmMain.paymentList.Add(_payment);
                        frmMain.CHECK_PAYMENT_CORRECT(true);

                        // POS đã ghi nhận payment thành công rồi thì hủy job query status.
                        CancelP5PaymentStatusJob(merchant_order_no);

                    }, null);
                }
            }
            catch (Exception ex)
            {
                string response_msg = "Payment Error: " + ex.Message;
                frmMain.CodePay_ShowHide_Processing(true, response_msg);
                LogHelper.SaveLOG_Payment(ex.Message, "ProcessPayment_CodePay WLAN_USB - myPOSOrderId: " + merchant_order_no + " Exception");
                LogHelper.SaveLOG_Crash("Message: " + ex.Message + "\nStack Trace:" + ex.StackTrace + "\nInnerException:" + ex.InnerException.Message + "\nPaymentData: " + payment_data, "CodePay_Process_WLAN_USB_Notify Exception");
            }
        }

        public static void CodePay_Process_T2_Notify(JObject jPayment, JObject jPrint, FormMain frmMain)
        {
            string merchant_order_no = "";
            try
            {
                string trans_status = jPayment["trans_status"] == null ? "2" : jPayment["trans_status"].ToString();
                if (!trans_status.Equals("2"))
                {
                    string trans_error_code = jPayment["trans_error_code"] == null ? "405" : jPayment["trans_error_code"].ToString();
                    string trans_error_msg = jPayment["trans_error_msg"] == null ? "Payment Error 404 !" : jPayment["trans_error_msg"].ToString();

                    if (frmMain.frmCreditProcessing == null || frmMain.frmCreditProcessing.IsDisposed)
                    {
                        return;  //Không hiện popup nếu đang không trong luồng thanh toán
                    }

                    frmMain.CodePay_ShowHide_Processing(true, trans_error_msg);
                    return;
                }

                string order_amount = jPayment["order_amount"].ToString();
                string tip_amount = jPayment["tip_amount"] == null ? "0" : jPayment["tip_amount"].ToString();
                string trans_no = jPayment["trans_no"].ToString();
                merchant_order_no = jPayment["merchant_order_no"].ToString();

                LogHelper.SaveLOG_Payment("T2 codepay_amount: " + order_amount + " - trans_no: " + trans_no, "ProcessPayment_CodePay T2 - myPOSOrderId: " + merchant_order_no);
                if (frmMain.paymentList == null)
                    frmMain.paymentList = new List<PaymentModel>();

                //frmMain.uiThread.Send(delegate (object state)
                frmMain.Invoke((MethodInvoker)delegate
                {
                    PaymentModel _payment = new PaymentModel("CC", double.Parse(order_amount));

                    string print_type = jPrint["print_type"] == null ? "" : jPrint["print_type"].ToString();
                    string signature_base64 = jPrint["signature_base64"] == null ? "" : jPrint["signature_base64"].ToString();
                    string pay_method_id = jPayment["pay_method_id"] == null ? "" : jPayment["pay_method_id"].ToString();
                    string card_network_type = jPayment["card_network_type"] == null ? "" : jPayment["card_network_type"].ToString();
                    string card_no = jPayment["card_no"] == null ? "" : jPayment["card_no"].ToString();
                    string auth_code = jPayment["auth_code"] == null ? "" : jPayment["auth_code"].ToString();
                    _payment.SetCreditPrintInfo(print_type, trans_no, pay_method_id, card_network_type, card_no, auth_code, signature_base64, tip_amount);

                    //Responce X 100 lưu giống CLover
                    var cloverResponce = new CloverResponce(merchant_order_no, (double.Parse(order_amount) * 100.0).ToString(), (double.Parse(tip_amount) * 100.0).ToString(), trans_no, frmMain.surcharge_amount, frmMain.surcharge_debit_amount, frmMain.dual_price_amount);
                    _payment.responce.Add(cloverResponce);

                    if (frmMain.current_clover_token.StartsWith("CODEPAY_GIFT"))
                    {
                        frmMain.paymentList.Add(_payment);
                        frmMain.CHECK_PAYMENT_CORRECT(true);
                    }
                    if (frmMain.current_clover_token.StartsWith("CODEPAY_APPT"))
                    {
                        frmMain.paymentList.Add(_payment);
                        frmMain.CHECK_PAYMENT_CORRECT(true);
                    }
                    else
                    {
                        frmMain.paymentList.Add(_payment);
                        frmMain.CHECK_PAYMENT_CORRECT(true);
                    }
                }, null);
            }
            catch (Exception ex)
            {
                string response_msg = "T2 Payment Error: " + ex.Message;
                frmMain.CodePay_ShowHide_Processing(true, response_msg);
                LogHelper.SaveLOG_Payment(ex.Message, "ProcessPayment_CodePay T2 - myPOSOrderId: " + merchant_order_no + " Exception");
            }
        }

        //public static void CodePay_T2_Process_Void(JObject jObject, JObject jPrint, JObject original_pos_body, TabAdjust frmTipsAdjust, FormMain frmMain)
        //{
        //    try
        //    {
        //        string trans_type = jObject["trans_type"] == null ? "" : jObject["trans_type"].ToString();
        //        if (trans_type.Equals("2") || trans_type.Equals("3"))  //VOID = 2   REFUND = 3
        //        {
        //            string trans_status = jObject["trans_status"].ToString();
        //            if (!trans_status.Equals("2"))
        //            {
        //                string response_msg = jObject["trans_error_msg"].ToString();
        //                MyControls.CustomMessageBox.Show("Void Error: " + response_msg);
        //            }
        //            else if (frmTipsAdjust != null) //Call API VOID //Call API VOID
        //            {
        //                string orig_merchant_order_no = original_pos_body["orig_merchant_order_no"] == null ? "" : original_pos_body["orig_merchant_order_no"].ToString();
        //                string order_amount = original_pos_body["order_amount"] == null ? "0" : original_pos_body["order_amount"].ToString();
        //                string tip_amount = original_pos_body["tip_amount"] == null ? "0" : original_pos_body["tip_amount"].ToString();

        //                if (string.IsNullOrEmpty(orig_merchant_order_no))
        //                {
        //                    orig_merchant_order_no = frmTipsAdjust.credit_card_order_id;
        //                }

        //                if (!string.IsNullOrEmpty(orig_merchant_order_no))
        //                {
        //                    //For test
        //                    //orig_merchant_order_no = "1764812293786059863";

        //                    MainPOS mainPos = new MainPOS();
        //                    string void_data = jObject.ToString(Newtonsoft.Json.Formatting.None);
        //                    string void_print = jPrint.ToString(Newtonsoft.Json.Formatting.None);
        //                    string responce = mainPos.TipAdjust_RefundInvoice(trans_type, orig_merchant_order_no, order_amount, tip_amount, frmTipsAdjust.save_note, void_data, void_print);
        //                    if (Utilitys.IsValidJson(responce))
        //                    {
        //                        string id = JObject.Parse(responce)["id"].ToString();
        //                        string new_status = JObject.Parse(responce)["new_status"].ToString();

        //                        if (frmTipsAdjust != null)
        //                        {
        //                            frmTipsAdjust.ChangeTicketStatus(id, new_status);
        //                            frmTipsAdjust.ResetAllData();
        //                        }

        //                        frmMain.CodePay_ShowHide_Processing(false);

        //                        //Print Void Receipt
        //                        if (jPrint["print_type"] != null && jPrint["print_type"].ToString().Equals("receipt"))
        //                            PrinterLocalHelper.PrintDirectVoidTicket(id, void_data);
        //                    }
        //                    else
        //                    {
        //                        CustomMessageBox.Show("Error: " + responce);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CustomMessageBox.Show("CodePay_T2_Process_Void Exception: " + ex.Message);
        //        LogHelper.SaveLOG_Crash(ex.Message, "CodePay_T2_Process_Void Exception");
        //    }
        //}

        public static bool IsT2CancelPaymentNotify(JObject jPayment)
        {
            string trans_status = jPayment["trans_status"] == null ? "2" : jPayment["trans_status"].ToString();
            if (!trans_status.Equals("2"))
            {
                string trans_error_code = jPayment["trans_error_code"] == null ? "405" : jPayment["trans_error_code"].ToString();
                string trans_error_msg = jPayment["trans_error_msg"] == null ? "Payment Error 404 !" : jPayment["trans_error_msg"].ToString();

                return true;
            }

            return false;
        }

        #endregion


        #region LIB

        public static CREDIT_DEVICE_TYPE GET_CREDIT_DEVICE()
        {
            string credit_card_device = ConfigLocalHelper.GetConfig("credit_card_device", "CLOVER");
            if (credit_card_device.Equals("CLOVER"))
                return CREDIT_DEVICE_TYPE.CLOVER;
            else if (credit_card_device.Equals("CODE PAY"))
                return CREDIT_DEVICE_TYPE.CODE_PAY;

            return CREDIT_DEVICE_TYPE.NONE;
        }

        public static CODEPAY_DEVICE GET_CODEPAY_DEVICE()
        {
            string codepay_device = ConfigLocalHelper.GetConfig("codepay_device", Constants.codepay_device_defaul);
            if (codepay_device.Equals("T2"))
                return CODEPAY_DEVICE.T2;

            return CODEPAY_DEVICE.P5;
        }

        public static bool USING_SYSTEM_CREDIT()
        {
            string using_system_credit = ConfigLocalHelper.GetConfig("using_system_credit", Constants.using_system_credit).Equals("OFF") ? "0" : "1";
            return (using_system_credit.Equals("1") ? true : false);
        }

        #endregion


        #region Codepay Helper

        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _p5PaymentStatusJobs = new ConcurrentDictionary<string, CancellationTokenSource>();

        private static void StartP5PaymentStatusJob(FormMain formMain, string merchant_order_no)
        {
            if (string.IsNullOrWhiteSpace(merchant_order_no))
                return;

            // Nếu vì lý do nào đó order này đã có job cũ thì hủy trước.
            CancelP5PaymentStatusJob(merchant_order_no);

            var cts = new CancellationTokenSource();

            if (!_p5PaymentStatusJobs.TryAdd(merchant_order_no, cts))
            {
                cts.Dispose();
                return;
            }

            Task.Run(async () =>
            {
                try
                {
                    await RunP5PaymentStatusJobAsync(formMain, merchant_order_no, cts.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException exx)
                {
                    // Job bị hủy do POS đã nhận notify/payment.
                    LogHelper.SaveLOG_Payment("P5 status job OperationCanceledException: " + exx.Message, "P5-PAYMENT-STATUS-JOB");
                }
                catch (Exception ex)
                {
                    LogHelper.SaveLOG_Payment("P5 status job Exception: " + ex.Message, "P5-PAYMENT-STATUS-JOB");
                }
                finally
                {
                    if (_p5PaymentStatusJobs.TryRemove(merchant_order_no, out var removedCts))
                        removedCts.Dispose();
                }
            });
        }

        private static void CancelP5PaymentStatusJob(string merchant_order_no)
        {
            if (string.IsNullOrWhiteSpace(merchant_order_no))
                return;

            if (_p5PaymentStatusJobs.TryRemove(merchant_order_no, out var cts))
            {
                try
                {
                    cts.Cancel();
                }
                catch { }

                cts.Dispose();

                LogHelper.SaveLOG_Payment("P5 payment status job cancelled. merchant_order_no=" + merchant_order_no, "P5-PAYMENT-STATUS-JOB");
            }
        }

        private static async Task RunP5PaymentStatusJobAsync(
            FormMain formMain,
            string merchant_order_no,
            CancellationToken ct)
        {
            // Check tại elapsed time: 30s, 45s, 60s ... 180s.
            int[] checkAtSeconds =
            {
                30, 45, 60, 75, 90, 105, 120, 135, 150, 165, 180
            };

            int previousSecond = 0;

            foreach (int currentSecond in checkAtSeconds)
            {
                int delayMs = (currentSecond - previousSecond) * 1000;
                previousSecond = currentSecond;

                await Task.Delay(delayMs, ct).ConfigureAwait(false);

                if (ct.IsCancellationRequested)
                    return;

                if (!formMain.active_interval_check_wlan_payment)
                    return;

                // Nếu trong lúc chờ, notify đã xử lý và job bị hủy thì không query nữa.
                if (!_p5PaymentStatusJobs.ContainsKey(merchant_order_no))
                    return;

                LogHelper.SaveLOG_Payment(
                    $"Query P5 payment status at {currentSecond}s. merchant_order_no={merchant_order_no}",
                    "P5-PAYMENT-STATUS-JOB"
                );

                string queryResult = await CODEPAY_WLAN_QUERY_ORDER_ASYNC(formMain, merchant_order_no, ct)
                    .ConfigureAwait(false);

                LogHelper.SaveLOG_Payment(
                    $"P5 query result at {currentSecond}s. merchant_order_no={merchant_order_no}, result={queryResult}",
                    "P5-PAYMENT-STATUS-JOB"
                );
            }

            LogHelper.SaveLOG_Payment(
                "P5 payment status job finished after 180s. merchant_order_no=" + merchant_order_no,
                "P5-PAYMENT-STATUS-JOB"
            );
        }


        static long ToCents(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return 0;
            s = s.Trim();

            // Nếu input có thể là "10,50" thì normalize:
            s = s.Replace(",", ".");

            var d = decimal.Parse(s, CultureInfo.InvariantCulture);
            return (long)Math.Round(d * 100m, MidpointRounding.AwayFromZero);
        }

        #endregion End Codepay Helper

    }

}
