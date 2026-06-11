using System.Collections.Generic;

namespace NailsChekin.Models.ListModel
{
    public class PaymentModel
    {
        public string type = "";
        public double amount = 0;
        public double cash_discount = 0;
        public double cash_discount_product = 0;
        public double repair_amount = 0;
        public double cash_received = 0;
        public double amt_due = 0;
        public string pincode = "";

        //Print info
        public string print_type = "";  //Receipt | Sms | Email
        public string trans_no = "";
        public string pay_method_id = "";  //VISA/MASTER CARD
        public string card_network_type = "";  //CREDIT = 2, DEBIT = xx
        public string card_no = "";
        public string auth_code = "";
        public string signature_url = "";
        public string signature_base64 = "";
        public string tip_amount = "";

        public List<CloverResponce> responce = new List<CloverResponce>();

        public PaymentModel() { }

        public PaymentModel(string type, double amount)
        {
            this.type = type;
            this.amount = amount;
        }

        public PaymentModel(string type, double amount, double cash_discount)
        {
            this.type = type;
            this.amount = amount;
            this.cash_discount = cash_discount;
        }

        public PaymentModel(string type, double pay_amount, double cash_discount, double cash_discount_product, double cash_received, double amt_due, double repair_amount, string pincode)
        {
            this.type = type;
            this.amount = pay_amount;
            this.cash_discount = cash_discount;  //Total Cash Discount Service + Product
            this.cash_discount_product = cash_discount_product;
            this.cash_received = cash_received;
            this.amt_due = amt_due;
            this.repair_amount = repair_amount;
            this.pincode = pincode;
        }

        public void SetCreditPrintInfo(string print_type, string trans_no, string pay_method_id, string card_network_type, string card_no, string auth_code, string signature_base64, string tip_amount)
        {
            this.print_type = print_type;
            this.trans_no = trans_no;
            this.pay_method_id = pay_method_id;
            this.card_network_type = card_network_type;
            this.card_no = card_no;
            this.auth_code = auth_code;
            this.signature_base64 = signature_base64;
            this.tip_amount = string.IsNullOrEmpty(tip_amount) ? "0" : tip_amount;
        }

        public string GetCreditPrintInfo()
        {
            string jPrint = "{";
            jPrint += "'print_type':'" + print_type + "',";
            jPrint += "'trans_no':'" + trans_no + "',";
            jPrint += "'pay_method_id':'" + pay_method_id + "',";
            jPrint += "'card_network_type':'" + card_network_type + "',";
            jPrint += "'card_no':'" + card_no + "',";
            jPrint += "'auth_code':'" + auth_code + "',";
            jPrint += "'signature_base64':'" + signature_base64 + "',";
            jPrint += "'amount':'" + amount + "',";
            jPrint += "'tip_amount':'" + tip_amount + "' ";
            jPrint += "}";

            return jPrint;
        }

        public void SetPincode(string pincode)
        {
            this.pincode = pincode;
        }

    }
}
