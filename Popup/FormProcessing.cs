using System;
using System.Drawing;
using System.Windows.Forms;

namespace NailsChekin.Popup
{
    public partial class FormProcessing : Form
    {
        FormMain parentForm = null;

        public FormProcessing()
        {
            InitializeComponent();
        }

        public FormProcessing(FormMain parent)
        {
            InitializeComponent();

            this.parentForm = parent;
        }

        private void FormProcessing_Load(object sender, EventArgs e)
        {
            //Align Center
            int width = this.Width;

            int top = panelControls.Location.Y;
            int left = panelControls.Left;
            int right = panelControls.Right;

            int offset = (width - right + left) / 2;
            panelControls.Location = new Point(offset, top);


        }
    }
}
