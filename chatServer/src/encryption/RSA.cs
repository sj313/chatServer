using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ChatServer.Encryption
{

    abstract class RSA
    {
            private static byte[] SignAndEncryptMessage(RSAParameters privKey, IEnumerable<byte> message, IEnumerable<byte> password) {
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(privKey);

            var signature = rsa.SignData(message.ToArray(), SHA256.Create());

            var combined = new List<byte>();
            combined.AddRange(signature);
            combined.AddRange(message);

            return AES.Encrypt(combined, password);
        }
        public static RSAParameters GetStoredEncryptedKeyPair(string path, IEnumerable<byte> password, bool includePrivateParameters)
        {
            var rsa = new RSACryptoServiceProvider();

            rsa.ImportCspBlob(AES.Decrypt(File.ReadAllBytes(path), password));

            return rsa.ExportParameters(includePrivateParameters);
        }

        static void GenerateAndStoreNewEncryptedKeyPair(string path, IEnumerable<byte> password)
        {
            var rsa = new RSACryptoServiceProvider(2048);
            var keyBlob = rsa.ExportCspBlob(true);
            File.WriteAllBytes(path, AES.Encrypt(keyBlob, password));
        }
    }
}