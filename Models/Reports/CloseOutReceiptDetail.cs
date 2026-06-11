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
    public partial class CloseOutReceiptDetail : DevExpress.XtraReports.UI.XtraReport
    {
        public CloseOutReceiptDetail()
        {
            InitializeComponent();
        }

        public CloseOutReceiptDetail(string responce)
        {
            InitializeComponent();

            if (responce.Trim().Length > 0)
            {
                this.DrawReceipt(responce);
            }
        }

        private void DrawReceipt(string responce)
        {
            try
            {
                JObject jData = JObject.Parse(responce);

                JObject storeInfo = JObject.Parse(jData["header"].ToString());
                JArray content = JArray.Parse(jData["content"].ToString());

                //storeInfo
                vendorName.Text = storeInfo["name"].ToString().ToUpper();
                vendorAddress.Text = storeInfo["address"].ToString();
                vendorFullAddress.Text = storeInfo["address2"].ToString();
                vendorPhone.Text = storeInfo["phone"].ToString();

                dateRange.Text = storeInfo["date"].ToString().Replace("Date: ", "");
                xrNailsName.Text = storeInfo["nails_tech"].ToString();
                string printSetting = storeInfo["printSetting"].ToString();

                double t_p = 0;
                double t_dr = 0;
                double t_s = 0;
                double t_r = 0;
                double t_ct = 0;
                double t_cct = 0;

                //List Detail
                int stt = 0;
                foreach (var item in content)
                {
                    string customer = item["customer"].ToString();
                    string service = item["service"].ToString();
                    double p = Math.Round(double.Parse(item["p"].ToString()), 2);
                    double dr = Math.Round(double.Parse(item["dr"].ToString()), 2);
                    double s = Math.Round(double.Parse(item["s"].ToString()), 2);
                    double r = Math.Round(double.Parse(item["r"].ToString()), 2);
                    double ct = Math.Round(double.Parse(item["ct"].ToString()), 2);
                    double cct = Math.Round(double.Parse(item["cct"].ToString()), 2);

                    if (stt == 0)
                    {
                        detailTable.Rows[0].Cells[0].Text = customer;
                        detailTable.Rows[0].Cells[1].Text = service;
                        detailTable.Rows[0].Cells[2].Text = "$" + dr;
                        detailTable.Rows[0].Cells[3].Text = "$" + s;
                        detailTable.Rows[0].Cells[4].Text = "$" + r;
                        detailTable.Rows[0].Cells[5].Text = "$" + ct;
                        detailTable.Rows[0].Cells[6].Text = "$" + cct;
                    }
                    else
                    {
                        detailTable.InsertRowBelow(null);
                        detailTable.Rows[stt].Cells[0].Text = customer;
                        detailTable.Rows[stt].Cells[1].Text = service;
                        detailTable.Rows[stt].Cells[2].Text = "$" + dr;
                        detailTable.Rows[stt].Cells[3].Text = "$" + s;
                        detailTable.Rows[stt].Cells[4].Text = "$" + r;
                        detailTable.Rows[stt].Cells[5].Text = "$" + ct;
                        detailTable.Rows[stt].Cells[6].Text = "$" + cct;
                    }

                    stt++;

                    t_p += p;
                    t_dr += dr;
                    t_s += s;
                    t_r += r;
                    t_ct += ct;
                    t_cct += cct;
                }

                xrT_DR.Text = "$" + t_dr;
                xrT_S.Text = "$" + t_s;
                xrT_R.Text = "$" + t_r;
                xrT_CT.Text = "$" + t_ct;
                xrT_CCT.Text = "$" + t_cct;

            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_Printer(ex.Message + Environment.NewLine + ex.StackTrace, "DrawReceipt CloseOutReceiptDetail Exception - responce: " + responce);
                CustomMessageBox.Show("Print Error: " + ex.Message);
            }
        }

    }
}
