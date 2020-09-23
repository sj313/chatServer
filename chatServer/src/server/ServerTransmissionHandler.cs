using System;
using System.Collections.Concurrent;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using ChatServer.Transmissions;
using Google.Protobuf;

namespace ChatServer
{
    public abstract class ServerTransmissionHandler
    {
        private static readonly ConcurrentQueue<Tuple<Transmission, Connection>> TransmissionsRecieved = new ConcurrentQueue<Tuple<Transmission, Connection>>();
        private static readonly ManualResetEvent transmissionsRecievedSignal = new ManualResetEvent(false);

        public static Transmission CreateTransmission(string messageString)
        {
            var messageBytes = Encoding.UTF8.GetBytes(messageString);
            var serverMessage = new ServerMessage(messageBytes);
            var message = new Message(0, serverMessage);
            return new Transmission(message, Server.SERVER_KEYS.ExportRSAPrivateKey(), Server.SERVER_KEYS.ExportRSAPublicKey());
        }
        
        public static void SendAll(string messageString)
        {
            SendAll(CreateTransmission(messageString));
            
        }

        public static void SendAll(Transmission transmission)
        {
            foreach (Connection connection in Server.CONNECTIONS.Values)
            {
                if (connection.Onboarded) Send(connection, transmission);
            }
        }

        public static void Send(Connection connection, string messageString)
        {
            Send(connection, CreateTransmission(messageString));
        }

        public static void Send(Connection connection, Transmission transmission)
        {
            var encryptedTransmission = new EncryptedTransmission(transmission, connection.SessionKey);
            try
            {
                var stream = connection.TCPClient.GetStream();
                var bytes = encryptedTransmission.ToByteArray();

                stream.Write(BitConverter.GetBytes((Int64)bytes.Length), 0, 8);
                stream.Write(bytes, 0, bytes.Length);
            }
            catch (Exception e) when (e is ObjectDisposedException || e is InvalidOperationException)
            {
                Console.WriteLine(e.StackTrace);
                return;
            }
            
        }

        public static void RecieveFrom(Connection connection)
        {
            while (true)
            {
                try
                {
                TransmissionsRecieved.Enqueue(new Tuple<Transmission, Connection>(Recieve(connection), connection));
                transmissionsRecievedSignal.Set();
                }
                catch
                {
                    Connection temp;
                    Server.CONNECTIONS.TryRemove(connection.ConnectionID, out temp);
                    return;
                }
            }
        }

        public static Transmission Recieve(Connection connection)
        {
            var stream = connection.TCPClient.GetStream();

            byte[] messageLength = new byte[8];
            stream.Read(messageLength, 0, 8);

            byte[] data = new byte[messageLength.asInt64()];
            stream.Read(data, 0, data.Length);

            var parser = new MessageParser<EncryptedTransmission>(() => new EncryptedTransmission());
            var encryptedTransmission = parser.ParseFrom(data);
            return encryptedTransmission.Decrypt(connection.SessionKey);
        }

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
    
        public static void SendOnboardRequest(Connection connection, string message)
        {
            var onboardingRequest = new OnboardingRequest();
            var request = new Request(onboardingRequest);
            var transmission = new Transmission(request, Server.SERVER_KEYS.ExportRSAPrivateKey(), Server.SERVER_KEYS.ExportRSAPublicKey());
            Send(connection, transmission);
        }

    }
}