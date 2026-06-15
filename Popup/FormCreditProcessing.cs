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
            try
            {
                FormMain main = this.parentForm;

                // merchant_order_no đang dùng để thu tiền nằm ở curent_order_payment_id
                // (KHÔNG phải curent_order_local_payment_id - field đó chỉ được set khi load lại
                //  sale item, nên lúc đang payment nó rỗng/giá trị cũ => trước đây X không hủy được).
                string merchant_order_no = main?.curent_order_payment_id;

                // Chỉ gửi lệnh hủy cho máy CodePay P5 (bỏ qua Clover).
                // T2 dùng cơ chế hủy riêng (qua socket / _pendingT2MerchantOrderNo) nên không xử lý ở đây.
                bool isCodePayP5 =
                    main != null
                    && !Constants.credit_card_device.Equals("CLOVER")
                    && CreditCardLib.GET_CODEPAY_DEVICE() != CODEPAY_DEVICE.T2
                    && !string.IsNullOrEmpty(merchant_order_no);

                if (isCodePayP5)
                {
                    //CANCEL P5 From POS - route theo đúng connection type đang cấu hình
                    P5_CONNECTTION_TYPE connType = P5Lib.Get_P5_ConecttionType_Setting();

                    // Gửi hủy ở background để X không bị treo nếu máy không phản hồi; form vẫn đóng ngay.
                    Task.Run(() =>
                    {
                        try
                        {
                            switch (connType)
                            {
                                case P5_CONNECTTION_TYPE.CLOUD:
                                    CreditCardLib.CODEPAY_CANCEL_ORDER(merchant_order_no);
                                    break;
                                case P5_CONNECTTION_TYPE.WLAN_LAN:
                                case P5_CONNECTTION_TYPE.PAIR_MODE:
                                    CreditCardLib.CODEPAY_WLAN_CANCEL_ORDER(main, merchant_order_no);
                                    break;
                                case P5_CONNECTTION_TYPE.USB:
                                    CreditCardLib.CODEPAY_USB_CANCEL_ORDER(main, merchant_order_no);
                                    break;
                            }
                        }
                        catch { /* hủy là best-effort, không chặn việc đóng form */ }
                    });
                }
            }
            catch { }

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
