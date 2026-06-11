namespace NailsChekin.Popup
{
    partial class FormFullPayment
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
            this.btnPAYNow = new DevExpress.XtraEditors.SimpleButton();
            this.lbFullAmount = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtCash = new DevExpress.XtraEditors.TextEdit();
            this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
            this.txtChangeDue = new DevExpress.XtraEditors.TextEdit();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.txtCashReceived = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtTotalCredit = new DevExpress.XtraEditors.TextEdit();
            this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
            this.lbTotalCharged = new DevExpress.XtraEditors.LabelControl();
            this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
            this.txtCredit = new DevExpress.XtraEditors.TextEdit();
            this.lbCardNum = new DevExpress.XtraEditors.LabelControl();
            this.lbStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnConfirmPayment = new DevExpress.XtraEditors.SimpleButton();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCash.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtChangeDue.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCashReceived.Properties)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalCredit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCredit.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // btnPAYNow
            // 
            this.btnPAYNow.Appearance.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.btnPAYNow.Appearance.Font = new System.Drawing.Font("Tahoma", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPAYNow.Appearance.Options.UseBackColor = true;
            this.btnPAYNow.Appearance.Options.UseFont = true;
            this.btnPAYNow.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPAYNow.Location = new System.Drawing.Point(329, 213);
            this.btnPAYNow.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnPAYNow.Name = "btnPAYNow";
            this.btnPAYNow.Size = new System.Drawing.Size(557, 141);
            this.btnPAYNow.TabIndex = 40;
            this.btnPAYNow.Text = "CARD #1 CHARGE";
            this.btnPAYNow.Click += new System.EventHandler(this.btnPAYNow_Click);
            // 
            // lbFullAmount
            // 
            this.lbFullAmount.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbFullAmount.Appearance.ForeColor = System.Drawing.Color.Red;
            this.lbFullAmount.Appearance.Options.UseFont = true;
            this.lbFullAmount.Appearance.Options.UseForeColor = true;
            this.lbFullAmount.Location = new System.Drawing.Point(557, 18);
            this.lbFullAmount.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lbFullAmount.Name = "lbFullAmount";
            this.lbFullAmount.Size = new System.Drawing.Size(171, 69);
            this.lbFullAmount.TabIndex = 36;
            this.lbFullAmount.Text = "$ 1.00";
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl1.Appearance.ForeColor = System.Drawing.Color.Red;
            this.labelControl1.Appearance.Options.UseFont = true;
            this.labelControl1.Appearance.Options.UseForeColor = true;
            this.labelControl1.Location = new System.Drawing.Point(167, 33);
            this.labelControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(369, 54);
            this.labelControl1.TabIndex = 35;
            this.labelControl1.Text = "FULL AMOUNT: ";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.groupBox1.Controls.Add(this.txtCash);
            this.groupBox1.Controls.Add(this.labelControl6);
            this.groupBox1.Controls.Add(this.txtChangeDue);
            this.groupBox1.Controls.Add(this.labelControl3);
            this.groupBox1.Controls.Add(this.txtCashReceived);
            this.groupBox1.Controls.Add(this.labelControl2);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(14, 111);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Size = new System.Drawing.Size(901, 184);
            this.groupBox1.TabIndex = 47;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "PAYMENT CASH";
            // 
            // txtCash
            // 
            this.txtCash.Location = new System.Drawing.Point(37, 98);
            this.txtCash.Margin = new System.Windows.Forms.Padding(4);
            this.txtCash.Name = "txtCash";
            this.txtCash.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 28F);
            this.txtCash.Properties.Appearance.Options.UseFont = true;
            this.txtCash.Size = new System.Drawing.Size(250, 64);
            this.txtCash.TabIndex = 40;
            this.txtCash.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txtCash_EditValueChanging);
            // 
            // labelControl6
            // 
            this.labelControl6.Appearance.Font = new System.Drawing.Font("Tahoma", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl6.Appearance.ForeColor = System.Drawing.Color.DarkOrange;
            this.labelControl6.Appearance.Options.UseFont = true;
            this.labelControl6.Appearance.Options.UseForeColor = true;
            this.labelControl6.Location = new System.Drawing.Point(37, 47);
            this.labelControl6.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.labelControl6.Name = "labelControl6";
            this.labelControl6.Size = new System.Drawing.Size(101, 45);
            this.labelControl6.TabIndex = 49;
            this.labelControl6.Text = "CASH";
            // 
            // txtChangeDue
            // 
            this.txtChangeDue.Enabled = false;
            this.txtChangeDue.Location = new System.Drawing.Point(625, 102);
            this.txtChangeDue.Margin = new System.Windows.Forms.Padding(4);
            this.txtChangeDue.Name = "txtChangeDue";
            this.txtChangeDue.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtChangeDue.Properties.Appearance.Options.UseFont = true;
            this.txtChangeDue.Size = new System.Drawing.Size(250, 60);
            this.txtChangeDue.TabIndex = 48;
            // 
            // labelControl3
            // 
            this.labelControl3.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl3.Appearance.Options.UseFont = true;
            this.labelControl3.Location = new System.Drawing.Point(625, 50);
            this.labelControl3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(211, 42);
            this.labelControl3.TabIndex = 47;
            this.labelControl3.Text = "Change Due";
            // 
            // txtCashReceived
            // 
            this.txtCashReceived.Location = new System.Drawing.Point(329, 101);
            this.txtCashReceived.Margin = new System.Windows.Forms.Padding(4);
            this.txtCashReceived.Name = "txtCashReceived";
            this.txtCashReceived.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCashReceived.Properties.Appearance.Options.UseFont = true;
            this.txtCashReceived.Size = new System.Drawing.Size(250, 60);
            this.txtCashReceived.TabIndex = 46;
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl2.Appearance.Options.UseFont = true;
            this.labelControl2.Location = new System.Drawing.Point(324, 53);
            this.labelControl2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(255, 42);
            this.labelControl2.TabIndex = 45;
            this.labelControl2.Text = "Cash Received";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Ivory;
            this.groupBox2.Controls.Add(this.txtTotalCredit);
            this.groupBox2.Controls.Add(this.labelControl7);
            this.groupBox2.Controls.Add(this.lbTotalCharged);
            this.groupBox2.Controls.Add(this.labelControl5);
            this.groupBox2.Controls.Add(this.txtCredit);
            this.groupBox2.Controls.Add(this.lbCardNum);
            this.groupBox2.Controls.Add(this.btnPAYNow);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(14, 321);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Size = new System.Drawing.Size(901, 380);
            this.groupBox2.TabIndex = 48;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "PAYMENT CREDIT";
            // 
            // txtTotalCredit
            // 
            this.txtTotalCredit.Location = new System.Drawing.Point(37, 133);
            this.txtTotalCredit.Margin = new System.Windows.Forms.Padding(4);
            this.txtTotalCredit.Name = "txtTotalCredit";
            this.txtTotalCredit.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 28F);
            this.txtTotalCredit.Properties.Appearance.Options.UseFont = true;
            this.txtTotalCredit.Size = new System.Drawing.Size(250, 64);
            this.txtTotalCredit.TabIndex = 52;
            this.txtTotalCredit.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txtTotalCredit_EditValueChanging);
            // 
            // labelControl7
            // 
            this.labelControl7.Appearance.Font = new System.Drawing.Font("Tahoma", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl7.Appearance.ForeColor = System.Drawing.Color.DarkOrange;
            this.labelControl7.Appearance.Options.UseFont = true;
            this.labelControl7.Appearance.Options.UseForeColor = true;
            this.labelControl7.Location = new System.Drawing.Point(37, 83);
            this.labelControl7.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.labelControl7.Name = "labelControl7";
            this.labelControl7.Size = new System.Drawing.Size(144, 45);
            this.labelControl7.TabIndex = 51;
            this.labelControl7.Text = "CREDIT";
            // 
            // lbTotalCharged
            // 
            this.lbTotalCharged.Appearance.Font = new System.Drawing.Font("Tahoma", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTotalCharged.Appearance.ForeColor = System.Drawing.Color.Red;
            this.lbTotalCharged.Appearance.Options.UseFont = true;
            this.lbTotalCharged.Appearance.Options.UseForeColor = true;
            this.lbTotalCharged.Location = new System.Drawing.Point(663, 133);
            this.lbTotalCharged.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lbTotalCharged.Name = "lbTotalCharged";
            this.lbTotalCharged.Size = new System.Drawing.Size(123, 60);
            this.lbTotalCharged.TabIndex = 50;
            this.lbTotalCharged.Text = "$0.00";
            // 
            // labelControl5
            // 
            this.labelControl5.Appearance.Font = new System.Drawing.Font("Tahoma", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl5.Appearance.ForeColor = System.Drawing.Color.Red;
            this.labelControl5.Appearance.Options.UseFont = true;
            this.labelControl5.Appearance.Options.UseForeColor = true;
            this.labelControl5.Location = new System.Drawing.Point(329, 136);
            this.labelControl5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.labelControl5.Name = "labelControl5";
            this.labelControl5.Size = new System.Drawing.Size(325, 57);
            this.labelControl5.TabIndex = 49;
            this.labelControl5.Text = "Total Charged: ";
            // 
            // txtCredit
            // 
            this.txtCredit.Location = new System.Drawing.Point(37, 294);
            this.txtCredit.Margin = new System.Windows.Forms.Padding(4);
            this.txtCredit.Name = "txtCredit";
            this.txtCredit.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCredit.Properties.Appearance.Options.UseFont = true;
            this.txtCredit.Size = new System.Drawing.Size(250, 60);
            this.txtCredit.TabIndex = 48;
            // 
            // lbCardNum
            // 
            this.lbCardNum.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCardNum.Appearance.Options.UseFont = true;
            this.lbCardNum.Location = new System.Drawing.Point(37, 244);
            this.lbCardNum.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lbCardNum.Name = "lbCardNum";
            this.lbCardNum.Size = new System.Drawing.Size(158, 42);
            this.lbCardNum.TabIndex = 47;
            this.lbCardNum.Text = "CARD #1";
            // 
            // lbStatus
            // 
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(39, 17);
            this.lbStatus.Text = "Status";
            // 
            // btnConfirmPayment
            // 
            this.btnConfirmPayment.Appearance.BackColor = System.Drawing.Color.Orange;
            this.btnConfirmPayment.Appearance.Font = new System.Drawing.Font("Tahoma", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfirmPayment.Appearance.Options.UseBackColor = true;
            this.btnConfirmPayment.Appearance.Options.UseFont = true;
            this.btnConfirmPayment.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfirmPayment.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnConfirmPayment.Location = new System.Drawing.Point(0, 724);
            this.btnConfirmPayment.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnConfirmPayment.Name = "btnConfirmPayment";
            this.btnConfirmPayment.Size = new System.Drawing.Size(927, 104);
            this.btnConfirmPayment.TabIndex = 50;
            this.btnConfirmPayment.Text = "CONFIRM PAYMENT";
            this.btnConfirmPayment.Click += new System.EventHandler(this.btnConfirmPayment_Click);
            // 
            // FormFullPayment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(927, 828);
            this.Controls.Add(this.btnConfirmPayment);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lbFullAmount);
            this.Controls.Add(this.labelControl1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormFullPayment";
            this.Text = "SPLIT PAYMENT";
            this.Shown += new System.EventHandler(this.FormFullPayment_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCash.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtChangeDue.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCashReceived.Properties)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTotalCredit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCredit.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraEditors.SimpleButton btnPAYNow;
        private DevExpress.XtraEditors.LabelControl lbFullAmount;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private System.Windows.Forms.GroupBox groupBox1;
        private DevExpress.XtraEditors.TextEdit txtChangeDue;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit txtCashReceived;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private System.Windows.Forms.GroupBox groupBox2;
        private DevExpress.XtraEditors.TextEdit txtCredit;
        private DevExpress.XtraEditors.LabelControl lbCardNum;
        private DevExpress.XtraEditors.LabelControl lbTotalCharged;
        private DevExpress.XtraEditors.LabelControl labelControl5;
        private System.Windows.Forms.ToolStripStatusLabel lbStatus;
        private DevExpress.XtraEditors.TextEdit txtCash;
        private DevExpress.XtraEditors.LabelControl labelControl6;
        private DevExpress.XtraEditors.TextEdit txtTotalCredit;
        private DevExpress.XtraEditors.LabelControl labelControl7;
        private DevExpress.XtraEditors.SimpleButton btnConfirmPayment;
    }
}