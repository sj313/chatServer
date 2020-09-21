using System;
using ChatServer.Encryption;
using Google.Protobuf;

namespace ChatServer.Transmissions {

    public sealed partial class Transmission
    {
        public Transmission(Message message, ReadOnlySpan<byte> privKey)
        {
            Message = message;
            Signature = ByteString.CopyFrom(RSA.CreateSignature(privKey, Message.ToByteArray()));
        }
        
        public Transmission(Request request, ReadOnlySpan<byte> privKey)
        {
            Request = request;
            Signature = ByteString.CopyFrom(RSA.CreateSignature(privKey, Request.ToByteArray()));
        }

        public bool Verify(ReadOnlySpan<byte> pubKey)
        {
            if (Message.IsInitialized())
                return RSA.Verify(pubKey, Message.ToByteArray());
            if (Request.IsInitialized())
                return RSA.Verify(pubKey, Request.ToByteArray());
            return false;
        }

    }

}