using Newtonsoft.Json.Linq;
using NailsChekin.UserControl;
using NailsChekin;
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
    public partial class FormDashboard : Form
    {
        FormMain parentForm = null;

        public FormDashboard()
        {
            InitializeComponent();
        }

        public FormDashboard(FormMain parent)
        {
            InitializeComponent();

            this.parentForm = parent;
        }

        private void FormDashboard_Load(object sender, EventArgs e)
        {
            this.Init_DailySale();
        }

        private void Init_DailySale()
        {
            //MainPOS mainPOS = new MainPOS();
            //string jData = mainPOS.Get_Dashboard();

            //UCDashboardItem temp = new UCDashboardItem();

            //if (jData.Trim().Length > 20)
            //{
            //    JArray historys = JArray.Parse(jData);

            //    string total_service = "0";
            //    string cash = "0";
            //    string credit_debit = "0";
            //    string giftcard = "0";
            //    string giftcard_sale = "0";
            //    string discount = "0";
            //    string reward = "0";
            //    string counpon = "0";
            //    string credit_tip = "0";
            //    string cash_tip = "0";
            //    string revenue = "0";

            //    string new_walk_ins = "0";
            //    string frequent_walk_ins = "0";
            //    string avg_ticket = "0";
            //    string new_appoiment = "0";
            //    string frequent_appoiment = "0";
            //    string frequent_checkIn = "0";
            //    string new_checkIn = "0";

            //    foreach (JObject item in historys.Children())
            //    {
            //        total_service = item.GetValue("total_service").ToString();
            //        cash = item.GetValue("cash").ToString();
            //        credit_debit = item.GetValue("credit_debit").ToString();
            //        giftcard = item.GetValue("giftcard").ToString();
            //        giftcard_sale = item.GetValue("giftcard_sale").ToString();
            //        discount = item.GetValue("discount").ToString();
            //        reward = item.GetValue("reward").ToString();
            //        counpon = item.GetValue("counpon").ToString();
            //        credit_tip = item.GetValue("credit_tip").ToString();
            //        cash_tip = item.GetValue("cash_tip").ToString();
            //        revenue = item.GetValue("revenue").ToString();
            //    }

            //    int locationX = 10;
            //    int locationY = 10;

            //    UCDashboardItem control = new UCDashboardItem(total_service, "TOTAL SERVICE", "");
            //    control.Location = new Point(locationX, locationY);
            //    locationX += temp.Width + 10;
            //    panelDailySale.Controls.Add(control);

            //    control = new UCDashboardItem(cash, "CASH", "");
            //    control.Location = new Point(locationX, locationY);
            //    locationX = locationX + temp.Width + 10;
            //    panelDailySale.Controls.Add(control);

            //    control = new UCDashboardItem(credit_debit, "CREDIT", "");
            //    control.Location = new Point(locationX, locationY);
            //    locationX = locationX + temp.Width + 10;
            //    panelDailySale.Controls.Add(control);

            //    control = new UCDashboardItem(giftcard, "GIFTCARD REDEEM", "");
            //    control.Location = new Point(locationX, locationY);
            //    locationX = locationX + temp.Width + 10;
            //    panelDailySale.Controls.Add(control);

            //    control = new UCDashboardItem(giftcard_sale, "GIFTCARD SALE", "");
            //    control.Location = new Point(locationX, locationY);
            //    locationX = locationX + temp.Width + 10;
            //    panelDailySale.Controls.Add(control);

            //    control = new UCDashboardItem(discount, "DISCOUNT", "");
            //    control.Location = new Point(locationX, locationY);
            //    locationX = locationX + temp.Width + 10;
            //    panelDailySale.Controls.Add(control);

            //    //Reset row
            //    locationX = 10;
            //    locationY = locationY + temp.Height + 10;

            //    control = new UCDashboardItem(reward, "REWARD", "");
            //    control.Location = new Point(locationX, locationY);
            //    locationX = locationX + temp.Width + 10;
            //    panelDailySale.Controls.Add(control);

            //    control = new UCDashboardItem(counpon, "COUPON", "");
            //    control.Location = new Point(locationX, locationY);
            //    locationX = locationX + temp.Width + 10;
            //    panelDailySale.Controls.Add(control);

            //    control = new UCDashboardItem(credit_tip, "CREDIT TIP", "");
            //    control.Location = new Point(locationX, locationY);
            //    locationX = locationX + temp.Width + 10;
            //    panelDailySale.Controls.Add(control);

            //    control = new UCDashboardItem(cash_tip, "CASH TIP", "");
            //    control.Location = new Point(locationX, locationY);
            //    locationX = locationX + temp.Width + 10;
            //    panelDailySale.Controls.Add(control);

            //    control = new UCDashboardItem(revenue, "REVENUE", "");
            //    control.Width = temp.Width * 2 + 10;
            //    control.Location = new Point(locationX, locationY);
            //    locationX = locationX + temp.Width + 10;
            //    panelDailySale.Controls.Add(control);

            //}
        }


        #region footer click

        private void lbFooterBuySupply_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void lbFooterCloseOutReport_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void lbFooterTipADJS_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void lbFooterApptBook_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void lbFooterBuyGiftCard_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void lbFooterBackOffice_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void lbFooterPayment_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void lbFooterCustomer_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void lbFooterNailsTech_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void lbFooterEmployees_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void lbFooterTurnSystem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnClose_ElementClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
        {
            this.Dispose();
        }

        #endregion


    }
}
