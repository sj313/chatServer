using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Google.Protobuf;
using System.Security.Cryptography;
using ChatServer.Transmisisons;
using System.Text;

namespace ChatServer
{
    public class MessageHandler
    {
        private readonly ConcurrentQueue<Message> MessagesRecieved = new ConcurrentQueue<Message>();
        private readonly ManualResetEvent messagesRecievedSignal = new ManualResetEvent(false);

        public void Send(User recipient, Message message)
        {
            var tempKey = new RSACryptoServiceProvider(2048);
            var password = Encoding.UTF8.GetBytes("Password");
            var transmission = new Transmission(tempKey.ExportRSAPrivateKey(), message.Serialize());
            var encryptedTransmission = new EncryptedTransmission(transmission, password);
            Send(recipient.getStream(), encryptedTransmission.ToByteArray());
        }

        void Send(NetworkStream stream, byte[] implicitBytes)
        {
            var data = implicitBytes.Length.asBtyes().Concat(implicitBytes).ToArray();

            stream.Write(data, 0, data.Length);
        }

        public Message Recieve(NetworkStream stream)
        {
            byte[] messageLength = new byte[8];
            stream.Read(messageLength, 0, 8);

            byte[] data = new byte[messageLength.asInt64()];
            stream.Read(data, 0, data.Length);

            var password = Encoding.UTF8.GetBytes("Password");
            var encryptedParser = new MessageParser<EncryptedTransmission>(() => new EncryptedTransmission());
            var encryptedTransmission = encryptedParser.ParseFrom(data);
            var parser = new MessageParser<Transmission>(() => new Transmission());
            var transmission = parser.ParseFrom(encryptedTransmission.Decrypt(password));

            var message = transmission.GetData();

            //should check if message is actually completely recieved 
            return Message.Deserialize(message);
        }

        public void RecieveFrom(NetworkStream stream)
        {
            while (true)
            {
                MessagesRecieved.Enqueue(Recieve(stream));
                messagesRecievedSignal.Set();
            }
        }

        public void OnMessageRecieved(Action<Message> doSomething)
        {
            while (true)
            {
                messagesRecievedSignal.WaitOne();
                Message message;
                if (MessagesRecieved.TryDequeue(out message))
                {
                    doSomething(message);
                    if (!MessagesRecieved.TryPeek(out message))
                    {
                        messagesRecievedSignal.Reset();
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
