
using System.Drawing;
using System.Windows.Forms;

namespace NailsChekin.Models.Helper
{
    public class LayoutHelper
    {
        public static bool mini_screen = false;

        public static int number_coloumn_nails = 3;
        public static int number_coloumn_customer = 3;
        public static int number_coloumn_service = 3;

        public static int coloumn_nails_width = 180;
        public static int coloumn_customer_width = 222;

        public static int footer_button_height = 56;

        public static void SetLabelFontSize(Label lbl, float sizeInPoints)
        {
            lbl.Font = new Font(
                lbl.Font.FontFamily,
                sizeInPoints,
                lbl.Font.Style,
                GraphicsUnit.Point  // nên dùng Point (pt)
            );
        }


        // Kích thước thật:
        //double physical = MonitorSizeHelper.GetCurrentMonitorInches(this.Handle, ReportMode.Physical);
        // Lấy label phổ biến:
        //string label = MonitorSizeHelper.ClassifyLabel(perceived); // "15.6\"" 

        // Kích thước cảm nhận (đã Scale) – để 14" scale như 15.6" thì sẽ ra ~15.6":
        private static double perceived = 0;
        private static double physical = 0;

        public static void SetScale(double _perceived)
        {
            perceived = _perceived;
            if (Is_MiniScreen())
            {
                mini_screen = true;

                number_coloumn_nails = 2;
                number_coloumn_customer = 2;
                number_coloumn_service = 2;

                coloumn_nails_width = 210;
                coloumn_customer_width = 300;

                footer_button_height = 66;
            }
        }

        public static void SetScale(double _physical, double _perceived, bool _isScreenMini)
        {
            physical = _physical;
            perceived = _perceived;

            mini_screen = _isScreenMini;

            //if (Is_MiniScreen())
            if (_isScreenMini)
            {
                mini_screen = true;

                number_coloumn_nails = 2;
                number_coloumn_customer = 2;
                number_coloumn_service = 2;

                coloumn_nails_width = 210;
                coloumn_customer_width = 300;

                footer_button_height = 66;
            }
        }

        //private static bool Is_MiniScreen()
        //{
        //    if (Constants.system_mode == SYSTEM_MODE.REAL)
        //    {
        //        if (physical >= 21) //man hinh 21 inch tro len
        //            return false;
        //        if (physical < 21)
        //            return true;
        //    }
        //    else
        //    {
        //        if (perceived >= 17) //Test trên máy Laptop
        //            return true;
        //    }

        //    return false; //Normal screen 24 inch !!!
        //}

        // constants tùy bạn
        private const double MIN_INCH = 21.0;

        // Nếu physical đôi lúc sai do EDID/adapter, ta coi "hợp lệ" khi nằm trong khoảng thực tế.
        private static bool IsValidInch(double inch)
            => inch >= 7.0 && inch <= 80.0 && !double.IsNaN(inch) && !double.IsInfinity(inch);

        /// <summary>
        /// True nếu màn hình (monitor hiện tại / primary tuỳ bạn set physical-perceived từ đâu) >= 21 inch.
        /// Ưu tiên Physical; nếu Physical không hợp lệ thì dùng Perceived.
        /// </summary>
        private static bool Is21InchOrLarger()
        {
            double inch = IsValidInch(physical) ? physical
                       : IsValidInch(perceived) ? perceived
                       : 0.0; // không đo được

            return inch >= MIN_INCH;
        }

        /// <summary>
        /// Mini screen = nhỏ hơn 21 inch.
        /// </summary>
        private static bool Is_MiniScreen()
        {
            return !Is21InchOrLarger();
        }

    }
}
