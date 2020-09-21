using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ChatServer.Encryption
{
    public abstract class RSA
    {
        public static byte[] Sign(ReadOnlySpan<byte> privKey, byte[] data)
        {
            var rsa = new RSACryptoServiceProvider(2048);
            rsa.ImportRSAPrivateKey(privKey, out int i);
            var signature = rsa.SignData(data, SHA256.Create());

            var signedData = new byte[data.Length + 256];
            signature.CopyTo(signedData, 0);
            data.CopyTo(signedData, 256);

            return signedData;
        }

        public static bool Verify(ReadOnlySpan<byte> publicKey, byte[] signedData)
        {
            var rsa = new RSACryptoServiceProvider(2048);
            rsa.ImportRSAPublicKey(publicKey, out int i);

            var signature = new byte[256];
            Array.Copy(signedData, 0, signature, 0, 256);

            var data = new byte[signedData.Length - 256];
            Array.Copy(signedData, 256, data, 0, signedData.Length - 256);

            return rsa.VerifyData(data, SHA256.Create(), signature);
        }

        public static byte[] RemoveSignature(byte[] signedData)
        {
            var data = new byte[signedData.Length - 256];
            Array.Copy(signedData, 256, data, 0, signedData.Length - 256);
            return data;
        }

        public static RSAParameters GetStoredEncryptedKeyPair(string path, IEnumerable<byte> password, bool includePrivateParameters)
        {
            var rsa = new RSACryptoServiceProvider(2048);

            rsa.ImportCspBlob(AES.Decrypt(File.ReadAllBytes(path), password));

            return rsa.ExportParameters(includePrivateParameters);
        }

        public static void GenerateAndStoreNewEncryptedKeyPair(string path, IEnumerable<byte> password)
        {
            var rsa = new RSACryptoServiceProvider(2048);
            var keyBlob = rsa.ExportCspBlob(true);
            File.WriteAllBytes(path, AES.Encrypt(keyBlob, password));
        }
    }
}