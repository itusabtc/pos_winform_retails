using NailsChekin.UserControl;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NailsChekin.Models.Helper
{
    class ConfigLocalHelper
    {
        public static void SaveConfigTxt()
        {
            //Save To File
            string configs = "";

            configs += "credit_card_device: " + Constants.credit_card_device.ToString() + "\n";

            configs += "fullmenu_column_default: " + Constants.fullmenu_column_default + "\n";
            configs += "appt_column_default: " + Constants.appt_column_default.ToString() + "\n";

            configs += "chkTipsOn: " + Constants.chkTipsOn.ToString() + "\n";
            configs += "chkTipsOff: " + Constants.chkTipsOff.ToString() + "\n";
            configs += "chkSigOnPaper: " + Constants.chkSigOnPaper.ToString() + "\n";
            configs += "chkSigOnScreen: " + Constants.chkSigOnScreen.ToString() + "\n";
            configs += "clover_connection_type: " + Constants.clover_connection_type.ToString() + "\n";

            configs += "codepay_connection_type: " + Constants.codepay_connection_type.ToString() + "\n";
            configs += "codepay_ip_address: " + Constants.codepay_ip_address.ToString() + "\n";
            configs += "codepay_app_id: " + Constants.codepay_app_id.ToString() + "\n";
            configs += "codepay_merchant_order_no: " + Constants.codepay_merchant_order_no.ToString() + "\n";

            //Web Print
            configs += "web_print_acrobatURL: " + Constants.web_print_acrobatURL + "\n";
            configs += "web_print_domain: " + Constants.web_print_domain.ToString() + "\n";
            configs += "web_print_filePath: " + Constants.web_print_filePath + "\n";

            configs += "turn_system_cloumn_show: " + Constants.turn_system_cloumn_show + "\n";
            configs += "checkin_option: " + Constants.checkin_option + "\n";
            configs += "inservice_setting: " + Constants.inservice_setting + "\n";
            configs += "appt_version_setting: " + Constants.appt_version_setting + "\n";
            configs += "quickmenu_option: " + Constants.quickmenu_option + "\n";

            configs += "using_system_credit: " + Constants.using_system_credit.ToString() + "\n";

            //TAX
            configs += "chkTaxOn: " + Constants.chkTaxOn.ToString() + "\n";
            configs += "chkTaxOff: " + Constants.chkTaxOff.ToString() + "\n";
            configs += "tax_percent: " + Constants.tax_percent.ToString() + "\n";

            //SURCHANGE
            configs += "chkSurChargeOn: " + Constants.chkSurChargeOn.ToString() + "\n";
            configs += "chkSurChargeOff: " + Constants.chkSurChargeOff.ToString() + "\n";
            configs += "surCharge_percent: " + Constants.surCharge_percent.ToString() + "\n";
            configs += "surCharge_unit: " + Constants.surCharge_unit.ToString() + "\n";

            //RECEIPT PRINT
            configs += "chkReceiptCusCheckin: " + Constants.chkReceiptCusCheckin.ToString() + "\n";
            configs += "chkShowPopupConfirmBill: " + Constants.chkShowPopupConfirmBill.ToString() + "\n";
            configs += "receiptFooter: " + Constants.receiptFooter + "\n";

            //PAIRING CODE
            configs += "pairing_code: " + Constants.pairing_code + "\n";

            //PRINTER NAME
            configs += "printer_name: " + Constants.printer_name.ToString() + "\n";

            //SURCHANGE
            configs += "chkSurChargeOn: " + Constants.chkSurChargeOn.ToString() + "\n";
            configs += "chkSurChargeOff: " + Constants.chkSurChargeOff.ToString() + "\n";
            configs += "surCharge_percent: " + Constants.surCharge_percent.ToString() + "\n";
            configs += "surCharge_minAmount: " + Constants.surCharge_minAmount.ToString() + "\n";
            configs += "surCharge_unit: " + Constants.surCharge_unit.ToString() + "\n";

            try
            {
                Constants.ALL_CONFIG = configs;

                string forderLog = "C:\\POSLogs\\Retails\\Config\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                string logWriteUrl = forderLog + "config.txt";
                using (StreamWriter sw = new StreamWriter(logWriteUrl, false))
                {
                    sw.WriteLine(configs);
                    sw.Close();
                }
            }
            catch (Exception ex) { }
        }

        public static void SaveAllStoreConfig(string configs)
        {
            try
            {
                string forderLog = "C:\\POSLogs\\Retails\\Config\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                string logWriteUrl = forderLog + "store_config.txt";

                StreamWriter sw = new StreamWriter(logWriteUrl, false);
                sw.WriteLine(configs);
                sw.Close();

            }
            catch (Exception ex) { }
        }

        public static string GetConfig(string key, string default_value)
        {
            try
            {
                //For test 2025-07-18
                if (Constants.pos_store_id.Equals("500276") && key.Equals("clover_connection_type"))
                    return "Network Pay Display";

                string value = ReadConfig(key);

                if (value.Trim().Length <= 0)
                    value = default_value;

                return value;
            }
            catch (Exception ex)
            {
                MessageBox.Show("GetConfig Exception: " + ex.Message);
                LogHelper.SaveLOG_Crash(ex.Message + "\nStackTrace: " + ex.StackTrace, "GetConfig Exception");
                return "";
            }
        }

        public static bool GetConfig(string key, bool default_value)
        {
            try
            {
                string value = ReadConfig(key);
                if (value.ToUpper().Equals("ON") || value.ToUpper().Equals("TRUE"))
                    return true;
                else if (value.ToUpper().Equals("OFF") || value.ToUpper().Equals("FALSE"))
                    return false;

                if (value.Trim().Length <= 0)
                    return default_value;
            }
            catch (Exception ex)
            {
                MessageBox.Show("GetConfig2 Exception: " + ex.Message);
                LogHelper.SaveLOG_Crash(ex.Message + "\nStackTrace: " + ex.StackTrace, "GetConfig2 Exception");
            }

            return true;
        }

        public static string GetStoreConfig(string key, string default_value)
        {
            string value = ReadStoreConfig(key);

            if (value.Trim().Length <= 0)
                value = default_value;

            return value;
        }

        public static void LoadAllConfigToSystem()
        {
            Constants.ALL_CONFIG = "";
            try
            {
                string logReadUrl = "C:\\POSLogs\\Retails\\Config\\config.txt";
                using (StreamReader sr = File.OpenText(logReadUrl))
                {
                    string content = sr.ReadToEnd();
                    Constants.ALL_CONFIG = content;

                    MapConfigToLocalSystem(false);
                }
            }
            catch { }
        }

        public static void MapConfigToLocalSystem(bool load_from_api = false)
        {
            Constants.credit_card_device = ConfigLocalHelper.GetConfig("credit_card_device", "CLOVER");

            Constants.clover_connection_type = ConfigLocalHelper.GetConfig("clover_connection_type", Constants.clover_connection_type_defaul);
            Constants.chkTipsOn = ConfigLocalHelper.GetConfig("chkTipsOn", true);
            Constants.chkTipsOff = ConfigLocalHelper.GetConfig("chkTipsOff", false);
            Constants.chkSigOnScreen = ConfigLocalHelper.GetConfig("chkSigOnScreen", false);
            Constants.chkSigOnPaper = ConfigLocalHelper.GetConfig("chkSigOnPaper", true);
            Constants.codepay_connection_type = ConfigLocalHelper.GetConfig("codepay_connection_type", Constants.codepay_connection_type_defaul);
            Constants.codepay_ip_address = ConfigLocalHelper.GetConfig("codepay_ip_address", Constants.codepay_ip_address_defaul);
            Constants.codepay_app_id = ConfigLocalHelper.GetConfig("codepay_app_id", Constants.codepay_app_id_default);

            Constants.checkin_option = ConfigLocalHelper.GetConfig("checkin_option", Constants.checkin_option_default);
            Constants.inservice_setting = ConfigLocalHelper.GetConfig("inservice_setting", Constants.inservice_setting_default);
            Constants.appt_version_setting = ConfigLocalHelper.GetConfig("appt_version_setting", Constants.appt_version_setting_default);

            Constants.quickmenu_option = ConfigLocalHelper.GetConfig("quickmenu_option", Constants.quickmenu_option_default);
            
            Constants.tax_percent = ConfigLocalHelper.GetConfig("tax_percent", "0");
            Constants.chkTaxOn = ConfigLocalHelper.GetConfig("chkTaxOn", false);
            Constants.chkTaxOff = ConfigLocalHelper.GetConfig("chkTaxOff", false);

            Constants.turn_system_cloumn_show = ConfigLocalHelper.GetConfig("turn_system_cloumn_show", "15");
            Constants.web_print_acrobatURL = ConfigLocalHelper.GetConfig("web_print_acrobatURL", Constants.web_print_acrobatURL_default);
            Constants.web_print_domain = ConfigLocalHelper.GetConfig("web_print_domain", Constants.web_print_domain_default);
            Constants.web_print_filePath = ConfigLocalHelper.GetConfig("web_print_filePath", Constants.web_print_filePath_default);
            Constants.fullmenu_column_default = ConfigLocalHelper.GetConfig("fullmenu_column_default", "4");
            Constants.appt_column_default = int.Parse(ConfigLocalHelper.GetConfig("appt_column_default", "5"));

            //Printer POS device
            Constants.printer_name = ConfigLocalHelper.GetConfig("printer_name", "");

            Constants.pos_store_code = ConfigLocalHelper.GetStoreConfig("store_code", "");
            Constants.pos_store_id = ConfigLocalHelper.GetStoreConfig("store_id", "");
            Constants.pos_timezone = int.Parse(ConfigLocalHelper.GetStoreConfig("timezone", "0"));

            if (load_from_api)
            {
                string responce = Utilitys.CALL_API("Store/getStoreSetting", "", "GET", true);
                if (!responce.StartsWith("Error"))
                {
                    string credit_setting_on = JObject.Parse(responce)["credit_setting_on"].ToString();

                    string tax_setting_on = JObject.Parse(responce)["tax_setting_on"].ToString();
                    string tax_percent = JObject.Parse(responce)["tax_percent"].ToString();

                    string receipt_footer = JObject.Parse(responce)["receipt_footer"].ToString();

                    Constants.using_system_credit = credit_setting_on.Equals("1") ? "ON" : "OFF";

                    Constants.tax_percent = tax_percent;
                    Constants.chkTaxOn = tax_setting_on.Equals("1") ? true : false;
                    Constants.chkTaxOff = tax_setting_on.Equals("1") ? false : true;

                    Constants.receiptFooter = receipt_footer;
                }
            }

        }

        public static void CreateForderConfig()
        {
            try
            {
                string forderLog = "C:\\POSLogs\\Config\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                forderLog = "C:\\POSLogs\\Clover\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                forderLog = "C:\\POSLogs\\CodePayW\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                forderLog = "C:\\POSLogs\\Payments\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                forderLog = "C:\\POSLogs\\CallerId\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                forderLog = "C:\\POSLogs\\Crashs\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                string forderPrinter = Constants.web_print_filePath;
                if (!Directory.Exists(forderPrinter))
                    Directory.CreateDirectory(forderPrinter);

            }
            catch (Exception ex) { }
        }

        public static void SaveAllConfig(string configs)
        {
            try
            {
                Constants.ALL_CONFIG = configs;

                string forderLog = "C:\\POSLogs\\Retails\\Config\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                string logWriteUrl = forderLog + "config.txt";
                using (StreamWriter sw = new StreamWriter(logWriteUrl, false))
                {
                    sw.WriteLine(configs);
                    sw.Close();
                }
            }
            catch (Exception ex) { }
        }

        public static string ReadConfig(string key)
        {
            try
            {
                string value = "";
                using (StringReader reader = new StringReader(Constants.ALL_CONFIG))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith(key))
                        {
                            value = line.Replace(key + ": ", "");
                            break;
                        }
                    }
                }

                return value;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string ReadAllConfig()
        {
            try
            {
                string value = "";

                string forderLog = "C:\\POSLogs\\Retails\\Config\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                string logReadUrl = forderLog + "config.txt";

                using (StreamReader sr = File.OpenText(logReadUrl))
                {
                    string s = sr.ReadToEnd();
                    sr.Close();
                }
                return value;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string ReadStoreConfig(string key)
        {
            try
            {
                string value = "";

                string forderLog = "C:\\POSLogs\\Retails\\Config\\";
                if (!Directory.Exists(forderLog))
                    Directory.CreateDirectory(forderLog);

                string logReadUrl = forderLog + "store_config.txt";

                using (StreamReader sr = File.OpenText(logReadUrl))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        if (s.StartsWith(key))
                        {
                            value = s.Replace(key + ": ", "");
                            break;
                        }
                    }
                    sr.Close();
                }
                return value;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string ReadFileText(string filename)
        {
            try
            {
                string value = "";

                string logReadUrl = filename;
                using (StreamReader sr = File.OpenText(logReadUrl))
                {
                    value = sr.ReadToEnd();
                    sr.Close();
                }
                return value;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

    }
}
