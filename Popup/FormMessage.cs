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
    public partial class FormMessage : Form
    {
        public FormMessage()
        {
            InitializeComponent();
        }

        public FormMessage(string message)
        {
            InitializeComponent();

            lbMessage.Text = message;
            lbMessage.MaximumSize = new Size(this.Size.Width - (lbMessage.Location.X * 2), 0);
            lbMessage.AutoSize = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
