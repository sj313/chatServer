using System;
using Xunit;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using ChatServer.Encryption;
using System.Collections.Generic;
using System.Linq;

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
            var ciphertextB = Encryption.AES.Encrypt(plaintextB, passwordB);
            Console.WriteLine(Encoding.Unicode.GetString(ciphertextB));
            plaintextB = Encryption.AES.Decrypt(ciphertextB, passwordB);
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
            
            Encryption.RSA.GenerateAndStoreNewEncryptedKeyPair(KEYBLOB_PATH, rsaPassword);

            using (var RSA = new RSACryptoServiceProvider())
            {
                RSA.ImportParameters(Encryption.RSA.GetStoredEncryptedKeyPair(KEYBLOB_PATH, rsaPassword, true));

                var message = Encoding.Unicode.GetBytes(MESSAGE);
                var messagePassword = Encoding.Unicode.GetBytes(MESSAGE_PASSWWORD);
                File.WriteAllBytes(MESSAGE_PATH, Encryption.RSA.SignAndEncryptMessage(RSA.ExportParameters(true), message, messagePassword));

                var decSignedMessage = new List<byte>(AES.Decrypt(File.ReadAllBytes(MESSAGE_PATH), messagePassword));

                var signature = decSignedMessage.Take(256);
                var decMessage = decSignedMessage.Skip(256).ToArray();

                Assert.Equal(message, decMessage);
                Assert.True(RSA.VerifyData(message, SHA256.Create(), signature.ToArray()));
            }
        }
    }
}
