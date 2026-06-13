using NailsChekin.Models;
using NailsChekin.MyControls;
using NailsChekin.UserControl;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NailsChekin.Popup
{
    public partial class FormKeyboardOnlyNumber : Form
    {
        private Control _parentForm;
        private string  _redirectUrl = "";
        private string  _controlId   = "";
        public string _defaultValue = "";

        private KeyBoardSearch _kb;
        private Label          _lbInput;
        private RoundPanel     _displayPanel;
        private bool           _isProcessing;

        public FormKeyboardOnlyNumber()
        {
            InitializeComponent();
        }

        public FormKeyboardOnlyNumber(Control parent, string default_value, string control_id, string redirect_url)
        {
            InitializeComponent();

            _parentForm  = parent;
            _controlId   = control_id;
            _redirectUrl = redirect_url;
            _defaultValue = default_value;

            this.KeyPress += FormKeyboardOnlyNumber_KeyPress;

            if (_redirectUrl.Contains("Change") || _redirectUrl.Contains("Discount"))
                btnClose.Title = "CANCEL";

            SetupKeyboard();
        }

        // ── Layout ──────────────────────────────────────────────────────────

        private void SetupKeyboard()
        {
            this.BackColor = Color.FromArgb(96, 165, 250);    // #60A5FA – blue-400, dịu hơn

            // Replace TextBox with a styled label at the same position
            txtCurrentText.Visible = false;

            _lbInput = new Label
            {
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font      = new Font("Segoe UI", 24f, FontStyle.Bold),
                ForeColor = Color.FromArgb(12, 74, 110),    // #0C4A6E – sky-950, dark readable
                BackColor = Color.Transparent
            };
            _displayPanel = new RoundPanel
            {
                BackColor   = Color.White,
                BorderColor = Color.FromArgb(56, 189, 248), // #38BDF8 – sky-400 border
                Bounds      = txtCurrentText.Bounds
            };
            _displayPanel.Controls.Add(_lbInput);
            Controls.Add(_displayPanel);
            _displayPanel.BringToFront();

            // KeyBoardSearch fills the keyboard panel
            _kb = new KeyBoardSearch
            {
                TargetControl      = _lbInput,
                Dock               = DockStyle.Fill,
                AllowDecimal       = true,
                MaxLength          = 30,
                ButtonFontSize     = 20f,
                ButtonCornerRadius = 10,
                DefaultValue       = _defaultValue
            };
            panelCart_Keyboard.Controls.Add(_kb);

            // overwriteOnNextKey: value pre-fill coi như đang "select" — bấm phím số đầu tiên
            // sẽ thay thế toàn bộ (vd đang hiện 22, bấm 1 => thành 1 chứ không phải 221)
            if (!string.IsNullOrEmpty(_defaultValue))
                _kb.SetValue(_defaultValue, overwriteOnNextKey: true);

            //_kb.ValueChanged += (s, e) => _lbInput.Text = _kb.Value;
            _kb.EnterPressed += (s, e) => EnterNow();
        }

        // ── Key events ──────────────────────────────────────────────────────

        private void FormKeyboardOnlyNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) EnterNow();
        }

        // ── Confirm ─────────────────────────────────────────────────────────

        private void EnterNow()
        {
            if (_isProcessing) return;

            string value = _kb?.Value.Trim() ?? "";
            if (value.Length == 0) { MessageBox.Show("Please check input enter"); return; }

            _isProcessing = true;

            if (_redirectUrl == "SearchCustomerPhone")
            {
                var resp = Utilitys.CALL_API("Customer/findPhoneV2?phone=" + value + "&paring_code=" + Constants.pairing_code, "", "GET", true);

                if (resp.StartsWith("Error"))
                {
                    MessageBox.Show(resp);
                    _isProcessing = false;
                    return;
                }

                ((FormMain)_parentForm).SearchCustomerByPhone(resp, value);
                this.Close();
            }
            else if (_redirectUrl == "ChangePriceLocal")
            {
                ((UCCartItem)_parentForm).UpdatePrice(value);
                this.Close();
            }
            else if (_redirectUrl == "ChangeQtyLocal")
            {
                ((UCCartItem)_parentForm).UpdateQty(value, true);
                this.Close();
            }

            _isProcessing = false;
        }

        // ── Close ───────────────────────────────────────────────────────────

        private void FormKeyboardOnlyNumber_FormClosed(object sender, FormClosedEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000);
                try
                {
                    Core.ClearAndDisposeV2(panelCart_Keyboard);  // dispose _kb + children
                    Core.ClearMemory();
                    if (!this.IsDisposed)
                        this.Invoke((Action)(() => this.Dispose()));
                }
                catch { }
            });
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
