using System;

namespace SteamBadgeUnlocker.API
{
    public class ClientInitializeException : Exception
    {
        public readonly ClientInitializeFailure Failure;

        public ClientInitializeException(ClientInitializeFailure failure)
        {
            this.Failure = failure;
        }

        public ClientInitializeException(ClientInitializeFailure failure, string message)
            : base(message)
        {
            this.Failure = failure;
        }

        public ClientInitializeException(ClientInitializeFailure failure, string message, Exception innerException)
            : base(message, innerException)
        {
            this.Failure = failure;
        }
    }
}
