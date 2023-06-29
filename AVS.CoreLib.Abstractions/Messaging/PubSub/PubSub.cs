#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AVS.CoreLib.Abstractions.Messaging.PubSub
{
    /// <summary>
    /// Represents pub/sub (one-to-many) communication model
    /// It helps to decouple modules from each other
    /// Provides hook up points for consumers(subscribers) to register to the event.
    /// Consumers <see cref="IEventConsumer{TEvent,TContext}"/> needs to be registered in DI
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
        Task PublishAsync<TEvent>(TEvent @event, IPublishContext context, CancellationToken ct = default) where TEvent: class, IEvent;

        /// <summary>
        /// to bypass publishing you might check whether any consumers exist
        /// </summary>
        bool AnyConsumers(Type eventType, Type contextType);
    }

    public interface IEventConsumer
    {
        void Handle(IEvent @event, IPublishContext context);
    }

    /// <summary>
    /// Represent an event consumer with a concrete TEvent and TContext types
    /// <remarks>
    /// Don't forget to register it in DI
    /// </remarks>
    /// </summary>
    public interface IEventConsumer<in TEvent, in TContext> : IEventConsumer
        where TEvent : IEvent
        where TContext : IPublishContext
    {
        void Handle(TEvent @event, TContext context);
    }
    
    public interface IPublishContext
    {
        PublishMode Mode { get; set; }
    }

    [Flags]
    public enum PublishMode
    {
        None = 0,
        /// <summary>
        /// when one of the consumer handle throws an exception, ignore the error and continue
        /// </summary>
        Isolated = 1,
        /// <summary>
        /// when no consumers found for the event throw an error
        /// </summary>
        Mandatory = 2,
        /// <summary>
        /// publish in async mode starting a new task for each consumer
        /// </summary>
        Parallel = 4,
        ParallelIsolated = Parallel | Isolated
    }

    /// <summary>
    /// Resolve from DI container event consumer(s)
    /// It is used by EventPublisher
    /// </summary>
    public interface IEventConsumerFactory
    {
        IEventConsumer[] ResolveAll(Type type);


    }
}