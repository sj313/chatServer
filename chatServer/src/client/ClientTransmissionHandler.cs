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
    public abstract class ClientTransmissionHandler
    {
        private static readonly ConcurrentQueue<Transmission> TransmissionsRecieved = new ConcurrentQueue<Transmission>();
        private static readonly ManualResetEvent transmissionsRecievedSignal = new ManualResetEvent(false);

        public static readonly RSACryptoServiceProvider ClientKeys = new RSACryptoServiceProvider(2048);


        public static void SetKeys(string keyPath, byte[] keyPass)
        {
            if (!File.Exists(keyPath))
                Encryption.RSA.GenerateAndStoreNewEncryptedKeyPair(keyPath, keyPass);
            ClientKeys.ImportParameters(Encryption.RSA.GetStoredEncryptedKeyPair(keyPath, keyPass, true));
        }

        public static Transmission CreateTransmission(string messageString)
        {
            var messageBytes = Encoding.UTF8.GetBytes(messageString);
            var encryptedMessage = new EncryptedMessage(messageBytes, Client.SERVER_PASS);
            var message = new Message(0, encryptedMessage);
            return new Transmission(message, ClientKeys.ExportRSAPrivateKey(), ClientKeys.ExportRSAPublicKey());
        }

        public static void Send(string messageString)
        {
            Send(CreateTransmission(messageString));
        }

        public static void SendOnboardingResponse()
        {
            var response = new OnboardingResponse(Client.USER.ID, Client.USER.Name);
            var request = new Request(response);
            var transmission = new Transmission(request, ClientKeys.ExportRSAPrivateKey(), ClientKeys.ExportRSAPublicKey());
            Send(transmission);
        }

        public static void Send(Transmission transmission)
        {
            var encryptedTransmission = new EncryptedTransmission(transmission, Client.SESSION_KEY);

            var stream = Client.TCP_CLIENT.GetStream();
            var bytes = encryptedTransmission.ToByteArray();

            stream.Write(BitConverter.GetBytes((Int64)bytes.Length), 0, 8);
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
            var stream = Client.TCP_CLIENT.GetStream();

            byte[] messageLength = new byte[8];
            stream.Read(messageLength, 0, 8);

            byte[] data = new byte[messageLength.asInt64()];
            stream.Read(data, 0, data.Length);

            var parser = new MessageParser<EncryptedTransmission>(() => new EncryptedTransmission());
            var encryptedTransmission = parser.ParseFrom(data);
            return encryptedTransmission.Decrypt(Client.SESSION_KEY);
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