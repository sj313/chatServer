using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;



namespace ChatServer
{
    public class Client
    {
        private const Int32 PORT = 9;
        private static readonly string HOSTNAME = Dns.GetHostName();
        private Connection USER = new Connection(new TcpClient());
        private MessageHandler MESSAGE_HANDLER = new MessageHandler();

        private UIController UICONTROLLER = new UIController();

        private ManualResetEventSlim ONBOARDED = new ManualResetEventSlim(false);

        public static void StartClient()
        {
            var CLEAR_AFTER = 3000;
            Client thisClient = new Client();
            thisClient.UICONTROLLER.Display += (x) =>
            {
                clearLines(Console.CursorTop, Console.CursorTop);
                Console.SetCursorPosition(0, Console.CursorTop);

                if (x.Author.Name.Equals(thisClient.USER.Name))
                {
                    System.Console.Write($"<You>: {x._message}\n\n");
                }
                else
                {
                    System.Console.Write($"<{x.Author.Name}>: {x._message}\n\n");
                }
                System.Console.Write("<You>: ");
            };

            thisClient.UICONTROLLER.Input += () =>
            {
                var inputLine = Console.ReadLine();

                //inputLine = inputLine.StartsWith(PREFIX) ? inputLine.Substring(PREFIX.Length) : inputLine;
                var returnMessage = new Message(thisClient.USER, inputLine);
                clearLines(Console.CursorTop - 1, Console.CursorTop - 1);
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                return returnMessage;
            };

            try
            {
                Task recievingMessages = new Task(() => { thisClient.MESSAGE_HANDLER.RecieveFrom(thisClient.USER.TCPConnection.GetStream()); });

                Task displayingMessages = new Task(() =>
                {
                    thisClient.MESSAGE_HANDLER.OnMessageRecieved((x) =>
                    {
                        if (x._message.Contains("Your username is: '") && !thisClient.ONBOARDED.IsSet)
                        {
                            thisClient.USER.User.Name = x._message.Split('\'')[1];
                            new Task(() => { Thread.Sleep(CLEAR_AFTER); clearLines(0, 5); Console.SetWindowPosition(0, 6); }).Start();
                            thisClient.ONBOARDED.Set();
                        }
                        thisClient.UICONTROLLER.Display(x);
                    });
                });

                Task gettingInput = new Task(() => { thisClient.UICONTROLLER.getInput((x) => { thisClient.MESSAGE_HANDLER.Send(thisClient.USER, x); }); });

                thisClient.ConnectToHost();
                displayingMessages.Start();
                gettingInput.Start();
                recievingMessages.Start();

                recievingMessages.Wait();
                gettingInput.Wait();
                recievingMessages.Wait();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                thisClient.USER.TCPConnection.Dispose();
            }

        }

        void ConnectToHost()
        {
            try
            {
                USER.TCPConnection.Connect(HOSTNAME, PORT);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }

        private static void clearLines(int start, int end)
        {
            var (startX, startY) = (Console.CursorLeft, Console.CursorTop);
            for (int i = start; i <= end; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(new string(' ', Console.WindowWidth));
            }
            Console.SetCursorPosition(startX, startY);
        }
    }
}
