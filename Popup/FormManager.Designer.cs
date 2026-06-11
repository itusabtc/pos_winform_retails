namespace NailsChekin.Popup
{
    partial class FormManager
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
            this.lbTitle = new DevExpress.XtraEditors.LabelControl();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.btnConfirm = new NailsChekin.MyControls.ButtonRound();
            this.panelCart_Keyboard = new System.Windows.Forms.Panel();
            this.buttonRound1 = new NailsChekin.MyControls.ButtonRound();
            this.SuspendLayout();
            // 
            // lbTitle
            // 
            this.lbTitle.Appearance.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTitle.Appearance.ForeColor = System.Drawing.Color.White;
            this.lbTitle.Appearance.Options.UseFont = true;
            this.lbTitle.Appearance.Options.UseForeColor = true;
            this.lbTitle.Location = new System.Drawing.Point(12, 7);
            this.lbTitle.Margin = new System.Windows.Forms.Padding(2);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(389, 39);
            this.lbTitle.TabIndex = 35;
            this.lbTitle.Text = "ENTER OFFICE PASSWORD";
            // 
            // txtInput
            // 
            this.txtInput.BackColor = System.Drawing.Color.White;
            this.txtInput.Font = new System.Drawing.Font("Tahoma", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtInput.Location = new System.Drawing.Point(12, 55);
            this.txtInput.Name = "txtInput";
            this.txtInput.ReadOnly = true;
            this.txtInput.Size = new System.Drawing.Size(536, 59);
            this.txtInput.TabIndex = 190;
            this.txtInput.TabStop = false;
            this.txtInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
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
            this.btnConfirm.Location = new System.Drawing.Point(288, 749);
            this.btnConfirm.MinimumSize = new System.Drawing.Size(120, 36);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Selected = false;
            this.btnConfirm.Size = new System.Drawing.Size(260, 75);
            this.btnConfirm.TabIndex = 189;
            this.btnConfirm.Title = "CONFIRM";
            this.btnConfirm.TitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnConfirm.TitleFontSize = 22F;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // panelCart_Keyboard
            // 
            this.panelCart_Keyboard.BackColor = System.Drawing.Color.Transparent;
            this.panelCart_Keyboard.Location = new System.Drawing.Point(12, 120);
            this.panelCart_Keyboard.Name = "panelCart_Keyboard";
            this.panelCart_Keyboard.Size = new System.Drawing.Size(536, 618);
            this.panelCart_Keyboard.TabIndex = 188;
            // 
            // buttonRound1
            // 
            this.buttonRound1.BackColor = System.Drawing.Color.Transparent;
            this.buttonRound1.BorderColor = System.Drawing.Color.Orange;
            this.buttonRound1.ButtonPadding = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.buttonRound1.ClickLocked = false;
            this.buttonRound1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonRound1.DisabledOverlayColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.buttonRound1.DisabledOverlayFont = null;
            this.buttonRound1.DisabledOverlayForeColor = System.Drawing.Color.White;
            this.buttonRound1.DisabledOverlayText = "Processing...";
            this.buttonRound1.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRound1.Location = new System.Drawing.Point(12, 749);
            this.buttonRound1.MinimumSize = new System.Drawing.Size(120, 36);
            this.buttonRound1.Name = "buttonRound1";
            this.buttonRound1.Selected = false;
            this.buttonRound1.Size = new System.Drawing.Size(260, 75);
            this.buttonRound1.TabIndex = 191;
            this.buttonRound1.Title = "CANCEL";
            this.buttonRound1.TitleBackColor = System.Drawing.Color.Orange;
            this.buttonRound1.TitleFontSize = 22F;
            this.buttonRound1.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FormManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(562, 834);
            this.Controls.Add(this.buttonRound1);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.panelCart_Keyboard);
            this.Controls.Add(this.lbTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormManager";
            this.Text = "FormManager";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lbTitle;
        private System.Windows.Forms.TextBox txtInput;
        private MyControls.ButtonRound btnConfirm;
        private System.Windows.Forms.Panel panelCart_Keyboard;
        private MyControls.ButtonRound buttonRound1;
    }
}