using System;
using System.Net.Sockets;

namespace ChatServer
{
    public class Connection
    {

        public byte[] SessionKey;
        public TcpClient TCPClient = new TcpClient();

        public Guid ConnectionID = Guid.NewGuid();

        public bool Joined = false;
        public User User;

        public Connection(TcpClient tcpClient)
        {
            TCPClient = tcpClient;
        }
        public Connection(TcpClient tcpClient, byte[] sessionKey)
        {
            TCPClient = tcpClient;
            SessionKey = sessionKey;
        }

    }
}
