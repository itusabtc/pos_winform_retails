namespace NailsChekin.Popup
{
    partial class FormConfirmPrintBill
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
            this.label3 = new System.Windows.Forms.Label();
            this.btnNoPrint = new DevExpress.XtraEditors.SimpleButton();
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.lbChange = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(169, 74);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(178, 42);
            this.label3.TabIndex = 40;
            this.label3.Text = "CHANGE";
            // 
            // btnNoPrint
            // 
            this.btnNoPrint.Appearance.BackColor = System.Drawing.Color.Red;
            this.btnNoPrint.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNoPrint.Appearance.ForeColor = System.Drawing.Color.Black;
            this.btnNoPrint.Appearance.Options.UseBackColor = true;
            this.btnNoPrint.Appearance.Options.UseFont = true;
            this.btnNoPrint.Appearance.Options.UseForeColor = true;
            this.btnNoPrint.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNoPrint.Location = new System.Drawing.Point(7, 193);
            this.btnNoPrint.Margin = new System.Windows.Forms.Padding(2);
            this.btnNoPrint.Name = "btnNoPrint";
            this.btnNoPrint.Size = new System.Drawing.Size(240, 41);
            this.btnNoPrint.TabIndex = 34;
            this.btnNoPrint.Text = "No Receipt";
            this.btnNoPrint.Click += new System.EventHandler(this.btnNoPrint_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Appearance.BackColor = System.Drawing.Color.Green;
            this.btnPrint.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrint.Appearance.Options.UseBackColor = true;
            this.btnPrint.Appearance.Options.UseFont = true;
            this.btnPrint.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPrint.Location = new System.Drawing.Point(259, 193);
            this.btnPrint.Margin = new System.Windows.Forms.Padding(2);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(240, 41);
            this.btnPrint.TabIndex = 33;
            this.btnPrint.Text = "Print Receipt";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // lbChange
            // 
            this.lbChange.AutoSize = true;
            this.lbChange.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbChange.ForeColor = System.Drawing.Color.Red;
            this.lbChange.Location = new System.Drawing.Point(193, 122);
            this.lbChange.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbChange.Name = "lbChange";
            this.lbChange.Size = new System.Drawing.Size(112, 42);
            this.lbChange.TabIndex = 31;
            this.lbChange.Text = "$1.00";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(11, 24);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(495, 31);
            this.label1.TabIndex = 41;
            this.label1.Text = "Do you want to print Customer Receipt?";
            // 
            // FormConfirmPrintBill
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(508, 242);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnNoPrint);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.lbChange);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfirmPrintBill";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Confirm Print Receipt?";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private DevExpress.XtraEditors.SimpleButton btnNoPrint;
        private DevExpress.XtraEditors.SimpleButton btnPrint;
        private System.Windows.Forms.Label lbChange;
        private System.Windows.Forms.Label label1;
    }
}