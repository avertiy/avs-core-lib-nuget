using System;
using System.Diagnostics;
using AVS.CoreLib.Abstractions.Messaging;
using AVS.CoreLib.Abstractions.Messaging.PubSub;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.Messaging.PubSub
{
    /// <summary>
    /// Represent an event consumer
    /// <remarks>Don't forget to register consumer in DI</remarks>
    /// </summary>
    public abstract class EventConsumer<TEvent, TContext> : IEventConsumer<TEvent, TContext>
        where TEvent : IEvent
        where TContext : IPublishContext
    {
        protected ILogger Logger { get; }

        protected EventConsumer(ILogger logger)
        {
            Logger = logger;
        }

        [DebuggerStepThrough]
        void IEventConsumer<TEvent, TContext>.Handle(TEvent @event, TContext context)
        {
            try
            {
                this.Handle(@event, context);
            }
            catch (Exception ex)
            {
                OnError(ex, @event, context);
            }
        }

        [DebuggerStepThrough]
        public void Handle(IEvent @event, IPublishContext context)
        {
            TEvent e = default;
            TContext ctx = default;
            try
            {
                e = (TEvent)@event;
                ctx = (TContext)context;
                this.Handle(e, ctx);
            }
            catch (InvalidCastException castException)
            {
                Logger.LogError(EventCodes.HandleEvent, castException, "Failed to handle {event} {context}", @event, context);
                //invalid cast exception should never happen
                //in case it happens something is wrong in pub/sub mechanism
                throw;
            }
            catch (Exception ex)
            {
                OnError(ex, e, ctx);
            }
        }

        protected abstract void Handle(TEvent @event, TContext context);

        protected virtual void OnError(Exception ex, TEvent @event, TContext context)
        {
            Logger.LogError(EventCodes.HandleEvent, ex, "Failed to handle {event} {context}", @event, context);
        }
    }
}