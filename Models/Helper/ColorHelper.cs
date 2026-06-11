using System.Drawing;
using System.Windows.Forms;

namespace NailsChekin.Models.Helper
{
    public class ColorHelper
    {
        public static Color DefaultBackgoundColor { get; set; } = ColorTranslator.FromHtml("#E1F0FB");

        public static Color DefaultBorderColor { get; set; } = ControlPaint.Dark(Color.FromArgb(120, 205, 236), 0.3f);   //Đậm hơn màu logo 30%
        public static Color DefaultForeColor { get; set; } = ControlPaint.Dark(Color.FromArgb(120, 205, 236), 0.3f);   //Đậm hơn màu logo 30%

        public static Color Customer_Waiting { get; set; } = DevExpress.LookAndFeel.DXSkinColors.FillColors.Warning; //System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
        public static Color Customer_InService { get; set; } = DevExpress.LookAndFeel.DXSkinColors.FillColors.Danger; //System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
        public static Color Customer_Payment { get; set; } = DevExpress.LookAndFeel.DXSkinColors.FillColors.Success; //System.Drawing.Color.Green;

        public static Color Nails_InService { get; set; } = DevExpress.LookAndFeel.DXSkinColors.FillColors.Danger;  //System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
        public static Color Nails_NextTurn { get; set; } = DevExpress.LookAndFeel.DXSkinColors.FillColors.Success;  //System.Drawing.Color.Green;

        public static Color Warning { get; set; } = DevExpress.LookAndFeel.DXSkinColors.FillColors.Warning; 
        public static Color Danger { get; set; } = DevExpress.LookAndFeel.DXSkinColors.FillColors.Danger; 
        public static Color Success { get; set; } = DevExpress.LookAndFeel.DXSkinColors.FillColors.Success;
        public static Color Question { get; set; } = DevExpress.LookAndFeel.DXSkinColors.FillColors.Question;
        public static Color Primary { get; set; } = DevExpress.LookAndFeel.DXSkinColors.FillColors.Primary;
    }
}
