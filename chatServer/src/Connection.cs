using System.Net.Sockets;

namespace ChatServer
{
    public class Connection
    {

        public byte[] SessionKey = new byte[] {0};
        public TcpClient TCPConnection = new TcpClient();

        public bool Onboarded = false;
        public User User = new User();

        public Connection(TcpClient connection)
        {
            TCPConnection = connection;
        }

    }
}
