using NailsChekin.Models;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NailsChekin.Popup
{
    public partial class FormMessageYesNo : Form
    {
        Control parentForm;

        string message = "";
        string redirect_url = "";
        string jData = "";

        public FormMessageYesNo()
        {
            InitializeComponent();
        }

        public FormMessageYesNo(Control parent, string message, string redirect_url, string jData = "")
        {
            InitializeComponent();

            this.Adjust_Screen();

            this.parentForm = parent;
            this.message = message;
            this.redirect_url = redirect_url;
            this.jData = jData;

            lbMessage.Text = message;
            lbMessage.MaximumSize = new Size(this.Size.Width - (lbMessage.Location.X * 2), 0);
            lbMessage.AutoSize = true;
        }

        public void Adjust_Screen()
        {
            //Bất đồng bộ
            this.BeginInvoke(new Action(() =>
            {
                btnClose.Location = new Point(this.Width - btnClose.Width - 10, btnClose.Location.Y);
            }));
        }

        private void btnYES_Click(object sender, EventArgs e)
        {
            if (this.redirect_url.Equals("ChangeStatusCreditDevice"))
            {
                ((FormMain)this.parentForm).ChangeStatusCreditDevice();
                this.Close();
            }
        }

        private void btnNO_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormMessageYesNo_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Xử lý không bị cảm giác giật do xài Dispose() ngay nếu đóng thẳng
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000); // 3 giây

                try
                {
                    Core.ClearMemory();

                    if (!this.IsDisposed)
                    {
                        this.Invoke((Action)(() => this.Dispose()));
                    }
                }
                catch { /* form đã dispose rồi thì thôi */ }
            });
        }

        
    }
}
