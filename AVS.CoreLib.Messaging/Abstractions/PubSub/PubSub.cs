#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions.Messaging;

namespace AVS.CoreLib.Messaging.Abstractions.PubSub
{
    /// <summary>
    /// Represents an event publisher in a pub/sub (one-to-many) communication model, which helps to decouple app modules from each other.
    /// Event producer(s) might publish event(s) by means of the event publisher. 
    /// Event publisher is responsible for resolving event consumers <see cref="IEventConsumer{TEvent,TContext}"/> corresponding to the event,
    /// and triggering consumer(s) to handle the event.    
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// publish an event & context to consumers (<see cref="IEventConsumer{TEvent,TContext}"/>)
        /// </summary>
        void Publish(IEvent @event, IPublishContext context);
        /// <summary>
        /// publish an event & context to consumers (<see cref="IEventConsumer{TEvent,TContext}"/>)
        /// </summary>
        /// <typeparam name="TEvent">
        /// when TEvent does not match @event.GetType() (i.e. TEvent is a base class)
        /// and you have consumers that handle all events of the TEvent type
        /// </typeparam>
        void Publish<TEvent>(TEvent @event, IPublishContext context) where TEvent : class, IEvent;

        /// <summary>
        /// publish an event & context to consumers (<see cref="IEventConsumer{TEvent,TContext}"/>)
        /// in async mode each consume 
        /// </summary>
        Task PublishAsync(IEvent @event, IPublishContext context, CancellationToken ct = default);

        /// <summary>
        /// Publish an <see cref="IEvent"/> with context argument
        /// consumers need to implement <see cref="IEventConsumer"/>
        /// </summary>
        /// <typeparam name="TEvent">
        /// when TEvent does not match @event.GetType() (i.e. TEvent is a base class)
        /// and you have consumers that handle all events of the TEvent type
        /// </typeparam>
        Task PublishAsync<TEvent>(TEvent @event, IPublishContext context, CancellationToken ct = default) where TEvent : class, IEvent;

        /// <summary>
        /// to bypass publishing you might check whether any consumers exist
        /// </summary>
        bool AnyConsumers(Type eventType, Type contextType);
    }


}