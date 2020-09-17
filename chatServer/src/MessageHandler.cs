using System;
using System.Collections;
using System.Collections.Concurrent;

using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ChatServer
{
    public class MessageHandler
    {
        private static readonly Int32 BUFFER_SIZE = 1024;
        private readonly ConcurrentQueue<Message> MessagesRecieved = new ConcurrentQueue<Message>();
        private readonly ManualResetEvent messagesRecievedSignal = new ManualResetEvent(false);

        public void Send(User recipient, Message message)
        {
            Send(recipient.getStream(), message.Serialize());
        }

        void Send(NetworkStream stream, byte[] implicitBytes)
        {
            stream.Write(implicitBytes, 0, implicitBytes.Length);

            // if (implicitBytes.Length > BUFFER_SIZE)
            // {
            //     Send(stream, implicitBytes.Skip(BUFFER_SIZE).ToArray());
            // }
        }

        public Message Recieve(NetworkStream stream)
        {
            byte[] data = new byte[BUFFER_SIZE];
            stream.Read(data, 0, BUFFER_SIZE);

            //should check if message is actually completely recieved 
            return Message.Deserialize(data);
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
