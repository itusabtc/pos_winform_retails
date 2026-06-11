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

namespace NailsChekin.UserControl
{
    public partial class UCDashboardItem : DevExpress.XtraEditors.XtraUserControl
    {
        public UCDashboardItem()
        {
            InitializeComponent();
        }

        public UCDashboardItem(string amount, string name, string color)
        {
            InitializeComponent();

            lbAmount.Text = amount;
            lbName.Text = name;

            //Random color
            Color[] colors = new Color[] { Color.Aqua, Color.Tomato, Color.YellowGreen, Color.Red, Color.Purple, Color.Pink };

            Random rd = new Random();
            int index = rd.Next(0, 5);

            this.Appearance.BackColor = colors[index];

        }

    }
}
