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
    public partial class FormSearchItemResult : Form
    {
        public bool is_search_sku = true;
        FormMain parent;

        public FormSearchItemResult()
        {
            InitializeComponent();
        }

        public FormSearchItemResult(FormMain parent, string barcode, bool is_search_sku)
        {
            InitializeComponent();

            this.parent = parent;
            txtSearchItemBarcode.Text = barcode;
            this.is_search_sku = is_search_sku;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            FormNewItem frm = new FormNewItem(parent, txtSearchItemBarcode.Text, this.is_search_sku);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog(this);
            frm.Dispose();

            this.Close();
        }

        private void FormSearchItemResult_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Xử lý không bị cảm giác giật do xài Dispose() ngay nếu đóng thẳng
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000); // 3 giây

                try
                {
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
