using NailsChekin.Models;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NailsChekin.Popup
{
    public partial class FormTaxRedeem : Form
    {
        Control parent;
        double tax_percent = 0;
        bool tax_include = true;

        public FormTaxRedeem()
        {
            InitializeComponent();
        }

        public FormTaxRedeem(Control parent, double tax_percent, bool tax_include)
        {
            InitializeComponent();

            this.parent = parent;
            this.tax_percent = tax_percent;
            this.tax_include = tax_include;

            txtCurrentText.Text = this.tax_percent + "%";

            var colorOn  = Color.FromArgb(56, 142, 60);
            var colorOff = Color.FromArgb(198, 40, 40);

            StyleRadioButton(rdApplyTaxON,  checkedBorder: Color.FromArgb(27, 94, 32));
            StyleRadioButton(rdApplyTaxOFF, checkedBorder: Color.FromArgb(127, 0, 0));

            rdApplyTaxON.CheckedChanged += (s, e) => {
                if (rdApplyTaxON.Checked) rdApplyTaxOFF.Checked = false;
                ApplyRadioColor(rdApplyTaxON, colorOn);
                ApplyRadioColor(rdApplyTaxOFF, colorOff);
            };
            rdApplyTaxOFF.CheckedChanged += (s, e) => {
                if (rdApplyTaxOFF.Checked) rdApplyTaxON.Checked = false;
                ApplyRadioColor(rdApplyTaxON, colorOn);
                ApplyRadioColor(rdApplyTaxOFF, colorOff);
            };

            rdApplyTaxON.Checked = tax_include;
            rdApplyTaxOFF.Checked = !tax_include;

            // Designer mặc định Checked=true nên CheckedChanged có thể không fire — apply màu thủ công
            ApplyRadioColor(rdApplyTaxON, colorOn);
            ApplyRadioColor(rdApplyTaxOFF, colorOff);
        }

        private static void StyleRadioButton(RadioButton rd, Color checkedBorder)
        {
            rd.Appearance = System.Windows.Forms.Appearance.Button;
            rd.FlatStyle = FlatStyle.Flat;
            rd.AutoSize = false;
            rd.Size = new Size(190, 100);
            rd.Font = new Font("Segoe UI", 26f, FontStyle.Bold);
            rd.Padding = new Padding(0);
            rd.FlatAppearance.BorderSize = 2;
            rd.FlatAppearance.BorderColor = checkedBorder;

            rd.Paint += (s, pe) =>
            {
                var g = pe.Graphics;
                g.Clear(rd.BackColor);
                using (var pen = new Pen(rd.FlatAppearance.BorderColor, rd.FlatAppearance.BorderSize))
                    g.DrawRectangle(pen, 0, 0, rd.Width - 1, rd.Height - 1);
                using (var brush = new SolidBrush(rd.ForeColor))
                using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    g.DrawString(rd.Text, rd.Font, brush, new RectangleF(0, 0, rd.Width, rd.Height), sf);
            };
        }

        private static void ApplyRadioColor(RadioButton rd, Color checkedBack)
        {
            rd.BackColor = rd.Checked ? checkedBack : SystemColors.Control;
            rd.ForeColor = rd.Checked ? Color.White : Color.Black;
            rd.Refresh();
        }

        private void FormTaxRedeem_Load(object sender, EventArgs e)
        {
            this.Adjust_Screen();
        }

        private void Adjust_Screen()
        {
            //Bất đồng bộ
            this.BeginInvoke(new Action(() =>
            {
                btnClose.Location = new Point(this.Width - btnClose.Width - 10, btnClose.Location.Y);
            }));
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            ((FormMain)parent).SetTaxInfo(this.tax_percent, rdApplyTaxON.Checked);
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormTaxRedeem_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Xử lý không bị cảm giác giật do xài Dispose() ngay nếu đóng thẳng
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000); // 3 giây

                try
                {
                    Core.ClearMemory();

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
