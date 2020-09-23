namespace ChatServer.Encryption
{
    public abstract class KeyStorage
    {
        public static System.Security.Cryptography.RSACryptoServiceProvider GetKeys(string keyPath, byte[] keyPass)
        {
            if (!System.IO.File.Exists(keyPath))
                RSA.GenerateAndStoreNewEncryptedKeyPair(keyPath, keyPass);
            return RSA.GetStoredEncryptedKeyPair(keyPath, keyPass);
        }
    
    }
}