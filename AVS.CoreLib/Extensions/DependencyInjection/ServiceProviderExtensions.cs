using System;
using AVS.CoreLib.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AVS.CoreLib.Extensions.DependencyInjection
{
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// note IOptions has scoped lifetime
        /// </summary>
        public static TOptions GetNamedOptions<TOptions>(
            this IServiceProvider sp, string name, bool required = true)
            where TOptions : class, new()
        {
            var snapshot = sp.GetService<IOptionsMonitor<TOptions>>();
            var options = snapshot.Get(name);
            if (required)
                Guard.AgainstNull(options, $"named {name} {typeof(TOptions).Name} required");
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

    }
}