using System;
using System.Runtime.Serialization;

namespace AVS.CoreLib.PowerConsole.Utilities
{
    public class ConsoleException : Exception
    {
        public ConsoleException()
        {
        }

        protected ConsoleException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ConsoleException(string message) : base(message)
        {
        }

        public ConsoleException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}