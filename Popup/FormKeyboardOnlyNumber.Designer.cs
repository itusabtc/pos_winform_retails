namespace NailsChekin.Popup
{
    partial class FormKeyboardOnlyNumber
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
            this.panelCart_Keyboard = new System.Windows.Forms.Panel();
            this.btnClose = new NailsChekin.MyControls.ButtonRound();
            this.txtCurrentText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // panelCart_Keyboard
            // 
            this.panelCart_Keyboard.BackColor = System.Drawing.Color.Transparent;
            this.panelCart_Keyboard.Location = new System.Drawing.Point(18, 77);
            this.panelCart_Keyboard.Name = "panelCart_Keyboard";
            this.panelCart_Keyboard.Size = new System.Drawing.Size(536, 618);
            this.panelCart_Keyboard.TabIndex = 169;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnClose.ButtonPadding = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.btnClose.ClickLocked = false;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.DisabledOverlayColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnClose.DisabledOverlayFont = null;
            this.btnClose.DisabledOverlayForeColor = System.Drawing.Color.White;
            this.btnClose.DisabledOverlayText = "Processing...";
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(18, 706);
            this.btnClose.MinimumSize = new System.Drawing.Size(120, 36);
            this.btnClose.Name = "btnClose";
            this.btnClose.Selected = false;
            this.btnClose.Size = new System.Drawing.Size(536, 75);
            this.btnClose.TabIndex = 185;
            this.btnClose.Title = "CLOSE SEARCH";
            this.btnClose.TitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnClose.TitleFontSize = 22F;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // txtCurrentText
            // 
            this.txtCurrentText.BackColor = System.Drawing.Color.White;
            this.txtCurrentText.Font = new System.Drawing.Font("Tahoma", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCurrentText.Location = new System.Drawing.Point(18, 12);
            this.txtCurrentText.Name = "txtCurrentText";
            this.txtCurrentText.ReadOnly = true;
            this.txtCurrentText.Size = new System.Drawing.Size(536, 59);
            this.txtCurrentText.TabIndex = 187;
            this.txtCurrentText.TabStop = false;
            this.txtCurrentText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // FormKeyboardOnlyNumber
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(573, 790);
            this.Controls.Add(this.txtCurrentText);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.panelCart_Keyboard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormKeyboardOnlyNumber";
            this.Text = "FormKeyboardOnlyNumber";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormKeyboardOnlyNumber_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelCart_Keyboard;
        private MyControls.ButtonRound btnClose;
        private System.Windows.Forms.TextBox txtCurrentText;
    }
}