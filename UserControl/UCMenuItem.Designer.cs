namespace NailsChekin.UserControl
{
    partial class UCMenuItem
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCMenuItem));
            this.btnMenu = new DevExpress.XtraEditors.SimpleButton();
            this.SuspendLayout();
            // 
            // btnMenu
            // 
            this.btnMenu.Appearance.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnMenu.Appearance.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMenu.Appearance.Options.UseBackColor = true;
            this.btnMenu.Appearance.Options.UseFont = true;
            this.btnMenu.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMenu.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
            this.btnMenu.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnMenu.ImageOptions.SvgImage")));
            this.btnMenu.Location = new System.Drawing.Point(0, 0);
            this.btnMenu.Margin = new System.Windows.Forms.Padding(2);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(222, 55);
            this.btnMenu.TabIndex = 26;
            this.btnMenu.Text = "Manicure";
            this.btnMenu.Click += new System.EventHandler(this.btnMenu_Click);
            // 
            // UCMenuItem
            // 
            this.Appearance.Font = new System.Drawing.Font("Tahoma", 9F);
            this.Appearance.Options.UseFont = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.btnMenu);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "UCMenuItem";
            this.Size = new System.Drawing.Size(222, 55);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnMenu;
    }
}
