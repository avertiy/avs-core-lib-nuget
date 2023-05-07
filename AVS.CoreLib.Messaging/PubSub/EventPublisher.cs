using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions.Messaging;
using AVS.CoreLib.Abstractions.Messaging.PubSub;
using AVS.CoreLib.Extensions;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.Messaging.PubSub
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IEventConsumerFactory _factory;
        private readonly ILogger _logger;

        public EventPublisher(IEventConsumerFactory factory, ILogger<EventPublisher> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public void Publish(IEvent @event)
        {
            Publish(@event, new PublishContext() { Isolated = true });
        }

        public void Publish(IEvent @event, IPublishContext context)
        {
            //get all event consumers
            var type = typeof(IEventConsumer<,>);
            var genericType = type.MakeGenericType(@event.GetType(), context.GetType());
            var consumers = _factory.ResolveAll(genericType);

            if (consumers.Length == 0 && context.Mandatory)
                throw new PublishEventException($"No IEventConsumer(s) registered for {genericType.ToStringNotation()}");

            foreach (var consumer in consumers)
            {
                try
                {
                    consumer.Handle(@event, context);
                }
                catch (Exception ex)
                {
                    _logger.LogError(EventCodes.HandleEvent, ex, $"Event consumer {@consumer.GetType().Name} failed to handle {@event.GetType().Name}");
                    if (context.Isolated)
                        continue;
                    break;
                }
            }
        }

        public async Task PublishAsync(IEvent @event, IPublishContext context, CancellationToken ct = default)
        {
            //get all event consumers
            var type = typeof(IEventConsumer<,>);
            var genericType = type.MakeGenericType(@event.GetType(), context.GetType());
            var consumers = _factory.ResolveAll(genericType);

            if (consumers.Length == 0 && context.Mandatory)
                throw new PublishEventException($"No IEventConsumer(s) registered for {genericType.ToStringNotation()}");

            var tasks = new List<Task>();
            foreach (var consumer in consumers)
            {
                try
                {
                    var task = Task.Run(() => consumer.Handle(@event, context), ct);
                    tasks.Add(task);
                }
                catch (Exception ex)
                {
                    _logger.LogError(EventCodes.HandleEvent, ex, $"Event consumer {@consumer.GetType().Name} failed to handle {@event.GetType().Name}");
                    if (context.Isolated)
                        continue;
                    break;
                }
            }

            await Task.WhenAll(tasks);
        }
    }
}