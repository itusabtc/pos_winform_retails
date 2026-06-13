using NailsChekin.Models;
using NailsChekin.Models.Payments;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NailsChekin.Popup
{
    public partial class FormCreditProcessing : Form
    {
        FormMain parentForm = null;
        bool is_void = false;
        public bool is_cancel_from_pos = false;

        public FormCreditProcessing()
        {
            InitializeComponent();
        }

        public FormCreditProcessing(FormMain parent, bool is_void = false)
        {
            InitializeComponent();

            this.parentForm = parent;
            this.is_void = is_void;
            this.is_cancel_from_pos = false;
        }

        private void FormCreditProcessing_Load(object sender, EventArgs e)
        {
            //Align Center
            int width = this.Width;

            int top = panelControls.Location.Y;
            int left = panelControls.Left;
            int right = panelControls.Right;

            int offset = (width - right + left) / 2;
            panelControls.Location = new Point(offset, top);

            lbMessage.Visible = false;
            lbMessage.Text = "";
        }

        private void svgImageBox1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.parentForm?.curent_order_local_payment_id))
            {
                //CANCEL P5 From POS
                if (!Constants.credit_card_device.Equals("CLOVER") && Constants.codepay_connection_type.Contains("WLAN") )
                    CreditCardLib.CODEPAY_WLAN_CANCEL_ORDER(this.parentForm, this.parentForm.curent_order_local_payment_id);
            }

            this.parentForm?.EnableDisableControl(true);
            this.Dispose();
        }

        public void ShowMessage(string message)
        {
            lbMessage.Visible = true;
            lbMessage.Text = message;
        }

        // 'new': cố ý che Form.Close() — đóng từ code POS phải enable lại control trên FormMain.
        // Lưu ý: đóng bằng Alt+F4 / nút X sẽ đi đường Form.Close() chuẩn, không qua hàm này.
        public new void Close()
        {
            this.parentForm?.EnableDisableControl(true);
            this.Dispose();
        }

    }
}
