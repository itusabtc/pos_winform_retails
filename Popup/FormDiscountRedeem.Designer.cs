
namespace NailsChekin.Popup
{
    partial class FormDiscountRedeem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDiscountRedeem));
            this.lbTitle = new DevExpress.XtraEditors.LabelControl();
            this.panelCart_Control_Keyboard = new System.Windows.Forms.Panel();
            this.txtCurrentText = new System.Windows.Forms.TextBox();
            this.btnClose = new DevExpress.XtraEditors.SvgImageBox();
            this.btnConfirm = new NailsChekin.MyControls.ButtonRound();
            this.roundPanel1 = new NailsChekin.MyControls.RoundPanel();
            this.rbUnitUsd = new System.Windows.Forms.RadioButton();
            this.rbUnitPercent = new System.Windows.Forms.RadioButton();
            this.labelControl10 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            this.roundPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbTitle
            // 
            this.lbTitle.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTitle.Appearance.Options.UseFont = true;
            this.lbTitle.Location = new System.Drawing.Point(11, 35);
            this.lbTitle.Margin = new System.Windows.Forms.Padding(2);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(144, 31);
            this.lbTitle.TabIndex = 205;
            this.lbTitle.Text = "DISCOUNT";
            // 
            // panelCart_Control_Keyboard
            // 
            this.panelCart_Control_Keyboard.Location = new System.Drawing.Point(11, 136);
            this.panelCart_Control_Keyboard.Name = "panelCart_Control_Keyboard";
            this.panelCart_Control_Keyboard.Size = new System.Drawing.Size(433, 535);
            this.panelCart_Control_Keyboard.TabIndex = 204;
            // 
            // txtCurrentText
            // 
            this.txtCurrentText.Font = new System.Drawing.Font("Tahoma", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCurrentText.ForeColor = System.Drawing.Color.Red;
            this.txtCurrentText.Location = new System.Drawing.Point(11, 71);
            this.txtCurrentText.Name = "txtCurrentText";
            this.txtCurrentText.Size = new System.Drawing.Size(433, 65);
            this.txtCurrentText.TabIndex = 203;
            this.txtCurrentText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnClose
            // 
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Location = new System.Drawing.Point(740, 2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(57, 56);
            this.btnClose.SizeMode = DevExpress.XtraEditors.SvgImageSizeMode.Stretch;
            this.btnClose.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnClose.SvgImage")));
            this.btnClose.TabIndex = 211;
            this.btnClose.Text = "svgCustomerReload";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
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
            this.btnConfirm.Location = new System.Drawing.Point(12, 688);
            this.btnConfirm.MinimumSize = new System.Drawing.Size(120, 36);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Selected = false;
            this.btnConfirm.Size = new System.Drawing.Size(776, 75);
            this.btnConfirm.TabIndex = 212;
            this.btnConfirm.Title = "CONFIRM";
            this.btnConfirm.TitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnConfirm.TitleFontSize = 22F;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // roundPanel1
            // 
            this.roundPanel1.BackColor = System.Drawing.Color.Transparent;
            this.roundPanel1.Controls.Add(this.rbUnitUsd);
            this.roundPanel1.Controls.Add(this.rbUnitPercent);
            this.roundPanel1.Controls.Add(this.labelControl10);
            this.roundPanel1.Location = new System.Drawing.Point(465, 136);
            this.roundPanel1.Name = "roundPanel1";
            this.roundPanel1.Padding = new System.Windows.Forms.Padding(8);
            this.roundPanel1.Size = new System.Drawing.Size(323, 535);
            this.roundPanel1.TabIndex = 213;
            // 
            // rbUnitUsd
            // 
            this.rbUnitUsd.AutoSize = true;
            this.rbUnitUsd.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbUnitUsd.Location = new System.Drawing.Point(196, 95);
            this.rbUnitUsd.Margin = new System.Windows.Forms.Padding(2);
            this.rbUnitUsd.Name = "rbUnitUsd";
            this.rbUnitUsd.Size = new System.Drawing.Size(86, 77);
            this.rbUnitUsd.TabIndex = 185;
            this.rbUnitUsd.Text = "$";
            this.rbUnitUsd.UseVisualStyleBackColor = true;
            // 
            // rbUnitPercent
            // 
            this.rbUnitPercent.AutoSize = true;
            this.rbUnitPercent.Checked = true;
            this.rbUnitPercent.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbUnitPercent.Location = new System.Drawing.Point(58, 95);
            this.rbUnitPercent.Margin = new System.Windows.Forms.Padding(2);
            this.rbUnitPercent.Name = "rbUnitPercent";
            this.rbUnitPercent.Size = new System.Drawing.Size(107, 77);
            this.rbUnitPercent.TabIndex = 184;
            this.rbUnitPercent.TabStop = true;
            this.rbUnitPercent.Text = "%";
            this.rbUnitPercent.UseVisualStyleBackColor = true;
            // 
            // labelControl10
            // 
            this.labelControl10.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl10.Appearance.Options.UseFont = true;
            this.labelControl10.Location = new System.Drawing.Point(40, 60);
            this.labelControl10.Margin = new System.Windows.Forms.Padding(2);
            this.labelControl10.Name = "labelControl10";
            this.labelControl10.Size = new System.Drawing.Size(216, 31);
            this.labelControl10.TabIndex = 183;
            this.labelControl10.Text = "DISCOUNT UNIT";
            // 
            // FormDiscountRedeem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(800, 781);
            this.Controls.Add(this.roundPanel1);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lbTitle);
            this.Controls.Add(this.panelCart_Control_Keyboard);
            this.Controls.Add(this.txtCurrentText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormDiscountRedeem";
            this.Text = "FormDiscountRedeem";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormDiscountRedeem_FormClosed);
            this.Load += new System.EventHandler(this.FormDiscountRedeem_Load);
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            this.roundPanel1.ResumeLayout(false);
            this.roundPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lbTitle;
        private System.Windows.Forms.Panel panelCart_Control_Keyboard;
        private System.Windows.Forms.TextBox txtCurrentText;
        private DevExpress.XtraEditors.SvgImageBox btnClose;
        private MyControls.ButtonRound btnConfirm;
        private MyControls.RoundPanel roundPanel1;
        private System.Windows.Forms.RadioButton rbUnitUsd;
        private System.Windows.Forms.RadioButton rbUnitPercent;
        private DevExpress.XtraEditors.LabelControl labelControl10;
    }
}