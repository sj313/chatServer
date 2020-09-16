using System;
using System.Net;
using System.Net.Sockets;

namespace ChatServer
{
    public class User
    {
        public string Name;
        public TcpClient Connection = new TcpClient();

        public User(TcpClient connection, string name)
        {
            this.Connection = connection;
            this.Name = name;
        }

        public User(string name) : this(null, name)
        {
        }

        public User(TcpClient connection) : this(connection, null)
        {
        }

        // void getName()
        // {
        //     this.sendMessage(new Message());

        // }

        // public void sendMessage(Message message)
        // {
        //     this.Connection.GetStream().Write(message.asBytes());
        // }

        // public string recieveMessage()
        // {

        // }

        public NetworkStream getStream()
        {
            return this.Connection.GetStream();
        }
    }
}
