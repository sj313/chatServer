using System;
using System.Security.Cryptography;
using ChatServer.Transmissions;
using System.Linq;

namespace ChatServer.Server
{
    public abstract class ServerTransmissionProcessor
    {
        public static void Process(Tuple<Transmission, Connection> transmission)
        {
            if (transmission.Item1.Message != null && transmission.Item2.Onboarded)
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
            ServerTransmissionHandler.SendAll(transmission);
        }

        private static void Onboard(Tuple<Transmission, Connection> transmission)
        {
            var response = transmission.Item1.Request.OnboardingResponse;
            var connection = transmission.Item2;
            if (response.UserID != null)
            {
                try
                {
                    (new RSACryptoServiceProvider(2048)).ImportRSAPublicKey(response.UserID.ToByteArray(), out int i);
                }
                catch
                {
                    ServerTransmissionHandler.SendOnboardRequest(transmission.Item2, Errors.Error.OnboardingInvalidUserID);
                    return;
                }

                foreach (Connection existingConnection in Server.CONNECTIONS.Values)
                {
                    
                    if (existingConnection.User.ID.SequenceEqual(response.UserID.ToByteArray()))
                    {
                        // TODO Allow multiple connections but make sure signature is verified
                        ServerTransmissionHandler.SendOnboardRequest(transmission.Item2, Errors.Error.OnboardingExistingConnectionWithUserID);
                        return;
                    }
                }
            }
            else
            {
                ServerTransmissionHandler.SendOnboardRequest(transmission.Item2, Errors.Error.OnboardingNoUserID);
                return;
            }

            // TODO: Check name doesn't already exist for user with this ID
            if (String.IsNullOrWhiteSpace(response.Name))
            {
                ServerTransmissionHandler.SendOnboardRequest(transmission.Item2, Errors.Error.OnboardingNoName);
                return;
            }

            connection.Onboarded = true;
            connection.User.Name = response.Name;
            connection.User.ID = response.UserID.ToByteArray();
            ServerTransmissionHandler.SendAll($"------------------------------ '{response.Name}' has entered the chat ------------------------------");

        }
    }
}