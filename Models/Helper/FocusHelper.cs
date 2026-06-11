using System;
using System.Windows.Forms;

namespace NailsChekin.Models.Helper
{
    static class FocusHelper
    {
        public static void FocusWhenReady(Control target, int tries = 20, int delayMs = 15)
        {
            if (target == null) return;

            async void TryFocus()
            {
                // điều kiện đủ để focus
                bool ready =
                    target.IsHandleCreated &&
                    target.Visible &&
                    target.Enabled &&
                    target.CanSelect &&
                    target.FindForm() is Form form &&
                    form.Visible;

                if (ready)
                {
                    // đặt ActiveControl để chắc ăn, rồi Select để caret xuất hiện
                    var formFocus = target.FindForm();
                    if (formFocus != null) formFocus.ActiveControl = target;
                    target.Select();
                    return;
                }

                if (tries-- <= 0) return;

                // chờ 1 “nhịp” UI rồi thử lại
                await System.Threading.Tasks.Task.Delay(delayMs);
                target.BeginInvoke((Action)TryFocus);
            }

            // bắt đầu sau 1 vòng message loop
            target.BeginInvoke((Action)TryFocus);
        }
    }
}
