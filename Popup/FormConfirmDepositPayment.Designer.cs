namespace NailsChekin.Popup
{
    partial class FormConfirmDepositPayment
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
            this.lbDeposit = new System.Windows.Forms.Label();
            this.lbRemainAmount = new System.Windows.Forms.Label();
            this.btnConfirm_NO = new DevExpress.XtraEditors.SimpleButton();
            this.btnConfirm_YES = new DevExpress.XtraEditors.SimpleButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCredit = new DevExpress.XtraEditors.TextEdit();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.txtCash = new DevExpress.XtraEditors.TextEdit();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.txtChangeDue = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.txtCashReceived = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.txtCredit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCash.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtChangeDue.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCashReceived.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // lbDeposit
            // 
            this.lbDeposit.AutoSize = true;
            this.lbDeposit.Font = new System.Drawing.Font("Microsoft Sans Serif", 19F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDeposit.ForeColor = System.Drawing.Color.Red;
            this.lbDeposit.Location = new System.Drawing.Point(24, 20);
            this.lbDeposit.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbDeposit.Name = "lbDeposit";
            this.lbDeposit.Size = new System.Drawing.Size(525, 30);
            this.lbDeposit.TabIndex = 40;
            this.lbDeposit.Text = "Customer [name] already paid $xx deposited!";
            // 
            // lbRemainAmount
            // 
            this.lbRemainAmount.AutoSize = true;
            this.lbRemainAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 21F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRemainAmount.ForeColor = System.Drawing.Color.Red;
            this.lbRemainAmount.Location = new System.Drawing.Point(366, 67);
            this.lbRemainAmount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbRemainAmount.Name = "lbRemainAmount";
            this.lbRemainAmount.Size = new System.Drawing.Size(73, 32);
            this.lbRemainAmount.TabIndex = 37;
            this.lbRemainAmount.Text = "$xxx";
            // 
            // btnConfirm_NO
            // 
            this.btnConfirm_NO.Appearance.BackColor = System.Drawing.Color.SandyBrown;
            this.btnConfirm_NO.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirm_NO.Appearance.ForeColor = System.Drawing.Color.Black;
            this.btnConfirm_NO.Appearance.Options.UseBackColor = true;
            this.btnConfirm_NO.Appearance.Options.UseFont = true;
            this.btnConfirm_NO.Appearance.Options.UseForeColor = true;
            this.btnConfirm_NO.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirm_NO.Location = new System.Drawing.Point(283, 313);
            this.btnConfirm_NO.Margin = new System.Windows.Forms.Padding(2);
            this.btnConfirm_NO.Name = "btnConfirm_NO";
            this.btnConfirm_NO.Size = new System.Drawing.Size(250, 68);
            this.btnConfirm_NO.TabIndex = 34;
            this.btnConfirm_NO.Text = "NO";
            this.btnConfirm_NO.Click += new System.EventHandler(this.btnConfirm_NO_Click);
            // 
            // btnConfirm_YES
            // 
            this.btnConfirm_YES.Appearance.BackColor = System.Drawing.Color.LightPink;
            this.btnConfirm_YES.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirm_YES.Appearance.Options.UseBackColor = true;
            this.btnConfirm_YES.Appearance.Options.UseFont = true;
            this.btnConfirm_YES.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirm_YES.Location = new System.Drawing.Point(29, 313);
            this.btnConfirm_YES.Margin = new System.Windows.Forms.Padding(2);
            this.btnConfirm_YES.Name = "btnConfirm_YES";
            this.btnConfirm_YES.Size = new System.Drawing.Size(246, 68);
            this.btnConfirm_YES.TabIndex = 33;
            this.btnConfirm_YES.Text = "YES";
            this.btnConfirm_YES.Click += new System.EventHandler(this.btnConfirm_YES_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(135, 116);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(304, 46);
            this.label2.TabIndex = 32;
            this.label2.Text = "Please Confirm ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 19F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(124, 67);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(238, 30);
            this.label1.TabIndex = 31;
            this.label1.Text = "Remain balance is: ";
            // 
            // txtCredit
            // 
            this.txtCredit.Location = new System.Drawing.Point(282, 208);
            this.txtCredit.Name = "txtCredit";
            this.txtCredit.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 11F);
            this.txtCredit.Properties.Appearance.Options.UseFont = true;
            this.txtCredit.Size = new System.Drawing.Size(221, 24);
            this.txtCredit.TabIndex = 48;
            // 
            // labelControl4
            // 
            this.labelControl4.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl4.Appearance.Options.UseFont = true;
            this.labelControl4.Location = new System.Drawing.Point(282, 187);
            this.labelControl4.Margin = new System.Windows.Forms.Padding(2);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(58, 18);
            this.labelControl4.TabIndex = 47;
            this.labelControl4.Text = "CREDIT";
            // 
            // txtCash
            // 
            this.txtCash.Location = new System.Drawing.Point(62, 208);
            this.txtCash.Name = "txtCash";
            this.txtCash.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 11F);
            this.txtCash.Properties.Appearance.Options.UseFont = true;
            this.txtCash.Size = new System.Drawing.Size(214, 24);
            this.txtCash.TabIndex = 46;
            // 
            // labelControl5
            // 
            this.labelControl5.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl5.Appearance.Options.UseFont = true;
            this.labelControl5.Location = new System.Drawing.Point(62, 187);
            this.labelControl5.Margin = new System.Windows.Forms.Padding(2);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(41, 18);
            this.labelControl5.TabIndex = 45;
            this.labelControl5.Text = "CASH";
            // 
            // txtChangeDue
            // 
            this.txtChangeDue.Enabled = false;
            this.txtChangeDue.Location = new System.Drawing.Point(281, 267);
            this.txtChangeDue.Name = "txtChangeDue";
            this.txtChangeDue.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 11F);
            this.txtChangeDue.Properties.Appearance.Options.UseFont = true;
            this.txtChangeDue.Size = new System.Drawing.Size(221, 24);
            this.txtChangeDue.TabIndex = 44;
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Location = new System.Drawing.Point(281, 246);
            this.labelControl3.Margin = new System.Windows.Forms.Padding(2);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(69, 16);
            this.labelControl3.TabIndex = 43;
            this.labelControl3.Text = "Change Due";
            // 
            // txtCashReceived
            // 
            this.txtCashReceived.Location = new System.Drawing.Point(61, 267);
            this.txtCashReceived.Name = "txtCashReceived";
            this.txtCashReceived.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 11F);
            this.txtCashReceived.Properties.Appearance.Options.UseFont = true;
            this.txtCashReceived.Size = new System.Drawing.Size(214, 24);
            this.txtCashReceived.TabIndex = 42;
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 10F);
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Location = new System.Drawing.Point(61, 246);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(2);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(83, 16);
            this.labelControl2.TabIndex = 41;
            this.labelControl2.Text = "Cash Received";
            // 
            // FormConfirmDepositPayment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 399);
            this.Controls.Add(this.txtCredit);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.txtCash);
            this.Controls.Add(this.labelControl5);
            this.Controls.Add(this.txtChangeDue);
            this.Controls.Add(this.labelControl3);
            this.Controls.Add(this.txtCashReceived);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.lbDeposit);
            this.Controls.Add(this.lbRemainAmount);
            this.Controls.Add(this.btnConfirm_NO);
            this.Controls.Add(this.btnConfirm_YES);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfirmDepositPayment";
            this.Text = "Confirm Deposit Payment";
            ((System.ComponentModel.ISupportInitialize)(this.txtCredit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCash.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtChangeDue.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCashReceived.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbDeposit;
        private System.Windows.Forms.Label lbRemainAmount;
        private DevExpress.XtraEditors.SimpleButton btnConfirm_NO;
        private DevExpress.XtraEditors.SimpleButton btnConfirm_YES;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraEditors.TextEdit txtCredit;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.TextEdit txtCash;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private DevExpress.XtraEditors.TextEdit txtChangeDue;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit txtCashReceived;
        private DevExpress.XtraEditors.LabelControl labelControl2;
    }
}