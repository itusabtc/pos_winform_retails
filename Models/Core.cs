using NailsChekin.Models.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NailsChekin.Models
{
    class Core
    {
        public static void ClearAndDispose(Control myPanel)
        {
            try
            {
                if (myPanel == null)
                    return;

                System.Windows.Forms.Control.ControlCollection controls = myPanel.Controls;
                foreach (Control c in controls)
                    c.Dispose();

                myPanel.Controls.Clear();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex) { }
        }

        public static void ClearAndDisposeV2(Control myPanel)
        {
            try
            {
                if (myPanel == null)
                    return;

                foreach (Control c in myPanel.Controls)
                {
                    if (c is UserControl.UCCartItem)
                    {
                        ((UserControl.UCCartItem)c).MyDispose();
                        c.Dispose();
                    }
                    else
                    {
                        c.Dispose();
                    }
                }

                while (myPanel.Controls.Count > 0)
                {
                    Control ctrl = myPanel.Controls[0];
                    myPanel.Controls.RemoveAt(0);  // ❗ Remove trước

                    // Gọi Dispose nếu control hỗ trợ
                    if (ctrl is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }

                if (myPanel != null && myPanel.Controls.Count > 0)
                    myPanel.Controls.Clear();

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_Crash("Message: " + ex.Message + "\nStack Trace:" + ex.StackTrace, "ClearAndDisposeV2 Exception");
            }
        }

        public static void ClearMemory()
        {
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex) { }
        }


        public static LAYOUT_MODE GET_LAYOUT_MODE()
        {
            string layout_mode = ConfigLocalHelper.GetConfig("layout_mode", "STANDARD");
            if (layout_mode.ToUpper().Equals("MINI SCREEN"))
            {
                return LAYOUT_MODE.MINI_SCREEN;
            }
            else if (layout_mode.ToUpper().Equals("LARGE SCREEN"))
            {
                return LAYOUT_MODE.LARGE_SCREEN;
            }

            return LAYOUT_MODE.STANDARD;
        }

        public static POS_ROLE GET_POS_ROLE()
        {
            //string pos_role = ConfigLocalHelper.GetConfig("pos_role", "PRIMARY");
            //if (Constants.pos_role.ToUpper().Equals("SECONDARY"))  //đã load khi mở ứng dụng rồi
            //{
            //    return POS_ROLE.SECONDARY;
            //}

            return POS_ROLE.PRIMARY;
        }

        public static PAYMENT_MODE GET_PAYMENT_MODE()
        {
            string payment_mode = ConfigLocalHelper.GetConfig("payment_mode", "Using SAVE");
            if (payment_mode.ToUpper().Equals("USING SAVE"))
            {
                return PAYMENT_MODE.USING_SAVE;
            }

            return PAYMENT_MODE.USING_SERVICE_NOW;
        }

        public static SYSTEM_MENU GET_SYSTEM_MENU()
        {
            //if (Constants.system_menu.ToUpper().Equals("QUICK MENU"))
            //{
            //    return SYSTEM_MENU.QUICK_MENU;
            //}

            return SYSTEM_MENU.FULL_MENU;
        }

        public static bool USING_DUAL_PRICE()
        {
            bool rdDualPriceOn = ConfigLocalHelper.GetConfig("rdDualPriceOn", false);
            if (rdDualPriceOn)
            {
                return true;
            }

            return false;
        }

        public static float DUAL_PRICE_PERCENT()
        {
            bool rdDualPriceOn = ConfigLocalHelper.GetConfig("rdDualPriceOn", false);
            string dual_price_percent = ConfigLocalHelper.GetConfig("dual_price_percent", "0");

            if (!rdDualPriceOn)
            {
                return 0;
            }

            return string.IsNullOrEmpty(dual_price_percent) ? 0 : float.Parse(dual_price_percent);
        }

        public static bool USING_CASH_DISCOUNT()
        {
            bool rdCashDiscountOn = ConfigLocalHelper.GetConfig("rdCashDiscountOn", false);
            if (rdCashDiscountOn)
            {
                return true;
            }

            return false;
        }

        public static float CASH_DISCOUNT_PERCENT()
        {
            bool rdCashDiscountOn = ConfigLocalHelper.GetConfig("rdCashDiscountOn", false);
            string cash_discount_percent = ConfigLocalHelper.GetConfig("cash_discount_percent", "0");

            if (!rdCashDiscountOn)
            {
                return 0;
            }

            return string.IsNullOrEmpty(cash_discount_percent) ? 0 : float.Parse(cash_discount_percent);
        }

        public static bool USING_CASH_DISCOUNT_PRODUCT()
        {
            bool rdCashDiscountOn = ConfigLocalHelper.GetConfig("rdCashDiscountProductOn", false);
            if (rdCashDiscountOn)
            {
                return true;
            }

            return false;
        }

        public static float CASH_DISCOUNT_PRODUCT_PERCENT()
        {
            bool rdCashDiscountProductOn = ConfigLocalHelper.GetConfig("rdCashDiscountProductOn", false);
            string cash_discount_product_percent = ConfigLocalHelper.GetConfig("cash_discount_product_percent", "0");

            if (!rdCashDiscountProductOn)
            {
                return 0;
            }

            return string.IsNullOrEmpty(cash_discount_product_percent) ? 0 : float.Parse(cash_discount_product_percent);
        }

        public static double TAX_PERCENT()
        {
            double tax = 0;

            if (Constants.chkTaxOn && Constants.tax_percent.Trim().Length > 0)
            {
                tax = double.Parse(Constants.tax_percent.Trim());
            }

            return tax;
        }

        public static bool USING_REWARD_PERCENT()
        {
            //string reward_method = string.IsNullOrEmpty(Constants.reward_method) ? "ByCHECKOUT" : Constants.reward_method;
            //if (reward_method.Equals("ByCHECKIN"))
            //{
            //    return true;
            //}

            return false;
        }

        public static float REWARD_REDEEM_MAX_PERCENT()
        {
            //string reward_method = string.IsNullOrEmpty(Constants.reward_method) ? "ByCHECKOUT" : Constants.reward_method;
            //string percent_redeem_max = string.IsNullOrEmpty(Constants.percent_redeem_max) ? "100" : Constants.percent_redeem_max;
            //if (reward_method.Equals("ByCHECKIN"))
            //{
            //    if (string.IsNullOrEmpty(percent_redeem_max))
            //        return 0;

            //    return float.Parse(percent_redeem_max);
            //}

            return 0;
        }

        public static float REWARD_REDEEM_MAX_AMOUNT()
        {
            //string reward_method = string.IsNullOrEmpty(Constants.reward_method) ? "ByCHECKOUT" : Constants.reward_method;
            //string amount_redeem_max = string.IsNullOrEmpty(Constants.amount_redeem_max) ? "0" : Constants.amount_redeem_max;
            //if (reward_method.Equals("ByCHECKOUT"))
            //{
            //    if (string.IsNullOrEmpty(amount_redeem_max))
            //        return 0;

            //    return float.Parse(amount_redeem_max);
            //}

            return 0;
        }

        public static double DISCOUNT_PERCENT_OWNER()
        {
            //string bear_discount_type = Constants.bear_discount_type;
            //string staff_bear_discount_value = string.IsNullOrEmpty(Constants.staff_bear_discount_value) ? "0" : Constants.staff_bear_discount_value;

            //if (bear_discount_type.Equals("Staff"))
            //{
            //    return 0;
            //}
            //else if (bear_discount_type.Equals("Store"))
            //{
            //    return 100;
            //}

            //return Math.Round((100.0 - double.Parse(staff_bear_discount_value)), 2);

            return 0;
        }

        public static double REWARD_REDEEM_PERCENT_OWNER()
        {
            //string bear_reward_type = Constants.bear_reward_type;
            //string staff_bear_discount_value = string.IsNullOrEmpty(Constants.staff_bear_reward_value) ? "0" : Constants.staff_bear_reward_value;

            //if (bear_reward_type.Equals("Staff"))
            //{
            //    return 0;
            //}
            //else if (bear_reward_type.Equals("Store"))
            //{
            //    return 100;
            //}

            //return Math.Round((100.0 - double.Parse(staff_bear_discount_value)), 2);

            return 0;
        }

        public static bool ALLOW_ADD_QUICKMENU_TO_CART()
        {
            return ConfigLocalHelper.GetConfig("allow_quick_menu_to_cart", false);
        }

        public static int GET_CUSTOMER_LAYOUT_COLUMN()
        {
            //string quickmenu_option = Constants.quickmenu_option.ToUpper();
            //string fullmenu_option = Constants.fullmenu_option.ToUpper();

            //if (Core.GET_LAYOUT_MODE() == LAYOUT_MODE.MINI_SCREEN)
            //{
            //    if (fullmenu_option.ToUpper().Equals("SHOW"))
            //        return 1;

            //    //Không hiện full menu thì hiện 2 cột customer và fullscreen
            //    return 2;
            //}
            //else //Nhiều tiệm màn hình lớn nhưng muốn hiện thị 1 cột
            //{
            //    string layout_customer_cloumn = string.IsNullOrEmpty(Constants.layout_customer_cloumn) ? "2" : Constants.layout_customer_cloumn;
            //    if (!layout_customer_cloumn.Equals("2"))
            //        return 1;
            //}

            return 2;
        }

        public static int GET_NAILS_TECH_LAYOUT_COLUMN()
        {
            //string quickmenu_option = Constants.quickmenu_option.ToUpper();
            //string fullmenu_option = Constants.fullmenu_option.ToUpper();

            //if (Core.GET_LAYOUT_MODE() == LAYOUT_MODE.MINI_SCREEN)
            //{
            //    return 1;
            //}
            //else //Nhiều tiệm màn hình lớn nhưng muốn hiện thị 1 cột
            //{
            //    string layout_nails_tech_cloumn = string.IsNullOrEmpty(Constants.layout_nails_tech_cloumn) ? "2" : Constants.layout_nails_tech_cloumn;
            //    if (!layout_nails_tech_cloumn.Equals("2"))
            //        return 1;
            //}

            return 2;
        }

    }
}
