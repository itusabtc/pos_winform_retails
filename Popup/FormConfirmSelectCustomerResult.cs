using NailsChekin.Models;
using NailsChekin.Models.Helper;
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
    public partial class FormConfirmSelectCustomerResult : Form
    {
        Control parent;
        JArray items;

        public FormConfirmSelectCustomerResult()
        {
            InitializeComponent();
        }

        public FormConfirmSelectCustomerResult(Control parent, JArray items)
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

        private void FormConfirmSelectCustomerResult_Load(object sender, EventArgs e)
        {
            this.Adjust_Screen();

            this.DrawCartView();
        }

        private void DrawCartView()
        {
            var content = panelCartItemsTouch.Content;
            int itemWidth = panelCartItemsTouch.Width;

            // Tạo tất cả controls + tính vị trí trước khi add vào panel
            var controls = new UCCustomerSearchResult[items.Count];
            int locationY = 5;
            for (int i = 0; i < items.Count; i++)
            {
                JObject obj = (JObject)items[i];
                string id = obj["id"].ToString();
                string phone = obj["phone"].ToString();
                string firstName = obj["firstName"].ToString();
                string lastName = obj["lastName"].ToString();
                string birthday = obj["birthday"].ToString();
                string address = obj["address"].ToString();

                var card = new UCCustomerSearchResult(this, id, phone, firstName, lastName, birthday, address);
                card.Width = itemWidth;
                card.Location = new Point(0, locationY);
                locationY += card.Height + 5;
                controls[i] = card;
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

        public void SetCustomerInfo(string id, string phone, string firstname, string lastname)
        {
            ((FormMain)parent).SetCustomerInfo(id, phone, firstname, lastname);
            this.Close();
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            int numberElement = panelCartItemsTouch.Content.Controls.Count;
            if (numberElement <= 0)
            {
                CustomMessageBox.Show("Please select customer !!!");
                return;
            }

            //List<CartItemModel> listItems = new List<CartItemModel>();
            //for (int i = 0; i < numberElement; i++)
            //{
            //    UCCartItemSelect control = (UCCartItemSelect)panelCartItemsTouch.Content.Controls[i];
            //    if (control.selected)
            //        listItems.Add(new CartItemModel(control.item_id, control.item_name, control.quantity, control.price));
            //}

            //((FormMain)parent).AddConfirmSelectItems(listItems);
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

        private void FormConfirmSelectCustomerResult_FormClosed(object sender, FormClosedEventArgs e)
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
