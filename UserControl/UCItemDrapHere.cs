using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using NailsChekin;

namespace NailsChekin.UserControl
{
    public partial class UCItemDrapHere : DevExpress.XtraEditors.XtraUserControl
    {
        public string id = "";
        public string name = "";
        public string price = "";

        public UCItemDrapHere()
        {
            InitializeComponent();
        }

        public UCItemDrapHere(string id, string name, string price)
        {
            InitializeComponent();

            this.id = id;
            this.name = name;
            this.price = price;

            lbName.Text = "Service: " + name;
        }

        private void svgImageBox1_Click(object sender, EventArgs e)
        {
            Control form_parent = (FormMain)this.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent;

            //((FormMain)form_parent).RemoveServiceDrapHere(this.id);

        }
    }
}
