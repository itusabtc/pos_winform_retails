
namespace NailsChekin.Popup
{
    partial class FormRewardRedeem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRewardRedeem));
            this.txtRedeemPercent = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.txtRedeemAmount = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.txtTotalService = new DevExpress.XtraEditors.TextEdit();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnClose = new DevExpress.XtraEditors.SvgImageBox();
            this.txtRewardBalance = new DevExpress.XtraEditors.TextEdit();
            this.labelControl10 = new DevExpress.XtraEditors.LabelControl();
            this.btnConfirm = new NailsChekin.MyControls.ButtonRound();
            ((System.ComponentModel.ISupportInitialize)(this.txtRedeemPercent.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRedeemAmount.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalService.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRewardBalance.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // txtRedeemPercent
            // 
            this.txtRedeemPercent.EditValue = "";
            this.txtRedeemPercent.Enabled = false;
            this.txtRedeemPercent.Location = new System.Drawing.Point(519, 297);
            this.txtRedeemPercent.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.txtRedeemPercent.Name = "txtRedeemPercent";
            this.txtRedeemPercent.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRedeemPercent.Properties.Appearance.Options.UseFont = true;
            this.txtRedeemPercent.Size = new System.Drawing.Size(420, 76);
            this.txtRedeemPercent.TabIndex = 207;
            this.txtRedeemPercent.EditValueChanged += new System.EventHandler(this.txtRedeemPercent_EditValueChanged);
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Location = new System.Drawing.Point(519, 238);
            this.labelControl3.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(186, 39);
            this.labelControl3.TabIndex = 206;
            this.labelControl3.Text = "% REDEEM";
            // 
            // txtRedeemAmount
            // 
            this.txtRedeemAmount.EditValue = "";
            this.txtRedeemAmount.Enabled = false;
            this.txtRedeemAmount.Location = new System.Drawing.Point(20, 494);
            this.txtRedeemAmount.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.txtRedeemAmount.Name = "txtRedeemAmount";
            this.txtRedeemAmount.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRedeemAmount.Properties.Appearance.Options.UseFont = true;
            this.txtRedeemAmount.Size = new System.Drawing.Size(420, 76);
            this.txtRedeemAmount.TabIndex = 203;
            this.txtRedeemAmount.EditValueChanged += new System.EventHandler(this.txtRedeemAmount_EditValueChanged);
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Location = new System.Drawing.Point(20, 438);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(304, 39);
            this.labelControl2.TabIndex = 202;
            this.labelControl2.Text = "REDEEM AMOUNT";
            // 
            // txtTotalService
            // 
            this.txtTotalService.EditValue = "$0";
            this.txtTotalService.Enabled = false;
            this.txtTotalService.Location = new System.Drawing.Point(20, 110);
            this.txtTotalService.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.txtTotalService.Name = "txtTotalService";
            this.txtTotalService.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTotalService.Properties.Appearance.Options.UseFont = true;
            this.txtTotalService.Size = new System.Drawing.Size(420, 76);
            this.txtTotalService.TabIndex = 201;
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Location = new System.Drawing.Point(20, 55);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(271, 39);
            this.labelControl1.TabIndex = 200;
            this.labelControl1.Text = "TOTAL SERVICE";
            // 
            // btnClose
            // 
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Location = new System.Drawing.Point(857, 5);
            this.btnClose.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(82, 79);
            this.btnClose.SizeMode = DevExpress.XtraEditors.SvgImageSizeMode.Stretch;
            this.btnClose.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnClose.SvgImage")));
            this.btnClose.TabIndex = 199;
            this.btnClose.Text = "svgCustomerReload";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // txtRewardBalance
            // 
            this.txtRewardBalance.EditValue = "0";
            this.txtRewardBalance.Enabled = false;
            this.txtRewardBalance.Location = new System.Drawing.Point(19, 297);
            this.txtRewardBalance.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.txtRewardBalance.Name = "txtRewardBalance";
            this.txtRewardBalance.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRewardBalance.Properties.Appearance.Options.UseFont = true;
            this.txtRewardBalance.Size = new System.Drawing.Size(420, 76);
            this.txtRewardBalance.TabIndex = 198;
            // 
            // labelControl10
            // 
            this.labelControl10.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl10.Appearance.Options.UseFont = true;
            this.labelControl10.Location = new System.Drawing.Point(19, 244);
            this.labelControl10.Margin = new System.Windows.Forms.Padding(5, 2, 5, 2);
            this.labelControl10.Name = "labelControl10";
            this.labelControl10.Size = new System.Drawing.Size(323, 39);
            this.labelControl10.TabIndex = 197;
            this.labelControl10.Text = "REWARD BALANCE";
            // 
            // btnConfirm
            // 
            this.btnConfirm.BackColor = System.Drawing.Color.Transparent;
            this.btnConfirm.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnConfirm.ButtonPadding = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.btnConfirm.ClickLocked = false;
            this.btnConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirm.DisabledOverlayColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnConfirm.DisabledOverlayFont = null;
            this.btnConfirm.DisabledOverlayForeColor = System.Drawing.Color.White;
            this.btnConfirm.DisabledOverlayText = "Processing...";
            this.btnConfirm.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirm.Location = new System.Drawing.Point(20, 611);
            this.btnConfirm.Margin = new System.Windows.Forms.Padding(4);
            this.btnConfirm.MinimumSize = new System.Drawing.Size(160, 44);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Selected = false;
            this.btnConfirm.Size = new System.Drawing.Size(919, 92);
            this.btnConfirm.TabIndex = 213;
            this.btnConfirm.Title = "CONFIRM";
            this.btnConfirm.TitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnConfirm.TitleFontSize = 22F;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // FormRewardRedeem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(953, 725);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.txtRedeemPercent);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.txtRedeemAmount);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.txtTotalService);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.txtRewardBalance);
            this.Controls.Add(this.labelControl10);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormRewardRedeem";
            this.Text = "FormRewardRedeem";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormRewardRedeem_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.txtRedeemPercent.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRedeemAmount.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalService.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRewardBalance.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.TextEdit txtRedeemPercent;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit txtRedeemAmount;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.TextEdit txtTotalService;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SvgImageBox btnClose;
        private DevExpress.XtraEditors.TextEdit txtRewardBalance;
        private DevExpress.XtraEditors.LabelControl labelControl10;
        private MyControls.ButtonRound btnConfirm;
    }
}