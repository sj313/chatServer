using System.Net.Sockets;
using System.Security.Cryptography;

namespace ChatServer.Encryption
{    
    public abstract class DiffieHellman
    {
        public static byte[] GetSharedKey(TcpClient client)
        {
            var diffieHellman = new ECDiffieHellmanCng();
            var stream = client.GetStream();
            stream.Write(diffieHellman.PublicKey.ToByteArray(), 0, 140);
            var clientKey = new byte[140];
            stream.Read(clientKey, 0, 140);
            return diffieHellman.DeriveKeyMaterial(CngKey.Import(clientKey, CngKeyBlobFormat.EccPublicBlob));    
        }
    }
}