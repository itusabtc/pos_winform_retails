using NailsChekin.Models;
using NailsChekin.MyControls;
using NailsChekin.UserControl;
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
    public partial class FormComment : Form
    {
        Control parentForm;

        string redirect_url = "";
        string control_id = "";

        string amount = "0";
        
        public FormComment()
        {
            InitializeComponent();
        }

        public FormComment(Control parent, string control_id, string title, string content, string amount, string redirect_url)
        {
            InitializeComponent();

            this.amount = amount;
            this.parentForm = parent;
            this.control_id = control_id;
            this.redirect_url = redirect_url;

            lbTitle.Text = title.ToUpper();
            txtCurrentText.Text = content;

            txtAmount.Text = amount;
            txtAmount.Enabled = false;            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            this.ConfirmNow();
        }

        private void ConfirmNow()
        {
            if (txtCurrentText.Text.Trim().Length <= 0)  //Clear input
            {
                CustomMessageBox.Show("Please enter note content !!!");
                return;
            }

            btnConfirm.Text = "Waiting...";
            btnConfirm.Enabled = false;

            if (parentForm is UCSaleItem)
            {
                if (this.redirect_url.Equals("delete_order"))
                {
                    ((UCSaleItem)parentForm).DeleteOrder(txtCurrentText.Text.Trim());
                }
            }

            this.Dispose();

        }

    }
}
