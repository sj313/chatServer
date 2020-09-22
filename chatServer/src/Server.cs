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
            try
            {
                UICONTROLLER.Display += System.Console.WriteLine;

                Task rebroadcasting = new Task(() => MESSAGE_HANDLER.OnMessageRecieved(RebroadcastMessage));

                Task lookingForConenctions = new Task(LookForConnections);

                rebroadcasting.Start();
                lookingForConenctions.Start();

                lookingForConenctions.Wait();
                rebroadcasting.Wait();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                foreach (var user in USERS)
                {
                    user.Connection.Dispose();
                }
            }
        }

        static void LookForConnections()
        {
            try
            {
                LISTENER.Start(1000);
                UICONTROLLER.Display("Starting up...");
                while (true)
                {
                    UICONTROLLER.Display("Looking for connections...");
                    var newClient = LISTENER.AcceptTcpClient();
                    UICONTROLLER.Display("Connection made!");
                    new Task(() => OnBoard(newClient)).Start();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                LISTENER.Stop();
                UICONTROLLER.Display("Shutting down...");
            }
        }

        static void RebroadcastMessage(Message message)
        {
            UICONTROLLER.Display(message._message);

            foreach (var user in USERS)
            {
                var recipient = USERS.Where((x) => { return x.Name.Equals(user.Name); }).First();
                MESSAGE_HANDLER.Send(recipient, message);
            }
        }

        private static void OnBoard(TcpClient newClient)
        {
            User newUser = new User(newClient);

            string username = null;

            MESSAGE_HANDLER.Send(newUser, new Message(SERVER_USER, "Sup, welcome to this awesome chat server!"));
            do
            {
                if (username != null)
                {
                    MESSAGE_HANDLER.Send(newUser, new Message(SERVER_USER, "That cannot be used"));
                }
                MESSAGE_HANDLER.Send(newUser, new Message(SERVER_USER, "Enter a Username: "));

                Message inputMessage = MESSAGE_HANDLER.Recieve(newUser.getStream());
                username = inputMessage._message;
            } while ((USERS.Where((x) => { return x.Name.Equals(username); }).Count() > 0) || String.IsNullOrEmpty(username));

            newUser.Name = username;
            MESSAGE_HANDLER.Send(newUser, new Message(SERVER_USER, $"Your username is: '{username}'"));
            USERS.Add(newUser);

            UICONTROLLER.Display($"New user: '{username}' added");

            new Task(() => { RebroadcastMessage(new Message(SERVER_USER, $"------------------------------ '{username}' has entered the chat ------------------------------")); }).Start();
            new Task(() => { MESSAGE_HANDLER.RecieveFrom(newUser.getStream()); }).Start();
        }
    }
}
