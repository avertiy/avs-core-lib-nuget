using System.Threading.Tasks;
using AVS.CoreLib.Abstractions.Messaging;
using AVS.CoreLib.Abstractions.Messaging.PubSub;

namespace AVS.CoreLib.Messaging.PubSub
{
    public class NullEventPublisher : IEventPublisher
    {
        public void Publish(IEvent @event, IPublishContext context)
        {
        }

        public Task PublishAsync(IEvent @event, IPublishContext context)
        {
            return Task.CompletedTask;
        }

        public void Publish(IEvent @event)
        {
        }
    }
}