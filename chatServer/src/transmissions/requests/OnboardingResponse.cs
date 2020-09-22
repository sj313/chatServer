using Google.Protobuf;

namespace ChatServer.Transmissions {
    public sealed partial class OnboardingResponse
    {
        public OnboardingResponse(byte[] userID)
        {
            UserID = ByteString.CopyFrom(userID);
        }

        public OnboardingResponse(byte[] userID, string name)
        {
            UserID = ByteString.CopyFrom(userID);
            Name = name;
        }
    }
}