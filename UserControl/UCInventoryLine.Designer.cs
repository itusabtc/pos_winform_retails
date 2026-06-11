namespace NailsChekin.UserControl
{
    partial class UCInventoryLine
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCInventoryLine));
            this.tbHeader = new System.Windows.Forms.TableLayoutPanel();
            this.lbID = new System.Windows.Forms.Label();
            this.lbBarcode = new System.Windows.Forms.Label();
            this.lbName = new System.Windows.Forms.Label();
            this.lbCatalog = new System.Windows.Forms.Label();
            this.lbPrice = new System.Windows.Forms.Label();
            this.txtQty = new System.Windows.Forms.TextBox();
            this.svgEdit = new DevExpress.XtraEditors.SvgImageBox();
            this.tbHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.svgEdit)).BeginInit();
            this.SuspendLayout();
            // 
            // tbHeader
            // 
            this.tbHeader.BackColor = System.Drawing.Color.White;
            this.tbHeader.ColumnCount = 7;
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tbHeader.Controls.Add(this.lbID, 0, 0);
            this.tbHeader.Controls.Add(this.lbBarcode, 1, 0);
            this.tbHeader.Controls.Add(this.lbName, 2, 0);
            this.tbHeader.Controls.Add(this.lbCatalog, 3, 0);
            this.tbHeader.Controls.Add(this.lbPrice, 4, 0);
            this.tbHeader.Controls.Add(this.txtQty, 5, 0);
            this.tbHeader.Controls.Add(this.svgEdit, 6, 0);
            this.tbHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbHeader.Location = new System.Drawing.Point(0, 0);
            this.tbHeader.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbHeader.Name = "tbHeader";
            this.tbHeader.RowCount = 1;
            this.tbHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tbHeader.Size = new System.Drawing.Size(1256, 58);
            this.tbHeader.TabIndex = 1;
            // 
            // lbID
            // 
            this.lbID.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbID.AutoSize = true;
            this.lbID.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbID.Location = new System.Drawing.Point(44, 14);
            this.lbID.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbID.Name = "lbID";
            this.lbID.Size = new System.Drawing.Size(36, 29);
            this.lbID.TabIndex = 0;
            this.lbID.Text = "ID";
            // 
            // lbBarcode
            // 
            this.lbBarcode.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbBarcode.AutoSize = true;
            this.lbBarcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbBarcode.Location = new System.Drawing.Point(154, 14);
            this.lbBarcode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbBarcode.Name = "lbBarcode";
            this.lbBarcode.Size = new System.Drawing.Size(130, 29);
            this.lbBarcode.TabIndex = 1;
            this.lbBarcode.Text = "BARCODE";
            // 
            // lbName
            // 
            this.lbName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbName.AutoSize = true;
            this.lbName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbName.Location = new System.Drawing.Point(460, 14);
            this.lbName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(82, 29);
            this.lbName.TabIndex = 2;
            this.lbName.Text = "NAME";
            // 
            // lbCatalog
            // 
            this.lbCatalog.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbCatalog.AutoSize = true;
            this.lbCatalog.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCatalog.Location = new System.Drawing.Point(730, 14);
            this.lbCatalog.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbCatalog.Name = "lbCatalog";
            this.lbCatalog.Size = new System.Drawing.Size(169, 29);
            this.lbCatalog.TabIndex = 3;
            this.lbCatalog.Text = "CATEGORIES";
            // 
            // lbPrice
            // 
            this.lbPrice.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbPrice.AutoSize = true;
            this.lbPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPrice.Location = new System.Drawing.Point(960, 14);
            this.lbPrice.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbPrice.Name = "lbPrice";
            this.lbPrice.Size = new System.Drawing.Size(85, 29);
            this.lbPrice.TabIndex = 4;
            this.lbPrice.Text = "PRICE";
            // 
            // txtQty
            // 
            this.txtQty.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtQty.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtQty.Location = new System.Drawing.Point(1069, 11);
            this.txtQty.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtQty.Name = "txtQty";
            this.txtQty.ReadOnly = true;
            this.txtQty.Size = new System.Drawing.Size(117, 36);
            this.txtQty.TabIndex = 5;
            this.txtQty.Click += new System.EventHandler(this.txtQty_Click);
            this.txtQty.TextChanged += new System.EventHandler(this.txtQty_TextChanged);
            this.txtQty.MouseLeave += new System.EventHandler(this.txtQty_MouseLeave);
            // 
            // svgEdit
            // 
            this.svgEdit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.svgEdit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.svgEdit.Location = new System.Drawing.Point(1200, 6);
            this.svgEdit.Name = "svgEdit";
            this.svgEdit.Size = new System.Drawing.Size(45, 45);
            this.svgEdit.SizeMode = DevExpress.XtraEditors.SvgImageSizeMode.Stretch;
            this.svgEdit.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgEdit.SvgImage")));
            this.svgEdit.TabIndex = 6;
            this.svgEdit.Text = "svgEdit";
            this.svgEdit.Click += new System.EventHandler(this.svgEdit_Click);
            // 
            // UCInventoryLine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbHeader);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "UCInventoryLine";
            this.Size = new System.Drawing.Size(1256, 58);
            this.tbHeader.ResumeLayout(false);
            this.tbHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.svgEdit)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tbHeader;
        private System.Windows.Forms.Label lbID;
        private System.Windows.Forms.Label lbBarcode;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.Label lbCatalog;
        private System.Windows.Forms.Label lbPrice;
        private System.Windows.Forms.TextBox txtQty;
        private DevExpress.XtraEditors.SvgImageBox svgEdit;
    }
}
