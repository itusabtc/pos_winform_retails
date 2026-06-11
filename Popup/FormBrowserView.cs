using NailsChekin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NailsChekin.Popup
{
    public partial class FormBrowserView : Form
    {
        FormMain parent_form;

        //ChromiumWebBrowser _chromeBrowser = null;

        public FormBrowserView()
        {
            InitializeComponent();

            //this.InitBrowser();

            InitializeChromium();
        }

        public FormBrowserView(FormMain parent, string url)
        {
            InitializeComponent();

            this.parent_form = parent;

            //webBrowser1.Navigate(url);
            //this.InitBrowser();
        }

        private void FormBrowserView_Load(object sender, EventArgs e)
        {
            //webBrowser1.Navigate("http://pos.nailspaofamerica.com");

            //this.InitBrowser();
        }

        private void InitializeChromium()
        {
            //DisplayHandler displayer = new DisplayHandler();

            //CefSettings settings = new CefSettings();
            //Cef.Initialize(settings);
            //settings.CefCommandLineArgs.Add("disable-gpu", "1");

            //_chromeBrowser = new ChromiumWebBrowser("pos.nailspaofamerica.com");
            //_chromeBrowser.Margin = new Padding(0);
            //_chromeBrowser.Padding = new Padding(0);
            ////_chromeBrowser.ClientSize = new System.Drawing.Size(2320, 1264);

            //PnlBrowser.Controls.Add(_chromeBrowser);
            //_chromeBrowser.Dock = DockStyle.Fill;
            //this._chromeBrowser.DisplayHandler = displayer;


        }


        public void InitBrowser()
        {
            //Cef.Initialize(new CefSettings());
            //ChromiumWebBrowser browser = new ChromiumWebBrowser("google.com");
            //browser.Width = 500;
            //browser.Height = 500;
            //this.Controls.Add(browser);
            //browser.Dock = DockStyle.Fill;

            //ChromiumWebBrowser browser2 = new ChromiumWebBrowser("pos.nailspaofamerica.com");
            //browser2.Width = this.Width;
            //browser2.Height = this.Height;
            //browser2.Location = new Point(0, 0);

            //this.Controls.Add(browser2);
            //browser2.Dock = DockStyle.Fill;

        }

        private void btnClose_ElementClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
        {
            this.Dispose();
        }

        private void navMinimize_ElementClick(object sender, DevExpress.XtraBars.Navigation.NavElementEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
    
}
