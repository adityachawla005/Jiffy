using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Jiffy.Utils;
using Jiffy.Security;

namespace Jiffy.Core
{
    public class FileDrop
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileHash { get; set; }

        public DateTime Timestamp { get; set; }
        public string SSID { get; set; }
        public string SenderIP { get; set; }
        public int Port { get; set; } = 5000;
        public bool IsTemporary { get; set; } = true; // 🔹 Mark as temp by default

        public FileDrop(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
            Timestamp = DateTime.Now;
            SSID = WiFiUtils.GetCurrentSSID();
            SenderIP = GetLocalIPAddress();
            FileHash = FileHasher.ComputeSHA256(filePath);
        }

        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            return "127.0.0.1";
        }

        public override string ToString()
        {
            return $"📂 {FileName}\n📶 {SSID}\n🕒 {Timestamp:t}";
        }
    }
}
