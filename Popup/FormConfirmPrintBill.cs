using NailsChekin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NailsChekin.Popup
{
    public partial class FormConfirmPrintBill : Form
    {
        FormMain parentForm = null;
        public string ticketId = "";
        public string print_setting = "";

        public double change_amount = 0;

        public FormConfirmPrintBill()
        {
            InitializeComponent();
        }

        public FormConfirmPrintBill(FormMain parent, string ticketId, string change_amount, string print_setting)
        {
            InitializeComponent();

            this.parentForm = parent;

            change_amount = change_amount.Trim().Length <= 0 ? "0" : change_amount;

            this.ticketId = ticketId;
            this.print_setting = print_setting;
            this.change_amount = double.Parse(change_amount);

            lbChange.Text = Utilitys.get_format_number(this.change_amount.ToString());
        }

        public FormConfirmPrintBill(string ticketId, string change_amount, string print_setting)
        {
            InitializeComponent();

            change_amount = change_amount.Trim().Length <= 0 ? "0" : change_amount;

            this.ticketId = ticketId;
            this.print_setting = print_setting;
            this.change_amount = double.Parse(change_amount);

            lbChange.Text = Utilitys.get_format_number(this.change_amount.ToString());
        }

        private void btnNoPrint_Click(object sender, EventArgs e)
        {
            //Print only staff
            if (this.print_setting.Equals("staff_customer"))
            {
                PrintBill(this.print_setting, "1");
            }

            //nếu setting chỉ có bill khách hàng thì không in

            this.Close();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            //Chọn Print thì sẽ in file tho setting
            PrintBill(this.print_setting, "0");

            this.Close();
        }

        private void PrintBill(string print_setting, string print_only_staff)
        {
            if (this.ticketId.Trim().Length <= 0)
            {
                MessageBox.Show("Please check ticket #");
                return;
            }

            Models.Helper.PrinterLocalHelper.PrintDirectTicket(ticketId, "");
        }
        

    }
}
