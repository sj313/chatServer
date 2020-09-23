using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ChatServer.Encryption
{
    public abstract class AES
    {
        private const int KEY_SIZE = 256; 
        private static readonly byte[] SALT = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16};
        private const int ITERATIONS = 1000;
    
        public static byte[] Encrypt(byte[] plaintext, byte[] password)
        {
            using (var aes = Create(password))
            {
                aes.GenerateIV();         
                using (var memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        memoryStream.Write(aes.IV);
                        cryptoStream.Write(plaintext.ToArray(), 0, plaintext.Count());
                    }
                    return memoryStream.ToArray();
                }
            }
        }

        public static byte[] Decrypt(byte[] ciphertext, byte[] password)
        {
            using (var aes = Create(password))
            {
                aes.IV = ciphertext.Take(16).ToArray();
                using (var memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(ciphertext.Skip(16).ToArray(), 0, ciphertext.Count() - 16);
                    }
                    return memoryStream.ToArray();
                }
            }
        }

        private static Aes Create(byte[] password)
        {
            var aes =  Aes.Create();
            aes.KeySize = KEY_SIZE;
            aes.Key = new Rfc2898DeriveBytes(SHA256.Create().ComputeHash(password), SALT, ITERATIONS).GetBytes(KEY_SIZE/8);
            return aes;
        }
    }
}
