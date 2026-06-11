using System;
using System.Management;

namespace NailsChekin.Models
{
    public class UsbWatcher
    {
        private ManagementEventWatcher watcher;
        private Action onUsbDisconnected; // delegate callback

        public UsbWatcher(Action disconnectedCallback)
        {
            onUsbDisconnected = disconnectedCallback;
        }

        public void Start()
        {
            try
            {
                // EventType = 3 nghĩa là thiết bị bị tháo ra
                WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 3");

                watcher = new ManagementEventWatcher(query);
                watcher.EventArrived += new EventArrivedEventHandler(DeviceRemovedEvent);
                watcher.Start();

                Console.WriteLine("USB Watcher Started.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("USB Watcher Error: " + ex.Message);
            }
        }

        private void DeviceRemovedEvent(object sender, EventArrivedEventArgs e)
        {
            Console.WriteLine("USB device disconnected!");

            // Gọi lại callback của bạn ở đây
            onUsbDisconnected?.Invoke(); // Gọi lại callback đã truyền

            Stop();
        }

        public void Stop()
        {
            try
            {
                if (watcher != null)
                {
                    watcher.Stop();
                    watcher.Dispose();
                }
            }
            catch { }
        }
    }
}
