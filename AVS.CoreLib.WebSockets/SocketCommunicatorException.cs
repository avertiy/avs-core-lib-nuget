using System;
using AVS.CoreLib.Exceptions;

namespace AVS.CoreLib.WebSockets
{
    public class SocketCommunicatorException : AppException
    {
        public SocketCommunicatorException(string message) : base(message)
        {
        }

        public SocketCommunicatorException(string message, string hint) : base(message, hint)
        {
        }

        public SocketCommunicatorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}