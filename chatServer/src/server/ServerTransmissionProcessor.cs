using System;
using System.Linq;
using System.Security.Cryptography;
using ChatServer.Transmissions;

namespace ChatServer
{
    public abstract class ServerTransmissionProcessor
    {
        public static void Process(Tuple<Transmission, Connection> transmission)
        {
            if (transmission.Item1.Message != null)
            {
                ProcessMessage(transmission.Item1);
                return;
            }
            if (transmission.Item1.Request != null)
            {
                ProcessRequest(transmission);
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
            if (request.OnboardingResponse != null)
            {
                Onboard(transmission);
            }
        }

        private static void Rebroadcast(Transmission transmission)
        {
            foreach (var recipient in Server.CONNECTIONS)
            {
                if (recipient.Onboarded) Server.TRANSMISSION_HANDLER.Send(recipient, transmission);
            }
        }

        private static void Onboard(Tuple<Transmission, Connection> transmission)
        {
            var response = transmission.Item1.Request.OnboardingResponse;
            var connection = transmission.Item2;
            if (response.UserID != null)
            {
                foreach (Connection existingConnection in Server.CONNECTIONS)
                {
                    try
                    {
                        (new RSACryptoServiceProvider(2048)).ImportRSAPublicKey(response.UserID.ToByteArray(), out int i);
                    }
                    catch
                    {
                        Server.TRANSMISSION_HANDLER.SendOnboardRequest(transmission.Item2, "UserID invalid");
                        return;
                    }

                    if (existingConnection.User.ID == response.UserID.ToByteArray())
                    {
                        Server.TRANSMISSION_HANDLER.SendOnboardRequest(transmission.Item2, "UserID already connected");
                        return;
                    }
                }
            }
            else
            {
                Server.TRANSMISSION_HANDLER.SendOnboardRequest(transmission.Item2, "UserID not provided");
                return;
            }

            if (String.IsNullOrWhiteSpace(response.Name))
            {
                Server.TRANSMISSION_HANDLER.SendOnboardRequest(transmission.Item2, "Name not provided");
                return;
            }

            connection.Onboarded = true;
            connection.User.Name = response.Name;
            connection.User.ID = response.UserID.ToByteArray();
            Server.TRANSMISSION_HANDLER.Send(Server.CONNECTIONS, $"------------------------------ '{response.Name}' has entered the chat ------------------------------");
            
        }
    }
}