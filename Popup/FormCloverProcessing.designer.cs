namespace NailsChekin.Popup
{
    partial class FormCloverProcessing
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCloverProcessing));
            this.panelControls = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.StatusPanel = new System.Windows.Forms.Panel();
            this.UIStateButtonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.DeviceCurrentStatus = new System.Windows.Forms.Label();
            this.svgImageBox1 = new DevExpress.XtraEditors.SvgImageBox();
            this.panelControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.StatusPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.svgImageBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControls
            // 
            this.panelControls.Controls.Add(this.pictureBox1);
            this.panelControls.Controls.Add(this.label1);
            this.panelControls.Location = new System.Drawing.Point(295, 60);
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size(536, 504);
            this.panelControls.TabIndex = 4;
            // 
            // pictureBox1
            // 
            //this.pictureBox1.Image = global::NailsChekin.Properties.Resources.arico_loading;
            //this.pictureBox1.Location = new System.Drawing.Point(42, 20);
            //this.pictureBox1.Name = "pictureBox1";
            //this.pictureBox1.Size = new System.Drawing.Size(444, 365);
            //this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            //this.pictureBox1.TabIndex = 2;
            //this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(44, 419);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(463, 42);
            this.label1.TabIndex = 1;
            this.label1.Text = "Processing, Please Wait....";
            // 
            // StatusPanel
            // 
            this.StatusPanel.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.StatusPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.StatusPanel.Controls.Add(this.UIStateButtonPanel);
            this.StatusPanel.Controls.Add(this.DeviceCurrentStatus);
            this.StatusPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.StatusPanel.Location = new System.Drawing.Point(0, 612);
            this.StatusPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.StatusPanel.Name = "StatusPanel";
            this.StatusPanel.Size = new System.Drawing.Size(1150, 168);
            this.StatusPanel.TabIndex = 28;
            // 
            // UIStateButtonPanel
            // 
            this.UIStateButtonPanel.BackColor = System.Drawing.SystemColors.Control;
            this.UIStateButtonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UIStateButtonPanel.Location = new System.Drawing.Point(0, 61);
            this.UIStateButtonPanel.Margin = new System.Windows.Forms.Padding(6);
            this.UIStateButtonPanel.MinimumSize = new System.Drawing.Size(20, 16);
            this.UIStateButtonPanel.Name = "UIStateButtonPanel";
            this.UIStateButtonPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 25);
            this.UIStateButtonPanel.Size = new System.Drawing.Size(1148, 105);
            this.UIStateButtonPanel.TabIndex = 21;
            // 
            // DeviceCurrentStatus
            // 
            this.DeviceCurrentStatus.BackColor = System.Drawing.SystemColors.Control;
            this.DeviceCurrentStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.DeviceCurrentStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DeviceCurrentStatus.Location = new System.Drawing.Point(0, 0);
            this.DeviceCurrentStatus.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.DeviceCurrentStatus.Name = "DeviceCurrentStatus";
            this.DeviceCurrentStatus.Size = new System.Drawing.Size(1148, 61);
            this.DeviceCurrentStatus.TabIndex = 24;
            this.DeviceCurrentStatus.Text = "Device Current Status";
            // 
            // svgImageBox1
            // 
            this.svgImageBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.svgImageBox1.Location = new System.Drawing.Point(1066, 12);
            this.svgImageBox1.Name = "svgImageBox1";
            this.svgImageBox1.Size = new System.Drawing.Size(65, 63);
            this.svgImageBox1.SizeMode = DevExpress.XtraEditors.SvgImageSizeMode.Stretch;
            this.svgImageBox1.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("svgImageBox1.SvgImage")));
            this.svgImageBox1.TabIndex = 29;
            this.svgImageBox1.Text = "svgImageBox1";
            this.svgImageBox1.Click += new System.EventHandler(this.svgImageBox1_Click);
            // 
            // FormCloverProcessing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1150, 780);
            this.Controls.Add(this.svgImageBox1);
            this.Controls.Add(this.StatusPanel);
            this.Controls.Add(this.panelControls);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormCloverProcessing";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ProcessPopupForm";
            this.panelControls.ResumeLayout(false);
            this.panelControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.StatusPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.svgImageBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel StatusPanel;
        private System.Windows.Forms.FlowLayoutPanel UIStateButtonPanel;
        private System.Windows.Forms.Label DeviceCurrentStatus;
        private DevExpress.XtraEditors.SvgImageBox svgImageBox1;
    }
}