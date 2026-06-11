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
    public partial class UCCustomerInfoLine : DevExpress.XtraEditors.XtraUserControl
    {
        public UCCustomerInfoLine()
        {
            InitializeComponent();
        }

        public UCCustomerInfoLine(string date, string itemName, string price, string qty, string amount)
        {
            InitializeComponent();

            lbDate.Text = date;
            lbService.Text = itemName;
            lbPrice.Text = price;
            lbQty.Text = qty;
            lbAmount.Text = amount;

        }

    }
}
