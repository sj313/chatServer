using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Text;

namespace ChatServer
{
    public class Server
    {
        private static readonly Int32 PORT = 9;
        private static readonly IPAddress LOCAL_ADDRESS = IPAddress.Any;
        private static readonly TcpListener LISTENER = new TcpListener(LOCAL_ADDRESS, PORT);
        public static readonly ConcurrentBag<Connection> CONNECTIONS = new ConcurrentBag<Connection>();
        public static readonly string SERVER_NAME = "<SuperBot 5000>";
        const string keyPath = @"..\resources\.serverkeys";
        private static readonly byte[] keyPass = Encoding.UTF8.GetBytes("ServerKeyPassword");



        public static void StartServer()
        {
            ServerTransmissionHandler.SetKeys(keyPath, keyPass);
            //UICONTROLLER.Display += (x) => { System.Console.WriteLine($"<{x.Author.Name}>: {x._message}"); };
            //System.Console.WriteLine(Dns.GetHostName());

            Task processTransmisisons = new Task(() => { ServerTransmissionHandler.OnTransmissionRecieved(); });
            processTransmisisons.Start();

            Task lookingForConenctions = new Task(LookForConnections);
            lookingForConenctions.Start();

            lookingForConenctions.Wait();
            processTransmisisons.Wait();

            foreach (var user in CONNECTIONS)
            {
                user.TCPConnection.Dispose();
            }
        }

        private static void LookForConnections()
        {
            try
            {
                LISTENER.Start(1000);
                //UICONTROLLER.Display(new Message(SERVER_USER, "Starting up..."));
                while (true)
                {
                    Console.WriteLine("Looking for connections...");
                    //UICONTROLLER.Display(new Message(SERVER_USER, "Looking for connections..."));
                    var newClient = LISTENER.AcceptTcpClient();
                    Console.WriteLine("Connected!");
                    //UICONTROLLER.Display(new Message(SERVER_USER, $"Connection made!"));
                    Onboard(newClient);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                LISTENER.Stop();
                //UICONTROLLER.Display(new Message(SERVER_USER, "Shutting down..."));
            }
        }

        private static void Onboard(TcpClient newClient)
        {
            Connection newConnection = new Connection(newClient);
            ServerTransmissionHandler.Send(newConnection, "Sup, welcome to this awesome chat server!");
            ServerTransmissionHandler.SendOnboardRequest(newConnection, "???");
            CONNECTIONS.Add(newConnection);
            Console.WriteLine("Onboard Complete");
            new Task(() => { ServerTransmissionHandler.RecieveFrom(newConnection); }).Start();
        

            //UICONTROLLER.Display(new Message(SERVER_USER, $"New user: '{username}' added"));

            // new Task(() => { TRANSMISSION_HANDLER.Send(CONNECTIONS, $"------------------------------ '{username}' has entered the chat ------------------------------"); }).Start();
            // 
        }
    }
}
