using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using NailsChekin.Models.Helper;
using NailsChekin.UserControl;
using NailsChekin.MyControls;

namespace NailsChekin.Models.Reports
{
    public partial class MonthlySaleReceipt : DevExpress.XtraReports.UI.XtraReport
    {
        public string responce = "";

        public MonthlySaleReceipt()
        {
            InitializeComponent();

        }

        public MonthlySaleReceipt(string fromDate, string toDate)
        {
            InitializeComponent();

            this.responce = MainReport.MonthlySaleReport_PrinterData(fromDate, toDate);
            if (responce.Trim().Length > 0)
            {
                this.DrawReceipt(responce, fromDate, toDate);

                JObject jData = JObject.Parse(responce);
            }
        }

        private void DrawReceipt(string responce, string fromDate, string toDate)
        {
            //{ "tenderType":"CreditCard","transactions":0,"salesTotal":0.0,"refunds":0.0,"manualRefunds":0.0,"taxes":0.0,"amountCollected":0.0,"total":0.0}

            try
            {
                JObject jData = JObject.Parse(responce);

                double total_count = Math.Round(double.Parse(jData["transactions"].ToString()), 2);
                double gross_sales = Math.Round(double.Parse(jData["salesTotal"].ToString()), 2);
                double refunds = Math.Round(double.Parse(jData["refunds"].ToString()), 2);
                double taxes = Math.Round(double.Parse(jData["taxes"].ToString()), 2);
                double net_sales = Math.Round(double.Parse(jData["netSales"].ToString()), 2);

                xrTotal_Count.Text = total_count.ToString();
                xrGross_Sales.Text = "$" + gross_sales;
                xrRefunds.Text = "$" + refunds;
                xrTaxes.Text = "$" + taxes;
                xrNet_Sales.Text = "$" + net_sales;

                lbTranCycle.Text = "TRAN CYCLE: " + fromDate + " - " + toDate;
                lbPrintTime.Text = "PRINT TIME: " + DateTime.Now.ToString("Mmm dd, yyyy hh:mm tt");

            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_Printer(ex.Message + Environment.NewLine + ex.StackTrace, "DrawReceipt MonthlySaleReceipt Exception - responce: " + responce);
                CustomMessageBox.Show("Print Error: " + ex.Message);
            }
        }
    }

}
