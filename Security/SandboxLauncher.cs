using System;
using System.Diagnostics;
using System.IO;

namespace Jiffy.Security
{
    public static class SandboxLauncher
    {
        public static void Launch(string filePath)
        {
            var file = new FileInfo(filePath);
            var sandboxTemplate = File.ReadAllText("sandbox-template.wsb");

            // Replace placeholders
            sandboxTemplate = sandboxTemplate
                .Replace("{FILE_PATH}", file.DirectoryName)
                .Replace("{FILENAME}", file.Name);

            // Save to temp .wsb
            string tempPath = Path.Combine(Path.GetTempPath(), $"launch_{Guid.NewGuid()}.wsb");
            File.WriteAllText(tempPath, sandboxTemplate);

            // Launch Windows Sandbox
            Process.Start("WindowsSandbox.exe", tempPath);
        }
    }
}
