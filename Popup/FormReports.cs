using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using NailsChekin.Models;
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
    public partial class FormReports : Form
    {
        FormMain parentForm = null;

        public FormReports()
        {
            InitializeComponent();
        }

        public FormReports(FormMain parent)
        {
            InitializeComponent();

            this.parentForm = parent;
        }

        private void FormReports_Load(object sender, EventArgs e)
        {
            DXPopupMenu popupType = new DXPopupMenu();
            popupType.Items.Add(new DXMenuItem() { Caption = "SALES REPORT" });
            ddlReportType.DropDownControl = popupType;
            foreach (DXMenuItem item in popupType.Items)
                item.Click += item_type_Click;

            DXPopupMenu popupSea = new DXPopupMenu();
            popupType.Items.Add(new DXMenuItem() { Caption = "SALES REPORT" });
            ddlReportType.DropDownControl = popupType;
            foreach (DXMenuItem item in popupType.Items)
                item.Click += item_type_Click;

            txtFromDate.Text = DateTime.Now.ToString("MM/01/yyyy");
            txtToDate.Text = DateTime.Now.ToString("MM/dd/yyyy");

            ddlReportType.Text = "SALES REPORT";
        }

        private void item_type_Click(object sender, EventArgs e)
        {
            ddlReportType.Text = ((DXMenuItem)sender).Caption;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtFromDate.Text.Trim()) || string.IsNullOrEmpty(txtToDate.Text.Trim()))
            {
                MessageBox.Show("Please check time report");
                return;
            }

            Models.Helper.PrinterLocalHelper.PrintDirectMonthlySaleReport(txtFromDate.Text, txtToDate.Text);
        }

        private void btnPrintPreview_Click(object sender, EventArgs e)
        {
            Models.Helper.PrinterLocalHelper.PrintDirectMonthlySaleReport(txtFromDate.Text, txtToDate.Text, true);
        }

        private void txtFromDate_Click(object sender, EventArgs e)
        {
            FormSelectDate frm = new FormSelectDate(this, "txtFromDate");
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog(this);
            frm.Dispose();
        }

        private void txtToDate_Click(object sender, EventArgs e)
        {
            FormSelectDate frm = new FormSelectDate(this, "txtToDate");
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog(this);
            frm.Dispose();
        }

        public void setDate(string controlId, string value)
        {
            TextEdit dateControl = (TextEdit)this.Controls.Find(controlId, true)[0];
            dateControl.Text = value;
        }

        private void btnFindNow_Click(object sender, EventArgs e)
        {

        }

        private void btnToday_Click(object sender, EventArgs e)
        {
            txtFromDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
            txtToDate.Text = DateTime.Now.ToString("MM/dd/yyyy");
        }

        private void btnYesterday_Click(object sender, EventArgs e)
        {
            DateTime yesterday = DateTime.Now.AddDays(-1);
            txtFromDate.Text = yesterday.ToString("MM/dd/yyyy");
            txtToDate.Text = yesterday.ToString("MM/dd/yyyy");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormReports_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Xử lý không bị cảm giác giật do xài Dispose() ngay nếu đóng thẳng
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000); // 3 giây
                try
                {
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
