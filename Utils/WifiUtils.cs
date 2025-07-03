using System.Diagnostics;
using System.Linq;

namespace Jiffy.Utils
{
    public static class WiFiUtils
    {
        public static string GetCurrentSSID()
        {
            try
            {
                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "netsh",
                        Arguments = "wlan show interfaces",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                // Match only SSID (not BSSID or other similar keys)
                var lines = output.Split('\n');
                var ssidLine = lines
                    .FirstOrDefault(line =>
                        line.TrimStart().StartsWith("SSID", StringComparison.OrdinalIgnoreCase) &&
                        !line.TrimStart().StartsWith("BSSID", StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrWhiteSpace(ssidLine))
                {
                    var parts = ssidLine.Split(':');
                    if (parts.Length > 1)
                        return parts[1].Trim();
                }

                return "unknown";
            }
            catch
            {
                return "unknown";
            }
        }
    }
}
