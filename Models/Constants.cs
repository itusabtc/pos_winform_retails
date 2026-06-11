using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NailsChekin.Models
{
    class Constants
    {
        public static string version_id = "2";
        public static bool IS_DEMO_MODE_NOT_ANT_PAY = false;

        public static string pos_store_code = "RET32132";
        public static string pos_store_id = "123456";
        public static string pos_access_token = "";
        public static string pos_api_sceret_key = "";

        public static int pos_timezone = -5;  

        //Image URL
        public static string imgURL = "https://nailssolutions.blob.core.windows.net/images/";

        //Printer Config

        //public static string hostName = "api-retails.nailsbeautysupply.com";
        public static string hostName = "178.63.64.96";  //NO CLOUDFARE !!!

        public static string web_print_filePath = "C:\\POSLogs\\Printer\\";
        public static string web_print_acrobatURL = @"C:\Program Files (x86)\Adobe\Acrobat Reader DC\Reader\AcroRd32.exe";
        public static string web_print_domain = "pos.nailspaofamerica.com";


        //Settting ==> Load from file
        public static string fullmenu_column_default = "1";
        public static string payment_width = "356";

        public static int appt_column_default = 9;

        //Webhook
        //public static string webhookURL = "https://api-retails.nailsbeautysupply.com/signalR/";
        public static string webhookURL = "https://websocket.nailspaofamerica.com/signalr/hubs";

        public static string socketIOUrl = "http://178.63.64.96:8899";

        //Login backoffice
        public static string backoffice_url = "https://bo-retails.nailsbeautysupply.com/";
        public static string backoffice_tips_url = "https://tips.nailspaofamerica.com/";
        public static string buy_supply_url = "https://admin.skynailsupply.com/";

        //API
        public static string pos_sceret_key = "max@@view@@01235";

        //Credit Cart Payment ( Clover )
        public static bool chkSigOnScreen = true;
        public static bool chkSigOnPaper = false;

        public static bool chkTipsOn = true;
        public static bool chkTipsOff = false;
        public static string clover_connection_type = "Clover Connector USB";
        public static string clover_ip_address = "wss://192.168.1.2:12345/remote_pay";

        public static string credit_card_device = "CODE PAY";

        public static string codepay_connection_type = "WLAN/LAN";
        public static string codepay_ip_address = "ws://192.168.1.147:35779";
        public static string codepay_merchant_order_no = "";

        //CODEPAY
        public static string codepay_merchant_no = "";  //312300000969
        public static string codepay_store_no = "";  //4123000007
        public static string codepay_terminal_sn = "";  //WPYB002329000082
        public static string codepay_app_id = "";  //wzbb77f4a64a0885ca

        public static string checkin_option = "Full CheckIn";
        public static string inservice_setting = "Require Nails Tech";
        public static string appt_version_setting = "Web Version";
        public static string quickmenu_option = "Show";

        public static string using_system_credit = "ON";
        public static string printer_name = "";

        //TAX
        public static bool chkTaxOn = false;
        public static bool chkTaxOff = false;
        public static string tax_percent = "0";

        //PINCODE
        public static bool chkPincodeOn = true;
        public static bool chkPincodeOff = false;

        //SURCHARGE
        public static bool chkSurChargeOn = false;
        public static bool chkSurChargeOff = false;
        public static string surCharge_percent = "0";
        public static string surCharge_debit_percent = "0";
        public static string surCharge_unit = "%";
        public static string surCharge_minAmount = "0";

        //RECEIPT PRINT
        public static bool chkReceiptCusCheckin = false;
        public static bool chkShowPopupConfirmBill = false;
        public static string receiptFooter = "";

        public static string pairing_code = "";

        public static string pincode_checkout_cash = "0";
        public static string pincode_checkout_cashApp = "0";
        public static string pincode_checkout_member = "0";
        public static string pincode_checkout_prepaid = "0";

        //Default VALUE
        public static string using_system_credit_default = "ON";

        public static string clover_connection_type_defaul = "Network Pay Display"; //Default
        public static string codepay_connection_type_defaul = "WLAN/LAN";
        public static string codepay_ip_address_defaul = "ws://192.168.1.147:35779";
        public static string codepay_app_id_default = "wzbb77f4a64a0885ca";
        public static string codepay_device_defaul = "P5";
        public static string codepay_merchant_order_no_default = "";

        public static string web_print_filePath_default = "C:\\POSLogs\\Printer\\";
        public static string web_print_acrobatURL_default = @"C:\Program Files (x86)\Adobe\Acrobat Reader DC\Reader\AcroRd32.exe";
        public static string web_print_domain_default = "pos.nailspaofamerica.com";
        public static string turn_system_cloumn_show = "12";
        public static string checkin_option_default = "Full CheckIn";
        public static string inservice_setting_default = "Require Nails Tech";
        public static string appt_version_setting_default = "Web Version";
        public static string quickmenu_option_default = "Show";

        //Form Size Default ( Client Size )
        System.Drawing.Size formMain_ClientSize = new System.Drawing.Size(1300, 820);
        System.Drawing.Size formAppt_Client = new System.Drawing.Size(1300, 820);

        public static string ALL_CONFIG = "";

        public static SYSTEM_MODE system_mode = SYSTEM_MODE.REAL;

    }

}
