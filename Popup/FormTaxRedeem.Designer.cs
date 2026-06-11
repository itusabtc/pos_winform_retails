
namespace NailsChekin.Popup
{
    partial class FormTaxRedeem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTaxRedeem));
            this.roundPanel1 = new NailsChekin.MyControls.RoundPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnConfirm = new NailsChekin.MyControls.ButtonRound();
            this.rdApplyTaxOFF = new System.Windows.Forms.RadioButton();
            this.lbTitle = new DevExpress.XtraEditors.LabelControl();
            this.rdApplyTaxON = new System.Windows.Forms.RadioButton();
            this.txtCurrentText = new System.Windows.Forms.TextBox();
            this.labelControl10 = new DevExpress.XtraEditors.LabelControl();
            this.btnClose = new DevExpress.XtraEditors.SvgImageBox();
            this.roundPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            this.SuspendLayout();
            // 
            // roundPanel1
            // 
            this.roundPanel1.BackColor = System.Drawing.Color.Transparent;
            this.roundPanel1.Controls.Add(this.panel1);
            this.roundPanel1.Controls.Add(this.rdApplyTaxOFF);
            this.roundPanel1.Controls.Add(this.lbTitle);
            this.roundPanel1.Controls.Add(this.rdApplyTaxON);
            this.roundPanel1.Controls.Add(this.txtCurrentText);
            this.roundPanel1.Controls.Add(this.labelControl10);
            this.roundPanel1.Location = new System.Drawing.Point(12, 90);
            this.roundPanel1.Name = "roundPanel1";
            this.roundPanel1.Padding = new System.Windows.Forms.Padding(8);
            this.roundPanel1.Size = new System.Drawing.Size(516, 535);
            this.roundPanel1.TabIndex = 219;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnConfirm);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(8, 427);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(500, 100);
            this.panel1.TabIndex = 217;
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
            this.btnConfirm.Location = new System.Drawing.Point(3, 22);
            this.btnConfirm.MinimumSize = new System.Drawing.Size(120, 36);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Selected = false;
            this.btnConfirm.Size = new System.Drawing.Size(486, 75);
            this.btnConfirm.TabIndex = 218;
            this.btnConfirm.Title = "CONFIRM";
            this.btnConfirm.TitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnConfirm.TitleFontSize = 22F;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // rdApplyTaxOFF
            // 
            this.rdApplyTaxOFF.AutoSize = true;
            this.rdApplyTaxOFF.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdApplyTaxOFF.Location = new System.Drawing.Point(270, 245);
            this.rdApplyTaxOFF.Margin = new System.Windows.Forms.Padding(2);
            this.rdApplyTaxOFF.Name = "rdApplyTaxOFF";
            this.rdApplyTaxOFF.Size = new System.Drawing.Size(178, 77);
            this.rdApplyTaxOFF.TabIndex = 185;
            this.rdApplyTaxOFF.Text = "OFF";
            this.rdApplyTaxOFF.UseVisualStyleBackColor = true;
            // 
            // lbTitle
            // 
            this.lbTitle.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTitle.Appearance.Options.UseFont = true;
            this.lbTitle.Location = new System.Drawing.Point(15, 26);
            this.lbTitle.Margin = new System.Windows.Forms.Padding(2);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(191, 31);
            this.lbTitle.TabIndex = 216;
            this.lbTitle.Text = "TAX PERCENT";
            // 
            // rdApplyTaxON
            // 
            this.rdApplyTaxON.AutoSize = true;
            this.rdApplyTaxON.Checked = true;
            this.rdApplyTaxON.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdApplyTaxON.Location = new System.Drawing.Point(67, 245);
            this.rdApplyTaxON.Margin = new System.Windows.Forms.Padding(2);
            this.rdApplyTaxON.Name = "rdApplyTaxON";
            this.rdApplyTaxON.Size = new System.Drawing.Size(146, 77);
            this.rdApplyTaxON.TabIndex = 184;
            this.rdApplyTaxON.TabStop = true;
            this.rdApplyTaxON.Text = "ON";
            this.rdApplyTaxON.UseVisualStyleBackColor = true;
            // 
            // txtCurrentText
            // 
            this.txtCurrentText.Enabled = false;
            this.txtCurrentText.Font = new System.Drawing.Font("Tahoma", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCurrentText.ForeColor = System.Drawing.Color.Red;
            this.txtCurrentText.Location = new System.Drawing.Point(15, 62);
            this.txtCurrentText.Name = "txtCurrentText";
            this.txtCurrentText.Size = new System.Drawing.Size(482, 65);
            this.txtCurrentText.TabIndex = 214;
            this.txtCurrentText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelControl10
            // 
            this.labelControl10.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl10.Appearance.Options.UseFont = true;
            this.labelControl10.Location = new System.Drawing.Point(19, 187);
            this.labelControl10.Margin = new System.Windows.Forms.Padding(2);
            this.labelControl10.Name = "labelControl10";
            this.labelControl10.Size = new System.Drawing.Size(187, 39);
            this.labelControl10.TabIndex = 183;
            this.labelControl10.Text = "APPLY TAX";
            // 
            // btnClose
            // 
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Location = new System.Drawing.Point(481, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(57, 56);
            this.btnClose.SizeMode = DevExpress.XtraEditors.SvgImageSizeMode.Stretch;
            this.btnClose.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnClose.SvgImage")));
            this.btnClose.TabIndex = 217;
            this.btnClose.Text = "svgCustomerReload";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FormTaxRedeem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(540, 639);
            this.Controls.Add(this.roundPanel1);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormTaxRedeem";
            this.Text = "FormTaxRedeem";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormTaxRedeem_FormClosed);
            this.Load += new System.EventHandler(this.FormTaxRedeem_Load);
            this.roundPanel1.ResumeLayout(false);
            this.roundPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MyControls.RoundPanel roundPanel1;
        private System.Windows.Forms.RadioButton rdApplyTaxOFF;
        private System.Windows.Forms.RadioButton rdApplyTaxON;
        private DevExpress.XtraEditors.LabelControl labelControl10;
        private MyControls.ButtonRound btnConfirm;
        private DevExpress.XtraEditors.SvgImageBox btnClose;
        private DevExpress.XtraEditors.LabelControl lbTitle;
        private System.Windows.Forms.TextBox txtCurrentText;
        private System.Windows.Forms.Panel panel1;
    }
}