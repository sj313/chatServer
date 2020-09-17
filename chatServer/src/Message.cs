using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;


namespace ChatServer
{
    public class Message
    {
        public User Author;
        public string _message;

        public Message(User author, string message)
        {
            this.Author = author;
            this._message = message;
        }

        Message(SeralizedMessage tempMessage) : this(new User(tempMessage.AuthorName), tempMessage._message)
        {
        }

        public byte[] Serialize()
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, new SeralizedMessage(Author.Name, this._message));
                return stream.ToArray();
            }
        }

        public static Message Deserialize(byte[] serialisedMessage)
        {
            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(serialisedMessage))
            {
                var tempMessage = (SeralizedMessage)formatter.Deserialize(stream);
                return new Message(tempMessage);
            }
        }
    }

    [Serializable]
    class SeralizedMessage
    {
        public string AuthorName;
        public string _message;

        public SeralizedMessage(string authorName, string message)
        {
            this.AuthorName = authorName;
            this._message = message;
        }
    }

}


