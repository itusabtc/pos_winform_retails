using CreditCardPayment;
using DevExpress.XtraEditors;
using NailsChekin.Models;
using NailsChekin.Models.Helper;
using NailsChekin.Models.ListModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NailsChekin.Popup
{
    public partial class FormFullPayment : Form
    {
        FormMain parentForm = null;

        public double total_amount = 100;
        public double split_amount = 0;

        private double total_cash = 0;
        private double total_charged = 0;
        private double total_charge_by_way = 0;

        public List<PaymentModel> paymentList = new List<PaymentModel>();
        public double surcharge_amount = 0;

        public string use_system_credit_setting = "1";
        public bool cloverStatus = false;
        public string CurrentPairingToken = ""; // If you store & load this value, you can skip pairing codes on the device most of the time.
        //CloverEventConnector clover; // Event connector is just a simple wrapper around the CloverConnector & CloverConnectorListener to get listener messages as C# events, class code can be viewed in the SDK github repo

        public FormFullPayment()
        {
            InitializeComponent();
        }

        public FormFullPayment(FormMain parent, double total, bool cloverStatus)
        {
            InitializeComponent();

            this.parentForm = parent;
            this.total_amount = total;
            
            lbFullAmount.Text = "$ " + this.total_amount;
            //txtTotalCredit.Text = total.ToString();

            txtCash.Text = "0";
            txtCash.Focus();
        }

        private void FormFullPayment_Shown(object sender, EventArgs e)
        {
            //MainPOS mainPOS = new MainPOS();
            //this.use_system_credit_setting = mainPOS.GetStoreSetting("use_system_credit_setting");

            //string responce = Utilitys.CALL_API("Store/getStoreSetting", "", "GET", true);
            //if (!responce.StartsWith("Error"))
            //{
            //    this.use_system_credit_setting = JObject.Parse(responce)["credit_setting_on"].ToString();
            //}

            //this.UpdateChargeText("1");
        }

        private void UpdateChargeText(string stt)
        {
            bool chkSurChargeOn = Utilitys.GetConfig("chkSurChargeOn", Constants.chkSurChargeOn);
            string surCharge_percent = Utilitys.GetConfig("surCharge_percent", Constants.surCharge_percent);
            string surCharge_unit = Utilitys.GetConfig("surCharge_unit", Constants.surCharge_unit);

            if (chkSurChargeOn && surCharge_percent.Trim().Length > 0)
            {
                string text = surCharge_unit.Equals("%") ? (surCharge_percent + surCharge_unit) : (surCharge_unit + surCharge_percent);
                btnPAYNow.Text = "CARD #" + stt + " CHARGE" + Environment.NewLine + "(SC." + text + ")";
            }
            else
            {
                btnPAYNow.Text = "CARD #" + stt + " CHARGE";
            }
        }


        #region button click event

        private void btnPAYNow_Click(object sender, EventArgs e)
        {
            this.EnableDisableControl(false);

            double creditAmount = 0;
            if (txtCredit.Text.Trim().Length > 0)
            {
                double.TryParse(txtCredit.Text, out creditAmount);
            }

            double totalCreditAmount = 0;
            if (txtTotalCredit.Text.Trim().Length > 0)
            {
                double.TryParse(txtTotalCredit.Text, out totalCreditAmount);
            }
            double cashAmount = 0;
            if (txtCash.Text.Trim().Length > 0)
            {
                double.TryParse(txtCash.Text, out cashAmount);
            }

            creditAmount = Math.Round(creditAmount, 2);
            if (Math.Round(creditAmount, 2) <= 0)
            {
                MessageBox.Show("Please check Credit Amount");
                this.EnableDisableControl(true);
                return;
            }

            if (use_system_credit_setting.Equals("1"))
            {
                //this.surcharge_amount = Utilitys.getSurchargeSplit(creditAmount, 0, cashAmount); //Surcharge không tính trên tip
                //creditAmount += this.surcharge_amount;

                //if (!Constants.credit_card_device.Equals("CLOVER"))
                //    this.parentForm.CodePay_Payment(totalCreditAmount, creditAmount, this);
                //else
                //    this.parentForm.Clover_Payment_Simple(totalCreditAmount, creditAmount, this);
            }
            else //Không dùng Clover
            {
                PaymentModel model = new PaymentModel("CC", creditAmount);
                var responce = new CloverResponce();
                responce.clover_amount = (creditAmount * 100).ToString();
                responce.clover_status = "Success";
                model.responce.Add(responce);
                this.paymentList.Add(model);

                this.Check_Payment_Correct();
            }           
        }

        public void EnableDisableControl(bool enable)
        {
            try
            {
                this.BeginInvoke(new Action(() =>
                {
                    btnPAYNow.Enabled = enable;
                    btnConfirmPayment.Enabled = enable;
                }));
            }
            catch (Exception ex)
            {
                Utilitys.SaveLOG_Crash("Message: " + ex.Message + "\nStackTrace:\n" + ex.StackTrace, "App Error FormPaymetnFull EnableDisableControl");
            }
        }


        #endregion
        
        public void Check_Payment_Correct()
        {
            try
            {
                total_charged = this.Get_Total_Charged();
                int stt = this.Get_STT_Charged();
                double credit_missing = this.Get_Total_Credit_Missing();

                lbTotalCharged.Text = "$" + total_charged;
                lbCardNum.Text = "CARD #" + stt;
                btnPAYNow.Text = "CARD #" + stt + " CHARGE";
                this.UpdateChargeText(stt.ToString());

                txtCredit.Text = credit_missing.ToString();
                LogHelper.SaveLOG_Payment("--3 credit_missing: " + credit_missing + " -- total_amount: " + this.total_amount + " -- total_charged: " + total_charged, "Check_Payment_Correct");

                if (credit_missing > 0.1) //Chềnh lệch 1,2 đ thì coi như complete
                {  
                    btnPAYNow.Enabled = true;
                }
                else
                {
                    btnPAYNow.Enabled = false;
                    this.ConfirmPaymentNow();  //Payment đủ thì qua luôn
                }

                double total_payment = this.Get_Total_Payment();

                LogHelper.SaveLOG_Payment("--3.1 total_payment: " + total_payment + " -- total_amount: " + this.total_amount + " -- total_charged: " + total_charged, "Check_Payment_Correct");
                if (total_payment > this.total_amount)
                {
                    btnConfirmPayment.Enabled = false;
                }
                else
                {
                    btnConfirmPayment.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_Payment(ex.Message + Environment.NewLine + ex.StackTrace, "STEP SPLIT PAYMENT branch Exception");
            }
        }

        private double Get_Total_Payment()
        {
            double cashAmount = 0;
            if (txtCash.Text.Trim().Length > 0)
            {
                double.TryParse(txtCash.Text, out cashAmount);
            }

            double charged = this.Get_Total_Charged();
            return Math.Round(cashAmount + charged, 2);
        }

        private double Get_Total_Credit_Missing()
        {
            double totalCreditAmount = 0;
            if (txtTotalCredit.Text.Trim().Length > 0)
            {
                double.TryParse(txtTotalCredit.Text, out totalCreditAmount);
            }

            double charged = this.Get_Total_Charged();
            double missing = (totalCreditAmount - charged ) < 0 ? 0 : ( totalCreditAmount - charged );

            return Math.Round(missing, 2);
        }

        private double Get_Total_Charged()
        {
            double total = 0;
            for (int i = 0; i < this.paymentList.Count; i++)
            {
                if (this.paymentList[i].type.Equals("CC"))
                {
                    var responces = this.paymentList[i].responce;
                    for (int j = 0; j < responces.Count; j++)
                    {
                        if (responces[j].clover_status.Equals("Success"))
                        {
                            double clover_amount = double.Parse(responces[j].clover_amount) / 100.0;
                            double clover_surcharge = double.Parse(responces[j].clover_surcharge);  //đang đơn vị $ rồi, nên không cần chia 100 

                            total += Math.Round(clover_amount - clover_surcharge, 2);
                        }
                    }
                }
            }
            return Math.Round(total, 2);
        }

        private int Get_STT_Charged()
        {
            int stt = 1;
            for (int i = 0; i < this.paymentList.Count; i++)
            {
                if (this.paymentList[i].type.Equals("CC"))
                {
                    var responces = this.paymentList[i].responce;
                    for (int j = 0; j < responces.Count; j++)
                    {
                        if (responces[j].clover_status.Equals("Success"))
                        {
                            stt++;
                        }
                    }
                }
            }
            return stt;
        }
        
        private void txtCash_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            txtCashReceived.Text = txtCash.Text;

            double cashReceived = 0;
            if (txtCashReceived.Text.Trim().Length > 0)
            {
                double.TryParse(txtCashReceived.Text, out cashReceived);
            }
            double cashAmount = 0;
            if (txtCash.Text.Trim().Length > 0)
            {
                double.TryParse(txtCash.Text, out cashAmount);
            }

            txtChangeDue.Text = (Math.Round(cashReceived - cashAmount, 2)).ToString();

            double charged = this.Get_Total_Charged();  //trường hợp nhập ngược sau khi đã charge
            double credit = (Math.Round(this.total_amount - cashAmount - charged, 2) < 0 ) ? 0 : Math.Round(this.total_amount - cashAmount - charged, 2);
            if (credit <= 0)
            {
                btnPAYNow.Enabled = false;
                txtCredit.Enabled = false;
            }
            else
            {
                btnPAYNow.Enabled = true;
                txtCredit.Enabled = true;
            }

            //Default
            txtTotalCredit.Text = credit.ToString();
            txtCredit.Text = credit.ToString();
        }

        private void txtTotalCredit_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            //double creditAmount = 0;
            //if (txtTotalCredit.Text.Trim().Length > 0)
            //{
            //    double.TryParse(txtTotalCredit.Text, out creditAmount);
            //}

            //txtCash.Text = Math.Round(this.total_amount - creditAmount, 2).ToString();

            //double cashAmount = 0;
            //if (txtCash.Text.Trim().Length > 0)
            //{
            //    double.TryParse(txtCash.Text, out cashAmount);
            //}

            ////Tính phần thiếu
            //double charged = Get_Total_Charged();
            //double request_amount = Math.Round(this.total_amount - cashAmount - charged, 2);
            //if (request_amount < 0)
            //    request_amount = 0;

            //txtCredit.Text = Math.Round(this.total_amount - cashAmount - charged, 2).ToString();
        }

        private void btnConfirmPayment_Click(object sender, EventArgs e)
        {
            this.ConfirmPaymentNow();
        }

        private void ConfirmPaymentNow()
        {
            try
            {
                //Check AMOUNT TOTAL >= AMOUNT
                double total_payment = this.Get_Total_Payment();
                if (total_payment < (this.total_amount - 2)) //Chênh lệch 1, 2 đồng do làm tròn thì coi như complete
                {
                    MessageBox.Show("Please check payment amount!");
                    return;
                }

                //Check nếu có cash đã được add trong payment list chưa
                if (txtCash.Text.Trim().Length > 0 && !txtCash.Text.Equals("0"))
                {
                    bool add_method_cash = false;
                    for (int i = 0; i < this.paymentList.Count; i++)
                    {
                        if (this.paymentList[i].type.Equals("Cash"))
                        {
                            add_method_cash = true;
                            break;
                        }
                    }

                    if (!add_method_cash)
                    {
                        PaymentModel model = new PaymentModel("Cash", double.Parse(txtCashReceived.Text));
                        this.paymentList.Add(model);
                    }
                }

                double cashAmount = 0;
                if (txtCash.Text.Trim().Length > 0)
                {
                    double.TryParse(txtCash.Text, out cashAmount);
                }

                double creditAmount = 0;
                if (txtCredit.Text.Trim().Length > 0)
                {
                    double.TryParse(txtCredit.Text, out creditAmount);
                }

                cashAmount = Math.Round(cashAmount, 2);
                creditAmount = Math.Round(creditAmount, 2);

                double cashReceived = 0;
                if (txtCashReceived.Text.Trim().Length > 0)
                {
                    double.TryParse(txtCashReceived.Text, out cashReceived);
                }

                if (cashReceived <= 0)
                    cashReceived = cashAmount;

                //if (cashAmount >= this.total_amount) //ONLY PAYMENT CASH
                //{
                //    this.parentForm.POS_FullPayment("0", cashAmount.ToString(), cashReceived.ToString(), this.paymentList, total_payment);
                //}
                //else
                //{
                //    this.parentForm.POS_FullPayment("1", cashAmount.ToString(), cashReceived.ToString(), this.paymentList, total_payment);
                //}

                LogHelper.SaveLOG_Payment(this.parentForm.payment_result, "Process Ticket Result");
                this.Dispose();
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_Payment(ex.Message + Environment.NewLine + ex.StackTrace, "ConfirmPaymentNow Exception");
            }
        }

        
    }


}
