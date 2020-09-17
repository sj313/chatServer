using System;
using System.Threading;
using System.Diagnostics;
using System.IO;
namespace ChatServer
{
    public class Startup
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0].Equals("server"))
                {
                    Server.StartServer();
                    return;
                }
                else if (args[0].Equals("client"))
                {
                    int temp;
                    var numberOfClients = args.Length == 2 ? Int32.TryParse(args[1], out temp) ? temp : 1 : 1;

                    if (numberOfClients > 0)
                    {
                        var newClientProcess = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = Directory.GetCurrentDirectory() + "/bin/Debug/netcoreapp3.1/ChatServer.exe",
                                Arguments = $"{args[0]} {numberOfClients - 1}",
                                UseShellExecute = true
                            }
                        };
                        newClientProcess.Start();
                        Client.StartClient();
                    }
                    return;
                }
            }

            ReportError();
        }

        private static void ReportError()
        {
            throw new Exception("You have to supply the argument 'client <number>' or 'server', dumbass");
        }
    }
}
