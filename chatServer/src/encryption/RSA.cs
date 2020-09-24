using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ChatServer.Encryption
{
    public abstract class RSA
    {
        public static byte[] CreateSignature(byte[] privKey, byte[] data)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.ImportRSAPrivateKey(privKey, out int i);
                var signature = rsa.SignData(data, SHA256.Create());
                return signature;
            }
        }

        public static bool Verify(byte[] publicKey, byte[] data, byte[] signature)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    rsa.ImportRSAPublicKey(publicKey, out int i);
                    return rsa.VerifyData(data, SHA256.Create(), signature);
                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.StackTrace);
                    return false;
                }
            }
        }

        public static RSACryptoServiceProvider GetStoredEncryptedKeyPair(string path, byte[] password)
        {
            var rsa = new RSACryptoServiceProvider(2048);
                rsa.ImportCspBlob(AES.Decrypt(File.ReadAllBytes(path), password));
                return rsa;
        }

        public static void GenerateAndStoreNewEncryptedKeyPair(string path, byte[] password)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                var keyBlob = rsa.ExportCspBlob(true);
                File.WriteAllBytes(path, AES.Encrypt(keyBlob, password));
            }
        }
    }
}