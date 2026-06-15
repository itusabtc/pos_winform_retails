#region LIBARY
using NailsChekin.Models;
using NailsChekin.Popup;
using NailsChekin.UserControl;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;
using com.clover.remotepay.sdk;
using CloverExamplePOS;
using com.clover.remotepay.transport;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NailsChekin.Models.ListModel;
using System.Threading.Tasks;
using NailsChekin.Models.Payments;
using WebSocketSharp;
using SocketIOClient;
using static NailsChekin.Models.Payments.P5LibUsbMode;
using NailsChekin.Models.Helper;
using NailsChekin.Models.Implements;
using NailsChekin.Models.Services;
using NailsChekin.MyControls;
using NailsChekin.Properties;
using DevExpress.XtraBars.Navigation;
using System.Text;
#endregion LIBARY

namespace NailsChekin
{
    public partial class FormMain : Form
    {
        #region Param

        private CloverManager _clover;
        public bool cloverStatus = false;
        private CloverPaymentProcessUI _processUI;

        public static string colorCodes = "Red__Blue";
        public string selected_ticket = "";

        string customer_selected = "0";
        string customer_name = "";

        public string selected_ticket_combine_id = "";
        public bool selected_ticket_combine = false;

        public bool selected_ticket_pending_payment = false;
        public string curent_order_local_payment_id = "";

        public bool waiting_clover_process = false;
        public bool clover_confirm_print = false;

        public double tax_percent = 0;
        public bool tax_include = true;
        public double tax_redeem = 0;
        public double discount_value = 0;
        public string discount_unit = "%";
        public double discount_redeem = 0;

        public string reward_balance = "0";
        public string credit_balance = "0";

        public double total_pending = 0;  //Thanh toan nhieu lan

        public SynchronizationContext uiThread;
       
        //private CartHelper _cart;

        public string CurrentPairingToken = ""; // If you store & load this value, you can skip pairing codes on the device most of the time.

        public string payment_result = "";
        private PictureBox picLoading;

        #endregion Param

        #region ON LOAD

        public FormMain()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();

            UIHelper.EnableDeepDoubleBuffer(this);
            typeof(Panel).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(panelBackground, true, null);

            this.BackColor = ColorHelper.DefaultBackgoundColor;
            AddLoading();

            //BAR CODE Tạo Timer để reset buffer nếu ngừng quét quá lâu
            timerBarcodeReset = new System.Windows.Forms.Timer();
            timerBarcodeReset.Interval = 200; // 200ms: nếu ngừng nhập quá 200ms thì coi như kết thúc quét
            timerBarcodeReset.Tick += TimerBarcodeReset_Tick;

            this.KeyPreview = true; // Quan trọng: form nhận được KeyPress dù đang focus control khác
            this.KeyPress += MainForm_KeyPress;
        }

        private async void Form3_Load(object sender, EventArgs e)
        {
            // Gán uiThread trước mọi nhánh — Clover/P5 callback cần nó kể cả khi vào nhánh login
            uiThread = SynchronizationContext.Current ?? new WindowsFormsSynchronizationContext();

            string store_id = NailsChekin.Models.Helper.ConfigLocalHelper.GetStoreConfig("store_id", "");
            string current_login = NailsChekin.Models.Helper.ConfigLocalHelper.GetStoreConfig("current_login", "");

            if (store_id.Trim().Length <= 0 || current_login.Equals("0"))
            {
                using (FormLogin frm = new FormLogin(this))
                    frm.ShowDialog(this);
            }
            else
            {
                double physical = MonitorSizeHelper.GetCurrentMonitorInches(this.Handle, ReportMode.Physical);
                double perceived = MonitorSizeHelper.GetCurrentMonitorInches(this.Handle, ReportMode.Perceived);
                bool isMini = MonitorSizeHelper.IsMiniScreen(this.Handle);

                LayoutHelper.SetScale(physical, perceived, isMini);

                picLoading.Visible = true;
                picLoading.BringToFront();
                Application.DoEvents(); // Vẽ ngay
                panelBackground.Visible = false; // Ẩn giao diện chính
                this.SuspendLayout();

                // Chạy xử lý nặng ở thread khác (tránh đơ UI) — block này không được đụng UI control
                await Task.Run(() =>
                {
                    this.InitConfig();
                    this.InitFormData(false);  // Adjust_Screen đã chuyển ra UI thread
                    PreloadBackgroundImages(); // Load ảnh đã cache
                });

                this.ResumeLayout();

                // Chạy trực tiếp trên UI thread (await đã về UI thread rồi)
                ApplyPanelTheme();
                this.AddHeaderTemplateBar();
                this.AddFooterTemplateBar();
                StartBackground_CreditDevice_Init_Task();

                // ✅ Hiện giao diện TRƯỚC khi adjust layout
                panelBackground.Visible = true;
                panelLayout_Right.Visible = true;
                panelLayout_Left.Visible = true;
                picLoading.Visible = false;
                this.Resize -= Parent_Resize;  // unhook trước khi dispose
                picLoading.Dispose();
                picLoading = null;

                // Adjust layout SAU khi form đã fully visible (UI thread, an toàn)
                this.Adjust_Screen();
            }
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            this.MainForm_Shown(sender, e);

            StartBackground_SocketIO_Task();

            // Auto check version mới (chỉ chạy 1 lần/phiên — nếu FormIntro đã check thì bỏ qua)
            UpdateHelper.AutoCheckOnStartup(this);
        }

        private async void FormMain_FormClosedAsync(object sender, FormClosedEventArgs e)
        {
            try { if (_socketHelper != null) await _socketHelper.StopAsync(); } catch { }
            _socketHelper?.Dispose();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Giải phóng Clover connector trước (không phụ thuộc printer):
            // CleanupClover unhook toàn bộ event tránh callback bắn vào form đã đóng
            try { CleanupClover(); } catch { }

            // Dừng và giải phóng timer barcode reset
            try { timerBarcodeReset?.Stop(); timerBarcodeReset?.Dispose(); } catch { }

            // Giải phóng CancellationTokenSource tab navigation
            try { _cts?.Cancel(); _cts?.Dispose(); _cts = null; } catch { }

            // Hủy watcher timeout T2 (Task.Delay 8h giữ CTS nếu không cancel)
            try { ClearT2PendingPayment(); _t2PaymentTimeoutCts?.Dispose(); _t2PaymentTimeoutCts = null; } catch { }

            // Đóng các popup non-modal còn mở
            try { pairingForm?.Dispose(); pairingForm = null; } catch { }
            try { frmCreditProcessing?.Dispose(); frmCreditProcessing = null; } catch { }

            // P5 LAN/PAIR: dừng reconnect loop + đóng socket (null trước để callback cũ không fire)
            try
            {
                var oldP5 = p5Lib;
                p5Lib = null;
                if (oldP5 != null) _ = oldP5.DisconnectAndDisposeAsync();
            }
            catch { }

            // P5 USB
            try { if (p5UsbLib != null) { P5LibUsbMode.CleanupStaticResources(); p5UsbLib = null; } } catch { }

            // Chạy purge cho máy in — trong mọi trường hợp đóng app
            // (UserClosing, WindowsShutDown, TaskManagerClosing, ApplicationExitCall)
            CleanupPrinterOnExit(e.CloseReason);
        }

        public void Adjust_Screen()
        {
            // Nếu đang ở background thread → marshal sang UI thread, rồi thoát
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(this.Adjust_Screen));
                return;
            }

            if (this.IsDisposed || !this.IsHandleCreated) return;

            try
            {
                // ---- Panel layout ----
                // Grow right panel FIRST (big screen) so left panel width is calculated correctly
                if (!LayoutHelper.mini_screen)
                {
                    panelLayout_Right.Width += 100;
                }

                // Calculate left panel width AFTER right panel is at its final width
                panelLayout_Left.Width = this.ClientSize.Width - panelLayout_Right.Width - 5;
                lbAddQuickItem.Left = panelLayout_Left.Width - lbAddQuickItem.Width - 20;
                lbCart_Search.Left = panelLayout_Left.Width - lbAddQuickItem.Width - lbCart_Search.Width - 20;
                svgCart_RemoveCustomer.Left = panelLayout_Left.Width - lbAddQuickItem.Width - lbCart_Search.Width - svgCart_RemoveCustomer.Width - 30;

                if (!LayoutHelper.mini_screen) //Bản 15.6 inch chuẩn, căn chỉnh cho bản 24 inch
                {
                    int size_height_scale = 16;  //6 button Total +120
                    int size_height_right_scale = 16; //6 button
                    int top = btnCart_Method_Cash.Top;
                    int control_height = btnCart_Method_Cash.Height + size_height_scale;
                    int control_right_height = btnCart_Discount.Height + size_height_right_scale;

                    // panelLayout_Right.Width already += 100 above (before left width calc)
                    panelCart_Control.Width += 100; panelCart_Control.Height += 120;

                    //LEFT
                    btnCart_Method_Cash.Width += 20; btnCart_Method_Cash.Height += size_height_scale; btnCart_Method_Cash.Top = top; top += control_height + 10;
                    btnCart_Method_Charge.Width += 20; btnCart_Method_Charge.Height += size_height_scale; btnCart_Method_Charge.Top = top; top += control_height + 10;
                    btnCart_Method_CashApp.Width += 20; btnCart_Method_CashApp.Height += size_height_scale; btnCart_Method_CashApp.Top = top; top += control_height + 10;
                    btnCart_Method_Member.Width += 20; btnCart_Method_Member.Height += size_height_scale; btnCart_Method_Member.Top = top; top += control_height + 10;
                    btnCart_Method_Prepaid.Width += 20; btnCart_Method_Prepaid.Height += size_height_scale; btnCart_Method_Prepaid.Top = top; top += control_height + 10;
                    btnCart_Cancel.Width += 20; btnCart_Cancel.Height += size_height_scale; btnCart_Cancel.Top = top; top += control_height + 10;

                    panelCart_Control_Keyboard.Width += 80; panelCart_Control_Keyboard.Left += 15; panelCart_Control_Keyboard.Height += 80;
                    btnCart_Save.Width = panelCart_Control_Keyboard.Width - 25; btnCart_Save.Left += 30; btnCart_Save.Top = panelCart_Control_Keyboard.Bottom + 5;

                    //RIGHT – thứ tự khớp Designer (Y: Tax=229, Discount=321, Reward=413, SalonCredit=506, Voucher=608, Print=700)
                    top = btnCart_Method_Cash.Top;
                    btnCart_Tax.Width        += 20; btnCart_Tax.Left        += 95; btnCart_Tax.Height        += size_height_right_scale; btnCart_Tax.Top        = top; top += control_right_height + 10;
                    btnCart_Discount.Width   += 20; btnCart_Discount.Left   += 95; btnCart_Discount.Height   += size_height_right_scale; btnCart_Discount.Top   = top; top += control_right_height + 10;
                    btnCart_Reward.Width     += 20; btnCart_Reward.Left     += 95; btnCart_Reward.Height     += size_height_right_scale; btnCart_Reward.Top     = top; top += control_right_height + 10;
                    btnCart_Method_SalonCredit.Width+= 20; btnCart_Method_SalonCredit.Left+= 95; btnCart_Method_SalonCredit.Height+= size_height_right_scale; btnCart_Method_SalonCredit.Top= top; top += control_right_height + 10;
                    btnCart_Voucher.Width    += 20; btnCart_Voucher.Left    += 95; btnCart_Voucher.Height    += size_height_right_scale; btnCart_Voucher.Top    = top; top += control_right_height + 10;
                    btnCart_Print.Width      += 20; btnCart_Print.Left      += 95; btnCart_Print.Height      += size_height_right_scale; btnCart_Print.Top      = top; top += control_right_height + 10;

                    lbCart_Total.Left += 25; LayoutHelper.SetLabelFontSize(lbCart_Total_Title, 26); LayoutHelper.SetLabelFontSize(lbCart_Total, 26);
                    lbCart_Paided_Title.Left += 45; lbCart_Paided.Left += 60; LayoutHelper.SetLabelFontSize(lbCart_Paided_Title, 26); LayoutHelper.SetLabelFontSize(lbCart_Paided, 26);
                    panelCartAntDue.Left += 45;
                    lbCart_Tender.Left += 10;
                }
                else
                {
                    panelCart_Control.Top = 10;
                }

                // ---- Scan panel ----
                panelStartScan.Left = (panelCartItemsTouch.Width - panelStartScan.Width) / 2;
                panelStartScan.Top = ((panelCartItemsTouch.Height - panelStartScan.Height) / 2);
                panelStartScan.BringToFront();
                panelStartScan.Visible = true;

                // ---- Tab layout ----
                this.BuildTabLayout();
            }
            catch (Exception ex)
            {
                // Log lỗi đầy đủ vào file để debug sau
                NailsChekin.Models.Helper.LogHelper.SaveLOG_Crash(ex.ToString(), "Adjust_Screen Error");
            }
        }

        private void BuildTabLayout()
        {
            //Load sẵn blank lần đầu
            tabDashboard.Controls.Add(new TabDashboard { Dock = DockStyle.Fill });    
            tabContent.SelectedPage = tabHome;
            tabContent.SelectedPageChanging += TabContent_SelectedPageChanging;

            tabContent.BackColor = ColorHelper.DefaultBackgoundColor;
            //foreach (NavigationPage p in tabContent.Pages)
            //{
            //    p.Appearance.BackColor = ColorHelper.DefaultBackgoundColor;
            //    p.Appearance.Options.UseBackColor = true;
            //}

            //tabContent.BackgroundImage = Properties.Resources.BG_Light; //không xái backgound được, chậm quá
            //tabHome.BackgroundImage = Properties.Resources.BG_Light; // ảnh đã resize sẵn
            //tabHome.BackgroundImageLayout = ImageLayout.Stretch; // hoặc None nếu đã đúng size

            tabContent.AllowTransitionAnimation = DevExpress.Utils.DefaultBoolean.False;  //xài có vẻ chậm hơn mấy giây
        }

        private bool _programmaticSwitch;
        private System.Threading.CancellationTokenSource _cts;
        private readonly HashSet<NavigationPage> _loaded = new HashSet<NavigationPage>(); // ghi nhớ trang đã load xong

        // helper: trang có thể hiển thị ngay, không cần overlay
        private bool IsInstantPage(NavigationPage p)
        {
            if (ReferenceEquals(p, tabHome) ) return true;                    // TabHome,tabPayment thiết kế sẵn
            //if (_loaded.Contains(p)) return true;                            // đã load lần trước
            //if (p.Controls.Count > 0 && !(p.Controls[0] is ILoadable)) return true; // có UI và không cần async
            return false;
        }

        private async void TabContent_SelectedPageChanging(object sender, DevExpress.XtraBars.Navigation.SelectedPageChangingEventArgs e)
        {
            // Nếu là lần chuyển do code set SelectedPage => cho qua
            if (_programmaticSwitch) return;

            var target = e.Page as DevExpress.XtraBars.Navigation.NavigationPage;
            if (target == null) return;

            // ⛳️ Trang nhanh (như TabHome) -> cho qua luôn, KHÔNG overlay
            if (IsInstantPage(target)) return;

            // Hủy lần trước (nếu user bấm liên tiếp) — dispose CTS cũ tránh leak
            try { _cts?.Cancel(); _cts?.Dispose(); } catch { }
            _cts = new System.Threading.CancellationTokenSource();
            var ct = _cts.Token;

            // Hoãn chuyển để nạp nội dung trước
            e.Cancel = true;

            var overlay = BusyOverlay.ShowOver(tabContent, Properties.Resources.ant_load, 200, 0);
            try
            {
                await System.Threading.Tasks.Task.Yield(); // cho overlay render ngay

                // tạo UC nếu trang chưa có (hoặc rely vào QueryControl)
                EnsurePageHasContent(target);

                // nếu UC hỗ trợ nạp async thì gọi ở đây
                var uc = (target.Controls.Count > 0) ? target.Controls[0] : null;
                if (uc is ILoadable loadable)
                    await loadable.EnsureLoadedAsync(ct);

                if (ct.IsCancellationRequested) return;

                // ✅ lần chuyển do code: bật cờ để không bị Cancel nữa
                _programmaticSwitch = true;
                tabContent.SelectedPage = target;   // event fire lại nhưng sẽ "return" ngay ở đầu
            }
            finally
            {
                _programmaticSwitch = false;
                overlay.CloseOverlay();
            }
        }

        private void EnsurePageHasContent(NavigationPage page)
        {
            if (page.Controls.Count > 0) return;
            var make = page.Tag as Func<Control>;
            var ctl = (make != null) ? make() : new Panel { Dock = DockStyle.Fill };
            page.Controls.Add(ctl);
        }

        KeyBoardTemplateBar kb;
        private bool _overlayInited;
        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (_overlayInited) return;
            _overlayInited = true;

            kb = new KeyBoardTemplateBar
            {
                Dock = DockStyle.Fill,
                AllowDecimal = true,
                MaxLength = 30,
                ButtonFontSize = 26f,
                ButtonCornerRadius = 12,
                TargetControl = lbCart_Tender
            };
            panelCart_Control_Keyboard.Controls.Add(kb);
        }

        public void InitConfig()
        {
            ConfigLocalHelper.MapConfigToLocalSystem(true);

            this.tax_percent = Core.TAX_PERCENT();
            this.tax_include = tax_percent > 0;

            // Hàm này được gọi từ background thread (Form3_Load) => phần UI phải marshal về UI thread
            void ApplyTaxUI()
            {
                if (tax_percent <= 0)
                {
                    btnCart_Tax.Enabled = false;
                    btnCart_Tax.Title = "TAX (0.00%)";
                }
                else
                {
                    btnCart_Tax.Title = "TAX (" + tax_percent + "%)";
                }
            }
            if (this.IsHandleCreated && this.InvokeRequired)
                this.BeginInvoke((Action)ApplyTaxUI);
            else
                ApplyTaxUI();

            this.ReloadSurchargeOrDualPriceOrCashDiscountSetting();
        }

        string surCharge_percent = "";
        string surCharge_debit_percent = "";
        string surCharge_unit = "";
        float dual_price_percent = 0;
        float cash_discount_percent = 0;
        float cash_discount_product_percent = 0;
        public void ReloadSurchargeOrDualPriceOrCashDiscountSetting()
        {
            //this.cash_discount_percent = Core.CASH_DISCOUNT_PERCENT();
            //this.cash_discount_product_percent = Core.CASH_DISCOUNT_PRODUCT_PERCENT();
            //this.dual_price_percent = Core.DUAL_PRICE_PERCENT();
            //if (this.dual_price_percent > 0)  //hệ thống sử dụng dual price
            //{
            //    return;
            //}

            bool chkSurChargeOn = ConfigLocalHelper.GetConfig("chkSurChargeOn", Constants.chkSurChargeOn);
            this.surCharge_percent = ConfigLocalHelper.GetConfig("surCharge_percent", Constants.surCharge_percent);
            this.surCharge_debit_percent = ConfigLocalHelper.GetConfig("surCharge_debit_percent", Constants.surCharge_debit_percent);
            this.surCharge_unit = ConfigLocalHelper.GetConfig("surCharge_unit", Constants.surCharge_unit);
        }

        public void InitFormData(bool reload_menu)
        {
            string current_login = NailsChekin.Models.Helper.ConfigLocalHelper.GetStoreConfig("current_login", "");
            if (current_login.Equals("0"))
            {
                return;
            }
        }

        private async void StartBackground_CreditDevice_Init_Task()
        {
            // Run the task on a background thread
            await Task.Run(() => InitializeCreditDeviceConnector_Async());
        }

        public async Task InitializeCreditDeviceConnector_Async(bool re_connect = false)
        {
            if (Core.GET_POS_ROLE() == POS_ROLE.SECONDARY) // Secondary không xử lý credit device
                return;

            if (CreditCardLib.GET_CREDIT_DEVICE() == CREDIT_DEVICE_TYPE.CLOVER && CreditCardLib.USING_SYSTEM_CREDIT())
            {
                if (re_connect && _clover != null)
                    _clover.Disconnect();

                this.InitClover();
                if (re_connect)
                    CartEnableControl();
            }
            else if (CreditCardLib.GET_CREDIT_DEVICE() == CREDIT_DEVICE_TYPE.CODE_PAY && CreditCardLib.USING_SYSTEM_CREDIT())
            {
                var connType = P5Lib.Get_P5_ConecttionType_Setting();

                if (connType == P5_CONNECTTION_TYPE.USB)
                {
                    p5UsbLib = new P5LibUsbMode(this);
                    p5UsbLib.InitUSBConnect();
                }
                else if (connType == P5_CONNECTTION_TYPE.WLAN_LAN || connType == P5_CONNECTTION_TYPE.PAIR_MODE)
                {
                    // Dispose old instance trước: dừng reconnect loop + đóng socket
                    // Tránh old CodePayHelper callbacks còn fire vào frmMain sau khi new instance đã connect
                    if (p5Lib != null)
                    {
                        var oldLib = p5Lib;
                        p5Lib = null; // null ngay để callback cũ không cập nhật state mới
                        try { await oldLib.DisconnectAndDisposeAsync().ConfigureAwait(false); } catch { }
                    }

                    p5Lib = new P5Lib(this);

                    if (connType == P5_CONNECTTION_TYPE.PAIR_MODE)
                    {
                        _pairedData = p5Lib.GetPairedData();
                        p5Lib.registerMdnsListener();
                    }

                    await ConnectP5Device();
                }
            }
        }

        private async void StartBackground_SocketIO_Task()
        {
            await Task.Run(() => ConnectToSocketIOServerAsync());
        }

        private SocketIoClientHelper _socketHelper;
        private async Task ConnectToSocketIOServerAsync()
        {
            // ===== Helpers cục bộ: set property bằng reflection để không lỗi khác version =====
            void TrySetProp(object obj, string name, object value)
            {
                try
                {
                    var p = obj.GetType().GetProperty(name);
                    if (p == null || !p.CanWrite) return;

                    var pt = p.PropertyType;

                    if (value != null && !pt.IsAssignableFrom(value.GetType()))
                    {
                        // TimeSpan <-> int(ms)
                        if (pt == typeof(TimeSpan) && value is TimeSpan ts) { p.SetValue(obj, ts, null); return; }
                        if (pt == typeof(int) && value is TimeSpan ts2) { p.SetValue(obj, (int)ts2.TotalMilliseconds, null); return; }

                        // Enum từ chuỗi
                        if (pt.IsEnum && value is string s)
                        {
                            var ev = Enum.Parse(pt, s, true);
                            p.SetValue(obj, ev, null);
                            return;
                        }

                        var converted = Convert.ChangeType(value, pt);
                        p.SetValue(obj, converted, null);
                        return;
                    }

                    p.SetValue(obj, value, null);
                }
                catch { /* ignore nếu property không có ở bản lib hiện tại */ }
            }

            void TrySetTransport(object _options, string transportName /* "WebSocket" hoặc "Polling" */)
            {
                try
                {
                    var p = _options.GetType().GetProperty("Transport");
                    if (p == null || !p.CanWrite) return;
                    var enumType = p.PropertyType;
                    if (!enumType.IsEnum) return;
                    var ev = Enum.Parse(enumType, transportName, true);
                    p.SetValue(_options, ev, null);
                }
                catch { }
            }
            // ===============================================================================

            // 1) Khởi tạo options (chỉ set các field phổ biến; bản nào không có sẽ được bỏ qua)
            var options = new SocketIOOptions();

            // Reconnect của thư viện (để Helper của bạn không phải tự viết lại)
            TrySetProp(options, "Reconnection", true);
            TrySetProp(options, "ReconnectionAttempts", 0);          // 0 = vô hạn (nếu lib hỗ trợ)
            TrySetProp(options, "ReconnectionDelay", 2000);          // ms
            TrySetProp(options, "RandomizationFactor", 0.25);        // nếu bản lib có

            // Thiết lập kết nối/transport (an toàn nhiều version)
            TrySetProp(options, "Path", "/socket.io/");
            TrySetProp(options, "EIO", 4);                           // Engine.IO v4 (nếu có)
            TrySetTransport(options, "WebSocket");                   // ưu tiên WebSocket
            TrySetProp(options, "ConnectionTimeout", TimeSpan.FromSeconds(6));

            // Header tuỳ chọn (nếu lib expose)
            TrySetProp(options, "ExtraHeaders", new System.Collections.Generic.Dictionary<string, string>
            {
                { "User-Agent", "AntPayPOS/1.0" }
            });

            // 2) Tạo helper với options trên
            _socketHelper = new SocketIoClientHelper(
                Constants.socketIOUrl,
                options,
                invoker: this // marshal về UI thread
            );

            // 3) Đăng ký sự kiện
            _socketHelper.Connected += () =>
            {
                navSocketStatus.Caption = "Socket: Connected!";
                Console.WriteLine("SocketIO: Connected");
            };

            _socketHelper.Disconnected += () =>
            {
                navSocketStatus.Caption = "Socket: Disconnected";
                Console.WriteLine("SocketIO: Disconnected");
            };

            _socketHelper.ReconnectAttempt += attempt =>
            {
                navSocketStatus.Caption = $"Socket: Reconnecting (try {attempt})…";
                Console.WriteLine("SocketIO: Reconnect attempt " + attempt);
            };

            _socketHelper.MessageReceived += msg =>
            {
                Console.WriteLine("SocketIO: " + msg);
                ProcessSocketIO_Message(msg);   // an toàn vì đã ở UI thread
            };

            _socketHelper.Error += ex =>
            {
                var s = ex?.Message ?? ex.ToString();
                if (s.Length > 200) s = s.Substring(0, 200);
                navSocketStatus.Caption = "Socket error: " + s;
                Console.WriteLine("SocketIO Error: " + ex);
            };

            // 4) Kết nối (bọc try/catch để UI không crash)
            try
            {
                await _socketHelper.StartAsync();
            }
            catch (Exception ex)
            {
                navSocketStatus.Caption = "Socket connect failed";
                Console.WriteLine("SocketIO Connect failed: " + ex);
            }
        }

        #endregion ONLOAD

        #region Extend Function

        public void CheckCardCorrect()
        {
            
        }

        public void UpdatePaymentCartAmount(bool check_promotion = true, bool update_cart = true, bool main_form_onload = false)
        {
            if (main_form_onload)
                return; // not call first from open

            try
            {
                double subTotal = 0;
                double totalDiscount = 0;
                double totalCoupon = 0;
                double subTotalIncludeTax = 0;
                double totalRedeem = 0;
                double totalRedeemVoucher = 0;
                double total = 0;
                double itemSold = 0;

                foreach (UCCartItem control in panelCartItemsTouch.Content.Controls.OfType<UCCartItem>())
                {
                    double quantity = double.Parse(control.quantity.Length <= 0 ? "0" : control.quantity);
                    double price = double.Parse(control.price.Length <= 0 ? "0" : control.price);
                    double discount = double.Parse(control.discount.Length <= 0 ? "0" : control.discount);
                    
                    subTotal += (quantity * price);
                    itemSold += quantity;
                    totalDiscount += discount;
                }
                subTotal = Math.Round(subTotal, 2);
                totalDiscount = Math.Round(totalDiscount, 2);

                if (totalDiscount <= 0)
                {
                    if (discount_value > 0)
                    {
                        if (discount_unit.Equals("$"))
                            totalDiscount = discount_value;
                        else
                            totalDiscount = Math.Round( ( subTotal * discount_value / 100.0), 2);
                    }
                }
                this.discount_redeem = totalDiscount;
                if (this.discount_redeem > 0)
                    btnCart_Discount.Title = "DISCOUNT" + Environment.NewLine + "-$" + this.discount_redeem;
                else
                    btnCart_Discount.Title = "DISCOUNT";

                // Round 2 số lẻ (trước đây Math.Round không tham số làm tròn về số nguyên — sai tiền)
                subTotal = Math.Round(subTotal - totalDiscount, 2);
                // subTotal đã trừ discount ở trên — không trừ thêm lần nữa khi tính tax
                this.tax_redeem = tax_include ? Math.Round(subTotal * (tax_percent / 100.0), 2) : 0;
                if (this.tax_redeem > 0)
                    btnCart_Tax.Title = "TAX (" + tax_percent + "%)" + Environment.NewLine + "$" + this.tax_redeem;
                else
                    btnCart_Tax.Title = "TAX (" + tax_percent + "%)";
                
                subTotalIncludeTax = Math.Round(subTotal + this.tax_redeem, 2);
                total = subTotalIncludeTax - totalCoupon - totalRedeemVoucher - this.salon_credit_redeem; 
                if (total < 0)
                    total = 0;

                lbCart_SubTotal.Text = "$" + subTotal;  lbCart_ItemSold.Text = itemSold.ToString();
                lbCart_Total.Text = "$" + subTotalIncludeTax;

                double total_pay_amount = CartHelper.GetPaymentTotal(this.paymentList);
                total = Math.Round((total - total_pay_amount), 2);
                
                if (total > 0)
                {
                    lbCart_Total.Text = "$" + total;
                    lbCart_Paided.Text = "$" + total_pay_amount;
                    lbCart_AmtDue.Text = "$" + total;  // total đã trừ total_pay_amount ở trên — không trừ lần 2
                    this.CartEnableControl();
                }
                else
                {
                    lbCart_AmtDue.Text = "$0.00";
                    this.CartDisableControl();
                }

                //if (Constants.check_promotion)  //Hạn chế xử lý
                //{
                //    if (check_promotion)
                //    {
                //        //Load Promotion backgound
                //        Task.Run(() =>
                //        {
                //            this.CheckPromotion();
                //        });
                //    }
                //}

                //if (Constants.send_payment_cart)  //Hạn chế socket, chỉ bật cho các tiệm xài
                //{
                //    if (update_cart)  //&& !check_promotion
                //    {
                //        Task.Run(() =>
                //        {
                //            Cart myCart = POS_GetCart();
                //            MainCart.SendCart(myCart);
                //        });
                //    }
                //}

                CartHelper.UpdateCartInfoSignalR(this.Get_JCart_Current());
            }
            catch { }
        }

        public string Get_JCart_Current()
        {
            string items = "";
            foreach (UCCartItem control in panelCartItemsTouch.Content.Controls.OfType<UCCartItem>())
            {
                items += @"{
                                'orderId': 0,
                                'itemId': " + control.item_id + @",
                                'itemName': '" + control.item_name + @"',
                                'qty': " + control.quantity + @",
                                'price': " + Utilitys.getTotalAmount(control.price) + @",
                                'priceDiscount': 0,
                                'discount': 0,
                                'subTotal': " + control.subTotal() + @",
                                'isPromotion':" + (string.IsNullOrEmpty(control.isPromotion) ? "0" : control.isPromotion) + @",
                                'scheme':'" + control.scheme + @"'
                            },";
            }

            if (items.Trim().Length > 0)
                items = items.Substring(0, items.Length - 1);

            string DATA = @"{
                                'id': 0,
                                'orderDate': '2026-06-10T23:08:43.154Z',
                                'comment': '',
                                'customerId': " + (string.IsNullOrEmpty(this.customer_selected) ? "0" : this.customer_selected) + @",
                                'customerName': '" + (string.IsNullOrEmpty(this.customer_name) ? "" : Regex.Replace(this.customer_name, "'", "")) + @"',
                                'customerPhone': '',
                                'orderStatus': 0,
                                'subtotal': " + Utilitys.getTotalAmount(lbCart_SubTotal.Text) + @",
                                'reward': 0, 
                                'totalReward':" + this.reward_balance + @",  
                                'discount': " + this.discount_redeem + @", 
                                'giftcardNumber': '',
                                'giftcardAmount': 0,
                                'tax': " + this.tax_redeem + @",
                                'taxPercent': '" + Constants.tax_percent + @"',
                                'tip': 0,
                                'orderTotal': " + Utilitys.getTotalAmount(lbCart_Total.Text) + @",
                                'methodOfPayment': 0,
                                'cash': 0,
                                'charge': 0,
                                'items': [" + items + @"],
                                'paringCode': '" + Constants.pairing_code + @"' 
                            }";
            return DATA;
        }

        double reward_percent_discount = 0;
        double reward_percent_owner = 100; //mac dinh chu chiu
        public void UpdateRedeemAmount(double redeem_amount, double reward_percent_discount, string reward_percent_owner)
        {
            //this.reward_percent_discount = reward_percent_discount;
            //this.reward_percent_owner = string.IsNullOrEmpty(reward_percent_owner) ? 100 : double.Parse(reward_percent_owner);
            //txtTotalReedem.Text = "$" + redeem_amount;

            //if (redeem_amount > 0) {
            //    btnCart_Reward.Title = "REWARD" + Environment.NewLine + ("-$" + redeem_amount);
            //}
            //else {
            //    //if (!string.IsNullOrEmpty(this.reward_balance) && double.Parse(this.reward_balance) > 0)
            //    //{
            //    //    if (Core.USING_REWARD_PERCENT())
            //    //        btnCart_Reward.Title = "REWARD" + Environment.NewLine + ("BAL: " + this.reward_balance + "%");
            //    //    else
            //    //        btnCart_Reward.Title = "REWARD" + Environment.NewLine + ("BAL: $" + this.reward_balance);
            //    //}
            //    //else
            //    //{
            //    //    btnCart_Reward.Title = "REWARD";
            //    //}
            //}

            //this.UpdatePaymentCartAmount();

            ////Update Amount
            //double total_amount = Utilitys.getTotalAmount(lbCart_AmtDue.Text);
            //double total_pay_amount = CartHelper.GetPaymentTotal(this.paymentList);
            //UIHelper.SafeUI(lbCart_Tender, () => lbCart_Tender.Text = Math.Round(total_amount - total_pay_amount, 2).ToString());

            ////Neu Tender <= 0 => check-out luon giong nhu bam cac nut payment
            //if (Math.Round(total_amount - total_pay_amount, 2) <= 0)
            //{
            //    Payment_New_Process("REWARD", "");
            //}

        }

        double salon_credit_redeem = 0;
        public void UpdateSalonCreditRedeemAmount(double redeem_amount)
        {
            this.salon_credit_redeem = redeem_amount;
            this.UpdatePaymentCartAmount();

            ////Update Amount
            //double total_amount = Utilitys.getTotalAmount(lbCart_AmtDue.Text);
            //double total_pay_amount = CartHelper.GetPaymentTotal(this.paymentList);
            //UIHelper.SafeUI(lbCart_Tender, () => lbCart_Tender.Text = Math.Round(total_amount - total_pay_amount, 2).ToString());

            ////Neu Tender <= 0 => check-out luon giong nhu bam cac nut payment
            //if (Math.Round(total_amount - total_pay_amount, 2) <= 0)
            //{
            //    Payment_New_Process("SALON CREDIT", "");
            //}

        }

        private double GetTotalAmount()
        {
            //Get Sub Total
            double subTotal = 0;
            double totalTip = 0;
            double totalDiscount = 0;

            //foreach (UCCartItem control in myCartTouchScrollPanel.Content.Controls.OfType<UCCartItem>())
            //{
            //    double quantity = double.Parse(control.quantity.Length <= 0 ? "0" : control.quantity);
            //    double price = double.Parse(control.price.Length <= 0 ? "0" : control.price);
            //    double tip = double.Parse(control.tip.Length <= 0 ? "0" : control.tip);

            //    subTotal += (quantity * price);
            //    totalTip += tip;
            //}

            subTotal = Math.Round(subTotal * 100) / 100;
            subTotal += totalTip;

            //if (txtDiscountPercent.Text.Trim().Length > 0 && double.Parse(txtDiscountPercent.Text.Trim()) < 100) //Percent
            //{
            //    double percent = double.Parse(txtDiscountPercent.Text.Trim());
            //    totalDiscount = percent * (subTotal) / 100.0;
            //    totalDiscount = Math.Round(totalDiscount * 100) / 100;
            //}
            //else if (txtDiscountFixAmount.Text.Trim().Length > 0) //FIX
            //{
            //    totalDiscount = double.Parse(txtDiscountFixAmount.Text.Trim());
            //}

            subTotal -= totalDiscount;
            return subTotal;
        }

        private double GetTotalAmountPayment()
        {
            double subTotal = 0;

            //subTotal = string.IsNullOrEmpty(txtSubTotal_TAX.Text) ? this.GetTotalAmount() : Utilitys.getTotalAmount(txtSubTotal_TAX.Text);

            //subTotal += string.IsNullOrEmpty(txtTotalTip.Text) ? 0 : 0;
            //subTotal += string.IsNullOrEmpty(txtTotalReedem.Text) ? 0 : Utilitys.getTotalAmount(txtTotalReedem.Text);

            return subTotal;
        }

        public double GetSubTotal()
        {
            //Get Sub Total
            double subTotal = 0;

            //foreach (UCCartItem control in myCartTouchScrollPanel.Content.Controls.OfType<UCCartItem>())
            //{
            //    double quantity = double.Parse(control.quantity.Length <= 0 ? "0" : control.quantity);
            //    double price = double.Parse(control.price.Length <= 0 ? "0" : control.price);
            //    double tip = double.Parse(control.tip.Length <= 0 ? "0" : control.tip);

            //    subTotal += (quantity * price);
            //}

            subTotal = Math.Round(subTotal * 100) / 100;
            return subTotal;
        }

        #endregion

        #region Call Function From Childrend Usercontrol
       
        /// <summary>
        /// Chạy action trên UI thread an toàn: bỏ qua nếu form đã dispose / chưa có handle
        /// (tránh InvalidOperationException khi callback từ background bắn về lúc form đang đóng).
        /// </summary>
        private void RunOnUi(Action action)
        {
            try
            {
                if (this.IsDisposed || !this.IsHandleCreated) return;
                if (this.InvokeRequired)
                    this.BeginInvoke(action);
                else
                    action();
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }

        public void EnableDisableControl(bool enable)
        {
            try
            {
                if (!enable)
                {
                    RunOnUi(() => this.CartDisableControl());
                }
                else
                {
                    RunOnUi(() =>
                    {
                        waiting_clover_process = false;
                        if (CreditCardLib.GET_CREDIT_DEVICE() == CREDIT_DEVICE_TYPE.CLOVER && _processUI != null)
                        {
                            _clover?.Process?.StopPayment(); // đóng popup
                        }

                        if (CreditCardLib.GET_CREDIT_DEVICE() == CREDIT_DEVICE_TYPE.CODE_PAY)
                        {
                            CodePay_ShowHide_Processing(false);
                        }

                        CartEnableControl();
                        this.HIDE_PAYMENT_NOW();
                    });
                }
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_Crash("Message: " + ex.Message + "\nStackTrace:\n" + ex.StackTrace, "App Error Formmain EnableDisableControl");
            }
        }

        public void setTileNavPane1_Default()
        {
            //navHeader.SelectedElement = tileNavHome;
        }

        #endregion

        #region Web Socket

        private void ProcessSocketIO_Message(string data)
        {
            if (!Utilitys.IsValidJson(data))
                return;

            string storeId = JObject.Parse(data)["storeId"].ToString();
            string action = JObject.Parse(data)["type"].ToString();
            string meta_data = JObject.Parse(data)["data"] == null ? "" : JObject.Parse(data)["data"].ToString();

            //Check Control Type: Customer / Staff
            if (storeId.Equals(Constants.pos_store_id))
            {
                if (action.Equals("CHECKIN") && data != null && !data.Equals("null"))
                {
                    if (JObject.Parse(meta_data)["paringCode"] != null)
                    {
                        if (JObject.Parse(meta_data)["paringCode"].ToString().Equals(Constants.pairing_code))
                        {
                            this.BeginInvoke(new Action(() => this.UpdateCustomers(meta_data)));
                        }
                    }
                }
                else if (action.Equals("CHECKOUT"))
                {
                    if (JObject.Parse(meta_data)["paringCode"] != null)
                    {
                        if (JObject.Parse(meta_data)["paringCode"].ToString().Equals(Constants.pairing_code))
                        {
                            this.BeginInvoke(new Action(() => this.ResetCustomers()));
                        }
                    }
                }
            }
        }

        public void CloseConfirm_Credit_Bill_IfOpen()
        {
            //if (this.IsDisposed) return;

            //this.BeginInvoke(new Action(() =>
            //{
            //    var opened = Application.OpenForms.OfType<FormConfirmT2PrintBill>().FirstOrDefault();
            //    if (opened != null && !opened.IsDisposed)
            //        opened.Close();
            //}));
        }

        #endregion

        #region Payment Function
        private string pincode_payment = "";

        private void SHOW_PAYMENT_PREPAID()
        {
            this.UpdatePaymentCartAmount();

            //double total_prepaid = Utilitys.getTotalAmount(txtTotalDeposit.Text);
            //double total_checkout = Utilitys.getTotalAmount(lbCart_AmtDue.Text);
            //if (Math.Round(total_checkout) <= 0)
            //{
            //    //Validate data trước khi thực hiện payment
            //    string use_system_credit_setting = "0";
            //    string error = this.POS_Payment_GetError("0", "0", "0", ref use_system_credit_setting);
            //    if (!string.IsNullOrEmpty(error))
            //    {
            //        CustomMessageBox.Show(error);
            //        return;
            //    }

            //    this.POS_CHECKOUT("0", this.paymentList, total_prepaid);
            //}
        }

        //NEW CLOVER PAYMENT LOGIC !!!
        public List<PaymentModel> paymentList = null;
        public double surcharge_amount = 0;
        public double surcharge_debit_amount = 0;
        public double dual_price_amount = 0;

        private bool _creditChargeInProgress = false;

        public void SHOW_PAYMENT_CREDIT(string pincode, double total_charge, double total_pos_tip)
        {
            // Chặn bấm Charge nhiều lần: nếu đang xử lý 1 phiên credit thì bỏ qua click mới.
            if (_creditChargeInProgress)
                return;
            _creditChargeInProgress = true;

            // Disable NGAY các nút thanh toán để không bấm lại trong lúc đang kết nối/charge (kể cả lúc reconnect Clover ~8s).
            CartDisableControl();

            ResetHandledPayments();

            // fire-and-forget, không block các hàm cha đã gọi SHOW_PAYMENT_CREDIT
            _ = SHOW_PAYMENT_CREDIT_InternalAsync(pincode, total_charge, total_pos_tip);

        }

        private async Task SHOW_PAYMENT_CREDIT_InternalAsync(string pincode, double total_charge, double total_pos_tip)
        {
            bool started = false; // true nếu đã thực sự gửi payment đi -> giữ disable; lỗi/return sớm -> bật lại control
            try
            {
                if (this.paymentList == null)  //ResetAllData / POS_CHECKOUT có thể đã set null
                    this.paymentList = new List<PaymentModel>();

                this.pincode_payment = pincode;
                this.current_clover_token = "";
                this.surcharge_amount = 0;
                this.surcharge_debit_amount = 0;
                this.dual_price_amount = 0;

                // Validate data trước khi payment
                string use_system_credit_setting = "1";
                string error = this.POS_Payment_GetError("1", "", "0", ref use_system_credit_setting);
                if (!string.IsNullOrEmpty(error))
                {
                    if (!error.Equals("Waiting Clover Connect"))
                        CustomMessageBox.Show(error);
                    return;
                }

                if (use_system_credit_setting.Equals("1")) // dùng máy cà thẻ
                {
                    if (total_charge > 0)
                    {
                        //if (Core.USING_DUAL_PRICE())
                        //{
                        //    this.dual_price_amount = Utilitys.getDualPrice((total_charge + total_pos_tip), total_pos_tip);
                        //    total_charge += this.dual_price_amount;
                        //}
                        //else
                        //{
                        //    this.surcharge_amount = Utilitys.getSurcharge((total_charge + total_pos_tip), total_pos_tip);
                        //    total_charge += this.surcharge_amount;
                        //}
                    }

                    if (CreditCardLib.GET_CREDIT_DEVICE() == CREDIT_DEVICE_TYPE.CODE_PAY)
                    {
                        if (total_charge <= 0) //Codepay chua xu ly chi thanh toan tip cho tu pos
                        {
                            CustomMessageBox.Show("Please check Amount Charge.");
                            return;
                        }

                        if (!CodePay_CheckConnect())
                        {
                            CustomMessageBox.Show("Can't connect to Code Pay device, please contact admin !");
                            return;
                        }

                        CartDisableControl(); // Validate xong moi Enable/Disable Cart Button
                        string payResult = await CodePay_Payment_SimpleAsync(total_charge, total_charge);
                        if (!string.IsNullOrEmpty(payResult) && !payResult.Equals("OK"))
                        {
                            CustomMessageBox.Show(payResult);
                            return;   // started=false -> finally sẽ bật lại control
                        }
                        started = true; // CodePay đã gửi đi -> giữ disable
                    }
                    else
                    {
                        if (total_charge <= 0 && total_pos_tip <= 0)
                        {
                            CustomMessageBox.Show("Please check Amount Charge.");
                            return;
                        }

                        // NHÁNH CLOVER:
                        // Cờ cloverStatus / _deviceReady có thể bị kẹt false (lỗi transient) hoặc kết nối đã rớt thật.
                        // EnsureCloverReadyAsync sẽ: ping nhẹ -> nếu chưa được thì reconnect đầy đủ rồi chờ DeviceReady
                        // (tương đương FormCloverReConnect bên NailsSolutionsPOS) trước khi báo lỗi.
                        if (!await EnsureCloverReadyAsync(8000))
                        {
                            CustomMessageBox.Show("Can't connect to clover device, please contact admin !");
                            return;
                        }

                        CartDisableControl(); // Validate xong moi Enable/Disable Cart Button
                        this.Clover_Payment_Simple(total_charge, total_charge, total_pos_tip);
                        started = true; // Clover đã gửi đi -> giữ disable
                    }
                }
                else // Add to payment CC => không dùng máy cà thẻ
                {
                    this.surcharge_amount = 0;
                    this.surcharge_debit_amount = 0;

                    PaymentModel model = new PaymentModel("CC", total_charge);
                    var responce = new CloverResponce();
                    responce.clover_amount = (total_charge * 100).ToString();
                    responce.clover_tip = (total_pos_tip * 100).ToString();
                    responce.clover_status = "Success";
                    model.responce.Add(responce);
                    this.paymentList.Add(model);

                    CartDisableControl(); // Validate xong moi Enable/Disable Cart Button
                    this.CHECK_PAYMENT_CORRECT();
                    started = true; // đã đẩy vào payment list / checkout -> giữ disable
                }
            }
            catch (Exception ex)
            {
                // tránh exception “rơi tự do” vì đây là fire-and-forget
                LogHelper.SaveLOG_CodePay(ex.Message, "SHOW_PAYMENT_CREDIT_InternalAsync Exception");
                CustomMessageBox.Show("Unexpected error during credit payment.\n" + ex.Message);
            }
            finally
            {
                // Cho phép bấm Charge lại cho lần sau.
                _creditChargeInProgress = false;

                // Nếu KHÔNG thực sự gửi payment (lỗi/return sớm) -> bật lại control để user thử lại.
                // Nếu đã gửi (started) -> giữ disable, các handler completion/fail sẽ tự bật lại.
                if (!started)
                    CartEnableControl();
            }
        }

        #region New Clover Process !!!!
        public void CHECK_PAYMENT_CORRECT(bool check_partical = false)
        {
            if (CreditCardLib.GET_CREDIT_DEVICE() == CREDIT_DEVICE_TYPE.CODE_PAY)
            {
                this.ConfirmPaymentCodePayNow(check_partical);
            }
            else
            {
                this.ConfirmPaymentCloverNow(check_partical);
            }
        }

        private void ConfirmPaymentCloverNow(bool check_partical = false)
        {
            //Check AMOUNT PAYMENT >= AMOUNT TOTAL
            double total_payment = CartHelper.GetPaymentTotal(this.paymentList);
            double total_tip_pos = 0;  //Máy credit thu tiền TIP cho từ POS nên phải trừ lại surcharge phần này
            //if (total_tip_pos > 0)
            //{
            //    if (Core.USING_DUAL_PRICE())  //DUAL PRICE không tính FEE Tip POS
            //    {
            //        //double surcharge_for_tip_paid = Utilitys.getDualPrice_From_Paided(total_tip_pos * 100.0);
            //        //total_payment += surcharge_for_tip_paid;
            //    }
            //    else
            //    {
            //        double surcharge_for_tip_paid = Utilitys.getSurcharge_From_Paided(total_tip_pos * 100.0);
            //        total_payment += surcharge_for_tip_paid;
            //    }
            //}
            double total_amount_request = Utilitys.getTotalAmount(lbCart_AmtDue.Text);

            bool repair_mode = false;
            double repair_amount = 0;
            total_payment += repair_amount;

            if (total_tip_pos > 0)
                total_amount_request -= total_tip_pos;

            LogHelper.SaveLOG_Payment("--4. total_payment: " + total_payment + " -- total_amount_request: " + total_amount_request, "CHECK_PAYMENT_CORRECT");
            if (Math.Round(total_payment, 2) < Math.Round(total_amount_request, 2))  //Chênh lệch 1, 2 đồng do làm tròn thì coi như complete
            {
                SaveCartPayment();
                if (_processUI != null)
                    _clover.Process.StopPayment(); // đóng popup
                return;
            }

            LogHelper.SaveLOG_Payment("CALL POS_CHECKOUT - check_partical: " + check_partical.ToString(), "CHECK_PAYMENT_CORRECT");
            this.POS_CHECKOUT("1", this.paymentList, total_payment);
            LogHelper.SaveLOG_Payment(this.payment_result, "Process Ticket Result");
        }

        /// <summary>
        /// Đảm bảo máy Clover sẵn sàng nhận lệnh trước khi charge.
        /// B1: nếu đang ready -> ok ngay.
        /// B2: ping nhẹ (cờ có thể stale nhưng kết nối còn sống) rồi chờ ngắn.
        /// B3: kết nối có thể đã rớt thật -> reconnect đầy đủ (giống FormCloverReConnect bên NailsSolutionsPOS)
        ///     rồi chờ DeviceReady tới timeoutMs.
        /// </summary>
        private async Task<bool> EnsureCloverReadyAsync(int timeoutMs)
        {
            if (_clover == null) return false;
            if (_clover.IsReady) return true;

            // Nudge: ping trạng thái máy.
            //  - Nếu máy còn sống mà cờ bị stale -> OnRetrieveDeviceStatusResponse -> DeviceAlive -> phục hồi cờ.
            //  - Nếu đang rớt -> lỗi này sẽ kích manager tự ScheduleReconnect.
            // KHÔNG tự InitClover/Disconnect ở đây: teardown manager song song với vòng reconnect đang chạy
            // sẽ thao tác trên object đã dispose -> NullReference COMMUNICATION_ERROR lặp vô hạn (đã thấy trong log).
            try { _clover.PingDeviceStatus(); } catch { }

            // Chờ manager tự kết nối lại (ScheduleReconnect/keepalive) tới khi máy thật sự ready.
            return await WaitCloverReadyAsync(timeoutMs);
        }

        private async Task<bool> WaitCloverReadyAsync(int timeoutMs)
        {
            int waited = 0;
            const int step = 150;
            while (waited < timeoutMs)
            {
                if (_clover != null && _clover.IsReady) return true;
                await Task.Delay(step);
                waited += step;
            }
            return _clover != null && _clover.IsReady;
        }

        private void ConfirmPaymentCodePayNow(bool check_partical = false)
        {
            //Check AMOUNT PAYMENT >= AMOUNT TOTAL
            double total_payment = CartHelper.GetPaymentTotal(this.paymentList);
            double total_tip_pos = 0;  //Máy credit thu tiền TIP cho từ POS nên phải trừ lại surcharge phần này
            //if (total_tip_pos > 0)
            //{
            //    if (Core.USING_DUAL_PRICE())  //DUAL PRICE không tính FEE Tip POS
            //    {
            //        //double surcharge_for_tip_paid = Utilitys.getDualPrice_From_Paided(total_tip_pos * 100.0);
            //        //total_payment += surcharge_for_tip_paid;
            //    }
            //    else
            //    {
            //        double surcharge_for_tip_paid = Utilitys.getSurcharge_From_Paided(total_tip_pos * 100.0);
            //        total_payment += surcharge_for_tip_paid;
            //    }
            //}

            double total_amount_request = Utilitys.getTotalAmount(lbCart_AmtDue.Text);  //Đã bao gồm TIP cho bên POS nếu có

            bool repair_mode = false;
            double repair_amount = 0;
            total_payment += repair_amount;
            if (total_tip_pos > 0)
                total_amount_request -= total_tip_pos;

            LogHelper.SaveLOG_Payment("--4.1 total_payment: " + total_payment + " -- total_amount_request: " + total_amount_request, "CHECK_PAYMENT_CORRECT");
            if (Math.Round(total_payment, 2) < Math.Round(total_amount_request, 2))  //Chênh lệch 1, 2 đồng do làm tròn thì coi như complete
            {
                //2025-08-16: thu tiền nhiều lần
                SaveCartPayment();
                CodePay_ShowHide_Processing(false);
                return;
            }

            LogHelper.SaveLOG_Payment("CALL CODEPAY POS_CHECKOUT - check_partical: " + check_partical.ToString(), "CHECK_PAYMENT_CORRECT");
            this.POS_CHECKOUT("1", this.paymentList, total_payment);
            LogHelper.SaveLOG_Payment(this.payment_result, "Process Ticket Result");
        }


        #endregion New Clover Process !!!!

        public string POS_Payment_GetError(string isCreditCard, string fastPayAmount, string isServiceNow, ref string use_system_credit_setting)
        {
            //try
            //{
            //    this.payment_result = "";

            //    bool require_nails_tech = true;
            //    if (isServiceNow.Equals("1"))
            //    {
            //        string inservice_setting = Constants.inservice_setting;
            //        if (inservice_setting.Equals("Require Nails Tech"))
            //            require_nails_tech = true;
            //        else if (inservice_setting.Equals("No Nails Tech"))
            //            require_nails_tech = false;
            //        else if (inservice_setting.Equals("No nails Tech and Services require"))
            //            require_nails_tech = false;
            //    }
            //    else if (isServiceNow.Equals("2"))  //SAVE ALLOW
            //        require_nails_tech = false;

            //    //Check ANY Staff
            //    if (require_nails_tech)
            //    {
            //        foreach (UCCartItem control in myCartTouchScrollPanel.Content.Controls.OfType<UCCartItem>())
            //        {
            //            if (control.IsAnyNailsTech())
            //            {
            //                this.payment_result = "Please add nails tech for service: " + control.service_name;
            //                return this.payment_result;
            //            }
            //        }
            //    }

            //    bool exits_cash_tip_line = chkCashTip.Checked;
            //    if (!isServiceNow.Equals("2"))  //SAVE BUTTON
            //    {
            //        foreach (UCCartItem control in myCartTouchScrollPanel.Content.Controls.OfType<UCCartItem>())
            //        {
            //            if (control.service_id.Trim().Length < 6)
            //            {
            //                this.payment_result = "Please add service for this customer!";
            //                return this.payment_result;
            //            }
            //            if (isServiceNow.Equals("0") && (control.service_name.ToUpper().Equals("ANY") || control.service_id.Trim().Length < 6))
            //            {
            //                this.payment_result = "Please add service for this customer!";
            //                return this.payment_result;
            //            }

            //            if (control.cash_tip.Equals("1"))
            //            {
            //                exits_cash_tip_line = true;
            //            }
            //        }
            //    }

            //    string couponCode = txtCouponCode.Text;
            //    string coupon_discountPrice = txtTotalCoupon.Text.Length <= 0 ? "0" : Utilitys.getTotalAmount(txtTotalCoupon.Text.Trim()).ToString();

            //    string gift_card_no = txtGiftCardCode.Text;
            //    string giftcard_value = txtGiftCardAmount.Text.Length <= 0 ? "0" : Utilitys.getTotalAmount(txtGiftCardAmount.Text).ToString();

            //    string voucher_code = txtVoucherCode.Text;
            //    string voucher_amount = txtTotalReedemVoucher.Text.Length <= 0 ? "0" : Utilitys.getTotalAmount(txtTotalReedemVoucher.Text.Trim()).ToString();

            //    string total_redeem = txtTotalReedem.Text.Length <= 0 ? "0" : Utilitys.getTotalAmount(txtTotalReedem.Text).ToString();
            //    string redeem_balance = this.reward_balance.Length <= 0 ? "0" : this.reward_balance;
            //    if (!Core.USING_REWARD_PERCENT())
            //    {
            //        if (double.Parse(total_redeem) > double.Parse(redeem_balance))
            //        {
            //            this.payment_result = "Please check Reward amount";
            //            return this.payment_result;
            //        }
            //    }

            //    string cusId = "0"; //GUEST
            //    foreach (UCCustomerDrapHere control in panelItemDrapHere.Controls.OfType<UCCustomerDrapHere>())
            //    {
            //        cusId = control.id;
            //    }

            //    //Check xem có phải đang vào Pending Payment để thanh toán / update cho ticket này không
            //    string _ticketId = "";
            //    int numTicket = 0;
            //    int lỉne = 0;
            //    bool exits_new_ticket = false;
            //    bool exits_item_in_current_ticket = false;
            //    foreach (UCCartItem control in myCartTouchScrollPanel.Content.Controls.OfType<UCCartItem>())
            //    {
            //        lỉne++;

            //        if (control.ticketId.Trim().Length > 0 && !control.ticketId.Equals("0"))
            //        {
            //            if (!_ticketId.Contains(control.ticketId))
            //            {
            //                _ticketId += control.ticketId + ",";
            //                numTicket++;
            //            }

            //            if (control.ticketId.Equals(this.selected_ticket))  //ticket đang update
            //                exits_item_in_current_ticket = true;
            //        }
            //        else
            //        {
            //            exits_new_ticket = true;
            //        }

            //        if (!isServiceNow.Equals("1") && control.is_quick_menu && !Constants.quickmenu_checkOut.Equals("ALLOW"))
            //        {
            //            this.payment_result = "Please select service quickmenu on line: " + lỉne;
            //            return this.payment_result;
            //        }

            //        if (isServiceNow.Equals("0") && control.IsAnyNailsTech())
            //        {
            //            this.payment_result = "Please select nails tech for service: " + control.service_name + " on line: " + lỉne;
            //            return this.payment_result;
            //        }

            //        //Done hết mới cho payment
            //        if (Core.GET_PAYMENT_MODE() == PAYMENT_MODE.USING_SERVICE_NOW)
            //        {
            //            if (isServiceNow.Equals("0") && control.staff_done.Equals("0"))
            //            {
            //                this.payment_result = "Please ask " + control.staff_name.ToUpper() + " to complete and mark " + control.service_name.ToUpper() + " as Done before payment.";
            //                return this.payment_result;
            //            }
            //        }
            //    }

            //    //if (isServiceNow.Equals("1"))  //Nếu bấm Service Now thì không cho add ticket khác 
            //    //{
            //    //    if (numTicket >= 2)
            //    //    {
            //    //        return "Please check ticket to Service Now: not allow add combine mode. Please reset PAYMENT CART !!!";
            //    //    }
            //    //}
            //    if (this.selected_ticket_combine)
            //    {
            //        if (numTicket <= 1)
            //        {
            //            this.payment_result = "Please check the selected combine tickets. Currently only one ticket is selected. Please enter the CANCEL button to reset CART CHECKOUT";
            //            return this.payment_result;
            //        }
            //    }

            //    if (this.repair_ticket_mode)  //Repair ticket chỉ thao tác cho 1 ticket
            //    {
            //        if (numTicket >= 2)
            //        {
            //            this.payment_result = "Please check ticket to Repair. no combine ticket apply";
            //            return this.payment_result;
            //        }
            //    }

            //    //Check trường hợp các tiệm xài nút SAVE chọn ticket này nhưng ruột của ticket khác mà không phải combine
            //    if (!string.IsNullOrEmpty(this.selected_ticket) && !this.selected_ticket_combine)
            //    {
            //        if (!exits_new_ticket && !exits_item_in_current_ticket)
            //        {
            //            this.payment_result = "Please check ticket to process: not allow add service for another ticket to current selected ticket";
            //            return this.payment_result;
            //        }
            //    }

            //    //CHECK COMBINE HAY KHÔNG
            //    string error = "";
            //    bool is_combine_ticket = MainCart.checkIsCombine(myCartTouchScrollPanel, ref error);
            //    if (!string.IsNullOrEmpty(error))
            //    {
            //        this.payment_result = "Error: " + error;
            //        return this.payment_result;
            //    }

            //    if (is_combine_ticket && this.repair_ticket_mode)
            //    {
            //        this.payment_result = "Not Allow Repair Ticket To Combine Payment !!!";
            //        return this.payment_result;
            //    }

            //    //CHECK INSERVICE Condition => INSERVICE VẪN CHO SAVE HOẶC PAYMENT
            //    //bool is_inservice = MainCart.checkIsInservice(myCartTouchScrollPanel, (isServiceNow.Equals("1") ? true : false), (isServiceNow.Equals("0") ? true : false), ref error);
            //    //if (!string.IsNullOrEmpty(error))
            //    //{
            //    //    return "Error: " + error;
            //    //}

            //    string ticket_update_id = "";
            //    if (!is_combine_ticket)
            //        ticket_update_id = MainCart.getCurrentTicketId(myCartTouchScrollPanel, this.selected_ticket, ref error);
            //    else
            //        ticket_update_id = MainCart.getCurrentTicketCombineId(myCartTouchScrollPanel, this.selected_ticket_combine_id, ref error);

            //    if (!string.IsNullOrEmpty(error))
            //    {
            //        this.payment_result = "Error: " + error;
            //        return this.payment_result;
            //    }

            //    string spId = "";
            //    string spName = "";
            //    string spPrice = "";
            //    string spUnitPrice = "";
            //    string spDiscount = "";
            //    string spTax = "";
            //    string spQuantity = "";
            //    string spStaff = "";
            //    string spTip = "";
            //    string spTicket = "";
            //    string spColor = "";
            //    string spCatId = "";

            //    foreach (UCCartItem control in myCartTouchScrollPanel.Content.Controls.OfType<UCCartItem>())
            //    {
            //        spId += control.service_id + "_";
            //        spName += control.service_name + "_";
            //        spPrice += (control.price.Trim().Length <= 0 ? "0" : control.price.Trim()) + "_";
            //        spUnitPrice += (control.price.Trim().Length <= 0 ? "0" : control.price.Trim()) + "_";
            //        spDiscount += (control.discount.Trim().Length <= 0 ? "0" : control.discount.Trim()) + "_";
            //        spTax += "0" + "_";
            //        spQuantity += (control.quantity.Trim().Length <= 0 ? "1" : control.quantity.Trim()) + "_";
            //        spStaff += control.staff_id + "_";

            //        spTip += (control.tip.Trim().Length <= 0 ? "0" : control.tip) + "_";
            //        spTicket += (control.ticketId.Trim().Length <= 0 ? "0" : control.ticketId) + "_"; //Combine ticket, item

            //        spColor += (control.color.Trim().Length <= 0 ? "NA" : control.color) + "_";
            //        spCatId += (control.catalog_id.Trim().Length <= 0 ? "0" : control.catalog_id) + "_";

            //        if (isServiceNow.Equals("0") && !this.repair_ticket_mode)
            //        {
            //            if ((control.price.Trim().Length <= 0 || control.price.Trim().Equals("0"))
            //                && (control.tip.Trim().Length <= 0 || control.tip.Equals("0")))  //Aloow Price = 0
            //            {
            //                this.payment_result = "Error: Please enter service price";
            //                return this.payment_result;
            //            }
            //        }

            //        //Check duplicate
            //    }

            //    if (string.IsNullOrEmpty(spId) && string.IsNullOrEmpty(spStaff))
            //    {
            //        this.payment_result = "Error: Payment cart blank !!!";
            //        return this.payment_result;
            //    }

            //    if (!isServiceNow.Equals("2") && (spId.Trim().Length <= 0 || spStaff.Trim().Length <= 0))
            //    {
            //        this.payment_result = "Please check nails technician, services";
            //        return this.payment_result;
            //    }

            //    if (isServiceNow.Equals("0") && isCreditCard.Equals("1"))
            //    {
            //        //Check CLover Connect
            //        MainPOS mainPOS = new MainPOS();
            //        use_system_credit_setting = mainPOS.GetStoreSetting("use_system_credit_setting");
            //        if (use_system_credit_setting.Equals("1")) //On dung Clover
            //        {
            //            if (exits_cash_tip_line)
            //            {
            //                this.payment_result = "You Can Not Add Cash Tip With Credit Card Payment Here, Please Go to Adjust After Payment";
            //                return this.payment_result;
            //            }

            //            if (Constants.credit_card_device.Equals("CLOVER"))
            //            {
            //                if (!cloverStatus)
            //                {
            //                    //this.ReconnectCloverConnector();  //Auto load on backgound !!!
            //                    FormCloverReConnect frm = new FormCloverReConnect(this);
            //                    frm.ShowDialog(this);
            //                    frm.Dispose();

            //                    this.payment_result = "Waiting Clover Connect";
            //                    return this.payment_result;
            //                }

            //                //Nếu Payment Credit thì lưu lại LOG ticket trên server, restore khi bị lỗi
            //                //string msg = this.POS_TEMP(isCreditCard, "0", fastPayAmount, this.paymentList, 0);
            //                //if (msg.Trim().Length > 0)
            //                //{
            //                //    return msg;
            //                //}
            //            }
            //            else  //Codeday
            //            {
            //                //string msg = this.POS_TEMP(isCreditCard, "0", fastPayAmount, this.paymentList, 0);
            //                //if (msg.Trim().Length > 0)
            //                //{
            //                //    return msg;
            //                //}
            //            }
            //        }
            //    }

            //    if (!string.IsNullOrEmpty(this.selected_ticket) && cusId.Trim().Length > 0 && !cusId.Equals("0"))
            //    {
            //        if (this.selected_ticket.Equals(cusId)) //Chặn trường hợp payment save mất khách hàng
            //        {
            //            this.payment_result = "Please check customer for ticket payment !!";
            //            return this.payment_result;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    this.payment_result = "Process Ticket Check-Error Exception: " + ex.Message;
            //    return this.payment_result;
            //}

            return "";
        }

        public void POS_CHECKOUT(string isCreditCard, List<PaymentModel> paymentList, double total_payment, string isSave = "0")
        {
            try
            {
                this.payment_result = "";

                string items = "";
                int numTicket = 0;
                string _ticketId = "";

                foreach (UCCartItem control in panelCartItemsTouch.Content.Controls.OfType<UCCartItem>())
                {
                    items += @"{
                                'orderId': '" + (string.IsNullOrEmpty(control.cart_order_id) ? "0" : control.cart_order_id) + @"',
                                'itemId': " + control.item_id + @",
                                'itemName': '" + control.item_name + @"',
                                'qty': " + control.quantity + @",
                                'price': " + Utilitys.getTotalAmount(control.price) + @",
                                'priceDiscount': 0,
                                'discount': 0,
                                'subTotal': " + control.subTotal() + @" 
                            },";

                    if (control.cart_order_id.Trim().Length > 0 && !control.cart_order_id.Equals("0"))
                    {
                        if (!_ticketId.Contains(control.cart_order_id))
                        {
                            _ticketId += control.cart_order_id + ",";
                            numTicket++;
                        }
                    }
                }

                if (numTicket >= 2)
                {
                    CustomMessageBox.Show("Please check sale order to Process. no combine order apply");
                    return;
                }

                if (items.Trim().Length <= 0)
                {
                    CustomMessageBox.Show("Please check item in cart");
                    return;
                }

                items = "[" + items.Substring(0, items.Length - 1) + "]";

                double cash = CartHelper.GetPaymentCashTotal(paymentList);
                double charge = CartHelper.GetPaymentChargeTotal(paymentList);
                string paymentId = "";
                string cloverOrderId = "";
                double cloverAmount = charge;
                double cloverTip = 0;

                // Serialize paymentList — luôn gửi bất kể loại payment (cash / credit / mixed)
                string jPaymentList = JsonConvert.SerializeObject(paymentList ?? new List<NailsChekin.Models.ListModel.PaymentModel>(), Formatting.None);

                //Call API
                this.EnableDisableControl(false);

                string customerId = Utilitys.CheckIsNumber(this.customer_selected) ? this.customer_selected : "0";
                string order_update_id = Utilitys.CheckIsNumber(this.curent_order_payment_id) ? this.curent_order_payment_id : "0";
                string DATA = @"{
                                  'id': " + order_update_id + @",
                                  'orderDate': '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + @"',
                                  'comment': '',
                                  'customerId': " + customerId + @",
                                  'customerName': '" + Regex.Replace(this.customer_name, "'", "") + @"',
                                  'customerPhone': '',
                                  'orderStatus': 0,
                                  'subtotal': " + Utilitys.getTotalAmount(lbCart_SubTotal.Text) + @",
                                  'reward': 0,
                                  'totalReward': 0,
                                  'discount': " + this.discount_redeem + @",
                                  'giftcardNumber': '',
                                  'giftcardAmount': 0,
                                  'tax': " + this.tax_redeem + @",
                                  'tip': 0,
                                  'orderTotal': " + Utilitys.getTotalAmount(lbCart_Total.Text) + @",
                                  'methodOfPayment': " + (isCreditCard.Equals("1") ? "1" : "0") + @",
                                  'cash': " + cash + @",
                                  'charge': " + charge + @",
                                  'paymentId': '" + paymentId + @"',
                                  'cloverOrderId': '" + cloverOrderId + @"',
                                  'employeeId': '',
                                  'cloverAmount': " + cloverAmount + @",
                                  'cloverTip': " + cloverTip + @",
                                  'items': " + items + @",
                                  'paymentList': " + jPaymentList + @",
                                  'orderSource': '" + (string.IsNullOrEmpty(this.orderSource) ? "POS" : this.orderSource) + @"'
                                }";

                string responce = Utilitys.CALL_API("Order/createUpdateOrder", DATA, "POST", true);
                if (responce.ToUpper().StartsWith("ERROR")) //Fail
                {
                    CustomMessageBox.Show("Process Ticket Error: " + Environment.NewLine + responce);
                    this.EnableDisableControl(true);
                }
                else
                {
                    string ticketId = responce;
                    if (isSave.Equals("0"))
                    {
                        this.EnableDisableControl(true);
                        this.ResetAllData();
                        this.paymentList = null;

                        Models.Helper.PrinterLocalHelper.PrintDirectTicket(ticketId, "");
                    }
                    else if (isSave.Equals("1")) //OPEN TICKET
                    {
                        this.selected_ticket = ticketId;
                        this.curent_order_payment_id = ticketId;
                    }

                    this.BeginInvoke(new Action(() =>
                    {
                        this.Activate();
                        this.Focus();
                    }));
                }
            }
            catch (Exception ex)
            {
                Utilitys.SaveLOG_Payment(ex.Message, "Process Ticket Error");
                CustomMessageBox.Show("Process Ticket Error: " + ex.Message);
                this.EnableDisableControl(true);
            }
        }

        //public Cart POS_GetCart()
        //{
        //    try
        //    {
        //        Cart cart = new Cart();

        //        double pay_amount = 0;

        //        string couponCode = txtCouponCode.Text;
        //        string coupon_discountPrice = txtTotalCoupon.Text.Length <= 0 ? "0" : Utilitys.getTotalAmount(txtTotalCoupon.Text.Trim()).ToString();

        //        string gift_card_no = txtGiftCardCode.Text;
        //        string giftcard_value = txtGiftCardAmount.Text.Length <= 0 ? "0" : Utilitys.getTotalAmount(txtGiftCardAmount.Text).ToString();

        //        string voucher_code = txtVoucherCode.Text;
        //        string voucher_amount = txtTotalReedemVoucher.Text.Length <= 0 ? "0" : Utilitys.getTotalAmount(txtTotalReedemVoucher.Text.Trim()).ToString();

        //        string total_reward = txtRewardBalance.Text.Length <= 0 ? "0" : Utilitys.getTotalAmount(txtRewardBalance.Text).ToString();
        //        string total_redeem = txtTotalReedem.Text.Length <= 0 ? "0" : Utilitys.getTotalAmount(txtTotalReedem.Text).ToString();

        //        string cusId = "0";
        //        string cusName = "GUEST";
        //        string cusPhone = "0";
        //        foreach (UCCustomerDrapHere control in panelItemDrapHere.Controls.OfType<UCCustomerDrapHere>())
        //        {
        //            cusId = control.id;
        //            cusName = control.name;
        //            cusPhone = control.phone;
        //        }

        //        string appointmentId = this.appoiment_waiting_id;
        //        string total_paying = pay_amount.ToString();
        //        string order_discount = txtDiscountPercent.Text.Trim().Length <= 0 ? "0" : txtDiscountPercent.Text.Trim();
        //        string order_discount_fix = txtDiscountFixAmount.Text.Trim().Length <= 0 ? "0" : txtDiscountFixAmount.Text.Trim();
        //        string paid_by = "Cash";

        //        string tax_include = chkIncludeTax.Checked ? "1" : "0";
        //        double total_discount = Utilitys.getTotalAmount(txtTotalDiscount.Text);
        //        double subTotal = Utilitys.getTotalAmount(txtSubTotal.Text);
        //        double subTotalInludeTax = Utilitys.getTotalAmount(txtSubTotal_TAX.Text);
        //        string tax_amount = (subTotalInludeTax - subTotal) < 0 ? "0" :Math.Round(subTotalInludeTax - subTotal, 2).ToString();
        //        double final_amount = Utilitys.getTotalAmount(lbCart_AmtDue.Text);

        //        total_discount = Math.Round(total_discount, 2);
        //        subTotal = Math.Round(subTotal, 2);
        //        subTotalInludeTax = Math.Round(subTotalInludeTax, 2);
        //        final_amount = Math.Round(final_amount, 2);

        //        //string confirm_deposit = "YES";
        //        //appt_deposit_id = this.appt_deposit_id;
        //        //appt_deposit_amount = this.appt_deposit_amount;

        //        //if (this.total_promotion_discount == null)
        //        //    this.total_promotion_discount = 0;
        //        if (this.promotion_json.Trim().Length <= 0)
        //            this.promotion_json = "[]";

        //        string isServiceNow = "0";

        //        List<CartItem> listItem = Cart.GetListItem(myCartTouchScrollPanel.Content);

        //        cart.pay_amount = pay_amount;
        //        cart.couponCode = couponCode;
        //        cart.coupon_discountPrice = coupon_discountPrice;
        //        cart.gift_card_no = gift_card_no;
        //        cart.giftcard_value = giftcard_value;
        //        cart.voucher_code = voucher_code;
        //        cart.voucher_amount = voucher_amount;
        //        cart.total_reward = total_reward;
        //        cart.total_redeem = total_redeem;
        //        cart.cusPhone = cusPhone;
        //        cart.cusName = cusName;
        //        cart.cusId = cusId;
        //        cart.total_paying = total_paying;

        //        cart.order_discount = order_discount;
        //        cart.order_discount_fix = order_discount_fix;
        //        cart.total_discount = total_discount.ToString();

        //        cart.tax_include = tax_include;
        //        cart.tax_percent = tax_percent.ToString();

        //        cart.subTotal = subTotal;
        //        cart.subTotalInludeTax = subTotalInludeTax;
        //        cart.tax_amount = tax_amount;

        //        cart.total_promotion_discount = total_promotion_discount;
        //        cart.appt_deposit_amount = appt_deposit_amount;
        //        cart.isServiceNow = isServiceNow;
        //        cart.listItems = listItem;

        //        if (dual_price_percent > 0)
        //        {
        //            cart.dual_price_percent = dual_price_percent;
        //            cart.dual_price_amount = this.dual_price_amount;
        //        }
        //        else
        //        {
        //            cart.surcharge_credit_percent = surCharge_percent;
        //            cart.surcharge_credit_amount = this.surcharge_amount;
        //            cart.surcharge_debit_percent = surCharge_debit_percent;
        //            cart.surcharge_debit_amount = this.surcharge_debit_amount;
        //            cart.surcharge_unit = surCharge_unit;
        //        }

        //        if (cash_discount_percent > 0)
        //        {
        //            cart.cash_discount_percent = cash_discount_percent;
        //            cart.cash_discount_amount = Math.Round(final_amount * cash_discount_percent / 100.0, 2);
        //        }

        //        cart.cash = final_amount;
        //        cart.charge = final_amount;

        //        return cart;
        //    }
        //    catch (Exception ex)
        //    {
        //        return new Cart();
        //    }
        //}

        public string current_clover_token = "";
        public string Clover_Payment_Simple(double total_credit_amount, double amount_charge, double pos_tip, string action_type = "TICKET")
        {
            this.current_clover_token = "PAY_12399";
            if (action_type.Equals("GIFTCARD"))
                this.current_clover_token = "GIFT_12345";
            else if (action_type.Equals("APPT"))
                this.current_clover_token = "APPT_12345";

            //Hàm mày chỉ thu tiền clover, thu xong rồi mới xử lý tiếp các bước sau !!!
            if ((amount_charge + pos_tip) <= 0)
            {
                CustomMessageBox.Show("Please check Amount Charge.");
                CartEnableControl();
                return "";
            }

            //Call API
            this.EnableDisableControl(false);

            long total_pay_clover = (long)(Math.Round(amount_charge * 100, 0)); // Clover X 100

            //Tip cho trên POS
            //double total_tip = 0;
            //if (amount_charge >= total_credit_amount) //Nếu chỉ thanh toán 1 thẻ mới cho chạy tip trên pos được, không sẽ bị double tip
            //{
            //    total_tip = 0;
            //}
            //long tip_amount = (long)(Math.Round(total_tip * 100, 0));

            long tip_amount = (long)(Math.Round(pos_tip * 100, 0));

            this.Pay(this.current_clover_token, 0, tip_amount, total_pay_clover);
            return "";
        }

        public FormCreditProcessing frmCreditProcessing;
        //public string CodePay_Payment_Simple(double total_credit_amount, double amount_charge)
        //{
        //    //if (total_credit_amount <= 0 || amount_charge <= 0)
        //    //{
        //    //    CustomMessageBox.Show("Please check Amount Charge.");
        //    //    return "";
        //    //}

        //    //if (CreditCardLib.GET_CODEPAY_DEVICE() == CODEPAY_DEVICE.T2)
        //    //{
        //    //    string merchant_order_no = Utilitys.createRamdomKey();
        //    //    string data = "{";
        //    //    data += "'action':'payment',";
        //    //    data += "'merchant_order_no':'" + merchant_order_no + "',";
        //    //    data += "'token':'" + this.current_clover_token + "',";
        //    //    data += "'amount':" + Math.Round(amount_charge, 2) + ",";
        //    //    data += "'pos_tip':0";
        //    //    data += "}";
        //    //    Task.Run(() => MainCart.SendRequestToT2(data));  //Call send socket trước để UI không chặn 

        //    //    // Đợi tối đa 2 phút nhận tín hiệu T2 bắn lại qua socket
        //    //    StartT2PaymentTimeoutWatcher(merchant_order_no);

        //    //    //Call API
        //    //    CodePay_ShowHide_Processing(true);
        //    //    return ""; //Doi T2 thu tien ban ve
        //    //}

        //    //if (P5Lib.Get_P5_ConecttionType_Setting() != P5_CONNECTTION_TYPE.CLOUD)
        //    //    return CodePay_Payment_WLAN_Simple(total_credit_amount, amount_charge);

        //    //#region WIFI PROCCESS
        //    //if (string.IsNullOrEmpty(Constants.codepay_merchant_no) || string.IsNullOrEmpty(Constants.codepay_store_no))
        //    //{
        //    //    CustomMessageBox.Show("Please check CodePay Merchant INFO");
        //    //    return "";
        //    //}

        //    //this.current_clover_token = "CODEPAY_12399";
        //    //this.curent_order_payment_id = Utilitys.createRamdomKey();

        //    ////CALL API PAYMENT CODE PAY
        //    //total_credit_amount = Math.Round(total_credit_amount, 2);
        //    //string msg = this.CodePay_Pay_Order(this.curent_order_payment_id, "0", total_credit_amount.ToString(), this.current_clover_token);
        //    //if (msg.StartsWith("Error"))
        //    //{
        //    //    CustomMessageBox.Show(msg);
        //    //}
        //    //else  //SHOW PROCESSING
        //    //{
        //    //    frmCreditProcessing = new FormCreditProcessing(this);
        //    //    frmCreditProcessing.StartPosition = FormStartPosition.CenterScreen;
        //    //    frmCreditProcessing.ShowDialog();
        //    //}
        //    //return "";
        //    //#endregion
        //}

        public async Task<string> CodePay_Payment_SimpleAsync(double total_credit_amount, double amount_charge)
        {
            // TH1: thiết bị CODEPAY T2 (giữ sync như cũ, vì bạn đã Task.Run rồi)
            if (CreditCardLib.GET_CODEPAY_DEVICE() == CODEPAY_DEVICE.T2)
            {
                string merchant_order_no = Utilitys.createRamdomKey();

                lock (_t2PaymentLock)
                {
                    if (_isT2PaymentPending)
                    {
                        return "Error: T2 is still processing the previous payment. Please wait for the response or cancel the current payment before starting a new one.";
                    }

                    _isT2PaymentPending = true;
                    _pendingT2MerchantOrderNo = merchant_order_no;
                }

                try
                {
                    string data = "{";
                    data += "'action':'payment',";
                    data += "'merchant_order_no':'" + Utilitys.createRamdomKey() + "',";
                    data += "'token':'" + this.current_clover_token + "',";
                    data += "'amount':" + Math.Round(amount_charge, 2) + ",";
                    data += "'pos_tip':0";
                    data += "}";

                    // Call send socket trước để UI không chặn 
                    //Task.Run(() => MainCart.SendRequestToT2(data));

                    // Đợi tối đa 2 phút nhận tín hiệu T2 bắn lại qua socket
                    StartT2PaymentTimeoutWatcher(merchant_order_no);

                    // Call API
                    CodePay_ShowHide_Processing(true);
                    return ""; // đợi T2 thu tiền bắn về
                }
                catch (Exception ex)
                {
                    ClearT2PendingPayment();

                    CodePay_ShowHide_Processing(false);

                    return "Error: Cannot send payment request to T2. " + ex.Message;
                }
            }

            // TH2: P5 – LAN / PAIR MODE (KHÔNG CLOUD) => dùng hàm WLAN async
            if (P5Lib.Get_P5_ConecttionType_Setting() != P5_CONNECTTION_TYPE.CLOUD)
            {
                return await CodePay_Payment_WLAN_SimpleAsync(total_credit_amount, amount_charge);
            }

            #region WIFI PROCESS (CLOUD)
            if (string.IsNullOrEmpty(Constants.codepay_merchant_no) || string.IsNullOrEmpty(Constants.codepay_store_no))
            {
                return "Error: Please check CodePay Merchant INFO.";
            }

            this.current_clover_token = "CODEPAY_12399";
            this.curent_order_payment_id = Utilitys.createRamdomKey();

            // CALL API PAYMENT CODEPAY (Cloud)
            total_credit_amount = Math.Round(total_credit_amount, 2);

            string msg = await CodePay_Pay_OrderAsync(
                this.curent_order_payment_id,
                "0",
                total_credit_amount.ToString(),
                this.current_clover_token
            );

            if (!string.IsNullOrEmpty(msg) && msg.StartsWith("Error"))
            {
                return msg;
            }

            // SUCCESS => SHOW PROCESSING (form ShowDialog phải Dispose thủ công, không sẽ leak handle)
            using (frmCreditProcessing = new FormCreditProcessing(this))
            {
                frmCreditProcessing.StartPosition = FormStartPosition.CenterScreen;
                frmCreditProcessing.ShowDialog();
            }
            frmCreditProcessing = null;

            return msg ?? "";
            #endregion
        }

        #region T2 process time out
        private readonly object _t2PaymentLock = new object();
        private bool _isT2PaymentPending = false;

        private CancellationTokenSource _t2PaymentTimeoutCts;
        private string _pendingT2MerchantOrderNo = "";
        private bool _t2PaymentCompleted = false;

        private void StartT2PaymentTimeoutWatcher(string merchant_order_no)
        {
            try
            {
                // Cancel + dispose CTS cũ trước khi thay (mỗi payment 1 CTS, không dispose sẽ leak)
                var oldCts = _t2PaymentTimeoutCts;
                _t2PaymentTimeoutCts = new CancellationTokenSource();
                try { oldCts?.Cancel(); oldCts?.Dispose(); } catch { }

                CancellationToken token = _t2PaymentTimeoutCts.Token;

                Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromMinutes(480), token); //Check tới khi user hủy bên T2

                        if (token.IsCancellationRequested)
                            return;

                        bool isStillPending = false;

                        lock (_t2PaymentLock)
                        {
                            isStillPending =
                                _isT2PaymentPending &&
                                _pendingT2MerchantOrderNo == merchant_order_no;
                        }

                        if (!isStillPending)
                            return;

                        RunOnUi(() => HandleT2PaymentTimeout(merchant_order_no));
                    }
                    catch (TaskCanceledException)
                    {
                        // Ignore
                    }
                    catch (Exception ex)
                    {
                        RunOnUi(() =>
                        {
                            ClearT2PendingPayment();
                            CodePay_ShowHide_Processing(false);
                            CustomMessageBox.Show("T2 timeout watcher error: " + ex.Message);
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                ClearT2PendingPayment();
                CodePay_ShowHide_Processing(false);
                CustomMessageBox.Show("Start T2 timeout error: " + ex.Message);
            }
        }

        private void HandleT2PaymentTimeout(string merchant_order_no)
        {
            try
            {
                bool isCurrentPayment = false;

                lock (_t2PaymentLock)
                {
                    isCurrentPayment =
                        _isT2PaymentPending &&
                        _pendingT2MerchantOrderNo == merchant_order_no;
                }

                if (!isCurrentPayment)
                    return;

                ClearT2PendingPayment();

                CodePay_ShowHide_Processing(false);

                SendCancelRequestToT2(merchant_order_no);

                CustomMessageBox.Show(
                    "Payment timeout. No response was received from T2 within 2 minutes. The payment request has been cancelled."
                );
            }
            catch (Exception ex)
            {
                ClearT2PendingPayment();
                CodePay_ShowHide_Processing(false);

                CustomMessageBox.Show("Handle T2 timeout error: " + ex.Message);
            }
        }

        private bool HandleT2SocketPaymentResponseBeforeProcess(string merchant_order_no, JObject metaObj)
        {
            try
            {
                if (string.IsNullOrEmpty(merchant_order_no))
                    return true;

                bool isCurrentPayment = false;

                lock (_t2PaymentLock)
                {
                    isCurrentPayment =
                        _isT2PaymentPending &&
                        _pendingT2MerchantOrderNo == merchant_order_no;
                }

                if (!isCurrentPayment)
                {
                    return false;
                }

                // Đúng payment đang chờ thì hủy timeout và clear pending
                ClearT2PendingPayment();

                CodePay_ShowHide_Processing(false);

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_CodePay(
                    "HandleT2SocketPaymentResponseBeforeProcess error: " + ex.Message,
                    "T2-POS-ERROR"
                );

                return false;
            }
        }

        private void HandleT2CancelPaymentNotify(string merchant_order_no, JObject metaObj)
        {
            //Bấm Cancel từ T2 thi sẽ reset lại round payment mới !!!
            try
            {
                bool shouldReset = false;

                lock (_t2PaymentLock)
                {
                    // Nếu T2 có gửi merchant_order_no thì chỉ reset khi đúng payment đang pending
                    if (!string.IsNullOrEmpty(merchant_order_no))
                    {
                        shouldReset =
                            _isT2PaymentPending &&
                            _pendingT2MerchantOrderNo == merchant_order_no;
                    }
                    else
                    {
                        // Nếu T2 không gửi merchant_order_no nhưng POS đang pending,
                        // vẫn nên reset để tránh kẹt vòng payment.
                        shouldReset = _isT2PaymentPending;
                    }
                }

                if (!shouldReset)
                {
                    LogHelper.SaveLOG_CodePay(
                        "Ignore T2 cancel because it is not current pending payment. merchant_order_no=" + merchant_order_no,
                        "T2-POS-CANCEL-IGNORE"
                    );

                    return;
                }

                LogHelper.SaveLOG_CodePay(
                    "T2 payment cancelled. merchant_order_no=" + merchant_order_no + ", meta=" + metaObj.ToString(),
                    "T2-POS-CANCEL"
                );

                // Nhả lock + cancel timeout
                ClearT2PendingPayment();

                // Đóng các UI payment đang mở
                this.CloseConfirm_Credit_Bill_IfOpen();
                CodePay_ShowHide_Processing(false);

                // Optional: reset current payment id/token nếu anh muốn ép tạo mới hoàn toàn lần sau
                this.curent_order_payment_id = "";
                this.current_clover_token = "";

                CustomMessageBox.Show("Payment was cancelled on T2. You can start a new payment now.");
            }
            catch (Exception ex)
            {
                ClearT2PendingPayment();
                CodePay_ShowHide_Processing(false);

                LogHelper.SaveLOG_CodePay(
                    "HandleT2CancelPaymentNotify error: " + ex.Message,
                    "T2-POS-CANCEL-ERROR"
                );

                CustomMessageBox.Show("T2 cancel payment error: " + ex.Message);
            }
        }

        private void SendCancelRequestToT2(string merchant_order_no)
        {
            //try
            //{
            //    string data = "{";
            //    data += "'action':'cancel_payment',";
            //    data += "'merchant_order_no':'" + merchant_order_no + "',";
            //    data += "'token':'CANCEL'";
            //    data += "}";

            //    _ = Task.Run(() => MainCart.SendRequestToT2(data));
            //}
            //catch
            //{
            //    // Không throw lỗi ở đây để tránh crash POS
            //}
        }

        private void ClearT2PendingPayment()
        {
            lock (_t2PaymentLock)
            {
                _isT2PaymentPending = false;
                _pendingT2MerchantOrderNo = "";
            }

            try
            {
                _t2PaymentTimeoutCts?.Cancel();
            }
            catch
            {
                // Ignore
            }
        }

        #endregion T2 process time out


        public string CodePay_Payment_WLAN_Simple(double total_credit_amount, double amount_charge)
        {
            if (!CodePay_CheckConnect())
            {
                CustomMessageBox.Show("Please check CodePay WLAN Connection !!!");
                return "";
            }

            this.curent_order_payment_id = Utilitys.createRamdomKey();
            if (string.IsNullOrEmpty(this.current_clover_token))
                this.current_clover_token = "CODEPAY_12999";

            //Hàm mày chỉ thu tiền codepay, bắn lệnh qua codepay thu tiền rồi nhận socket !!!!
            if (amount_charge <= 0)
            {
                CustomMessageBox.Show("Please check Amount Charge.");
                return "";
            }

            //CALL API PAYMENT CODE PAY
            total_credit_amount = Math.Round(total_credit_amount, 2);
            double total_tip = 0;

            string msg = this.CodePay_Pay_Order(this.curent_order_payment_id, total_tip.ToString(), total_credit_amount.ToString(), this.current_clover_token);
            if (msg.StartsWith("Error"))
            {
                CustomMessageBox.Show(msg);
            }
            else  //Show Form Processing
            {
                frmCreditProcessing = new FormCreditProcessing(this);
                frmCreditProcessing.StartPosition = FormStartPosition.CenterScreen;
                frmCreditProcessing.ShowDialog();
                frmCreditProcessing.Dispose();
                frmCreditProcessing = null;
            }

            return "";
        }

        public async Task<string> CodePay_Payment_WLAN_SimpleAsync(double total_credit_amount, double amount_charge)
        {
            // ❗ Nếu đã dùng CodePayHelper rồi thì KHÔNG cần chặn vì chưa connect
            // Device mất kết nối thì helper sẽ tự reconnect trong CodePay_Pay_OrderAsync

            this.curent_order_payment_id = Utilitys.createRamdomKey();

            if (string.IsNullOrEmpty(this.current_clover_token))
                this.current_clover_token = "CODEPAY_12999";

            // CALL API PAYMENT CODEPAY
            total_credit_amount = Math.Round(total_credit_amount, 2);
            double total_tip = 0;

            // ⬇⬇⬇  GỌI ASYNC  ⬇⬇⬇
            string msg = await CodePay_Pay_OrderAsync(
                this.curent_order_payment_id,
                total_tip.ToString(),
                total_credit_amount.ToString(),
                this.current_clover_token
            );

            if (!string.IsNullOrEmpty(msg) && msg.StartsWith("Error"))
            {
                return "Không có kết nối được với máy cà thẻ" + Environment.NewLine + msg;
            }

            // Thành công => show form processing (using đảm bảo dispose cả khi ShowDialog throw)
            using (frmCreditProcessing = new FormCreditProcessing(this))
            {
                frmCreditProcessing.StartPosition = FormStartPosition.CenterScreen;
                frmCreditProcessing.ShowDialog();
            }
            frmCreditProcessing = null;

            return msg;   // hoặc "" nếu bạn không cần dùng kết quả
        }

        public void ResetAllData()
        {
            try
            {
                this.BeginInvoke(new Action(() =>
                {
                    //Remove ONLY CART ITEM, NO HEADER
                    //Snapshot ToList trước khi remove — không sửa collection khi đang duyệt (nguyên nhân trước đây phải chạy 5 lần)
                    var cartItems = panelCartItemsTouch.Content.Controls.OfType<UCCartItem>().ToList();
                    foreach (var control in cartItems)
                    {
                        panelCartItemsTouch.Content.Controls.Remove(control);
                        control.MyDispose();
                        control.Dispose();
                    }

                    this.selected_ticket = "";
                    this.curent_order_payment_id = "";
                    this.current_clover_token = "";

                    this.selected_ticket_combine_id = "";
                    this.selected_ticket_combine = false;

                    this.voucher_code_apply = "";
                    this.voucher_redeem_amount = 0;

                    svgCart_RemoveCustomer.Visible = false;
                    CartHelper.RemoveCustomerInfoSignalR();
                    lbCart_CustomerName.Text = "CUSTOMER: GUEST";
                    this.customer_selected = "";
                    this.reward_balance = "0";
                    this.reward_percent_discount = 0;
                    this.reward_percent_owner = 100; //mac dinh chu chiu
                    this.credit_balance = "0";

                    this.tax_percent = Core.TAX_PERCENT();
                    this.tax_include = true;
                    this.tax_redeem = 0;

                    this.discount_value = 0;
                    this.discount_unit = "%";
                    this.discount_redeem = 0;

                    this.coupon_redeem_amount = 0;
                    this.coupon_code_apply = "";
                    
                    this.total_pending = 0;
                    lbCart_AmtDue.Text = "$0.00";
                    lbCart_Tender.Text = "$0.00";
                    lbCart_Paided.Text = "$0.00";

                    this.pincode_payment = "";

                    this.UpdatePaymentCartAmount();

                    //Clear Payment credit
                    this.surcharge_amount = 0;
                    this.surcharge_debit_amount = 0;
                    if (this.paymentList != null)
                    {
                        this.paymentList.Clear();
                        this.paymentList = null;
                    }

                    CodePay_ShowHide_Processing();
                    ResetHandledPayments();
                    CartDisableControl();

                    SyncStartScanPanel();
                    Core.ClearMemory();

                }));
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_Crash(ex.Message + " \nStackTrace: " + ex.StackTrace, "ResetAllData Exception");
            }
        }

        #endregion

        #region Clover, Codepay Implement Event + Sale Function
        public string curent_order_payment_id = "";
        string curent_order_payment_tip = "0";
        string include_pos_tip = "-1";

        #region Clover Function

        private void InitClover()
        {
            // QUAN TRỌNG: cleanup instance cũ để tránh double callback
            CleanupClover();

            _clover = new CloverManager(
                owner: this,
                ui: uiThread,
                updateStatus: UpdateCreditDeviceStatus,
                enableDisableClover: EnableDisableClover,
                setCloverStatus: v => cloverStatus = v,
                setWaitingFlag: v => waiting_clover_process = v,
                setConfirmPrint: v => clover_confirm_print = v,
                getToken: () => current_clover_token,
                log: msg => UpdateCloverLog(current_clover_token, msg),
                showSignatureForm: ShowSignatureFormHandler // <-- handler
            );

            // đăng ký events
            HookCloverEvents();

            // connect
            var ok = _clover.ConnectNetworkIfPaired(
                Properties.Settings.Default.lastWSEndpoint,
                Properties.Settings.Default.pairingAuthToken
            );
        }

        // ====== Cleanup instance cũ để tránh double callback ======
        private void CleanupClover()
        {
            if (_clover == null) return;

            try { UnhookCloverEvents(); } catch { }

            try { _clover.SaleSucceeded -= Clover_SaleSucceeded; } catch { }
            try { _clover?.Process?.StopPayment(); } catch { }

            // Nếu CloverManager có Disconnect/Dispose thì gọi
            try { _clover?.Disconnect(); } catch { }
            try { (_clover as IDisposable)?.Dispose(); } catch { }

            _clover = null;
        }

        private void HookCloverEvents()
        {
            _clover.PairingNeeded += Clover_PairingNeeded;
            _clover.PairingCodeReceived += Clover_PairingCodeReceived;
            _clover.PairingSuccess += Clover_PairingSuccess;
            _clover.PairingStateChanged += Clover_PairingStateChanged;

            _clover.SaleSucceeded += Clover_SaleSucceeded;
            _clover.SaleFailed += Clover_SaleFailed;

            _clover.TipUpdated += Clover_TipUpdated;

            _clover.VoidSucceeded += Clover_VoidSucceeded;
            _clover.VoidFailed += Clover_VoidFailed;

            _clover.RefundSucceeded += Clover_RefundSucceeded;
            _clover.RefundFailed += Clover_RefundFailed;

            _clover.PaymentFinished += Clover_PaymentFinished;
        }

        private void UnhookCloverEvents()
        {
            // nếu _clover null thì return
            if (_clover == null) return;

            _clover.PairingNeeded -= Clover_PairingNeeded;
            _clover.PairingCodeReceived -= Clover_PairingCodeReceived;
            _clover.PairingSuccess -= Clover_PairingSuccess;
            _clover.PairingStateChanged -= Clover_PairingStateChanged;

            _clover.SaleSucceeded -= Clover_SaleSucceeded;
            _clover.SaleFailed -= Clover_SaleFailed;

            _clover.TipUpdated -= Clover_TipUpdated;

            _clover.VoidSucceeded -= Clover_VoidSucceeded;
            _clover.VoidFailed -= Clover_VoidFailed;

            _clover.RefundSucceeded -= Clover_RefundSucceeded;
            _clover.RefundFailed -= Clover_RefundFailed;

            _clover.PaymentFinished -= Clover_PaymentFinished;
        }

        // Nên gọi khi bắt đầu ticket mới / clear cart / hoặc sau PaymentFinished
        private void ResetHandledPayments()
        {
            try { lock (_handledLock) _handledPaymentIds.Clear(); } catch { }
        }

        private void UpdateCloverLog(string current_clover_token, string msg)
        {

        }

        #region Chuyển Lamba qua Hander

        private readonly HashSet<string> _handledPaymentIds = new HashSet<string>();
        private readonly object _handledLock = new object();

        private bool TryMarkPaymentHandled(string paymentId)
        {
            if (string.IsNullOrWhiteSpace(paymentId))
                return true; // không có id thì cho chạy

            lock (_handledLock)
            {
                if (_handledPaymentIds.Contains(paymentId)) return false;
                _handledPaymentIds.Add(paymentId);
                return true;
            }
        }

        private void Clover_SaleSucceeded(com.clover.sdk.v3.payments.Payment payment, SaleResponse saleResp)
        {
            var paymentId = payment?.id;

            if (!TryMarkPaymentHandled(paymentId))
            {
                UpdateCreditDeviceStatus($"Duplicate SaleSucceeded ignored. paymentId={paymentId}");
                return;
            }

            if (InvokeRequired)
                BeginInvoke(new Action(() => HandleSaleSucceededOnUi(payment, saleResp)));
            else
                HandleSaleSucceededOnUi(payment, saleResp);
        }

        private void HandleSaleSucceededOnUi(com.clover.sdk.v3.payments.Payment payment, SaleResponse saleResp)
        {
            try
            {
                if (this.paymentList == null)
                    this.paymentList = new List<PaymentModel>();

                PaymentModel pm = new PaymentModel("CC", (payment?.amount ?? 0L) / 100.0);
                pm.responce.Add(new CloverResponce(saleResp, surcharge_amount, surcharge_debit_amount, dual_price_amount));
                paymentList.Add(pm);

                double total_amount = Utilitys.getTotalAmount(lbCart_AmtDue.Text);
                double total_pay_amount = CartHelper.GetPaymentTotal(this.paymentList);
                double remaining = Math.Round(total_amount - total_pay_amount, 2);

                lbCart_Tender.Text = "$" + remaining;

                LogHelper.SaveLOG_Payment(PaymentToLogLine(payment) + "  - remaining: " + remaining + " - current_clover_token: " + current_clover_token + " payment_now_mode: " + PAYMENT_NOW_MODE.GIFT_CARD.ToString(), "HandleSaleSucceededOnUi");
                if (remaining <= 0)
                {
                    POS_CHECKOUT("0", this.paymentList, total_pay_amount);
                    LogHelper.SaveLOG_Payment(this.payment_result, "Ticket Checkout Now Result");
                }
                else
                {
                    CHECK_PAYMENT_CORRECT(true);
                }
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_Payment(ex.Message, "HandleSaleSucceededOnUi Exception");
            }
        }

        private void Clover_SaleFailed(string reason)
        {
            UpdateCreditDeviceStatus("Sale failed: " + reason);

            try { _clover?.Process?.StopPayment(); } catch { }

            if (!"Request Canceled".Equals(reason))  //null-safe
            {
                RunOnUi(() => ShowSaleFailed(reason));
            }
        }

        private void ShowSaleFailed(string reason)
        {
            pairingForm?.Dispose();
            pairingForm = new AlertForm(this)
            {
                Title = "Sale failed",
                Label = "Sale failed: " + reason
            };
            pairingForm.Show();
        }

        private void Clover_TipUpdated(long tipCents)
        {
            if (InvokeRequired) BeginInvoke(new Action(() => HandleTipUpdatedOnUi(tipCents)));
            else HandleTipUpdatedOnUi(tipCents);
        }

        private void HandleTipUpdatedOnUi(long tipCents)
        {
            UpdateCreditDeviceStatus($"Tip: {(tipCents / 100.0m):0.00}");
            curent_order_payment_tip = tipCents.ToString();
            if (tipCents > 0) include_pos_tip = "0";
        }

        // resp type: đổi đúng theo event của bạn nếu cần
        private void Clover_VoidSucceeded(com.clover.sdk.v3.payments.Payment payment, object resp)
        {
            UpdateCreditDeviceStatus("Void OK");
            CartEnableControl();
        }

        private void Clover_VoidFailed(string reason)
        {
            UpdateCreditDeviceStatus("Void failed: " + reason);
            CartEnableControl();
        }

        private void Clover_RefundSucceeded(com.clover.sdk.v3.payments.Payment payment, object resp)
        {
            UpdateCreditDeviceStatus("Refund OK");
            CartEnableControl();
        }

        private void Clover_RefundFailed(string reason)
        {
            UpdateCreditDeviceStatus("Refund failed: " + reason);
            CartEnableControl();
        }

        // param type có thể là Payment hoặc object tuỳ CloverManager
        private void Clover_PaymentFinished(bool _)
        {
            try { _clover?.Process?.StopPayment(); } catch { }
            CartEnableControl();

            // Tuỳ bạn: reset dedupe mỗi phiên thanh toán
            ResetHandledPayments();
        }

        private void Clover_PairingNeeded(string msg)
        {
            if (InvokeRequired) BeginInvoke((Action)(() => CustomMessageBox.Show(msg)));
            else CustomMessageBox.Show(msg);
        }

        private void Clover_PairingCodeReceived(string code)
        {
            if (InvokeRequired) BeginInvoke((Action)(() => OnPairingCode(code)));
            else OnPairingCode(code);
        }

        private void Clover_PairingSuccess(string token)
        {
            void run()
            {
                OnPairingSuccess(token);
                Properties.Settings.Default.pairingAuthToken = token;
                Properties.Settings.Default.Save();
            }

            if (InvokeRequired) BeginInvoke((Action)run);
            else run();
        }

        // PairingState type có thể khác -> sửa đúng type theo event signature của bạn
        private void Clover_PairingStateChanged(string st, string msg)
        {
            if (InvokeRequired) BeginInvoke((Action)(() => OnPairingState(st, msg)));
            else OnPairingState(st, msg);
        }

        private void ShowSignatureFormHandler(VerifySignatureRequest req) // đổi type req theo CloverManager của bạn nếu cần
        {
            uiThread.Send(_ =>
            {
                new SignatureForm(this) { VerifySignatureRequest = req }.Show();
            }, null);
        }

        private string PaymentToLogLine(com.clover.sdk.v3.payments.Payment p)
        {
            if (p == null) return "Payment=null";

            // tuỳ SDK field nào có thì dùng, field nào không có thì bỏ
            return $"Payment(id={p.id}, amount={p.amount}, tipAmount={p.tipAmount}, taxAmount={p.taxAmount}, " +
                   $"createdTime={p.createdTime}, externalPaymentId={p.externalPaymentId}, orderId={p.order?.id})";
        }

        #endregion End chuyển lamba qua hander

        // ví dụ khi user chọn "Network Pay" và nhập endpoint
        public void SwitchCloverMode(CloverDeviceConfiguration newConfig)
        {
            // 1) (tuỳ chọn) reset để chắc chắn thiết bị thoát trạng thái cũ
            _clover?.ResetDevice("switching Clover Mode");

            //Begin Pair lại nếu xài network pay
            // clear token để force pair
            Properties.Settings.Default.pairingAuthToken = "";
            Properties.Settings.Default.Save();

            _clover.BeginPairing(Properties.Settings.Default.lastWSEndpoint);

            //_clover.SwitchCloverMode(Properties.Settings.Default.lastWSEndpoint, Properties.Settings.Default.pairingAuthToken, newConfig);
        }

        private void EnableDisableClover(bool obj)
        {

        }

        AlertForm pairingForm;

        public void OnPairingCode(string pairingCode)
        {
            RunOnUi(() =>
            {
                pairingForm?.Dispose();
                pairingForm = new AlertForm(this);
                pairingForm.Title = "Pairing Code";
                pairingForm.Label = "Enter this code on the Clover Mini: " + pairingCode;
                pairingForm.Show();

                LogHelper.SaveLOG_Payment("OnPairingCode", "Enter this code on the Clover Mini: " + pairingCode);
            });
        }

        public void OnPairingSuccess(string pairingAuthToken)
        {
            CurrentPairingToken = pairingAuthToken;

            NailsChekin.Properties.Settings.Default.pairingAuthToken = pairingAuthToken;
            NailsChekin.Properties.Settings.Default.selectedConfig = "WS";
            NailsChekin.Properties.Settings.Default.Save();
            RunOnUi(() => pairingForm?.Dispose());

            LogHelper.SaveLOG_Payment("OnPairingSuccess", pairingAuthToken);
        }

        public void OnPairingState(string state, string message)
        {
            LogHelper.SaveLOG_Payment("OnPairingState", state + " MSG: " + message);
            if (state == "AUTHENTICATING")
            {
                RunOnUi(() =>
                {
                    pairingForm?.Dispose();
                    pairingForm = new AlertForm(this);
                    pairingForm.Title = "Pairing Security Pin";
                    pairingForm.Label = message;
                    pairingForm.Show();
                });
            }
        }

        //////////////// Sale methods /////////////
        private void Pay(string orderId, long tax_amount, long tip_amount, long amount)
        {
            this.waiting_clover_process = true;
            this.clover_confirm_print = false;

            //LogHelper.SaveLOG_Payment(orderId + " Tax:" + tax_amount + " Tip:" + tip_amount + " Amount:" + amount, "Call Pay");
            //MainPOS.UpdateCloverLog(orderId, "Call Pay: Order " + orderId + " Tax " + tax_amount + " Amount " + amount);

            this.curent_order_payment_id = orderId;
            this.curent_order_payment_tip = "0";
            this.include_pos_tip = "-1";
            bool repair_mode = false;

            // Gọi service:
            _clover.Process.StartPayment(this); // mở popup

            _clover.StartSaleWithAutoRetry(
                orderId: orderId,
                taxAmount: tax_amount,
                tipAmount: tip_amount,
                amount: amount,
                repairMode: repair_mode,
                tipsOn: Constants.chkTipsOn
            );
        }

        public void Void(string orderId, string paymentId, string cloverOrderId, string employeeId)
        {
            LogHelper.SaveLOG_Payment(orderId + " " + paymentId + " " + cloverOrderId + " " + employeeId, "Call Void");
            try
            {
                //VoidPaymentRequest request = new VoidPaymentRequest();

                //request.PaymentId = paymentId;
                //request.EmployeeId = employeeId;
                //request.OrderId = cloverOrderId;

                //request.VoidReason = "USER_CANCEL";

                ////data.CloverConnector.VoidPayment(request);

                _clover.VoidPayment(
                    paymentId: paymentId,
                    orderId: cloverOrderId
                );
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_Payment(ex.Message, "Call Void Order (" + orderId + ") Exception");
            }
        }

        public void Refund(string orderId, string paymentId, string cloverOrderId, long amount)
        {
            LogHelper.SaveLOG_Payment(orderId + " " + paymentId + " " + cloverOrderId + " " + amount, "Call Refund");
            try
            {
                //RefundPaymentRequest request = new RefundPaymentRequest();

                //request.DisablePrinting = false;
                //request.DisableReceiptSelection = false;

                //request.PaymentId = paymentId;
                //request.OrderId = cloverOrderId;

                //request.Amount = amount;
                //request.FullRefund = true;

                ////data.CloverConnector.RefundPayment(request);

                _clover.RefundPayment(
                   paymentId: paymentId,
                   amountCents: amount * 100
               );
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_Payment(ex.Message, "Call Refund Order (" + orderId + ") Exception");
            }
        }

        #endregion

        #region CodePay Function

        private string CodePay_Pay_Order(string ticketId, string tip_amount, string amount, string note)
        {
            bool repair_mode = false;

            if (P5Lib.Get_P5_ConecttionType_Setting() == P5_CONNECTTION_TYPE.CLOUD)
                return CreditCardLib.CODEPAY_PAY_ORDER(ticketId, tip_amount, amount, note, repair_mode);
            else if (P5Lib.Get_P5_ConecttionType_Setting() == P5_CONNECTTION_TYPE.USB)
                return CreditCardLib.CODEPAY_USB_PAY_ORDER(this, ticketId, tip_amount, amount, note, repair_mode);
            else //LAN AND PAIR MODE
                return CreditCardLib.CODEPAY_WLAN_PAY_ORDER(this, ticketId, tip_amount, amount, note, repair_mode);
        }

        private Task<string> CodePay_Pay_OrderAsync(string ticketId, string tip_amount, string amount, string note)
        {
            bool repair_mode = false;

            var connType = P5Lib.Get_P5_ConecttionType_Setting();
            if (connType == P5_CONNECTTION_TYPE.CLOUD)
            {
                // hàm sync -> bọc Task.FromResult
                string r = CreditCardLib.CODEPAY_PAY_ORDER(ticketId, tip_amount, amount, note, repair_mode);
                return Task.FromResult(r);
            }
            else if (connType == P5_CONNECTTION_TYPE.USB)
            {
                // USB branch trong CodePay_Pay_OrderAsync: => USB cũng chạy nền, không block UI
                return Task.Run(() =>
                {
                    return CreditCardLib.CODEPAY_USB_PAY_ORDER(this, ticketId, tip_amount, amount, note, repair_mode);
                });
            }
            else
            {
                // LAN & PAIR MODE: dùng bản async đã viết
                return Task.Run(() => CreditCardLib.CODEPAY_WLAN_PAY_ORDERAsync(this, ticketId, tip_amount, amount, note, repair_mode));
            }
        }

        public void CodePay_ShowHide_Processing(bool isShow = false, string message = "", bool is_void = false)
        {
            try
            {
                if (isShow)
                {
                    if (frmCreditProcessing == null || frmCreditProcessing.IsDisposed)
                    {
                        frmCreditProcessing = new FormCreditProcessing(this, is_void);
                    }

                    frmCreditProcessing.ShowMessage(string.IsNullOrEmpty(message) ? "" : ("Error: " + message));
                    frmCreditProcessing.ShowDialog();
                }
                else if (frmCreditProcessing != null && frmCreditProcessing.IsDisposed == false)
                {
                    void CloseAndDispose()
                    {
                        // Close không dispose form đã ShowDialog => phải Dispose tránh leak
                        var frm = frmCreditProcessing;
                        frmCreditProcessing = null;
                        try { frm?.Close(); frm?.Dispose(); } catch { }
                    }

                    if (this.InvokeRequired)
                        this.Invoke((MethodInvoker)CloseAndDispose);
                    else
                        CloseAndDispose();
                }
            }
            catch { }
        }

        #endregion

        #region CODEPAY USB/WEBSOCKET

        public bool isCodePaySocketConnect = false;
        //private WebSocketSharp.WebSocket _clientWebSocket;

        private static NailsChekin.Models.Payments.DeviceData _pairedData = null;
        public P5Lib p5Lib;

        //CodePay USB
        public P5LibUsbMode p5UsbLib;
        public ST_ECR_TRANS_CALLBACK _transCallback = new ST_ECR_TRANS_CALLBACK();

        public async Task ConnectP5Device()
        {
            // Check whether _clientWebSocket has been initialized and is in a connected state
            if (null != p5Lib._clientWebSocket && p5Lib._clientWebSocket.ReadyState == WebSocketState.Open)
            {
                // If the WebSocket connection is open, output 'Connected!!' and exit the current method
                Console.WriteLine("Connected!!");
                this.UpdateCreditDeviceStatus("Connected!!");
                return;
            }

            string serverUrl = P5Lib.Get_ServerURL();
            if (string.IsNullOrEmpty(serverUrl))
            {
                Console.WriteLine("Please check P5 Setting info !!");
                this.UpdateCreditDeviceStatus("Please check P5 Setting info");
                return;
            }

            // Initiate the WebSocket client's connection process
            p5Lib.connectServer(serverUrl);
        }

        public void CodePay_Process_OnMessage(string receivedMessage)
        {
            try
            {
                if (Utilitys.IsValidJson(receivedMessage))
                {
                    string closeOrder = JObject.Parse(receivedMessage)["closeOrder"] == null ? "" : JObject.Parse(receivedMessage)["closeOrder"].ToString();
                    if (closeOrder.ToUpper().Equals("TRUE"))
                        return;

                    JObject jObject = JObject.Parse(receivedMessage);
                    string trans_type = jObject["biz_data"]["trans_type"] == null ? "" : jObject["biz_data"]["trans_type"].ToString();
                    string topic = JObject.Parse(receivedMessage)["topic"] == null ? "" : JObject.Parse(receivedMessage)["topic"].ToString();

                    if (trans_type.Equals("2") || trans_type.Equals("3"))  //VOID = 2   REFUND = 3
                    {
                        string response_code = jObject["response_code"].ToString();
                        string response_msg = jObject["response_msg"].ToString();

                        if (!response_code.Equals("0"))
                        {
                            if (trans_type.Equals("2"))
                                CustomMessageBox.Show("Void Error: " + response_msg);
                            else
                                CustomMessageBox.Show("Refund Error: " + response_msg);
                        }
                        //else if (frmTipsAdjust != null) //Call API VOID //Call API VOID
                        //{
                        //    string orig_merchant_order_no = jObject["biz_data"]["orig_merchant_order_no"] == null ? "" : jObject["biz_data"]["orig_merchant_order_no"].ToString();
                        //    string order_amount = jObject["biz_data"]["order_amount"] == null ? "0" : jObject["biz_data"]["order_amount"].ToString();
                        //    string tip_amount = jObject["biz_data"]["tip_amount"] == null ? "0" : jObject["biz_data"]["tip_amount"].ToString();

                        //    if (string.IsNullOrEmpty(orig_merchant_order_no))
                        //    {
                        //        orig_merchant_order_no = frmTipsAdjust.credit_card_order_id;
                        //    }

                        //    if (!string.IsNullOrEmpty(orig_merchant_order_no))
                        //    {
                        //        MainPOS mainPos = new MainPOS();
                        //        string responce = mainPos.TipAdjust_RefundInvoice(trans_type, orig_merchant_order_no, order_amount, tip_amount, frmTipsAdjust.save_note, "", "");
                        //        if (Utilitys.IsValidJson(responce))
                        //        {
                        //            string id = JObject.Parse(responce)["id"].ToString();
                        //            string new_status = JObject.Parse(responce)["new_status"].ToString();

                        //            if (frmTipsAdjust != null)
                        //            {
                        //                frmTipsAdjust.ChangeTicketStatus(id, new_status);
                        //                frmTipsAdjust.ResetAllData();
                        //            }
                        //        }
                        //        else
                        //        {
                        //            CustomMessageBox.Show("Error: " + responce);
                        //        }
                        //    }
                        //}
                    }
                    else if (trans_type.Equals("1"))  //SALE, QUERY
                    {
                        this.active_interval_check_wlan_payment = false;
                        CreditCardLib.CodePay_Process_WLAN_USB_Notify(receivedMessage, this);
                    }
                    else if (topic.Equals("ecrhub.pay.query"))
                    {
                        //1 số trường hợp xài WLAN/LAN payment bên P5 xong không bắn lại POS, gài thêm interval task call check
                        if (this.active_interval_check_wlan_payment)
                        {
                            //{ "response_code":"106","callAppMode":"5","topic":"ecrhub.pay.query","biz_data":{ "expires":0,"on_screen_signature":false,"merchant_order_no":"762a300064b3445dr7BNp","on_screen_tip":false,"is_auto_settlement":false,"trans_status":"1","confirm_on_terminal":true,"limit_length":false},"app_id":"wz6012822ca2f1as78","response_msg":"Transaction failed[[E07108]Transaction does not exist, please check the transaction number]"}
                            //{"response_code":"0","callAppMode":"5","topic":"ecrhub.pay.query","biz_data":{"trans_end_time":"2025-06-09 18:28:20","expires":0,"pay_scenario":"1","entry_mode":"4","on_screen_signature":false,"merchant_order_no":"f9c6804a331841crlkfvW","on_screen_tip":false,"card_no":"41472024****2282","is_auto_settlement":false,"order_amount":"18.90","signature_url":"https://mgt.codepay.us/bis/ip/file/download/https://mgt.codepay.us/bis/ip/file/download/CACEC7888F6744A38BE21AB8B2A9733B02C3F9318F8883B48C754F7A2AFEEF9D7A86E275AA77657AAF7D2D3EFC85772DCB274D647DF560357EA25580354E7BC9781DE4130D5CDEB300D9A101D973A4E53E71953F95D78465","trans_status":"2","trans_type":"1","trans_no":"51124000030250609000001","merchant_no":"312400003068","merchant_name":"ANT POS DEMO","pay_method_id":"Visa","ref_no":"008200006622","card_type":"2","pay_channel_terminal_id":"12345678","auth_code":"310220","pay_method_category":"BANKCARD","confirm_on_terminal":true,"limit_length":false,"card_network_type":"2","pay_channel_merchant_id":"123456789012345"},"app_id":"wz6012822ca2f1as78","response_msg":"SUCCESS"}

                            string response_code = jObject["response_code"].ToString();
                            if (response_code.Equals("0"))
                            {
                                CreditCardLib.CodePay_Process_WLAN_USB_Notify(receivedMessage, this);
                                this.active_interval_check_wlan_payment = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("CodePay_Process_OnMessage Exception: " + ex.Message);
                LogHelper.SaveLOG_Crash(ex.Message, "CodePay_Process_OnMessage Exception");
            }
        }

        public void UpdateCreditDeviceStatus(string msg)
        {
            try
            {
                LogHelper.SaveLOG_Payment(msg, "UpdateCreditDeviceStatus");

                if (msg.Length > 100)
                    msg = msg.Substring(0, 100) + "...";

            }
            catch { }
        }

        public bool CodePay_CheckConnect()
        {
            if (CreditCardLib.GET_CREDIT_DEVICE() != CREDIT_DEVICE_TYPE.CODE_PAY)
                return false;

            //if (p5Lib._clientWebSocket == null)
            //    return false;

            //if (p5Lib._clientWebSocket.ReadyState == WebSocketState.Open)
            //    return true;

            //return false;

            //Auto reconnect when PAY !!!
            return true;
        }

        public bool active_interval_check_wlan_payment = false;
        public async Task<string> SendTextAsync_OLD(string message) // ==> không thực hiện Connect trong này dễ treo UI
        {
            // If _clientWebSocket is null or its state is not connected (Open), attempt to connect
            if (null == p5Lib._clientWebSocket || p5Lib._clientWebSocket.ReadyState != WebSocketState.Open)
            {
                //2025-05-25 Set time out connect => hiện tại khi không connect được treo luôn POS (do kết nối quá lâu) !!!
                var timeout = TimeSpan.FromSeconds(6);
                var task = Task.Run(() => {
                    p5Lib.disconnectServer();
                    p5Lib.connectServer(P5Lib.Get_ServerURL());
                });

                if (!task.Wait(timeout))
                {
                    // Gọi Close hoặc Abort nếu cần
                    return "Error: Connection to the CodePay device failed. Kindly restart the device and try again";
                }
                //p5Lib._clientWebSocket.Connect();

                // Wait for 1 second to give the connection some time
                await Task.Delay(1000);

                // If the connection is successful and the WebSocket state is Open, proceed with the payment operation
                if (p5Lib._clientWebSocket.ReadyState == WebSocketState.Open)
                {
                    p5Lib.startTransactions(message);
                }
            }
            else
            {
                // If the WebSocket is already connected, directly proceed with the payment operation
                p5Lib.startTransactions(message);
            }

            return "";
        }

        public async Task<string> SendTextAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return "Error: Empty payment request.";

            if (p5Lib == null)
                return "Error: CodePay device is not initialized. Please check connection settings.";

            try
            {
                // Helper sẽ tự EnsureConnected + retry + send.
                string err = await p5Lib.startTransactionsAsync(message);

                // err == "" nghĩa là send OK (hoặc đã reconnect OK)
                return err ?? "";
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_CodePay(ex.Message, "SendTextAsync Exception");
                return "Error: Unexpected error while sending payment. Please try again.";
            }
        }


        private bool _usbConnecting = false;
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            const int WM_DEVICECHANGE = 0x0219;
            const int DBT_DEVICEARRIVAL = 0x8000;
            const int DBT_DEVICEREMOVECOMPLETE = 0x8004;

            if (m.Msg == WM_DEVICECHANGE)
            {
                int wParam = unchecked((int)m.WParam.ToInt64()); // ToInt32 có thể throw OverflowException trên x64

                if (wParam == DBT_DEVICEARRIVAL) // USB được cắm vào
                {
                    if (!_usbConnecting)
                    {
                        _usbConnecting = true;  // Đặt flag để không xử lý lặp
                        Task.Run(async () =>
                        {
                            await Task.Delay(1000); // Đợi thiết bị ổn định hoàn toàn
                                                    //this.Invoke(() =>
                                                    //{
                            if (CreditCardLib.GET_CREDIT_DEVICE() == CREDIT_DEVICE_TYPE.CODE_PAY && CreditCardLib.USING_SYSTEM_CREDIT())
                            {
                                if (P5Lib.Get_P5_ConecttionType_Setting() == P5_CONNECTTION_TYPE.USB)
                                {
                                    this.UpdateCreditDeviceStatus("USB device added.");
                                    this.p5UsbLib = new P5LibUsbMode(this);
                                    P5LibUsbMode.ReConnectUsb();
                                }
                            }
                            else if (CreditCardLib.GET_CREDIT_DEVICE() == CREDIT_DEVICE_TYPE.CLOVER && CreditCardLib.USING_SYSTEM_CREDIT())
                            {
                                if (CloverLib.Get_ConecttionType_Setting() == CLOVER_CONNECTTION_TYPE.USB)
                                {
                                    this.UpdateCreditDeviceStatus("USB device added. Reconnect...");
                                    //ReconnectCloverConnector();
                                }
                            }

                            _usbConnecting = false; // Cho phép lần sau
                            //});
                        });
                    }
                }
                else if (wParam == DBT_DEVICEREMOVECOMPLETE) // USB bị rút ra
                {
                    if (CreditCardLib.GET_CREDIT_DEVICE() == CREDIT_DEVICE_TYPE.CODE_PAY && CreditCardLib.USING_SYSTEM_CREDIT())
                    {
                        if (P5Lib.Get_P5_ConecttionType_Setting() == P5_CONNECTTION_TYPE.USB)
                        {
                            P5LibUsbMode.CleanupStaticResources();
                            this.p5UsbLib = null;
                            //this.UpdateCreditDeviceStatus("USB device removed.");
                        }
                    }
                }
            }

            base.WndProc(ref m);
        }

        #endregion CODEPAY WEBSOCKET

        #endregion

        #region Header, Footer Menu
        private HeaderTemplateBar _header;
        private void AddHeaderTemplateBar()
        {
            // Logo Dock=Left
            PictureBox picIcon = new PictureBox
            {
                Dock = DockStyle.Left,
                Width = Constants.IS_DEMO_MODE_NOT_ANT_PAY ? 120 : 120,
                Image = Properties.Resources.ant_logo,
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            picIcon.Click += (s, e) =>
            {
                FormSetting frm = new FormSetting(this);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog(this);
                frm.Dispose();
            };
            panelLayout_Header.Controls.Add(picIcon);

            // Header bar
            _header = new HeaderTemplateBar(panelLayout_Header);
            _header.ApplySlimRightPreset();   // hoặc tự SetRightItems(...)
            _header.RightPadding = new Padding(8, 2, 12, 8); // đẩy hàng phải cao thêm 1px nữa
            _header.ForceRender();

            _header.ItemClicked += (s, e) =>
            {
                if (e.Command == HeaderCommand.Minimize) this.WindowState = FormWindowState.Minimized;
                if (e.Command == HeaderCommand.Close) this.Close();
                if (e.Command == HeaderCommand.ItemLookUp)
                {
                    FormItemLockup frm = new FormItemLockup(this, "ItemLockUp");
                    frm.StartPosition = FormStartPosition.CenterScreen;
                    frm.ShowDialog(this);
                    frm.Dispose();
                }
                if (e.Command == HeaderCommand.Reports)
                {
                    
                }
                if (e.Command == HeaderCommand.BackOffice)
                {
                    try { using (var process = new Process()) { process.StartInfo.FileName = Constants.backoffice_url; process.StartInfo.Verb = "open"; process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized; process.Start(); } } catch { }
                }
                if (e.Command == HeaderCommand.BuySupply && !Constants.IS_DEMO_MODE_NOT_ANT_PAY )
                {
                    try { using (var process = new Process()) { process.StartInfo.FileName = Constants.buy_supply_url; process.StartInfo.Verb = "open"; process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized; process.Start(); } } catch { }
                }
                if (e.Command == HeaderCommand.OffCreditDevice)
                {
                    if (Core.GET_POS_ROLE() == POS_ROLE.PRIMARY)
                    {
                        if (_header.GetItemText(HeaderCommand.OffCreditDevice).Equals("CREDIT ON"))
                        {
                            FormMessageYesNo frm = new FormMessageYesNo(this, "Are you sure you want disconnect credit card payment?", "ChangeStatusCreditDevice");
                            frm.StartPosition = FormStartPosition.CenterScreen;
                            frm.ShowDialog(this);
                            frm.Dispose();
                        }
                        else
                        {
                            ChangeStatusCreditDevice();
                        }
                    }
                }
            };

            if (CreditCardLib.USING_SYSTEM_CREDIT())
                _header.SetItemText(HeaderCommand.OffCreditDevice, "CREDIT ON");
        }

        private FooterTemplateBar _footer;
        private void AddFooterTemplateBar()
        {
            // giả sử có panelFooter (Dock=Top/Bottom tuỳ bạn)
            _footer = new FooterTemplateBar(panelLayout_Footer)
            {
                FontSize = LayoutHelper.mini_screen ? 14f : 15f,
                ButtonHeight = LayoutHelper.footer_button_height,
                DefaultBackColor = Color.FromArgb(76, 129, 187),
                DefaultBorderColor = Color.FromArgb(LayoutHelper.footer_button_height, 109, 167),
                DefaultForeColor = Color.White,
                DefaultCornerRadius = 22,
                Spacing = 4,
                SideInset = 6,        // chừa mép 2 bên
                //EvenDistribution = false,
                AllowOverflow = false
            };

            // giảm padding top/bottom của thanh
            _footer.Host.Padding = new Padding(_footer.SideInset, 0, _footer.SideInset, 0);

            // 1) Nút theo text, có thể cuộn ngang
            //_footer.WidthMode = FooterTemplateBar.ButtonWidthMode.ByText;
            //_footer.AllowOverflow = true;

            // 2) Kiểu dãn chữ
            //_footer.WidthMode = FooterTemplateBar.ButtonWidthMode.ByTextFill;
            _footer.WidthMode = FooterTemplateBar.ButtonWidthMode.Equal;

            _footer.SetItems(new[]
            {
                new MenuItemDef { Text = "DASH BOARD",  Command = MainCommand.Dashboard },

                new MenuItemDef { Text = "ADD ITEM", Command = MainCommand.ItemEntry },
                new MenuItemDef { Text = "PRODUCT SEARCH", Command = MainCommand.ItemLookUp },

                new MenuItemDef { Text = "INVENTORY",    Command = MainCommand.Inventory },
                new MenuItemDef { Text = "CUSTOMERS",    Command = MainCommand.Customer },

                new MenuItemDef { Text = "PAYMENT",     Command = MainCommand.Payment },
                
                new MenuItemDef { Text = "SALES",      Command = MainCommand.Sales },
                new MenuItemDef { Text = "OPEN SALES",   Command = MainCommand.OpenSale },
                //new MenuItemDef { Text = "CLOSE OUT",   Command = MainCommand.CloseOut },

                new MenuItemDef { Text = "CASH DRAWER", Command = MainCommand.CashDrawer },
                new MenuItemDef { Text = "BUY SUPPLY",  Command = MainCommand.BuySupply },
            });
            _footer.ItemClicked += Footer_ItemClicked;
            _footer.SelectCommand(MainCommand.Payment);
        }

        private async void Footer_ItemClicked(object sender, MenuItemClickedEventArgs e)
        {
            // Tạo & tính bounds theo client (tránh bị khung/title trừ)
            var yTop = panelLayout_Header.Bottom + 5;
            var height = this.ClientSize.Height - panelLayout_Header.Height - panelLayout_Footer.Height;
            var rect = new Rectangle(5, yTop, this.ClientSize.Width - 10, Math.Max(1, height));

            // xử lý điều hướng theo e.Command
            if (e.Command == MainCommand.Dashboard)
            {
                tabContent.SelectedPage = tabDashboard;
            }
            else if (e.Command == MainCommand.ItemEntry)
            {
                FormNewItem frm = new FormNewItem(this, "", false);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog(this);
                frm.Dispose();
            }
            else if (e.Command == MainCommand.ItemLookUp)
            {
                FormItemLockup frm = new FormItemLockup(this, "ItemLockUp");
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog(this);
                frm.Dispose();
            }
            else if (e.Command == MainCommand.Customer)
            {
                FormItemLockup frm = new FormItemLockup(this, "CustomerLockUp");
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog(this);
                frm.Dispose();
            }
            else if (e.Command == MainCommand.Inventory)
            {
                FormItemLockup frm = new FormItemLockup(this, "InventoryLookUp");
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog(this);
                frm.Dispose();
            }
            else if (e.Command == MainCommand.Payment)
            {
                tabContent.SelectedPage = tabHome;
            }
            else if (e.Command == MainCommand.Sales)
            {
                FormSaleList frm = new FormSaleList(this, "");
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog(this);
                frm.Dispose();
            }
            else if (e.Command == MainCommand.OpenSale)
            {
                FormSaleList frm = new FormSaleList(this, "0");
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog(this);
                frm.Dispose();
            }
            else if (e.Command == MainCommand.BackOffice)
            {
                try { using (var process = new Process()) { process.StartInfo.FileName = Constants.backoffice_url; process.StartInfo.Verb = "open"; process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized; process.Start(); } } catch { }
            }
            else if (e.Command == MainCommand.CashDrawer)
            {
                //FormManager frm = new FormManager(this, "0", "", "OpenCashDrawer", "", "OPEN CASH DRAWER");
                //frm.StartPosition = FormStartPosition.CenterScreen;
                //frm.ShowDialog(this);
                //frm.Dispose();

                string error = PrinterLocalHelper.OpenCashDraweFromPrinter(Constants.printer_name);
                if (error.Trim().Length > 0)
                    CustomMessageBox.Show(error);
            }
            else if (e.Command == MainCommand.BuySupply)
            {
                try { using (var process = new Process()) { process.StartInfo.FileName = Constants.buy_supply_url; process.StartInfo.Verb = "open"; process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized; process.Start(); } } catch { }
            }
        }

        public void Show_Tab_Default()
        {
            tabContent.SelectedPage = tabHome;
            _footer.SelectCommand(MainCommand.Payment);
        }

        public void Show_Paymeny_Tab()
        {
            if (tabContent.SelectedPage != tabHome)
            {
                tabContent.SelectedPage = tabHome;
                _footer.SelectCommand(MainCommand.Payment);
            }
        }

        public void ChangeStatusCreditDevice()
        {
            string current_text = _header.GetItemText(HeaderCommand.OffCreditDevice).ToUpper();
            string using_system_credit = "1";
            if (current_text.Equals("CREDIT OFF"))
                using_system_credit = "1";
            else if (current_text.Equals("CREDIT ON"))
                using_system_credit = "0";

            //Update Setting In Server => ONLY LOCAL !!!!
            string jData = "{ ";
            jData += " 'using_system_credit':'" + using_system_credit + "', ";
            jData += " } ";

            string responce = ApiHelper.CALL_API("Stores/update-setting-system-credit", jData);
            if (responce.ToUpper().StartsWith("ERROR"))
            {
                CustomMessageBox.Show("Error: " + responce);
                return;
            }

            if (current_text.Equals("CREDIT OFF"))
            {
                _header.SetItemText(HeaderCommand.OffCreditDevice, "CREDIT ON");
            }
            else if (current_text.Equals("CREDIT ON"))
            {
                _header.SetItemText(HeaderCommand.OffCreditDevice, "CREDIT OFF");
            }

            Constants.using_system_credit = using_system_credit.Equals("1") ? "ON" : "OFF";
            Models.Helper.ConfigLocalHelper.SaveConfigTxt();
        }

        public void Show_REPORTS()
        {
            
            //using (FormReports frm = new FormReports(this))
            //{
            //    frm.StartPosition = FormStartPosition.CenterScreen;
            //    frm.ShowDialog(this);
            //    frm.Dispose();
            //}
        }

        #endregion

        #region Voucher and Coupon, Backgound check promotion ...
        double total_promotion_discount = 0;
        string promotion_json = "";

        private void bgwCheckPromotion_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            //MainPOS mainPOS = new MainPOS();
            //mainPOS.Check_Promotion(myCartTouchScrollPanel, ref total_promotion_discount, ref promotion_json);
        }

        private void bgwCheckPromotion_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {

        }

        private void bgwCheckPromotion_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            //txtPromotionAmount.Text = "$" + total_promotion_discount;

            ////double totalCoupon = 0;
            ////if (txtTotalCoupon.Text.Trim().Length > 0)
            ////{
            ////    totalCoupon = double.Parse(txtTotalCoupon.Text.Trim());
            ////}

            //txtTotalCoupon.Text = "$" + (total_promotion_discount);
            //this.UpdatePaymentCartAmount(false);
        }

        private void CheckPromotion()
        {
            //MainPOS mainPOS = new MainPOS();
            //mainPOS.Check_Promotion(myCartTouchScrollPanel, ref total_promotion_discount, ref promotion_json);

            ////Nếu có promtion mới chạy cập nhật tiền !!!!
            //this.BeginInvoke(new Action(() =>
            //{
            //    txtPromotionAmount.Text = "$" + total_promotion_discount;

            //    txtTotalCoupon.Text = "$" + (total_promotion_discount);

            //    this.UpdatePaymentCartAmount(false, true);
            //}));
        }

        #region Redeem Gift Card, Reward, Voucher, Coupon
        public string coupon_code_apply = "";
        public double coupon_redeem_amount = 0;
        public string coupon_redeem_unit = "%";


        public string voucher_code_apply = "";
        public double voucher_redeem_amount = 0;
        public string voucher_redeem_unit = "%";

        public void RedeemVoucher(string voucher_code_apply, double voucher_discount, string voucher_discount_unit, double redeem_amount)
        {
            this.voucher_redeem_amount = redeem_amount;
            this.voucher_redeem_unit = voucher_discount_unit;
            this.voucher_code_apply = voucher_code_apply;
           
            if (redeem_amount > 0)
            {
                btnCart_Voucher.Title = "VOUCHER" + Environment.NewLine + "$" + redeem_amount;
            }
            else
            {
                btnCart_Voucher.Title = "VOUCHER";
            }

            UpdatePaymentCartAmount(false);

            //Update Amount
            double total_amount = Utilitys.getTotalAmount(lbCart_AmtDue.Text);
            double total_pay_amount = CartHelper.GetPaymentTotal(this.paymentList);
            UIHelper.SafeUI(lbCart_Tender, () => lbCart_Tender.Text = "$" + Math.Round(total_amount - total_pay_amount, 2).ToString());
        }

        #endregion

        #endregion End Voucher and Coupon, Backgound check promotion ...

        #region New Keyboard Enter

        public void Show_DiscountADJS(string controlId, string text)
        {
            ShowMyKeyBoard(controlId, text);
        }

        public void Show_PrePaid()
        {
            //ShowMyKeyBoard("txtTotalDeposit", Utilitys.getTotalAmount(txtTotalDeposit.Text).ToString());
        }

        private void ShowMyKeyBoard(string control_id, string control_text)
        {
            //FormKeyBoardOnly frm = new FormKeyBoardOnly(this, control_id, control_text, "local_change");
            //frm.StartPosition = FormStartPosition.CenterScreen;
            //frm.ShowDialog(this);
            //frm.Dispose();
        }

        public void ConfirmKeyboardEnter(string control_id, string value)
        {
            //if (control_id.Equals("txtDiscountPercent"))
            //{
            //    txtDiscountPercent.Text = value;
            //}
            //else if (control_id.Equals("txtDiscountFixAmount"))
            //{
            //    txtDiscountFixAmount.Text = value;
            //}
            //else if (control_id.Equals("txtCouponCode"))
            //{
            //    txtCouponCode.Text = value;
            //}
            //else if (control_id.Equals("txtTotalReedem"))
            //{
            //    txtTotalReedem.Text = value;
            //}
            //else if (control_id.Equals("txtTipAmount"))
            //{
            //    txtTipAmount.Text = value;
            //}
            //else if (control_id.Equals("txtTotalDeposit"))
            //{
            //    txtTotalDeposit.Text = "$" + value;
            //    this.SHOW_PAYMENT_PREPAID();
            //}
            //else if (control_id.Equals("search_customer"))
            //{
            //    SEARCH_CUSTOMER(value);
            //}
            //else if (control_id.Equals("search_ticket"))
            //{
            //    SEARCH_TICKET(value);
            //}
        }

        #endregion End New Keyboard Enter

        #region New Payment Function !!!!
        private PAYMENT_NOW_MODE payment_now_mode = PAYMENT_NOW_MODE.TICKET;
        private void SHOW_PAYMENT_NOW(string ticket_save_complete_id, bool is_combine)
        {
            kb.Value = "";  //Reset keyboard để nhận giá trị mới
            tabContent.SelectedPage = tabHome;

            payment_now_mode = PAYMENT_NOW_MODE.TICKET;

            
            lbCart_Deposited_Title.Visible = true;
            lbCart_Deposited.Visible = true;

            btnCart_Discount.Enabled = true; btnCart_Discount.Title = "DISCOUNT";       btnCart_Discount.TitleForeColor = Color.White;
            btnCart_Reward.Enabled = true; btnCart_Reward.Title = "REWARD";             btnCart_Reward.TitleForeColor = Color.White;
            btnCart_Method_SalonCredit.Enabled = true; btnCart_Method_SalonCredit.Title = "CREDIT";   btnCart_Method_SalonCredit.TitleForeColor = Color.White;
            btnCart_Tax.Enabled = true; btnCart_Tax.Title = "COUPON";             btnCart_Tax.TitleForeColor = Color.White;
            btnCart_Voucher.Enabled = true; btnCart_Voucher.Title = "VOUCHER";          btnCart_Voucher.TitleForeColor = Color.White;
            btnCart_Print.Enabled = true;                                               btnCart_Print.TitleForeColor = Color.White;

            btnCart_Method_Cash.Enabled = true; btnCart_Cancel.Title = "CASH";
            btnCart_Method_Cash.TitleForeColor = Color.White;
            btnCart_Cancel.Enabled = true; btnCart_Cancel.Title = "GIFT CARD";
            btnCart_Cancel.TitleForeColor = Color.White;
            

            this.selected_ticket = ticket_save_complete_id;
            if (is_combine)
            {
                this.selected_ticket_combine = true;
                this.selected_ticket_combine_id = ticket_save_complete_id;
            }

            // fire-and-forget ( xử lý vẽ giao diện không bị giật )
            //_ = Task.Run(() =>
            //{
            //    var cartData = CartHelper.GetCartDetail(ticket_save_complete_id, is_combine, ref this.paymentList);
            //    if (this.reopen_ticket_mode) {
            //        this.paymentList = new List<PaymentModel>(); //RESET !!!
            //    }

            //    double total_pay_amount = CartHelper.GetPaymentTotal(this.paymentList);
            //    double amtDue = Math.Round((Utilitys.getTotalAmount(lbCart_AmtDue.Text) - this.repair_original_amount - total_pay_amount), 2);
            //    double tender = amtDue;

            //    if (repair_ticket_mode)
            //    {
            //        UIHelper.SafeUI(lbCart_Deposited_Title, () => lbCart_Deposited_Title.Visible = false);
            //        UIHelper.SafeUI(lbCart_Deposited, () => lbCart_Deposited.Visible = false);
            //        UIHelper.SafeUI(lbCart_Repaired, () => lbCart_Repaired.Text = "$" + repair_original_amount.ToString("0.00"));
            //    }

            //    UIHelper.SafeUI(lbCart_Paided, () => lbCart_Paided.Text = "$" + total_pay_amount.ToString("0.00"));
            //    UIHelper.SafeUI(lbCart_AmtDue, () => lbCart_AmtDue.Text = "$" + amtDue);
            //    UIHelper.SafeUI(lbCart_Tender, () => lbCart_Tender.Text = tender.ToString());
            //    kb.DefaultValue = amtDue.ToString();

            //    // cập nhật UI phải marshal về UI thread
            //    panelCart_View_Content.BeginInvoke((MethodInvoker)(() =>
            //    {
            //        double total_discount = 0;
            //        CartHelper.RenderCart(panelCart_View_Content, cartData, ref total_discount);
            //        if (total_discount > 0)
            //        {
            //            btnCart_Discount.Enabled = false;
            //            btnCart_Discount.TitleForeColor = Color.Silver;
            //            btnCart_Discount.Title = "DISCOUNT" + Environment.NewLine + ("-$" + total_discount);
            //        }

            //        if (!string.IsNullOrEmpty(this.reward_balance) && double.Parse(this.reward_balance) > 0)
            //        {
            //            if (Core.USING_REWARD_PERCENT())
            //                btnCart_Reward.Title = "REWARD" + Environment.NewLine + ("BAL: " + this.reward_balance + "%");
            //            else
            //                btnCart_Reward.Title = "REWARD" + Environment.NewLine + ("BAL: $" + this.reward_balance);
            //        }

            //        if (!string.IsNullOrEmpty(this.credit_balance) && double.Parse(this.credit_balance) > 0)
            //        {
            //            btnCart_SalonCredit.Title = "CREDIT" + Environment.NewLine + ("BAL: $" + this.credit_balance);
            //        }
            //    }));
            //});
        }

        private void lbCart_Tender_TextChanged(object sender, EventArgs e)
        {
            this.TenderUpdateCartAmount();
        }

        private void TenderUpdateCartAmount()
        {
            RunOnUi(() => {
                //Cash Discount, Dual Price thay doi theo Tender
                btnCart_Method_Cash.Title = "CASH";
                btnCart_Method_Charge.Title = "CHARGE";

                double amt_due = Utilitys.getTotalAmount(lbCart_AmtDue.Text);
                double tender = Utilitys.getTotalAmount(lbCart_Tender.Text);
                double cash_payment_request = amt_due;
                //if (Core.USING_CASH_DISCOUNT() || Core.USING_CASH_DISCOUNT_PRODUCT())
                //{
                //    if (this.payment_now_mode == PAYMENT_NOW_MODE.GIFT_CARD)  //GIFTCARD không xài Cash Discount
                //    {
                //        double cash_discount = OrderHelper.Get_Cash_Discount(amt_due, tender);
                //        cash_payment_request = Math.Round(amt_due - cash_discount, 2);
                //        btnCart_Method_Cash.Title = "CASH" + Environment.NewLine + "$" + Math.Round(tender - cash_discount, 2);
                //    }
                //    else
                //    {
                //        this.cash_discount_percent = Core.CASH_DISCOUNT_PERCENT();
                //        this.cash_discount_product_percent = Core.CASH_DISCOUNT_PRODUCT_PERCENT();
                //        double total_cash_discount = 0;
                //        if (this.cash_discount_percent > 0)
                //        {
                //            double total_service_amount = OrderHelper.Get_Service_Amount(myCartTouchScrollPanel.Content, amt_due, tender);
                //            double cash_discount_service = OrderHelper.Get_Cash_Discount_Service(total_service_amount, amt_due, tender);
                //            total_cash_discount += cash_discount_service;
                //        }
                //        if (this.cash_discount_product_percent > 0)
                //        {
                //            double total_service_product_amount = OrderHelper.Get_Product_Amount(myCartTouchScrollPanel.Content, amt_due, tender);
                //            double cash_product_discount = OrderHelper.Get_Cash_Discount_Product(total_service_product_amount, amt_due, tender);
                //            total_cash_discount += cash_product_discount;
                //        }

                //        if (cash_payment_request > 0)
                //        {
                //            cash_payment_request = Math.Round(amt_due - total_cash_discount, 2);
                //            btnCart_Method_Cash.Title = "CASH" + Environment.NewLine + "$" + Math.Round(tender - total_cash_discount, 2);
                //        }
                //    }
                //}

                //if (Core.USING_DUAL_PRICE())
                //{
                //    this.dual_price_amount = Utilitys.getDualPrice(tender, 0);
                //    if (this.dual_price_amount > 0)
                //        btnCart_Method_Charge.Title = "CHARGE" + Environment.NewLine + "$" + Math.Round(tender + this.dual_price_amount, 2);
                //}
                //else
                //{
                //    this.surcharge_amount = Utilitys.getSurcharge(tender, 0);
                //    if (this.surcharge_amount > 0)
                //        btnCart_Method_Charge.Title = "CHARGE" + Environment.NewLine + "$" + Math.Round(tender + this.dual_price_amount, 2);
                //}

                //Change Due
                if (tender > cash_payment_request)
                {
                    lbCart_Change.Text = "$" + Math.Round(tender - cash_payment_request, 2);
                }
                else
                {
                    lbCart_Change.Text = "$0.00";
                }

            });
        }

        public void HIDE_PAYMENT_NOW()
        {
            this.CartEnableControl();
            //panelCart_View_Content.Controls.Clear();

            tabContent.SelectedPage = tabHome;
            _footer.SelectCommand(MainCommand.Payment);

        }

        public void ConfirmOptionKeyboardEnter(string control_id, string value, string unit, string discount_percent_owner = "0")
        {
            //if (control_id.Equals("btnCart_Discount"))
            //{
            //    txtDiscountPercent.Text = "";
            //    txtDiscountFixAmount.Text = "";
            //    txtDiscountPercentOwner.Text = string.IsNullOrEmpty(discount_percent_owner) ? "100" : discount_percent_owner;

            //    if (unit.Equals("%"))
            //        txtDiscountPercent.Text = value;
            //    else
            //        txtDiscountFixAmount.Text = value;

            //    Update_ItemLine_Discount(txtDiscountPercent.Text, txtDiscountFixAmount.Text);

            //    //Update Amount
            //    double total_amount = Utilitys.getTotalAmount(lbCart_AmtDue.Text);
            //    double total_pay_amount = CartHelper.GetPaymentTotal(this.paymentList);
            //    UIHelper.SafeUI(lbCart_Tender, () => lbCart_Tender.Text = Math.Round(total_amount - total_pay_amount, 2).ToString());

            //    //Discount 100%
            //    if (unit.Equals("%") && double.Parse(value) >= 100)
            //    {
            //        this.paymentList.Add(new PaymentModel("DISCOUNT", this.GetSubTotal()));
            //        this.POS_CHECKOUT("0", this.paymentList, total_pay_amount);
            //    }
            //    else if (Math.Round(total_amount - total_pay_amount, 2) <= 0) //Neu Tender <= 0 => check-out luon giong nhu bam cac nut payment
            //    {
            //        Payment_New_Process("DISCOUNT", "");
            //    }
            //}
        }

        
        private void btnCart_Cancel_Click(object sender, EventArgs e)
        {
            this.HIDE_PAYMENT_NOW();
            this.ResetAllData();
        }

        private void btnCart_Save_Click(object sender, EventArgs e)
        {
            CartDisableControl();

            bool is_combine_ticket = false; // MainCart.checkIsCombine(myCartTouchScrollPanel);
            //CartHelper.SaveCartPayment(this.selected_ticket, is_combine_ticket, paymentList, true);
            SaveCartPayment();

            this.ResetAllData();
            this.EnableDisableControl(true);
        }

        private void btnCart_Method_Cash_Click(object sender, EventArgs e)
        {
            if (Constants.pincode_checkout_cash.Equals("1"))
            {
                FormManager frm = new FormManager(this, "", "", "CheckOut", "{'payment_type':'CASH'}", "CASH PAYMENT");
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog(this);
                frm.Dispose();
            }
            else
            {
                Payment_New_Process("CASH", "");
            }
        }

        private void btnCart_Method_CashApp_Click(object sender, EventArgs e)
        {
            if (Constants.pincode_checkout_cashApp.Equals("1"))
            {
                FormManager frm = new FormManager(this, "", "", "CheckOut", "{'payment_type':'CASH APP'}", "CASH APP PAYMENT");
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog(this);
                frm.Dispose();
            }
            else
            {
                Payment_New_Process("CASH APP", "");
            }
        }

        private void btnCart_Method_Member_Click(object sender, EventArgs e)
        {
            if (Constants.pincode_checkout_member.Equals("1"))
            {
                FormManager frm = new FormManager(this, "", "", "CheckOut", "{'payment_type':'MEMBER'}", "MEMBER PAYMENT");
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog(this);
                frm.Dispose();
            }
            else
            {
                Payment_New_Process("MEMBER", "");
            }
        }

        private void btnCart_Method_Prepaid_Click(object sender, EventArgs e)
        {
            if (Constants.pincode_checkout_prepaid.Equals("1"))
            {
                FormManager frm = new FormManager(this, "", "", "CheckOut", "{'payment_type':'PREPAID'}", "PREPAID PAYMENT");
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog(this);
                frm.Dispose();
            }
            else
            {
                Payment_New_Process("PREPAID", "");
            }        
        }

        private void btnCart_Method_Charge_Click(object sender, EventArgs e)
        {
            Payment_New_Process("CHARGE", "");
        }

        private void btnCart_Tax_Click(object sender, EventArgs e)
        {
            FormTaxRedeem frm = new FormTaxRedeem(this, tax_percent, tax_include);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog(this);
            frm.Dispose();
        }

        private void btnCart_Discount_Click(object sender, EventArgs e)
        {
            FormDiscountRedeem frm = new FormDiscountRedeem(this, this.discount_value, this.discount_unit);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog(this);
            frm.Dispose();
        }

        private void btnCart_Reward_Click(object sender, EventArgs e)
        {

        }

        private void btnCart_Method_SalonCredit_Click(object sender, EventArgs e)
        {
            Payment_New_Process("SALON CREDIT", "");
        }

        private void btnCart_Coupon_Click(object sender, EventArgs e)
        {

        }

        private void btnCart_Voucher_Click(object sender, EventArgs e)
        {

        }

        private void btnCart_Method_Giftcard_Click(object sender, EventArgs e)
        {

        }

        public void Payment_New_Process(string paid_by, string pincode)
        {
            if (!string.IsNullOrEmpty(pincode))
                this.pincode_payment = pincode;
            if (this.paymentList == null)
                this.paymentList = new List<PaymentModel>();

            double total_amount = Utilitys.getTotalAmount(lbCart_Total.Text);
            double pay_amount = Utilitys.getTotalAmount(lbCart_Tender.Text);  //Tender = Amount Receiver
            double paid_amount = Utilitys.getTotalAmount(lbCart_Paided.Text);
            double cash_discount = 0;

            if (pay_amount <= 0 && !paid_by.Equals("CASH") && !paid_by.Equals("GIFT CARD") && !paid_by.Equals("REWARD") && !paid_by.Equals("SALON CREDIT") && !paid_by.Equals("DISCOUNT") )
            {
                CustomMessageBox.Show("Please Check Payment Amount");
                return;
            }
            else if (paid_by.Equals("GIFT CARD"))
            {
                pay_amount = 0;
            }
            else if (paid_by.Equals("REWARD"))
            {
                pay_amount = 0;
            }
            else if (paid_by.Equals("SALON CREDIT"))
            {
                pay_amount = this.salon_credit_redeem;
            }
            else if (paid_by.Equals("DISCOUNT"))
            {
                pay_amount = 0;
            }
            else if (paid_by.Equals("CASH"))
            {
                double amtDue = Utilitys.getTotalAmount(lbCart_AmtDue.Text);
                double tender = Utilitys.getTotalAmount(lbCart_Tender.Text);
                //if (Core.USING_CASH_DISCOUNT())
                //{
                //    double total_service_amount = OrderHelper.Get_Service_Amount(myCartTouchScrollPanel.Content, amtDue, tender);

                //    this.cash_discount_percent = Core.CASH_DISCOUNT_PERCENT();
                //    cash_discount_service = OrderHelper.Get_Cash_Discount_Service(total_service_amount, amtDue, tender);
                //}
               
                //Nếu payment Cash thì phải lấy amount gốc để còn tính amount recived
                if (tender < amtDue)
                    pay_amount = tender;
                else
                    pay_amount = amtDue;

                cash_discount = 0;
                pay_amount -= cash_discount;
            }

            if ((paid_by.Equals("CHARGE") || paid_by.Equals("CASH APP") || paid_by.Equals("MEMBER") || paid_by.Equals("PREPAID")))
            {
                double amt_due = Utilitys.getTotalAmount(lbCart_AmtDue.Text);
                if (pay_amount > amt_due)
                {
                    CustomMessageBox.Show("The amount you entered is higher than the amount due.");
                    return;
                }
            }

            pay_amount = Math.Round(pay_amount, 2);  //1 số case discount, tax lẻ
            total_amount = Math.Round(total_amount, 2);

            //Double Click => SAI LOGIC neu payment credit check bi loi nao do
            //CartDisableControl();

            if (paid_by.Equals("CHARGE"))
            {
                //Nếu cho TIP từ POS thì phải trừ lại, tip thu riêng
                double total_pos_tip = 0;
                SHOW_PAYMENT_CREDIT("", pay_amount - total_pos_tip, total_pos_tip);
                return;
            }

            //Double Click
            CartDisableControl();

            PaymentModel model = new PaymentModel(paid_by, pay_amount, cash_discount, 0, Utilitys.getTotalAmount(lbCart_Tender.Text), Utilitys.getTotalAmount(lbCart_AmtDue.Text), 0, pincode);
            this.paymentList.Add(model);

            double total_pay_amount = CartHelper.GetPaymentTotal(this.paymentList);
            double total_cash_discount = 0; // CartHelper.GetPaymentTotal_CashDiscount(this.paymentList);

            if ((Math.Round(total_pay_amount + total_cash_discount, 2) + 0.5) >= total_amount) // >= số tiền cần thanh toán thì tự complete ticket ( lẻ ít cũng tính là DONE )
            {
                this.POS_CHECKOUT("0", this.paymentList, pay_amount);
            }
            else
            {
                SaveCartPayment();
            }
        }

        private void SaveCartPayment()
        {
            RunOnUi(() =>
            {
                double total_amount = Utilitys.getTotalAmount(lbCart_Total.Text);
                double total_paid = CartHelper.GetPaymentTotal(this.paymentList);
                double tender = Math.Round((total_amount - total_paid), 2);

                lbCart_Paided.Text = "$" + total_paid;
                lbCart_AmtDue.Text = "$" + tender;
                lbCart_Tender.Text = "$" + tender;
                if (kb != null) kb.Value = ""; //Reset Keyboard (kb chỉ tạo sau MainForm_Shown)
            }
            );

            //Chạy ngầm save trên server nếu chưa complete
            _ = Task.Run(() =>
            {
                //bool is_combine_ticket = false; // MainCart.checkIsCombine(myCartTouchScrollPanel.Content);
                //CartHelper.SaveCartPayment(this.selected_ticket, is_combine_ticket, paymentList, false);

                this.POS_CHECKOUT(this.selected_ticket, this.paymentList, 0, "1");
                this.CartEnableControl();
            });
        }

        private void btnPaymentCard_Cancel_Click(object sender, EventArgs e)
        {
            this.ResetAllData();
        }

        public void CartEnableControl()
        {
            RunOnUi(() =>
                {
                    btnCart_Method_Cash.Enabled = true;
                    btnCart_Method_Charge.Enabled = true;
                    btnCart_Method_CashApp.Enabled = false;
                    btnCart_Method_Member.Enabled = false;
                    btnCart_Method_Prepaid.Enabled = false;
                    btnCart_Method_SalonCredit.Enabled = true;

                    btnCart_Cancel.Enabled = true;
                    btnCart_Save.Enabled = true;

                    btnCart_Tax.Enabled = true;
                    btnCart_Discount.Enabled = true;
                    btnCart_Print.Enabled = true;
                }
            );
        }
        public void CartDisableControl()
        {
            RunOnUi(() =>
            {
                btnCart_Method_Cash.Enabled = false;
                btnCart_Method_Charge.Enabled = false;
                btnCart_Method_CashApp.Enabled = false;
                btnCart_Method_Member.Enabled = false;
                btnCart_Method_Prepaid.Enabled = false;
                btnCart_Method_SalonCredit.Enabled = false;

                btnCart_Cancel.Enabled = false;
                btnCart_Save.Enabled = false;

                btnCart_Tax.Enabled = false;
                btnCart_Discount.Enabled = false;
                btnCart_Print.Enabled = false;
            }
            );
        }

        private void lbCart_CustomerName_Click(object sender, EventArgs e)
        {
            FormKeyboardOnlyNumber frm = new FormKeyboardOnlyNumber(this, "", "txtSearchCustomerPhone", "SearchCustomerPhone");
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog(this);
            frm.Dispose();
        }

        private void lbCart_Search_Click(object sender, EventArgs e)
        {
            FormKeyboardOnlyNumber frm = new FormKeyboardOnlyNumber(this, "", "txtSearchCustomerPhone", "SearchCustomerPhone");
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog(this);
            frm.Dispose();
        }

        private void lbAddQuickItem_Click(object sender, EventArgs e)
        {
            FormAddQuickItem frm = new FormAddQuickItem(this);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog(this);
            frm.Dispose();
        }

        #endregion New Function !!!!


        #region Xử lý layout load không giật: không xài nền trong suốt, vẽ boder....

        private void AddLoading()
        {
            picLoading = new PictureBox
            {
                Image = Resources.ant_load,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = Constants.IS_DEMO_MODE_NOT_ANT_PAY ? new Size(500, 200) : new Size(200, 200),
                BackColor = Color.Transparent,
                //Location = new Point((this.Width - 200) / 2, (this.Height - 200) / 2),
                Visible = false,
                Anchor = AnchorStyles.None
            };
            this.Controls.Add(picLoading);

            // căn giữa lần đầu (sau khi add)
            CenterLoading();

            // luôn căn giữa khi thay đổi kích thước
            this.Resize -= Parent_Resize;
            this.Resize += Parent_Resize;
        }

        private void Parent_Resize(object sender, EventArgs e) => CenterLoading();

        private void CenterLoading()
        {
            if (picLoading == null || picLoading.IsDisposed) return;
            // DisplayRectangle đã trừ Padding/viền
            var rc = this.DisplayRectangle;
            picLoading.Location = new Point(
                rc.Left + (rc.Width - picLoading.Width) / 2,
                rc.Top + (rc.Height - picLoading.Height) / 2
            );
            picLoading.BringToFront();
        }

        const int WM_SETREDRAW = 0x000B;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        void ShowOverlay(Control overlay)
        {
            // không làm gì nặng ở đây — chỉ hiển/ẩn
            overlay.SuspendLayout();
            SendMessage(overlay.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);

            overlay.Visible = true;     // hoặc BringToFront()
            overlay.BringToFront();

            SendMessage(overlay.Handle, WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
            overlay.ResumeLayout(true);
        }

        void HideOverlay(Control overlay)
        {
            overlay.Visible = false;
        }

        protected override CreateParams CreateParams
        {
            get { var cp = base.CreateParams; cp.ExStyle |= 0x02000000; return cp; }
        }

        void SuspendRedraw(Control c)
        {
            if (c?.IsHandleCreated == true) SendMessage(c.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
        }
        void ResumeRedraw(Control c)
        {
            if (c?.IsHandleCreated == true)
            {
                SendMessage(c.Handle, WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
                c.Invalidate(true);
                c.Update();
            }
        }

        bool _wasMin = false;
        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                _wasMin = true;

                // KHÔNG gọi WM_SETREDRAW trên Form
                SuspendLayout();
                // chỉ tắt redraw cho panel nặng (nếu muốn)
                SuspendRedraw(panelBackground);
                SuspendRedraw(panelLayout_Right);
                SuspendRedraw(panelLayout_Left);
            }
            else if (_wasMin)
            {
                _wasMin = false;

                // bật lại redraw cho panel
                ResumeRedraw(panelBackground);
                ResumeRedraw(panelLayout_Right);
                ResumeRedraw(panelLayout_Left);

                ResumeLayout(true);

                // cập nhật lại ảnh nền đã scale (nếu cần)
                //BeginInvoke((Action)ApplyBackgrounds);
            }
        }

        private static Image _backgroundImage;
        private static Image _backgroundImageBorder1200x1600;
        private static Image _backgroundImageBorderSmall;
        // An toàn chạy trên background thread: chỉ cache ảnh, không đụng control
        private void PreloadBackgroundImages()
        {
            if (_backgroundImage == null)
                _backgroundImage = Properties.Resources.BG_Light;
            if (_backgroundImageBorder1200x1600 == null)
                _backgroundImageBorder1200x1600 = Properties.Resources.border_1200x1600_1px;
            if (_backgroundImageBorderSmall == null)
                _backgroundImageBorderSmall = Properties.Resources.border_small_control;
        }

        // Phải chạy trên UI thread: set màu/border cho control
        private void ApplyPanelTheme()
        {
            panelBackground.BackColor = ColorHelper.DefaultBackgoundColor;

            panelLayout_Left.BackColor = ColorTranslator.FromHtml("#E1F0FB");

            panelLayout_Right.BorderColor = ColorHelper.DefaultBorderColor;
            panelLayout_Left.BorderColor = ColorHelper.DefaultBorderColor;

            ApplyColorTheme();
        }

        private void ApplyColorTheme()
        {
            // === + QUICK ITEM ===
            lbAddQuickItem.ForeColor = Color.FromArgb(0, 131, 143);   // Cyan 800 — nổi, rõ, không quá chói

            // === Payment buttons (left column) ===
            var payBlue   = Color.FromArgb(21,  101, 192);   // Blue 800   — CASH / CHARGE
            var payPurple = Color.FromArgb(81,  45,  168);   // Deep Purple 800 — MEMBER
            var payGreen  = Color.FromArgb(27,  94,  32);    // Green 900  — PREPAID
            var payTeal   = Color.FromArgb(0,   105, 92);    // Teal 800   — SALON CREDIT
            var payCash   = Color.FromArgb(0,   150, 136);   // Teal 600   — CASH APP

            btnCart_Method_Cash.TitleBackColor         = payBlue;   btnCart_Method_Cash.BorderColor         = payBlue;
            btnCart_Method_Charge.TitleBackColor       = payBlue;   btnCart_Method_Charge.BorderColor       = payBlue;
            btnCart_Method_CashApp.TitleBackColor      = payCash;   btnCart_Method_CashApp.BorderColor      = payCash;
            btnCart_Method_Member.TitleBackColor       = payPurple; btnCart_Method_Member.BorderColor       = payPurple;
            btnCart_Method_Prepaid.TitleBackColor      = payGreen;  btnCart_Method_Prepaid.BorderColor      = payGreen;
            btnCart_Method_SalonCredit.TitleBackColor  = payTeal;   btnCart_Method_SalonCredit.BorderColor  = payTeal;

            // === PRINT button ===
            btnCart_Print.TitleBackColor = Color.FromArgb(27, 94, 32);   // Green 900 — thay LimeGreen
            btnCart_Print.BorderColor    = Color.FromArgb(27, 94, 32);
        }


        #endregion Xử lý layout load không giật: không xài nền trong suốt, vẽ boder....


        #region Xử lý Purge Print

        /// <summary>
        /// Dọn dẹp máy in khi app đóng.
        /// Tách ra method riêng để crash handler cũng có thể gọi.
        /// </summary>
        private static void CleanupPrinterOnExit(CloseReason reason = CloseReason.ApplicationExitCall)
        {
            try
            {
                string printerName = Constants.printer_name;

                // BƯỚC 1: Xóa print job còn tồn đọng theo đúng printer_name đã cấu hình
                // Không dùng SafePurgePOSQueuesOnExit() vì nó filter hardcode "POS"
                if (!string.IsNullOrWhiteSpace(printerName))
                {
                    // WMI delete nhanh, không restart Spooler — phù hợp cho normal exit
                    PrinterHelper.PurgeAllPrintJobs_WMI(retries: 2, delayMs: 300);
                }
                else
                {
                    // Fallback: không có printer_name thì dùng hàm cũ filter "POS"
                    PrinterHelper.SafePurgePOSQueuesOnExit();
                }
            }
            catch { /* best-effort, không chặn đóng app */ }

            try
            {
                // BƯỚC 2: Gửi ESC @ để reset buffer nội bộ của POS-80C
                // Quan trọng nhất: nếu session này có lệnh in dở, ESC @ xóa sạch
                // trước khi PC tắt, tránh máy in in rác lúc khởi động lại
                if (!string.IsNullOrWhiteSpace(Constants.printer_name))
                {
                    PrinterHelper.ResetPrinterEscPos(Constants.printer_name);
                }
            }
            catch { /* best-effort */ }

            try
            {
                // BƯỚC 3: Chỉ khi Windows đang tắt máy — dừng hẳn Spooler
                // (nặng hơn, cần ~5-10s nhưng đảm bảo spool file bị xóa trước khi OS shutdown)
                if (reason == CloseReason.WindowsShutDown)
                {
                    PrinterHelper.HardPurgeSpooler_Silent();
                }
            }
            catch { /* best-effort */ }
        }

        #endregion


        #region Process Barcode

        private System.Windows.Forms.Timer timerBarcodeReset;
        private readonly StringBuilder typingBuffer = new StringBuilder();
        private readonly StringBuilder scanBuffer = new StringBuilder();
        public string current_text = "";

        private DateTime scanFirstCharTime = DateTime.MinValue;
        private DateTime scanLastCharTime = DateTime.MinValue;

        private const int ScanIdleTimeoutMs = 80;     // scanner ngưng 80ms coi như kết thúc
        private const int MaxScanCharGapMs = 30;      // tốc độ rất nhanh mới coi là scan
        private const int MinScanLength = 6;          // barcode tối thiểu
        private bool _enterHandledByCommandKey = false;

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!CanHandleKeyboard())
                return;

            // Không xử lý Enter trong KeyPress, control char
            if (e.KeyChar == (char)Keys.Enter || e.KeyChar == '\n')
            {
                e.Handled = true;
                return;
            }

            if (char.IsControl(e.KeyChar))
                return;

            char ch = e.KeyChar;

            // Chỉ gom vào buffer tạm để phân loại scan hay typing
            if (scanBuffer.Length == 0)
                scanFirstCharTime = DateTime.Now;

            scanBuffer.Append(ch);
            scanLastCharTime = DateTime.Now;

            timerBarcodeReset.Stop();
            timerBarcodeReset.Interval = ScanIdleTimeoutMs;
            timerBarcodeReset.Start();
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            Debug.WriteLine("ProcessCmdKey: " + keyData);
            Debug.WriteLine("ActiveControl: " + (this.ActiveControl == null ? "null" : this.ActiveControl.Name));

            if (!CanHandleKeyboard())
                return base.ProcessCmdKey(ref msg, keyData);

            if (keyData == Keys.Enter)
            {
                _enterHandledByCommandKey = true;

                string value = scanBuffer.ToString().Trim();
                scanBuffer.Clear();

                string valueTyping = typingBuffer.ToString().Trim();
                typingBuffer.Clear();

                if (!string.IsNullOrWhiteSpace(value) || !string.IsNullOrWhiteSpace(valueTyping))
                {
                    this.SearchItemsByBarcodeOrSKU(value, true, false);
                }

                return true;
            }

            if (keyData == Keys.Back)
            {
                ResetScanBuffer();

                if (typingBuffer.Length > 0)
                {
                    typingBuffer.Remove(typingBuffer.Length - 1, 1);
                }

                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }


        private void ProcessScanBuffer()
        {
            string scannedValue = scanBuffer.ToString().Trim();
            DateTime first = scanFirstCharTime;
            DateTime last = scanLastCharTime;

            ResetScanBuffer();

            if (IsLikelyScan(scannedValue, first, last))
            {
                // Khi có scan thật, ưu tiên giá trị scan
                this.SearchItemsByBarcodeOrSKU(scannedValue, true, false);
            }
        }

        private void ProcessTypingBuffer()
        {
            string typedValue = typingBuffer.ToString().Trim();
            typingBuffer.Clear();

            if (!string.IsNullOrWhiteSpace(typedValue))
            {
                timerBarcodeReset.Stop();
                ResetScanBuffer();
                typingBuffer.Clear();

                //FormItemLockup frm = new FormItemLockup(this, "ItemLockUp", typedValue);
                //frm.StartPosition = FormStartPosition.CenterScreen;
                //frm.ShowDialog(this);
                //frm.Dispose();

                using (var frm = new FormItemLockup(this, "ItemLockUp", typedValue))
                {
                    frm.ShowDialog(this);
                }

                this.Activate();
                this.Focus();

            }
        }

        private void ProcessTypingChar(char ch, string currentText)
        {
            FormItemLockup frm = new FormItemLockup(this, "ItemLockUp", currentText);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog(this);
            frm.Dispose();
        }

        private void ProcessTypingText(string currentText)
        {
            if (string.IsNullOrWhiteSpace(currentText))
                return;

            FormItemLockup frm = new FormItemLockup(this, "ItemLockUp", currentText);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog(this);
            frm.Dispose();
        }

        private void ResetScanBuffer()
        {
            scanBuffer.Clear();
            scanFirstCharTime = DateTime.MinValue;
            scanLastCharTime = DateTime.MinValue;
        }

        private bool IsLikelyScan(string text, DateTime firstTime, DateTime lastTime)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            text = text.Trim();

            if (text.Length < MinScanLength)
                return false;

            if (firstTime == DateTime.MinValue || lastTime == DateTime.MinValue)
                return false;

            double totalMs = (lastTime - firstTime).TotalMilliseconds;
            if (totalMs < 0)
                return false;

            if (text.Length <= 1)
                return false;

            double avgGap = totalMs / (text.Length - 1);

            return avgGap <= MaxScanCharGapMs;
        }

        private void TimerBarcodeReset_Tick(object sender, EventArgs e)
        {
            timerBarcodeReset.Stop();

            if (Form.ActiveForm != this)
            {
                ResetScanBuffer();
                return;
            }

            if (scanBuffer.Length == 0)
                return;

            if ((DateTime.Now - scanLastCharTime).TotalMilliseconds < ScanIdleTimeoutMs)
            {
                timerBarcodeReset.Start();
                return;
            }

            string inputText = scanBuffer.ToString().Trim();
            bool isScan = IsLikelyScan(inputText, scanFirstCharTime, scanLastCharTime);

            if (isScan)
            {
                // Là barcode scan -> xử lý ngay
                this.SearchItemsByBarcodeOrSKU(inputText, true, false);

                // Scan thì tuyệt đối không nhập vào typingBuffer
            }
            else
            {
                // Không phải scan -> coi là người dùng gõ tay
                typingBuffer.Append(inputText);

                ProcessTypingBuffer();
            }

            ResetScanBuffer();
        }

        private bool CanHandleKeyboard()
        {
            return this.Visible
                    && !this.IsDisposed
                    && this.IsHandleCreated
                    && this.WindowState != FormWindowState.Minimized;
        }

        #endregion Process Barcode


        #region NEW RETAIL FUNCTION CLONE

        public string paidAmount = "0";
        public string orderSource = "";
        public void AddSaleItemToCard(string orderId, string responce, string paidAmount, string orderSource)
        {
            this.paidAmount = paidAmount; lbCart_Paided.Text = "$" + paidAmount; 
            this.orderSource = orderSource;

            JArray jArray = JArray.Parse(responce);
            if (jArray.Count <= 0) return;

            this.curent_order_payment_id = orderId;
            this.curent_order_local_payment_id = orderId;

            // Dispose controls cũ trước khi clear
            var content = panelCartItemsTouch.Content;
            var oldControls = content.Controls.OfType<UCCartItem>().ToList();
            content.Controls.Clear();
            foreach (var old in oldControls) { old.MyDispose(); old.Dispose(); }  //MyDispose không release window handle

            // Tạo tất cả controls + tính vị trí trước, không add trong vòng lặp
            int itemWidth = panelCartItemsTouch.Width - 5;
            int locationY = 5;
            var controls = new UCCartItem[jArray.Count];
            for (int i = 0; i < jArray.Count; i++)
            {
                var obj = (JObject)jArray[i];
                var cardItem = new UCCartItem(this,
                    obj["itemId"].ToString(), obj["itemName"].ToString(),
                    obj["price"].ToString(), obj["qty"].ToString(),
                    "0", "0", "");
                cardItem.cart_order_id = obj["orderId"].ToString();
                cardItem.Width = itemWidth;
                cardItem.Location = new Point(5, locationY);
                locationY += cardItem.Height + 5;
                controls[i] = cardItem;
            }

            // Add tất cả 1 lần — 1 layout pass
            content.SuspendLayout();
            try   { content.Controls.AddRange(controls); }
            finally { content.ResumeLayout(); }

            this.UpdatePaymentCartAmount();
            SyncStartScanPanel();
        }

        public void AddCustomerToCard(string responce)
        {
            JArray jArray = JArray.Parse(responce);

            if (jArray.Count <= 0)
            {
                return;
            }

            if (jArray.Count >= 1)  //Show select
            {
                FormConfirmSelectCustomerResult frm = new FormConfirmSelectCustomerResult(this, jArray);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog(this);
                frm.Dispose();
            }
        }

        public void SetCustomerInfo(string id, string phone, string firstname, string lastname)
        {
            try
            {
                //Update Customer Info
                customer_selected = id;
                customer_name = firstname + " " + lastname;
                lbCart_CustomerName.Text = "CUSTOMER: " + customer_name;
                svgCart_RemoveCustomer.Visible = true;

                //Get Reward
                var reward = Utilitys.CALL_API("Customer/getRewardBalance?customer_id=" + id, "", "GET", true);

                this.reward_balance = reward.ToString();
                //lbCart_RewardBalance.Text = "Reward Balance: $" + this.reward_balance;
            }
            catch { }
        }

        public void SearchCustomerByPhone(string responce, string phone)
        {
            if (string.IsNullOrEmpty(responce))
            {
                lbCart_CustomerName.Text = "CUSTOMER: Phone Not Found !";
                this.reward_balance = "0";

                //Show Add New
                FormAddNewCustomer frm = new FormAddNewCustomer(this, phone);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog(this);
                frm.Dispose();

                return;
            }

            this.UpdateCustomers(responce);
            
        }

        public void UpdateCustomers(string responce)
        {
            JObject jData = JObject.Parse(responce);
            customer_selected = jData["id"].ToString();
            customer_name = jData["firstName"].ToString() + " " + jData["lastName"].ToString();

            lbCart_CustomerName.Text = "CUSTOMER: " + customer_name.ToUpper();
            svgCart_RemoveCustomer.Visible = true;

            //Get Reward
            string customer_id = Newtonsoft.Json.Linq.JObject.Parse(responce)["id"].ToString(); ;
            var reward = Utilitys.CALL_API("Customer/getRewardBalance?customer_id=" + customer_id, "", "GET", true);

            this.reward_balance = reward.ToString();
        }

        public void ResetCustomers(bool send_signal = false)
        {
            customer_selected = "";
            customer_name = "GUEST";
            lbCart_CustomerName.Text = "CUSTOMER: " + customer_name.ToUpper();
            svgCart_RemoveCustomer.Visible = false;
            this.reward_balance = "0";

            if (send_signal)
                CartHelper.RemoveCustomerInfoSignalR();
        }


        bool is_search_sku = false;
        public void SearchItemsByBarcodeOrSKU(string search, bool is_search_barcode, bool is_search_sku)
        {
            string responce = null;
            this.is_search_sku = is_search_sku;

            if (is_search_barcode || is_search_sku)
            {
                if (is_search_barcode)
                    responce = Utilitys.CALL_API("Product/search-barcode?barcode=" + Regex.Replace(search, "\r", ""), "", "GET", true);
                else
                    responce = Utilitys.CALL_API("Product/search-sku?barcode=" + Regex.Replace(search, "\r", ""), "", "GET", true);
            }
            else
            {
                //Search Item
                string DATA = @"{
                                'category_id':null, 
                                'category_slugname':null, 
                                'searchString':'" + Regex.Replace(search, "\r", "") + @"',
                                'pageIndex':0, 
                                'pageSize':50
                            }";

                responce = Utilitys.CALL_API("Product/search", DATA, "POST", true);
            }

            if (responce.StartsWith("Error"))
            {
                CustomMessageBox.Show(responce);
                return;
            }

            this.AddScanItemToCard(responce, Regex.Replace(search, "\r", ""), is_search_barcode);
            this.current_text = "";
        }

        public void AddScanItemToCard(string responce, string barcode, bool is_search_barcode)
        {
            JArray jArray = JArray.Parse(responce);
            if (jArray.Count <= 0)
            {
                FormSearchItemResult frm = new FormSearchItemResult(this, barcode, !is_search_barcode);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog(this);
                frm.Dispose();
                return;
            }

            if (jArray.Count == 1)  //Add to card, multi to select
            {
                //Right
                this.ResetDefaultFocus();

                foreach (JObject obj in jArray)
                {
                    string item_id = obj["id"].ToString();
                    string item_name = obj["name"].ToString();
                    string price = obj["price"].ToString();
                    string qty = "1";

                    //Bước này check có Promotion hay không, nếu có add thêm line Promotion, không add line Cart
                    responce = Check_Promotion(item_id, item_name, price, qty, true);
                    if (Utilitys.IsValidJson(responce) && !responce.Equals("[]"))
                    {
                        return;
                    }

                    this.AddItemToCard(item_id, item_name, price);
                }
            }
            else if (jArray.Count >= 2)  //Show select
            {
                //int locationY = 5;
                //foreach (JObject obj in jArray)
                //{
                //    string item_id = obj["id"].ToString();
                //    string item_name = obj["name"].ToString();
                //    string price = obj["price"].ToString();

                //    UCCartItemMini cardItem = new NailsChekin.UserControl.UCCartItemMini(item_id, item_name, price);
                //    cardItem.Width = myTouchScrollPanel.Width;
                //    cardItem.Location = new Point(0, locationY); locationY += (cardItem.Height + 5);
                //    myTouchScrollPanel.Content.Controls.Add(cardItem);
                //}

                FormConfirmSelectItemResult frm = new FormConfirmSelectItemResult(this, jArray);
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog(this);
                frm.Dispose();
            }
        }

        public void AddItemToCard(string item_id, string item_name, string price)
        {
            bool exitst = false;
            foreach (UCCartItem control in panelCartItemsTouch.Content.Controls.OfType<UCCartItem>())
            {
                if (control.item_id.Equals(item_id) && control.isPromotion.Equals("0"))
                {
                    control.increaseQuantity(1);
                    exitst = true;
                }
            }

            if (!exitst)
            {
                var _content = panelCartItemsTouch.Content;
                UCCartItem cardItem = new NailsChekin.UserControl.UCCartItem(this, item_id, item_name, price);
                cardItem.Width = panelCartItemsTouch.Width - 5;
                int shift = cardItem.Height + 5;
                _content.SuspendLayout();
                try
                {
                    foreach (Control ctrl in _content.Controls)
                        ctrl.Location = new Point(ctrl.Location.X, ctrl.Location.Y + shift);
                    cardItem.Location = new Point(5, 5);
                    _content.Controls.Add(cardItem);
                }
                finally { _content.ResumeLayout(); }
            }

            this.UpdatePaymentCartAmount(false);
            SyncStartScanPanel();
        }

        public void AddQuickPaymentItems(List<CartItemModel> cartItems)
        {
            if (cartItems.Count <= 0)
            {
                CustomMessageBox.Show("Erorr: Please check cart item");
                return;
            }

            var content = panelCartItemsTouch.Content;
            content.SuspendLayout();
            try
            {
                // Tạo tất cả controls trước để lấy height
                var newControls = new List<UCCartItem>(cartItems.Count);
                foreach (var ci in cartItems)
                {
                    var card = new NailsChekin.UserControl.UCCartItem(this, ci.item_id, ci.item_name, ci.price, ci.quantity);
                    card.Width = panelCartItemsTouch.Width - 5;
                    newControls.Add(card);
                }

                int itemHeight = newControls.Count > 0 ? newControls[0].Height : 0;
                int totalShift = newControls.Count * (itemHeight + 5);

                // Dịch tất cả items hiện có xuống 1 lần duy nhất thay vì N lần
                foreach (Control ctrl in content.Controls)
                    ctrl.Location = new Point(ctrl.Location.X, ctrl.Location.Y + totalShift);

                // Thêm items mới vào đầu (item cuối list lên y=5, giữ đúng thứ tự gốc)
                int y = 5;
                for (int i = newControls.Count - 1; i >= 0; i--)
                {
                    newControls[i].Location = new Point(5, y);
                    content.Controls.Add(newControls[i]);
                    y += itemHeight + 5;
                }
            }
            finally
            {
                content.ResumeLayout();
            }

            this.UpdatePaymentCartAmount(false);
            SyncStartScanPanel();
        }

        public void AddConfirmSelectItems(List<CartItemModel> cartItems)
        {
            if (cartItems.Count <= 0)
            {
                CustomMessageBox.Show("Erorr: Please check cart item");
                return;
            }

            var content = panelCartItemsTouch.Content;

            // Tách thành 2 nhóm: item_id đã có → tăng qty; chưa có → thêm mới
            var toIncrement = new List<UCCartItem>();
            var toAdd       = new List<CartItemModel>();

            foreach (var ci in cartItems)
            {
                var existing = content.Controls
                    .OfType<NailsChekin.UserControl.UCCartItem>()
                    .FirstOrDefault(c => c.item_id == ci.item_id);

                if (existing != null)
                    toIncrement.Add(existing);
                else
                    toAdd.Add(ci);
            }

            content.SuspendLayout();
            try
            {
                // Tăng qty với items đã có trong cart
                foreach (var ctrl in toIncrement)
                    ctrl.increaseQuantity(1);

                // Thêm controls mới cho items chưa có
                if (toAdd.Count > 0)
                {
                    var newControls = new List<NailsChekin.UserControl.UCCartItem>(toAdd.Count);
                    foreach (var ci in toAdd)
                    {
                        var card = new NailsChekin.UserControl.UCCartItem(this, ci.item_id, ci.item_name, ci.price, ci.quantity);
                        card.Width = panelCartItemsTouch.Width - 5;
                        newControls.Add(card);
                    }

                    int itemHeight = newControls[0].Height;
                    int totalShift = newControls.Count * (itemHeight + 5);

                    // Dịch tất cả items hiện có xuống 1 lần
                    foreach (Control ctrl in content.Controls)
                        ctrl.Location = new Point(ctrl.Location.X, ctrl.Location.Y + totalShift);

                    // Chèn items mới vào đầu danh sách
                    int y = 5;
                    for (int i = newControls.Count - 1; i >= 0; i--)
                    {
                        newControls[i].Location = new Point(5, y);
                        content.Controls.Add(newControls[i]);
                        y += itemHeight + 5;
                    }
                }
            }
            finally
            {
                content.ResumeLayout();
            }

            this.UpdatePaymentCartAmount(false);
            SyncStartScanPanel();
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
            SyncStartScanPanel();
        }

        /// <summary>
        /// Hiện panelStartScan khi cart trống, ẩn khi có item.
        /// </summary>
        private void SyncStartScanPanel()
        {
            bool isEmpty = panelCartItemsTouch.Content.Controls.Count == 0;
            panelStartScan.Visible = isEmpty;
            if (isEmpty) panelStartScan.BringToFront();
        }

        public void ResetDefaultFocus()
        {
            this.BeginInvoke(new Action(() =>
            {
                this.current_text = "";
            }));
        }

        public string Check_Promotion(string item_id, string item_name, string price, string qty, bool is_scan)
        {
            string items = "";
            foreach (UCCartItem control in panelCartItemsTouch.Content.Controls.OfType<UCCartItem>())
            {
                items += @"{
                                'itemId': " + control.item_id + @",
                                'itemName': '" + Regex.Replace(control.item_name, "'", "") + @"',
                                'qty': " + control.quantity + @",
                                'price': " + Utilitys.getTotalAmount(control.price) + @",
                                'isPromotion':" + (string.IsNullOrEmpty(control.isPromotion) ? "0" : control.isPromotion) + @",
                                'scheme':'" + control.scheme + @"'
                            },";
            }

            if (items.Trim().Length > 0)
            {
                items = items.Substring(0, items.Length - 1);

                //Call API GET PROMOTION
                string jDATA = @"{
                                'item_id': " + item_id + @",
                                'item_name': '" + Regex.Replace(item_name, "'", "") + @"',
                                'price': " + price + @",
                                'qty': " + qty + @",
                                'items': [" + items + @"],
                                'is_scan': " + (is_scan ? "1" : "0") + @"  
                                }";

                string responce = Utilitys.CALL_API("Product/checkPromotionV2", jDATA, "POST", true);
                if (Utilitys.IsValidJson(responce))
                {
                    JArray jArray = JArray.Parse(responce);
                    foreach (var item in jArray)
                    {
                        string id = item["itemId"].ToString();
                        string name = item["name"].ToString();
                        string quantity = item["qty"].ToString();
                        string amount = item["amount"].ToString();
                        string scheme = item["scheme"].ToString();

                        bool exitst = false;
                        foreach (UCCartItem control in panelCartItemsTouch.Content.Controls.OfType<UCCartItem>())
                        {
                            if (control.item_id.Equals(item_id) && control.isPromotion.Equals("1"))
                            {
                                control.UpdateQty(quantity, false);
                                control.UpdatePromotionAmount(amount);
                                exitst = true;
                            }
                        }

                        if (!exitst)
                        {
                            var _content = panelCartItemsTouch.Content;
                            UCCartItem cardItem = new NailsChekin.UserControl.UCCartItem(this, id, name, amount, quantity, "0", "1", scheme);
                            cardItem.Width = panelCartItemsTouch.Width - 5;
                            int shift = cardItem.Height + 5;
                            _content.SuspendLayout();
                            try
                            {
                                foreach (Control ctrl in _content.Controls)
                                    ctrl.Location = new Point(ctrl.Location.X, ctrl.Location.Y + shift);
                                cardItem.Location = new Point(5, 5);
                                _content.Controls.Add(cardItem);
                            }
                            finally { _content.ResumeLayout(); }
                        }

                        if (!is_scan)  //Điều chỉnh thì cập nhật lại số lượng của item đang thao tác luôn
                        {
                            foreach (UCCartItem control in panelCartItemsTouch.Content.Controls.OfType<UCCartItem>())
                            {
                                if (control.item_id.Equals(item_id) && control.isPromotion.Equals("0"))
                                {
                                    string new_qty = Math.Round(double.Parse(control.quantity) - double.Parse(quantity), 2).ToString();
                                    control.UpdateQty(new_qty, false);
                                    exitst = true;
                                }
                            }
                        }
                    }

                    this.UpdatePaymentCartAmount(false);
                }

                return responce;
            }

            return "";
        }

        public void SendSearchItemLockUp(string searchKey)
        {
            this.SearchItemsByBarcodeOrSKU(searchKey, false, false);
        }

        public void ShowInventoryForm(string search)
        {
            FormInventoryAdjust frm = new FormInventoryAdjust(this, search);
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog(this);
            frm.Dispose();
        }

        public void SendSearchCustomerLockUp(string searchKey)
        {
            this.SearchCustomer(searchKey);
        }

        public string search_customer_lookup = "";
        public void SearchCustomer(string search)
        {
            this.search_customer_lookup = search;

            //Search Item
            string DATA = @"{
                            'searchString':'" + Regex.Replace(search, "\r", "") + @"',
                            'pageIndex':0, 
                            'pageSize':50
                        }";

            string responce = Utilitys.CALL_API("Customer/search", DATA, "POST", true);
            if (responce.StartsWith("Error"))
            {
                CustomMessageBox.Show(responce);
                return;
            }

            this.AddCustomerToCard(responce);
        }


        #endregion NEW RETAIL FUNCTION CLONE

        private void svgCart_RemoveCustomer_Click(object sender, EventArgs e)
        {
            this.ResetCustomers(true);
        }

        private void lbCart_AmtDue_TextChanged(object sender, EventArgs e)
        {
            double amtDue = Utilitys.getTotalAmount(lbCart_AmtDue.Text);
            UIHelper.SafeUI(lbCart_Tender, () => lbCart_Tender.Text = "$" + Math.Round(amtDue, 2).ToString());
        }

        public void SetTaxInfo(double tax_percent, bool tax_include)
        {
            this.tax_percent = tax_percent;
            this.tax_include = tax_include;
            this.UpdatePaymentCartAmount();
        }

        public void SetDiscountInfo(double discount_value, string discount_unit)
        {
            this.discount_value = discount_value;
            this.discount_unit = discount_unit;
            this.UpdatePaymentCartAmount();
        }


    }
}
