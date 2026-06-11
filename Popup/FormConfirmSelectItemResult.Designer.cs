namespace NailsChekin.Popup
{
    partial class FormConfirmSelectItemResult
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormConfirmSelectItemResult));
            this.btnClose = new DevExpress.XtraEditors.SvgImageBox();
            this.panelContent = new System.Windows.Forms.Panel();
            this.panelLeft = new NailsChekin.MyControls.RoundPanel();
            this.panelCartItemsTouch = new NailsChekin.Models.Implements.KineticScrollPanel();
            this.tableHeader = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnFinish = new NailsChekin.MyControls.ButtonRound();
            this.btnCancel = new NailsChekin.MyControls.ButtonRound();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lbTitle = new System.Windows.Forms.Label();
            this.panelFooter = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            this.panelContent.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.tableHeader.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.panelFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Location = new System.Drawing.Point(1297, 4);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(63, 61);
            this.btnClose.SizeMode = DevExpress.XtraEditors.SvgImageSizeMode.Stretch;
            this.btnClose.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnClose.SvgImage")));
            this.btnClose.TabIndex = 179;
            this.btnClose.Text = "svgCustomerReload";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panelContent
            // 
            this.panelContent.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panelContent.Controls.Add(this.panelLeft);
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(0, 85);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(1371, 605);
            this.panelContent.TabIndex = 183;
            // 
            // panelLeft
            // 
            this.panelLeft.BackColor = System.Drawing.Color.Transparent;
            this.panelLeft.BorderColor = System.Drawing.Color.WhiteSmoke;
            this.panelLeft.Controls.Add(this.panelCartItemsTouch);
            this.panelLeft.Controls.Add(this.tableHeader);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Margin = new System.Windows.Forms.Padding(4);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Padding = new System.Windows.Forms.Padding(11, 10, 11, 10);
            this.panelLeft.Size = new System.Drawing.Size(1371, 605);
            this.panelLeft.TabIndex = 175;
            // 
            // panelCartItemsTouch
            // 
            this.panelCartItemsTouch.AutoRefreshLayoutBounds = true;
            this.panelCartItemsTouch.BackColor = System.Drawing.Color.WhiteSmoke;
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
            this.panelCartItemsTouch.Location = new System.Drawing.Point(11, 61);
            this.panelCartItemsTouch.Name = "panelCartItemsTouch";
            this.panelCartItemsTouch.Size = new System.Drawing.Size(1349, 534);
            this.panelCartItemsTouch.TabIndex = 0;
            // 
            // tableHeader
            // 
            this.tableHeader.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tableHeader.ColumnCount = 6;
            this.tableHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tableHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableHeader.Controls.Add(this.label1, 0, 0);
            this.tableHeader.Controls.Add(this.label2, 1, 0);
            this.tableHeader.Controls.Add(this.label3, 2, 0);
            this.tableHeader.Controls.Add(this.label5, 3, 0);
            this.tableHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableHeader.Location = new System.Drawing.Point(11, 10);
            this.tableHeader.Name = "tableHeader";
            this.tableHeader.RowCount = 1;
            this.tableHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableHeader.Size = new System.Drawing.Size(1349, 51);
            this.tableHeader.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(674, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 29);
            this.label2.TabIndex = 1;
            this.label2.Text = "QTY";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(865, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 29);
            this.label3.TabIndex = 2;
            this.label3.Text = "PRICE";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(1050, 11);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(124, 29);
            this.label5.TabIndex = 3;
            this.label5.Text = "AMOUNT";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnFinish
            // 
            this.btnFinish.BackColor = System.Drawing.Color.Transparent;
            this.btnFinish.BorderColor = System.Drawing.Color.Green;
            this.btnFinish.ButtonPadding = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.btnFinish.ClickLocked = false;
            this.btnFinish.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFinish.DisabledOverlayColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnFinish.DisabledOverlayFont = null;
            this.btnFinish.DisabledOverlayForeColor = System.Drawing.Color.White;
            this.btnFinish.DisabledOverlayText = "Processing...";
            this.btnFinish.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFinish.Location = new System.Drawing.Point(825, 8);
            this.btnFinish.Margin = new System.Windows.Forms.Padding(4);
            this.btnFinish.MinimumSize = new System.Drawing.Size(160, 44);
            this.btnFinish.Name = "btnFinish";
            this.btnFinish.Selected = false;
            this.btnFinish.Size = new System.Drawing.Size(535, 75);
            this.btnFinish.TabIndex = 184;
            this.btnFinish.Title = "FINISH";
            this.btnFinish.TitleBackColor = System.Drawing.Color.Green;
            this.btnFinish.Click += new System.EventHandler(this.btnFinish_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnCancel.ButtonPadding = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.btnCancel.ClickLocked = false;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.DisabledOverlayColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnCancel.DisabledOverlayFont = null;
            this.btnCancel.DisabledOverlayForeColor = System.Drawing.Color.White;
            this.btnCancel.DisabledOverlayText = "Processing...";
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(13, 8);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.MinimumSize = new System.Drawing.Size(160, 44);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Selected = false;
            this.btnCancel.Size = new System.Drawing.Size(270, 75);
            this.btnCancel.TabIndex = 183;
            this.btnCancel.Title = "CANCEL";
            this.btnCancel.TitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // panelHeader
            // 
            this.panelHeader.Controls.Add(this.lbTitle);
            this.panelHeader.Controls.Add(this.btnClose);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1371, 85);
            this.panelHeader.TabIndex = 184;
            // 
            // lbTitle
            // 
            this.lbTitle.AutoSize = true;
            this.lbTitle.BackColor = System.Drawing.Color.Transparent;
            this.lbTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTitle.ForeColor = System.Drawing.Color.Red;
            this.lbTitle.Location = new System.Drawing.Point(7, 23);
            this.lbTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(351, 52);
            this.lbTitle.TabIndex = 176;
            this.lbTitle.Text = "SELECT ITEMS";
            this.lbTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelFooter
            // 
            this.panelFooter.Controls.Add(this.btnFinish);
            this.panelFooter.Controls.Add(this.btnCancel);
            this.panelFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelFooter.Location = new System.Drawing.Point(0, 690);
            this.panelFooter.Name = "panelFooter";
            this.panelFooter.Size = new System.Drawing.Size(1371, 90);
            this.panelFooter.TabIndex = 182;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(226, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "ITEM NAME";
            // 
            // FormConfirmSelectItemResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1371, 780);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.panelFooter);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormConfirmSelectItemResult";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormConfirmSelectItemResult";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormConfirmSelectItemResult_FormClosed);
            this.Load += new System.EventHandler(this.FormConfirmSelectItemResult_Load);
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            this.panelContent.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            this.tableHeader.ResumeLayout(false);
            this.tableHeader.PerformLayout();
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelFooter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SvgImageBox btnClose;
        private System.Windows.Forms.Panel panelContent;
        private MyControls.RoundPanel panelLeft;
        private Models.Implements.KineticScrollPanel panelCartItemsTouch;
        private System.Windows.Forms.TableLayoutPanel tableHeader;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private MyControls.ButtonRound btnFinish;
        private MyControls.ButtonRound btnCancel;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.Panel panelFooter;
        private System.Windows.Forms.Label label1;
    }
}