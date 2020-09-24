using System;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ChatServer
{
    public class Startup
    {
        public static void Main(string[] args)
        {
            int numberOfClients;
            if (args.Length == 1 && args[0].Equals("server"))
            {
                Server.Server.StartServer();
                return;
            }
            else if (args.Length == 1 && args[0].Equals("client"))
            {
                Client.Client.StartClient();
                return;
            }
            else if (args.Length == 3 && args[0].Equals("server") && args[1].Equals("client") && int.TryParse(args[2], out numberOfClients))
            {
                StartNClients(numberOfClients);
                Server.Server.StartServer();
                return;
            }
            else if (args.Length == 2 && args[0].Equals("client") && int.TryParse(args[1], out numberOfClients))
            {
                StartNClients(numberOfClients);
                Thread.Sleep(Timeout.Infinite);
            }
            else
            {
                //ReportError();
                DataBaseChecking();
            }
        }

        private static void DataBaseChecking()
        {
            using (var ctx = new DataLayer.ChatServerContext())
            {
                ctx.Database.Delete();
                ctx.SaveChanges();
            }

            using (var ctx = new DataLayer.ChatServerContext())
            {
                var user = new User(new byte[] { Byte.MaxValue, Byte.MaxValue, Byte.MaxValue, Byte.MaxValue, Byte.MaxValue }, "Scott");
                var user2 = new User(new byte[] { Byte.MinValue, Byte.MinValue, Byte.MinValue, Byte.MinValue, Byte.MinValue }, "Pheobe");

                var chat = new Chat(0, "Gamer Chat");
                var chat2 = new Chat(0, "Work Chat");

                var message = new Transmissions.Message();
                message.ServerMessage = new Transmissions.ServerMessage() { _message = "hi this is the server".asBtyeMessage() };

                var message2 = new Transmissions.Message();
                message2.EncryptedMessage = new Transmissions.EncryptedMessage() { _user_id = user.ID, _message = "hi this is scott".asBtyeMessage() };

                chat.Messages = new[] { message };

                chat2.Messages = new[] { message2 };


                //ctx.Users = new[] { user);

                chat.Users = new[] { user, user2 };


                chat2.Users = new[] { user };

                ctx.Chats.Add(chat);
                ctx.Chats.Add(chat2);

                ctx.SaveChanges();

                System.Console.WriteLine($"chats: [\n  {String.Join(",\n  ", ctx.Chats.OrderBy(x => x.Name).Select(x => x.Name))}\n]\n\n");

                System.Console.WriteLine($"users: [\n  {String.Join(",\n  ", ctx.Users.Select(x => x.Name))}\n]\n\n");

                System.Console.WriteLine($"messages: [\n  {String.Join(",\n  ", ctx.Messages.OrderByDescending(x => x._createdAt).ToArray().Select(x => $"<{ctx.GetSenderNameForMessage(x)}: {x._createdAt.ToString("dd/MM/yyyy HH:mm:ss:ffff")}> {x.GetUnderlyingMessageContent().asStringMessage()}"))}\n]\n\n");

                System.Console.WriteLine($"chat messages: [\n  {String.Join(",\n  ", ctx.Chats.ToArray().Select(x => $"({x.Name}): [\n    {String.Join(",\n    ", x.Messages.OrderByDescending(x => x._createdAt).Select(x => $"<{ctx.GetSenderNameForMessage(x)}: {x._createdAt.ToString("dd/MM/yyyy HH:mm:ss:ffff")}> {x.GetUnderlyingMessageContent().asStringMessage()}"))}\n  ]"))}\n]\n\n");

                System.Console.WriteLine($"chat users: [\n  {String.Join(",\n  ", ctx.Chats.ToArray().Select(x => $"({x.Name}): [\n    {String.Join(",\n    ", x.Users.Select(x => x.Name))}\n  ]"))}\n]\n\n");


            }
        }

        private static void ReportError()
        {
            throw new Exception("Usage: 'server? client? <numberOfClients>?' - E.G: 'server client 1', or 'server', or 'client', or 'client'");
        }

        private static Process[] StartNClients(int N)
        {
            var returnArray = new Process[N];
            for (int i = 0; i < N; i++)
            {
                var newClientProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = Directory.GetCurrentDirectory() + "/bin/Debug/netcoreapp3.1/ChatServer.exe",
                        Arguments = "client",
                        UseShellExecute = true
                    }
                };
                newClientProcess.Start();
                returnArray[i] = newClientProcess;
            }
            return returnArray;
        }
    }
}
