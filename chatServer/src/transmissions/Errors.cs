using System.Collections.Generic;

namespace ChatServer.Transmissions
{
    public abstract class Errors
    {
        public enum Error
        {
            NoError = 0,
            OnboardingNoUserID = 1,
            OnboardingInvalidUserID = 2,
            OnboardingExistingConnectionWithUserID = 3,
            OnboardingNoName = 4,
        }

        public static Dictionary<Error, string> ErrorMsg = new Dictionary<Error, string>(
            new KeyValuePair<Error, string>[] {
                new KeyValuePair<Error, string>(Error.NoError, "Success"),
                new KeyValuePair<Error, string>(Error.OnboardingNoUserID, "Onboarding error: No UserID was provided"),
                new KeyValuePair<Error, string>(Error.OnboardingInvalidUserID, "Onboarding error: UserID was invalid"),
                new KeyValuePair<Error, string>(Error.OnboardingExistingConnectionWithUserID, "Onboarding error: A connection using this UserID already exists"),
                new KeyValuePair<Error, string>(Error.OnboardingNoName, "Onboarding error: No Name was provided"),
            }
        );
    }
}