using com.clover.remotepay.sdk;
using com.clover.remotepay.transport;
using com.clover.sdk.v3.payments;
using NailsChekin.Models.ListModel;
using NailsChekin.Models.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NailsChekin.Models.Helper
{
    public class CartHelper
    {
        private readonly CloverConnectorService _clover;
        private readonly Action<string> _status;         // UpdateCreditDeviceStatus(...)
        private readonly Action<bool> _setWaitingFlag;   // waiting_clover_process = ...
        private readonly Action<bool> _setConfirmPrint;  // clover_confirm_print = ...
        private readonly Action<string> _log;            // optional: LogHelper/UpdateCloverLog
        private readonly Func<string> _getPairToken;     // lấy token hiện tại nếu cần log

        public event Action<Payment, SaleResponse> SaleSucceeded;      // ví dụ: lưu DB, cập nhật cart
        public event Action<string> SaleFailed;         // truyền reason ra UI

        public event Action<Payment, VoidPaymentResponse> VoidSucceeded; // trả cả Payment & resp
        public event Action<string> VoidFailed;

        public event Action<Payment, RefundPaymentResponse> RefundSucceeded; // trả Payment & resp
        public event Action<string> RefundFailed;

        public event Action<bool> PaymentFinished;       // báo UI đóng popup: true/false

        // Báo ra ngoài khi Clover báo tip mới (đơn vị: cents)
        public event Action<long> TipUpdated;

        public CartHelper(CloverConnectorService clover,
                          Action<string> updateStatus,
                          Action<bool> setWaitingFlag,
                          Action<bool> setConfirmPrint,
                          Func<string> getPairToken = null,
                          Action<string> log = null)
        {
            _clover = clover ?? throw new ArgumentNullException(nameof(clover));
            _status = updateStatus ?? (_ => { });
            _setWaitingFlag = setWaitingFlag ?? (_ => { });
            _setConfirmPrint = setConfirmPrint ?? (_ => { });
            _getPairToken = getPairToken ?? (() => string.Empty);
            _log = log ?? (_ => { });
        }

        // ================= SALE =================
        public void HandleSaleResponse(SaleResponse resp)
        {
            try
            {
                _status("OnSaleResponse !!!");
                _setWaitingFlag(false);

                if (resp?.Success == true && resp.Payment != null)
                {
                    var pay = resp.Payment;
                    var amountCents = pay?.amount ?? 0L;   // pay? => long? nên dùng được với ??

                    _status($"Sale OK • Amount: {(amountCents / 100.0m):0.00}");
                    _log($"[Clover] Sale OK. OrderID={pay.order?.id}, PaymentID(ext)={pay.externalPaymentId}, PaymentID={pay.id}");

                    // gọi event mới: (Payment, SaleResponse)
                    SaleSucceeded?.Invoke(pay, resp);

                    // show receipt options như cũ
                    TryDisplayReceiptOptions(pay);

                    _setConfirmPrint(true);
                    PaymentFinished?.Invoke(true);
                }
                else
                {
                    string reason = resp?.Reason ?? resp?.Message ?? "Unknown";
                    if (string.IsNullOrEmpty(reason))
                        reason = resp?.Message;

                    _status($"Sale FAIL: {reason}");
                    _log($"[Clover] Sale FAIL. Reason={reason}");
                    SaleFailed?.Invoke(reason);
                    PaymentFinished?.Invoke(false);
                }
            }
            catch (Exception ex)
            {
                _status("Sale exception: " + ex.Message);
                _log("[Clover] Sale exception: " + ex);
                SaleFailed?.Invoke(ex.Message);
                PaymentFinished?.Invoke(false);
            }
        }

        // ================= TIP =================
        public void HandleTipAdded(TipAddedMessage msg)
        {
            // Nhiều SDK dùng TipAmount (cents). Nếu khác, thử msg.Amount.
            long tipCents = 0;
            try
            {
                tipCents = (long)(msg?.tipAmount ?? 0);
            }
            catch { /* ignore parse issues */ }

            _status?.Invoke($"Tip added: {(tipCents / 100.0m):C}");
            TipUpdated?.Invoke(tipCents); // phát cho UI (FormMain) cập nhật
        }

        // ================ VOID ================
        public void HandleVoidPaymentResponse(VoidPaymentResponse resp)
        {
            try
            {
                _status("OnVoidPaymentResponse !!!");

                if (resp == null)
                {
                    _status("Void: response null.");
                    _log("[Clover] Void response is null");
                    VoidFailed?.Invoke("No response");
                    PaymentFinished?.Invoke(false);
                    return;
                }

                if (resp.Success)
                {
                    var pay = resp.Payment; // có thể null tùy SDK
                    _status("Void OK.");
                    _log($"[Clover] Void OK. Order={pay?.order?.id}, Payment(ext)={pay?.externalPaymentId}, Payment={pay?.id}");

                    // Báo nghiệp vụ
                    VoidSucceeded?.Invoke(pay, resp);

                    // Hiện lựa chọn in hóa đơn trên Clover
                    var req = new DisplayPaymentReceiptOptionsRequest
                    {
                        OrderID = resp.Payment?.order?.id,                  // fallback nếu SDK set ở resp
                        PaymentID = !string.IsNullOrEmpty(pay?.externalPaymentId)
                                            ? pay.externalPaymentId
                                            : (pay?.id ?? resp.PaymentId),
                        DisablePrinting = false
                    };
                    SafeDisplayReceiptOptions(req);

                    _setConfirmPrint(true);
                    PaymentFinished?.Invoke(true);
                }
                else
                {
                    var reason = resp.Reason ?? resp.Message ?? "Unknown";
                    _status($"Void FAIL: {reason}");
                    _log($"[Clover] Void FAIL. Reason={reason}");
                    VoidFailed?.Invoke(reason);
                    PaymentFinished?.Invoke(false);
                }
            }
            catch (Exception ex)
            {
                _status("Void exception: " + ex.Message);
                _log("[Clover] Void exception: " + ex);
                VoidFailed?.Invoke(ex.Message);
                PaymentFinished?.Invoke(false);
            }
        }

        // ================ REFUND ================
        public void HandleRefundPaymentResponse(RefundPaymentResponse resp)
        {
            try
            {
                _status("OnRefundPaymentResponse !!!");

                if (resp == null)
                {
                    _status("Refund: response null.");
                    _log("[Clover] Refund response is null");
                    RefundFailed?.Invoke("No response");
                    PaymentFinished?.Invoke(false);
                    return;
                }

                if (resp.Success)
                {
                    // Một số SDK có Payment, một số không -> thử lấy bằng reflection
                    Payment pay = null;
                    var pinfo = resp.GetType().GetProperty("Payment");
                    if (pinfo != null)
                        pay = pinfo.GetValue(resp) as Payment;

                    var orderId = pay?.order?.id ?? resp.OrderId;
                    var paymentId = !string.IsNullOrEmpty(pay?.externalPaymentId) ? pay.externalPaymentId
                                   : (!string.IsNullOrEmpty(pay?.id) ? pay.id
                                   : resp.PaymentId);

                    _status("Refund OK.");
                    _log($"[Clover] Refund OK. Order={orderId}, Payment={paymentId}");

                    // Báo nghiệp vụ (giữ đúng chữ ký mới nếu bạn có)
                    RefundSucceeded?.Invoke(pay, resp);

                    // Hiển thị chọn in receipt (tên property PascalCase: OrderID/PaymentID)
                    var req = new DisplayPaymentReceiptOptionsRequest
                    {
                        OrderID = orderId,
                        PaymentID = paymentId,
                        DisablePrinting = false
                    };
                    SafeDisplayReceiptOptions(req);

                    _setConfirmPrint(true);
                    PaymentFinished?.Invoke(true);
                }
                else
                {
                    var reason = resp.Reason ?? resp.Message ?? "Unknown";
                    _status($"Refund FAIL: {reason}");
                    _log($"[Clover] Refund FAIL. Reason={reason}");
                    RefundFailed?.Invoke(reason);
                    PaymentFinished?.Invoke(false);
                }
            }
            catch (Exception ex)
            {
                _status("Refund exception: " + ex.Message);
                _log("[Clover] Refund exception: " + ex);
                RefundFailed?.Invoke(ex.Message);
                PaymentFinished?.Invoke(false);
            }
        }

        // ------------- dùng lại cho Sale (nếu muốn show receipt sau khi Sale) -------------
        private void TryDisplayReceiptOptions(Payment p)
        {
            //if (p == null) return;

            //var req = new DisplayPaymentReceiptOptionsRequest
            //{
            //    OrderID = p.order?.id,
            //    // ưu tiên externalPaymentId như code cũ của bạn; fallback về id
            //    PaymentID = !string.IsNullOrEmpty(p.externalPaymentId) ? p.externalPaymentId : p.id,
            //    DisablePrinting = false
            //};
            //SafeDisplayReceiptOptions(req);
        }

        private void SafeDisplayReceiptOptions(DisplayPaymentReceiptOptionsRequest req)
        {
            //try
            //{
            //    if (req == null) return;
            //    _clover.DisplayReceiptOptions(req);
            //    _log($"DisplayReceiptOptions: OrderID={req.OrderID}, PaymentID={req.PaymentID}");
            //}
            //catch (Exception ex)
            //{
            //    _status("DisplayReceiptOptions error: " + ex.Message);
            //    _log(ex.ToString());
            //}
        }


        //public static void LoadCartDetail(Control panel, string ticket_id, bool is_combine)
        //{
        //    string jData = "{";
        //    jData += "'ticket_id':'" + ticket_id + "', ";
        //    jData += "'is_combine':'" + (is_combine ? "1" : "0") + "' ";
        //    jData += "}";

        //    string response = ApiHelper.CALL_API("Order/info", jData);
        //    if (Utilitys.IsValidJson(response))
        //    {
        //        JObject jResponce = JObject.Parse(response);
        //        // meta = jResponce["meta"].ToString();

        //        int locationY = 5;
        //        JArray jsonData = JArray.Parse(jResponce["info"].ToString());
        //        foreach (JObject item in jsonData)
        //        {
        //            string staff_id = item["staffId"].ToString();
        //            string staff_name = item["staffName"].ToString();
        //            string service_id = item["id"].ToString();
        //            string service_name = item["name"].ToString();
        //            string service_price = item["price"].ToString();
        //            string service_priceCC = item["price"].ToString();
        //            string quantity = item["quantity"].ToString();
        //            string discount = item["discount"].ToString();
        //            string tip = item["tip"].ToString();
        //            string isQuickMenu = item["isQuickMenu"].ToString();
        //            string catalog = item["catalog"].ToString();
        //            string cashTip = item["cashTip"].ToString();
        //            string saleId = item["ticketId"].ToString();

        //            string staffDone = item["staffDone"] == null ? "0" : item["staffDone"].ToString();
        //            string cart_id = item["cart_id"] == null ? "" : item["cart_id"].ToString();

        //            UCViewCartItem control = new UCViewCartItem(staff_name, service_name, service_price, discount, tip);
        //            control.Width = panel.Width;
        //            control.Location = new Point(0, locationY);
        //            panel.Controls.Add(control);

        //            locationY += control.Height + 5;
        //        }
        //    }

        //}

        //public static string GetCartDetail(string ticket_id, bool is_combine, ref List<PaymentModel> paymentList)
        //{
        //    paymentList = new List<PaymentModel>();

        //    string jData = "{";
        //    jData += "'ticket_id':'" + ticket_id + "', ";
        //    jData += "'is_combine':'" + (is_combine ? "1" : "0") + "' ";
        //    jData += "}";

        //    string response = ApiHelper.CALL_API("Order/info", jData);
        //    if (Utilitys.IsValidJson(response))
        //    {
        //        JObject jResponce = JObject.Parse(response);
        //        string payment_json = jResponce["meta"]["payment_json"]?.ToString();

        //        if (!string.IsNullOrEmpty(payment_json))
        //            paymentList = JsonConvert.DeserializeObject<List<PaymentModel>>(payment_json);
        //    }

        //    return response;
        //}

        //public static void RenderCart(Control panel, string response, ref double total_discount)
        //{
        //    panel.Controls.Clear();
        //    if (Utilitys.IsValidJson(response))
        //    {
        //        JObject jResponce = JObject.Parse(response);

        //        int locationY = 5;
        //        JArray jsonData = JArray.Parse(jResponce["info"].ToString());
        //        foreach (JObject item in jsonData)
        //        {
        //            string staff_id = item["staffId"].ToString();
        //            string staff_name = item["staffName"].ToString();
        //            string service_id = item["id"].ToString();
        //            string service_name = item["name"].ToString();
        //            string service_price = item["price"].ToString();
        //            string service_priceCC = item["price"].ToString();
        //            string quantity = item["quantity"].ToString();
        //            string discount = item["discount"].ToString();
        //            string tip = item["tip"].ToString();
        //            string isQuickMenu = item["isQuickMenu"].ToString();
        //            string catalog = item["catalog"].ToString();
        //            string cashTip = item["cashTip"].ToString();
        //            string saleId = item["ticketId"].ToString();

        //            string staffDone = item["staffDone"] == null ? "0" : item["staffDone"].ToString();
        //            string cart_id = item["cart_id"] == null ? "" : item["cart_id"].ToString();

        //            UCViewCartItem control = new UCViewCartItem(staff_name, service_name, service_price, discount, tip);
        //            control.Width = panel.Width;
        //            control.Location = new Point(0, locationY);
        //            panel.Controls.Add(control);

        //            locationY += control.Height + 5;
        //            total_discount += double.Parse(string.IsNullOrEmpty(discount) ? "0" : discount);
        //        }
        //    }
        //}

        public static double GetPaymentTotal(List<PaymentModel> paymentList)
        {
            if (paymentList == null) return 0;

            double total = 0;
            for (int i = 0; i < paymentList.Count; i++)
            {
                total += paymentList[i].amount;
            }
            return Math.Round(total, 2);
        }
        public static double GetPaymentChargeTotal(List<PaymentModel> paymentList)
        {
            if (paymentList == null) return 0;

            double total = 0;
            for (int i = 0; i < paymentList.Count; i++)
            {
                var pm = paymentList[i];
                if (pm == null || !"CC".Equals(pm.type)) continue;

                var responces = pm.responce;
                if (responces == null) continue;

                for (int j = 0; j < responces.Count; j++)
                {
                    var r = responces[j];
                    if (r == null || !"Success".Equals(r.clover_status)) continue;

                    double.TryParse(r.clover_amount, out double clover_amount);
                    double.TryParse(r.clover_surcharge, out double clover_surcharge);  //đang đơn vị $ rồi, nên không cần chia 100

                    total += Math.Round(clover_amount / 100.0 - clover_surcharge, 2);
                }
            }
            return Math.Round(total, 2);
        }
        public static double GetPaymentCashTotal(List<PaymentModel> paymentList)
        {
            if (paymentList == null) return 0;

            double total = 0;
            for (int i = 0; i < paymentList.Count; i++)
            {
                var pm = paymentList[i];
                if (pm == null) continue;

                if (!"CC".Equals(pm.type))  //Khác credit đưa vào Cash trước
                    total += pm.amount;
            }
            return Math.Round(total, 2);
        }
        public static double GetPaymentChange(List<PaymentModel> paymentList)
        {
            if (paymentList == null) return 0;

            double total = 0;
            for (int i = 0; i < paymentList.Count; i++)
            {
                var pm = paymentList[i];
                if (pm == null) continue;

                if ("CASH".Equals(pm.type))  //Khác credit đưa vào Cash trước
                    total += (pm.cash_received - pm.amt_due);
            }
            return Math.Round(total, 2);
        }
        public static double GetPaymentTotal_CashDiscount(List<PaymentModel> paymentList)
        {
            if (paymentList == null) return 0;

            double total = 0;
            for (int i = 0; i < paymentList.Count; i++)
            {
                total += paymentList[i].cash_discount;
            }
            return Math.Round(total, 2);
        }
        public static bool CheckPaymentMethod(List<PaymentModel> paymentList, string method)
        {
            if (paymentList == null) return false;

            bool exits = false;
            for (int i = 0; i < paymentList.Count; i++)
            {
                if (paymentList[i].type.ToUpper().Equals(method.ToUpper()))
                {
                    exits = true;
                    break;
                }
            }
            return exits;
        }

        public static string SaveCartPayment(string ticket_id, bool is_combine, List<PaymentModel> paymentList, bool send_socket)
        {
            string jPayment = JsonConvert.SerializeObject(paymentList);

            string jData = "{";
            jData += "'ticket_id':'" + ticket_id + "', ";
            jData += "'is_combine':'" + (is_combine ? "1" : "0") + "', ";
            jData += "'jPayment':" + jPayment + ", ";
            jData += "'send_socket':" + (send_socket ? 1 : 0) + " ";
            jData += "}";

            string response = ApiHelper.CALL_API("Order/save-cart-payment", jData);
            if (Utilitys.IsValidJson(response))
            {

            }

            return response;
        }

        public static void AutoDeletePaymentCash()
        {
            string DATA = @"{}";

            string responce = Utilitys.CALL_API("Order/autoDeletePaymentCash", DATA, "POST", true);
            if (responce.ToUpper().StartsWith("ERROR")) //Fail
            {
                string error = responce;
                return;
            }
        }


        public static string UpdateCartInfoSignalR(string jData)
        {
            //đồng bộ qua lại với app checkIn
            string responce = Utilitys.CALL_API("Cart/UpdateCartInfo?paring_code=" + Constants.pairing_code, jData, "POST", true);
            return responce;
        }

        public static string RemoveCustomerInfoSignalR()
        {
            //đồng bộ qua lại với app checkIn
            string responce = Utilitys.CALL_API("Customer/removeCustomerV2?paring_code=" + Constants.pairing_code, "", "GET", true);
            return responce;
        }

    }

}
