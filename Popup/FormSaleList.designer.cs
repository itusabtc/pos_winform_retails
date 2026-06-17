namespace NailsChekin.Popup
{
    partial class FormSaleList
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSaleList));
            this.txtFromDate = new DevExpress.XtraEditors.TextEdit();
            this.txtSearchCustomer = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.ddlPaidBySearch = new DevExpress.XtraEditors.DropDownButton();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl24 = new DevExpress.XtraEditors.LabelControl();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.txtSearchReceipt = new DevExpress.XtraEditors.TextEdit();
            this.panelTicketsTouch = new NailsChekin.Models.Implements.KineticScrollPanel();
            this.tbHeader = new System.Windows.Forms.TableLayoutPanel();
            this.panel_Content = new System.Windows.Forms.Panel();
            this.txtToDate = new DevExpress.XtraEditors.TextEdit();
            this.panelLayout_Content = new NailsChekin.MyControls.RoundPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnFindNow = new NailsChekin.MyControls.ButtonRound();
            this.btnDeleteSelected = new NailsChekin.MyControls.ButtonRound();
            this.panelLayout_Header = new System.Windows.Forms.Panel();
            this.lbTitle = new System.Windows.Forms.Label();
            this.btnClose = new DevExpress.XtraEditors.SvgImageBox();
            ((System.ComponentModel.ISupportInitialize)(this.txtFromDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchCustomer.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchReceipt.Properties)).BeginInit();
            this.tbHeader.SuspendLayout();
            this.panel_Content.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtToDate.Properties)).BeginInit();
            this.panelLayout_Content.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelLayout_Header.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            this.SuspendLayout();
            // 
            // txtFromDate
            // 
            this.txtFromDate.Location = new System.Drawing.Point(18, 46);
            this.txtFromDate.Name = "txtFromDate";
            this.txtFromDate.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFromDate.Properties.Appearance.Options.UseFont = true;
            this.txtFromDate.Size = new System.Drawing.Size(140, 36);
            this.txtFromDate.TabIndex = 68;
            this.txtFromDate.Click += new System.EventHandler(this.txtFromDate_Click);
            // 
            // txtSearchCustomer
            // 
            this.txtSearchCustomer.Location = new System.Drawing.Point(526, 46);
            this.txtSearchCustomer.Name = "txtSearchCustomer";
            this.txtSearchCustomer.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearchCustomer.Properties.Appearance.Options.UseFont = true;
            this.txtSearchCustomer.Size = new System.Drawing.Size(190, 36);
            this.txtSearchCustomer.TabIndex = 64;
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(18, 17);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(98, 25);
            this.labelControl1.TabIndex = 66;
            this.labelControl1.Text = "From Date";
            // 
            // labelControl7
            // 
            this.labelControl7.Appearance.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl7.Appearance.Options.UseFont = true;
            this.labelControl7.Location = new System.Drawing.Point(526, 16);
            this.labelControl7.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(89, 25);
            this.labelControl7.TabIndex = 63;
            this.labelControl7.Text = "Customer";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(51, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "ID";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(170, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 23);
            this.label2.TabIndex = 1;
            this.label2.Text = "DATE";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(293, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 23);
            this.label3.TabIndex = 2;
            this.label3.Text = "CUSTOMER";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(526, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 23);
            this.label4.TabIndex = 3;
            this.label4.Text = "PRODUCTS";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(766, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 23);
            this.label5.TabIndex = 4;
            this.label5.Text = "AMOUNT";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(929, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 23);
            this.label6.TabIndex = 5;
            this.label6.Text = "CASH";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(1048, 10);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 23);
            this.label7.TabIndex = 6;
            this.label7.Text = "CHARGE";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(1199, 10);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 23);
            this.label8.TabIndex = 7;
            this.label8.Text = "STATUS";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Location = new System.Drawing.Point(751, 16);
            this.labelControl3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(91, 25);
            this.labelControl3.TabIndex = 65;
            this.labelControl3.Text = "Receipt #";
            // 
            // ddlPaidBySearch
            // 
            this.ddlPaidBySearch.Appearance.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ddlPaidBySearch.Appearance.Options.UseFont = true;
            this.ddlPaidBySearch.Location = new System.Drawing.Point(353, 46);
            this.ddlPaidBySearch.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ddlPaidBySearch.Name = "ddlPaidBySearch";
            this.ddlPaidBySearch.Size = new System.Drawing.Size(140, 34);
            this.ddlPaidBySearch.TabIndex = 74;
            this.ddlPaidBySearch.Click += new System.EventHandler(this.ddlPaidBySearch_Click);
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Location = new System.Drawing.Point(187, 16);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(73, 25);
            this.labelControl2.TabIndex = 70;
            this.labelControl2.Text = "To Date";
            // 
            // labelControl24
            // 
            this.labelControl24.Appearance.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl24.Appearance.Options.UseFont = true;
            this.labelControl24.Location = new System.Drawing.Point(355, 17);
            this.labelControl24.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.labelControl24.Name = "labelControl24";
            this.labelControl24.Size = new System.Drawing.Size(70, 25);
            this.labelControl24.TabIndex = 73;
            this.labelControl24.Text = "Paid By";
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkSelectAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkSelectAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkSelectAll.Location = new System.Drawing.Point(1587, 3);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(63, 38);
            this.chkSelectAll.TabIndex = 8;
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // txtSearchReceipt
            // 
            this.txtSearchReceipt.Location = new System.Drawing.Point(751, 46);
            this.txtSearchReceipt.Name = "txtSearchReceipt";
            this.txtSearchReceipt.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearchReceipt.Properties.Appearance.Options.UseFont = true;
            this.txtSearchReceipt.Size = new System.Drawing.Size(140, 36);
            this.txtSearchReceipt.TabIndex = 67;
            // 
            // panelTicketsTouch
            // 
            this.panelTicketsTouch.AutoRefreshLayoutBounds = true;
            // 
            // 
            // 
            this.panelTicketsTouch.Content.Location = new System.Drawing.Point(0, 0);
            this.panelTicketsTouch.Content.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelTicketsTouch.Content.Name = "";
            this.panelTicketsTouch.Content.Size = new System.Drawing.Size(150, 81);
            this.panelTicketsTouch.Content.TabIndex = 0;
            this.panelTicketsTouch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTicketsTouch.EnableHorizontal = false;
            this.panelTicketsTouch.EnableVertical = true;
            this.panelTicketsTouch.Friction = 0.92F;
            this.panelTicketsTouch.HardStopEdges = true;
            this.panelTicketsTouch.Location = new System.Drawing.Point(0, 44);
            this.panelTicketsTouch.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelTicketsTouch.Name = "panelTicketsTouch";
            this.panelTicketsTouch.Size = new System.Drawing.Size(1653, 487);
            this.panelTicketsTouch.TabIndex = 2;
            // 
            // tbHeader
            // 
            this.tbHeader.BackColor = System.Drawing.Color.Orange;
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
            this.tbHeader.Controls.Add(this.label1, 0, 0);
            this.tbHeader.Controls.Add(this.label2, 1, 0);
            this.tbHeader.Controls.Add(this.label3, 2, 0);
            this.tbHeader.Controls.Add(this.label4, 3, 0);
            this.tbHeader.Controls.Add(this.label5, 4, 0);
            this.tbHeader.Controls.Add(this.label6, 5, 0);
            this.tbHeader.Controls.Add(this.label7, 6, 0);
            this.tbHeader.Controls.Add(this.label8, 7, 0);
            this.tbHeader.Controls.Add(this.chkSelectAll, 12, 0);
            this.tbHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.tbHeader.Location = new System.Drawing.Point(0, 0);
            this.tbHeader.Name = "tbHeader";
            this.tbHeader.RowCount = 1;
            this.tbHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tbHeader.Size = new System.Drawing.Size(1653, 44);
            this.tbHeader.TabIndex = 1;
            // 
            // panel_Content
            // 
            this.panel_Content.Controls.Add(this.panelTicketsTouch);
            this.panel_Content.Controls.Add(this.tbHeader);
            this.panel_Content.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_Content.Location = new System.Drawing.Point(6, 92);
            this.panel_Content.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel_Content.Name = "panel_Content";
            this.panel_Content.Size = new System.Drawing.Size(1653, 531);
            this.panel_Content.TabIndex = 75;
            // 
            // txtToDate
            // 
            this.txtToDate.Location = new System.Drawing.Point(187, 46);
            this.txtToDate.Name = "txtToDate";
            this.txtToDate.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtToDate.Properties.Appearance.Options.UseFont = true;
            this.txtToDate.Size = new System.Drawing.Size(140, 36);
            this.txtToDate.TabIndex = 72;
            this.txtToDate.Click += new System.EventHandler(this.txtToDate_Click);
            // 
            // panelLayout_Content
            // 
            this.panelLayout_Content.BackColor = System.Drawing.Color.Transparent;
            this.panelLayout_Content.Controls.Add(this.panel1);
            this.panelLayout_Content.Controls.Add(this.panel_Content);
            this.panelLayout_Content.Controls.Add(this.txtSearchReceipt);
            this.panelLayout_Content.Controls.Add(this.txtToDate);
            this.panelLayout_Content.Controls.Add(this.labelControl3);
            this.panelLayout_Content.Controls.Add(this.ddlPaidBySearch);
            this.panelLayout_Content.Controls.Add(this.labelControl2);
            this.panelLayout_Content.Controls.Add(this.labelControl24);
            this.panelLayout_Content.Controls.Add(this.txtFromDate);
            this.panelLayout_Content.Controls.Add(this.txtSearchCustomer);
            this.panelLayout_Content.Controls.Add(this.labelControl1);
            this.panelLayout_Content.Controls.Add(this.labelControl7);
            this.panelLayout_Content.Location = new System.Drawing.Point(4, 77);
            this.panelLayout_Content.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelLayout_Content.Name = "panelLayout_Content";
            this.panelLayout_Content.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.panelLayout_Content.Size = new System.Drawing.Size(1665, 629);
            this.panelLayout_Content.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnFindNow);
            this.panel1.Controls.Add(this.btnDeleteSelected);
            this.panel1.Location = new System.Drawing.Point(915, 8);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(740, 78);
            this.panel1.TabIndex = 76;
            // 
            // btnFindNow
            // 
            this.btnFindNow.BackColor = System.Drawing.Color.Transparent;
            this.btnFindNow.BorderColor = System.Drawing.Color.Green;
            this.btnFindNow.ButtonPadding = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.btnFindNow.ClickLocked = false;
            this.btnFindNow.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFindNow.DisabledOverlayColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnFindNow.DisabledOverlayFont = null;
            this.btnFindNow.DisabledOverlayForeColor = System.Drawing.Color.White;
            this.btnFindNow.DisabledOverlayText = "Processing...";
            this.btnFindNow.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFindNow.Location = new System.Drawing.Point(8, 3);
            this.btnFindNow.MinimumSize = new System.Drawing.Size(120, 36);
            this.btnFindNow.Name = "btnFindNow";
            this.btnFindNow.Selected = false;
            this.btnFindNow.Size = new System.Drawing.Size(319, 71);
            this.btnFindNow.TabIndex = 195;
            this.btnFindNow.Title = "FIND NOW";
            this.btnFindNow.TitleBackColor = System.Drawing.Color.Green;
            this.btnFindNow.TitleFontSize = 22F;
            this.btnFindNow.Click += new System.EventHandler(this.btnFindNow_Click);
            // 
            // btnDeleteSelected
            // 
            this.btnDeleteSelected.BackColor = System.Drawing.Color.Transparent;
            this.btnDeleteSelected.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnDeleteSelected.ButtonPadding = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.btnDeleteSelected.ClickLocked = false;
            this.btnDeleteSelected.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDeleteSelected.DisabledOverlayColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnDeleteSelected.DisabledOverlayFont = null;
            this.btnDeleteSelected.DisabledOverlayForeColor = System.Drawing.Color.White;
            this.btnDeleteSelected.DisabledOverlayText = "Processing...";
            this.btnDeleteSelected.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleteSelected.Location = new System.Drawing.Point(356, 5);
            this.btnDeleteSelected.MinimumSize = new System.Drawing.Size(120, 36);
            this.btnDeleteSelected.Name = "btnDeleteSelected";
            this.btnDeleteSelected.Selected = false;
            this.btnDeleteSelected.Size = new System.Drawing.Size(381, 68);
            this.btnDeleteSelected.TabIndex = 194;
            this.btnDeleteSelected.Title = "DELETE SELECTED";
            this.btnDeleteSelected.TitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnDeleteSelected.TitleFontSize = 22F;
            this.btnDeleteSelected.Visible = false;
            this.btnDeleteSelected.Click += new System.EventHandler(this.btnDeleteSelected_Click);
            // 
            // panelLayout_Header
            // 
            this.panelLayout_Header.Controls.Add(this.lbTitle);
            this.panelLayout_Header.Controls.Add(this.btnClose);
            this.panelLayout_Header.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLayout_Header.Location = new System.Drawing.Point(0, 0);
            this.panelLayout_Header.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelLayout_Header.Name = "panelLayout_Header";
            this.panelLayout_Header.Size = new System.Drawing.Size(1677, 74);
            this.panelLayout_Header.TabIndex = 185;
            // 
            // lbTitle
            // 
            this.lbTitle.AutoSize = true;
            this.lbTitle.BackColor = System.Drawing.Color.Transparent;
            this.lbTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTitle.ForeColor = System.Drawing.Color.Red;
            this.lbTitle.Location = new System.Drawing.Point(5, 19);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(195, 39);
            this.lbTitle.TabIndex = 176;
            this.lbTitle.Text = "SALE LIST";
            this.lbTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnClose
            // 
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Location = new System.Drawing.Point(1583, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(47, 50);
            this.btnClose.SizeMode = DevExpress.XtraEditors.SvgImageSizeMode.Stretch;
            this.btnClose.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnClose.SvgImage")));
            this.btnClose.TabIndex = 179;
            this.btnClose.Text = "svgCustomerReload";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FormSaleList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1677, 718);
            this.Controls.Add(this.panelLayout_Header);
            this.Controls.Add(this.panelLayout_Content);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSaleList";
            this.Text = "SALE LIST";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormSaleList_FormClosed);
            this.Load += new System.EventHandler(this.FormSaleList_Load);
            this.Shown += new System.EventHandler(this.FormSaleList_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.txtFromDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchCustomer.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchReceipt.Properties)).EndInit();
            this.tbHeader.ResumeLayout(false);
            this.tbHeader.PerformLayout();
            this.panel_Content.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtToDate.Properties)).EndInit();
            this.panelLayout_Content.ResumeLayout(false);
            this.panelLayout_Content.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panelLayout_Header.ResumeLayout(false);
            this.panelLayout_Header.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.TextEdit txtFromDate;
        private DevExpress.XtraEditors.TextEdit txtSearchCustomer;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.DropDownButton ddlPaidBySearch;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl24;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private DevExpress.XtraEditors.TextEdit txtSearchReceipt;
        private Models.Implements.KineticScrollPanel panelTicketsTouch;
        private System.Windows.Forms.TableLayoutPanel tbHeader;
        private System.Windows.Forms.Panel panel_Content;
        private DevExpress.XtraEditors.TextEdit txtToDate;
        private MyControls.RoundPanel panelLayout_Content;
        private System.Windows.Forms.Panel panelLayout_Header;
        private System.Windows.Forms.Label lbTitle;
        private DevExpress.XtraEditors.SvgImageBox btnClose;
        private System.Windows.Forms.Panel panel1;
        private MyControls.ButtonRound btnDeleteSelected;
        private MyControls.ButtonRound btnFindNow;
    }
}