using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using ChatServer.Transmissions;
using Google.Protobuf;

namespace ChatServer.Client
{
    public abstract class ClientTransmissionHandler
    {
        private static readonly ConcurrentQueue<Transmission> TransmissionsRecieved = new ConcurrentQueue<Transmission>();
        private static readonly ManualResetEvent transmissionsRecievedSignal = new ManualResetEvent(false);
        public static Transmission CreateTransmission(string messageString)
        {
            var messageBytes = Encoding.UTF8.GetBytes(messageString);
            var encryptedMessage = new EncryptedMessage(messageBytes, Client.SERVER_PASS);
            var message = new Message(0, encryptedMessage);
            return new Transmission(message, Client.CLIENT_KEYS.ExportRSAPrivateKey(), Client.CLIENT_KEYS.ExportRSAPublicKey());
        }

        public static void Send(string messageString)
        {
            Send(CreateTransmission(messageString));
        }

        public static void SendOnboardingResponse()
        {
            var response = new OnboardingResponse(Client.USER.ID, Client.USER.Name);
            var request = new Request(response);
            var transmission = new Transmission(request, Client.CLIENT_KEYS.ExportRSAPrivateKey(), Client.CLIENT_KEYS.ExportRSAPublicKey());
            Send(transmission);
        }

        public static void Send(Transmission transmission)
        {
            var encryptedTransmission = new EncryptedTransmission(transmission, Client.Connection.SessionKey);

            var stream = Client.Connection.TCPClient.GetStream();
            var bytes = encryptedTransmission.ToByteArray();

            stream.Write(BitConverter.GetBytes(bytes.LongLength), 0, 8);
            stream.Write(bytes, 0, bytes.Length);
        }

        public static void RecieveFrom()
        {
            while (true)
            {
                TransmissionsRecieved.Enqueue(new Transmission(Recieve()));
                transmissionsRecievedSignal.Set();
            }
        }

        public static Transmission Recieve()
        {
            var stream = Client.Connection.TCPClient.GetStream();

            byte[] messageLength = new byte[8];
            stream.Read(messageLength, 0, 8);

            byte[] data = new byte[messageLength.asInt64()];
            stream.Read(data, 0, data.Length);

            var parser = new MessageParser<EncryptedTransmission>(() => new EncryptedTransmission());
            var encryptedTransmission = parser.ParseFrom(data);
            return encryptedTransmission.Decrypt(Client.Connection.SessionKey);
        }

        public static void OnTransmissionRecieved()
        {
            while (true)
            {
                transmissionsRecievedSignal.WaitOne();
                Transmission transmission;
                if (TransmissionsRecieved.TryDequeue(out transmission))
                {
                    ClientTransmissionProcessor.Process(transmission);
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