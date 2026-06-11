using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Globalization;
using com.clover.remotepay.sdk;
using com.clover.remotepay.transport;
using com.clover.remote.order;
using com.clover.sdk.v3.payments;
using Newtonsoft.Json;
using com.clover.sdk.v3.printer;
using Transport = com.clover.remotepay.transport;
using CloverExamplePOS;
using CreditCardPayment;
using Microsoft.Win32;

namespace POSPrintService
{
    public partial class Form1 : Form, ICloverConnectorListener
    {
        //Startup registry key and value
        private static readonly string StartupKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private static readonly string StartupValue = "POSPrinterTool";

        SynchronizationContext uiThread;
        private VerifySignatureRequest signatureVerifyRequest;

        public Form1()
        {
            InitializeComponent();

            //Crdit Card
            uiThread = SynchronizationContext.Current;

            //this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = true;
            this.MaximizeBox = false;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //ddlSibnatureLocation.Items.Clear();
            //ddlSibnatureLocation.Items.Add("On Paper");
            //ddlSibnatureLocation.Items.Add("On Screen");
            //ddlSibnatureLocation.SelectedValue = "On Paper";

            //Config
            this.readConfig();

            notifyIcon1.BalloonTipText = "Printer and Credit Card Tool";
            notifyIcon1.BalloonTipTitle = "Nganhnails POS Tool";

            //Set the application to run at startup
            RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey, true);
            key.SetValue(StartupValue, Application.ExecutablePath.ToString());

            //Auto run Printer Tool
           
            //Credit card
            //this.InitializeCreditCardConnector(USBConfig);
            //data.CreateStore();

            //Autorun Payment Tool
            //runWorker_Payment = "1";
            //backgroundWorker2.WorkerSupportsCancellation = true;
            //backgroundWorker2.RunWorkerAsync();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                notifyIcon1.Visible = true;
                //notifyIcon1.ShowBalloonTip(5000);
            }
            else
            {
                this.ShowInTaskbar = true;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowInTaskbar = true;
            notifyIcon1.Visible = false;
            this.WindowState = FormWindowState.Normal;
        }

        
        #region Credit Cart Payment

        CloverExamplePosData data = new CloverExamplePosData();

        const String APPLICATION_ID = "com.clover.CloverExamplePOS:3.0.2";
        CloverDeviceConfiguration USBConfig = new USBCloverDeviceConfiguration("__deviceID__", APPLICATION_ID, false, 1);

        DisplayOrder DisplayOrder;
        Dictionary<POSLineItem, DisplayLineItem> posLineItemToDisplayLineItem = new Dictionary<POSLineItem, DisplayLineItem>();

        string curent_order_payment_id = "";
        string curent_order_payment_tip = "0";

        string runWorker_Payment = "0";

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            //if ((backgroundWorker1.CancellationPending == true))
            if (runWorker_Payment.Equals("0"))
            {
                e.Cancel = true;
                lbCreditCardStatus.Text = "Worker Payment Stoped...";
                return;
            }
            else
            {
                lbCreditCardStatus.Text = "Worker Payment Runing...";

                //Check Sale Payment
                string waiting_payment_content = this.checkPayment();
                if (waiting_payment_content.Trim().Length > 0 && waiting_payment_content.Contains("Waiting_Payment"))
                {
                    this.runPaymentFromServer(waiting_payment_content);
                    Thread.Sleep(17000);
                }

                //Check Void Refund
                string waiting_void_refund_content = this.checkVoidRefund();
                if (waiting_void_refund_content.Trim().Length > 0 && waiting_void_refund_content.Contains("Waiting..."))
                {
                    this.runVoidRefundFromServer(waiting_void_refund_content);
                    Thread.Sleep(17000);
                }

                Thread.Sleep(7000);
                backgroundWorker2.ReportProgress(100);
            }
        }

        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lbCreditCardStatus.Text = "Payment Processing - Order Id: " + curent_order_payment_id;
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lbCreditCardStatus.Text = "Payment Finish - Order Id: " + curent_order_payment_id;

            //Call continue
            if( runWorker_Payment.Equals("1") )
                backgroundWorker2.RunWorkerAsync();

        }

        private string checkPayment()
        {
            try
            {
                //POSService.MaxViewWebServiceSoapClient client = new POSService.MaxViewWebServiceSoapClient();

                //string msg = client.GetPaymentWaiting(txtStoreCode.Text, "max@@view@@01235");

                //if (msg.StartsWith("Error: "))
                //{
                //    lbCreditCardStatus.Text = "Error Check Payment From Server " + msg;
                //    return null;
                //}

                //return msg;

                return "";
            }
            catch (Exception ex) //No internet
            {
                lbCreditCardStatus.Text = "Payment Exception: " + ex.Message;
                Thread.Sleep(7000);
                return "";
            }
        }

        private void runPaymentFromServer(string waiting_payment_content)
        {
            try
            {
                this.SaveLOG_Payment(waiting_payment_content, "waiting_payment_content");

                //string[] arr = Regex.Split(waiting_payment_content, " ");
                //string orderId = arr[0];
                //string discount = arr[1];
                //string tax = arr[2];
                //string total = arr[3];
                //string status = arr[4];

                //if (data.CloverConnector == null)
                //{
                //    this.InitializeCreditCardConnector(USBConfig);
                //    data.CreateStore();
                //}

                //if (status.Equals("Waiting_Payment"))
                //{
                //    //Pay credit
                //    this.Pay(orderId, (long)(double.Parse(tax) * 100), (long)(double.Parse(total) * 100));

                //    //Update Content Waiting_Customer_Payment
                //    string new_content = orderId + " " + discount + " " + tax + " " + total + " Waiting_Customer_Payment";
                //    POSService.MaxViewWebServiceSoapClient client = new POSService.MaxViewWebServiceSoapClient();
                //    string msg = client.UpdatePaymentWaiting(txtStoreCode.Text, "max@@view@@01235", new_content);

                //}

                //Waiting Responce...

            }
            catch (Exception ex)
            {
                lbCreditCardStatus.Text = "Payment Exception: " + ex.Message;
                this.SaveLOG_Payment(ex.Message, "Payment Exception: ");
            }
        }

        private string checkVoidRefund()
        {
            try
            {
                //POSService.MaxViewWebServiceSoapClient client = new POSService.MaxViewWebServiceSoapClient();

                //string msg = client.GetVoidRefundWaiting(txtStoreCode.Text, "max@@view@@01235");

                //if (msg.StartsWith("Error: "))
                //{
                //    lbCreditCardStatus.Text = "Error Check Void/Refund From Server " + msg;
                //    return null;
                //}

                //return msg;

                return "";
            }
            catch (Exception ex) //No internet
            {
                lbCreditCardStatus.Text = "Void/Refund Exception: " + ex.Message;
                Thread.Sleep(7000);
                return "";
            }
        }

        private void runVoidRefundFromServer(string waiting_voidrefund_content)
        {
            try
            {
                this.SaveLOG_Payment(waiting_voidrefund_content, "waiting_voidrefund_content");

                string[] arr = Regex.Split(waiting_voidrefund_content, " ");
                
                //string orderId = arr[0];
                //string transactionType = arr[1];
                //string payment_id = arr[2];
                //string cloverOrderId = arr[3];
                //string employeeId = arr[4];
                //string status = arr[5];

                //if (data.CloverConnector == null)
                //{
                //    this.InitializeCreditCardConnector(USBConfig);
                //    data.CreateStore();
                //}

                //if (status.Equals("Waiting..."))
                //{
                //    if (transactionType.Equals("Void"))
                //        this.Void(orderId, payment_id, cloverOrderId, employeeId);
                //    else
                //        this.Refund(orderId, payment_id, cloverOrderId, 10);

                //    //Update Content Waiting_Customer_Payment
                //    string new_content = orderId + " " + transactionType + " " + payment_id + " " + cloverOrderId + " " + employeeId + " Waiting_Customer_Action";
                //    POSService.MaxViewWebServiceSoapClient client = new POSService.MaxViewWebServiceSoapClient();
                //    string msg = client.UpdateVoidRefundWaiting(txtStoreCode.Text, "max@@view@@01235", new_content);

                //}

                //Waiting Responce...

            }
            catch (Exception ex)
            {
                lbCreditCardStatus.Text = "Void/Refund Exception: " + ex.Message;
                this.SaveLOG_Payment(ex.Message, "Void/Refund Exception: ");
            }
        }



        public void InitializeCreditCardConnector(CloverDeviceConfiguration config)
        {
            try
            {
                if (data.CloverConnector != null)
                {
                    data.CloverConnector.RemoveCloverConnectorListener(this);

                    OnDeviceDisconnected(); // for any disabling, messaging, etc.
                    //SaleButton.Enabled = false; // everything can work except Pay
                    data.CloverConnector.Dispose();
                }

                data.CloverConnector = CloverConnectorFactory.createICloverConnector(config);
                data.CloverConnector.AddCloverConnectorListener(this);
                data.CloverConnector.InitializeConnection();

                lbCreditCardStatus.Text = "Credit Card Connected !!!";
            }
            catch (Exception ex)
            {
                lbCreditCardStatus.Text = "Connector Error: " + ex.Message;
            }
        }

        public void DisableCreditCardConnector(CloverDeviceConfiguration config)
        {
            try
            {
                if (data.CloverConnector != null)
                {
                    data.CloverConnector.RemoveCloverConnectorListener(this);

                    OnDeviceDisconnected(); // for any disabling, messaging, etc.
                    //SaleButton.Enabled = false; // everything can work except Pay
                }

                data.CloverConnector.Dispose();
                data.CloverConnector = null;

                lbCreditCardStatus.Text = "Credit Card DisConnected !!!";
            }
            catch (Exception ex)
            {
                lbCreditCardStatus.Text = "DisConnected Error: " + ex.Message;
            }
        }


        //////////////// Sale methods /////////////
        private void Pay(string orderId, long tax_amount, long amount)
        {
            this.SaveLOG_Payment(orderId + " " + tax_amount + " " + amount, "Call Pay");

            curent_order_payment_id = orderId;
            curent_order_payment_tip = "0";

            try
            {
                //Display         
                data.Store.CreateOrder();

                //Sale
                SaleRequest request = new SaleRequest();
                request.ExternalId = orderId + "__" + ExternalIDUtil.GenerateRandomString(16);
                request.Amount = amount;
                request.Type = PayIntent.TransactionType.PAYMENT;

                request.CardEntryMethods = 34567;
                request.CardNotPresent = false;

                // SaleRequest supported TipModes: TIP_PROVIDED, NO_TIP, ON_SCREEN_BEFORE_PAYMENT
                // NOTE: ON_PAPER would turn the Sale into an AUTH, so it is not valid here
                if(chkTipsOn.Checked)
                    request.TipMode = com.clover.remotepay.sdk.TipMode.ON_SCREEN_BEFORE_PAYMENT;
                else
                    request.TipMode = com.clover.remotepay.sdk.TipMode.NO_TIP;

                request.TipAmount = null;
                request.TippableAmount = null;

                request.TipSuggestions = null;
                request.TaxAmount = tax_amount;

                request.DisableCashback = null;
                request.DisableRestartTransactionOnFail = null;

                request.DisablePrinting = false;
                request.DisableReceiptSelection = false;

                request.DisableDuplicateChecking = null;

                request.SignatureThreshold = 0;
                request.SignatureEntryLocation = null;

                if (chkSigOnPaper.Checked)
                    request.AutoAcceptSignature = true;
                else  //On Screen Show Popup
                    request.AutoAcceptSignature = false;

                request.AutoAcceptPaymentConfirmations = true;

                request.AllowOfflinePayment = null;
                request.ApproveOfflinePaymentWithoutPrompt = null;
                request.ForceOfflinePayment = null;

                data.CloverConnector.Sale(request);
            }
            catch (Exception ex)
            {
                this.SaveLOG_Payment(ex.Message, "Call Payment Order (" + orderId + ") Exception");
            }
        }


        private void Void(string orderId, string paymentId, string cloverOrderId, string employeeId)
        {
            this.SaveLOG_Payment(orderId + " " + paymentId + " " + cloverOrderId + " " + employeeId, "Call Void");

            try
            {
                VoidPaymentRequest request = new VoidPaymentRequest();

                request.PaymentId = paymentId;
                request.EmployeeId = employeeId;
                request.OrderId = cloverOrderId;

                request.VoidReason = "USER_CANCEL";

                data.CloverConnector.VoidPayment(request);
            }
            catch (Exception ex)
            {
                this.SaveLOG_Payment(ex.Message, "Call Void Order (" + orderId + ") Exception");
            }
        }

        private void Refund(string orderId, string paymentId, string cloverOrderId, long amount)
        {
            this.SaveLOG_Payment(orderId + " " + paymentId + " " + cloverOrderId + " " + amount, "Call Refund");

            try
            {
                RefundPaymentRequest request = new RefundPaymentRequest();

                request.DisablePrinting = false;
                request.DisableReceiptSelection = false;

                request.PaymentId = paymentId;
                request.OrderId = cloverOrderId;

                request.Amount = amount;
                request.FullRefund = true;

                data.CloverConnector.RefundPayment(request);
            }
            catch (Exception ex)
            {
                this.SaveLOG_Payment(ex.Message, "Call Refund Order (" + orderId + ") Exception");
            }

        }


        private void UpdateDisplayOrderTotals()
        {
            double tax_amount = 0.05;
            double PreTaxSubTotal = 10;
            double total = 15;

            DisplayOrder.tax = tax_amount.ToString("C2");
            DisplayOrder.subtotal = (PreTaxSubTotal / 100.0).ToString("C2");
            DisplayOrder.total = (total / 100.0).ToString("C2");


        }

        private void NewOrder(int welcomeDelay)
        {
            foreach (POSOrder order in data.Store.Orders) //any pending orders will be removed when creating a new one
            {
                if (order.Status == POSOrder.OrderStatus.PENDING)
                {
                    data.Store.Orders.Remove(order);
                }
            }

            //data.Store.CreateOrder();

            DisplayOrder = DisplayFactory.createDisplayOrder();
            DisplayOrder.title = Guid.NewGuid().ToString();
            posLineItemToDisplayLineItem.Clear();

            data.CloverConnector.ShowWelcomeScreen();

        }



        #region Event Implement

        public void OnSaleResponse(SaleResponse response)
        {
            lbCreditCardStatus.Text = "OnSaleResponse !!! " + response.Result + " " + response.Message;
            this.SaveLOG_Payment(response.Result + " " + response.Message, "OnSaleResponse !!!");

            try
            {
                string clover_order_id = "";
                string orderId = "";
                string clover_amount = "0";
                string clover_msg = "";
                string clover_status = "";
                string payment_id = ""; 
                string order_id = "";
                string employee_id = "";

                clover_order_id = response.Payment.externalPaymentId;
                if (clover_order_id.Trim().Length > 0)
                {
                    orderId = Regex.Split(clover_order_id, "__")[0];
                }

                if (response.Success)
                {
                    clover_amount = response.Payment.amount.ToString();
                    clover_msg = "";
                    clover_status = "Success";
                    payment_id = response.Payment.externalPaymentId;
                    order_id = response.Payment.order.id;
                    employee_id = response.Payment.employee.id;

                    POSPayment payment = new POSPayment(response.Payment.id, response.Payment.externalPaymentId, response.Payment.order.id, response.Payment.employee.id, response.Payment.amount, response.Payment.tipAmount ?? 0, response.Payment.cashbackAmount);
                    payment.PaymentSource = response.Payment;

                    data.Store.CurrentOrder.Status = POSOrder.OrderStatus.CLOSED;
                    payment.PaymentStatus = POSPayment.Status.PAID;

                    data.Store.CurrentOrder.AddPayment(payment);
                    data.Store.CurrentOrder.Date = (new DateTime(1970, 1, 1)).AddMilliseconds(response.Payment.createdTime).ToLocalTime();


                    //Print Debit Test
                    DisplayPaymentReceiptOptionsRequest request = new DisplayPaymentReceiptOptionsRequest()
                    {
                        OrderID = payment.OrderID,
                        PaymentID = payment.PaymentID,
                        DisablePrinting = false
                    };
                    data.CloverConnector.DisplayPaymentReceiptOptions(request);
                }
                else if (response.Result.Equals(ResponseCode.FAIL))
                {
                    clover_msg = response.Message;
                    clover_status = "FAIL";
                }
                else if (response.Result.Equals(ResponseCode.CANCEL))
                {
                    clover_msg = response.Message;
                    clover_status = "CANCEL";
                }

                this.SaveLOG_Payment("clover_order_id: " + clover_order_id + " clover_status: " + clover_status + " clover_msg: " + clover_msg
                            + " clover_amount: " + clover_amount + " payment_id: " + payment_id + " order_id: " + order_id + " employee_id: " + employee_id + " Tip: " + curent_order_payment_tip, "OnSaleResponse");

                //Update Db
                //POSService.MaxViewWebServiceSoapClient client = new POSService.MaxViewWebServiceSoapClient();
                //string msg = client.UpdatePaymentStatus(txtStoreCode.Text, "max@@view@@01235", orderId, clover_order_id, clover_status, clover_msg, clover_amount, curent_order_payment_tip,
                //                                                 payment_id, order_id, employee_id);

                //this.SaveLOG_Payment(msg, "Call Service MSG");

                //this.NewOrder(1);
            }
            catch (Exception ex)
            {
                this.SaveLOG_Payment(ex.Message, "OnSaleResponse Exception");
            }

        }

        public void OnTipAdded(TipAddedMessage message)
        {
            lbCreditCardStatus.Text = "OnTipAdded !!!";

            string msg = message.tipAmount.ToString();
            this.SaveLOG_Payment(msg, "OnTipAdded !!!");

            if (message.tipAmount > 0)
            {
                curent_order_payment_tip = message.tipAmount.ToString();

                msg = "Tip Added: " + (message.tipAmount / 100.0).ToString("C2");
                OnDeviceActivityStart(new CloverDeviceEvent(0, msg));
            }

        }

        public void OnVoidPaymentResponse(VoidPaymentResponse response)
        {
            lbCreditCardStatus.Text = "OnVoidPaymentResponse";

            try
            {
                string clover_order_id = "";
                string orderId = "";
                string clover_amount = "0";
                string clover_msg = "";
                string clover_status = "";
                string payment_id = "";
                string order_id = "";
                string employee_id = "";

                clover_order_id = response.Payment.externalPaymentId;
                if (clover_order_id.Trim().Length > 0)
                {
                    orderId = Regex.Split(clover_order_id, "__")[0];
                }

                if (response.Success)
                {
                    clover_amount = response.Payment.amount.ToString();
                    clover_msg = "";
                    clover_status = "Success";
                    payment_id = response.Payment.externalPaymentId;
                    order_id = response.Payment.order.id;
                    employee_id = response.Payment.employee.id;

                    //Print Void Test
                    DisplayPaymentReceiptOptionsRequest request = new DisplayPaymentReceiptOptionsRequest()
                    {
                        OrderID = response.Payment.order.id,
                        PaymentID = response.Payment.externalPaymentId,
                        DisablePrinting = false
                    };
                    data.CloverConnector.DisplayPaymentReceiptOptions(request);

                }
                else if (response.Result.Equals(ResponseCode.FAIL))
                {
                    lbCreditCardStatus.Text = "Void Fail: " + response.Message;
                }

                this.SaveLOG_Payment("clover_order_id: " + clover_order_id + " clover_status: " + clover_status + " clover_msg: " + clover_msg
                                + " clover_amount: " + clover_amount + " payment_id: " + payment_id + " order_id: " + order_id + " employee_id: " + employee_id, "OnVoidPaymentResponse");

                //Update Db
                //POSService.MaxViewWebServiceSoapClient client = new POSService.MaxViewWebServiceSoapClient();
                //string msg = client.UpdateVoidRefundStatus(txtStoreCode.Text, "max@@view@@01235", "Void", orderId, clover_order_id, clover_status, clover_msg, clover_amount,
                //                                                 payment_id, order_id, employee_id);

                //this.SaveLOG_Payment(msg, "Call Void Service MSG");
                
            }
            catch (Exception ex)
            {
                this.SaveLOG_Payment(ex.Message, "OnVoidPaymentResponse Exception");
            }

        }


        public void OnRefundPaymentResponse(RefundPaymentResponse response)
        {
            lbCreditCardStatus.Text = "OnRefundPaymentResponse !!!";

            try
            {
                string clover_order_id = "";
                string orderId = "";
                string clover_amount = "0";
                string clover_msg = "";
                string clover_status = "";
                string payment_id = "";
                string order_id = "";
                string employee_id = "";

                clover_order_id = response.PaymentId;
                if (clover_order_id.Trim().Length > 0)
                {
                    orderId = Regex.Split(clover_order_id, "__")[0];
                }

                if (response.Success)
                {
                    clover_amount = response.Refund.amount.ToString();
                    clover_msg = "";
                    clover_status = "Success";
                    payment_id = response.PaymentId;
                    order_id = response.OrderId;
                    employee_id = response.Refund.employee.id;

                    //string paymentID = response.PaymentId;
                    //string employeeID = response.Refund.employee.id;
                    //if (paymentID != null)
                    //{
                    //    POSRefund refund = new POSRefund(response.Refund.id, response.PaymentId, response.OrderId, employeeID, response.Refund.amount ?? 0);

                    //    //((POSOrder)orderObj).Status = POSOrder.OrderStatus.OPEN; //re-open order for editing/payment
                    //    //((POSOrder)orderObj).AddRefund(refund);
                    //}
                    //else
                    //{
                    //    lbCreditCardStatus.Text = "Couldn't find paymentID " + paymentID;
                    //}

                    //Print Refund Test                    
                    DisplayPaymentReceiptOptionsRequest request = new DisplayPaymentReceiptOptionsRequest()
                    {
                        OrderID = response.OrderId,
                        PaymentID = response.PaymentId,
                        DisablePrinting = false
                    };
                    data.CloverConnector.DisplayPaymentReceiptOptions(request);
                }
                else if (response.Result.Equals(ResponseCode.FAIL))
                {
                    lbCreditCardStatus.Text = "Refund Fail: " + response.Message;
                }

                this.SaveLOG_Payment("clover_order_id: " + clover_order_id + " clover_status: " + clover_status + " clover_msg: " + clover_msg
                                + " clover_amount: " + clover_amount + " payment_id: " + payment_id + " order_id: " + order_id + " employee_id: " + employee_id, "OnRefundPaymentResponse");

                //Update Db
                //POSService.MaxViewWebServiceSoapClient client = new POSService.MaxViewWebServiceSoapClient();
                //string msg = client.UpdateVoidRefundStatus(txtStoreCode.Text, "max@@view@@01235", "Refund", orderId, clover_order_id, clover_status, clover_msg, clover_amount,
                //                                                 payment_id, order_id, employee_id);

                //this.SaveLOG_Payment(msg, "Call Refund Service MSG");

            }
            catch (Exception ex)
            {
                this.SaveLOG_Payment(ex.Message, "OnRefundPaymentResponse Exception");
            }
        }



        public void OnAuthResponse(AuthResponse response)
        {
            lbCreditCardStatus.Text = "OnAuthResponse !!!";
        }

        public void OnCapturePreAuthResponse(CapturePreAuthResponse response)
        {
            lbCreditCardStatus.Text = "OnCapturePreAuthResponse !!!";
        }

        public void OnCloseoutResponse(CloseoutResponse response)
        {
            lbCreditCardStatus.Text = "OnCloseoutResponse !!!";
        }

        public void OnConfirmPaymentRequest(ConfirmPaymentRequest request)
        {
            lbCreditCardStatus.Text = "OnConfirmPaymentRequest !!!";
        }

        public void OnCustomActivityResponse(CustomActivityResponse response)
        {
            lbCreditCardStatus.Text = "OnCustomActivityResponse !!!";
        }

        public void OnCustomerProvidedData(CustomerProvidedDataEvent response)
        {
            lbCreditCardStatus.Text = "OnCustomerProvidedData !!!";
        }

        public void OnDeviceActivityEnd(CloverDeviceEvent deviceEvent)
        {
            lbCreditCardStatus.Text = "OnDeviceActivityEnd !!!";
        }

        public void OnDeviceActivityStart(CloverDeviceEvent deviceEvent)
        {
            lbCreditCardStatus.Text = "OnDeviceActivityStart !!!";
        }

        public void OnDeviceConnected()
        {
            lbCreditCardStatus.Text = "OnDeviceConnected !!!";
        }

        public void OnDeviceDisconnected()
        {
            lbCreditCardStatus.Text = "OnDeviceDisconnected !!!";
        }

        public void OnDeviceError(CloverDeviceErrorEvent deviceErrorEvent)
        {
            lbCreditCardStatus.Text = "OnDeviceError !!!";
        }

        public void OnDeviceReady(MerchantInfo merchantInfo)
        {
            lbCreditCardStatus.Text = "OnDeviceReady !!!";
        }

        public void OnDisplayReceiptOptionsResponse(DisplayReceiptOptionsResponse response)
        {
            lbCreditCardStatus.Text = "OnDisplayReceiptOptionsResponse !!!";
        }

        public void OnInvalidStateTransitionResponse(InvalidStateTransitionNotification message)
        {
            lbCreditCardStatus.Text = "OnInvalidStateTransitionResponse !!!";
        }

        public void OnManualRefundResponse(ManualRefundResponse response)
        {
            lbCreditCardStatus.Text = "OnManualRefundResponse !!!";
        }

        public void OnMessageFromActivity(MessageFromActivity response)
        {
            lbCreditCardStatus.Text = "OnMessageFromActivity !!!";
        }

        public void OnPreAuthResponse(PreAuthResponse response)
        {
            lbCreditCardStatus.Text = "OnPreAuthResponse !!!";
        }

        public void OnPrintJobStatusRequest(PrintJobStatusRequest request)
        {
            lbCreditCardStatus.Text = "OnPrintJobStatusRequest !!!";
        }

        public void OnPrintJobStatusResponse(PrintJobStatusResponse response)
        {
            lbCreditCardStatus.Text = "OnPrintJobStatusResponse !!!";
        }

        public void OnPrintManualRefundDeclineReceipt(PrintManualRefundDeclineReceiptMessage message)
        {
            lbCreditCardStatus.Text = "OnPrintManualRefundDeclineReceipt !!!";
        }

        public void OnPrintManualRefundReceipt(PrintManualRefundReceiptMessage message)
        {
            lbCreditCardStatus.Text = "OnPrintManualRefundReceipt !!!";
        }

        public void OnPrintPaymentDeclineReceipt(PrintPaymentDeclineReceiptMessage message)
        {
            lbCreditCardStatus.Text = "OnPrintPaymentDeclineReceipt !!!";
        }

        public void OnPrintPaymentMerchantCopyReceipt(PrintPaymentMerchantCopyReceiptMessage message)
        {
            lbCreditCardStatus.Text = "OnPrintPaymentMerchantCopyReceipt !!!";
        }

        public void OnPrintPaymentReceipt(PrintPaymentReceiptMessage message)
        {
            lbCreditCardStatus.Text = "OnPrintPaymentReceipt !!!";
        }

        public void OnPrintRefundPaymentReceipt(PrintRefundPaymentReceiptMessage message)
        {
            lbCreditCardStatus.Text = "OnPrintRefundPaymentReceipt !!!";
        }

        public void OnReadCardDataResponse(ReadCardDataResponse response)
        {
            lbCreditCardStatus.Text = "OnReadCardDataResponse !!!";
        }

        public void OnResetDeviceResponse(ResetDeviceResponse response)
        {
            lbCreditCardStatus.Text = "OnResetDeviceResponse !!!";
        }

        public void OnRetrieveDeviceStatusResponse(RetrieveDeviceStatusResponse response)
        {
            lbCreditCardStatus.Text = "OnRetrieveDeviceStatusResponse !!!";
        }

        public void OnRetrievePaymentResponse(RetrievePaymentResponse response)
        {
            lbCreditCardStatus.Text = "OnRetrievePaymentResponse !!!";
        }

        public void OnRetrievePendingPaymentsResponse(RetrievePendingPaymentsResponse response)
        {
            lbCreditCardStatus.Text = "OnRetrievePendingPaymentsResponse !!!";
        }

        public void OnRetrievePrintersResponse(RetrievePrintersResponse response)
        {
            lbCreditCardStatus.Text = "OnRetrievePrintersResponse !!!";
        }


        public void OnTipAdjustAuthResponse(TipAdjustAuthResponse response)
        {
            lbCreditCardStatus.Text = "OnTipAdjustAuthResponse !!!";

            string msg = "PaymentId: " + response.PaymentId + " Message: " + response.Message + " TipAmount: " + response.TipAmount + " Result: " + response.Result + " Reason: " + response.Reason;
            this.SaveLOG_Payment(msg, "OnTipAdjustAuthResponse !!!");

            //if (response.Success)
            //{
            //    POSOrder order = data.Store.GetOrder(response.PaymentId);
            //    order.ModifyTipAmount(response.PaymentId, response.TipAmount);
            //}
            //else
            //{
            //    uiThread.Send(delegate(object state)
            //    {
            //        AlertForm.Show(this, response.Reason, response.Message);
            //    }, null);
            //}
        }

        public void OnVaultCardResponse(VaultCardResponse response)
        {
            lbCreditCardStatus.Text = "OnVaultCardResponse !!!";
        }

        public void OnVerifySignatureRequest(VerifySignatureRequest request)
        {
            lbCreditCardStatus.Text = "OnVerifySignatureRequest !!!";

            if (chkSigOnScreen.Checked)
            {
                //Nếu Tool đang ẩn thì bật lên để ký
                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.ShowInTaskbar = true;
                    notifyIcon1.Visible = false;
                    this.WindowState = FormWindowState.Normal;
                }

                //Mở Form ký lên
                Form1 parentForm = this;
                uiThread.Send(delegate(object state)
                {
                    SignatureForm sigForm = new SignatureForm(parentForm);
                    sigForm.VerifySignatureRequest = request;
                    sigForm.Show();
                }, null);
            }
            else //On Paper
                request.Accept();
        }

        public void OnVoidPaymentRefundResponse(VoidPaymentRefundResponse response)
        {
            lbCreditCardStatus.Text = "OnVoidPaymentRefundResponse !!!";
        }

        

        #endregion


        #region Button Click

        private void btnRun_Click(object sender, EventArgs e)
        {
            //this.saveConfig();

            //backgroundWorker1.WorkerSupportsCancellation = true;

            //btnRun.Enabled = false;
            //btnStop.Enabled = true;

            //if (backgroundWorker1.IsBusy)
            //{
            //    backgroundWorker1.CancelAsync();
            //}
            //else
            //{
            //    lbPrinterStatus.Text = "Running...";
            //    btnRun.Text = "Running...";
            //    backgroundWorker1.RunWorkerAsync();
            //}


            //backgroundWorker2.WorkerSupportsCancellation = true;
            //runWorker_Payment = "1";
            //btnStartCreditCardConnect.Enabled = false;
            //btnStopScreditCard.Enabled = true;

            //if (backgroundWorker2.IsBusy)
            //{
            //    backgroundWorker2.CancelAsync();
            //}
            //else
            //{
            //    lbCreditCardStatus.Text = "Payment Running...";
            //    backgroundWorker2.RunWorkerAsync();
            //}

            this.InitializeCreditCardConnector(USBConfig);
            data.CreateStore();

            btnStopScreditCard.Enabled = true;
            btnStartCreditCardConnect.Enabled = false;

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            backgroundWorker1.WorkerSupportsCancellation = true;
            if (backgroundWorker1.IsBusy == true )
            {
                backgroundWorker1.CancelAsync();
                //backgroundWorker1.Dispose();
            }

            //backgroundWorker1.Dispose();
            //backgroundWorker1 = null;

            btnRun.Enabled = true;
            lbPrinterStatus.Text = "Printer Stoped...";
            

            backgroundWorker2.WorkerSupportsCancellation = true;
            if (backgroundWorker2.IsBusy == true )
            {
                backgroundWorker2.CancelAsync();
            }


            this.DisableCreditCardConnector(USBConfig);  //Disable Clover
            lbCreditCardStatus.Text = "Payment Stoped...";
            runWorker_Payment = "0";

            btnStop.Enabled = false;
        }

        private void btnStartCreditCardConnect_Click(object sender, EventArgs e)
        {
            runWorker_Payment = "1";

            if (!backgroundWorker2.IsBusy)
                backgroundWorker2.RunWorkerAsync();
            else
                lbCreditCardStatus.Text = "Worker Payment Busy...";

            this.InitializeCreditCardConnector(USBConfig);  //Enable Clover
            btnStopScreditCard.Enabled = true;
            btnStartCreditCardConnect.Enabled = false;
        }

        private void btnStopScreditCard_Click(object sender, EventArgs e)
        {
            runWorker_Payment = "0";
            backgroundWorker2.WorkerSupportsCancellation = true;

            if (backgroundWorker2.IsBusy == true)
            {
                backgroundWorker2.CancelAsync();
            }

            this.DisableCreditCardConnector(USBConfig);  //Disable Clover
            btnStopScreditCard.Enabled = false;
            btnStartCreditCardConnect.Enabled = true;
        }

        private void chkTipsOn_CheckedChanged(object sender, EventArgs e)
        {
            if (chkTipsOn.Checked)
                chkTipsOff.Checked = false;

            this.saveConfig();
        }

        private void chkTipsOff_CheckedChanged(object sender, EventArgs e)
        {
            if (chkTipsOff.Checked)
                chkTipsOn.Checked = false;

            this.saveConfig();
        }

        private void chkSigOnPaper_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSigOnPaper.Checked)
                chkSigOnScreen.Checked = false;

            this.saveConfig();
        }

        private void chkSigOnScreen_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSigOnScreen.Checked)
                chkSigOnPaper.Checked = false;

            this.saveConfig();
        }

        #endregion


        #endregion


        #region Read Config File, LOG

        private void readConfig()
        {
            string acrobatURL = "";
            string hostName = "";
            string storeCode = "";

            string tips = "";
            string signatureLocation = "";

            try
            {
                string logReadUrl = txtForderLog.Text + "SERVICE_CONFIG.txt";

                using (StreamReader sr = File.OpenText(logReadUrl))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        if (s.StartsWith("acrobatURL: "))
                            acrobatURL = s.Replace("acrobatURL: ", "");
                        else if (s.StartsWith("hostName: "))
                            hostName = s.Replace("hostName: ", "");
                        else if (s.StartsWith("storeCode: "))
                            storeCode = s.Replace("storeCode: ", "");
                        else if (s.StartsWith("tips: "))
                            tips = s.Replace("tips: ", "");
                        else if (s.StartsWith("signatureLocation: "))
                            signatureLocation = s.Replace("signatureLocation: ", "");
                    }

                    sr.Close();
                }
            }
            catch (Exception ex)
            {
                acrobatURL = "";
                hostName = "";
                storeCode = "";
                tips = "On";
                signatureLocation = "On Paper";
            }

            if (acrobatURL.Trim().Length > 0 && hostName.Trim().Length > 0 && storeCode.Trim().Length > 0)
            {
                txtAcrobatURL.Text = acrobatURL;
                txtDomain.Text = hostName;
                txtStoreCode.Text = storeCode;

                if (tips.Equals("Off"))
                {
                    chkTipsOn.Checked = false;
                    chkTipsOff.Checked = true;
                }
                else
                {
                    chkTipsOn.Checked = true;
                    chkTipsOff.Checked = false;
                }

                if (signatureLocation.Equals("On Screen"))
                {
                    chkSigOnPaper.Checked = false;
                    chkSigOnScreen.Checked = true;
                }
                else
                {
                    chkSigOnPaper.Checked = true;
                    chkSigOnScreen.Checked = false;
                }
            }
            else
            {
                //txtAcrobatURL.Text = ConfigurationManager.AppSettings["acrobatURL"].ToString();
                //txtDomain.Text = ConfigurationManager.AppSettings["hostName"].ToString();
                //txtStoreCode.Text = ConfigurationManager.AppSettings["storeCode"].ToString();

                chkTipsOn.Checked = true;
                chkTipsOff.Checked = false;

                chkSigOnPaper.Checked = true;
                chkSigOnScreen.Checked = false;
            }

        }

        private void saveConfig()
        {
            try
            {
                string forderLog = txtForderLog.Text;
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                string logWriteUrl = forderLog + "SERVICE_CONFIG.txt";

                StreamWriter sw = new StreamWriter(logWriteUrl, false);

                sw.WriteLine("acrobatURL: " + txtAcrobatURL.Text);
                sw.WriteLine("hostName: " + txtDomain.Text);
                sw.WriteLine("storeCode: " + txtStoreCode.Text);

                string tips = chkTipsOn.Checked ? "On" : "Off";
                string signature = chkSigOnPaper.Checked ? "On Paper" : "On Screen";

                sw.WriteLine("tips: " + tips);
                sw.WriteLine("signatureLocation: " + signature);

                sw.Close();
            }
            catch (Exception ex)
            {
                this.SaveLOG(ex.Message, "Save Config Error");
            }
        }


        private void SaveLOG(string log, string title)
        {
            try
            {
                string forderLog = txtForderLog.Text;
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                string logWriteUrl = forderLog + "SERVICE_LOG.txt";

                StreamWriter sw = new StreamWriter(logWriteUrl, true);
                try
                {
                    sw.WriteLine(DateTime.Now.ToString() + "." + title + ": " + log);
                }
                catch
                {
                    sw.WriteLine(DateTime.Now.ToString() + "." + title + " EXCEPTION: " + log);
                }

                sw.Close();
            }
            catch (Exception ex) { }
        }

        private void SaveLOG_Payment(string log, string title)
        {
            try
            {
                string forderLog = txtForderLog.Text;
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                string logWriteUrl = forderLog + "SERVICE_PAYMENT_LOG.txt";

                StreamWriter sw = new StreamWriter(logWriteUrl, true);
                try
                {
                    sw.WriteLine(DateTime.Now.ToString() + "." + title + ": " + log);
                }
                catch
                {
                    sw.WriteLine(DateTime.Now.ToString() + "." + title + " EXCEPTION: " + log);
                }

                sw.Close();
            }
            catch (Exception ex) { }
        }


        #endregion

        private void btnTest_Click(object sender, EventArgs e)
        {
            //Mở Form ký lên
            //Form1 parentForm = this;
            //uiThread.Send(delegate (object state)
            //{
            //    Form2 sigForm = new Form2(parentForm);
            //    sigForm.Show();
            //}, null);

            

        }

        public void OnIncrementPreAuthResponse(IncrementPreAuthResponse response)
        {
            throw new NotImplementedException();
        }
    }
}
