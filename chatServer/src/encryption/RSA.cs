using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ChatServer.Encryption
{
    public abstract class RSA
    {
        public static byte[] SignAndEncryptMessage(RSAParameters privKey, IEnumerable<byte> message, IEnumerable<byte> password) {
            var RSA = new RSACryptoServiceProvider();
            RSA.ImportParameters(privKey);

            var signature = RSA.SignData(message.ToArray(), SHA256.Create());

            var combined = new List<byte>();
            combined.AddRange(signature);
            combined.AddRange(message);

            return AES.Encrypt(combined, password);
        }
        public static RSAParameters GetStoredEncryptedKeyPair(string path, IEnumerable<byte> password, bool includePrivateParameters)
        {
            var RSA = new RSACryptoServiceProvider();

            RSA.ImportCspBlob(AES.Decrypt(File.ReadAllBytes(path), password));

            return RSA.ExportParameters(includePrivateParameters);
        }

        public static void GenerateAndStoreNewEncryptedKeyPair(string path, IEnumerable<byte> password)
        {
            var RSA = new RSACryptoServiceProvider(2048);
            var keyBlob = RSA.ExportCspBlob(true);
            File.WriteAllBytes(path, AES.Encrypt(keyBlob, password));
        }
    }
}