namespace NailsChekin.Popup
{
    partial class FormConfirmPayment
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnPaymentCard_Cash = new DevExpress.XtraEditors.SimpleButton();
            this.btnPaymentCard_Change = new DevExpress.XtraEditors.SimpleButton();
            this.lbRemainAmount = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbCollectAmount = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lbTotalAmount = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lbCollectAmount2 = new System.Windows.Forms.Label();
            this.lbTotalAmount2 = new System.Windows.Forms.Label();
            this.lbRemainAmount2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(254, 89);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(215, 29);
            this.label1.TabIndex = 3;
            this.label1.Text = "Clover only collect ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(212, 133);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(253, 29);
            this.label2.TabIndex = 6;
            this.label2.Text = "Please Pay remaining ";
            // 
            // btnPaymentCard_Cash
            // 
            this.btnPaymentCard_Cash.Appearance.BackColor = System.Drawing.Color.SandyBrown;
            this.btnPaymentCard_Cash.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPaymentCard_Cash.Appearance.ForeColor = System.Drawing.Color.Black;
            this.btnPaymentCard_Cash.Appearance.Options.UseBackColor = true;
            this.btnPaymentCard_Cash.Appearance.Options.UseFont = true;
            this.btnPaymentCard_Cash.Appearance.Options.UseForeColor = true;
            this.btnPaymentCard_Cash.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPaymentCard_Cash.Location = new System.Drawing.Point(613, 317);
            this.btnPaymentCard_Cash.Margin = new System.Windows.Forms.Padding(2);
            this.btnPaymentCard_Cash.Name = "btnPaymentCard_Cash";
            this.btnPaymentCard_Cash.Size = new System.Drawing.Size(400, 68);
            this.btnPaymentCard_Cash.TabIndex = 24;
            this.btnPaymentCard_Cash.Text = "CASH ($XX)";
            this.btnPaymentCard_Cash.Click += new System.EventHandler(this.btnPaymentCard_Cash_Click);
            // 
            // btnPaymentCard_Change
            // 
            this.btnPaymentCard_Change.Appearance.BackColor = System.Drawing.Color.LightPink;
            this.btnPaymentCard_Change.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPaymentCard_Change.Appearance.Options.UseBackColor = true;
            this.btnPaymentCard_Change.Appearance.Options.UseFont = true;
            this.btnPaymentCard_Change.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPaymentCard_Change.Location = new System.Drawing.Point(16, 317);
            this.btnPaymentCard_Change.Margin = new System.Windows.Forms.Padding(2);
            this.btnPaymentCard_Change.Name = "btnPaymentCard_Change";
            this.btnPaymentCard_Change.Size = new System.Drawing.Size(400, 68);
            this.btnPaymentCard_Change.TabIndex = 23;
            this.btnPaymentCard_Change.Text = "CHARGE ($XX)";
            this.btnPaymentCard_Change.Click += new System.EventHandler(this.btnPaymentCard_Change_Click);
            // 
            // lbRemainAmount
            // 
            this.lbRemainAmount.AutoSize = true;
            this.lbRemainAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRemainAmount.ForeColor = System.Drawing.Color.Red;
            this.lbRemainAmount.Location = new System.Drawing.Point(461, 133);
            this.lbRemainAmount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbRemainAmount.Name = "lbRemainAmount";
            this.lbRemainAmount.Size = new System.Drawing.Size(68, 31);
            this.lbRemainAmount.TabIndex = 25;
            this.lbRemainAmount.Text = "$xxx";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(544, 133);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(218, 29);
            this.label4.TabIndex = 26;
            this.label4.Text = " by Cash or Charge";
            // 
            // lbCollectAmount
            // 
            this.lbCollectAmount.AutoSize = true;
            this.lbCollectAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCollectAmount.ForeColor = System.Drawing.Color.Red;
            this.lbCollectAmount.Location = new System.Drawing.Point(464, 89);
            this.lbCollectAmount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbCollectAmount.Name = "lbCollectAmount";
            this.lbCollectAmount.Size = new System.Drawing.Size(59, 29);
            this.lbCollectAmount.TabIndex = 27;
            this.lbCollectAmount.Text = "$xxx";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(540, 89);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(156, 29);
            this.label6.TabIndex = 28;
            this.label6.Text = "You still owe ";
            // 
            // lbTotalAmount
            // 
            this.lbTotalAmount.AutoSize = true;
            this.lbTotalAmount.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTotalAmount.ForeColor = System.Drawing.Color.Red;
            this.lbTotalAmount.Location = new System.Drawing.Point(697, 89);
            this.lbTotalAmount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbTotalAmount.Name = "lbTotalAmount";
            this.lbTotalAmount.Size = new System.Drawing.Size(59, 29);
            this.lbTotalAmount.TabIndex = 29;
            this.lbTotalAmount.Text = "$xxx";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(36, 23);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(954, 36);
            this.label3.TabIndex = 30;
            this.label3.Text = "Warning: Unsufficient Fund. Not Enough Money from Credit/Debit Card";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(241, 211);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(265, 29);
            this.label5.TabIndex = 31;
            this.label5.Text = "Máy cà thẻ chỉ lấy được ";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(586, 211);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(110, 29);
            this.label7.TabIndex = 32;
            this.label7.Text = "còn thiếu";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(7, 252);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(661, 29);
            this.label8.TabIndex = 33;
            this.label8.Text = "Bạn hỏi khách hàng muốn trả thêm thẻ khác cho phần còn lại ";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(734, 252);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(320, 29);
            this.label9.TabIndex = 34;
            this.label9.Text = "hay trả tiền mặt phần còn lại?";
            // 
            // lbCollectAmount2
            // 
            this.lbCollectAmount2.AutoSize = true;
            this.lbCollectAmount2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCollectAmount2.ForeColor = System.Drawing.Color.Red;
            this.lbCollectAmount2.Location = new System.Drawing.Point(507, 210);
            this.lbCollectAmount2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbCollectAmount2.Name = "lbCollectAmount2";
            this.lbCollectAmount2.Size = new System.Drawing.Size(59, 29);
            this.lbCollectAmount2.TabIndex = 35;
            this.lbCollectAmount2.Text = "$xxx";
            // 
            // lbTotalAmount2
            // 
            this.lbTotalAmount2.AutoSize = true;
            this.lbTotalAmount2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTotalAmount2.ForeColor = System.Drawing.Color.Red;
            this.lbTotalAmount2.Location = new System.Drawing.Point(700, 210);
            this.lbTotalAmount2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbTotalAmount2.Name = "lbTotalAmount2";
            this.lbTotalAmount2.Size = new System.Drawing.Size(59, 29);
            this.lbTotalAmount2.TabIndex = 36;
            this.lbTotalAmount2.Text = "$xxx";
            // 
            // lbRemainAmount2
            // 
            this.lbRemainAmount2.AutoSize = true;
            this.lbRemainAmount2.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbRemainAmount2.ForeColor = System.Drawing.Color.Red;
            this.lbRemainAmount2.Location = new System.Drawing.Point(657, 247);
            this.lbRemainAmount2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbRemainAmount2.Name = "lbRemainAmount2";
            this.lbRemainAmount2.Size = new System.Drawing.Size(68, 31);
            this.lbRemainAmount2.TabIndex = 37;
            this.lbRemainAmount2.Text = "$xxx";
            // 
            // FormConfirmPayment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1066, 406);
            this.Controls.Add(this.lbRemainAmount2);
            this.Controls.Add(this.lbTotalAmount2);
            this.Controls.Add(this.lbCollectAmount2);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbTotalAmount);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lbCollectAmount);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lbRemainAmount);
            this.Controls.Add(this.btnPaymentCard_Cash);
            this.Controls.Add(this.btnPaymentCard_Change);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfirmPayment";
            this.Text = "Warning: Unsufficient Fund. Not Enough Money from Credit/Debit Card";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private DevExpress.XtraEditors.SimpleButton btnPaymentCard_Cash;
        private DevExpress.XtraEditors.SimpleButton btnPaymentCard_Change;
        private System.Windows.Forms.Label lbRemainAmount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbCollectAmount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lbTotalAmount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lbCollectAmount2;
        private System.Windows.Forms.Label lbTotalAmount2;
        private System.Windows.Forms.Label lbRemainAmount2;
    }
}