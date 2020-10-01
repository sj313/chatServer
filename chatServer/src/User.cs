using System.Security.Cryptography;
using System.Collections.Generic;
namespace ChatServer
{
    public class User
    {
        public string Name { get; set; }
        public byte[] ID { get; set; }

        public User(byte[] id, string name)
        {
            Name = name;
            ID = id;
        }

    }
}