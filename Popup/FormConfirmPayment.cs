using NailsChekin;
using System;
using System.Windows.Forms;

namespace NailsChekin.Popup
{
    public partial class FormConfirmPayment : Form
    {
        FormMain parentForm = null;
        public string ticketId = "";

        public double collect_amount = 0;
        public double total_amount = 0;
        public double remain_amount = 0;

        public FormConfirmPayment()
        {
            InitializeComponent();
        }

        public FormConfirmPayment(FormMain parent, string ticketId, string collect_amount, string total_amount, string remain_amount)
        {
            InitializeComponent();

            this.parentForm = parent;

            this.ticketId = ticketId;
            this.collect_amount = double.Parse(collect_amount);
            this.total_amount = double.Parse(total_amount);
            this.remain_amount = double.Parse(remain_amount);

            lbCollectAmount.Text = "$" + this.collect_amount;
            lbTotalAmount.Text = "$" + this.remain_amount;
            lbRemainAmount.Text = "$" + this.remain_amount;

            lbCollectAmount2.Text = "$" + this.collect_amount;
            lbTotalAmount2.Text = "$" + this.remain_amount;
            lbRemainAmount2.Text = "$" + this.remain_amount;

            btnPaymentCard_Change.Text = "CHARGE ($" + this.remain_amount + ")";
            btnPaymentCard_Cash.Text = "CASH ($" + this.remain_amount + ")";
        }

        private void btnPaymentCard_Change_Click(object sender, EventArgs e)
        {
            //string isCreditCard = "1";

            //double total_paying = this.remain_amount;
            //this.parentForm.POS_Payment(isCreditCard, total_paying.ToString(), "0", "0", "0");

            //if (this.parentForm.payment_result.Trim().Length <= 0)
            //    this.Dispose();
        }

        private void btnPaymentCard_Cash_Click(object sender, EventArgs e)
        {
            //string isCreditCard = "0";

            //double total_paying = this.remain_amount;
            //this.parentForm.POS_Payment(isCreditCard, "$" + total_paying.ToString(), "0", "0", "0"); //Fastpay Process $ Prefix

            //if(this.parentForm.payment_result.Trim().Length <= 0)
            //    this.Dispose();
        }


    }
}
