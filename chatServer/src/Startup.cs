using System;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
namespace ChatServer
{
    public class Startup
    {
        public static void Main(string[] args)
        {
            int numberOfClients;
            if (args.Length == 1 && args[0].Equals("server"))
            {
                Server.StartServer();
                return;
            }
            else if (args.Length == 1 && args[0].Equals("client"))
            {
                Client.StartClient();
                return;
            }
            else if (args.Length == 3 && args[0].Equals("server") && args[1].Equals("client") && int.TryParse(args[2], out numberOfClients))
            {
                StartNClients(numberOfClients);
                Server.StartServer();
                return;
            }
            else if (args.Length == 2 && args[0].Equals("client") && int.TryParse(args[1], out numberOfClients))
            {
                StartNClients(numberOfClients);
                Thread.Sleep(Timeout.Infinite);
            }
            else
            {
                ReportError();
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
