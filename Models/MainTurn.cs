using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NailsChekin.Models
{
    class MainTurn
    {

        public TableLayoutPanel AddDynamicTableLayoutPanel_Header(Control panel, int columnCount, string[] nails_id, string[] nails, string[] nails_img)
        {
            panel.Controls.Clear();

            TableLayoutPanel tbLayoutNailsTech = new System.Windows.Forms.TableLayoutPanel();

            tbLayoutNailsTech.BackColor = System.Drawing.SystemColors.ControlLight;
            tbLayoutNailsTech.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;

            tbLayoutNailsTech.Dock = System.Windows.Forms.DockStyle.Fill;
            tbLayoutNailsTech.Location = new System.Drawing.Point(0, 0);
            tbLayoutNailsTech.Name = "tbLayoutNailsTech";

            tbLayoutNailsTech.RowCount = 2;
            tbLayoutNailsTech.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            tbLayoutNailsTech.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));

            //tbLayoutNailsTech.Size = new System.Drawing.Size(894, 106);
            tbLayoutNailsTech.Size = new System.Drawing.Size(panel.Width, 106);
            tbLayoutNailsTech.TabIndex = 0;

            tbLayoutNailsTech.ColumnCount = columnCount;

            float total_width = panel.Width;
            total_width = (float)(total_width - 10.0);

            //FIX COLUMN 0
            tbLayoutNailsTech.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5.0F));

            float width = (float)(95.0 / (columnCount - 1));
            for (int i = 1; i < columnCount; i++)
            {
                tbLayoutNailsTech.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, width));
            }
            

            for (int i = 1; i < tbLayoutNailsTech.ColumnCount; i++)
            {
                Label label = new Label();
                label.Name = "Tech" + i;
                label.Anchor = System.Windows.Forms.AnchorStyles.None;
                label.AutoSize = true;
                label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                label.Location = new System.Drawing.Point(759, 3);
                label.Text = (nails[i - 1] == null ? "" : nails[i - 1]); //Phần tử 0 fix cố định là Time

                PictureBox picBox = new PictureBox();
                picBox.Dock = System.Windows.Forms.DockStyle.Fill;
                picBox.Location = new System.Drawing.Point(268, 25);
                picBox.Size = new System.Drawing.Size(202, 77);
                picBox.SizeMode = PictureBoxSizeMode.Zoom;
                if (nails_img != null && nails_img[i - 1] != null && !nails_img[i - 1].Equals("NA"))
                {
                    try
                    {
                        picBox.LoadAsync(Constants.imgURL + nails_img[i - 1]);
                    }
                    catch { }
                }

                tbLayoutNailsTech.Controls.Add(label, i, 0);
                tbLayoutNailsTech.Controls.Add(picBox, i, 1);

                tbLayoutNailsTech.Name = (nails_id[i - 1] == null ? "" : nails_id[i - 1]);
            }

            Label label_time = new Label();
            label_time.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            label_time.AutoSize = true;
            label_time.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label_time.Location = new System.Drawing.Point(4, 78);
            label_time.Margin = new System.Windows.Forms.Padding(3, 0, 3, 10);
            label_time.Size = new System.Drawing.Size(39, 17);
            label_time.Text = "Time";
            tbLayoutNailsTech.Controls.Add(label_time, 0, 1);

            panel.Controls.Add(tbLayoutNailsTech);
            return tbLayoutNailsTech;

        }


    }
}
