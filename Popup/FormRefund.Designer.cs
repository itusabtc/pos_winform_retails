namespace NailsChekin.Popup
{
    partial class FormRefund
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRefund));
            this.lbDate = new System.Windows.Forms.Label();
            this.lbCustomer = new System.Windows.Forms.Label();
            this.lbStatus = new System.Windows.Forms.Label();
            this.lbTotal = new System.Windows.Forms.Label();
            this.lbPaid = new System.Windows.Forms.Label();
            this.imgRow = new System.Windows.Forms.ImageList(this.components);
            this.panelLayout_Header = new System.Windows.Forms.Panel();
            this.lbTitle = new System.Windows.Forms.Label();
            this.btnClose = new DevExpress.XtraEditors.SvgImageBox();
            this.panelLayout_Content = new NailsChekin.MyControls.RoundPanel();
            this.panelCartItemsTouch = new NailsChekin.Models.Implements.KineticScrollPanel();
            this.tbHeader = new System.Windows.Forms.TableLayoutPanel();
            this.lbName = new System.Windows.Forms.Label();
            this.lbQty = new System.Windows.Forms.Label();
            this.lbPrice = new System.Windows.Forms.Label();
            this.lbDiscount = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.chkSelected = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panelFooter = new System.Windows.Forms.Panel();
            this.btnRefundAll = new NailsChekin.MyControls.ButtonRound();
            this.btnRefundSelected = new NailsChekin.MyControls.ButtonRound();
            this.panelLayout_Header.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            this.panelLayout_Content.SuspendLayout();
            this.tbHeader.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panelFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbDate
            // 
            this.lbDate.AutoSize = true;
            this.lbDate.Font = new System.Drawing.Font("Tahoma", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDate.Location = new System.Drawing.Point(34, 14);
            this.lbDate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbDate.Name = "lbDate";
            this.lbDate.Size = new System.Drawing.Size(66, 27);
            this.lbDate.TabIndex = 1;
            this.lbDate.Text = "Date:";
            // 
            // lbCustomer
            // 
            this.lbCustomer.AutoSize = true;
            this.lbCustomer.Font = new System.Drawing.Font("Tahoma", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCustomer.Location = new System.Drawing.Point(336, 14);
            this.lbCustomer.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbCustomer.Name = "lbCustomer";
            this.lbCustomer.Size = new System.Drawing.Size(112, 27);
            this.lbCustomer.TabIndex = 2;
            this.lbCustomer.Text = "Customer:";
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.Font = new System.Drawing.Font("Tahoma", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbStatus.Location = new System.Drawing.Point(34, 65);
            this.lbStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(80, 27);
            this.lbStatus.TabIndex = 3;
            this.lbStatus.Text = "Status:";
            // 
            // lbTotal
            // 
            this.lbTotal.AutoSize = true;
            this.lbTotal.Font = new System.Drawing.Font("Tahoma", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTotal.Location = new System.Drawing.Point(336, 65);
            this.lbTotal.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbTotal.Name = "lbTotal";
            this.lbTotal.Size = new System.Drawing.Size(69, 27);
            this.lbTotal.TabIndex = 4;
            this.lbTotal.Text = "Total:";
            // 
            // lbPaid
            // 
            this.lbPaid.AutoSize = true;
            this.lbPaid.Font = new System.Drawing.Font("Tahoma", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPaid.Location = new System.Drawing.Point(554, 65);
            this.lbPaid.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbPaid.Name = "lbPaid";
            this.lbPaid.Size = new System.Drawing.Size(61, 27);
            this.lbPaid.TabIndex = 5;
            this.lbPaid.Text = "Paid:";
            // 
            // imgRow
            // 
            this.imgRow.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imgRow.ImageSize = new System.Drawing.Size(1, 40);
            this.imgRow.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // panelLayout_Header
            // 
            this.panelLayout_Header.Controls.Add(this.lbTitle);
            this.panelLayout_Header.Controls.Add(this.btnClose);
            this.panelLayout_Header.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLayout_Header.Location = new System.Drawing.Point(0, 0);
            this.panelLayout_Header.Margin = new System.Windows.Forms.Padding(2);
            this.panelLayout_Header.Name = "panelLayout_Header";
            this.panelLayout_Header.Size = new System.Drawing.Size(1168, 71);
            this.panelLayout_Header.TabIndex = 186;
            // 
            // lbTitle
            // 
            this.lbTitle.AutoSize = true;
            this.lbTitle.BackColor = System.Drawing.Color.Transparent;
            this.lbTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTitle.ForeColor = System.Drawing.Color.Red;
            this.lbTitle.Location = new System.Drawing.Point(5, 19);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(167, 39);
            this.lbTitle.TabIndex = 176;
            this.lbTitle.Text = "REFUND";
            this.lbTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnClose
            // 
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Location = new System.Drawing.Point(1055, 6);
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
            this.panelLayout_Content.Controls.Add(this.tbHeader);
            this.panelLayout_Content.Controls.Add(this.panel2);
            this.panelLayout_Content.Controls.Add(this.panelFooter);
            this.panelLayout_Content.Location = new System.Drawing.Point(12, 80);
            this.panelLayout_Content.Margin = new System.Windows.Forms.Padding(2);
            this.panelLayout_Content.Name = "panelLayout_Content";
            this.panelLayout_Content.Padding = new System.Windows.Forms.Padding(6);
            this.panelLayout_Content.Size = new System.Drawing.Size(1145, 718);
            this.panelLayout_Content.TabIndex = 187;
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
            this.panelCartItemsTouch.Location = new System.Drawing.Point(6, 176);
            this.panelCartItemsTouch.Name = "panelCartItemsTouch";
            this.panelCartItemsTouch.Size = new System.Drawing.Size(1133, 455);
            this.panelCartItemsTouch.TabIndex = 182;
            // 
            // tbHeader
            // 
            this.tbHeader.BackColor = System.Drawing.Color.Orange;
            this.tbHeader.ColumnCount = 6;
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 38.09524F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.523809F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tbHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.523809F));
            this.tbHeader.Controls.Add(this.lbName, 0, 0);
            this.tbHeader.Controls.Add(this.lbQty, 1, 0);
            this.tbHeader.Controls.Add(this.lbPrice, 2, 0);
            this.tbHeader.Controls.Add(this.lbDiscount, 3, 0);
            this.tbHeader.Controls.Add(this.label1, 4, 0);
            this.tbHeader.Controls.Add(this.chkSelected, 5, 0);
            this.tbHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.tbHeader.Location = new System.Drawing.Point(6, 125);
            this.tbHeader.Margin = new System.Windows.Forms.Padding(2);
            this.tbHeader.Name = "tbHeader";
            this.tbHeader.RowCount = 1;
            this.tbHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tbHeader.Size = new System.Drawing.Size(1133, 51);
            this.tbHeader.TabIndex = 181;
            // 
            // lbName
            // 
            this.lbName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbName.AutoSize = true;
            this.lbName.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbName.Location = new System.Drawing.Point(185, 13);
            this.lbName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(60, 25);
            this.lbName.TabIndex = 0;
            this.lbName.Text = "ITEM";
            // 
            // lbQty
            // 
            this.lbQty.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbQty.AutoSize = true;
            this.lbQty.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbQty.Location = new System.Drawing.Point(459, 13);
            this.lbQty.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbQty.Name = "lbQty";
            this.lbQty.Size = new System.Drawing.Size(51, 25);
            this.lbQty.TabIndex = 1;
            this.lbQty.Text = "QTY";
            // 
            // lbPrice
            // 
            this.lbPrice.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbPrice.AutoSize = true;
            this.lbPrice.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPrice.Location = new System.Drawing.Point(583, 13);
            this.lbPrice.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbPrice.Name = "lbPrice";
            this.lbPrice.Size = new System.Drawing.Size(70, 25);
            this.lbPrice.TabIndex = 2;
            this.lbPrice.Text = "PRICE";
            // 
            // lbDiscount
            // 
            this.lbDiscount.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbDiscount.AutoSize = true;
            this.lbDiscount.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDiscount.Location = new System.Drawing.Point(722, 13);
            this.lbDiscount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbDiscount.Name = "lbDiscount";
            this.lbDiscount.Size = new System.Drawing.Size(114, 25);
            this.lbDiscount.TabIndex = 3;
            this.lbDiscount.Text = "DISCOUNT";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(903, 13);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 25);
            this.label1.TabIndex = 4;
            this.label1.Text = "TOTAL";
            // 
            // chkSelected
            // 
            this.chkSelected.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chkSelected.AutoSize = true;
            this.chkSelected.Font = new System.Drawing.Font("Tahoma", 26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkSelected.Location = new System.Drawing.Point(1069, 18);
            this.chkSelected.Margin = new System.Windows.Forms.Padding(2);
            this.chkSelected.Name = "chkSelected";
            this.chkSelected.Size = new System.Drawing.Size(15, 14);
            this.chkSelected.TabIndex = 5;
            this.chkSelected.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkSelected.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lbPaid);
            this.panel2.Controls.Add(this.lbCustomer);
            this.panel2.Controls.Add(this.lbStatus);
            this.panel2.Controls.Add(this.lbTotal);
            this.panel2.Controls.Add(this.lbDate);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(6, 6);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1133, 119);
            this.panel2.TabIndex = 180;
            // 
            // panelFooter
            // 
            this.panelFooter.Controls.Add(this.btnRefundAll);
            this.panelFooter.Controls.Add(this.btnRefundSelected);
            this.panelFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelFooter.Location = new System.Drawing.Point(6, 631);
            this.panelFooter.Margin = new System.Windows.Forms.Padding(2);
            this.panelFooter.Name = "panelFooter";
            this.panelFooter.Size = new System.Drawing.Size(1133, 81);
            this.panelFooter.TabIndex = 11;
            // 
            // btnRefundAll
            // 
            this.btnRefundAll.BackColor = System.Drawing.Color.Transparent;
            this.btnRefundAll.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnRefundAll.ButtonPadding = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.btnRefundAll.ClickLocked = false;
            this.btnRefundAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefundAll.DisabledOverlayColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnRefundAll.DisabledOverlayFont = null;
            this.btnRefundAll.DisabledOverlayForeColor = System.Drawing.Color.White;
            this.btnRefundAll.DisabledOverlayText = "Processing...";
            this.btnRefundAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefundAll.Location = new System.Drawing.Point(419, 6);
            this.btnRefundAll.MinimumSize = new System.Drawing.Size(120, 36);
            this.btnRefundAll.Name = "btnRefundAll";
            this.btnRefundAll.Selected = false;
            this.btnRefundAll.Size = new System.Drawing.Size(375, 68);
            this.btnRefundAll.TabIndex = 197;
            this.btnRefundAll.Title = "REFUND ALL";
            this.btnRefundAll.TitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnRefundAll.TitleFontSize = 22F;
            this.btnRefundAll.Click += new System.EventHandler(this.btnRefundAll_Click);
            // 
            // btnRefundSelected
            // 
            this.btnRefundSelected.BackColor = System.Drawing.Color.Transparent;
            this.btnRefundSelected.BorderColor = System.Drawing.Color.Green;
            this.btnRefundSelected.ButtonPadding = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.btnRefundSelected.ClickLocked = false;
            this.btnRefundSelected.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefundSelected.DisabledOverlayColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnRefundSelected.DisabledOverlayFont = null;
            this.btnRefundSelected.DisabledOverlayForeColor = System.Drawing.Color.White;
            this.btnRefundSelected.DisabledOverlayText = "Processing...";
            this.btnRefundSelected.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefundSelected.Location = new System.Drawing.Point(13, 6);
            this.btnRefundSelected.MinimumSize = new System.Drawing.Size(120, 36);
            this.btnRefundSelected.Name = "btnRefundSelected";
            this.btnRefundSelected.Selected = false;
            this.btnRefundSelected.Size = new System.Drawing.Size(375, 71);
            this.btnRefundSelected.TabIndex = 196;
            this.btnRefundSelected.Title = "REFUND SELECTED";
            this.btnRefundSelected.TitleBackColor = System.Drawing.Color.Green;
            this.btnRefundSelected.TitleFontSize = 22F;
            this.btnRefundSelected.Click += new System.EventHandler(this.btnRefundSelected_Click);
            // 
            // FormRefund
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1168, 809);
            this.Controls.Add(this.panelLayout_Content);
            this.Controls.Add(this.panelLayout_Header);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormRefund";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Refund";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormRefund_FormClosed);
            this.Load += new System.EventHandler(this.FormRefund_Load);
            this.panelLayout_Header.ResumeLayout(false);
            this.panelLayout_Header.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            this.panelLayout_Content.ResumeLayout(false);
            this.tbHeader.ResumeLayout(false);
            this.tbHeader.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panelFooter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lbDate;
        private System.Windows.Forms.Label lbCustomer;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.Label lbTotal;
        private System.Windows.Forms.Label lbPaid;
        private System.Windows.Forms.ImageList imgRow;
        private System.Windows.Forms.Panel panelLayout_Header;
        private System.Windows.Forms.Label lbTitle;
        private DevExpress.XtraEditors.SvgImageBox btnClose;
        private MyControls.RoundPanel panelLayout_Content;
        private System.Windows.Forms.Panel panelFooter;
        private MyControls.ButtonRound btnRefundSelected;
        private MyControls.ButtonRound btnRefundAll;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TableLayoutPanel tbHeader;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.Label lbQty;
        private System.Windows.Forms.Label lbPrice;
        private System.Windows.Forms.Label lbDiscount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkSelected;
        private Models.Implements.KineticScrollPanel panelCartItemsTouch;
    }
}
