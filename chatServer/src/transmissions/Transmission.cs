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

        public Transmission(Response response, byte[] privKey, byte[] pubKey)
        {
            Response = response;
            Signature = ByteString.CopyFrom(RSA.CreateSignature(privKey, Response.ToByteArray()));
            SenderID = ByteString.CopyFrom(pubKey);
        }

        private bool Verify()
        {
            if (Message != null)
                return RSA.Verify(SenderID.ToByteArray(), Message.ToByteArray(), Signature.ToByteArray());
            if (Request != null)
                return RSA.Verify(SenderID.ToByteArray(), Request.ToByteArray(), Signature.ToByteArray());
            if (Response != null)
                return RSA.Verify(SenderID.ToByteArray(), Response.ToByteArray(), Signature.ToByteArray());
            return false;
        }

        public Errors.Error Validate()
        {
            if (SenderID == null) return Errors.Error.NoSenderID;
            if (Message == null && Request == null && Response == null) return Errors.Error.NoContent;
            if (!Verify()) return Errors.Error.InvalidSignature;
            if (Message != null) return Message.Validate();
            if (Request != null) return Request.Validate();
            if (Response != null) return Response.Validate();
            return Errors.Error.NoError;

        }

    }

}