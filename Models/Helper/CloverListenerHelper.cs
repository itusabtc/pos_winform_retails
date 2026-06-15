using com.clover.remotepay.sdk;
using com.clover.remotepay.transport;
using NailsChekin.Models;
using System;
using System.Threading;

namespace NailsChekin.Models.Helper
{
    public class CloverListenerHelper : ICloverConnectorListener
    {
        // ==== Bridge delegates để không đụng trực tiếp UI ở đây ====
        private readonly Action<string> _updateStatus;                       // UpdateCreditDeviceStatus
        private readonly Action<bool> _enableDisableClover;                  // EnableDisableClover
        private readonly Action<bool> _setCloverStatus;                      // set cờ cloverStatus trong FormMain
        private readonly Func<string> _getCurrentToken;                      // this.current_clover_token (log lỗi)
        private readonly Action<CloverDeviceEvent> _deviceActivityStartUI;   // _processUI.HandleDeviceActivityStart
        private readonly Action<InputOption> _invokeInputOption;             // data.CloverConnector.InvokeInputOption
        private readonly Action<DisplayPaymentReceiptOptionsRequest> _displayReceiptOptions; // DisplayReceiptOptions
        private readonly Action<VerifySignatureRequest> _showSignatureForm;  // Mở form ký
        private readonly Action _reconnect;                                  // Nếu FormMain có hàm reconnect

        // Biến hỗ trợ tránh reconnect đúp
        private int _isReconnecting = 0;

        // ====== Public events để Manager subscribe ======
        public event Action<SaleResponse> SaleResponse;
        public event Action<TipAddedMessage> TipAdded;
        public event Action<VoidPaymentResponse> VoidPaymentResponse;
        public event Action<RefundPaymentResponse> RefundPaymentResponse;

        public event Action DeviceDisconnected;                  // rớt cổng/USB
        public event Action<string, string> DeviceError;         // (code, msg)
        public event Action DeviceReady;                         // thiết bị sẵn sàng
        public event Action<string, string> InvalidStateTransition; // (requested, current)
        public event Action<bool> ResetDeviceCompleted;          // true = reset thành công

        public CloverListenerHelper(
            Action<string> updateStatus,
            Action<bool> enableDisableClover,
            Action<bool> setCloverStatus,
            Func<string> getCurrentToken,
            Action<CloverDeviceEvent> deviceActivityStartUI,
            Action<InputOption> invokeInputOption,
            Action<DisplayPaymentReceiptOptionsRequest> displayReceiptOptions,
            Action<VerifySignatureRequest> showSignatureForm,
            Action reconnect = null
        )
        {
            _updateStatus = updateStatus;
            _enableDisableClover = enableDisableClover;
            _setCloverStatus = setCloverStatus;
            _getCurrentToken = getCurrentToken;
            _deviceActivityStartUI = deviceActivityStartUI;
            _invokeInputOption = invokeInputOption;
            _displayReceiptOptions = displayReceiptOptions;
            _showSignatureForm = showSignatureForm;
            _reconnect = reconnect;
        }

        // ====== Nhóm sự kiện "core" ======
        public void OnSaleResponse(SaleResponse response)
        {
            SaleResponse?.Invoke(response);
        }

        public void OnVoidPaymentResponse(VoidPaymentResponse response)
        {
            VoidPaymentResponse?.Invoke(response);
        }

        public void OnRefundPaymentResponse(RefundPaymentResponse response)
        {
            RefundPaymentResponse?.Invoke(response);
        }

        public void OnDeviceActivityStart(CloverDeviceEvent deviceEvent)
        {
            _updateStatus?.Invoke("OnDeviceActivityStart !!!");
            _deviceActivityStartUI?.Invoke(deviceEvent);
        }

        public void OnDeviceActivityEnd(CloverDeviceEvent deviceEvent)
        {
            _updateStatus?.Invoke("OnDeviceActivityEnd !!!");
        }

        public void OnVerifySignatureRequest(VerifySignatureRequest request)
        {
            _updateStatus?.Invoke("OnVerifySignatureRequest !!!");

            // Nếu ký trên màn hình thì mở form ký; nếu "On Paper" thì Accept
            if (Constants.chkSigOnScreen)
            {
                _showSignatureForm?.Invoke(request);
            }
            else
            {
                request.Accept();
            }
        }

        public void OnDeviceConnected()
        {
            _updateStatus?.Invoke("OnDeviceConnected !!!");
            _setCloverStatus?.Invoke(true);
            _enableDisableClover?.Invoke(true);
            LogHelper.SaveLOG_Test_Clover("", "OnDeviceConnected");
        }

        public void OnDeviceDisconnected()
        {
            _updateStatus?.Invoke("OnDeviceDisconnected !!!");
            _setCloverStatus?.Invoke(false);
            _enableDisableClover?.Invoke(false);
            LogHelper.SaveLOG_Test_Clover("", "OnDeviceDisconnected");

            // phát event cho Manager
            DeviceDisconnected?.Invoke();
        }

        public void OnDeviceError(CloverDeviceErrorEvent deviceErrorEvent)
        {
            try
            {
                var msg = deviceErrorEvent?.Message ?? "";
                var code = deviceErrorEvent != null ? deviceErrorEvent.ErrorType.ToString() : string.Empty;
                _updateStatus?.Invoke("OnDeviceError !!! " + (msg.Length > 100 ? msg.Substring(0, 100) : msg));
                _setCloverStatus?.Invoke(false);

                // phát sự kiện cho Manager
                DeviceError?.Invoke(code, msg);

                // auto-reconnect nhanh như FormMain
                //if ((msg.Contains("IoTimedOut") || msg.Contains("Win32Error")) &&
                //    Interlocked.Exchange(ref _isReconnecting, 1) == 0)
                //{
                //    try
                //    {
                //        _reconnect?.Invoke();
                //        LogHelper.SaveLOG_Test_Clover("[CLOVER] Reconnected thành công", "OnDeviceError");
                //    }
                //    catch (Exception ex)
                //    {
                //        LogHelper.SaveLOG_Test_Clover("[CLOVER] Reconnect thất bại: " + ex, "OnDeviceError");
                //    }
                //    finally
                //    {
                //        Interlocked.Exchange(ref _isReconnecting, 0);
                //    }
                //}

                //_enableDisableClover?.Invoke(false);
            }
            catch { }
        }

        public void OnDeviceReady(MerchantInfo merchantInfo)
        {
            _updateStatus?.Invoke("OnDeviceReady !!!");
            _setCloverStatus?.Invoke(true);
            _enableDisableClover?.Invoke(true);
            DeviceReady?.Invoke();
            // LogHelper.SaveLOG_Test_Clover(merchantInfo.merchantName, "OnDeviceReady");
        }

        // ====== Các response/notification còn lại (giữ behavior hiện tại: chỉ update status/log) ======
        public void OnAuthResponse(AuthResponse response) => _updateStatus?.Invoke("OnAuthResponse !!!");
        public void OnCapturePreAuthResponse(CapturePreAuthResponse response) => _updateStatus?.Invoke("OnCapturePreAuthResponse !!!");
        public void OnCloseoutResponse(CloseoutResponse response) => _updateStatus?.Invoke("OnCloseoutResponse !!!");
        public void OnConfirmPaymentRequest(ConfirmPaymentRequest request) => _updateStatus?.Invoke("OnConfirmPaymentRequest !!!");
        public void OnCustomActivityResponse(CustomActivityResponse response) => _updateStatus?.Invoke("OnCustomActivityResponse !!!");
        public void OnCustomerProvidedData(CustomerProvidedDataEvent response) => _updateStatus?.Invoke("OnCustomerProvidedData !!!");
        public void OnDisplayReceiptOptionsResponse(DisplayReceiptOptionsResponse response) => _updateStatus?.Invoke("OnDisplayReceiptOptionsResponse !!!");
        public void OnInvalidStateTransitionResponse(InvalidStateTransitionNotification message)
        {
            var requested = message?.RequestedTransition ?? "";
            var current = message?.State ?? "";
            _updateStatus?.Invoke($"OnInvalidStateTransitionResponse: {requested} <- {current}");
            InvalidStateTransition?.Invoke(requested, current);
        }
        public void OnManualRefundResponse(ManualRefundResponse response) => _updateStatus?.Invoke("OnManualRefundResponse !!!");
        public void OnMessageFromActivity(MessageFromActivity response) => _updateStatus?.Invoke("OnMessageFromActivity !!!");
        public void OnPreAuthResponse(PreAuthResponse response) => _updateStatus?.Invoke("OnPreAuthResponse !!!");

        public void OnPrintJobStatusRequest(PrintJobStatusRequest request)
        {
            _updateStatus?.Invoke("OnPrintJobStatusRequest !!!");
            // nếu cần kích "confirm print" thì expose delegate riêng rồi gọi ở đây
        }

        public void OnPrintJobStatusResponse(PrintJobStatusResponse response)
        {
            _updateStatus?.Invoke("OnPrintJobStatusResponse !!!");
        }

        public void OnPrintManualRefundDeclineReceipt(PrintManualRefundDeclineReceiptMessage message) => _updateStatus?.Invoke("OnPrintManualRefundDeclineReceipt !!!");
        public void OnPrintManualRefundReceipt(PrintManualRefundReceiptMessage message) => _updateStatus?.Invoke("OnPrintManualRefundReceipt !!!");
        public void OnPrintPaymentDeclineReceipt(PrintPaymentDeclineReceiptMessage message) => _updateStatus?.Invoke("OnPrintPaymentDeclineReceipt !!!");
        public void OnPrintPaymentMerchantCopyReceipt(PrintPaymentMerchantCopyReceiptMessage message) => _updateStatus?.Invoke("OnPrintPaymentMerchantCopyReceipt !!!");
        public void OnPrintPaymentReceipt(PrintPaymentReceiptMessage message) => _updateStatus?.Invoke("OnPrintPaymentReceipt !!!");
        public void OnPrintRefundPaymentReceipt(PrintRefundPaymentReceiptMessage message) => _updateStatus?.Invoke("OnPrintRefundPaymentReceipt !!!");

        public void OnReadCardDataResponse(ReadCardDataResponse response) => _updateStatus?.Invoke("OnReadCardDataResponse !!!");
        public void OnResetDeviceResponse(ResetDeviceResponse response)
        {
            _updateStatus?.Invoke("OnResetDeviceResponse !!!");
            ResetDeviceCompleted?.Invoke(response?.Success ?? false);
        }
        public void OnRetrieveDeviceStatusResponse(RetrieveDeviceStatusResponse response) => _updateStatus?.Invoke("OnRetrieveDeviceStatusResponse !!!");
        public void OnRetrievePaymentResponse(RetrievePaymentResponse response) => _updateStatus?.Invoke("OnRetrievePaymentResponse !!!");
        public void OnRetrievePendingPaymentsResponse(RetrievePendingPaymentsResponse response) => _updateStatus?.Invoke("OnRetrievePendingPaymentsResponse !!!");
        public void OnRetrievePrintersResponse(RetrievePrintersResponse response) => _updateStatus?.Invoke("OnRetrievePrintersResponse !!!");

        // ====== tiện ích: Process UI gọi giống FormMain.getHandler(io) ======
        public EventHandler MakeInputHandler(InputOption io)
        {
            return new EventHandler((s, e) => _invokeInputOption?.Invoke(io));
        }

        public void DisplayReceiptOptions(DisplayPaymentReceiptOptionsRequest req)
        {
            _displayReceiptOptions?.Invoke(req);
        }

        // ====== KHÔNG throw NotImplemented để tránh crash nếu SDK gọi ======
        public void OnTipAdjustAuthResponse(TipAdjustAuthResponse response) { /* optional log */ }
        public void OnIncrementPreAuthResponse(IncrementPreAuthResponse response) { /* optional log */ }
        public void OnVoidPaymentRefundResponse(VoidPaymentRefundResponse response) { /* optional log */ }
        public void OnVaultCardResponse(VaultCardResponse response) { /* optional log */ }

        public void OnTipAdded(TipAddedMessage message)
        {
            TipAdded?.Invoke(message);
        }
    }
}

