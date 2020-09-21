using System;
using Xunit;
using System.Text;
using System.IO;
using ChatServer.Encryption;

namespace ChatServer.Tests
{
    public class EncryptionTests
    {
        [Fact]
        public void AESTest()
        {
            Console.WriteLine("With Bytes:");
            var plaintextB = Encoding.Unicode.GetBytes("Hello");
            var passwordB = Encoding.Unicode.GetBytes("Test");
            Console.WriteLine(Encoding.Unicode.GetString(plaintextB));
            var ciphertextB = AES.Encrypt(plaintextB, passwordB);
            Console.WriteLine(Encoding.Unicode.GetString(ciphertextB));
            plaintextB = AES.Decrypt(ciphertextB, passwordB);
            Console.WriteLine(Encoding.Unicode.GetString(plaintextB));
        }
        
        [Fact]
        public void RSATest()
        {
            const string KEYBLOB_PATH = @"..\..\..\..\resources\.keyblob";
            const string MESSAGE_PATH = @"..\..\..\..\resources\.message";

            const string RSA_PASSWORD = "TestPassword";
            const string MESSAGE_PASSWWORD = "TestPassword2";
            const string MESSAGE = "TestMessage";


            var rsaPassword = Encoding.Unicode.GetBytes(RSA_PASSWORD);
            
            RSA.GenerateAndStoreNewEncryptedKeyPair(KEYBLOB_PATH, rsaPassword);

            using (var rsa = new System.Security.Cryptography.RSACryptoServiceProvider())
            {
                rsa.ImportParameters(RSA.GetStoredEncryptedKeyPair(KEYBLOB_PATH, rsaPassword, true));

                var message = Encoding.Unicode.GetBytes(MESSAGE);
                var messagePassword = Encoding.Unicode.GetBytes(MESSAGE_PASSWWORD);
                var encMessage = AES.Encrypt(RSA.Sign(rsa.ExportRSAPrivateKey(), message), messagePassword);
                File.WriteAllBytes(MESSAGE_PATH, encMessage);

                var decSignedMessage = AES.Decrypt(File.ReadAllBytes(MESSAGE_PATH), messagePassword);

                Assert.Equal(message, RSA.RemoveSignature(decSignedMessage));
                Assert.True(RSA.Verify(rsa.ExportRSAPublicKey(), decSignedMessage));
            }
        }
    }
}
