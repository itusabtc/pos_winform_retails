
namespace NailsChekin.Popup
{
    partial class FormMessageYesNo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMessageYesNo));
            this.btnNO = new NailsChekin.MyControls.ButtonRound();
            this.btnYES = new NailsChekin.MyControls.ButtonRound();
            this.lbTitle = new System.Windows.Forms.Label();
            this.lbMessage = new System.Windows.Forms.Label();
            this.btnClose = new DevExpress.XtraEditors.SvgImageBox();
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).BeginInit();
            this.SuspendLayout();
            // 
            // btnNO
            // 
            this.btnNO.BackColor = System.Drawing.Color.Transparent;
            this.btnNO.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnNO.ButtonPadding = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.btnNO.ClickLocked = false;
            this.btnNO.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNO.DisabledOverlayColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnNO.DisabledOverlayFont = null;
            this.btnNO.DisabledOverlayForeColor = System.Drawing.Color.White;
            this.btnNO.DisabledOverlayText = "Processing...";
            this.btnNO.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNO.Location = new System.Drawing.Point(18, 308);
            this.btnNO.MinimumSize = new System.Drawing.Size(120, 36);
            this.btnNO.Name = "btnNO";
            this.btnNO.Selected = false;
            this.btnNO.Size = new System.Drawing.Size(330, 85);
            this.btnNO.TabIndex = 193;
            this.btnNO.Title = "NO";
            this.btnNO.TitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnNO.TitleFontSize = 22F;
            this.btnNO.Click += new System.EventHandler(this.btnNO_Click);
            // 
            // btnYES
            // 
            this.btnYES.BackColor = System.Drawing.Color.Transparent;
            this.btnYES.BorderColor = System.Drawing.Color.Green;
            this.btnYES.ButtonPadding = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.btnYES.ClickLocked = false;
            this.btnYES.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnYES.DisabledOverlayColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnYES.DisabledOverlayFont = null;
            this.btnYES.DisabledOverlayForeColor = System.Drawing.Color.White;
            this.btnYES.DisabledOverlayText = "Processing...";
            this.btnYES.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnYES.Location = new System.Drawing.Point(665, 308);
            this.btnYES.MinimumSize = new System.Drawing.Size(120, 36);
            this.btnYES.Name = "btnYES";
            this.btnYES.Selected = false;
            this.btnYES.Size = new System.Drawing.Size(319, 85);
            this.btnYES.TabIndex = 192;
            this.btnYES.Title = "YES";
            this.btnYES.TitleBackColor = System.Drawing.Color.Green;
            this.btnYES.TitleFontSize = 22F;
            this.btnYES.Click += new System.EventHandler(this.btnYES_Click);
            // 
            // lbTitle
            // 
            this.lbTitle.AutoSize = true;
            this.lbTitle.BackColor = System.Drawing.Color.Transparent;
            this.lbTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbTitle.ForeColor = System.Drawing.Color.Red;
            this.lbTitle.Location = new System.Drawing.Point(11, 9);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(331, 39);
            this.lbTitle.TabIndex = 195;
            this.lbTitle.Text = "CONFIRM ACTION";
            this.lbTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbMessage
            // 
            this.lbMessage.AutoSize = true;
            this.lbMessage.BackColor = System.Drawing.Color.Transparent;
            this.lbMessage.Font = new System.Drawing.Font("Tahoma", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbMessage.ForeColor = System.Drawing.Color.Black;
            this.lbMessage.Location = new System.Drawing.Point(40, 120);
            this.lbMessage.Name = "lbMessage";
            this.lbMessage.Size = new System.Drawing.Size(169, 42);
            this.lbMessage.TabIndex = 196;
            this.lbMessage.Text = "MESSAGE";
            this.lbMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnClose
            // 
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Location = new System.Drawing.Point(942, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(47, 50);
            this.btnClose.SizeMode = DevExpress.XtraEditors.SvgImageSizeMode.Stretch;
            this.btnClose.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnClose.SvgImage")));
            this.btnClose.TabIndex = 197;
            this.btnClose.Text = "svgCustomerReload";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FormMessageYesNo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(996, 405);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lbMessage);
            this.Controls.Add(this.lbTitle);
            this.Controls.Add(this.btnNO);
            this.Controls.Add(this.btnYES);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormMessageYesNo";
            this.Text = "FormMessageYesNo";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMessageYesNo_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.btnClose)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MyControls.ButtonRound btnNO;
        private MyControls.ButtonRound btnYES;
        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.Label lbMessage;
        private DevExpress.XtraEditors.SvgImageBox btnClose;
    }
}