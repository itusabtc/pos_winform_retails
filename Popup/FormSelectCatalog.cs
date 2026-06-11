using NailsChekin.Models;
using NailsChekin.Models.Helper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NailsChekin.Popup
{
    public partial class FormSelectCatalog : Form
    {
        FormNewItem parentForm = null;

        public FormSelectCatalog()
        {
            InitializeComponent();
        }

        public FormSelectCatalog(FormNewItem parent)
        {
            InitializeComponent();

            this.BackColor = ColorHelper.DefaultBackgoundColor;
            panelLeft.BorderColor = ColorHelper.DefaultBorderColor;

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();

            UIHelper.EnableDeepDoubleBuffer(this);
            typeof(Panel).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(panelCartItemsTouch, true, null);

            this.parentForm = parent;

        }

        private void FormSelectCatalog_Load(object sender, EventArgs e)
        {
            this.Adjust_Screen();

            _ = this.GetCatalogAsync();
        }

        public void Adjust_Screen()
        {
            //Bất đồng bộ
            this.BeginInvoke(new Action(() =>
            {
                btnClose.Location = new Point(this.Width - btnClose.Width - 10, btnClose.Location.Y);
            }));
        }

        private async Task GetCatalogAsync()
        {
            // Gọi API trên background thread — không block UI
            string responce = await Task.Run(() => Utilitys.CALL_API("Catalog", "", "GET", true));

            if (responce.StartsWith("Error"))
            {
                MessageBox.Show(responce);
                return;
            }

            // Parse JSON trên background thread
            var items = await Task.Run(() =>
            {
                var list = new List<(string id, string name)>();
                foreach (JObject obj in JArray.Parse(responce))
                    list.Add((obj["id"].ToString(), obj["name"].ToString()));
                return list;
            });

            // Draw controls trên UI thread
            DrawCatalogItems(items);
        }

        private void DrawCatalogItems(List<(string id, string name)> items)
        {
            const int numberColumn = 5;
            const int padding     = 12;  // khoảng cách viền ngoài
            const int hGap        = 20;  // khoảng cách ngang giữa các cột
            const int vGap        = 20;  // khoảng cách dọc giữa các hàng

            int itemWidth = (panelCartItemsTouch.Width - padding * 2 - hGap * (numberColumn - 1)) / numberColumn;

            // Tạo tất cả controls trước khi add vào panel
            var controls = new NailsChekin.UserControl.UCMenuItem[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                var ctrl = new NailsChekin.UserControl.UCMenuItem(items[i].id, items[i].name, "", "0", this);
                ctrl.Width = itemWidth;
                controls[i] = ctrl;
            }

            // Lấy height thực tế từ control (tránh lệch khi DPI scale)
            int itemHeight = controls.Length > 0 ? controls[0].Height : 55;

            // Tính vị trí cho từng control
            int locationX = padding, locationY = padding;
            for (int i = 0; i < controls.Length; i++)
            {
                controls[i].Location = new Point(locationX, locationY);

                if ((i + 1) % numberColumn == 0)
                {
                    locationY += itemHeight + vGap;
                    locationX  = padding;
                }
                else
                {
                    locationX += itemWidth + hGap;
                }
            }

            // Dispose controls cũ, add tất cả mới 1 lần duy nhất
            panelCartItemsTouch.Content.SuspendLayout();
            try
            {
                foreach (Control old in panelCartItemsTouch.Content.Controls)
                    old.Dispose();

                panelCartItemsTouch.Content.Controls.Clear();
                panelCartItemsTouch.Content.Controls.AddRange(controls);
            }
            finally
            {
                panelCartItemsTouch.Content.ResumeLayout();
            }
        }


        public string catalog_selected_id = "";
        public string catalog_selected_name = "";
        public void SetCatalogSelected(string id, string name)
        {
            this.catalog_selected_id = id;
            this.catalog_selected_name = name;

            parentForm.SetCatalogSelected(id, name);
            this.Close();
        }

        private void FormSelectCatalog_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Xử lý không bị cảm giác giật do xài Dispose() ngay nếu đóng thẳng
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000); // 3 giây

                try
                {
                    Core.ClearAndDisposeV2(panelCartItemsTouch.Content);
                    Core.ClearAndDisposeV2(panelContent);
                    Core.ClearMemory();

                    if (!this.IsDisposed)
                    {
                        this.Invoke((Action)(() => this.Dispose()));
                    }
                }
                catch { /* form đã dispose rồi thì thôi */ }
            });
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
