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

        public User USER = new User(new TcpClient());
        public MessageHandler MESSAGE_HANDLER = new MessageHandler();

        public UIController UICONTROLLER = new UIController();

        private ClientController ClientController;

        private ManualResetEventSlim ONBOARDED = new ManualResetEventSlim(false);

        public Client()
        {
            this.UICONTROLLER.Display += Console.WriteLine;
            this.UICONTROLLER.Input += Console.ReadLine;

            try
            {
                Task recievingMessages = new Task(() => { this.MESSAGE_HANDLER.RecieveFrom(this.USER.getStream()); });

                Task displayingMessages = new Task(() =>
                {
                    this.MESSAGE_HANDLER.OnMessageRecieved((x) =>
                    {
                        this.UICONTROLLER.Display(x._message);
                    });
                });

                Task gettingInput = new Task(() => this.UICONTROLLER.getInput(ClientController.ParseCommand));

                this.ConnectToHost();
                displayingMessages.Start();
                gettingInput.Start();
                recievingMessages.Start();

                this.ClientController = new ClientController(this);

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
                this.USER.Connection.Dispose();
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
