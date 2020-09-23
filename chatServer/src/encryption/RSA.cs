using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ChatServer.Encryption
{
    public abstract class RSA
    {
        public static byte[] Sign(byte[] privKey, byte[] data)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
               
                var signature = CreateSignature(privKey, data);

                var signedData = new byte[data.Length + 256];
                signature.CopyTo(signedData, 0);
                data.CopyTo(signedData, 256);

                return signedData;
            }
        }
        
        public static byte[] CreateSignature(byte[] privKey, byte[] data)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.ImportRSAPrivateKey(privKey, out int i);
                var signature = rsa.SignData(data, SHA256.Create());
                return signature;
            }
        }

        public static bool Verify(byte[] publicKey, byte[] signedData)
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.ImportRSAPublicKey(publicKey, out int i);
                return rsa.VerifyData(GetData(signedData), SHA256.Create(), GetSignature(signedData));
            }
        }

        public static byte[] GetData(byte[] signedData)
        {
            var data = new byte[signedData.Length - 256];
            Array.Copy(signedData, 256, data, 0, signedData.Length - 256);
            return data;
        }

        public static byte[] GetSignature(byte[] signedData)
        {
            return signedData.Take(256).ToArray();
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