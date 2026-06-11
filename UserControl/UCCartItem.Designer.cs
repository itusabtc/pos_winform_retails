namespace NailsChekin.UserControl
{
    partial class UCCartItem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCCartItem));
            this.tbLayout = new System.Windows.Forms.TableLayoutPanel();
            this.btnRemove = new DevExpress.XtraEditors.SvgImageBox();
            this.lbTotal = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.svgDecrease = new DevExpress.XtraEditors.SvgImageBox();
            this.svgIncrease = new DevExpress.XtraEditors.SvgImageBox();
            this.lbQty = new System.Windows.Forms.Label();
            this.txtPrice = new System.Windows.Forms.TextBox();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.tbLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnRemove)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.svgDecrease)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.svgIncrease)).BeginInit();
            this.SuspendLayout();
            // 
            // tbLayout
            // 
            this.tbLayout.BackColor = System.Drawing.Color.FloralWhite;
            this.tbLayout.ColumnCount = 5;
            this.tbLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tbLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18F));
            this.tbLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18F));
            this.tbLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18F));
            this.tbLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.tbLayout.Controls.Add(this.btnRemove, 4, 0);
            this.tbLayout.Controls.Add(this.lbTotal, 3, 0);
            this.tbLayout.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tbLayout.Controls.Add(this.txtPrice, 2, 0);
            this.tbLayout.Controls.Add(this.txtProductName, 0, 0);
            this.tbLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLayout.Font = new System.Drawing.Font("Tahoma", 9F);
            this.tbLayout.Location = new System.Drawing.Point(0, 0);
            this.tbLayout.Margin = new System.Windows.Forms.Padding(2);
            this.tbLayout.Name = "tbLayout";
            this.tbLayout.RowCount = 1;
            this.tbLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tbLayout.Size = new System.Drawing.Size(1060, 53);
            this.tbLayout.TabIndex = 2;
            // 
            // btnRemove
            // 
            this.btnRemove.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRemove.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRemove.Location = new System.Drawing.Point(996, 2);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(2);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(62, 49);
            this.btnRemove.SizeMode = DevExpress.XtraEditors.SvgImageSizeMode.Zoom;
            this.btnRemove.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnRemove.SvgImage")));
            this.btnRemove.TabIndex = 1;
            this.btnRemove.Text = "svgImageBox1";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // lbTotal
            // 
            this.lbTotal.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbTotal.AutoSize = true;
            this.lbTotal.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTotal.ForeColor = System.Drawing.Color.Blue;
            this.lbTotal.Location = new System.Drawing.Point(861, 10);
            this.lbTotal.Name = "lbTotal";
            this.lbTotal.Size = new System.Drawing.Size(75, 33);
            this.lbTotal.TabIndex = 9;
            this.lbTotal.Text = "Total";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.Controls.Add(this.svgDecrease, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.svgIncrease, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.lbQty, 1, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(427, 4);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(184, 45);
            this.tableLayoutPanel2.TabIndex = 10;
            // 
            // svgDecrease
            // 
            this.svgDecrease.Cursor = System.Windows.Forms.Cursors.Hand;
            this.svgDecrease.Location = new System.Drawing.Point(3, 4);
            this.svgDecrease.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.svgDecrease.Name = "svgDecrease";
            this.svgDecrease.Size = new System.Drawing.Size(38, 37);
            this.svgDecrease.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgDecrease.SvgImage")));
            this.svgDecrease.TabIndex = 0;
            this.svgDecrease.Text = "svgDecrease";
            this.svgDecrease.Click += new System.EventHandler(this.svgDecrease_Click);
            // 
            // svgIncrease
            // 
            this.svgIncrease.Cursor = System.Windows.Forms.Cursors.Hand;
            this.svgIncrease.Location = new System.Drawing.Point(141, 4);
            this.svgIncrease.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.svgIncrease.Name = "svgIncrease";
            this.svgIncrease.Size = new System.Drawing.Size(38, 37);
            this.svgIncrease.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgIncrease.SvgImage")));
            this.svgIncrease.TabIndex = 1;
            this.svgIncrease.Text = "svgImageBox3";
            this.svgIncrease.Click += new System.EventHandler(this.svgIncrease_Click);
            // 
            // lbQty
            // 
            this.lbQty.AutoSize = true;
            this.lbQty.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbQty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbQty.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbQty.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbQty.Location = new System.Drawing.Point(49, 0);
            this.lbQty.Name = "lbQty";
            this.lbQty.Size = new System.Drawing.Size(86, 45);
            this.lbQty.TabIndex = 2;
            this.lbQty.Text = "Qty";
            this.lbQty.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbQty.Click += new System.EventHandler(this.lbQty_Click);
            // 
            // txtPrice
            // 
            this.txtPrice.Cursor = System.Windows.Forms.Cursors.Hand;
            this.txtPrice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPrice.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPrice.Location = new System.Drawing.Point(617, 4);
            this.txtPrice.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtPrice.Name = "txtPrice";
            this.txtPrice.ReadOnly = true;
            this.txtPrice.Size = new System.Drawing.Size(184, 44);
            this.txtPrice.TabIndex = 11;
            this.txtPrice.Click += new System.EventHandler(this.txtPrice_Click);
            this.txtPrice.TextChanged += new System.EventHandler(this.txtPrice_TextChanged);
            this.txtPrice.MouseLeave += new System.EventHandler(this.txtPrice_MouseLeave);
            // 
            // txtProductName
            // 
            this.txtProductName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProductName.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtProductName.Location = new System.Drawing.Point(3, 4);
            this.txtProductName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.Size = new System.Drawing.Size(418, 44);
            this.txtProductName.TabIndex = 12;
            this.txtProductName.Click += new System.EventHandler(this.txtProductName_Click);
            this.txtProductName.TextChanged += new System.EventHandler(this.txtProductName_TextChanged);
            // 
            // UCCartItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbLayout);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "UCCartItem";
            this.Size = new System.Drawing.Size(1060, 53);
            this.tbLayout.ResumeLayout(false);
            this.tbLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnRemove)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.svgDecrease)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.svgIncrease)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tbLayout;
        private DevExpress.XtraEditors.SvgImageBox btnRemove;
        private System.Windows.Forms.Label lbTotal;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private DevExpress.XtraEditors.SvgImageBox svgDecrease;
        private DevExpress.XtraEditors.SvgImageBox svgIncrease;
        private System.Windows.Forms.Label lbQty;
        private System.Windows.Forms.TextBox txtPrice;
        private System.Windows.Forms.TextBox txtProductName;
    }
}
