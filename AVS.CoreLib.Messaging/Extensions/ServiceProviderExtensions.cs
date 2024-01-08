using AVS.CoreLib.Abstractions.Messaging;
using AVS.CoreLib.Messaging.Abstractions.CommandBus;
using AVS.CoreLib.Messaging.Abstractions.PubSub;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AVS.CoreLib.Messaging.Extensions
{
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Add <see cref="ICommandHandler{TMessage}"/> as singleton if it has not been already registered
        /// </summary>
        public static void TryAddMessageHandler<TMessage, THandler>(this IServiceCollection services)
            where TMessage : class, ICommand
            where THandler : class, ICommandHandler<TMessage>
        {
            // Forward requests to THandler
            services.TryAddSingleton<ICommandHandler<TMessage>, THandler>();
        }
        /// <summary>
        /// Add <see cref="IEventConsumer{TEvent,TContext}"/> as singleton if it has not been already registered
        /// </summary>
        public static void TryAddEventConsumer<TEvent, TContext, TConsumer>(this IServiceCollection services)
            where TEvent : class, IEvent
            where TContext : class, IPublishContext
            where TConsumer : class, IEventConsumer<TEvent, TContext>
        {
            // Forward requests to THandler
            services.TryAddSingleton<IEventConsumer<TEvent, TContext>, TConsumer>();
        }
    }
}