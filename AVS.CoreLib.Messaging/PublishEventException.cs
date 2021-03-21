using System;

namespace AVS.CoreLib.Messaging
{
    public class PublishEventException : Exception
    {
        public PublishEventException(string message) : base(message)
        {
        }

        public PublishEventException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}