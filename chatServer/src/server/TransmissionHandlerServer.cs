using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using ChatServer.Transmissions;
using Google.Protobuf;

namespace ChatServer
{
    public class TransmissionHandlerServer
    {
        private readonly ConcurrentQueue<Tuple<Transmission, Connection>> TransmissionsRecieved = new ConcurrentQueue<Tuple<Transmission, Connection>>();
        private readonly ManualResetEvent transmissionsRecievedSignal = new ManualResetEvent(false);

        private readonly byte[] keyPass = Encoding.UTF8.GetBytes("ServerKeyPassword");
        const string keyPath = @"..\..\resources\.serverkeys";
        private readonly RSACryptoServiceProvider ServerKeys = new RSACryptoServiceProvider(2048);

        public TransmissionHandlerServer()
        {
            ServerKeys.ImportParameters(Encryption.RSA.GetStoredEncryptedKeyPair(keyPath, keyPass, true));
        }

        public Transmission CreateTransmission(string messageString)
        {
            var messageBytes = Encoding.UTF8.GetBytes(messageString);
            var serverMessage = new ServerMessage(messageBytes);
            var message = new Message(0, serverMessage);
            return new Transmission(message, ServerKeys.ExportRSAPrivateKey(), ServerKeys.ExportRSAPublicKey());
        }
        
        public void Send(ConcurrentBag<Connection> connections, string messageString)
        {
            var transmission = CreateTransmission(messageString);
            foreach (Connection connection in connections)
            {
                Send(connection, transmission);
            }
        }

        public void Send(Connection connection, string messageString)
        {
            Send(connection, CreateTransmission(messageString));
        }

        public void Send(Connection connection, Transmission transmission)
        {
            var encryptedTransmission = new EncryptedTransmission(transmission, connection.SessionKey);

            var stream = connection.TCPConnection.GetStream();
            var bytes = encryptedTransmission.ToByteArray();

            stream.Write(BitConverter.GetBytes((Int64)bytes.Length), 0, 8);
            stream.Write(bytes, 0, bytes.Length);
        }

        public void RecieveFrom(Connection connection)
        {
            while (true)
            {
                TransmissionsRecieved.Enqueue(new Tuple<Transmission, Connection>(Recieve(connection), connection));
                transmissionsRecievedSignal.Set();
            }
        }

        public Transmission Recieve(Connection connection)
        {
            var stream = connection.TCPConnection.GetStream();

            byte[] messageLength = new byte[8];
            stream.Read(messageLength, 0, 8);

            byte[] data = new byte[messageLength.asInt64()];
            stream.Read(data, 0, data.Length);

            var parser = new MessageParser<EncryptedTransmission>(() => new EncryptedTransmission());
            var encryptedTransmission = parser.ParseFrom(data);
            return encryptedTransmission.Decrypt(connection.SessionKey);
        }

        public void OnTransmissionRecieved()
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
    
        public void SendOnboardRequest(Connection connection, string message)
        {
            var onboardingRequest = new OnboardingRequest();
            var request = new Request(onboardingRequest);
            var transmission = new Transmission(request, ServerKeys.ExportRSAPrivateKey(), ServerKeys.ExportRSAPublicKey());
            Send(connection, transmission);
        }

    }
}