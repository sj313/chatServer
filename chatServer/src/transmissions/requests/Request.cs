namespace ChatServer.Transmissions
{
    public sealed partial class Request
    {
        public Request(JoinRequest joinRequest)
        {
            JoinRequest = joinRequest;
        }
    }
}