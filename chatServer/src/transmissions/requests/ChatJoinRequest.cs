using System;
using Google.Protobuf;

namespace ChatServer.Transmissions
{
    public sealed partial class ChatJoinRequest
    {
        public Guid ChatID {get {return new Guid(ChatIDBytes.ToByteArray());}} 

        public ChatJoinRequest(Guid chatID)
        {
            ChatIDBytes = ByteString.CopyFrom(chatID.ToByteArray());
        }

        internal Errors.Error Validate()
        {
            if (ChatIDBytes == null) return Errors.Error.NoContent;
            if (ChatIDBytes.ToByteArray().Length != 16) return Errors.Error.InvalidContent;
            return Errors.Error.NoError;
        }
    }
}