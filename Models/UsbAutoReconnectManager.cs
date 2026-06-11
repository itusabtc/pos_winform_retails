using System;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace NailsChekin.Models
{
    public class UsbAutoReconnectManager
    {
        private ManagementEventWatcher watcher;
        private Action onUsbDisconnected;
        private Func<Task> onUsbReconnected;

        public UsbAutoReconnectManager(Action disconnectedCallback, Func<Task> reconnectedCallback)
        {
            onUsbDisconnected = disconnectedCallback;
            onUsbReconnected = reconnectedCallback;
        }

        public void Start()
        {
            try
            {
                // EventType 2 = Device Arrival (USB cắm vào)
                // EventType 3 = Device Removal (USB rút ra)
                string queryText = "SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2 OR EventType = 3";

                WqlEventQuery query = new WqlEventQuery(queryText);
                watcher = new ManagementEventWatcher(query);
                watcher.EventArrived += OnDeviceChanged;
                watcher.Start();

                Console.WriteLine("UsbAutoReconnectManager started.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Watcher error: " + ex.Message);
            }
        }

        private void OnDeviceChanged(object sender, EventArrivedEventArgs e)
        {
            int eventType = Convert.ToInt32(e.NewEvent.Properties["EventType"].Value);
            Console.WriteLine($"USB Event Received: EventType = {eventType}");

            if (eventType == 3) // USB removed
            {
                onUsbDisconnected?.Invoke();
            }
            else if (eventType == 2) // USB inserted
            {
                Task.Run(async () =>
                {
                    // Wait a bit for Windows to recognize the device
                    await Task.Delay(2000);

                    Console.WriteLine("Attempting to reconnect to USB device...");
                    await onUsbReconnected?.Invoke();
                });
            }
        }

        public void Stop()
        {
            try
            {
                watcher?.Stop();
                watcher?.Dispose();
            }
            catch { }
        }
    }
}
