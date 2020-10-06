using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using ChatServer.Transmissions;
using Google.Protobuf;

namespace ChatServer.Server
{
    public abstract class ServerTransmissionHandler
    {
        // A queue of all transmissions to process
        private static readonly ConcurrentQueue<Tuple<Transmission, Connection>> TransmissionsRecieved = new ConcurrentQueue<Tuple<Transmission, Connection>>();
        // Signal when a transmission is received
        private static readonly ManualResetEvent transmissionsRecievedSignal = new ManualResetEvent(false);

        // Creates a Message (ServerMessage)
        private static Message CreateMessage(Guid chatID, string messageString)
        {
            var messageBytes = Encoding.UTF8.GetBytes(messageString);
            var serverMessage = new ServerMessage(messageBytes);
            return new Message(chatID, serverMessage);
        }

        // Create a Transmission (Message)
        private static Transmission CreateTransmission(Message message)
        {
            return new Transmission(message, Server.SERVER_KEYS.ExportRSAPrivateKey(), Server.SERVER_KEYS.ExportRSAPublicKey());
        }

        // Create a Transmission (Request)
        private static Transmission CreateTransmission(Request request)
        {
            return new Transmission(request, Server.SERVER_KEYS.ExportRSAPrivateKey(), Server.SERVER_KEYS.ExportRSAPublicKey());
        }

        // Create a Transmission (Response)
        private static Transmission CreateTransmission(Response response)
        {
            return new Transmission(response, Server.SERVER_KEYS.ExportRSAPrivateKey(), Server.SERVER_KEYS.ExportRSAPublicKey());
        }
        
        // Send a Transmission to the specified connection
        private static void Send(Connection connection, Transmission transmission)
        {
            // Create EncryptedTransmission from the Transmission using the connections SessionKey
            var encryptedTransmission = new EncryptedTransmission(transmission, connection.SessionKey);
            try
            {
                // Attempt to write to the TCP stream
                var stream = connection.TCPClient.GetStream();
                var bytes = encryptedTransmission.ToByteArray();

                stream.Write(BitConverter.GetBytes((long)bytes.Length), 0, 8);
                stream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception e) when (e is ObjectDisposedException || e is InvalidOperationException)
            {
                // Error message if Transmisison failed to send
                Console.WriteLine("Sending failed!");
                Console.WriteLine(e.StackTrace);
            }
        }

        // Send a Transmission to a chat
        public static void Send(Guid chatID, Transmission transmission)
        {
            // Check that chat exists
            if (ServerConnections.CHAT_USERS.ContainsKey(chatID))
            {
                // Loop through all users in chat
                foreach (byte[] userID in ServerConnections.CHAT_USERS[chatID].Keys)
                {
                    // Check user is connected
                    if (ServerConnections.USER_CONNECTIONS.ContainsKey(userID))
                    {
                        // Loop through connections for user
                        foreach (Connection connection in ServerConnections.USER_CONNECTIONS[userID].Values)
                        {
                            // Create Transmission and send to connection
                            Send(connection, transmission);
                            Console.WriteLine($"Sent Transmission to connection {connection.ConnectionID}");
                        }
                    }
                }
            }
        }

        // Send server message to a chat
        public static void Send(Guid chatID, string messageString)
        {
            // Check that chat exists
            if (ServerConnections.CHAT_USERS.ContainsKey(chatID))
            {
                // Loop through all users in chat
                foreach (byte[] userID in ServerConnections.CHAT_USERS[chatID].Keys)
                {
                    // Check user is connected
                    if (ServerConnections.USER_CONNECTIONS.ContainsKey(userID))
                    {
                        // Loop through connections for user
                        foreach (Connection connection in ServerConnections.USER_CONNECTIONS[userID].Values)
                        {
                            // Create Transmission and send to connection
                            Send(connection, CreateTransmission(CreateMessage(chatID, messageString)));
                        }
                    }
                }
            }
        }

        // Send server message to all chats a user is in (Currently for connecting and disconnecting)
        public static void Send(byte[] userID, string messageString)
        {

            // Check user is in any chats
            if (ServerConnections.USER_CHATS.ContainsKey(userID))
            {
                // Send message to all chats user is in
               foreach (Guid chatID in ServerConnections.USER_CHATS[userID].Keys)
                {
                    Send(chatID, messageString);
                } 
            }

        }

        // Send ServerMessage to a connection (ChatID 0 is used to specify direct message)
        public static void Send(Connection connection, string messageString)
        {
            Send(connection, CreateTransmission(CreateMessage(new Guid(), messageString)));
        }

        // Send Request to connection
        private static void Send(Connection connection, Request request)
        {
            Send(connection, CreateTransmission(request));
        }

        // Send Response to connection
        private static void Send(Connection connection, Response response)
        {
            Send(connection, CreateTransmission(response));
        }

        // Send Response with error code to connection
        public static void SendResponse(Connection connection, Errors.Error error)
        {
            var response = new Response(error);
            Send(connection, CreateTransmission(response));
        }

        // Main loop to receive from a connection
        public static void RecieveFrom(Connection connection)
        {
            while (true)
            {
                try
                {
                    // Attempt to add new Transmission to queue
                    TransmissionsRecieved.Enqueue(new Tuple<Transmission, Connection>(Recieve(connection), connection));
                    // Signal Transmission has been received
                    transmissionsRecievedSignal.Set();
                }
                catch (System.IO.IOException)
                {
                    // Remove connection when error occurs
                    Console.WriteLine($"{connection.ConnectionID} (Connection): Disconnected (IOException)");
                    ServerConnections.Remove(connection);
                    return;
                }
                catch (System.Security.Cryptography.CryptographicException)
                {
                    // Remove connection when error occurs
                    Console.WriteLine($"{connection.ConnectionID} (Connection): Disconnected (CryptographicException)");
                    ServerConnections.Remove(connection);
                    return;
                }
            }
        }

        // Receive Transmission from Connection
        private static Transmission Recieve(Connection connection)
        {
            var stream = connection.TCPClient.GetStream();

            // Read data size from stream
            byte[] dataSize = new byte[8];
            stream.Read(dataSize, 0, 8);

            // Read data from stream
            byte[] data = new byte[dataSize.asLong()];
            stream.Read(data, 0, data.Length);

            // Parse data as EncryptedTransmission and decrypt with Connection SessionKey
            var encryptedTransmission = EncryptedTransmission.Parser.ParseFrom(data);
            return encryptedTransmission.Decrypt(connection.SessionKey);
        }

        // Ask Scott what this does, it's definitely important though
        public static void OnTransmissionRecieved()
        {
            while (true)
            {
                transmissionsRecievedSignal.WaitOne();
                Tuple<Transmission, Connection> transmission;
                if (TransmissionsRecieved.TryDequeue(out transmission))
                {
                    ServerTransmissionProcessor.Process(transmission);
                    if (!TransmissionsRecieved.TryPeek(out transmission))
                    {
                        transmissionsRecievedSignal.Reset();
                    }
                }
                else
                {
                    throw new Exception("This should never run. The signalling is bad");
                }
            }
        }
    }
}