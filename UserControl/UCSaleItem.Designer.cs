namespace NailsChekin.UserControl
{
    partial class UCSaleItem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCSaleItem));
            this.tbHeader = new System.Windows.Forms.TableLayoutPanel();
            this.lbID = new System.Windows.Forms.Label();
            this.lbDate = new System.Windows.Forms.Label();
            this.lbCustomer = new System.Windows.Forms.Label();
            this.lbProduct = new System.Windows.Forms.Label();
            this.lbAmount = new System.Windows.Forms.Label();
            this.lbCash = new System.Windows.Forms.Label();
            this.svgPrinter = new DevExpress.XtraEditors.SvgImageBox();
            this.lbCharge = new System.Windows.Forms.Label();
            this.lbStatus = new System.Windows.Forms.Label();
            this.svgPaymentNow = new DevExpress.XtraEditors.SvgImageBox();
            this.svgEmailSms = new DevExpress.XtraEditors.SvgImageBox();
            this.svgDelete = new DevExpress.XtraEditors.SvgImageBox();
            this.chkSelected = new System.Windows.Forms.CheckBox();
            this.tbHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.svgPrinter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.svgPaymentNow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.svgEmailSms)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.svgDelete)).BeginInit();
            this.SuspendLayout();
            // 
            // tbHeader
            // 
            this.tbHeader.BackColor = System.Drawing.Color.White;
            this.tbHeader.ColumnCount = 13;
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 4F));
            this.tbHeader.Controls.Add(this.lbID, 0, 0);
            this.tbHeader.Controls.Add(this.lbDate, 1, 0);
            this.tbHeader.Controls.Add(this.lbCustomer, 2, 0);
            this.tbHeader.Controls.Add(this.lbProduct, 3, 0);
            this.tbHeader.Controls.Add(this.lbAmount, 4, 0);
            this.tbHeader.Controls.Add(this.lbCash, 5, 0);
            this.tbHeader.Controls.Add(this.svgPrinter, 8, 0);
            this.tbHeader.Controls.Add(this.lbCharge, 6, 0);
            this.tbHeader.Controls.Add(this.lbStatus, 7, 0);
            this.tbHeader.Controls.Add(this.svgPaymentNow, 9, 0);
            this.tbHeader.Controls.Add(this.svgEmailSms, 10, 0);
            this.tbHeader.Controls.Add(this.svgDelete, 11, 0);
            this.tbHeader.Controls.Add(this.chkSelected, 12, 0);
            this.tbHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbHeader.Location = new System.Drawing.Point(0, 0);
            this.tbHeader.Name = "tbHeader";
            this.tbHeader.RowCount = 1;
            this.tbHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tbHeader.Size = new System.Drawing.Size(1343, 46);
            this.tbHeader.TabIndex = 2;
            // 
            // lbID
            // 
            this.lbID.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbID.AutoSize = true;
            this.lbID.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbID.Location = new System.Drawing.Point(42, 14);
            this.lbID.Name = "lbID";
            this.lbID.Size = new System.Drawing.Size(22, 18);
            this.lbID.TabIndex = 0;
            this.lbID.Text = "ID";
            // 
            // lbDate
            // 
            this.lbDate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbDate.AutoSize = true;
            this.lbDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDate.Location = new System.Drawing.Point(137, 14);
            this.lbDate.Name = "lbDate";
            this.lbDate.Size = new System.Drawing.Size(47, 18);
            this.lbDate.TabIndex = 1;
            this.lbDate.Text = "DATE";
            // 
            // lbCustomer
            // 
            this.lbCustomer.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbCustomer.AutoSize = true;
            this.lbCustomer.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCustomer.Location = new System.Drawing.Point(233, 14);
            this.lbCustomer.Name = "lbCustomer";
            this.lbCustomer.Size = new System.Drawing.Size(95, 18);
            this.lbCustomer.TabIndex = 2;
            this.lbCustomer.Text = "CUSTOMER";
            // 
            // lbProduct
            // 
            this.lbProduct.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbProduct.AutoSize = true;
            this.lbProduct.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbProduct.Location = new System.Drawing.Point(422, 14);
            this.lbProduct.Name = "lbProduct";
            this.lbProduct.Size = new System.Drawing.Size(93, 18);
            this.lbProduct.TabIndex = 3;
            this.lbProduct.Text = "PRODUCTS";
            // 
            // lbAmount
            // 
            this.lbAmount.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbAmount.AutoSize = true;
            this.lbAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAmount.Location = new System.Drawing.Point(619, 14);
            this.lbAmount.Name = "lbAmount";
            this.lbAmount.Size = new System.Drawing.Size(73, 18);
            this.lbAmount.TabIndex = 4;
            this.lbAmount.Text = "AMOUNT";
            // 
            // lbCash
            // 
            this.lbCash.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbCash.AutoSize = true;
            this.lbCash.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCash.Location = new System.Drawing.Point(749, 13);
            this.lbCash.Name = "lbCash";
            this.lbCash.Size = new System.Drawing.Size(54, 20);
            this.lbCash.TabIndex = 7;
            this.lbCash.Text = "CASH";
            // 
            // svgPrinter
            // 
            this.svgPrinter.Cursor = System.Windows.Forms.Cursors.Hand;
            this.svgPrinter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.svgPrinter.Location = new System.Drawing.Point(1074, 3);
            this.svgPrinter.Name = "svgPrinter";
            this.svgPrinter.Size = new System.Drawing.Size(47, 40);
            this.svgPrinter.SizeMode = DevExpress.XtraEditors.SvgImageSizeMode.Zoom;
            this.svgPrinter.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgPrinter.SvgImage")));
            this.svgPrinter.TabIndex = 8;
            this.svgPrinter.Text = "svgPayment";
            this.svgPrinter.Click += new System.EventHandler(this.svgPrinter_Click);
            // 
            // lbCharge
            // 
            this.lbCharge.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbCharge.AutoSize = true;
            this.lbCharge.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCharge.Location = new System.Drawing.Point(848, 13);
            this.lbCharge.Name = "lbCharge";
            this.lbCharge.Size = new System.Drawing.Size(71, 19);
            this.lbCharge.TabIndex = 9;
            this.lbCharge.Text = "CHARGE";
            // 
            // lbStatus
            // 
            this.lbStatus.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbStatus.AutoSize = true;
            this.lbStatus.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStatus.Location = new System.Drawing.Point(969, 13);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(69, 19);
            this.lbStatus.TabIndex = 10;
            this.lbStatus.Text = "STATUS";
            // 
            // svgPaymentNow
            // 
            this.svgPaymentNow.Cursor = System.Windows.Forms.Cursors.Hand;
            this.svgPaymentNow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.svgPaymentNow.Location = new System.Drawing.Point(1127, 3);
            this.svgPaymentNow.Name = "svgPaymentNow";
            this.svgPaymentNow.Size = new System.Drawing.Size(47, 40);
            this.svgPaymentNow.SizeMode = DevExpress.XtraEditors.SvgImageSizeMode.Zoom;
            this.svgPaymentNow.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgPaymentNow.SvgImage")));
            this.svgPaymentNow.TabIndex = 11;
            this.svgPaymentNow.Text = "svgPrint";
            this.svgPaymentNow.Click += new System.EventHandler(this.svgPaymentNow_Click);
            // 
            // svgEmailSms
            // 
            this.svgEmailSms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.svgEmailSms.Location = new System.Drawing.Point(1180, 3);
            this.svgEmailSms.Name = "svgEmailSms";
            this.svgEmailSms.Size = new System.Drawing.Size(47, 40);
            this.svgEmailSms.SizeMode = DevExpress.XtraEditors.SvgImageSizeMode.Zoom;
            this.svgEmailSms.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgEmailSms.SvgImage")));
            this.svgEmailSms.TabIndex = 12;
            this.svgEmailSms.Text = "svgImageBox1";
            this.svgEmailSms.Click += new System.EventHandler(this.svgEmailSms_Click);
            // 
            // svgDelete
            // 
            this.svgDelete.Location = new System.Drawing.Point(1233, 2);
            this.svgDelete.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.svgDelete.Name = "svgDelete";
            this.svgDelete.Size = new System.Drawing.Size(47, 41);
            this.svgDelete.SizeMode = DevExpress.XtraEditors.SvgImageSizeMode.Zoom;
            this.svgDelete.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgDelete.SvgImage")));
            this.svgDelete.TabIndex = 13;
            this.svgDelete.Text = "svgImageBox1";
            this.svgDelete.Click += new System.EventHandler(this.svgDelete_Click);
            // 
            // chkSelected
            // 
            this.chkSelected.AutoSize = true;
            this.chkSelected.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkSelected.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkSelected.Location = new System.Drawing.Point(1286, 3);
            this.chkSelected.Name = "chkSelected";
            this.chkSelected.Size = new System.Drawing.Size(54, 40);
            this.chkSelected.TabIndex = 14;
            this.chkSelected.UseVisualStyleBackColor = true;
            this.chkSelected.CheckedChanged += new System.EventHandler(this.chkSelected_CheckedChanged);
            // 
            // UCSaleItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tbHeader);
            this.Name = "UCSaleItem";
            this.Size = new System.Drawing.Size(1343, 46);
            this.tbHeader.ResumeLayout(false);
            this.tbHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.svgPrinter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.svgPaymentNow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.svgEmailSms)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.svgDelete)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tbHeader;
        private System.Windows.Forms.Label lbID;
        private System.Windows.Forms.Label lbDate;
        private System.Windows.Forms.Label lbCustomer;
        private System.Windows.Forms.Label lbProduct;
        private System.Windows.Forms.Label lbAmount;
        private System.Windows.Forms.Label lbCash;
        private System.Windows.Forms.Label lbCharge;
        private System.Windows.Forms.Label lbStatus;
        private DevExpress.XtraEditors.SvgImageBox svgPrinter;
        private DevExpress.XtraEditors.SvgImageBox svgPaymentNow;
        private DevExpress.XtraEditors.SvgImageBox svgEmailSms;
        private DevExpress.XtraEditors.SvgImageBox svgDelete;
        private System.Windows.Forms.CheckBox chkSelected;
    }
}
