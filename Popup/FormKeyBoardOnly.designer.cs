namespace NailsChekin.Popup
{
    partial class FormKeyBoardOnly
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
            this.txtCurrentText = new System.Windows.Forms.TextBox();
            this.panelCart_Control_Keyboard = new System.Windows.Forms.Panel();
            this.btnConfirm = new NailsChekin.MyControls.ButtonRound();
            this.btnCart_Cancel = new NailsChekin.MyControls.ButtonRound();
            this.lbTitile = new DevExpress.XtraEditors.LabelControl();
            this.SuspendLayout();
            // 
            // txtCurrentText
            // 
            this.txtCurrentText.Font = new System.Drawing.Font("Tahoma", 39.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCurrentText.ForeColor = System.Drawing.Color.Red;
            this.txtCurrentText.Location = new System.Drawing.Point(60, 71);
            this.txtCurrentText.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtCurrentText.Name = "txtCurrentText";
            this.txtCurrentText.Size = new System.Drawing.Size(576, 87);
            this.txtCurrentText.TabIndex = 90;
            this.txtCurrentText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtCurrentText.TextChanged += new System.EventHandler(this.txtCurrentText_TextChanged);
            // 
            // panelCart_Control_Keyboard
            // 
            this.panelCart_Control_Keyboard.Location = new System.Drawing.Point(60, 162);
            this.panelCart_Control_Keyboard.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelCart_Control_Keyboard.Name = "panelCart_Control_Keyboard";
            this.panelCart_Control_Keyboard.Size = new System.Drawing.Size(577, 658);
            this.panelCart_Control_Keyboard.TabIndex = 169;
            // 
            // btnConfirm
            // 
            this.btnConfirm.BackColor = System.Drawing.Color.Transparent;
            this.btnConfirm.BorderColor = System.Drawing.Color.SteelBlue;
            this.btnConfirm.ButtonPadding = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.btnConfirm.ClickLocked = false;
            this.btnConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirm.DisabledOverlayColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnConfirm.DisabledOverlayFont = null;
            this.btnConfirm.DisabledOverlayForeColor = System.Drawing.Color.White;
            this.btnConfirm.DisabledOverlayText = "Processing...";
            this.btnConfirm.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirm.Location = new System.Drawing.Point(356, 834);
            this.btnConfirm.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnConfirm.MinimumSize = new System.Drawing.Size(160, 44);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Selected = false;
            this.btnConfirm.Size = new System.Drawing.Size(333, 111);
            this.btnConfirm.TabIndex = 172;
            this.btnConfirm.Title = "CONFIRM";
            this.btnConfirm.TitleBackColor = System.Drawing.Color.SteelBlue;
            this.btnConfirm.TitleFontSize = 22F;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnCart_Cancel
            // 
            this.btnCart_Cancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCart_Cancel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnCart_Cancel.ButtonPadding = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.btnCart_Cancel.ClickLocked = false;
            this.btnCart_Cancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCart_Cancel.DisabledOverlayColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnCart_Cancel.DisabledOverlayFont = null;
            this.btnCart_Cancel.DisabledOverlayForeColor = System.Drawing.Color.White;
            this.btnCart_Cancel.DisabledOverlayText = "Processing...";
            this.btnCart_Cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCart_Cancel.Location = new System.Drawing.Point(8, 834);
            this.btnCart_Cancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCart_Cancel.MinimumSize = new System.Drawing.Size(160, 44);
            this.btnCart_Cancel.Name = "btnCart_Cancel";
            this.btnCart_Cancel.Selected = false;
            this.btnCart_Cancel.Size = new System.Drawing.Size(333, 111);
            this.btnCart_Cancel.TabIndex = 171;
            this.btnCart_Cancel.Title = "CANCEL";
            this.btnCart_Cancel.TitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnCart_Cancel.TitleFontSize = 22F;
            this.btnCart_Cancel.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lbTitile
            // 
            this.lbTitile.Appearance.Font = new System.Drawing.Font("Arial", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTitile.Appearance.ForeColor = System.Drawing.Color.White;
            this.lbTitile.Appearance.Options.UseFont = true;
            this.lbTitile.Appearance.Options.UseForeColor = true;
            this.lbTitile.Location = new System.Drawing.Point(16, 5);
            this.lbTitile.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lbTitile.Name = "lbTitile";
            this.lbTitile.Size = new System.Drawing.Size(240, 51);
            this.lbTitile.TabIndex = 173;
            this.lbTitile.Text = "Enter Value";
            // 
            // FormKeyBoardOnly
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(699, 964);
            this.Controls.Add(this.lbTitile);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnCart_Cancel);
            this.Controls.Add(this.panelCart_Control_Keyboard);
            this.Controls.Add(this.txtCurrentText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormKeyBoardOnly";
            this.Text = "FormKeyBoardOnly";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormKeyBoardOnly_FormClosed);
            this.Shown += new System.EventHandler(this.FormKeyBoardOnly_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtCurrentText;
        private System.Windows.Forms.Panel panelCart_Control_Keyboard;
        private MyControls.ButtonRound btnConfirm;
        private MyControls.ButtonRound btnCart_Cancel;
        private DevExpress.XtraEditors.LabelControl lbTitile;
    }
}