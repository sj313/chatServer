using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;

namespace ChatServer
{
    public abstract class Client
    {
        private const Int32 PORT = 9;
        private static readonly string HOSTNAME = Dns.GetHostName();
        public static readonly User USER = new User();
        public static Connection Connection;
        //Temp
        private static readonly byte[] KeyPass = Encoding.UTF8.GetBytes("ClientKeyPassword");
        private const string KEY_PATH = @"..\resources\.clientKeys";
        public static readonly byte[] SERVER_PASS = Encoding.UTF8.GetBytes("ServerPassword");
        public static readonly RSACryptoServiceProvider CLIENT_KEYS = Encryption.KeyStorage.GetKeys(KEY_PATH, KeyPass);

        public static void StartClient()
        {
            UIController.Display += (x) => Console.WriteLine(x);
            Console.WriteLine("Please Enter Your Password");
            // var keyPass = Encoding.UTF8.GetBytes(Console.ReadLine());
            USER.ID = CLIENT_KEYS.ExportRSAPublicKey();
            USER.Name = "Test";
            AttemptConnect();

            // var CLEAR_AFTER = 3000;
            // UICONTROLLER.Display += (x) =>
            // {
            //     clearLines(Console.CursorTop, Console.CursorTop);
            //     Console.SetCursorPosition(0, Console.CursorTop);

            //     if (x.Author.Name.Equals(TCP_CLIENT.Name))
            //     {
            //         System.Console.Write($"<You>: {x._message}\n\n");
            //     }
            //     else
            //     {
            //         System.Console.Write($"<{x.Author.Name}>: {x._message}\n\n");
            //     }
            //     System.Console.Write("<You>: ");
            // };

            UIController.Input += () =>
            {
                var inputLine = Console.ReadLine();

                // inputLine = inputLine.StartsWith(PREFIX) ? inputLine.Substring(PREFIX.Length) : inputLine;
                // var returnMessage = new Message(TCP_CLIENT, inputLine);
                // clearLines(Console.CursorTop - 1, Console.CursorTop - 1);
                // Console.SetCursorPosition(0, Console.CursorTop - 1);
                // return returnMessage;
                return inputLine;
            };

            try
            {
                Task recievingMessages = new Task(() => { ClientTransmissionHandler.RecieveFrom(); });

                Task displayingMessages = new Task(() =>
                {
                    ClientTransmissionHandler.OnTransmissionRecieved();
                });

                Task gettingInput = new Task(() => { UIController.getInput((x) => { ClientTransmissionHandler.Send(x); }); });

                displayingMessages.Start();
                gettingInput.Start();
                recievingMessages.Start();

                displayingMessages.Wait();
                gettingInput.Wait();
                recievingMessages.Wait();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                Connection.TCPClient.Dispose();
            }

        }

        private static void AttemptConnect()
        {
            var success = false;
            var tcpClient = new TcpClient();

            while (!success)
            {
                UIController.Display("Attempting to connect...");
                try
                {
                    tcpClient.Connect(HOSTNAME, PORT);
                    UIController.Display("Connected Succesfully, Establishing session key...");
                    Connection = new Connection(tcpClient, Encryption.DiffieHellman.GetSharedKey(tcpClient));
                    UIController.Display("Session key established");
                    success = true;

                }
                catch (SocketException e)
                {
                    UIController.Display("Connection failed, retrying in 1 second...");
                    System.Threading.Thread.Sleep(1000);
                    Console.WriteLine("SocketException: {0}", e);
                }
            }



        }

        // private static void clearLines(int start, int end)
        // {
        //     var (startX, startY) = (Console.CursorLeft, Console.CursorTop);
        //     for (int i = start; i <= end; i++)
        //     {
        //         Console.SetCursorPosition(0, i);
        //         Console.Write(new string(' ', Console.WindowWidth));
        //     }
        //     Console.SetCursorPosition(startX, startY);
        // }
    }
}
