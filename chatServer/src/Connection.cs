using System;
using System.Net.Sockets;

namespace ChatServer
{
    public class Connection
    {

        public byte[] SessionKey;
        public TcpClient TCPClient = new TcpClient();

        public Guid ConnectionID = new Guid();

        public bool Onboarded = false;
        public User User = new User();

        public Connection(TcpClient tcpClient, byte[] sessionKey)
        {
            TCPClient = tcpClient;
            SessionKey = sessionKey;
        }

    }
}
