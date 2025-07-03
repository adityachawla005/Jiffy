﻿using System.Net;
using System.Net.Sockets;

namespace Jiffy.Utils
{
    public static class NetworkUtils
    {
        public static string GetLocalIPAddress()
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                var endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint?.Address.ToString() ?? "127.0.0.1";
            }
        }
    }
}
