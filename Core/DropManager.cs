using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using Jiffy.Utils;

namespace Jiffy.Core
{
    public static class DropManager
    {
        private static List<FileDrop> drops = new List<FileDrop>();

        // 🔔 This event lets UI auto-refresh on new drop
        public static Action OnDropReceived;

        public static FileDrop CreateDrop(string filePath)
        {
            var drop = new FileDrop(filePath);
            drops.Add(drop);
            return drop;
        }

        public static void AddExternalDrop(FileDrop drop)
        {
            if (drop.SenderIP == GetLocalIPAddress())
                return; // 🔥 Ignore your own broadcast

            if (!drops.Any(d =>
                d.FileName == drop.FileName &&
                d.Timestamp == drop.Timestamp &&
                d.SSID == drop.SSID))
            {
                drops.Add(drop);
            }
        }

        // Add this helper method:
        public static void RemoveDrop(FileDrop drop)
        {
            drops.Remove(drop);
        }

        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            }
            return "127.0.0.1";
        }

        public static List<FileDrop> GetDropsForCurrentSSID()
        {
            var currentSSID = WiFiUtils.GetCurrentSSID();
            return drops.Where(d => d.SSID == currentSSID).ToList();
        }
    }
}
