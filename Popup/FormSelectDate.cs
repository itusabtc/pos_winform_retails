using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Calendar;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraScheduler;
using NailsChekin.Models;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NailsChekin.Popup
{
    public partial class FormSelectDate : Form
    {
        Control parentForm = null;

        string parentInputName = "";
        string current_date = "";

        public FormSelectDate()
        {
            InitializeComponent();
        }

        public FormSelectDate(Control parent, string inputName)
        {
            InitializeComponent();

            this.parentForm = parent;
            this.parentInputName = inputName; 
        }

        private void FormSelectDate_Load(object sender, EventArgs e)
        {
            dateCalender.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            dateCalender.CalendarView = CalendarView.Vista;
            dateCalender.VistaCalendarViewStyle = VistaCalendarViewStyle.YearView | VistaCalendarViewStyle.MonthView;

            dateCalender.EditValueChanged += DateCalender_EditValueChanged;
            btnConfirm.Width = dateCalender.Width;
        }

        private void DateCalender_EditValueChanged(object sender, EventArgs e)
        {
            if (dateCalender.View == DateEditCalendarViewType.MonthInfo)   // v19.2 có property này
            {
                current_date = dateCalender.DateTime.ToString("MM-dd-yyyy");
                //MessageBox.Show("current_date 2: " + current_date);
                ConfirmSelect();
                this.Close();
            }
        }

        private void dateCalender_DateTimeChanged(object sender, EventArgs e)
        {
            current_date = dateCalender.DateTime.ToString("MM/dd/yyyy");
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            this.ConfirmSelect();

            this.Close();
        }

        private void ConfirmSelect()
        {
            if (this.parentForm is FormSaleList)
            {
                ((FormSaleList)this.parentForm).setDate(this.parentInputName, this.current_date);
            }
            else if (this.parentForm is FormReports)
            {
                ((FormReports)this.parentForm).setDate(this.parentInputName, this.current_date);
            }

            Core.ClearMemory();          
        }

        private void dateCalender_EditValueChanged(object sender, EventArgs e)
        {
            //if (current_date.Trim().Length > 0)
            //{
            //    current_date = dateCalender.DateTime.ToString("MM/dd/yyyy");

            //    this.ConfirmSelect();
            //    this.Dispose();
            //}

            //if (!dateCalender.IsPopupOpen)  // popup đã đóng ➜ chắc chắn có ngày
            //{          
            //    current_date = dateCalender.DateTime.ToString("MM/dd/yyyy");
            //    ConfirmSelect();
            //    Close();
            //}

            //if (dateCalender.View == DateEditCalendarViewType.MonthInfo)   // v19.2 có property này
            //{
            //    current_date = dateCalender.DateTime.ToString("MM/dd/yyyy");
            //    MessageBox.Show("current_date 2: " + current_date);
            //    //ConfirmSelect();
            //    //Close();
            //}

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormSelectDate_FormClosed(object sender, FormClosedEventArgs e)
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

        private void FormSelectDate_Shown(object sender, EventArgs e)
        {
            this.Width = dateCalender.Width + 55;
            btnClose.Left = this.Width - btnClose.Width - 10;

            btnConfirm.Width = dateCalender.Width;
            btnConfirm.Top = dateCalender.Bottom + 10;
        }
    }
}
