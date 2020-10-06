using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;

namespace ChatServer.Server
{
    public class Server
    {
        private static readonly int PORT = 9;
        private static readonly IPAddress LOCAL_ADDRESS = IPAddress.Any;
        const string KEY_PATH = @"..\resources\.serverkeys";
        private static readonly byte[] KEY_PASS = Encoding.UTF8.GetBytes("ServerKeyPassword");
        public static RSACryptoServiceProvider SERVER_KEYS = Encryption.KeyStorage.GetKeys(KEY_PATH, KEY_PASS);

        public static void StartServer()
        {
            //UICONTROLLER.Display += (x) => { System.Console.WriteLine($"<{x.Author.Name}>: {x._message}"); };
            //System.Console.WriteLine(Dns.GetHostName());

            Task processTransmisisons = new Task(() => { ServerTransmissionHandler.OnTransmissionRecieved(); });
            processTransmisisons.Start();

            Task lookingForConenctions = new Task(LookForConnections);
            lookingForConenctions.Start();

            lookingForConenctions.Wait();
            processTransmisisons.Wait();

            foreach (var connection in ServerConnections.CONNECTIONS)
            {
                ServerConnections.Remove(connection.Value);
            }
        }

        private static void LookForConnections()
        {
            var tcpListener = new TcpListener(LOCAL_ADDRESS, PORT);
            try
            {
                tcpListener.Start(1000);
                //UICONTROLLER.Display(new Message(SERVER_USER, "Starting up..."));
                while (true)
                {
                    try
                    {
                        var connection = new Connection(tcpListener.AcceptTcpClient());
                        Console.WriteLine($"{connection.ConnectionID} (Connection): Connection Established");
                        connection.SessionKey = Encryption.DiffieHellman.GetSharedKey(connection.TCPClient);
                        Console.WriteLine($"{connection.ConnectionID} (Connection): Session Key Established");
                        ServerConnections.Add(connection);
                        new Task(() => { ServerTransmissionHandler.RecieveFrom(connection); }).Start();
                        
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Connection failed, Error: {0}", e);
                    }
                    
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                tcpListener.Stop();
                //UICONTROLLER.Display(new Message(SERVER_USER, "Shutting down..."));
            }
        }
    
    }
}
