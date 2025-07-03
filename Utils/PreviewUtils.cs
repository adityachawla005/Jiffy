using System;
using System.IO;
using System.Linq;

namespace Jiffy.Utils
{
    public static class PreviewUtils
    {
        private static readonly string[] SafeExtensions = new[]
        {
            ".txt", ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".pdf"
        };

        public static bool IsSafeToPreview(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            var ext = Path.GetExtension(filePath).ToLowerInvariant();
            return SafeExtensions.Contains(ext);
        }
    }
}
