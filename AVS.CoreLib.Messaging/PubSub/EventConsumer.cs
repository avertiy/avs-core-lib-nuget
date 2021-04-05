using System;
using AVS.CoreLib.Abstractions.Messaging;
using AVS.CoreLib.Abstractions.Messaging.PubSub;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.Messaging.PubSub
{
    public abstract class EventConsumer<TEvent, TContext> : IEventConsumer<TEvent, TContext>
        where TEvent : IEvent
        where TContext : IPublishContext
    {
        protected ILogger Logger { get; }

        protected EventConsumer(ILogger logger)
        {
            Logger = logger;
        }

        public void Handle(TEvent @event, TContext context)
        {
            try
            {
                this.HandleInternal(@event, context);
            }
            catch (Exception ex)
            {
                HandleError(ex, @event, context);
            }
        }

        public void Handle(IEvent @event, IPublishContext context)
        {
            try
            {
                this.HandleInternal((TEvent)@event, (TContext)context);
            }
            catch (Exception ex)
            {
                Logger.LogError(EventCodes.HandleEvent, ex, "Failed to handle {event} {context}", @event, context);
            }
        }

        protected virtual void HandleInternal(TEvent @event, TContext context)
        {

        }

        protected virtual void HandleError(Exception ex, TEvent @event, TContext context)
        {
            Logger.LogError(EventCodes.HandleEvent, ex, "Failed to handle {event} {context}", @event, context);
        }
    }
}