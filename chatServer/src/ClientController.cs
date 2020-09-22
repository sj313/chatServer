using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;
using System.Linq;
using System.Reflection;



namespace ChatServer
{
    public class ClientController
    {
        private const string COMMAND_WORD = "cmd";
        private static readonly Regex hasCommandPrefix = new Regex($"^{COMMAND_WORD} ");
        private static readonly Regex hasCommandAndAction = new Regex($"^{COMMAND_WORD} .+");
        private static readonly Regex hasCommandAndActionAndParams = new Regex($"^{COMMAND_WORD} .+ .+");

        // commands come in the form:
        //              'cmd <action> <params>' i.e: 'cmd <action> <param1> <param2>
        //       OR     'cmd <action>'
        //       OR     '<message>'

        private Client Client;

        public ClientController(Client client)
        {
            this.Client = client;
            this.OnBoard();
        }

        private void OnBoard()
        {
            //send onboarding request to the server
        }

        public void ParseCommand(string command)
        {
            if (hasCommandAndAction.IsMatch(command))
            {
                string[] parts = Regex.Split(command, " ");
                string action = parts[1];

                string[] messageParams;
                if (hasCommandAndActionAndParams.IsMatch(command))
                {
                    messageParams = parts.Skip(2).ToArray();
                    //pairing up any things that start with " and end with " so params can have spaces
                    var openQuotePositions = messageParams.Where(x => x.StartsWith("\"")).Select(x => Array.IndexOf(messageParams, x));
                    var closingQuotePositions = messageParams.Where(x => x.EndsWith("\"")).Select(x => Array.IndexOf(messageParams, x));

                    messageParams = openQuotePositions.Zip(closingQuotePositions, (x, y) => String.Join(" ", messageParams.Skip(x).Take(y - x + 1))).ToArray();
                }

                /////////////////match the function////////////////////////
            }
            else if (hasCommandPrefix.IsMatch(command))
            {
                ThrowCommandError($"An action must come after '{COMMAND_WORD}'. Use '${COMMAND_WORD} actions' to see available actions");
            }
            else
            {
                SendToChat(command);
            }
        }

        private void ThrowCommandError(string? message)
        {
            if (message == null)
            {
                //do default thing
            }
            else
            {
                //do thing with message
            }
        }

        private void SendToChat(string message)
        {
            //this.Client.MESSAGE_HANDLER.Send(message);
        }
    }
}
