using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ChatServer.Encryption
{
    public abstract class AES
    {
        private const int KEY_SIZE = 256; 
        private const int BLOCK_SIZE = 128; 
        private static readonly byte[] SALT = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16};
        private const int ITERATIONS = 1000;
    
        public static byte[] Encrypt(IEnumerable<byte> plaintext, IEnumerable<byte> password)
        {
            RijndaelManaged aes = Setup(KEY_SIZE, BLOCK_SIZE, ITERATIONS, SALT, SHA256.Create().ComputeHash(password.ToArray()));
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

        public static byte[] Decrypt(IEnumerable<byte> ciphertext, IEnumerable<byte> password)
        {
            RijndaelManaged aes = Setup(KEY_SIZE, BLOCK_SIZE, ITERATIONS, SALT, SHA256.Create().ComputeHash(password.ToArray()));
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

        private static RijndaelManaged Setup(int keySize, int BlockSize, int iterations, byte[] salt, byte[] passwordHash)
        {
            RijndaelManaged aes = new RijndaelManaged();

            aes.KeySize = keySize;
            aes.BlockSize = BlockSize;
            aes.Key = new Rfc2898DeriveBytes(passwordHash, salt, iterations).GetBytes(keySize/8);
            // Defaults
            // aes.Mode = CipherMode.CBC;
            // aes.Padding = PaddingMode.PKCS7;
            return aes;
        }
    }
}
