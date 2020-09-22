namespace ChatServer.Transmissions {
    public sealed partial class Request
    {
        public Request(OnboardingRequest onboardingRequest)
        {
            OnboardingRequest = onboardingRequest;
        }
        public Request(OnboardingResponse onboardingResponse)
        {
            OnboardingResponse = onboardingResponse;
        }
    }
}