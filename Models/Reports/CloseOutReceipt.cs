using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using NailsChekin.UserControl;
using NailsChekin.Models.Helper;
using NailsChekin.MyControls;

namespace NailsChekin.Models.Reports
{
    public partial class CloseOutReceipt : DevExpress.XtraReports.UI.XtraReport
    {
        public string responce = "";

        public CloseOutReceipt()
        {
            InitializeComponent();
        }


        public CloseOutReceipt(string date, ref string bill_staff_detail)
        {
            InitializeComponent();

            this.responce = MainReport.CloseOutReport_PrinterData(date, "");
            if (responce.Trim().Length > 0)
            {
                this.DrawReceipt(responce);

                JObject jData = JObject.Parse(responce);
                bill_staff_detail = jData["bill_staff_detail"].ToString();  //Print template detail !!!!
            }
        }

        private void DrawReceipt(string responce)
        {
            try
            {
                JObject jData = JObject.Parse(responce);

                JObject storeInfo = JObject.Parse(jData["storeInfo"].ToString());
                JObject totalInfo = JObject.Parse(jData["totalInfo"].ToString());
                JObject customerInfo = JObject.Parse(jData["customerInfo"].ToString());

                JArray staffDetailInfo = JArray.Parse(jData["staffDetailInfo"].ToString());
                //JArray bill_staff_detail = JArray.Parse(jData["bill_staff_detail"].ToString());

                //storeInfo
                vendorName.Text = storeInfo["name"].ToString().ToUpper();
                vendorAddress.Text = storeInfo["address"].ToString();
                vendorFullAddress.Text = storeInfo["address2"].ToString();
                vendorPhone.Text = storeInfo["phone"].ToString();

                //string payroll_column_print = storeInfo["payroll_column_print"].ToString();
                //discountCaption.Text = payroll_column_print.Contains("DIS") ? "DIS." : "";
                //supplyDeductCaption.Text = payroll_column_print.Contains("S.D") ? "S.D." : "";

                dateRange.Text = storeInfo["date"].ToString();
               
                //Total Info
                double total_revenue = Math.Round(double.Parse(totalInfo["total_revenue"].ToString()), 2);
                double cash = Math.Round(double.Parse(totalInfo["cash"].ToString()), 2);
                double credit_debit = Math.Round(double.Parse(totalInfo["credit_debit"].ToString()), 2);
                double surcharge = Math.Round(double.Parse(totalInfo["surcharge"].ToString()), 2);
                double giftcard = Math.Round(double.Parse(totalInfo["giftcard"].ToString()), 2);
                double giftcard_sale = Math.Round(double.Parse(totalInfo["giftcard_sale"].ToString()), 2);
                double discount = Math.Round(double.Parse(totalInfo["discount"].ToString()), 2);
                double reward = Math.Round(double.Parse(totalInfo["reward"].ToString()), 2);
                double counpon = Math.Round(double.Parse(totalInfo["counpon"].ToString()), 2);
                double credit_tip = Math.Round(double.Parse(totalInfo["credit_tip"].ToString()), 2);
                double cash_tip = Math.Round(double.Parse(totalInfo["cash_tip"].ToString()), 2);
                double total_supply_deduction = Math.Round(double.Parse(totalInfo["total_supply_deduction"].ToString()), 2);

                //Revenue Detail
                string totalInfo_title = "";
                string totalInfo_value = "";
                string[] revennue_title = new string[] { "Cash", "Credit Debit", "Surcharge", "Giftcard", "Giftcard Sale", "Discount", "Reward", "Counpon", "Credit Tip", "Cash Tip", "Total Supply Deduction" };
                double[] revennue_detail = new double[] { cash, credit_debit - surcharge, surcharge, -1 * giftcard, giftcard_sale, -1 * discount, -1 * reward, -1 * counpon, credit_tip, cash_tip, total_supply_deduction };
                for (int i = 0; i < revennue_title.Length; i++)
                {
                    totalInfo_title += revennue_title[i];
                    totalInfo_value += "$" + revennue_detail[i];

                    if (i < revennue_title.Length - 1)
                    {
                        totalInfo_title += Environment.NewLine;
                        totalInfo_value += Environment.NewLine;
                    }
                }

                xrTotal_Revenue_Value.Text = "$" + total_revenue;
                xrTotal_Info.Text = totalInfo_title;
                xrTotal_Info_Value.Text = totalInfo_value;

                //Customer Detail
                string cusInfo_title = "";
                string cusInfo_value = "";

                int frequent_walk_ins = int.Parse(customerInfo["frequent_walk_ins"].ToString());
                int frequent_appoiment = int.Parse(customerInfo["frequent_appoiment"].ToString());
                int new_walk_ins = int.Parse(customerInfo["new_walk_ins-ins"].ToString());
                int new_appoiment = int.Parse(customerInfo["new_appoiment"].ToString());
                int total_cutomer = new_walk_ins + frequent_walk_ins + new_appoiment + frequent_appoiment;

                int[] cus_level = new int[] { 1, 2, 2, 1, 2, 2 };
                string[] cus_title = new string[] { "Frequent Customer", "Walk-ins", "Appoiment", "New Customer", "Walk-ins", "Appoiment" };
                double[] cus_detail = new double[] { frequent_walk_ins + frequent_appoiment, frequent_walk_ins, frequent_appoiment, new_walk_ins + new_appoiment, new_walk_ins, new_appoiment };

                for (int i = 0; i < cus_title.Length; i++)
                {
                    cusInfo_title += ( cus_level[i] == 2 ? "   " : "" ) + cus_title[i];
                    cusInfo_value += cus_detail[i];

                    if (i < cus_title.Length - 1)
                    {
                        cusInfo_title += Environment.NewLine;
                        cusInfo_value += Environment.NewLine;
                    }
                }

                xrTotal_Customer_Value.Text = total_cutomer.ToString();
                xrCustomer_Info.Text = cusInfo_title;
                xrCustomer_Info_Value.Text = cusInfo_value;

                //List Staff Detail
                int stt = 0;
                foreach (var item in staffDetailInfo)
                {
                    string staff = item["staff"].ToString();
                    double p = Math.Round(double.Parse(item["p"].ToString()), 2);
                    double d = Math.Round(double.Parse(item["d"].ToString()), 2);
                    double s = Math.Round(double.Parse(item["s"].ToString()), 2);
                    double r = Math.Round(double.Parse(item["r"].ToString()), 2);
                    double ct = Math.Round(double.Parse(item["ct"].ToString()), 2);
                    double cct = Math.Round(double.Parse(item["cct"].ToString()), 2);

                    if (stt == 0)
                    {
                        detailTable.Rows[0].Cells[0].Text = staff;
                        detailTable.Rows[0].Cells[1].Text = "$" + d;
                        detailTable.Rows[0].Cells[2].Text = "$" + s;
                        detailTable.Rows[0].Cells[3].Text = "$" + r;
                        detailTable.Rows[0].Cells[4].Text = "$" + cct;
                    }
                    else
                    {
                        detailTable.InsertRowBelow(null);
                        detailTable.Rows[stt].Cells[0].Text = staff;
                        detailTable.Rows[stt].Cells[1].Text = "$" + d;
                        detailTable.Rows[stt].Cells[2].Text = "$" + s;
                        detailTable.Rows[stt].Cells[3].Text = "$" + r;
                        detailTable.Rows[stt].Cells[4].Text = "$" + cct;
                    }

                    stt++;
                }
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_Printer(ex.Message + Environment.NewLine + ex.StackTrace, "DrawReceipt CloseOutReceipt Exception - responce: " + responce);
                CustomMessageBox.Show("Print Error: " + ex.Message);
            }
        }


    }
}
