using System;
using AVS.CoreLib.Abstractions.Messaging;
using AVS.CoreLib.Abstractions.Messaging.PubSub;

namespace AVS.CoreLib.Messaging.PubSub
{
    public class PublishContext : IPublishContext
    {
        public PublishMode Mode { get; set; }
    }

    public class Event : IEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Timestamp { get; set; }
        public override string ToString()
        {
            return $"{this.GetType().Name} #{Id}";
        }
    }
}