using System;
using System.Drawing;
using System.Windows.Forms;
using NailsChekin;
using NailsChekin.Popup;

namespace NailsChekin.UserControl
{
    public partial class UCMenuItem : DevExpress.XtraEditors.XtraUserControl
    {
        public string id = "";
        public string name = "";
        public string price = "";
        public string time = "30";
        public string img_index = "0";

        public Control parent;

        public UCMenuItem()
        {
            InitializeComponent();
        }

        public UCMenuItem(string id, string name, string price, string img_index, Control parent)
        {
            InitializeComponent();

            this.id = id;
            this.name = name.ToUpper();
            this.price = price;
            this.img_index = img_index;

            btnMenu.Text = name.ToUpper();

            this.parent = parent;
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.SetSelected(Color.Orange);

            if (this.parent is FormSelectCatalog)
                ((FormSelectCatalog)this.parent).SetCatalogSelected(this.id, this.name);
            else if (this.parent is FormSelectSubCatalog)
                ((FormSelectSubCatalog)this.parent).SetSubCatalogSelected(this.id, this.name);
        }

        public void SetSelected(Color color)
        {
            this.btnMenu.Appearance.BackColor = color;
            //this.btnName.Text = "Selected";
        }

    }
}
