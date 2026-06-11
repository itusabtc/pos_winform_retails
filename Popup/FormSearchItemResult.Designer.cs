namespace NailsChekin.Popup
{
    partial class FormSearchItemResult
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
            this.label14 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtSearchItemBarcode = new System.Windows.Forms.TextBox();
            this.btnCancel = new NailsChekin.MyControls.ButtonRound();
            this.btnFinish = new NailsChekin.MyControls.ButtonRound();
            this.SuspendLayout();
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Tahoma", 26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(41, 210);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(957, 53);
            this.label14.TabIndex = 66;
            this.label14.Text = "Would you like to add this product to database?";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Red;
            this.label8.Location = new System.Drawing.Point(185, 134);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(632, 53);
            this.label8.TabIndex = 65;
            this.label8.Text = "No Result - Product Not Found.";
            // 
            // txtSearchItemBarcode
            // 
            this.txtSearchItemBarcode.BackColor = System.Drawing.Color.White;
            this.txtSearchItemBarcode.Font = new System.Drawing.Font("Tahoma", 26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearchItemBarcode.Location = new System.Drawing.Point(193, 54);
            this.txtSearchItemBarcode.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtSearchItemBarcode.Name = "txtSearchItemBarcode";
            this.txtSearchItemBarcode.Size = new System.Drawing.Size(601, 60);
            this.txtSearchItemBarcode.TabIndex = 64;
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
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(50, 304);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.MinimumSize = new System.Drawing.Size(160, 44);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Selected = false;
            this.btnCancel.Size = new System.Drawing.Size(350, 75);
            this.btnCancel.TabIndex = 184;
            this.btnCancel.Title = "NO";
            this.btnCancel.TitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnCancel.TitleFontSize = 26F;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
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
            this.btnFinish.Font = new System.Drawing.Font("Microsoft Sans Serif", 26F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFinish.Location = new System.Drawing.Point(637, 304);
            this.btnFinish.Margin = new System.Windows.Forms.Padding(4);
            this.btnFinish.MinimumSize = new System.Drawing.Size(160, 44);
            this.btnFinish.Name = "btnFinish";
            this.btnFinish.Selected = false;
            this.btnFinish.Size = new System.Drawing.Size(350, 75);
            this.btnFinish.TabIndex = 185;
            this.btnFinish.Title = "YES";
            this.btnFinish.TitleBackColor = System.Drawing.Color.Green;
            this.btnFinish.TitleFontSize = 26F;
            this.btnFinish.Click += new System.EventHandler(this.btnFinish_Click);
            // 
            // FormSearchItemResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1048, 426);
            this.Controls.Add(this.btnFinish);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtSearchItemBarcode);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label8);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSearchItemResult";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormSearchItemResult";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormSearchItemResult_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtSearchItemBarcode;
        private MyControls.ButtonRound btnCancel;
        private MyControls.ButtonRound btnFinish;
    }
}