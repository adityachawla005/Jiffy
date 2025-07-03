using System;
using System.IO;
using System.Net.Sockets;

namespace Jiffy.Core
{
    public static class FileFetcher
    {
        /// <summary>
        /// Downloads a file from the specified IP and port and stores it in a temp location.
        /// </summary>
        /// <param name="ip">Sender's IP</param>
        /// <param name="port">Sender's TCP server port</param>
        /// <param name="fileName">Name of the file</param>
        /// <returns>Local path to the downloaded file, or null if failed</returns>
        public static string Fetch(string ip, int port, string fileName)
        {
            try
            {
                using TcpClient client = new TcpClient();
                client.Connect(ip, port);

                using NetworkStream stream = client.GetStream();
                string savePath = Path.Combine(Path.GetTempPath(), fileName);

                using FileStream fs = File.Create(savePath);
                {
                    byte[] buffer = new byte[8192]; // 8 KB buffer
                    int bytesRead;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fs.Write(buffer, 0, bytesRead);
                    }
                }

                return savePath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FileFetcher] Error: {ex.Message}");
                return null;
            }
        }
    }
}
