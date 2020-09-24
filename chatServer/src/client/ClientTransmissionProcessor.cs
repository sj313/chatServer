using System;
using System.Text;
using ChatServer.Transmissions;

namespace ChatServer.Client
{
    public abstract class ClientTransmissionProcessor
    {
        public static void Process(Transmission transmission)
        {
            if (transmission.Message != null)
            {
                ProcessMessage(transmission);
                return;
            }
            if (transmission.Request != null)
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
                DecryptAndDisplay(transmission);
            }
            if (message.ServerMessage != null)
            {
                //Temp
                UIController.Display(Encoding.UTF8.GetString(message.ServerMessage.Message.ToByteArray()));
            }
        }

        private static void DecryptAndDisplay(Transmission transmission)
        {
            //Temp
            var message = Encoding.UTF8.GetString(transmission.Message.EncryptedMessage.Decrypt(Client.SERVER_PASS));
            UIController.Display(message);
        }

        private static void ProcessRequest(Transmission transmission)
        {
            var request = transmission.Request;
            if (request.OnboardingRequest != null)
            {
                ProcessOnboarding(transmission);
            }
        }

        private static void ProcessOnboarding(Transmission transmission)
        {
            //Temp
            if (transmission.Request.ErrorID == (int)Errors.Error.NoError)
            {
                Console.WriteLine("Received onboarding request, responding...");
                ClientTransmissionHandler.SendOnboardingResponse();
            }
            else
            {
                Errors.Error error = (Errors.Error)transmission.Request.ErrorID;
                Console.WriteLine("Received onboarding request with error: {0}", Errors.ErrorMsg[error]);
                Console.WriteLine("Resending onboarding response in 1 second...");
                System.Threading.Thread.Sleep(1000);
                ClientTransmissionHandler.SendOnboardingResponse();
            }

        }
    }
}