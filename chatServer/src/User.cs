using System.Security.Cryptography;
using System.Collections.Generic;
namespace ChatServer
{
    public class User
    {
        public string Name { get; set; }
        public byte[] ID { get; set; }

        public virtual ICollection<Chat> Chats { get; set; }

        //Temp constructer with autogenerated ID
        public User() : this((new RSACryptoServiceProvider(2048)).ExportRSAPublicKey(), null) { }

        public User(byte[] id, string name)
        {
            Name = name;
            ID = id;
        }

    }
}