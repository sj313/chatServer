using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Google.Protobuf;
using System.Security.Cryptography;
using System.Text;
using ChatServer.Transmissions;

namespace ChatServer
{
    public class MessageHandler
    {
        private readonly ConcurrentQueue<Message> MessagesRecieved = new ConcurrentQueue<Message>();
        private readonly ManualResetEvent messagesRecievedSignal = new ManualResetEvent(false);

        private readonly byte[] TEMP_PASSWORD = Encoding.UTF8.GetBytes("Password");

        public void Send(User recipient, Message newMessage)
        {
            var tempKey = (new RSACryptoServiceProvider(2048)).ExportRSAPrivateKey();
            var encryptedMessage = new EncryptedMessage(newMessage.Serialize(), TEMP_PASSWORD);
            var message = new Transmissions.Message(encryptedMessage);
            var transmission = new Transmission(message, tempKey);
            var encryptedTransmission = new EncryptedTransmission(transmission, TEMP_PASSWORD);

            Send(recipient.getStream(), encryptedTransmission.ToByteArray());
        }

        private void Send(NetworkStream stream, byte[] implicitBytes)
        {
            var data = implicitBytes.Length.asBtyes().Concat(implicitBytes).ToArray();

            stream.Write(data, 0, data.Length);
        }

        public Message Recieve(NetworkStream stream)
        {
            byte[] readData()
            {
                byte[] messageLength = new byte[8];
                stream.Read(messageLength, 0, 8);

                byte[] data = new byte[messageLength.asInt64()];
                stream.Read(data, 0, data.Length);
                return data;
            }
            
            byte[] parseAndDecrypt(byte[] data)
            {
                var parser = new MessageParser<EncryptedTransmission>(() => new EncryptedTransmission());
                var encryptedTransmission = parser.ParseFrom(data);
                var transmission = encryptedTransmission.Decrypt(TEMP_PASSWORD);
                var message = transmission.Message;
                var encryptedMessage = message.EncryptedMessage;
                return encryptedMessage.Decrypt(TEMP_PASSWORD);
            }

            var data = readData();
            var message = parseAndDecrypt(data);       

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
