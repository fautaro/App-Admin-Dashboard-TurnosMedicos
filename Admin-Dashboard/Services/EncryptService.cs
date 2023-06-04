using System.Security.Cryptography;
using System.Text;

namespace Admin_Dashboard.Services
{
    public class EncryptService
    {

        private static readonly byte[] Key = Encoding.UTF8.GetBytes("abcdefghijklmnop"); // Clave de 16 bytes
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("1234567890abcdef"); // Vector de inicialización de 16 bytes


        public static string EncryptText(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                        cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                        cryptoStream.FlushFinalBlock();
                        byte[] encryptedBytes = memoryStream.ToArray();
                        return Convert.ToBase64String(encryptedBytes);
                    }
                }
            }
        }


        public static string DecryptText(string encryptedText)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        byte[] decryptedBytes = new byte[encryptedBytes.Length];
                        int decryptedByteCount = cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);
                        return Encoding.UTF8.GetString(decryptedBytes, 0, decryptedByteCount);
                    }
                }
            }
        }
    }
}
