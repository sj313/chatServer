using System;
using ChatServer.Transmissions;

namespace ChatServer.Server
{
    public abstract class ServerTransmissionProcessor
    {
        public static void Process(Tuple<Transmission, Connection> transmission)
        {
            if (transmission.Item1.SenderID == null)
            {
                // TODO: Process error
                return;
            }
            if (!transmission.Item1.Verify())
            {
                //TODO: Process error
                return;
            }
            if (transmission.Item1.Message != null && transmission.Item2.Joined)
            {
                ProcessMessage(transmission.Item1);
                return;
            }
            if (transmission.Item1.Request != null)
            {
                ProcessRequest(transmission);
                return;
            }
            if (transmission.Item1.Response != null)
            {
                ProcessResponse(transmission);
                return;
            }
        }

        private static void ProcessMessage(Transmission transmission)
        {
            var message = transmission.Message;
            if (message.EncryptedMessage != null)
            {
                Rebroadcast(transmission);
            }
        }

        private static void ProcessRequest(Tuple<Transmission, Connection> transmission)
        {
            var request = transmission.Item1.Request;
            var connection = transmission.Item2;
            if (request.JoinRequest != null)
            {
                Console.WriteLine("Received Join Request");
                connection.User.ID = transmission.Item1.SenderID.ToByteArray();
                // TODO: Set name if already exists
                connection.User.Name = "TempName";
                connection.Joined = true;
                ServerTransmissionHandler.SendResponse(transmission.Item2, Errors.Error.NoError);
                ServerTransmissionHandler.SendAll($"-------------------User: {connection.User.Name} has joined--------------------");
            }
        }

        private static void ProcessResponse(Tuple<Transmission, Connection> transmission)
        {
            return;
        }

        private static void Rebroadcast(Transmission transmission)
        {
            ServerTransmissionHandler.SendAll(transmission);
        }
    }
}