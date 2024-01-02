using System;

namespace AVS.CoreLib.Messaging
{
    public class SendCommandException : Exception
    {
        public SendCommandException()
        {
        }

        public SendCommandException(string message) : base(message)
        {
        }

        public SendCommandException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}