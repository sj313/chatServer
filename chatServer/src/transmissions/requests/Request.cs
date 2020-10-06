namespace ChatServer.Transmissions
{
    public sealed partial class Request
    {
        public Request(ServerJoinRequest serverJoinRequest)
        {
            ServerJoinRequest = serverJoinRequest;
        }
        
        public Request(ChatJoinRequest chatJoinRequest)
        {
            ChatJoinRequest = chatJoinRequest;
        }

        public Request(ChatLeaveRequest chatLeaveRequest)
        {
            ChatLeaveRequest = chatLeaveRequest;
        }

        public Request(NameChangeRequest nameChangeRequest)
        {
            NameChangeRequest = nameChangeRequest;
        }

        internal Errors.Error Validate()
        {
            if (ServerJoinRequest == null && ChatJoinRequest == null && ChatLeaveRequest == null && NameChangeRequest == null) return Errors.Error.NoContent;
            if (ChatJoinRequest != null) return ChatJoinRequest.Validate();
            // if (ChatLeaveRequest != null) return ChatLeaveRequest.Validate();
            // if (NameChangeRequest != null) return NameChangeRequest.Validate();
            return Errors.Error.NoError;
        }
    }
}