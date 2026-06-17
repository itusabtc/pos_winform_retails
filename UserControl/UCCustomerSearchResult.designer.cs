namespace NailsChekin.UserControl
{
    partial class UCCustomerSearchResult
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCCustomerSearchResult));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.svgSelect = new DevExpress.XtraEditors.SvgImageBox();
            this.lbPhone = new System.Windows.Forms.Label();
            this.lbFirstname = new System.Windows.Forms.Label();
            this.lbLastname = new System.Windows.Forms.Label();
            this.svgInfo = new DevExpress.XtraEditors.SvgImageBox();
            this.lbBirthday = new System.Windows.Forms.Label();
            this.lbAddress = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.svgSelect)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.svgInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.FloralWhite;
            this.tableLayoutPanel1.ColumnCount = 7;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.Controls.Add(this.svgSelect, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbPhone, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbFirstname, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbLastname, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.svgInfo, 6, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbBirthday, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbAddress, 5, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Font = new System.Drawing.Font("Tahoma", 9F);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1065, 49);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // svgSelect
            // 
            this.svgSelect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.svgSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.svgSelect.Location = new System.Drawing.Point(2, 2);
            this.svgSelect.Margin = new System.Windows.Forms.Padding(2);
            this.svgSelect.Name = "svgSelect";
            this.svgSelect.Size = new System.Drawing.Size(49, 45);
            this.svgSelect.SizeMode = DevExpress.XtraEditors.SvgImageSizeMode.Zoom;
            this.svgSelect.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgSelect.SvgImage")));
            this.svgSelect.TabIndex = 5;
            this.svgSelect.Text = "svgIcon";
            this.svgSelect.Click += new System.EventHandler(this.svgSelect_Click);
            // 
            // lbPhone
            // 
            this.lbPhone.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lbPhone.AutoSize = true;
            this.lbPhone.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPhone.Location = new System.Drawing.Point(56, 10);
            this.lbPhone.Name = "lbPhone";
            this.lbPhone.Size = new System.Drawing.Size(78, 29);
            this.lbPhone.TabIndex = 6;
            this.lbPhone.Text = "Phone";
            // 
            // lbFirstname
            // 
            this.lbFirstname.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbFirstname.AutoSize = true;
            this.lbFirstname.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFirstname.Location = new System.Drawing.Point(232, 10);
            this.lbFirstname.Name = "lbFirstname";
            this.lbFirstname.Size = new System.Drawing.Size(118, 29);
            this.lbFirstname.TabIndex = 7;
            this.lbFirstname.Text = "Firstname";
            // 
            // lbLastname
            // 
            this.lbLastname.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbLastname.AutoSize = true;
            this.lbLastname.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLastname.Location = new System.Drawing.Point(392, 10);
            this.lbLastname.Name = "lbLastname";
            this.lbLastname.Size = new System.Drawing.Size(116, 29);
            this.lbLastname.TabIndex = 8;
            this.lbLastname.Text = "Lastname";
            // 
            // svgInfo
            // 
            this.svgInfo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.svgInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.svgInfo.Location = new System.Drawing.Point(1011, 3);
            this.svgInfo.Name = "svgInfo";
            this.svgInfo.Size = new System.Drawing.Size(51, 43);
            this.svgInfo.SizeMode = DevExpress.XtraEditors.SvgImageSizeMode.Stretch;
            this.svgInfo.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgInfo.SvgImage")));
            this.svgInfo.TabIndex = 9;
            this.svgInfo.Text = "svgImageBox1";
            this.svgInfo.Click += new System.EventHandler(this.svgInfo_Click);
            // 
            // lbBirthday
            // 
            this.lbBirthday.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbBirthday.AutoSize = true;
            this.lbBirthday.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbBirthday.Location = new System.Drawing.Point(559, 10);
            this.lbBirthday.Name = "lbBirthday";
            this.lbBirthday.Size = new System.Drawing.Size(100, 29);
            this.lbBirthday.TabIndex = 10;
            this.lbBirthday.Text = "Birthday";
            // 
            // lbAddress
            // 
            this.lbAddress.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbAddress.AutoSize = true;
            this.lbAddress.Font = new System.Drawing.Font("Tahoma", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAddress.Location = new System.Drawing.Point(800, 10);
            this.lbAddress.Name = "lbAddress";
            this.lbAddress.Size = new System.Drawing.Size(97, 29);
            this.lbAddress.TabIndex = 11;
            this.lbAddress.Text = "Address";
            // 
            // UCCustomerSearchResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "UCCustomerSearchResult";
            this.Size = new System.Drawing.Size(1065, 49);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.svgSelect)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.svgInfo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraEditors.SvgImageBox svgSelect;
        private System.Windows.Forms.Label lbPhone;
        private System.Windows.Forms.Label lbFirstname;
        private System.Windows.Forms.Label lbLastname;
        private DevExpress.XtraEditors.SvgImageBox svgInfo;
        private System.Windows.Forms.Label lbBirthday;
        private System.Windows.Forms.Label lbAddress;
    }
}
