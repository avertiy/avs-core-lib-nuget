#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions.Messaging;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Messaging.Abstractions.PubSub;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.Messaging.PubSub
{
    /// <summary>
    /// Represents in-app implementation of the <see cref="IEventPublisher"/> (pub/sub approach to decouple app modules).
    /// </summary>
    public class EventPublisher : IEventPublisher
    {
        private readonly IEventConsumerFactory _factory;
        private readonly ILogger _logger;

        public EventPublisher(IEventConsumerFactory factory, ILogger<EventPublisher> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        #region Publish
        public void Publish(IEvent @event, IPublishContext context)
        {
            var consumers = ResolveConsumers(@event.GetType(), context.GetType(), context.Mode.HasFlag(PublishMode.Mandatory));
            PublishInternal(@event, context, consumers);
        }

        public void Publish<TEvent>(TEvent @event, IPublishContext context) where TEvent : class, IEvent
        {
            var consumers = ResolveAllConsumers<TEvent>(@event.GetType(), context.GetType(), context.Mode.HasFlag(PublishMode.Mandatory));
            PublishInternal(@event, context, consumers);
        }

        private void PublishInternal(IEvent @event, IPublishContext context, IEventConsumer[] consumers)
        {
            foreach (var consumer in consumers)
            {
                try
                {
                    consumer.Handle(@event, context);
                }
                catch (Exception ex)
                {
                    _logger.LogError(EventCodes.HandleEvent, ex, $"Event consumer {consumer.GetType().ToStringNotation()} failed to handle {@event.GetType().Name}");
                    if (context.Mode.HasFlag(PublishMode.Isolated))
                        continue;
                    break;
                }
            }
        }
        #endregion

        #region PublishAsync
        public async Task PublishAsync(IEvent @event, IPublishContext context, CancellationToken ct = default)
        {
            var consumers = ResolveConsumers(@event.GetType(), context.GetType(), context.Mode.HasFlag(PublishMode.Mandatory));
            await PublishInternalAsync(@event, context, consumers, ct);
        }

        public async Task PublishAsync<TEvent>(TEvent @event, IPublishContext context, CancellationToken ct = default)
            where TEvent : class, IEvent
        {
            var consumers = ResolveAllConsumers<TEvent>(@event.GetType(), context.GetType(), context.Mode.HasFlag(PublishMode.Mandatory));
            await PublishInternalAsync(@event, context, consumers, ct);
        }

        private async Task PublishInternalAsync(IEvent @event, IPublishContext context, IEventConsumer[] consumers, CancellationToken ct = default)
        {
            if (context.Mode.HasFlag(PublishMode.Parallel))
            {
                var tasks = new List<Task>(consumers.Length);
                foreach (var consumer in consumers)
                {
                    try
                    {
                        var task = Task.Run(() => consumer.Handle(@event, context), ct);
                        tasks.Add(task);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(EventCodes.HandleEvent, ex, $"Event consumer {consumer.GetType().ToStringNotation()} failed to handle {@event.GetType().Name}");
                        if (context.Mode.HasFlag(PublishMode.Isolated))
                            continue;
                        break;
                    }
                }

                await Task.WhenAll(tasks);
            }
            else
            {
                await Task.Run(() =>
                {
                    foreach (var consumer in consumers)
                    {
                        try
                        {
                            consumer.Handle(@event, context);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(EventCodes.HandleEvent, ex,
                                $"Event consumer {consumer.GetType().ToStringNotation()} failed to handle {@event.GetType().Name}");
                            if (context.Mode.HasFlag(PublishMode.Isolated))
                                continue;
                            break;
                        }
                    }
                }, ct);
            }
        }
        #endregion

        public bool AnyConsumers(Type eventType, Type contextType)
        {
            var type = typeof(IEventConsumer<,>);
            var genericType = type.MakeGenericType(eventType, contextType);
            var consumers = _factory.ResolveAll(genericType);
            return consumers.Any();
        }

        private IEventConsumer[] ResolveAllConsumers<TEvent>(Type eventType, Type contextType, bool mandatory)
            where TEvent : class, IEvent
        {
            
            var type = typeof(TEvent);
            // get consumers of TEvent type
            var consumers = ResolveConsumers(type, contextType, mandatory);

            if (eventType == type) return consumers;

            // if TEvent is base type get consumers of the concrete event type
            var allConsumers = new List<IEventConsumer>(consumers);

            consumers = ResolveConsumers(eventType, contextType, false);
            if (consumers.Any())
                allConsumers.AddRange(consumers);

            return allConsumers.ToArray();
        }

        private IEventConsumer[] ResolveConsumers(Type eventType, Type contextType, bool required)
        {
            var type = typeof(IEventConsumer<,>);
            var genericType = type.MakeGenericType(eventType, contextType);
            var consumers = _factory.ResolveAll(genericType);

            if (required && consumers.Length == 0)
                throw new NoEventConsumersException(genericType);

            return consumers;
        }
    }

    public static class EventPublisherExtensions
    {
        public static bool AnyConsumers<TEvent, TContext>(this IEventPublisher publisher, IEvent @event)
            where TEvent : class, IEvent
        {
            var eventType = @event.GetType();
            var result = publisher.AnyConsumers(eventType, typeof(TContext));
            if (result || eventType == typeof(TEvent))
                return result;

            return publisher.AnyConsumers(typeof(TEvent), typeof(TContext));
        }

        public static void Publish(this IEventPublisher publisher, IEvent @event)
        {
            publisher.Publish(@event, new PublishContext() { Mode = PublishMode.Isolated });
        }
    }
}