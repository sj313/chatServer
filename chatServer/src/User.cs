using System.Security.Cryptography;

namespace ChatServer
{
    public class User
    {
        public string Name;
        public byte[] ID;


        //Temp constructer with autogenerated ID
        public User() : this((new RSACryptoServiceProvider(2048)).ExportRSAPublicKey(), null) {}

        public User(byte[] id, string name)
        {
            Name = name;
            ID = id;
        }
    }
}