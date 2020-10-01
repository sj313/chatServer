using System;
using System.Linq;
using System.Security.Cryptography;
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
            if (transmission.Response != null)
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
                DecryptAndDisplay(transmission);
            }
            if (message.ServerMessage != null)
            {
                //Temp
                UIController.Display(Encoding.UTF8.GetString(message.ServerMessage.Message.ToByteArray()));
            }
        }

        private static void ProcessRequest(Transmission transmission)
        {
            return;
        }
        private static void DecryptAndDisplay(Transmission transmission)
        {
            //Temp
            if (transmission.SenderID.ToByteArray().SequenceEqual(Client.CLIENT_KEYS.ExportRSAPublicKey())) return;
            
            try
            {
                var message = Encoding.UTF8.GetString(transmission.Message.EncryptedMessage.Decrypt(Client.SERVER_PASS));
                UIController.Display(message);
            }
            catch (CryptographicException)
            {
                UIController.Display($"Received incorrectly encrypted message from user: {"???"}");
            }
        }

        private static void ProcessResponse(Transmission transmission)
        {
            if (transmission.Response.ErrorID == (int)Errors.Error.NoError)
                Client.Connection.Joined = true;
            return;
        }
    }
}