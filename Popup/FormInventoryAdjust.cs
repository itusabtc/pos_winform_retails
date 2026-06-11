using Newtonsoft.Json.Linq;
using NailsChekin.UserControl;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using NailsChekin.Models;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NailsChekin.Models.Helper;

namespace NailsChekin.Popup
{
    public partial class FormInventoryAdjust : Form
    {
        FormMain parentForm = null;

        public FormInventoryAdjust()
        {
            InitializeComponent();
        }

        public FormInventoryAdjust(FormMain parent, string search)
        {
            InitializeComponent();

            this.BackColor = ColorHelper.DefaultBackgoundColor;
            panelLayout_Content.BorderColor = ColorHelper.DefaultBorderColor;

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();

            UIHelper.EnableDeepDoubleBuffer(this);

            this.parentForm = parent;
            txtSearchKey.Text = search;

            InitBarcodeHandling();
        }

        private async void FormTipsAdjust_Load(object sender, EventArgs e)
        {
            this.Adjust_Screen();

            await this.SendSearchItemLockUpAsync(txtSearchKey.Text, true);

            this.Activate();
            txtSearchKey.Focus();
            txtSearchKey.SelectionStart = txtSearchKey.Text.Length;
        }

        private void Adjust_Screen()
        {
            btnClose.Location = new Point(this.Width - btnClose.Width - 10, btnClose.Location.Y);
            panelLayout_Content.Width = this.Width - 20;
            panelLayout_Content.Height = this.Height - panelHeader.Height - 10;
        }

        private void InitBarcodeHandling()
        {
            this.KeyPreview = true;

            txtSearchKey.PreviewKeyDown -= TxtSearchKey_PreviewKeyDown;
            txtSearchKey.PreviewKeyDown += TxtSearchKey_PreviewKeyDown;
            txtSearchKey.KeyDown -= TxtSearchKey_KeyDown;
            txtSearchKey.KeyDown += TxtSearchKey_KeyDown;
        }

        private void TxtSearchKey_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                e.IsInputKey = true;
        }

        private async void TxtSearchKey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string value = txtSearchKey.Text.Trim();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    await SendSearchItemLockUpAsync(value, true);
                }

                txtSearchKey.Focus();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private async void btnFindNow_Click(object sender, EventArgs e)
        {
            await SendSearchItemLockUpAsync(txtSearchKey.Text, true);
        }

        // ---- Search / Load items ----

        public async Task SendSearchItemLockUpAsync(string search, bool clearFirst = false)
        {
            if (clearFirst)
            {
                if (string.IsNullOrEmpty(search))
                    search = txtSearchKey.Text;

                ClearItems();
            }

            string cleanSearch = Regex.Replace(search ?? "", @"[\r\n]", "");
            string DATA = @"{
                'category_id':null,
                'category_slugname':null,
                'searchString':'" + cleanSearch + @"',
                'pageIndex':0,
                'pageSize':100
            }";

            string responce = await Task.Run(() => Utilitys.CALL_API("Product/search", DATA, "POST", true));

            if (this.IsDisposed) return;

            if (responce.StartsWith("Error"))
            {
                MessageBox.Show(responce);
                return;
            }

            JArray jArray = JArray.Parse(responce);
            int locationY = 5;
            foreach (JObject obj in jArray)
            {
                string item_id = obj["id"].ToString();
                string barcode = obj["barcode"].ToString();
                string item_name = obj["name"].ToString();
                string catalog = obj["category_Name"].ToString();
                string price = obj["price"].ToString();
                string qty = obj["quantity"].ToString();

                UCInventoryLine control = new UCInventoryLine(item_id, barcode, item_name, catalog, price, qty);
                control.Width = panelCartItemsTouch.Width - 5;
                control.Location = new Point(0, locationY);
                locationY += control.Height + 5;
                panelCartItemsTouch.Content.Controls.Add(control);
            }

            txtSearchKey.Text = "";
            txtSearchKey.Focus();
        }

        public async void ResetSendSearchItemLockUp(string search, bool reload_update = false)
        {
            txtSearchKey.Text = search;
            await SendSearchItemLockUpAsync(search, reload_update);
            txtSearchKey.Focus();
        }

        private void ClearItems()
        {
            var items = panelCartItemsTouch.Content.Controls.OfType<UCInventoryLine>().ToList();
            foreach (var ctrl in items)
            {
                panelCartItemsTouch.Content.Controls.Remove(ctrl);
                ctrl.Dispose();
            }
        }

        // ---- Close / Dispose ----

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormInventoryAdjust_FormClosed(object sender, FormClosedEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000);

                try
                {
                    Core.ClearAndDisposeV2(panelCartItemsTouch.Content);
                    Core.ClearMemory();
                }
                catch { }

                if (!this.IsDisposed)
                {
                    try { this.Invoke((Action)(() => this.Dispose())); } catch { }
                }
            });
        }
    }
}
