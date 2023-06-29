using System;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Messaging
{
    public class NoEventConsumersException : Exception
    {
        public NoEventConsumersException(Type genericType) : base($"No event consumers `{genericType.ToStringNotation()}` found")
        {
        }

        public NoEventConsumersException(string message) : base(message)
        {
        }

        public NoEventConsumersException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}