using System.Reflection;
using System.Windows.Forms;


namespace NailsChekin.Models.Helper
{
    public static class SmoothUI
    {
        // Bật double-buffer + user paint cho bất kỳ Control nào
        public static void EnableDoubleBuffer(Control c)
        {
            // DoubleBuffered (protected) -> reflection
            typeof(Control).GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic)
                ?.SetValue(c, true, null);

            // SetStyle(...) (protected) -> reflection
            var setStyle = typeof(Control).GetMethod("SetStyle",
                BindingFlags.Instance | BindingFlags.NonPublic);

            setStyle?.Invoke(c, new object[] {
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.UserPaint |
            ControlStyles.ResizeRedraw, true });

            // UpdateStyles() (protected) -> reflection
            typeof(Control).GetMethod("UpdateStyles",
                BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(c, null);
        }
    }
}
