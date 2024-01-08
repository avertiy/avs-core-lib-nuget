using System;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions.Messaging;
using AVS.CoreLib.Messaging.Abstractions.PubSub;

namespace AVS.CoreLib.Messaging.PubSub
{
    /// <summary>
    /// Represents a null (do nothing) implementation of <see cref="IEventPublisher"/>
    /// </summary>
    public class NullEventPublisher : IEventPublisher
    {
        public void Publish(IEvent @event, IPublishContext context)
        {
        }

        public void Publish<TEvent>(TEvent @event, IPublishContext context) where TEvent : class, IEvent
        {
        }

        public Task PublishAsync(IEvent @event, IPublishContext context, CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }

        public Task PublishAsync<TEvent>(TEvent @event, IPublishContext context, CancellationToken ct = default) where TEvent : class, IEvent
        {
            return Task.CompletedTask;
        }

        public bool AnyConsumers(Type eventType, Type contextType)
        {
            return true;
        }
    }
}