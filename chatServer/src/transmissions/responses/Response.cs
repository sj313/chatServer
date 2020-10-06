using System;

namespace ChatServer.Transmissions
{
    public sealed partial class Response
    {
        public Response(Errors.Error error)
        {
            ErrorID = (int)error;
        }

        internal Errors.Error Validate()
        {
            return Errors.Error.NoError;
        }
    }
}