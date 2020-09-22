using ChatServer.Encryption;
using Google.Protobuf;

namespace ChatServer.Transmissions {

    public sealed partial class Transmission
    {
        public Transmission(Message message, byte[] privKey, byte[] pubKey)
        {
            Message = message;
            Signature = ByteString.CopyFrom(RSA.CreateSignature(privKey, Message.ToByteArray()));
            SenderID = ByteString.CopyFrom(pubKey);
        }
        
        public Transmission(Request request, byte[] privKey, byte[] pubKey)
        {
            Request = request;
            Signature = ByteString.CopyFrom(RSA.CreateSignature(privKey, Request.ToByteArray()));
            SenderID = ByteString.CopyFrom(pubKey);
        }

        public bool Verify()
        {
            if (Message.IsInitialized())
                return RSA.Verify(SenderID.ToByteArray(), Message.ToByteArray());
            if (Request.IsInitialized())
                return RSA.Verify(SenderID.ToByteArray(), Request.ToByteArray());
            return false;
        }

    }

}