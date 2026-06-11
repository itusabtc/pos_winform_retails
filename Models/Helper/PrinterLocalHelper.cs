using DevExpress.XtraReports.UI;
using NailsChekin.Models.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Printing;
using Newtonsoft.Json.Linq;

namespace NailsChekin.Models.Helper
{
    class PrinterLocalHelper
    {
        public static string PrintDirectTicket(string orderId, string responce)
        {
            try
            {
                if(string.IsNullOrEmpty(responce))
                    responce = MainReport.Receipt_PrinterData(orderId);

                string payment_info = JObject.Parse(responce)["payment_info"] == null ? "{}" : JObject.Parse(responce)["payment_info"].ToString();
                if (!payment_info.Equals("{}") && Utilitys.IsValidJson(payment_info) )
                {
                    return PrintDirectTicketWithSignature(orderId, responce);
                }

                TicketReceipt report = new TicketReceipt(orderId, responce);

                // KHỔ GIẤY: 80mm = ~315 (hundredths of an inch). Đổi theo khổ 58/80 bạn dùng.
                report.ReportUnit = DevExpress.XtraReports.UI.ReportUnit.HundredthsOfAnInch;
                report.PaperKind = System.Drawing.Printing.PaperKind.Custom;
                report.PageWidth = 315;                    // ~80mm
                //report.PageHeight = 3276; // dài lớn để giả lập continuous roll
                report.Margins = new System.Drawing.Printing.Margins(0, 0, 1, 5);
                report.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0);

                // In dạng CUỘN (không giới hạn chiều dài)
                report.RollPaper = true;                   // có từ các bản DevExpress mới

                //SinglePageHelper.GenerateSinglePageReport(report);  //KHÔNG SỬ DỤNG HÀM NÀY => MÁY IN HIỂU LÀ 1 KHUNG GIẤY NÊN BỊ GIỚI HẠN CHIỀU DÀI !!!

                ReportPrintTool printTool = new ReportPrintTool(report);
                printTool.PrintingSystem.ShowMarginsWarning = false;
                printTool.PrintingSystem.ShowPrintStatusDialog = false;

                if (!string.IsNullOrEmpty(Constants.printer_name))
                    printTool.Print(Constants.printer_name);
                else if (Constants.system_mode == SYSTEM_MODE.TEST)
                    printTool.ShowPreviewDialog();

                report.Dispose();
                printTool.Dispose();
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_Printer(ex.Message + Environment.NewLine + ex.StackTrace, "PrintDirectTicket Exception - responce: " + responce);
            }

            return "OK";
        }

        public static string PrintDirectTicketWithSignature(string orderId, string responce)
        {
            try
            {
                if (string.IsNullOrEmpty(responce))
                    responce = MainReport.Receipt_PrinterData(orderId);

                var report = new TicketReceiptWithSignatue(orderId, responce);

                // KHỔ GIẤY: 80mm = ~315 (hundredths of an inch). Đổi theo khổ 58/80 bạn dùng.
                report.ReportUnit = DevExpress.XtraReports.UI.ReportUnit.HundredthsOfAnInch;
                report.PaperKind = System.Drawing.Printing.PaperKind.Custom;
                report.PageWidth = 315;                    // ~80mm
                //report.PageHeight = 3276; // dài lớn để giả lập continuous roll
                report.Margins = new System.Drawing.Printing.Margins(0, 0, 1, 5);
                report.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0);

                // In dạng CUỘN (không giới hạn chiều dài)
                report.RollPaper = true;                   // có từ các bản DevExpress mới

                //SinglePageHelper.GenerateSinglePageReport(report);  //KHÔNG SỬ DỤNG HÀM NÀY => MÁY IN HIỂU LÀ 1 KHUNG GIẤY NÊN BỊ GIỚI HẠN CHIỀU DÀI !!!

                ReportPrintTool printTool = new ReportPrintTool(report);
                printTool.PrintingSystem.ShowMarginsWarning = false;
                printTool.PrintingSystem.ShowPrintStatusDialog = false;

                if (!string.IsNullOrEmpty(Constants.printer_name))
                    printTool.Print(Constants.printer_name);
                else if (Constants.system_mode == SYSTEM_MODE.TEST)
                    printTool.ShowPreviewDialog();

                report.Dispose();
                printTool.Dispose();
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_Printer(ex.Message + Environment.NewLine + ex.StackTrace, "PrintDirectTicketWithSignature Exception - responce: " + responce);
            }

            return "OK";
        }

        public static string PrintDirectTicketBK(string orderId, string responce)
        {
            try
            {
                TicketReceipt report = new TicketReceipt(orderId, responce);
                SinglePageHelper.GenerateSinglePageReport(report);

                ReportPrintTool printTool = new ReportPrintTool(report);
                printTool.PrintingSystem.ShowMarginsWarning = false;
                printTool.PrintingSystem.ShowPrintStatusDialog = false;

                //printTool.ShowPreviewDialog();

                if (!string.IsNullOrEmpty(Constants.printer_name))
                    printTool.Print(Constants.printer_name);

                report.Dispose();
                printTool.Dispose();
            }
            catch (Exception ex) { }

            return "OK";
        }

        public static string PrintDirectMonthlySaleReport(string fromDate, string toDate, bool is_review = false)
        {
            try
            {
                MonthlySaleReceipt report = new MonthlySaleReceipt(fromDate, toDate);
                SinglePageHelper.GenerateSinglePageReport(report);

                ReportPrintTool printTool = new ReportPrintTool(report);
                printTool.PrintingSystem.ShowMarginsWarning = false;
                printTool.PrintingSystem.ShowPrintStatusDialog = false;

                if (is_review)
                {
                    ResetReportPrintToolSettings(printTool);

                    printTool.ShowPreviewDialog();
                }
                else
                {
                    if (!string.IsNullOrEmpty(Constants.printer_name))
                        printTool.Print(Constants.printer_name);
                }

                report.Dispose();
                printTool.Dispose();
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_Printer(ex.Message + Environment.NewLine + ex.StackTrace, "PrintDirectMonthlySaleReport Exception - fromDate: " + fromDate + " - toDate: " + toDate);
            }

            return "OK";
        }

        public static void ResetReportPrintToolSettings(ReportPrintTool printTool)
        {
            try
            {
                // Create a new instance of the ReportPrintTool
                //ReportPrintTool printTool = new ReportPrintTool(report);

                // Reset printing and preview settings to default values
                printTool.PrintingSystem.StartPrint -= null; // Unsubscribe all event handlers
                printTool.PrintingSystem.ClearContent(); // Clear any cached content
                                                         //printTool.PrintingSystem.ExportOptions.Reset(); // Reset export options to default

                //printTool.PrintingSystem.PageSettings.Assign(new DevExpress.XtraPrinting.PageSettings()); // Reset page settings
                printTool.PrintingSystem.PageSettings.Assign(new PageSettings(), new Margins());
                printTool.PrintingSystem.ShowMarginsWarning = true; // Reset margin warning
                printTool.PrintingSystem.ShowPrintStatusDialog = true; // Reset print status dialog

                // Reset other customizable properties as needed
                printTool.AutoShowParametersPanel = true; // Show parameters panel by default
                printTool.PreviewRibbonForm.MdiParent = null; // Detach from any MDI parent
                printTool.PreviewRibbonForm.WindowState = System.Windows.Forms.FormWindowState.Normal; // Reset window state

                // (Optional) Dispose the print tool if no longer needed
                //printTool.Dispose();
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_Printer(ex.Message + Environment.NewLine + ex.StackTrace, "ResetReportPrintToolSettings Exception");
            }
        }

        public static string OpenCashDraweFromPrinter(string printerName)
        {
            try
            {
                //Mở Drawer gắn qua máy in, không có usb cắm trực tiếp
                PrinterHelper.SafeOpenDrawer(printerName);
                return "";
            }
            catch (Exception ex)
            {
                return "OpenCashDrawerDirect Exception: " + ex.Message;
            }
        }

    }

}
