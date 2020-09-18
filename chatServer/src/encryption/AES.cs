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
            RijndaelManaged AES = Setup(KEY_SIZE, BLOCK_SIZE, ITERATIONS, SALT, SHA256.Create().ComputeHash(password.ToArray()));
            AES.GenerateIV();

            using (var memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, AES.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    memoryStream.Write(AES.IV);
                    cryptoStream.Write(plaintext.ToArray(), 0, plaintext.Count());
                }
                return memoryStream.ToArray();
            }
        }

        public static byte[] Decrypt(IEnumerable<byte> ciphertext, IEnumerable<byte> password)
        {
            RijndaelManaged AES = Setup(KEY_SIZE, BLOCK_SIZE, ITERATIONS, SALT, SHA256.Create().ComputeHash(password.ToArray()));
            AES.IV = ciphertext.Take(16).ToArray();

            using (var memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, AES.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(ciphertext.Skip(16).ToArray(), 0, ciphertext.Count() - 16);
                }
                return memoryStream.ToArray();
            }
        }

        private static RijndaelManaged Setup(int keySize, int BlockSize, int iterations, byte[] salt, byte[] passwordHash)
        {
            RijndaelManaged AES = new RijndaelManaged();

            AES.KeySize = keySize;
            AES.BlockSize = BlockSize;
            AES.Key = new Rfc2898DeriveBytes(passwordHash, salt, iterations).GetBytes(keySize/8);
            // Defaults
            // AES.Mode = CipherMode.CBC;
            // AES.Padding = PaddingMode.PKCS7;
            return AES;
        }
    }
}
