using NailsChekin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NailsChekin.Popup
{
    public partial class FormConfirmDepositPayment : Form
    {
        FormMain parentForm = null;
        public string apptId = "";
        public string name = "";

        public double deposit_amount = 0;
        public double total_amount = 0;
        public double remain_amount = 0;

        public string isCreditCard = "";
        public string fastPayAmount = "";
        public string isServiceNow = "";
        public string custom_split_amount = "";
        public string amount_received = "";

        public FormConfirmDepositPayment()
        {
            InitializeComponent();
        }

        public FormConfirmDepositPayment(FormMain parent, string appt_deposit_id, string name, string deposit_amount, string total_amount, string remain_amount,
                                                          string isCreditCard, string fastPayAmount, string custom_split_amount, string amount_received)
        {
            InitializeComponent();

            this.parentForm = parent;

            this.apptId = appt_deposit_id;
            this.name = name;

            this.deposit_amount = double.Parse(deposit_amount);
            this.total_amount = double.Parse(total_amount);
            this.remain_amount = double.Parse(remain_amount);

            //Store from Parent
            this.isCreditCard = isCreditCard;
            this.fastPayAmount = fastPayAmount;
            this.isServiceNow = "0";
            this.custom_split_amount = custom_split_amount;
            this.amount_received = amount_received;

            lbDeposit.Text = "Customer " + this.name + " already paid $" + this.deposit_amount + " deposited!";
            lbRemainAmount.Text = "$" + this.remain_amount;
        }

        private void btnConfirm_YES_Click(object sender, EventArgs e)
        {
            double total_paying = this.remain_amount;

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

            if (Math.Round(cashAmount + creditAmount, 2) != this.remain_amount)
            {
                MessageBox.Show("Please check Cash/Credit Amount");
                return;
            }

            double cashReceived = 0;
            if (txtCashReceived.Text.Trim().Length > 0)
            {
                double.TryParse(txtCashReceived.Text, out cashReceived);
            }

            if (cashReceived <= 0)
                cashReceived = cashAmount;

            if (cashAmount >= this.total_amount) //ONLY PAYMENT CASH
            {
                //this.parentForm.POS_Payment("0", cashAmount.ToString(), "0", "0", cashReceived.ToString());
            }
            else
            {
                //Phần Payment còn lại sẽ chạy qua máy Credit Card
                double remaining_amount = (this.total_amount - cashAmount);
                remaining_amount += Utilitys.getSurcharge(remaining_amount, 0);

                //this.parentForm.POS_Payment("1", remaining_amount.ToString(), "0", cashAmount.ToString(), cashReceived.ToString(), 
                //                            "YES", this.apptId, this.deposit_amount.ToString());
            }

            //this.parentForm.POS_Payment(isCreditCard, total_paying.ToString(), this.isServiceNow, this.custom_split_amount, this.amount_received, 
            //                            "YES", this.apptId, this.deposit_amount.ToString() );

            this.Dispose();
        }

        private void btnConfirm_NO_Click(object sender, EventArgs e)
        {
            double total_paying = this.total_amount;

            //this.parentForm.POS_Payment(isCreditCard, total_paying.ToString(), this.isServiceNow, this.custom_split_amount, this.amount_received,
            //                            "NO", "", "" );

            this.Dispose();
        }


    }
}
