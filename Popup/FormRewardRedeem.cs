using NailsChekin.Models;
using NailsChekin.MyControls;
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
    public partial class FormRewardRedeem : Form
    {
        FormMain formMain;

        double reward_balance = 0;
        double reward_percent_discount = 0;
        double sub_total_amount = 0;
        double amount_redeem = 0;

        public FormRewardRedeem()
        {
            InitializeComponent();
        }

        public FormRewardRedeem(FormMain parent, string reward_balance, string sub_total_amount, string percent_owner = "100")
        {
            InitializeComponent();

            this.formMain = parent;

            this.reward_balance = double.Parse(reward_balance);
            this.sub_total_amount = double.Parse(sub_total_amount);

            txtTotalService.Text = "$" + sub_total_amount;
            this.UpdateRedeemAmount();

        }

        private void FormRewardRedeem_FormClosed(object sender, FormClosedEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000);
                try
                {
                    if (!this.IsDisposed)
                    {
                        this.Invoke((Action)(() => this.Dispose()));
                    }
                }
                catch { /* form đã dispose rồi thì thôi */ }
            });
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            double redeem_percent = string.IsNullOrEmpty(txtRedeemPercent.Text) ? 0 : Utilitys.getTotalAmount(txtRedeemPercent.Text);
            if (Core.USING_REWARD_PERCENT())
            {
                double max_percent_redeem = Core.REWARD_REDEEM_MAX_PERCENT();
                if (redeem_percent > max_percent_redeem)
                {
                    txtRedeemPercent.Focus();
                    CustomMessageBox.Show("The percent you entered is higher than your max reward redeem setting");
                    return;
                }
                if (redeem_percent > this.reward_balance)
                {
                    txtRedeemPercent.Focus();
                    CustomMessageBox.Show("The amount you entered is higher than your reward balance");
                    return;
                }
            }
            else
            {
                double max_amount_redeem = Core.REWARD_REDEEM_MAX_AMOUNT();
                if (max_amount_redeem > 0 && this.amount_redeem > max_amount_redeem)
                {
                    txtRedeemAmount.Focus();
                    CustomMessageBox.Show("The redeem amount you entered is higher than your max reward redeem setting (" + max_amount_redeem + ")");
                    return;
                }
                if (this.amount_redeem > this.reward_balance)
                {
                    txtRedeemAmount.Focus();
                    CustomMessageBox.Show("The amount you entered is higher than your reward balance");
                    return;
                }
            }

            if (this.amount_redeem <= 0)
            {
                CustomMessageBox.Show("Please check a positive amount to redeem");
                return;
            }

            this.formMain.UpdateRedeemAmount(this.amount_redeem, this.reward_percent_discount, "100");
            this.Close();
        }

        public void UpdateRedeemAmount()
        {
            this.reward_percent_discount = 0;
            this.amount_redeem = this.GET_MAX_REDEEM_AMOUNT();

            if (Core.USING_REWARD_PERCENT())
            {
                double max_percent_redeem = Core.REWARD_REDEEM_MAX_PERCENT();
                txtRedeemPercent.Text = this.reward_balance > max_percent_redeem ? max_percent_redeem.ToString() : this.reward_balance.ToString();
            }
            else
            {
                double max_amount_redeem = Core.REWARD_REDEEM_MAX_AMOUNT();
                if (max_amount_redeem > 0 && amount_redeem > max_amount_redeem)
                    txtRedeemAmount.Text = "$" + max_amount_redeem;
                else
                    txtRedeemAmount.Text = "$" + amount_redeem;
            }
        }

        private void txtRedeemAmount_EditValueChanged(object sender, EventArgs e)
        {
            this.amount_redeem = Utilitys.getTotalAmount(txtRedeemAmount.Text);

            if (this.amount_redeem > this.reward_balance)
                this.amount_redeem = reward_balance;
        }

        private void txtRedeemPercent_EditValueChanged(object sender, EventArgs e)
        {
            double max_percent_redeem = Core.REWARD_REDEEM_MAX_PERCENT();
            double redeem_percent = Utilitys.getTotalAmount(txtRedeemPercent.Text);
            if (redeem_percent > max_percent_redeem)
                redeem_percent = max_percent_redeem;

            this.amount_redeem = Math.Round(sub_total_amount * redeem_percent / 100.0, 2);
            this.reward_percent_discount = redeem_percent;

            txtRedeemAmount.Text = "$" + amount_redeem;
        }

        private double GET_MAX_REDEEM_AMOUNT()
        {
            double max_amount_redeem = 0;
            if (Core.USING_REWARD_PERCENT())
            {
                txtRewardBalance.Text = this.reward_balance + "%";
                txtRedeemPercent.Enabled = true;
                txtRedeemAmount.Enabled = false;

                //Max % setting
                if (reward_balance > 0)
                {
                    if (reward_balance > Core.REWARD_REDEEM_MAX_PERCENT())
                        this.reward_percent_discount = Core.REWARD_REDEEM_MAX_PERCENT();
                    else
                        this.reward_percent_discount = reward_balance;

                    max_amount_redeem = Math.Round(sub_total_amount * reward_percent_discount / 100.0, 2);
                }
            }
            else
            {
                txtRewardBalance.Text = "$" + this.reward_balance;
                txtRedeemPercent.Enabled = false;
                txtRedeemAmount.Enabled = true;

                if (reward_balance > 0)
                {
                    if (reward_balance > sub_total_amount)
                        max_amount_redeem = sub_total_amount;
                    else
                        max_amount_redeem = reward_balance;
                }
            }

            return Math.Round(max_amount_redeem, 2);
        }

    }
}
