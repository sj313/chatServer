using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Linq;

using System.Collections.Concurrent;

namespace ChatServer
{
    public class Server
    {
        private static readonly Int32 PORT = 9;
        private static readonly IPAddress LOCAL_ADDRESS = IPAddress.Any;
        private static readonly TcpListener LISTENER = new TcpListener(LOCAL_ADDRESS, PORT);
        public static readonly ConcurrentBag<Connection> CONNECTIONS = new ConcurrentBag<Connection>();
        public static readonly TransmissionHandlerServer TRANSMISSION_HANDLER = new TransmissionHandlerServer();
        private static readonly UIController UICONTROLLER = new UIController();

        public static readonly string SERVER_NAME = "<SuperBot 5000>";

        public static void StartServer()
        {
            //UICONTROLLER.Display += (x) => { System.Console.WriteLine($"<{x.Author.Name}>: {x._message}"); };
            //System.Console.WriteLine(Dns.GetHostName());

            Task processTransmisisons = new Task(() => { TRANSMISSION_HANDLER.OnTransmissionRecieved(); });
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

        static void LookForConnections()
        {
            try
            {
                LISTENER.Start(1000);
                //UICONTROLLER.Display(new Message(SERVER_USER, "Starting up..."));
                while (true)
                {
                    //UICONTROLLER.Display(new Message(SERVER_USER, "Looking for connections..."));
                    var newClient = LISTENER.AcceptTcpClient();
                    //UICONTROLLER.Display(new Message(SERVER_USER, $"Connection made!"));
                    new Task(() => Onboard(newClient)).Start();
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
            TRANSMISSION_HANDLER.Send(newConnection, "Sup, welcome to this awesome chat server!");
            TRANSMISSION_HANDLER.SendOnboardRequest(newConnection, "???");
            CONNECTIONS.Add(newConnection);
        

            //UICONTROLLER.Display(new Message(SERVER_USER, $"New user: '{username}' added"));

            // new Task(() => { TRANSMISSION_HANDLER.Send(CONNECTIONS, $"------------------------------ '{username}' has entered the chat ------------------------------"); }).Start();
            // new Task(() => { TRANSMISSION_HANDLER.RecieveFrom(newUser); }).Start();
        }
    }
}
