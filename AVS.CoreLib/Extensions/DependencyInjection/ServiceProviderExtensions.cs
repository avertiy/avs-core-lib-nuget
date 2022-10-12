using System;
using System.Collections.Generic;
using AVS.CoreLib.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AVS.CoreLib.Extensions.DependencyInjection
{
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Get named options through IOptionsMonitor
        /// note IOptions has scoped lifetime
        /// </summary>
        /// <example>
        /// services.Configure&lt;ClientOptions&gt;("Client1", o=> config.GetSection("ClientOptions:Client1").Bind(o));
        /// services.Configure&lt;ClientOptions&gt;("Client2", o=> config.GetSection("ClientOptions:Client2").Bind(o));
        /// configure default options:
        /// services.Configure&lt;ClientOptions&gt;(o=> section.Bind(o)); 
        /// </example>
        public static TOptions GetOptions<TOptions>(
            this IServiceProvider sp, string name, bool required = true)
            where TOptions : class, new()
        {
            var snapshot = sp.GetService<IOptionsMonitor<TOptions>>();
            var options = snapshot.Get(name);
            if (required)
                Guard.AgainstNull(options, $"{typeof(TOptions).Name}:{name} has not been configured.");
            return options;
        }

        public static void Scoped<TService>(this IServiceProvider serviceProvider, Action<TService> action)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<TService>();
                action(service);
            }
        }

        /// <summary>
        /// Get <typeparamref name="TService"/> by name/key 
        /// </summary>
        /// <typeparam name="TService">an interface/abstraction</typeparam>
        /// <param name="serviceProvider">IServiceProvider</param>
        /// <param name="name">is a string argument used as a key to get a certain service instance/implementation of the <typeparamref name="TService"/></param>
        /// <returns></returns>
        public static TService GetNamedService<TService>(this IServiceProvider serviceProvider, string name)
        {
            var func = serviceProvider.GetService<Func<string, TService>>();
            if (func == null)
            {
                throw new InvalidOperationException(
                    $"Service factory Func<string,{nameof(TService)}> not found, use AddServiceFactory to register the service factory");
            }

            return func(name);
        }
    }

    
}