using NailsChekin.Models;
using NailsChekin.Models.Helper;
using NailsChekin.Models.ListModel;
using NailsChekin.MyControls;
using NailsChekin.UserControl;
using Newtonsoft.Json.Linq;
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
    public partial class FormConfirmSelectItemResult : Form
    {
        Control parent;
        JArray items;

        public FormConfirmSelectItemResult()
        {
            InitializeComponent();
        }

        public FormConfirmSelectItemResult(Control parent, JArray items)
        {
            InitializeComponent();

            panelLeft.BorderColor = ColorHelper.DefaultBorderColor;

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();

            UIHelper.EnableDeepDoubleBuffer(this);
            typeof(Panel).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(panelCartItemsTouch, true, null);

            this.parent = parent;
            this.items = items;
        }

        private void FormConfirmSelectItemResult_Load(object sender, EventArgs e)
        {
            this.Adjust_Screen();

            this.DrawCartView();
        }

        private void DrawCartView()
        {
            var content   = panelCartItemsTouch.Content;
            int itemWidth = panelCartItemsTouch.Width;

            // Tạo tất cả controls + tính vị trí trước khi add vào panel
            var controls = new UCCartItemSelect[items.Count];
            int locationY = 5;
            for (int i = 0; i < items.Count; i++)
            {
                JObject obj      = (JObject)items[i];
                string item_id   = obj["id"].ToString();
                string item_name = obj["name"].ToString();
                string price     = obj["price"].ToString();

                var card      = new UCCartItemSelect(parent, item_id, item_name, price, "1");
                card.Width    = itemWidth;
                card.Location = new Point(0, locationY);
                locationY    += card.Height + 5;
                controls[i]   = card;
            }

            // Dispose controls cũ, add tất cả mới 1 lần duy nhất
            content.SuspendLayout();
            try
            {
                foreach (Control old in content.Controls)
                    old.Dispose();

                content.Controls.Clear();
                content.Controls.AddRange(controls);
            }
            finally
            {
                content.ResumeLayout();
            }
        }

        public void Adjust_Screen()
        {
            //Bất đồng bộ
            this.BeginInvoke(new Action(() =>
            {
                btnClose.Location = new Point(this.Width - btnClose.Width - 10, btnClose.Location.Y);
            }));
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            int numberElement = panelCartItemsTouch.Content.Controls.Count;
            if (numberElement <= 0)
            {
                CustomMessageBox.Show("Please select quick item !!!");
                return;
            }

            List<CartItemModel> listItems = new List<CartItemModel>();
            for (int i = 0; i < numberElement; i++)
            {
                UCCartItemSelect control = (UCCartItemSelect)panelCartItemsTouch.Content.Controls[i];
                if(control.selected)
                    listItems.Add(new CartItemModel(control.item_id, control.item_name, control.quantity, control.price));
            }

            ((FormMain)parent).AddConfirmSelectItems(listItems);
            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormConfirmSelectItemResult_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Xử lý không bị cảm giác giật do xài Dispose() ngay nếu đóng thẳng
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000); // 3 giây

                try
                {
                    Core.ClearMemory();

                    if (!this.IsDisposed)
                    {
                        this.Invoke((Action)(() => this.Dispose()));
                    }
                }
                catch { /* form đã dispose rồi thì thôi */ }
            });
        }

        
    }
}
