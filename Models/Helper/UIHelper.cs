using NailsChekin.Models.Implements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NailsChekin.Models.Helper
{
    class UIHelper
    {
        public static T GetParentForm<T>(Control control) where T : class
        {
            Control current = control.Parent;
            while (current != null)
            {
                T match = current as T;
                if (match != null)
                    return match;
                current = current.Parent;
            }
            return null;
        }

        public static void SafeUI(Control c, Action ui)
        {
            if (c.IsDisposed) return;
            if (c.InvokeRequired) c.BeginInvoke(ui);
            else ui();
        }

        //Paint flicker — render ra offscreen buffer trước rồi mới blit ra màn hình, tránh nhấp nháy khi vẽ lại
        public static void EnableDeepDoubleBuffer(Control root)
        {
            var prop = typeof(Control).GetProperty(
                "DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            void Recur(Control c)
            {
                try { prop?.SetValue(c, true, null); } catch { }
                foreach (Control ch in c.Controls) Recur(ch);
            }
            Recur(root);
        }

        public static async Task<bool> SortTABPaymentCart(KineticScrollPanel panel)
        {
            int numberElement = panel.Content.Controls.Count;
            int height = panel.Height;

            //Đẩy các phần tử hiện tại xuống dưới, để add phần tử mới thêm lên đầu ( khi quá dài sẽ ko bị che srcoll )
            int locationY = 5;
            int count = 0;

            for (int i = 0; i < numberElement; i++)
            {
                Control control = panel.Content.Controls[i];

                int current_x = control.Location.X;
                int current_y = control.Location.Y;

                control.Location = new Point(current_x, current_y + (43 + 5));
            }

            return true;
        }

    }
}
