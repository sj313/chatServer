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
        private static readonly ConcurrentBag<User> USERS = new ConcurrentBag<User>();

        private static readonly MessageHandler MESSAGE_HANDLER = new MessageHandler();
        private static readonly UIController UICONTROLLER = new UIController();

        public static readonly string SERVER_NAME = "<SuperBot 5000>";
        public static readonly User SERVER_USER = new User(SERVER_NAME);

        public static void StartServer()
        {
            UICONTROLLER.Display += (x) => { System.Console.WriteLine($"<{x.Author.Name}>: {x._message}"); };
            //System.Console.WriteLine(Dns.GetHostName());


            Task rebroadcasting = new Task(() => { MESSAGE_HANDLER.OnMessageRecieved(RebroadcastMessage); });
            rebroadcasting.Start();

            Task lookingForConenctions = new Task(LookForConnections);
            lookingForConenctions.Start();

            lookingForConenctions.Wait();
            rebroadcasting.Wait();

            foreach (var user in USERS)
            {
                user.Connection.Dispose();
            }
        }

        static void LookForConnections()
        {
            try
            {
                LISTENER.Start(1000);
                UICONTROLLER.Display(new Message(SERVER_USER, "Starting up..."));
                while (true)
                {
                    UICONTROLLER.Display(new Message(SERVER_USER, "Looking for connections..."));
                    var newClient = LISTENER.AcceptTcpClient();
                    UICONTROLLER.Display(new Message(SERVER_USER, $"Connection made!"));
                    new Task(() => OnBoard(newClient)).Start();
                    //OnBoard(newClient);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                LISTENER.Stop();
                UICONTROLLER.Display(new Message(SERVER_USER, "Shutting down..."));
            }
        }

        static void RebroadcastMessage(Message message)
        {
            foreach (var user in USERS)
            {
                var recipient = USERS.Where((x) => { return x.Name.Equals(user.Name); }).First();
                MESSAGE_HANDLER.Send(recipient, message);
                UICONTROLLER.Display(message);
            }
        }

        private static void OnBoard(TcpClient newClient)
        {
            User newUser = new User(newClient);

            string username = string.Empty;

            MESSAGE_HANDLER.Send(newUser, new Message(SERVER_USER, "Sup, welcome to this awesome chat server!\n"));
            do
            {
                if (!username.Equals(string.Empty))
                {
                    MESSAGE_HANDLER.Send(newUser, new Message(SERVER_USER, "That Username is taken!"));
                }
                MESSAGE_HANDLER.Send(newUser, new Message(SERVER_USER, "Enter a Username: "));

                Message inputMessage = MESSAGE_HANDLER.Recieve(newUser.getStream());
                username = inputMessage._message;
            } while ((USERS.Where((x) => { return x.Name.Equals(username); }).Count() > 0));

            newUser.Name = username;
            MESSAGE_HANDLER.Send(newUser, new Message(SERVER_USER, $"Your username is: '{username}'"));
            USERS.Add(newUser);

            UICONTROLLER.Display(new Message(SERVER_USER, $"New user: '{username}' added"));

            new Task(() => { RebroadcastMessage(new Message(SERVER_USER, $"------------------------------ '{username}' has entered the chat ------------------------------")); }).Start();
            new Task(() => { MESSAGE_HANDLER.RecieveFrom(newUser.getStream()); }).Start();
        }
    }
}
