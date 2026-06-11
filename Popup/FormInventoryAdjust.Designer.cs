namespace NailsChekin.Popup
{
    partial class FormInventoryAdjust
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInventoryAdjust));
            this.dockManager1 = new DevExpress.XtraBars.Docking.DockManager(this.components);
            this.tbHeader = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtSearchKey = new DevExpress.XtraEditors.TextEdit();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lbTitle = new System.Windows.Forms.Label();
            this.btnClose = new DevExpress.XtraEditors.SvgImageBox();
            this.panelLayout_Content = new NailsChekin.MyControls.RoundPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panelCartItemsTouch = new NailsChekin.Models.Implements.KineticScrollPanel();
            this.btnFindNow = new NailsChekin.MyControls.ButtonRound();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).BeginInit();
            this.tbHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchKey.Properties)).BeginInit();
            this.panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            this.panelLayout_Content.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dockManager1
            // 
            this.dockManager1.Form = this;
            this.dockManager1.TopZIndexControls.AddRange(new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "DevExpress.XtraBars.StandaloneBarDockControl",
            "System.Windows.Forms.StatusBar",
            "System.Windows.Forms.MenuStrip",
            "System.Windows.Forms.StatusStrip",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonControl",
            "DevExpress.XtraBars.Navigation.OfficeNavigationBar",
            "DevExpress.XtraBars.Navigation.TileNavPane",
            "DevExpress.XtraBars.TabFormControl",
            "DevExpress.XtraBars.FluentDesignSystem.FluentDesignFormControl",
            "DevExpress.XtraBars.ToolbarForm.ToolbarFormControl"});
            // 
            // tbHeader
            // 
            this.tbHeader.BackColor = System.Drawing.Color.Orange;
            this.tbHeader.ColumnCount = 7;
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tbHeader.Controls.Add(this.label1, 0, 0);
            this.tbHeader.Controls.Add(this.label2, 1, 0);
            this.tbHeader.Controls.Add(this.label3, 2, 0);
            this.tbHeader.Controls.Add(this.label4, 3, 0);
            this.tbHeader.Controls.Add(this.label5, 4, 0);
            this.tbHeader.Controls.Add(this.label6, 5, 0);
            this.tbHeader.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tbHeader.Location = new System.Drawing.Point(0, 95);
            this.tbHeader.Name = "tbHeader";
            this.tbHeader.RowCount = 1;
            this.tbHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tbHeader.Size = new System.Drawing.Size(1700, 44);
            this.tbHeader.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(68, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "ID";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(236, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 26);
            this.label2.TabIndex = 1;
            this.label2.Text = "BARCODE";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(642, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 26);
            this.label3.TabIndex = 2;
            this.label3.Text = "NAME";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(1027, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(156, 26);
            this.label4.TabIndex = 3;
            this.label4.Text = "CATEGORIES";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(1320, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 26);
            this.label5.TabIndex = 4;
            this.label5.Text = "PRICE";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(1501, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 26);
            this.label6.TabIndex = 5;
            this.label6.Text = "QTY";
            // 
            // txtSearchKey
            // 
            this.txtSearchKey.Location = new System.Drawing.Point(46, 42);
            this.txtSearchKey.Name = "txtSearchKey";
            this.txtSearchKey.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearchKey.Properties.Appearance.Options.UseFont = true;
            this.txtSearchKey.Size = new System.Drawing.Size(416, 40);
            this.txtSearchKey.TabIndex = 48;
            // 
            // labelControl7
            // 
            this.labelControl7.Appearance.Font = new System.Drawing.Font("Tahoma", 16F);
            this.labelControl7.Appearance.Options.UseFont = true;
            this.labelControl7.Location = new System.Drawing.Point(46, 10);
            this.labelControl7.Margin = new System.Windows.Forms.Padding(2);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(221, 25);
            this.labelControl7.TabIndex = 47;
            this.labelControl7.Text = "Product Name, barcode";
            // 
            // panelHeader
            // 
            this.panelHeader.Controls.Add(this.lbTitle);
            this.panelHeader.Controls.Add(this.btnClose);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Margin = new System.Windows.Forms.Padding(2);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1740, 71);
            this.panelHeader.TabIndex = 182;
            // 
            // lbTitle
            // 
            this.lbTitle.AutoSize = true;
            this.lbTitle.BackColor = System.Drawing.Color.Transparent;
            this.lbTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTitle.ForeColor = System.Drawing.Color.Red;
            this.lbTitle.Location = new System.Drawing.Point(5, 19);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(251, 39);
            this.lbTitle.TabIndex = 176;
            this.lbTitle.Text = "INVENTORYS";
            this.lbTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnClose
            // 
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Location = new System.Drawing.Point(1681, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(47, 50);
            this.btnClose.SizeMode = DevExpress.XtraEditors.SvgImageSizeMode.Stretch;
            this.btnClose.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnClose.SvgImage")));
            this.btnClose.TabIndex = 179;
            this.btnClose.Text = "svgCustomerReload";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panelLayout_Content
            // 
            this.panelLayout_Content.BackColor = System.Drawing.Color.Transparent;
            this.panelLayout_Content.Controls.Add(this.panelCartItemsTouch);
            this.panelLayout_Content.Controls.Add(this.panel1);
            this.panelLayout_Content.Location = new System.Drawing.Point(7, 77);
            this.panelLayout_Content.Name = "panelLayout_Content";
            this.panelLayout_Content.Padding = new System.Windows.Forms.Padding(8);
            this.panelLayout_Content.Size = new System.Drawing.Size(1716, 693);
            this.panelLayout_Content.TabIndex = 183;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnFindNow);
            this.panel1.Controls.Add(this.txtSearchKey);
            this.panel1.Controls.Add(this.labelControl7);
            this.panel1.Controls.Add(this.tbHeader);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(8, 8);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1700, 139);
            this.panel1.TabIndex = 0;
            // 
            // panelCartItemsTouch
            // 
            this.panelCartItemsTouch.AutoRefreshLayoutBounds = true;
            // 
            // 
            // 
            this.panelCartItemsTouch.Content.Location = new System.Drawing.Point(0, 0);
            this.panelCartItemsTouch.Content.Name = "";
            this.panelCartItemsTouch.Content.TabIndex = 0;
            this.panelCartItemsTouch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCartItemsTouch.EnableHorizontal = false;
            this.panelCartItemsTouch.EnableVertical = true;
            this.panelCartItemsTouch.Friction = 0.92F;
            this.panelCartItemsTouch.HardStopEdges = true;
            this.panelCartItemsTouch.Location = new System.Drawing.Point(8, 147);
            this.panelCartItemsTouch.Name = "panelCartItemsTouch";
            this.panelCartItemsTouch.Size = new System.Drawing.Size(1700, 538);
            this.panelCartItemsTouch.TabIndex = 1;
            // 
            // btnFindNow
            // 
            this.btnFindNow.BackColor = System.Drawing.Color.Transparent;
            this.btnFindNow.BorderColor = System.Drawing.Color.SteelBlue;
            this.btnFindNow.ButtonPadding = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.btnFindNow.ClickLocked = false;
            this.btnFindNow.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFindNow.DisabledOverlayColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnFindNow.DisabledOverlayFont = null;
            this.btnFindNow.DisabledOverlayForeColor = System.Drawing.Color.White;
            this.btnFindNow.DisabledOverlayText = "Processing...";
            this.btnFindNow.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFindNow.Location = new System.Drawing.Point(503, 12);
            this.btnFindNow.MinimumSize = new System.Drawing.Size(120, 36);
            this.btnFindNow.Name = "btnFindNow";
            this.btnFindNow.Selected = false;
            this.btnFindNow.Size = new System.Drawing.Size(318, 72);
            this.btnFindNow.TabIndex = 183;
            this.btnFindNow.Title = "FIND NOW";
            this.btnFindNow.TitleBackColor = System.Drawing.Color.SteelBlue;
            this.btnFindNow.Click += new System.EventHandler(this.btnFindNow_Click);
            // 
            // FormInventoryAdjust
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1740, 782);
            this.Controls.Add(this.panelLayout_Content);
            this.Controls.Add(this.panelHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormInventoryAdjust";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "INVENTORY";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormInventoryAdjust_FormClosed);
            this.Load += new System.EventHandler(this.FormTipsAdjust_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
            this.tbHeader.ResumeLayout(false);
            this.tbHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSearchKey.Properties)).EndInit();
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            this.panelLayout_Content.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraBars.Docking.DockManager dockManager1;
        private System.Windows.Forms.TableLayoutPanel tbHeader;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private DevExpress.XtraEditors.TextEdit txtSearchKey;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lbTitle;
        private DevExpress.XtraEditors.SvgImageBox btnClose;
        private MyControls.RoundPanel panelLayout_Content;
        private System.Windows.Forms.Panel panel1;
        private Models.Implements.KineticScrollPanel panelCartItemsTouch;
        private MyControls.ButtonRound btnFindNow;
    }
}