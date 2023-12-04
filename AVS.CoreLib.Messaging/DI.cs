using AVS.CoreLib.Abstractions.Messaging.CommandBus;
using AVS.CoreLib.Abstractions.Messaging.PubSub;
using AVS.CoreLib.Messaging.CommandBus;
using AVS.CoreLib.Messaging.PubSub;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AVS.CoreLib.Messaging
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add MessageBus infrastructure
        /// You can implement message handlers <see cref="ICommandHandler"/>
        /// and send messages via <see cref="ICommandBus"/>
        /// </summary>
        public static IServiceCollection AddMessageBus(this IServiceCollection services)
        {
            // register service factory delegates
            services.AddSingleton<ServiceFactory>(sp => sp.GetService);
            // register a required part to automatically resolve message handlers 
            services.AddSingleton<ICommandHandlerFactory, CommandHandlerFactory>();
            services.AddSingleton<ICommandBus, CommandBus.CommandBus>();
            return services;
        }

        /// <summary>
        /// Add PubSub infrastructure
        /// You can implement event consumers <see cref="IEventConsumer{TEvent,TContext}"/>
        /// and publish events via <see cref="IEventPublisher"/>
        /// </summary>
        public static void AddEventPublisher(this IServiceCollection services)
        {
            services.AddSingleton<ServiceFactoryResolveAll>(sp => sp.GetServices);
            services.TryAddSingleton<IEventConsumerFactory, EventConsumerFactory>();
            services.TryAddTransient<IEventPublisher, EventPublisher>();
        }

        public static void AddNullEventPublisher(this IServiceCollection services)
        {
            services.TryAddTransient<IEventPublisher, NullEventPublisher>();
        }
    }
}
