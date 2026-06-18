using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NailsChekin.Models
{
    public class MainReport
    {
        public static string Receipt_PrinterData(string ticketId)
        {
            string responce = Utilitys.CALL_API("Order/" + ticketId + "/printData", "", "GET", true);
            return responce;
        }

        // Receipt TỔNG cho combine: gộp item + tiền của tất cả đơn trong nhóm, kèm cash/charge/change
        public static string CombineReceipt_PrinterData(string combineId, double cash, double charge, double change)
        {
            System.Globalization.CultureInfo inv = System.Globalization.CultureInfo.InvariantCulture;
            string endpoint = "Order/combinePrintData?combineId=" + Uri.EscapeDataString(combineId ?? "")
                + "&cash=" + cash.ToString(inv)
                + "&charge=" + charge.ToString(inv)
                + "&change=" + change.ToString(inv);
            return Utilitys.CALL_API(endpoint, "", "GET", true);
        }

        public static string CloseOutReport_PrinterData(string date, string staffId)
        {
            //string staffs = "";

            //POSService.MaxViewWebServiceSoapClient service = new POSService.MaxViewWebServiceSoapClient();
            ////string jsonStrResponse = service.AppCheckIn_CloseOutReport_DataPrint(NailsChekin.Models.Constants.pos_store_id, NailsChekin.Models.Constants.pos_sceret_key);

            ////date = "02-23-2023";
            //string jsonStrResponse = service.Report_DailySale_DataPrint(NailsChekin.Models.Constants.pos_store_id, staffId, date, date, "1", NailsChekin.Models.Constants.pos_sceret_key);
            //return jsonStrResponse;

            return "";
        }

        public static string MonthlySaleReport_PrinterData(string fromDate, string toDate, string tenderType = "CreditCard")
        {
            string responce = Utilitys.CALL_API("Report/salesByTenderReport?fromDate=" + fromDate + "&toDate=" + toDate + "&tenderType=" + tenderType, "", "GET", true);
            return responce;
        }

    }

}
