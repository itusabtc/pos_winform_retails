namespace POSPrintService
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.lbPrinterStatus = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.lbCreditCardStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabelTimer = new System.Windows.Forms.ToolStripLabel();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.txtStoreCode = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtForderLog = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtForderPrint = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDomain = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtAcrobatURL = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnStopScreditCard = new System.Windows.Forms.Button();
            this.btnStartCreditCardConnect = new System.Windows.Forms.Button();
            this.chkTipsOn = new System.Windows.Forms.CheckBox();
            this.chkTipsOff = new System.Windows.Forms.CheckBox();
            this.chkSigOnPaper = new System.Windows.Forms.CheckBox();
            this.chkSigOnScreen = new System.Windows.Forms.CheckBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipText = "Nganhnails POS Tool";
            this.notifyIcon1.BalloonTipTitle = "Nganhnails POS Tool";
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Tag = "Nganhnails POS Tool";
            this.notifyIcon1.Text = "Nganhnails POS Tool";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            //this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            
            // 
            // backgroundWorker2
            // 
            this.backgroundWorker2.WorkerReportsProgress = true;
            this.backgroundWorker2.WorkerSupportsCancellation = true;
            this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker2_DoWork);
            this.backgroundWorker2.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker2_ProgressChanged);
            this.backgroundWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker2_RunWorkerCompleted);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator2,
            this.toolStripSeparator3,
            this.lbPrinterStatus,
            this.toolStripSeparator1,
            this.lbCreditCardStatus,
            this.toolStripSeparator4,
            this.toolStripLabelTimer});
            this.toolStrip1.Location = new System.Drawing.Point(0, 781);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1256, 37);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 37);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 37);
            // 
            // lbPrinterStatus
            // 
            this.lbPrinterStatus.Name = "lbPrinterStatus";
            this.lbPrinterStatus.Size = new System.Drawing.Size(156, 34);
            this.lbPrinterStatus.Text = "Printer Status";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 37);
            // 
            // lbCreditCardStatus
            // 
            this.lbCreditCardStatus.Name = "lbCreditCardStatus";
            this.lbCreditCardStatus.Size = new System.Drawing.Size(206, 32);
            this.lbCreditCardStatus.Text = "Credit Card Status";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 37);
            // 
            // toolStripLabelTimer
            // 
            this.toolStripLabelTimer.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabelTimer.Name = "toolStripLabelTimer";
            this.toolStripLabelTimer.Size = new System.Drawing.Size(222, 34);
            this.toolStripLabelTimer.Text = "toolStripLabelTimer";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(58, 351);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 25);
            this.label7.TabIndex = 36;
            this.label7.Text = "Tips";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(27, 405);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(192, 25);
            this.label6.TabIndex = 35;
            this.label6.Text = "Signature Location";
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(58, 611);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(393, 98);
            this.btnStop.TabIndex = 32;
            this.btnStop.Text = "Stop Tool...";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // txtStoreCode
            // 
            this.txtStoreCode.Location = new System.Drawing.Point(229, 271);
            this.txtStoreCode.Name = "txtStoreCode";
            this.txtStoreCode.Size = new System.Drawing.Size(972, 31);
            this.txtStoreCode.TabIndex = 31;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(58, 278);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(120, 25);
            this.label5.TabIndex = 30;
            this.label5.Text = "Store Code";
            // 
            // txtForderLog
            // 
            this.txtForderLog.Enabled = false;
            this.txtForderLog.Location = new System.Drawing.Point(229, 208);
            this.txtForderLog.Name = "txtForderLog";
            this.txtForderLog.Size = new System.Drawing.Size(972, 31);
            this.txtForderLog.TabIndex = 29;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(58, 215);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(128, 25);
            this.label4.TabIndex = 28;
            this.label4.Text = "Forder Logs";
            // 
            // txtForderPrint
            // 
            this.txtForderPrint.Enabled = false;
            this.txtForderPrint.Location = new System.Drawing.Point(229, 142);
            this.txtForderPrint.Name = "txtForderPrint";
            this.txtForderPrint.Size = new System.Drawing.Size(972, 31);
            this.txtForderPrint.TabIndex = 27;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(58, 149);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(125, 25);
            this.label3.TabIndex = 26;
            this.label3.Text = "Forder Print";
            // 
            // txtDomain
            // 
            this.txtDomain.Location = new System.Drawing.Point(229, 78);
            this.txtDomain.Name = "txtDomain";
            this.txtDomain.Size = new System.Drawing.Size(972, 31);
            this.txtDomain.TabIndex = 25;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(58, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 25);
            this.label2.TabIndex = 24;
            this.label2.Text = "Domain";
            // 
            // txtAcrobatURL
            // 
            this.txtAcrobatURL.Location = new System.Drawing.Point(229, 19);
            this.txtAcrobatURL.Name = "txtAcrobatURL";
            this.txtAcrobatURL.Size = new System.Drawing.Size(972, 31);
            this.txtAcrobatURL.TabIndex = 23;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(58, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 25);
            this.label1.TabIndex = 22;
            this.label1.Text = "Acrobat URL";
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(58, 485);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(532, 98);
            this.btnRun.TabIndex = 21;
            this.btnRun.Text = "Run Tool...";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnStopScreditCard
            // 
            this.btnStopScreditCard.Location = new System.Drawing.Point(690, 611);
            this.btnStopScreditCard.Name = "btnStopScreditCard";
            this.btnStopScreditCard.Size = new System.Drawing.Size(511, 98);
            this.btnStopScreditCard.TabIndex = 41;
            this.btnStopScreditCard.Text = "Stop Credit Card Connect...";
            this.btnStopScreditCard.UseVisualStyleBackColor = true;
            this.btnStopScreditCard.Click += new System.EventHandler(this.btnStopScreditCard_Click);
            // 
            // btnStartCreditCardConnect
            // 
            this.btnStartCreditCardConnect.Enabled = false;
            this.btnStartCreditCardConnect.Location = new System.Drawing.Point(690, 485);
            this.btnStartCreditCardConnect.Name = "btnStartCreditCardConnect";
            this.btnStartCreditCardConnect.Size = new System.Drawing.Size(511, 98);
            this.btnStartCreditCardConnect.TabIndex = 42;
            this.btnStartCreditCardConnect.Text = "Start Credit Card Connect...";
            this.btnStartCreditCardConnect.UseVisualStyleBackColor = true;
            this.btnStartCreditCardConnect.Click += new System.EventHandler(this.btnStartCreditCardConnect_Click);
            // 
            // chkTipsOn
            // 
            this.chkTipsOn.AutoSize = true;
            this.chkTipsOn.Location = new System.Drawing.Point(351, 352);
            this.chkTipsOn.Name = "chkTipsOn";
            this.chkTipsOn.Size = new System.Drawing.Size(72, 29);
            this.chkTipsOn.TabIndex = 43;
            this.chkTipsOn.Text = "On";
            this.chkTipsOn.UseVisualStyleBackColor = true;
            this.chkTipsOn.CheckedChanged += new System.EventHandler(this.chkTipsOn_CheckedChanged);
            // 
            // chkTipsOff
            // 
            this.chkTipsOff.AutoSize = true;
            this.chkTipsOff.Location = new System.Drawing.Point(547, 352);
            this.chkTipsOff.Name = "chkTipsOff";
            this.chkTipsOff.Size = new System.Drawing.Size(72, 29);
            this.chkTipsOff.TabIndex = 44;
            this.chkTipsOff.Text = "Off";
            this.chkTipsOff.UseVisualStyleBackColor = true;
            this.chkTipsOff.CheckedChanged += new System.EventHandler(this.chkTipsOff_CheckedChanged);
            // 
            // chkSigOnPaper
            // 
            this.chkSigOnPaper.AutoSize = true;
            this.chkSigOnPaper.Location = new System.Drawing.Point(351, 405);
            this.chkSigOnPaper.Name = "chkSigOnPaper";
            this.chkSigOnPaper.Size = new System.Drawing.Size(135, 29);
            this.chkSigOnPaper.TabIndex = 45;
            this.chkSigOnPaper.Text = "On Paper";
            this.chkSigOnPaper.UseVisualStyleBackColor = true;
            this.chkSigOnPaper.CheckedChanged += new System.EventHandler(this.chkSigOnPaper_CheckedChanged);
            // 
            // chkSigOnScreen
            // 
            this.chkSigOnScreen.AutoSize = true;
            this.chkSigOnScreen.Location = new System.Drawing.Point(547, 404);
            this.chkSigOnScreen.Name = "chkSigOnScreen";
            this.chkSigOnScreen.Size = new System.Drawing.Size(146, 29);
            this.chkSigOnScreen.TabIndex = 46;
            this.chkSigOnScreen.Text = "On Screen";
            this.chkSigOnScreen.UseVisualStyleBackColor = true;
            this.chkSigOnScreen.CheckedChanged += new System.EventHandler(this.chkSigOnScreen_CheckedChanged);
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(466, 611);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(124, 98);
            this.btnTest.TabIndex = 47;
            this.btnTest.Text = "Testing..";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1256, 818);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.chkSigOnScreen);
            this.Controls.Add(this.chkSigOnPaper);
            this.Controls.Add(this.chkTipsOff);
            this.Controls.Add(this.chkTipsOn);
            this.Controls.Add(this.btnStartCreditCardConnect);
            this.Controls.Add(this.btnStopScreditCard);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.txtStoreCode);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtForderLog);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtForderPrint);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtDomain);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtAcrobatURL);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Form1";
            this.Text = "POS Printer Tool";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Timer timer1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel lbPrinterStatus;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripStatusLabel lbCreditCardStatus;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TextBox txtStoreCode;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtForderLog;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtForderPrint;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDomain;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtAcrobatURL;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.Button btnStopScreditCard;
        private System.Windows.Forms.Button btnStartCreditCardConnect;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripLabel toolStripLabelTimer;
        private System.Windows.Forms.CheckBox chkTipsOn;
        private System.Windows.Forms.CheckBox chkTipsOff;
        private System.Windows.Forms.CheckBox chkSigOnPaper;
        private System.Windows.Forms.CheckBox chkSigOnScreen;
        private System.Windows.Forms.Button btnTest;
    }
}

