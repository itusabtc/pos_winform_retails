using com.clover.remotepay.sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NailsChekin.Models.ListModel
{
    public class CloverResponce
    {
        public string clover_order_id = "";
        public string orderId = "";
        public string clover_amount = "0";
        public string clover_tip = "0";
        public string clover_msg = "";
        public string clover_status = "Success";
        public string payment_id = "";
        public string order_id = "";
        public string employee_id = "";

        public string clover_credit_surcharge = "0";
        public string clover_debit_surcharge = "0";
        public string clover_surcharge = "0";
        public string clover_dual_price = "0";

        public double repair_amount = 0;
        public double repair_tip_from_pos = 0;

        public CloverResponce()
        {
            clover_order_id = "";
            orderId = "12345";
            clover_amount = "0";
            clover_tip = "0";
            clover_msg = "";
            clover_status = "Success";
            payment_id = "";
            order_id = "";
            employee_id = "";
            clover_surcharge = "0";
        }

        public CloverResponce(SaleResponse response, double surcharge_amount)
        {
            var p = response?.Payment;
            clover_order_id = p?.externalPaymentId ?? "";
            orderId = "12345";
            clover_amount = (p?.amount)?.ToString() ?? "0";
            clover_tip = (p?.tipAmount)?.ToString() ?? "0";
            clover_msg = "";
            clover_status = "Success";
            payment_id = p?.externalPaymentId ?? "";
            order_id = p?.order?.id ?? "";
            employee_id = p?.employee?.id ?? "";

            //SURCHARGE cần tính lại theo đúng số tiền thu được từ clover trả về ( có trường hợp thẻ thiếu tiền ... hoặc đã thanh toán 1 phần )
            clover_surcharge = Utilitys.getSurcharge_From_Paided(double.Parse(clover_amount)).ToString();
        }

        public CloverResponce(SaleResponse response, double surcharge_credit_amount, double surcharge_debit_amount, double dual_price_amount)
        {
            var p = response?.Payment;
            clover_order_id = p?.externalPaymentId ?? "";
            orderId = "12345";
            clover_amount = (p?.amount)?.ToString() ?? "0";
            clover_tip = (p?.tipAmount)?.ToString() ?? "0";
            clover_msg = "";
            clover_status = "Success";
            payment_id = p?.externalPaymentId ?? "";
            order_id = p?.order?.id ?? "";
            employee_id = p?.employee?.id ?? "";

            //SURCHARGE cần tính lại theo đúng số tiền thu được từ clover trả về ( có trường hợp thẻ thiếu tiền ... hoặc đã thanh toán 1 phần )
            //if (dual_price_amount > 0)
            //    clover_dual_price = Utilitys.getDualPrice_From_Paided(double.Parse(clover_amount)).ToString();

            if (surcharge_credit_amount > 0)
                clover_credit_surcharge = Utilitys.getSurcharge_From_Paided(double.Parse(clover_amount)).ToString();

            if(surcharge_debit_amount > 0)
                clover_debit_surcharge = Utilitys.getSurcharge_Debit_From_Paided(double.Parse(clover_amount)).ToString();

            clover_surcharge = Math.Round((double.Parse(clover_credit_surcharge) + double.Parse(clover_debit_surcharge)), 2).ToString();

        }

        public CloverResponce(string order_number, string order_amount, string order_tip, string trans_no, double surcharge_credit_amount, double surcharge_debit_amount)
        {
            clover_order_id = order_number;
            orderId = "12345";
            clover_amount = order_amount;
            clover_tip = string.IsNullOrEmpty(order_tip) ? "0" : order_tip;
            clover_msg = "";
            clover_status = "Success";
            payment_id = trans_no;
            order_id = order_number;
            employee_id = "";

            //SURCHARGE cần tính lại theo đúng số tiền thu được từ clover trả về ( có trường hợp thẻ thiếu tiền ... hoặc đã thanh toán 1 phần )
            if (surcharge_credit_amount > 0)
                clover_credit_surcharge = Utilitys.getSurcharge_From_Paided(double.Parse(clover_amount)).ToString();

            if (surcharge_debit_amount > 0)
                clover_debit_surcharge = Utilitys.getSurcharge_Debit_From_Paided(double.Parse(clover_amount)).ToString();

            //Surcharge không tinh Tip

            clover_surcharge = Math.Round((double.Parse(clover_credit_surcharge) + double.Parse(clover_debit_surcharge)), 2).ToString();

        }

        public CloverResponce(string order_number, string order_amount, string order_tip, string trans_no, double surcharge_credit_amount, double surcharge_debit_amount, double dual_price_amount)
        {
            clover_order_id = order_number;
            orderId = "12345";
            clover_amount = order_amount;
            clover_tip = string.IsNullOrEmpty(order_tip) ? "0" : order_tip;
            clover_msg = "";
            clover_status = "Success";
            payment_id = trans_no;
            order_id = order_number;
            employee_id = "";

            //SURCHARGE cần tính lại theo đúng số tiền thu được từ clover trả về ( có trường hợp thẻ thiếu tiền ... hoặc đã thanh toán 1 phần )
            //if (dual_price_amount > 0)
            //    clover_dual_price = Utilitys.getDualPrice_From_Paided(double.Parse(clover_amount)).ToString();

            if (surcharge_credit_amount > 0)
                clover_credit_surcharge = Utilitys.getSurcharge_From_Paided(double.Parse(clover_amount)).ToString();

            if (surcharge_debit_amount > 0)
                clover_debit_surcharge = Utilitys.getSurcharge_Debit_From_Paided(double.Parse(clover_amount)).ToString();

            //Surcharge không tinh Tip

            clover_surcharge = Math.Round((double.Parse(clover_credit_surcharge) + double.Parse(clover_debit_surcharge) + double.Parse(clover_dual_price)), 2).ToString();

        }

        public void SetRepairMode(double amount, double tip_amount)
        {
            this.repair_amount = amount;
            this.repair_tip_from_pos = tip_amount;
        }

    }


}
