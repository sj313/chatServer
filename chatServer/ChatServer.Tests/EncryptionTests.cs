using Xunit;
using System.Text;
using ChatServer.Encryption;

namespace ChatServer.Tests
{
    public class EncryptionTests
    {
        [Fact]
        public void AESTest()
        {
            var plaintext = Encoding.Unicode.GetBytes("Plaintext");
            var password = Encoding.Unicode.GetBytes("Password");
            var ciphertext = AES.Encrypt(plaintext, password);
            Assert.Equal(plaintext, AES.Decrypt(ciphertext, password));
        }
        
        [Fact]
        public void RSATest()
        {
            const string KEYBLOB_PATH = @"..\..\..\..\resources\.keyblob";

            const string RSA_PASSWORD = "TestPassword";
            const string MESSAGE = "TestMessage";


            var rsaPassword = Encoding.Unicode.GetBytes(RSA_PASSWORD);
            
            RSA.GenerateAndStoreNewEncryptedKeyPair(KEYBLOB_PATH, rsaPassword);

            var rsa = RSA.GetStoredEncryptedKeyPair(KEYBLOB_PATH, rsaPassword);

            var message = Encoding.Unicode.GetBytes(MESSAGE);
            var signedMessage = RSA.Sign(rsa.ExportRSAPrivateKey(), message);


            Assert.Equal(message, RSA.GetData(signedMessage));
            Assert.True(RSA.Verify(rsa.ExportRSAPublicKey(), signedMessage));
        }
    }
}
