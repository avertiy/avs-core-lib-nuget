using System;
using AVS.CoreLib.Abstractions.Messaging;
using AVS.CoreLib.Messaging.Abstractions.PubSub;

namespace AVS.CoreLib.Messaging
{
    public class HandleEventException : Exception
    {
        public IEvent Event { get; set; }
        public IPublishContext Context { get; set; }

        public HandleEventException(string message) : base(message)
        {
        }

        public HandleEventException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}