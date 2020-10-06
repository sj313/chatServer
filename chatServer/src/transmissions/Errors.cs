using System.Collections.Generic;

namespace ChatServer.Transmissions
{
    public abstract class Errors
    {
        public enum Error
        {
            NoError = 0,
            NoSenderID = 1,
            NoContent = 2,
            InvalidSignature = 3,
            InvalidContent = 4,
        }

        public static Dictionary<Error, string> ErrorMsg = new Dictionary<Error, string>(
            new KeyValuePair<Error, string>[] {
                new KeyValuePair<Error, string>(Error.NoError, "Success"),
                new KeyValuePair<Error, string>(Error.NoSenderID, "Transmission error: No UserID was provided"),
                new KeyValuePair<Error, string>(Error.NoContent, "Transmission error: Transmisison contains no content"),
                new KeyValuePair<Error, string>(Error.InvalidSignature, "Transmisison error: Transmission content was incorrectly signed"),
                new KeyValuePair<Error, string>(Error.InvalidContent, "Transmisison error: Transmission content was invalid"),
            }
        );
    }
}