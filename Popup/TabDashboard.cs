using System.Drawing;
using System.Threading.Tasks;
using NailsChekin.UserControl;
using Newtonsoft.Json.Linq;
using NailsChekin.Models.Helper;
using NailsChekin.MyControls;
using System.Threading;
using System;

namespace NailsChekin.Popup
{
    public partial class TabDashboard : DevExpress.XtraEditors.XtraUserControl, ILoadable
    {
        private bool _loaded;                         // đánh dấu đã load
        private readonly SemaphoreSlim _gate = new SemaphoreSlim(1, 1); // chống double-click

        public TabDashboard()
        {
            InitializeComponent();
        }

        private void TabDashboard_Load(object sender, System.EventArgs e)
        {
            
        }

        public async Task EnsureLoadedAsync(CancellationToken ct)
        {
            //if (_loaded) return;  //Phải reload chứ
            this.Controls.Clear();

            await _gate.WaitAsync(ct);
            try
            {
                //if (_loaded) return;

                // Nếu control đã bị dispose / chưa tạo handle thì đảm bảo có handle trước khi vẽ
                //if (!IsHandleCreated) CreateControl();
                if (ct.IsCancellationRequested) return;

                // GỌI HÀM VẼ UI (hàm này đang thêm control trực tiếp vào this.Controls)
                await LoadDataAsync();                // LoadDataAsync đã chạy trên UI thread

                //if (!ct.IsCancellationRequested)
                //    _loaded = true;                   // tránh load lại lần sau
            }
            finally
            {
                _gate.Release();
            }
        }

        public async Task LoadDataAsync()
        {
            //MainPOS mainPOS = new MainPOS();
            //string jData = mainPOS.Get_Dashboard();

            //if (jData.Trim().Length > 20)
            //{
            //    JArray historys = JArray.Parse(jData);

            //    double total_service = 0;
            //    double total_supply = 0;
            //    double total_supply_plus = 0;
            //    double cash = 0;
            //    double credit_debit = 0;
            //    double cash_app = 0;
            //    double member = 0;
            //    double repair_amount = 0;
            //    double prepaid = 0;
            //    double giftcard = 0;
            //    double giftcard_sale = 0;
            //    double giftcard_sale_credit = 0;
            //    double giftcard_sale_cash = 0;
            //    double discount = 0;
            //    double cash_discount = 0;
            //    double tax = 0;
            //    double bar = 0;
            //    double surCharge = 0;
            //    double reward = 0;
            //    double counpon = 0;
            //    double credit_tip = 0;
            //    double cash_tip = 0;
            //    double product_sale = 0;
            //    double total_credit_transaction = 0;
            //    double total_cash_received = 0;
            //    double adjust = 0;

            //    foreach (JObject item in historys)
            //    {
            //        total_service = Math.Round(double.Parse(item["total_service"].ToString()), 2);
            //        total_supply = Math.Round(double.Parse(item["total_supply"].ToString()), 2);
            //        total_supply_plus = Math.Round(double.Parse(item["total_supply_plus"].ToString()), 2);
            //        credit_debit = Math.Round(double.Parse(item["credit_debit"].ToString()), 2);
            //        cash = Math.Round(double.Parse(item["cash"].ToString()), 2);
            //        total_cash_received = Math.Round(double.Parse(item["total_cash_received"].ToString()), 2);
            //        prepaid = Math.Round(double.Parse(item["prepaid"].ToString()), 2);
            //        cash_app = Math.Round(double.Parse(item["cash_app"].ToString()), 2);
            //        member = Math.Round(double.Parse(item["member"].ToString()), 2);
            //        repair_amount = Math.Round(double.Parse(item["repair_amount"].ToString()), 2);
            //        giftcard = Math.Round(double.Parse(item["giftcard"].ToString()), 2);
            //        giftcard_sale = Math.Round(double.Parse(item["giftcard_sale"].ToString()), 2);
            //        giftcard_sale_credit = Math.Round(double.Parse(item["giftcard_sale_credit"].ToString()), 2);
            //        giftcard_sale_cash = giftcard_sale - giftcard_sale_credit;

            //        discount = Math.Round(double.Parse(item["discount"].ToString()), 2);
            //        cash_discount = Math.Round(double.Parse(item["cash_discount"].ToString()), 2);
            //        tax = Math.Round(double.Parse(item["tax"].ToString()), 2);
            //        bar = Math.Round(double.Parse(item["bar"].ToString()), 2);
            //        surCharge = Math.Round(double.Parse(item["surCharge"].ToString()), 2);
            //        reward = Math.Round(double.Parse(item["reward"].ToString()), 2);
            //        counpon = Math.Round(double.Parse(item["counpon"].ToString()), 2);
            //        credit_tip = Math.Round(double.Parse(item["credit_tip"].ToString()), 2);
            //        cash_tip = Math.Round(double.Parse(item["cash_tip"].ToString()), 2);
            //        product_sale = Math.Round(double.Parse(item["product_sale"].ToString()), 2);

            //        adjust = Math.Round(double.Parse(item["adjust"].ToString()), 2);
            //    }

            //    int locationX = 10;
            //    int locationY = 10;
            //    int control_width = ((this.Width - 30) / 5) - 10;

            //    string[] colors = new string[] { "FBBC05", "4285F4", "34A853", "EA4335" };
            //    Random rd = new Random();

            //    UCDashboardRound control = new UCDashboardRound(total_service.ToString(), "TOTAL SERVICE", colors[rd.Next(0, 3)]) { CardBaseColor = ColorHelper.Success };
            //    control.Width = control_width;
            //    control.Location = new Point(locationX, locationY);
            //    locationX += control_width + 10;
            //    this.Controls.Add(control);

            //    control = new UCDashboardRound(total_cash_received.ToString(), "CASH RECEIVE", colors[rd.Next(0, 3)]) { CardBaseColor = ColorHelper.Success };
            //    control.Width = control_width;
            //    control.Location = new Point(locationX, locationY);
            //    locationX += control_width + 10;
            //    this.Controls.Add(control);

            //    control = new UCDashboardRound(cash.ToString(), "CASH", colors[rd.Next(0, 3)]) { CardBaseColor = ColorHelper.Success };
            //    control.Width = control_width;
            //    control.Location = new Point(locationX, locationY);
            //    locationX += control_width + 10;
            //    this.Controls.Add(control);

            //    control = new UCDashboardRound(credit_debit.ToString(), "CREDIT", colors[rd.Next(0, 3)]) { CardBaseColor = ColorHelper.Success };
            //    control.Width = control_width;
            //    control.Location = new Point(locationX, locationY);
            //    locationX += control_width + 10;
            //    this.Controls.Add(control);

            //    control = new UCDashboardRound(cash_app.ToString(), "CASH APP", colors[rd.Next(0, 3)]) { CardBaseColor = ColorHelper.Success };
            //    control.Width = control_width;
            //    control.Location = new Point(locationX, locationY);
            //    locationX += control_width + 10;
            //    this.Controls.Add(control);

            //    //Reset row
            //    locationX = 10;
            //    locationY = locationY + control.Height + 10;

            //    control = new UCDashboardRound(member.ToString(), "MEMBER", colors[rd.Next(0, 3)]) { CardBaseColor = ColorHelper.Danger };
            //    control.Width = control_width;
            //    control.Location = new Point(locationX, locationY);
            //    locationX += control_width + 10;
            //    this.Controls.Add(control);

            //    control = new UCDashboardRound(repair_amount.ToString(), "SERVICE REPAIR", colors[rd.Next(0, 3)]) { CardBaseColor = ColorHelper.Danger };
            //    control.Width = control_width;
            //    control.Location = new Point(locationX, locationY);
            //    locationX += control_width + 10;
            //    this.Controls.Add(control);

            //    control = new UCDashboardRound(prepaid.ToString(), "PREPAID/DEPOSIT", colors[rd.Next(0, 3)]) { CardBaseColor = ColorHelper.Danger };
            //    control.Width = control_width;
            //    control.Location = new Point(locationX, locationY);
            //    locationX += control_width + 10;
            //    this.Controls.Add(control);

            //    control = new UCDashboardRound(giftcard_sale_cash.ToString(), "GIFTCARD CASH SALE", colors[rd.Next(0, 3)]) { CardBaseColor = ColorHelper.Danger };
            //    control.Width = control_width;
            //    control.Location = new Point(locationX, locationY);
            //    locationX += control_width + 10;
            //    this.Controls.Add(control);

            //    control = new UCDashboardRound(giftcard_sale_credit.ToString(), "GIFTCARD CREDIT SALE", colors[rd.Next(0, 3)]) { CardBaseColor = ColorHelper.Danger, SubtitleSize = 14 };
            //    control.Width = control_width;
            //    control.Location = new Point(locationX, locationY);
            //    locationX += control_width + 10;
            //    this.Controls.Add(control);

            //    //Reset row
            //    locationX = 10;
            //    locationY = locationY + control.Height + 10;

            //    control = new UCDashboardRound(giftcard_sale.ToString(), "GIFTCARD SALE", colors[rd.Next(0, 3)]) { CardBaseColor = ColorHelper.Question };
            //    control.Width = control_width;
            //    control.Location = new Point(locationX, locationY);
            //    locationX += control_width + 10;
            //    this.Controls.Add(control);

            //    control = new UCDashboardRound(giftcard.ToString(), "GIFTCARD REDEEM", colors[rd.Next(0, 3)]) { CardBaseColor = ColorHelper.Question };
            //    control.Width = control_width;
            //    control.Location = new Point(locationX, locationY);
            //    locationX += control_width + 10;
            //    this.Controls.Add(control);

            //    control = new UCDashboardRound(discount.ToString(), "DISCOUNT", colors[rd.Next(0, 3)]) { CardBaseColor = ColorHelper.Question };
            //    control.Width = control_width;
            //    control.Location = new Point(locationX, locationY);
            //    locationX += control_width + 10;
            //    this.Controls.Add(control);

            //    control = new UCDashboardRound(cash_discount.ToString(), "CASH DISCOUNT", colors[rd.Next(0, 3)]) { CardBaseColor = ColorHelper.Question };
            //    control.Width = control_width;
            //    control.Location = new Point(locationX, locationY);
            //    locationX += control_width + 10;
            //    this.Controls.Add(control);

            //    control = new UCDashboardRound(reward.ToString(), "REWARD", colors[rd.Next(0, 3)]) { CardBaseColor = ColorHelper.Question };
            //    control.Width = control_width;
            //    control.Location = new Point(locationX, locationY);
            //    locationX += control_width + 10;
            //    this.Controls.Add(control);

            //    //Reset row
            //    locationX = 10;
            //    locationY = locationY + control.Height + 10;

            //    control = new UCDashboardRound(counpon.ToString(), "COUPON/PROMOTION", colors[rd.Next(0, 3)]) { CardBaseColor = ColorHelper.Warning };
            //    control.Width = control_width;
            //    control.Location = new Point(locationX, locationY);
            //    locationX += control_width + 10;
            //    this.Controls.Add(control);

            //    control = new UCDashboardRound(credit_tip.ToString(), "CREDIT TIP", colors[rd.Next(0, 3)]) { CardBaseColor = ColorHelper.Warning };
            //    control.Width = control_width;
            //    control.Location = new Point(locationX, locationY);
            //    locationX += control_width + 10;
            //    this.Controls.Add(control);

            //    control = new UCDashboardRound(cash_tip.ToString(), "CASH TIP", colors[rd.Next(0, 3)]) { CardBaseColor = ColorHelper.Warning };
            //    control.Width = control_width;
            //    control.Location = new Point(locationX, locationY);
            //    locationX += control_width + 10;
            //    this.Controls.Add(control);

            //    control = new UCDashboardRound(surCharge.ToString(), "SUR CHARGE/DUAL PRICE", colors[rd.Next(0, 3)]) { CardBaseColor = ColorHelper.Warning, SubtitleSize = 14 };
            //    control.Width = control_width;
            //    control.Location = new Point(locationX, locationY);
            //    locationX += control_width + 10;
            //    this.Controls.Add(control);

            //    control = new UCDashboardRound((total_supply - total_supply_plus).ToString(), "SUPPLY DEDUCTION", colors[rd.Next(0, 3)]) { CardBaseColor = ColorHelper.Warning };
            //    control.Width = control_width;
            //    control.Location = new Point(locationX, locationY);
            //    locationX += control_width + 10;
            //    this.Controls.Add(control);

            //    //Reset row
            //    locationX = 10;
            //    locationY = locationY + control.Height + 10;

            //    control = new UCDashboardRound(total_supply_plus.ToString(), "NAILS TECH SUPPLY", "EA4335") { CardBaseColor = ColorHelper.Success };
            //    control.Width = control_width;
            //    control.Location = new Point(locationX, locationY);
            //    locationX += control_width + 10;
            //    this.Controls.Add(control);

            //    control = new UCDashboardRound(tax.ToString(), "TAX", colors[rd.Next(0, 3)]) { CardBaseColor = ColorHelper.Success };
            //    control.Width = control_width;
            //    control.Location = new Point(locationX, locationY);
            //    locationX += control_width + 10;
            //    this.Controls.Add(control);

            //    control = new UCDashboardRound(product_sale.ToString(), "PRODUCT SALE", colors[rd.Next(0, 3)]) { CardBaseColor = ColorHelper.Success };
            //    control.Width = control_width;
            //    control.Location = new Point(locationX, locationY);
            //    locationX += control_width + 10;
            //    this.Controls.Add(control);

            //    control = new UCDashboardRound(total_credit_transaction.ToString(), "CREDIT TRANSACTION", colors[rd.Next(0, 3)]) { CardBaseColor = ColorHelper.Success };
            //    control.Width = control_width;
            //    control.Location = new Point(locationX, locationY);
            //    locationX += control_width + 10;
            //    this.Controls.Add(control);

            //    control = new UCDashboardRound(adjust.ToString(), "ADJUST", "EA4335") { CardBaseColor = ColorHelper.Success };
            //    control.Width = control_width;
            //    control.Location = new Point(locationX, locationY);
            //    locationX += control_width + 10;
            //    this.Controls.Add(control);
            //}
        }

    }

}
