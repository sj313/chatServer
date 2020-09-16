using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;


namespace ChatServer
{
    public class Client
    {
        private static readonly Int32 PORT = 10;
        private static readonly IPAddress HOSTNAME = IPAddress.Parse("192.168.187.163");
        private User USER = new User(new TcpClient());
        private MessageHandler MESSAGE_HANDLER = new MessageHandler();

        private UIController UICONTROLLER = new UIController();

        public static void Main(string[] args)
        {
            Client thisClient = new Client();
            thisClient.UICONTROLLER.Display += (x) =>
            {
                if (x.Author.Name.Equals(thisClient.USER.Name))
                {
                    System.Console.WriteLine($"<You>: {x._message}");
                }
                else
                {
                    System.Console.WriteLine($"<{x.Author.Name}>: {x._message}");

                }
            };
            thisClient.UICONTROLLER.Input += () =>
            {
                var returnMessage = new Message(thisClient.USER, Console.ReadLine());

                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, Console.CursorTop);
                return returnMessage;

            };

            thisClient.ConnectToHost();
            try
            {
                Task recievingMessages = new Task(() => { thisClient.MESSAGE_HANDLER.RecieveFrom(thisClient.USER.getStream()); });
                recievingMessages.Start();

                Task displayingMessages = new Task(() =>
                {
                    thisClient.MESSAGE_HANDLER.OnMessageRecieved((x) =>
                    {
                        thisClient.UICONTROLLER.Display(x);
                        if (x._message.Contains("Your username is: '"))
                        {
                            thisClient.USER.Name = x._message.Split('\'')[1];
                        }
                    });
                });
                displayingMessages.Start();

                Task gettingInput = new Task(() => { thisClient.UICONTROLLER.getInput((x) => { thisClient.MESSAGE_HANDLER.Send(thisClient.USER, x); }); });
                gettingInput.Start();



                recievingMessages.Wait();
                gettingInput.Wait();
                displayingMessages.Wait();

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                thisClient.USER.Connection.Dispose();
            }

        }

        void ConnectToHost()
        {
            try
            {
                USER.Connection.Connect(HOSTNAME, PORT);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }

        void getUserName()
        {
            try
            {
                USER.Connection.Connect(HOSTNAME, PORT);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }
    }
}
