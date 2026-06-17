namespace NailsChekin.UserControl
{
    partial class UCCustomerInfoLine
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lbAmount = new System.Windows.Forms.Label();
            this.lbQty = new System.Windows.Forms.Label();
            this.lbPrice = new System.Windows.Forms.Label();
            this.lbService = new System.Windows.Forms.Label();
            this.lbDate = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.HighlightText;
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel1.Controls.Add(this.lbAmount, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbQty, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbPrice, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbService, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbDate, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(827, 83);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // lbAmount
            // 
            this.lbAmount.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbAmount.AutoSize = true;
            this.lbAmount.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAmount.Location = new System.Drawing.Point(725, 29);
            this.lbAmount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbAmount.Name = "lbAmount";
            this.lbAmount.Size = new System.Drawing.Size(79, 24);
            this.lbAmount.TabIndex = 4;
            this.lbAmount.Text = "Amount";
            // 
            // lbQty
            // 
            this.lbQty.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbQty.AutoSize = true;
            this.lbQty.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbQty.Location = new System.Drawing.Point(640, 29);
            this.lbQty.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbQty.Name = "lbQty";
            this.lbQty.Size = new System.Drawing.Size(41, 24);
            this.lbQty.TabIndex = 3;
            this.lbQty.Text = "Qty";
            // 
            // lbPrice
            // 
            this.lbPrice.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbPrice.AutoSize = true;
            this.lbPrice.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPrice.Location = new System.Drawing.Point(531, 29);
            this.lbPrice.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbPrice.Name = "lbPrice";
            this.lbPrice.Size = new System.Drawing.Size(53, 24);
            this.lbPrice.TabIndex = 2;
            this.lbPrice.Text = "Price";
            // 
            // lbService
            // 
            this.lbService.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbService.AutoSize = true;
            this.lbService.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbService.Location = new System.Drawing.Point(271, 29);
            this.lbService.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbService.Name = "lbService";
            this.lbService.Size = new System.Drawing.Size(77, 24);
            this.lbService.TabIndex = 1;
            this.lbService.Text = "Product";
            // 
            // lbDate
            // 
            this.lbDate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbDate.AutoSize = true;
            this.lbDate.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDate.Location = new System.Drawing.Point(35, 29);
            this.lbDate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbDate.Name = "lbDate";
            this.lbDate.Size = new System.Drawing.Size(53, 24);
            this.lbDate.TabIndex = 0;
            this.lbDate.Text = "Date";
            // 
            // UCCustomerInfoLine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "UCCustomerInfoLine";
            this.Size = new System.Drawing.Size(827, 83);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lbAmount;
        private System.Windows.Forms.Label lbQty;
        private System.Windows.Forms.Label lbPrice;
        private System.Windows.Forms.Label lbService;
        private System.Windows.Forms.Label lbDate;

    }
}
