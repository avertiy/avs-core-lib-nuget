using System;
using System.Collections.Generic;

namespace AVS.CoreLib.Abstractions.Messaging.PubSub
{
    /// <summary>
    /// Represents 1-to-many model of communication.
    /// Provides hook up points for subscribers to register to the event, and eventually handle the event if it's raised.
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// Publish <see cref="IEvent"/>
        /// does not require the event to be consumed
        /// does not throw an exception if the event isn't consumed.
        /// </summary>
        void Publish(IEvent @event);

        /// <summary>
        /// Publish <see cref="IEvent"/>
        /// does not require the event to be consumed
        /// does not throw an exception if the event isn't consumed.
        /// </summary>
        void Publish(IEvent @event, IPublishContext context);
    }

    public interface IEventConsumer
    {
        void Handle(IEvent @event, IPublishContext context);
    }

    public interface IEventConsumer<in TEvent, in TContext> : IEventConsumer
        where TEvent : IEvent
        where TContext : IPublishContext
    {
        void Handle(TEvent @event, TContext context);
    }

    public interface IPublishContext
    {
        /// <summary>
        /// True if the event must have at least one consumer 
        /// </summary>
        bool Mandatory { get; set; }
        /// <summary>
        /// True to continue publishing even if some consumer fails and unhandled exception occurs
        /// </summary>
        bool Isolated { get; set; }
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