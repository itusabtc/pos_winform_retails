using System;
using System.Windows.Forms;
using System.Text.Json;
using WebSocketSharp;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using WebSocketSharp.Server;
using Makaretu.Dns;
using Tmds.MDns;
using ECRWlanDemo;
using System.IO;
using NailsChekin.Models.Helper;
using System.Threading;

namespace NailsChekin.Models.Payments
{
    public class P5Lib
    {
        private CodePayHelper _codePay;

        // client websocket
        public WebSocket _clientWebSocket;

        // pair server websocket
        public WebSocketServer _serverWebSocket;

        // mdns register service
        private ServiceDiscovery _serviceDiscovery;

        // mdns listener
        private ServiceBrowser _serviceBrowser;

        // ECR System Name, This name will be displayed on the terminal.
        private static string ECR_NAME = "My ECR";

        // ECR system unique ID
        private static string MAC_ADDRESS = "123456";

        // client type
        private static string REMOTE_CLIENT_TYPE = "_ecr-hub-client._tcp.";

        // server type
        public static string REMOTE_SERVER_TYPE = "_ecr-hub-server._tcp";

        // default port
        public static ushort PORT = 35779;

        // Paired terminal data
        private static DeviceData _pairedData = null;

        private FormMain frmMain;

        // Prevent duplicate warm-up connect
        private int _warmUpConnecting = 0;

        // Prevent duplicate mdns event attach
        private int _mdnsListenerRegistered = 0;

        // Prevent duplicate pair server open
        private int _pairServerOpening = 0;

        public P5Lib() { }

        public P5Lib(FormMain frmMain)
        {
            this.frmMain = frmMain;
            InitCodePayHelper();
        }

        private void InitCodePayHelper()
        {
            _codePay = new CodePayHelper(
                uiInvokeTarget: frmMain,
                getServerUrl: () => P5Lib.Get_ServerURL(),
                getSocket: () => _clientWebSocket,
                setSocket: ws => _clientWebSocket = ws,
                updateStatus: s =>
                {
                    if (frmMain != null)
                        frmMain.UpdateCreditDeviceStatus(s);
                },
                onMessage: msg =>
                {
                    if (frmMain != null)
                        frmMain.CodePay_Process_OnMessage(msg);
                },
                setConnectedFlag: connected =>
                {
                    if (frmMain != null)
                        frmMain.isCodePaySocketConnect = connected;
                },
                log: (msg, title) => LogHelper.SaveLOG_CodePay(msg, title)
            );

            _codePay.ConnectTimeoutMs = 5000;
            _codePay.ConnectRetries = 2;
            _codePay.SendReconnectRetries = 1;
        }

        public void OpenPairServer()
        {
            if (Interlocked.Exchange(ref _pairServerOpening, 1) == 1)
                return;

            try
            {
                LogHelper.SaveLOG_CodePay("", "OpenPairServer");

                if (_serverWebSocket == null)
                {
                    var ipAddress = getLoacalIpAddress();
                    LogHelper.SaveLOG_CodePay(ipAddress, "ipAddress");

                    if (string.IsNullOrWhiteSpace(ipAddress))
                        return;

                    var url = "ws://" + ipAddress + ":" + P5Lib.PORT;

                    Console.WriteLine("url:" + url);
                    LogHelper.SaveLOG_CodePay(url, "url");

                    _serverWebSocket = new WebSocketServer(url);
                }

                if (!_serverWebSocket.IsListening)
                {
                    try
                    {
                        _serverWebSocket.AddWebSocketService<EchoService>("/");
                    }
                    catch (Exception ex)
                    {
                        // If service was already added, do not crash POS.
                        LogHelper.SaveLOG_CodePay(ex.Message, "AddWebSocketService Exception");
                    }

                    _serverWebSocket.Start();

                    Console.WriteLine("WebSocket server started");
                    LogHelper.SaveLOG_CodePay("WebSocket server started", "_serverWebSocket.IsListening");

                    registerMDNSService();
                    registerMdnsListener();
                }
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_CodePay(ex.Message, "OpenPairServer Exception");
            }
            finally
            {
                Interlocked.Exchange(ref _pairServerOpening, 0);
            }
        }

        public void registerMdnsListener()
        {
            try
            {
                if (_serviceBrowser == null)
                    _serviceBrowser = new ServiceBrowser();

                // Prevent duplicate event handler registration.
                if (Interlocked.Exchange(ref _mdnsListenerRegistered, 1) == 0)
                {
                    _serviceBrowser.ServiceAdded += onServiceAdded;
                    _serviceBrowser.ServiceRemoved += onServiceRemoved;
                    _serviceBrowser.ServiceChanged += onServiceChanged;
                }

                Console.WriteLine("Browsing for type: {0}", REMOTE_SERVER_TYPE);
                LogHelper.SaveLOG_CodePay("Browsing for type: " + REMOTE_SERVER_TYPE, "registerMdnsListener");

                if (!_serviceBrowser.IsBrowsing)
                    _serviceBrowser.StartBrowse("_ecr-hub-server._tcp");
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_CodePay(ex.Message, "registerMdnsListener Exception");
            }
        }

        void onServiceChanged(object sender, ServiceAnnouncementEventArgs e)
        {
            UpdateServer(e.Announcement);
        }

        void onServiceRemoved(object sender, ServiceAnnouncementEventArgs e)
        {
            Console.WriteLine("on service removed");
        }

        void onServiceAdded(object sender, ServiceAnnouncementEventArgs e)
        {
            UpdateServer(e.Announcement);
        }

        void UpdateServer(ServiceAnnouncement service)
        {
            try
            {
                if (service == null)
                    return;

                Console.WriteLine("{0}' on {1}", service.Instance, service.NetworkInterface.Name);
                Console.WriteLine("\tHost: {0} ({1})", service.Hostname, string.Join(", ", service.Addresses));
                Console.WriteLine("\tPort: {0}", service.Port);

                var info = string.Join(", ", service.Txt);
                Console.WriteLine("\tTxt : [{0}]", info);

                if (string.IsNullOrWhiteSpace(info))
                    return;

                DeviceData data = null;

                try
                {
                    data = JsonSerializer.Deserialize<DeviceData>(info);
                }
                catch (Exception ex)
                {
                    LogHelper.SaveLOG_CodePay(ex.Message + " | TXT=" + info, "UpdateServer Deserialize Exception");
                    return;
                }

                if (data == null || string.IsNullOrWhiteSpace(data.MacAddress))
                    return;

                if (_pairedData != null && _pairedData.MacAddress == data.MacAddress)
                {
                    bool changed =
                        data.IpAddress != _pairedData.IpAddress ||
                        data.Port != _pairedData.Port;

                    if (!changed)
                        return;

                    _pairedData.MacAddress = data.MacAddress;
                    _pairedData.IpAddress = data.IpAddress;
                    _pairedData.Port = data.Port;

                    var url = "ws://" + _pairedData.IpAddress + ":" + _pairedData.Port;

                    LogHelper.SaveLOG_CodePay(url, "UpdateServer connectServer");

                    connectServer(url);
                    save_p5_paircode();
                }
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_CodePay(ex.Message, "UpdateServer Exception");
            }
        }

        // Register local ip into wlan
        public void registerMDNSService()
        {
            try
            {
                if (_serviceDiscovery == null)
                    _serviceDiscovery = new ServiceDiscovery();

                var profile = new ServiceProfile(ECR_NAME, REMOTE_CLIENT_TYPE, PORT);
                profile.AddProperty("mac_address", MAC_ADDRESS);
                profile.AddProperty("ip_address", getLoacalIpAddress() + ":" + PORT);

                _serviceDiscovery.AnswersContainsAdditionalRecords = true;
                _serviceDiscovery.Advertise(profile);
                _serviceDiscovery.Announce(profile);

                Console.WriteLine("register success");
                LogHelper.SaveLOG_CodePay("ip_address: " + (getLoacalIpAddress() + ":" + PORT), "registerMDNSService Success");
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_CodePay(ex.Message, "registerMDNSService Exception");
            }
        }

        // UnRegister local ip
        private void unRegisterMDNSService()
        {
            try
            {
                if (_serviceDiscovery != null)
                {
                    _serviceDiscovery.Unadvertise();
                    _serviceDiscovery.Dispose();
                    _serviceDiscovery = null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_CodePay(ex.Message, "unRegisterMDNSService Exception");
            }
        }

        // The ECR receives requests to pair and unpair payment terminals.
        public class EchoService : WebSocketBehavior
        {
            protected override void OnMessage(WebSocketSharp.MessageEventArgs e)
            {
                try
                {
                    Console.WriteLine("MessageEventArgs" + e.Data);

                    ECRHubMessageData data = JsonSerializer.Deserialize<ECRHubMessageData>(e.Data);

                    if (data == null)
                        return;

                    Console.WriteLine("topic" + data.Topic);

                    if (data.Topic == CodePayConstants.ECR_HUB_TOPIC_PAIR)
                    {
                        string deviceName = "";
                        try
                        {
                            if (data.DeviceData != null)
                                deviceName = data.DeviceData.DeviceName;
                        }
                        catch { }

                        DialogResult result = MessageBox.Show(
                            "Confirm pairing this device:" + deviceName,
                            "Confirm",
                            MessageBoxButtons.OKCancel,
                            MessageBoxIcon.Question
                        );

                        if (result == DialogResult.OK)
                        {
                            Console.WriteLine("confirm pair");

                            _pairedData = data.DeviceData;
                            data.ResponseCode = CodePayConstants.SUCCESS_STATUS;

                            string message = JsonSerializer.Serialize(data);
                            Console.WriteLine("Send Data JSON: " + message);

                            Send(message);
                            save_p5_paircode();
                        }
                        else if (result == DialogResult.Cancel)
                        {
                            Console.WriteLine("cancel pair");

                            data.ResponseCode = CodePayConstants.FAIL_STATUS;

                            string message = JsonSerializer.Serialize(data);
                            Console.WriteLine("Send Data JSON: " + message);

                            Send(message);
                        }
                    }
                    else if (data.Topic == CodePayConstants.ECR_HUB_TOPIC_UNPAIR)
                    {
                        _pairedData = null;

                        data.ResponseCode = CodePayConstants.SUCCESS_STATUS;

                        string message = JsonSerializer.Serialize(data);
                        Console.WriteLine("Send Data JSON: " + message);

                        Send(message);
                    }
                }
                catch (Exception ex)
                {
                    try
                    {
                        LogHelper.SaveLOG_CodePay(ex.Message, "EchoService OnMessage Exception");
                    }
                    catch { }
                }
            }
        }

        public string getLoacalIpAddress()
        {
            try
            {
                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (NetworkInterface network in networkInterfaces)
                {
                    if (network.OperationalStatus == OperationalStatus.Up &&
                        network.Supports(NetworkInterfaceComponent.IPv4))
                    {
                        IPInterfaceProperties ipProperties = network.GetIPProperties();

                        foreach (UnicastIPAddressInformation address in ipProperties.UnicastAddresses)
                        {
                            if (address.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                var ipAddress = address.Address.ToString();

                                Console.WriteLine("IP Address: " + address.Address);
                                LogHelper.SaveLOG_CodePay(address.Address.ToString(), "IP Address");

                                if (ipAddress.StartsWith("192") ||
                                    ipAddress.StartsWith("172") ||
                                    ipAddress.StartsWith("10"))
                                {
                                    return ipAddress;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_CodePay(ex.Message, "getLoacalIpAddress Exception");
            }

            return "";
        }

        // Starting a transaction request - safe wrapper
        public async void startTransactions(ECRHubMessageData data)
        {
            try
            {
                if (data == null)
                {
                    LogHelper.SaveLOG_CodePay("ECRHubMessageData is null", "startTransactions");
                    return;
                }

                string message = JsonSerializer.Serialize(data);
                string err = await startTransactionsAsync(message).ConfigureAwait(false);

                if (!string.IsNullOrWhiteSpace(err))
                    LogHelper.SaveLOG_CodePay(err, "startTransactions Error");
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_CodePay(ex.Message, "startTransactions Exception");
            }
        }

        // Kept for compatibility. Internally uses the new safe SendPaymentAsync.
        public async void startTransactions_OLD(string message)
        {
            try
            {
                string err = await startTransactionsAsync(message).ConfigureAwait(false);

                if (!string.IsNullOrWhiteSpace(err))
                    LogHelper.SaveLOG_CodePay(err, "startTransactions_OLD Error");
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_CodePay(ex.Message, "startTransactions_OLD Exception");
            }
        }

        public void startTransactions(string message)
        {
            try
            {
                if (_codePay == null)
                {
                    LogHelper.SaveLOG_CodePay("CodePayHelper is null", "startTransactions");
                    return;
                }

                _ = Task.Run(async () =>
                {
                    try
                    {
                        string err = await _codePay.SendPaymentAsync(message).ConfigureAwait(false);

                        if (!string.IsNullOrWhiteSpace(err))
                            LogHelper.SaveLOG_CodePay(err, "startTransactions SendPaymentAsync Error");
                    }
                    catch (Exception ex)
                    {
                        LogHelper.SaveLOG_CodePay(ex.Message, "startTransactions SendPaymentAsync Exception");
                    }
                });
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_CodePay(ex.Message, "startTransactions Exception");
            }
        }

        public Task<string> startTransactionsAsync(string message)
        {
            if (_codePay == null)
                return Task.FromResult("Error: CodePayHelper is null.");

            return _codePay.SendPaymentAsync(message);
        }

        public bool checkDeviceOpened()
        {
            try
            {
                return _clientWebSocket != null &&
                       _clientWebSocket.ReadyState == WebSocketState.Open;
            }
            catch
            {
                return false;
            }
        }

        // Kept for compatibility only. Do not use old direct socket logic.
        private void disconnectServer_OLD()
        {
            disconnectServer();
        }

        /// <summary>
        /// Properly await disconnect + dispose của CodePayHelper.
        /// Dùng khi cần đảm bảo old P5Lib không còn callback nào active trước khi tạo instance mới.
        /// </summary>
        public async Task DisconnectAndDisposeAsync()
        {
            if (_codePay != null)
            {
                try { await _codePay.DisconnectAsync().ConfigureAwait(false); } catch { }
                try { _codePay.Dispose(); } catch { }
                _codePay = null;
            }

            // Close fallback WebSocket nếu _codePay không manage nó
            try
            {
                var ws = _clientWebSocket;
                _clientWebSocket = null;
                if (ws != null)
                {
                    try { ws.CloseAsync(); } catch { try { ws.Close(); } catch { } }
                }
            }
            catch { }
        }

        public void disconnectServer()
        {
            try
            {
                if (_codePay != null)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _codePay.DisconnectAsync().ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.SaveLOG_CodePay(ex.Message, "disconnectServer DisconnectAsync Exception");
                        }
                    });

                    return;
                }
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_CodePay(ex.Message, "disconnectServer Exception");
            }

            try
            {
                var ws = _clientWebSocket;
                _clientWebSocket = null;

                if (ws != null)
                {
                    try { ws.CloseAsync(); }
                    catch { try { ws.Close(); } catch { } }
                }

                SafeUi(() =>
                {
                    if (frmMain != null)
                    {
                        frmMain.UpdateCreditDeviceStatus("Disconnected from payment terminal");
                        frmMain.isCodePaySocketConnect = false;
                    }
                });
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_CodePay(ex.Message, "disconnectServer fallback Exception");
            }
        }

        // Kept for compatibility only. Do not use old direct socket logic.
        private WebSocket connectServer_OLD(string ip)
        {
            return connectServer(ip);
        }

        public WebSocket connectServer(string ip)
        {
            try
            {
                if (_codePay != null)
                {
                    if (Interlocked.Exchange(ref _warmUpConnecting, 1) == 1)
                        return _clientWebSocket;

                    Task.Run(async () =>
                    {
                        try
                        {
                            await _codePay.WarmUpConnectAsync().ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.SaveLOG_CodePay(ex.Message, "connectServer WarmUp Exception");
                        }
                        finally
                        {
                            Interlocked.Exchange(ref _warmUpConnecting, 0);
                        }
                    });

                    return _clientWebSocket;
                }

                // Fallback: still runs in background, so it will not freeze the POS UI.
                if (Interlocked.Exchange(ref _warmUpConnecting, 1) == 1)
                    return _clientWebSocket;

                Task.Run(() =>
                {
                    WebSocket oldWs = null;

                    try
                    {
                        oldWs = _clientWebSocket;

                        if (oldWs != null)
                        {
                            if (oldWs.ReadyState == WebSocketState.Open ||
                                oldWs.ReadyState == WebSocketState.Connecting)
                            {
                                return;
                            }

                            _clientWebSocket = null;

                            try { oldWs.CloseAsync(); }
                            catch { try { oldWs.Close(); } catch { } }
                        }

                        if (string.IsNullOrWhiteSpace(ip))
                        {
                            LogHelper.SaveLOG_CodePay("ip is empty", "connectServer fallback");
                            return;
                        }

                        var ws = new WebSocket(ip);

                        ws.OnOpen += (sender, e) =>
                        {
                            SafeUi(() =>
                            {
                                if (frmMain != null)
                                {
                                    frmMain.UpdateCreditDeviceStatus("Payment terminal server connected");
                                    frmMain.isCodePaySocketConnect = true;
                                }
                            });

                            LogHelper.SaveLOG_CodePay("WebSocket opened. URL=" + ip, "connectServer fallback");
                        };

                        ws.OnMessage += (sender, e) =>
                        {
                            SafeUi(() =>
                            {
                                if (frmMain != null)
                                    frmMain.CodePay_Process_OnMessage(e.Data);
                            });
                        };

                        ws.OnError += (sender, e) =>
                        {
                            SafeUi(() =>
                            {
                                if (frmMain != null)
                                {
                                    frmMain.UpdateCreditDeviceStatus("Payment terminal error: " + e.Message);
                                    frmMain.isCodePaySocketConnect = false;
                                }
                            });

                            LogHelper.SaveLOG_CodePay(e.Message, "connectServer fallback OnError");
                        };

                        ws.OnClose += (sender, e) =>
                        {
                            SafeUi(() =>
                            {
                                if (frmMain != null)
                                {
                                    frmMain.UpdateCreditDeviceStatus("Payment terminal server not connected");
                                    frmMain.isCodePaySocketConnect = false;
                                }
                            });

                            LogHelper.SaveLOG_CodePay("Closed. Code=" + e.Code + ", Reason=" + e.Reason, "connectServer fallback OnClose");
                        };

                        _clientWebSocket = ws;

                        // This runs in background, not UI thread.
                        ws.Connect();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.SaveLOG_CodePay(ex.Message, "connectServer fallback Exception");

                        SafeUi(() =>
                        {
                            if (frmMain != null)
                            {
                                frmMain.UpdateCreditDeviceStatus("Error connecting to WebSocket server: " + ex.Message);
                                frmMain.isCodePaySocketConnect = false;
                            }
                        });
                    }
                    finally
                    {
                        Interlocked.Exchange(ref _warmUpConnecting, 0);
                    }
                });
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_CodePay(ex.Message, "connectServer Exception");
                Interlocked.Exchange(ref _warmUpConnecting, 0);
            }

            return _clientWebSocket;
        }

        // DO NOT USE - old unsafe connect logic removed.
        private WebSocket connectServer__(string ip)
        {
            return connectServer(ip);
        }

        private void SafeUi(Action action)
        {
            try
            {
                if (action == null)
                    return;

                if (frmMain != null &&
                    !frmMain.IsDisposed &&
                    frmMain.IsHandleCreated &&
                    frmMain.InvokeRequired)
                {
                    frmMain.BeginInvoke(action);
                }
                else
                {
                    action();
                }
            }
            catch { }
        }

        public DeviceData GetPairedData()
        {
            if (_pairedData == null)
            {
                string p5_paircode = get_p5_paired_code();

                if (!string.IsNullOrEmpty(p5_paircode))
                    _pairedData = JsonSerializer.Deserialize<DeviceData>(p5_paircode);
            }

            return _pairedData;
        }

        private static void save_p5_paircode()
        {
            Save_P5_Config(JsonSerializer.Serialize(_pairedData), "pairedData");
        }

        private string get_p5_paired_code()
        {
            return Get_P5_Config("pairedData");
        }

        public static void Save_P5_Config(string log, string title)
        {
            try
            {
                string forderLog = "C:\\POSLogs\\Config\\P5_config.txt";

                string dir = Path.GetDirectoryName(forderLog);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                using (StreamWriter sw = new StreamWriter(forderLog, false))
                {
                    try
                    {
                        sw.WriteLine(title + ": " + log);
                    }
                    catch
                    {
                        sw.WriteLine(DateTime.Now.ToString() + "." + title + " EXCEPTION: " + log);
                    }
                }
            }
            catch { }
        }

        public static string Get_P5_Config(string key)
        {
            try
            {
                string value = "";
                string forderLog = "C:\\POSLogs\\Config\\P5_config.txt";

                if (!File.Exists(forderLog))
                    return "";

                using (StreamReader sr = File.OpenText(forderLog))
                {
                    string s = "";

                    while ((s = sr.ReadLine()) != null)
                    {
                        if (s.StartsWith(key))
                        {
                            value = s.Replace(key + ": ", "");
                            break;
                        }
                    }
                }

                return value;
            }
            catch
            {
                return "";
            }
        }

        public static string Get_ServerURL()
        {
            try
            {
                var connectionType = Get_P5_ConecttionType_Setting();

                if (connectionType == P5_CONNECTTION_TYPE.WLAN_LAN)
                {
                    string codepay_ip_address = ConfigLocalHelper.GetConfig("codepay_ip_address", "");

                    if (string.IsNullOrWhiteSpace(codepay_ip_address))
                        return "";

                    if (!codepay_ip_address.StartsWith("ws"))
                        codepay_ip_address = "ws://" + codepay_ip_address;

                    return codepay_ip_address;
                }
                else if (connectionType == P5_CONNECTTION_TYPE.PAIR_MODE)
                {
                    if (_pairedData == null)
                    {
                        string p5_paircode = Get_P5_Config("pairedData");

                        if (!string.IsNullOrEmpty(p5_paircode))
                            _pairedData = JsonSerializer.Deserialize<DeviceData>(p5_paircode);
                    }

                    if (_pairedData != null)
                    {
                        string serverUrl = _pairedData.IpAddress + ":" + _pairedData.Port;

                        if (!serverUrl.StartsWith("ws"))
                            serverUrl = "ws://" + serverUrl;

                        return serverUrl;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.SaveLOG_CodePay(ex.Message, "Get_ServerURL Exception");
            }

            return "";
        }

        public static P5_CONNECTTION_TYPE Get_P5_ConecttionType_Setting()
        {
            string codepay_connection_type = ConfigLocalHelper.GetConfig("codepay_connection_type", "");

            if (codepay_connection_type.Equals("WLAN/LAN"))
                return P5_CONNECTTION_TYPE.WLAN_LAN;
            else if (codepay_connection_type.Equals("PAIR MODE"))
                return P5_CONNECTTION_TYPE.PAIR_MODE;
            else if (codepay_connection_type.Equals("USB"))
                return P5_CONNECTTION_TYPE.USB;
            else if (codepay_connection_type.Equals("CLOUD"))
                return P5_CONNECTTION_TYPE.CLOUD;

            return P5_CONNECTTION_TYPE.NONE;
        }
    }

    internal class CodePayConstants
    {
        public static string ECR_HUB_TOPIC_PAIR = "ecrhub.pair";
        public static string ECR_HUB_TOPIC_UNPAIR = "ecrhub.unpair";

        public static string PAYMENT_TOPIC = "ecrhub.pay.order";
        public static string QUERY_TOPIC = "ecrhub.pay.query";
        public static string CLOSE_TOPIC = "ecrhub.pay.close";

        public static string SUCCESS_STATUS = "000";
        public static string FAIL_STATUS = "001";
    }
}