using AVS.CoreLib.Abstractions.Messaging;
using AVS.CoreLib.Abstractions.Messaging.PubSub;

namespace AVS.CoreLib.Messaging.PubSub
{
    public class NullEventPublisher : IEventPublisher
    {
        public void Publish(IEvent @event, IPublishContext context)
        {
        }

        public void Publish(IEvent @event)
        {
        }
    }
}