
namespace NailsChekin.UserControl
{
    partial class UCSaleItemRefund
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lbName = new System.Windows.Forms.Label();
            this.lbQty = new System.Windows.Forms.Label();
            this.lbPrice = new System.Windows.Forms.Label();
            this.lbDiscount = new System.Windows.Forms.Label();
            this.lbTotal = new System.Windows.Forms.Label();
            this.chkSelected = new System.Windows.Forms.CheckBox();
            this.tbLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbLayout
            // 
            this.tbLayout.BackColor = System.Drawing.Color.White;
            this.tbLayout.ColumnCount = 6;
            this.tbLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.09524F));
            this.tbLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.523809F));
            this.tbLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tbLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tbLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tbLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.523809F));
            this.tbLayout.Controls.Add(this.lbName, 0, 0);
            this.tbLayout.Controls.Add(this.lbQty, 1, 0);
            this.tbLayout.Controls.Add(this.lbPrice, 2, 0);
            this.tbLayout.Controls.Add(this.lbDiscount, 3, 0);
            this.tbLayout.Controls.Add(this.lbTotal, 4, 0);
            this.tbLayout.Controls.Add(this.chkSelected, 5, 0);
            this.tbLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLayout.Location = new System.Drawing.Point(0, 0);
            this.tbLayout.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tbLayout.Name = "tbLayout";
            this.tbLayout.RowCount = 1;
            this.tbLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tbLayout.Size = new System.Drawing.Size(933, 51);
            this.tbLayout.TabIndex = 2;
            // 
            // lbName
            // 
            this.lbName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbName.AutoSize = true;
            this.lbName.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbName.Location = new System.Drawing.Point(150, 14);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(54, 23);
            this.lbName.TabIndex = 0;
            this.lbName.Text = "ITEM";
            // 
            // lbQty
            // 
            this.lbQty.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbQty.AutoSize = true;
            this.lbQty.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbQty.Location = new System.Drawing.Point(376, 14);
            this.lbQty.Name = "lbQty";
            this.lbQty.Size = new System.Drawing.Size(45, 23);
            this.lbQty.TabIndex = 1;
            this.lbQty.Text = "QTY";
            // 
            // lbPrice
            // 
            this.lbPrice.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbPrice.AutoSize = true;
            this.lbPrice.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPrice.Location = new System.Drawing.Point(479, 14);
            this.lbPrice.Name = "lbPrice";
            this.lbPrice.Size = new System.Drawing.Size(61, 23);
            this.lbPrice.TabIndex = 2;
            this.lbPrice.Text = "PRICE";
            // 
            // lbDiscount
            // 
            this.lbDiscount.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbDiscount.AutoSize = true;
            this.lbDiscount.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDiscount.Location = new System.Drawing.Point(592, 14);
            this.lbDiscount.Name = "lbDiscount";
            this.lbDiscount.Size = new System.Drawing.Size(101, 23);
            this.lbDiscount.TabIndex = 3;
            this.lbDiscount.Text = "DISCOUNT";
            // 
            // lbTotal
            // 
            this.lbTotal.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbTotal.AutoSize = true;
            this.lbTotal.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTotal.Location = new System.Drawing.Point(743, 14);
            this.lbTotal.Name = "lbTotal";
            this.lbTotal.Size = new System.Drawing.Size(65, 23);
            this.lbTotal.TabIndex = 4;
            this.lbTotal.Text = "TOTAL";
            // 
            // chkSelected
            // 
            this.chkSelected.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chkSelected.AutoSize = true;
            this.chkSelected.Font = new System.Drawing.Font("Tahoma", 26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkSelected.Location = new System.Drawing.Point(880, 18);
            this.chkSelected.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkSelected.Name = "chkSelected";
            this.chkSelected.Size = new System.Drawing.Size(15, 14);
            this.chkSelected.TabIndex = 5;
            this.chkSelected.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkSelected.UseVisualStyleBackColor = true;
            this.chkSelected.CheckedChanged += new System.EventHandler(this.chkSelected_CheckedChanged);
            // 
            // UCSaleItemRefund
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbLayout);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "UCSaleItemRefund";
            this.Size = new System.Drawing.Size(933, 51);
            this.tbLayout.ResumeLayout(false);
            this.tbLayout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tbLayout;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.Label lbQty;
        private System.Windows.Forms.Label lbPrice;
        private System.Windows.Forms.Label lbDiscount;
        private System.Windows.Forms.Label lbTotal;
        private System.Windows.Forms.CheckBox chkSelected;
    }
}
