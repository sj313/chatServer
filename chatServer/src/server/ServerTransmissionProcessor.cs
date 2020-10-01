using System;
using System.Linq;
using ChatServer.Transmissions;

namespace ChatServer.Server
{
    public abstract class ServerTransmissionProcessor
    {
        // Main function to process an incomming Transmission
        public static void Process(Tuple<Transmission, Connection> transmission)
        {
            Console.WriteLine($"{transmission.Item2.ConnectionID} (Connection): Processing Transmission");
            // Verify Transmission includes SenderID
            if (transmission.Item1.SenderID == null)
            {
                // TODO: Process error
                return;
            }
            // Verify Transmission is correctly signed
            if (!transmission.Item1.Verify())
            {
                //TODO: Process error
                return;
            }
            // Process as Message if Transmission contains Message and Connection has joined succesfully
            if (transmission.Item1.Message != null && transmission.Item2.Joined)
            {
                ProcessMessage(transmission);
                return;
            }
            // Process as Request if Transmission contains Request
            if (transmission.Item1.Request != null)
            {
                ProcessRequest(transmission);
                return;
            }
            // Process as Response if Transmission contains Response
            if (transmission.Item1.Response != null)
            {
                ProcessResponse(transmission);
                return;
            }
        }

        private static void ProcessMessage(Tuple<Transmission, Connection> transmission)
        {
            //TODO: Check permissions
            var message = transmission.Item1.Message;
            // Rebroadcast if Message is an Encryptedmessage
            if (message.EncryptedMessage != null)
            {
                Rebroadcast(transmission);
            }
            // Throw error if Message is a ServerMessage (Client should never send a ServerMessage)
            if (message.ServerMessage != null)
            {
                //TODO: Process error
                return;
            }
        }

        private static void ProcessRequest(Tuple<Transmission, Connection> transmission)
        {
            if (transmission.Item1.Request.JoinRequest != null)
            {
                ProcessJoinRequest(transmission);
            }
        }

        private static void ProcessResponse(Tuple<Transmission, Connection> transmission)
        {
            Console.WriteLine($"{transmission.Item2.ConnectionID} (Connection): Processing Response");
            // Clients do not currently send responses
            return;
        }

        private static void Rebroadcast(Tuple<Transmission, Connection> transmission)
        {
            Console.WriteLine($"{transmission.Item2.ConnectionID} (Connection): Processing Rebroadcast to {transmission.Item1.Message.ChatID} (Chat)");
            ServerTransmissionHandler.Send(transmission.Item1.Message.ChatID, transmission.Item1);
        }

        private static void ProcessJoinRequest(Tuple<Transmission, Connection> transmission)
        {
            Console.WriteLine($"{transmission.Item2.ConnectionID} (Connection): Processing Join Request");

            var connection = transmission.Item2;
            var userID = transmission.Item1.SenderID.ToByteArray();

            // Update connection User data
            // TODO: Set name if already exists in DB
            connection.User = new User(userID, (new Guid(userID.Skip(16).Take(16).ToArray())).ToString());

            // Check to see if the User is currently connected on any other Connections
            var firstConnection = !ServerConnections.USER_CONNECTIONS.ContainsKey(userID);

            // Add Connection to relevant collections
            ServerConnections.Add(connection);
            // Temp
            // Add User to chat 1 by default
            ServerConnections.AddToChat(userID, 1);
            
            // Send join message if this is the first Connection for this User
            if (firstConnection)
            {
                ServerTransmissionHandler.Send(userID, $"-------------------User: {connection.User.Name} has joined--------------------");
            }

            // Update Connection to show succesful join
            connection.Joined = true;
            // Send success Response
            ServerTransmissionHandler.SendResponse(connection, Errors.Error.NoError);
            // Send success ServerMessage
            ServerTransmissionHandler.Send(connection, "SERVER: Join Successful");
            // Send user count ServerMessage
            ServerTransmissionHandler.Send(connection, $"SERVER: {ServerConnections.USER_CONNECTIONS.Keys.Count} users currently online");
        
        }
    }
}