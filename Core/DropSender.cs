using Newtonsoft.Json;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Jiffy.Core;
using System.IO;
using System.Threading;

public static class DropSender
{
    private const int UdpPort = 41234;
    private static readonly IPAddress MulticastAddress = IPAddress.Parse("239.0.0.222");

    public static void Broadcast(FileDrop drop)
    {
        // ✅ Start file sharing TCP server in a background thread
        ThreadPool.QueueUserWorkItem(_ => StartTcpFileServer(drop));

        // ✅ Broadcast metadata over UDP
        try
        {
            using UdpClient client = new UdpClient();
            IPEndPoint endPoint = new IPEndPoint(MulticastAddress, UdpPort);

            var json = JsonConvert.SerializeObject(drop);
            var bytes = Encoding.UTF8.GetBytes(json);
            client.Send(bytes, bytes.Length, endPoint);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[UDP Sender] Error: {ex.Message}");
        }
    }

    private static void StartTcpFileServer(FileDrop drop)
    {
        try
        {
            TcpListener listener = new TcpListener(IPAddress.Any, drop.Port);
            listener.Start();

            // Timeout logic to avoid hanging forever
            IAsyncResult result = listener.BeginAcceptTcpClient(null, null);
            bool connected = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(10));

            if (!connected)
            {
                listener.Stop();
                System.Diagnostics.Debug.WriteLine("[TCP Sender] No client connected within timeout.");
                return;
            }

            using TcpClient client = listener.EndAcceptTcpClient(result);
            using NetworkStream networkStream = client.GetStream();
            using FileStream fs = new FileStream(drop.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            byte[] buffer = new byte[8192];
            int bytesRead;
            while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
            {
                networkStream.Write(buffer, 0, bytesRead);
            }

            System.Diagnostics.Debug.WriteLine("[TCP Sender] File sent successfully.");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[TCP Sender] Error: {ex.Message}");
        }
    }
}
