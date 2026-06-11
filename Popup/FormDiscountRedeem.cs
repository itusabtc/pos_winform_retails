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
    public partial class FormDiscountRedeem : Form
    {
        Control parent;
        double discount_value = 0;
        string discount_unit = "%";

        public FormDiscountRedeem()
        {
            InitializeComponent();
        }

        public FormDiscountRedeem(Control parent, double discount_value, string discount_unit)
        {
            InitializeComponent();

            this.parent = parent;
            txtCurrentText.Text = discount_value.ToString();

            var colorPct = Color.FromArgb(21, 101, 192);
            var colorUsd = Color.FromArgb(46, 125, 50);

            StyleRadioButton(rbUnitPercent, checkedBorder: Color.FromArgb(13, 71, 161));
            StyleRadioButton(rbUnitUsd,     checkedBorder: Color.FromArgb(27, 94, 32));

            // Căn giữa trong roundPanel1 (323px): (323 - 120 - 20 - 120) / 2 = 31
            rbUnitPercent.Location = new Point(31,  rbUnitPercent.Location.Y);
            rbUnitUsd.Location     = new Point(171, rbUnitUsd.Location.Y);

            rbUnitPercent.CheckedChanged += (s, e) => {
                if (rbUnitPercent.Checked) rbUnitUsd.Checked = false;
                ApplyRadioColor(rbUnitPercent, colorPct);
                ApplyRadioColor(rbUnitUsd, colorUsd);
            };
            rbUnitUsd.CheckedChanged += (s, e) => {
                if (rbUnitUsd.Checked) rbUnitPercent.Checked = false;
                ApplyRadioColor(rbUnitPercent, colorPct);
                ApplyRadioColor(rbUnitUsd, colorUsd);
            };

            rbUnitPercent.Checked = discount_unit.Equals("%");
            rbUnitUsd.Checked     = discount_unit.Equals("$");

            // Designer mặc định Checked=true nên CheckedChanged có thể không fire — apply màu thủ công
            ApplyRadioColor(rbUnitPercent, colorPct);
            ApplyRadioColor(rbUnitUsd, colorUsd);

        }

        private static void StyleRadioButton(RadioButton rd, Color checkedBorder)
        {
            rd.Appearance = System.Windows.Forms.Appearance.Button;
            rd.FlatStyle = FlatStyle.Flat;
            rd.AutoSize = false;
            rd.Size = new Size(120, 80);
            rd.Font = new Font("Segoe UI", 24f, FontStyle.Bold);
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

        private void FormDiscountRedeem_Load(object sender, EventArgs e)
        {
            this.Adjust_Screen();

            var kb = new MyControls.KeyBoardTemplateBar
            {
                Dock = DockStyle.Fill,
                AllowDecimal = true,
                MaxLength = 10,
                ButtonFontSize = 26f,
                ButtonCornerRadius = 12,
                TargetControl = txtCurrentText,
                DefaultValue = this.discount_value.ToString()
            };
            kb.ButtonFontSize = 36.0f;
            panelCart_Control_Keyboard.Controls.Add(kb);
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
            double discount = string.IsNullOrEmpty(txtCurrentText.Text) ? 0 : double.Parse(txtCurrentText.Text);
            ((FormMain)parent).SetDiscountInfo(discount, rbUnitPercent.Checked ? "%" : "$");
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormDiscountRedeem_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Xử lý không bị cảm giác giật do xài Dispose() ngay nếu đóng thẳng
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000); // 3 giây

                try
                {
                    Core.ClearAndDisposeV2(panelCart_Control_Keyboard);
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
