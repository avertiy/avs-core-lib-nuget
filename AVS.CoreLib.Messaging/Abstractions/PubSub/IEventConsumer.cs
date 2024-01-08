#nullable enable
using System;
using AVS.CoreLib.Abstractions.Messaging;

namespace AVS.CoreLib.Messaging.Abstractions.PubSub
{
    /// <summary>
    /// Represents an event consumer
    /// </summary>
    public interface IEventConsumer
    {
        void Handle(IEvent @event, IPublishContext context);
    }

    /// <summary>
    /// Represents an event consumer with a concrete TEvent and TContext types
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


    /// <summary>
    /// Resolves event consumer(s), if any registered in DI as <see cref="IEventConsumer{TEvent,TContext}"/>" 
    /// <seealso cref="AVS.CoreLib.Messaging.PubSub.EventPublisher"/>    
    /// </summary>
    public interface IEventConsumerFactory
    {
        IEventConsumer[] ResolveAll(Type type);
    }
}