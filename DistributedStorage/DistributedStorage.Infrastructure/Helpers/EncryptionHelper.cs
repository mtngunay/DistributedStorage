using System.Security.Cryptography;
using System.Text;

namespace DistributedStorage.Infrastructure.Helpers
{
    public static class EncryptionHelper
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("N2s4W9p8Q1r7U5o6L0x3JzCvTyEbKdFm");
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("A1b2C3d4E5f6G7hm");

        public static byte[] Encrypt(byte[] data)
        {
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;
            using var encryptor = aes.CreateEncryptor();
            return PerformCryptography(data, encryptor);
        }

        public static byte[] Decrypt(byte[] data)
        {
            using var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;
            using var decryptor = aes.CreateDecryptor();
            return PerformCryptography(data, decryptor);
        }

        private static byte[] PerformCryptography(byte[] data, ICryptoTransform cryptoTransform)
        {
            using var ms = new MemoryStream();
            using (var cryptoStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
            }
            return ms.ToArray();
        }
    }
}