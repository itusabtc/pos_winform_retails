using NailsChekin.Models;
using NailsChekin.Models.Helper;
using NailsChekin.UserControl;
using System;
using System.Windows.Forms;

namespace NailsChekin.Popup
{
    public partial class FormKeyBoardOnly : Form
    {
        Control parentForm;

        string redirect_url = "";
        string control_id = "";

        public FormKeyBoardOnly()
        {
            InitializeComponent();
        }

        public FormKeyBoardOnly(Control parent, string control_id, string control_text, string redirect_url)
        {
            InitializeComponent();

            this.parentForm = parent;
            this.control_id = control_id;
            this.redirect_url = redirect_url;

            this.KeyPreview = true;
            this.KeyPress += new KeyPressEventHandler(FormKeyBoardOnly_KeyPress);

            txtCurrentText.Text = control_text;
        }

        public FormKeyBoardOnly(Control parent, string control_id, string control_text, string redirect_url, string form_title)
        {
            InitializeComponent();

            lbTitile.Text = form_title.ToString();
            this.parentForm = parent;
            this.control_id = control_id;
            this.redirect_url = redirect_url;

            this.KeyPreview = true;
            this.KeyPress += new KeyPressEventHandler(FormKeyBoardOnly_KeyPress);

            txtCurrentText.Text = control_text;
        }

        private void FormKeyBoardOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                this.EnterNow();
            }
        }

        private void FormKeyBoardOnly_Shown(object sender, EventArgs e)
        {
            var kb = new MyControls.KeyBoardTemplateBar
            {
                Dock = DockStyle.Fill,
                AllowDecimal = true,
                MaxLength = 50,
                ButtonFontSize = 26f,
                ButtonCornerRadius = 12,
                TargetControl = txtCurrentText
            };
            kb.ButtonFontSize = 36.0f;
            panelCart_Control_Keyboard.Controls.Add(kb);
        }

        private void EnterNow()
        {
            //if (txtCurrentText.Text.Trim().Length <= 0)  //Clear input
            //{
            //    MessageBox.Show("Please check input enter");
            //    return;
            //}

            btnConfirm.Title = "Waiting...";
            btnConfirm.Enabled = false;

            if (this.redirect_url.Equals("local_change"))
            {
                //if (parentForm is UCCartItem)
                //{
                //    ((UCCartItem)this.parentForm).ConfirmKeyboardEnter(this.control_id, txtCurrentText.Text.Trim());
                //}
                //else if (parentForm is FormMain)
                //{
                //    ((FormMain)this.parentForm).ConfirmKeyboardEnter(this.control_id, txtCurrentText.Text.Trim());
                //}
                //else if (parentForm is UCTipsAdjustDetail)
                //{
                //    ((UCTipsAdjustDetail)this.parentForm).ConfirmKeyboardEnter(this.control_id, txtCurrentText.Text.Trim());
                //}
                //else if (parentForm is FormCustomerInfo)
                //{
                //    ((FormCustomerInfo)this.parentForm).ConfirmKeyboardEnter(this.control_id, txtCurrentText.Text.Trim());
                //}
                //else if (parentForm is FormGiftCardInfo)
                //{
                //    ((FormGiftCardInfo)this.parentForm).ConfirmKeyboardEnter(this.control_id, txtCurrentText.Text.Trim());
                //}
                //else if (parentForm is UCMenuItem)
                //{
                //    ((UCMenuItem)this.parentForm).ConfirmKeyboardEnter(this.control_id, txtCurrentText.Text.Trim());
                //}
                //else if (parentForm is TabCustomer)
                //{
                //    ((TabCustomer)this.parentForm).ConfirmKeyboardEnter(this.control_id, txtCurrentText.Text.Trim());
                //}
                //else if (parentForm is TabGiftCard)
                //{
                //    ((TabGiftCard)this.parentForm).ConfirmKeyboardEnter(this.control_id, txtCurrentText.Text.Trim());
                //}
                //else if (parentForm is TabAdjust)
                //{
                //    ((TabAdjust)this.parentForm).ConfirmKeyboardEnter(this.control_id, txtCurrentText.Text.Trim());
                //}
                //else if (parentForm is UCPaymentCartItem)
                //{
                //    ((UCPaymentCartItem)this.parentForm).ConfirmKeyboardEnter(this.control_id, txtCurrentText.Text.Trim());
                //}
                //else if (parentForm is FormDiscountRedeem)
                //{
                //    ((FormDiscountRedeem)this.parentForm).ConfirmKeyboardEnter(this.control_id, txtCurrentText.Text.Trim());
                //}
                
                this.Close();
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            this.EnterNow();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormKeyBoardOnly_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Xử lý không bị cảm giác giật do xài Dispose() ngay nếu đóng thẳng
            _ = System.Threading.Tasks.Task.Run(async () =>
            {
                await System.Threading.Tasks.Task.Delay(3000); // 3 giây
                try
                {
                    this.MyDispose();

                    if (!this.IsDisposed)
                    {
                        this.Invoke((Action)(() => this.Dispose()));
                    }
                }
                catch { /* form đã dispose rồi thì thôi */ }
            });           
        }

        public void MyDispose()
        {
            try
            {
                this.KeyPress -= FormKeyBoardOnly_KeyPress;

                Core.ClearMemory();
            }
            catch { }
        }

        private void txtCurrentText_TextChanged(object sender, EventArgs e)
        {
            string textData = txtCurrentText.Text.Trim();
            if (SwipeHelper.IsSwipe(textData)) 
            {
                txtCurrentText.Text = SwipeHelper.ExtractCardNumber(textData);
            }
        }

        
    }
}
