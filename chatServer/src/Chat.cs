using System;
using System.Collections.Generic;
namespace ChatServer
{
    public class Chat
    {
        public Int64 ID { get; set; }
        public string Name { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Transmissions.Message> Messages { get; set; }

        public Chat(Int64 Id, string name)
        {
            this.ID = Id;
            this.Name = name;
        }

        public Chat() { }

    }
}
