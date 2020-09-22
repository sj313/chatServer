using System.Net.Sockets;

namespace ChatServer
{
    public class Connection
    {

        public byte[] SessionKey;
        public TcpClient TCPConnection = new TcpClient();

        public bool Onboarded = false;
        public User User = new User(null);

        public Connection(TcpClient connection)
        {
            TCPConnection = connection;
        }

    }
}
