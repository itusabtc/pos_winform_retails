namespace NailsChekin.UserControl
{
    partial class UCCartItemSelect
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCCartItemSelect));
            this.tbLayout = new System.Windows.Forms.TableLayoutPanel();
            this.btnRemove = new DevExpress.XtraEditors.SvgImageBox();
            this.lbTotal = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.svgDecrease = new DevExpress.XtraEditors.SvgImageBox();
            this.svgIncrease = new DevExpress.XtraEditors.SvgImageBox();
            this.lbQty = new System.Windows.Forms.Label();
            this.txtPrice = new System.Windows.Forms.TextBox();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.svgSelect = new DevExpress.XtraEditors.SvgImageBox();
            this.tbLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnRemove)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.svgDecrease)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.svgIncrease)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.svgSelect)).BeginInit();
            this.SuspendLayout();
            // 
            // tbLayout
            // 
            this.tbLayout.BackColor = System.Drawing.Color.FloralWhite;
            this.tbLayout.ColumnCount = 6;
            this.tbLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tbLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tbLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tbLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tbLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tbLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tbLayout.Controls.Add(this.btnRemove, 4, 0);
            this.tbLayout.Controls.Add(this.lbTotal, 3, 0);
            this.tbLayout.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tbLayout.Controls.Add(this.txtPrice, 2, 0);
            this.tbLayout.Controls.Add(this.txtProductName, 0, 0);
            this.tbLayout.Controls.Add(this.svgSelect, 5, 0);
            this.tbLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLayout.Font = new System.Drawing.Font("Tahoma", 9F);
            this.tbLayout.Location = new System.Drawing.Point(0, 0);
            this.tbLayout.Margin = new System.Windows.Forms.Padding(2);
            this.tbLayout.Name = "tbLayout";
            this.tbLayout.RowCount = 1;
            this.tbLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tbLayout.Size = new System.Drawing.Size(1036, 54);
            this.tbLayout.TabIndex = 3;
            // 
            // btnRemove
            // 
            this.btnRemove.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRemove.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRemove.Location = new System.Drawing.Point(933, 2);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(2);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(47, 50);
            this.btnRemove.SizeMode = DevExpress.XtraEditors.SvgImageSizeMode.Zoom;
            this.btnRemove.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnRemove.SvgImage")));
            this.btnRemove.TabIndex = 1;
            this.btnRemove.Text = "svgImageBox1";
            // 
            // lbTotal
            // 
            this.lbTotal.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbTotal.AutoSize = true;
            this.lbTotal.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTotal.ForeColor = System.Drawing.Color.Blue;
            this.lbTotal.Location = new System.Drawing.Point(816, 10);
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
            this.tableLayoutPanel2.Location = new System.Drawing.Point(469, 4);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(149, 45);
            this.tableLayoutPanel2.TabIndex = 10;
            // 
            // svgDecrease
            // 
            this.svgDecrease.Cursor = System.Windows.Forms.Cursors.Hand;
            this.svgDecrease.Location = new System.Drawing.Point(3, 4);
            this.svgDecrease.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.svgDecrease.Name = "svgDecrease";
            this.svgDecrease.Size = new System.Drawing.Size(31, 37);
            this.svgDecrease.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgDecrease.SvgImage")));
            this.svgDecrease.TabIndex = 0;
            this.svgDecrease.Text = "svgDecrease";
            // 
            // svgIncrease
            // 
            this.svgIncrease.Cursor = System.Windows.Forms.Cursors.Hand;
            this.svgIncrease.Location = new System.Drawing.Point(114, 4);
            this.svgIncrease.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.svgIncrease.Name = "svgIncrease";
            this.svgIncrease.Size = new System.Drawing.Size(32, 37);
            this.svgIncrease.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgIncrease.SvgImage")));
            this.svgIncrease.TabIndex = 1;
            this.svgIncrease.Text = "svgImageBox3";
            // 
            // lbQty
            // 
            this.lbQty.AutoSize = true;
            this.lbQty.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbQty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbQty.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lbQty.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbQty.Location = new System.Drawing.Point(40, 0);
            this.lbQty.Name = "lbQty";
            this.lbQty.Size = new System.Drawing.Size(68, 45);
            this.lbQty.TabIndex = 2;
            this.lbQty.Text = "Qty";
            this.lbQty.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtPrice
            // 
            this.txtPrice.Cursor = System.Windows.Forms.Cursors.Hand;
            this.txtPrice.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPrice.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPrice.Location = new System.Drawing.Point(624, 4);
            this.txtPrice.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtPrice.Name = "txtPrice";
            this.txtPrice.ReadOnly = true;
            this.txtPrice.Size = new System.Drawing.Size(149, 44);
            this.txtPrice.TabIndex = 11;
            // 
            // txtProductName
            // 
            this.txtProductName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtProductName.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtProductName.Location = new System.Drawing.Point(3, 4);
            this.txtProductName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtProductName.Name = "txtProductName";
            this.txtProductName.Size = new System.Drawing.Size(460, 44);
            this.txtProductName.TabIndex = 12;
            // 
            // svgSelect
            // 
            this.svgSelect.Location = new System.Drawing.Point(985, 3);
            this.svgSelect.Name = "svgSelect";
            this.svgSelect.Size = new System.Drawing.Size(48, 48);
            this.svgSelect.SizeMode = DevExpress.XtraEditors.SvgImageSizeMode.Zoom;
            this.svgSelect.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgSelect.SvgImage")));
            this.svgSelect.TabIndex = 13;
            this.svgSelect.Text = "svgImageBox1";
            this.svgSelect.Click += new System.EventHandler(this.svgSelect_Click);
            // 
            // UCCartItemSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbLayout);
            this.Name = "UCCartItemSelect";
            this.Size = new System.Drawing.Size(1036, 54);
            this.tbLayout.ResumeLayout(false);
            this.tbLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnRemove)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.svgDecrease)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.svgIncrease)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.svgSelect)).EndInit();
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
        private DevExpress.XtraEditors.SvgImageBox svgSelect;
    }
}
