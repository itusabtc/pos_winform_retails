using DevExpress.XtraEditors;
using NailsChekin.Models;
using NailsChekin.Models.Helper;
using NailsChekin.Models.ListModel;
using NailsChekin.MyControls;
using NailsChekin.UserControl;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NailsChekin.Popup
{
    public partial class FormAddQuickItem : Form
    {
        Control parent = null;
        public string current_text = "";

        private Label      _lbInput;
        private RoundPanel _displayPanel;

        public FormAddQuickItem()
        {
            InitializeComponent();
        }

        public FormAddQuickItem(Control parent)
        {
            InitializeComponent();
            this.BackColor = ColorHelper.DefaultBackgoundColor;
            panelLeft.BorderColor = ColorHelper.DefaultBorderColor;

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();

            UIHelper.EnableDeepDoubleBuffer(this);
            typeof(Panel).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(panelCartItemsTouch, true, null);

            this.parent = parent;
        }

        private void FormAddQuickItem_Load(object sender, EventArgs e)
        {
            RebuildLayout();
            this.Adjust_Screen();

            var kb = new KeyBoardSearch
            {
                Dock               = DockStyle.Fill,
                AllowDecimal       = true,
                MaxLength          = 30,
                ButtonFontSize     = 26f,
                ButtonCornerRadius = 12,
                TargetControl      = txtBarcode  // still the data store
            };
            panelCart_Keyboard.Controls.Add(kb);

            // Replace txtBarcode visually with a centered display label
            txtBarcode.Visible = false;

            _lbInput = new Label
            {
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font      = new Font("Segoe UI", 26f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                BackColor = Color.Transparent,
                Text      = ""
            };

            _displayPanel = new RoundPanel
            {
                BackColor   = Color.White,
                BorderColor = Color.FromArgb(226, 232, 240),
                Name        = "displayPanel"
            };
            _displayPanel.Controls.Add(_lbInput);
            panelKeyboard.Controls.Add(_displayPanel);

            // Update display on every key tap
            kb.ValueChanged += (s, ev) => _lbInput.Text = kb.Value;

            // ENTER key: add formula as quick item then clear
            kb.EnterPressed += (s, ev) =>
            {
                string formula = txtBarcode.Text.Trim();
                if (!string.IsNullOrEmpty(formula))
                {
                    AddQuickPaymentItems(formula);
                    kb.Clear();  // triggers ValueChanged → label clears
                }
            };
        }

        private void RebuildLayout()
        {
            SuspendLayout();
            panelContent.SuspendLayout();

            // Move confirm button from panelContent into the right column (panelKeyboard)
            panelContent.Controls.Remove(btnFinish);
            btnFinish.Dock = DockStyle.None;            // manually positioned in PositionKeyboard
            panelKeyboard.Controls.Add(btnFinish);

            // Repurpose panelKeyboard as the right column
            panelKeyboard.Dock    = DockStyle.Right;
            panelKeyboard.Width   = 580;
            panelKeyboard.Padding = Padding.Empty;      // PositionKeyboard handles all margins

            // All inner controls will be manually positioned via PositionKeyboard
            txtBarcode.Dock         = DockStyle.None;
            panelCart_Keyboard.Dock = DockStyle.None;

            // After removing btnFinish: [panelKeyboard=0, panelLeft=1]
            // panelKeyboard must be at highest index → processed first → docks Right ✓
            panelContent.Controls.SetChildIndex(panelKeyboard, panelContent.Controls.Count - 1);

            // Item list fills remaining left area
            //panelLeft.Dock = DockStyle.Fill;
            panelLeft.Width = this.Width - panelKeyboard.Width - 10;
            panelLeft.Height = this.Height - panelHeader.Height - 10;

            panelContent.ResumeLayout(true);
            ResumeLayout(true);
        }

        public void Adjust_Screen()
        {
            btnClose.Location = new Point(Width - btnClose.Width - 10, btnClose.Location.Y);
            lbSubTotal.Left   = (panelHeader.Width - lbSubTotal.Width) / 2;
            // Use BeginInvoke so panelKeyboard has its final maximized size before positioning
            BeginInvoke(new Action(PositionKeyboard));
        }

        private void PositionKeyboard()
        {
            const int Pad  = 14;
            const int Gap  = 10;
            const int BtnH = 80;

            int W = panelKeyboard.ClientSize.Width;
            int H = panelKeyboard.ClientSize.Height;

            const int InpH = 66;  // display label height

            // Confirm button: bottom with margin
            btnFinish.SetBounds(Pad, H - Pad - BtnH, W - Pad * 2, BtnH);

            // Display label: top with margin (replaces txtBarcode)
            _displayPanel?.SetBounds(Pad, Pad, W - Pad * 2, InpH);

            // Available space between display label and confirm button
            int spaceTop    = Pad + InpH + Gap;
            int spaceBottom = H - Pad - BtnH - Gap;
            int avail       = Math.Max(0, spaceBottom - spaceTop);

            // Ideal height: number buttons square (4 cols → cell width = kbW/4 = squareH)
            // Row percentages: rows 0-3 = 21% each, row 4 = 16% → ratio 21:16 ≈ 1:0.76
            // Total ideal = squareH × 4 + squareH × 0.76 = squareH × 4.76
            int kbW     = W - Pad * 2;
            int squareH = kbW / 4;
            int kbH     = squareH * 4 + squareH * 76 / 100;
            kbH         = Math.Max(100, Math.Min(avail, kbH));   // cap at available space

            // Center keyboard vertically in the available space
            int kbTop = spaceTop + Math.Max(0, (avail - kbH) / 2);

            panelCart_Keyboard.SetBounds(Pad, kbTop, kbW, kbH);
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
                UCCartItem control = (UCCartItem)panelCartItemsTouch.Content.Controls[i];
                listItems.Add(new CartItemModel(control.item_id, control.item_name, control.quantity, control.price));
            }

            ((FormMain)parent).AddQuickPaymentItems(listItems);
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

        private void FormAddQuickItem_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Xử lý không bị cảm giác giật do xài Dispose() ngay nếu đóng thẳng
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000); // 3 giây

                try
                {
                    Core.ClearAndDisposeV2(panelCartItemsTouch.Content);
                    Core.ClearAndDisposeV2(panelLeft);
                    Core.ClearMemory();

                    if (!this.IsDisposed)
                    {
                        this.Invoke((Action)(() => this.Dispose()));
                    }
                }
                catch { /* form đã dispose rồi thì thôi */ }
            });
        }


        void keyboard_Click(object sender, EventArgs e)
        {
            SimpleButton button = sender as SimpleButton;

            switch (button.Text)
            {
                case "0":
                    current_text += button.Text;
                    break;
                case "1":
                    current_text += button.Text;
                    break;
                case "2":
                    current_text += button.Text;
                    break;
                case "3":
                    current_text += button.Text;
                    break;
                case "4":
                    current_text += button.Text;
                    break;
                case "5":
                    current_text += button.Text;
                    break;
                case "6":
                    current_text += button.Text;
                    break;
                case "7":
                    current_text += button.Text;
                    break;
                case "8":
                    current_text += button.Text;
                    break;
                case "9":
                    current_text += button.Text;
                    break;
                case ".":
                    current_text += button.Text;
                    break;
                case "*":
                    current_text += button.Text;
                    break;
                case "C":
                    current_text = "";
                    break;
                case "<<":
                    current_text = current_text.Length > 1 ? (current_text.Substring(0, current_text.Length - 1)) : "";
                    break;
            }

            txtBarcode.Text = current_text;
        }

        private void btnQuickPaymentConfirm_Click(object sender, EventArgs e)
        {
            this.AddQuickPaymentItems(this.current_text);
        }

        private void AddQuickPaymentItems(string quickSyntax)
        {
            if (string.IsNullOrEmpty(quickSyntax))
            {
                CustomMessageBox.Show("Erorr: Please check quick payment item");
                return;
            }

            if (!quickSyntax.Contains("*"))
            {
                string quantity = "1";
                string price = quickSyntax;

                this.AddQucikItemToCard(quantity, price, this.current_text);
            }
            else
            {
                int index = quickSyntax.IndexOf("*");
                string quantity = quickSyntax.Substring(0, index);
                string price = quickSyntax.Substring(index + 1, quickSyntax.Length - (index + 1));

                this.AddQucikItemToCard(quantity, price, this.current_text);
            }

            this.current_text = "";
            txtBarcode.Text = "";
        }

        public void AddQucikItemToCard(string quantity, string price, string barcode)
        {
            string item_id = "0";
            string item_name = "Item";

            var content = panelCartItemsTouch.Content;
            content.SuspendLayout();
            try
            {
                UCCartItem cardItem = new NailsChekin.UserControl.UCCartItem(this, item_id, item_name, price, quantity);
                cardItem.Width = panelCartItemsTouch.Width - 5;
                int shift = cardItem.Height + 5;

                // Dịch tất cả items hiện có xuống 1 lần, không repaint từng cái
                foreach (Control ctrl in content.Controls)
                    ctrl.Location = new Point(ctrl.Location.X, ctrl.Location.Y + shift);

                cardItem.Location = new Point(5, 5);
                content.Controls.Add(cardItem);
            }
            finally
            {
                content.ResumeLayout();
            }

            this.UpdatePaymentCartAmount();
        }

        public void RemoveCartItem(string item_id, string cart_item_id)
        {
            var content = panelCartItemsTouch.Content;

            UCCartItem target = null;
            foreach (UCCartItem ctrl in content.Controls.OfType<UCCartItem>())
            {
                if (ctrl.cart_item_id == cart_item_id)
                {
                    target = ctrl;
                    break;
                }
            }

            if (target == null) return;

            int removedY = target.Location.Y;
            int shift = target.Height + 5;

            content.SuspendLayout();
            try
            {
                content.Controls.Remove(target);

                foreach (Control ctrl in content.Controls)
                {
                    if (ctrl.Location.Y > removedY)
                        ctrl.Location = new Point(ctrl.Location.X, ctrl.Location.Y - shift);
                }

                target.MyDispose();
                target.Dispose();  //MyDispose không release window handle
            }
            finally
            {
                content.ResumeLayout();
            }

            this.UpdatePaymentCartAmount();
        }

        public void UpdatePaymentCartAmount()
        {
            try
            {
                double subTotal = 0;

                foreach (UCCartItem control in panelCartItemsTouch.Content.Controls.OfType<UCCartItem>())
                {
                    double quantity = double.Parse(control.quantity.Length <= 0 ? "0" : control.quantity);
                    double price = double.Parse(control.price.Length <= 0 ? "0" : control.price);
                    double discount = double.Parse(control.discount.Length <= 0 ? "0" : control.discount);

                    subTotal += (quantity * price);
                }

                lbSubTotal.Text = "TOTAL: $" + Math.Round(subTotal, 2);
            }
            catch { }
        }

    }
}
