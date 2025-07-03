using System.IO;
using System.Security.Cryptography;

namespace Jiffy.Security
{
    public static class FileHasher
    {
        public static string ComputeSHA256(string filePath)
        {
            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(filePath);
            var hash = sha256.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
