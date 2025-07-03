using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using Jiffy.Core;
using Jiffy.Utils;
using Jiffy.Security;
using System.Windows;

namespace Jiffy.Core
{
    public static class DropListener
    {
        private const int Port = 41234;
        private static readonly IPAddress MulticastAddress = IPAddress.Parse("239.0.0.222");
        private static UdpClient listener;
        private static Thread listenThread;

        public static void Start()
        {
            listener = new UdpClient();
            listener.ExclusiveAddressUse = false;

            IPEndPoint localEp = new IPEndPoint(IPAddress.Any, Port);
            listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            listener.Client.Bind(localEp);
            listener.JoinMulticastGroup(MulticastAddress);

            listenThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        var data = listener.Receive(ref localEp);
                        var json = Encoding.UTF8.GetString(data);
                        var drop = JsonConvert.DeserializeObject<FileDrop>(json);

                        if (drop?.SSID == WiFiUtils.GetCurrentSSID())
                        {
                            // ⬇️ Fetch file from sender
                            var fetchedPath = FileFetcher.Fetch(drop.SenderIP, drop.Port, drop.FileName);
                            if (fetchedPath != null)
                            {
                                // ✅ Verify file hash
                                string receivedHash = FileHasher.ComputeSHA256(fetchedPath);
                                if (drop.FileHash == receivedHash)
                                {
                                    drop.FilePath = fetchedPath;
                                    DropManager.AddExternalDrop(drop);
                                }
                                else
                                {
                                    // ⚠️ Hash mismatch warning
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        MessageBox.Show($"⚠️ File hash mismatch: {drop.FileName}", "Security Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    });
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Optionally log or debug
                    }
                }
            });

            listenThread.IsBackground = true;
            listenThread.Start();
        }

        public static void Stop()
        {
            listener?.Close();
            listenThread?.Interrupt();
        }
    }
}
