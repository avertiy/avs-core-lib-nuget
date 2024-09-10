using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AVS.CoreLib.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddOptions<TOptions>(this IServiceCollection services, IConfigurationSection section)
            where TOptions : class, new()
        {
            services.Configure<TOptions>(section.Bind);
        }

        /// <summary>
        /// register singleton TOptions type 
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> to get config section.</param>
        /// <param name="name">The Name to register named options (leave blank to register as a default) [optional]</param>
        /// <param name="configure">The configure options action [optional]</param>
        /// <returns></returns>
        public static IServiceCollection AddOptions<TOptions>(this IServiceCollection services,
            IConfiguration configuration,
            string? name = null,
            Action<TOptions>? configure = null)
            where TOptions : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var optionsType = typeof(TOptions);
            var sectionKey = name == null ? optionsType.Name : $"{optionsType.Name}:{name}";
            var section = configuration.GetSection(sectionKey);
            var options = new ConfigureNamedOptions<TOptions>(name ?? string.Empty, o =>
            {
                section.Bind(o);
                configure?.Invoke(o);
            });

            services.AddSingleton<IConfigureOptions<TOptions>>(options);
            return services;
        }

        public static IServiceCollection AddOptions<TOptions>(this IServiceCollection services, Action<TOptions> configureOptions,
            string? name = null)
            where TOptions : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.AddSingleton<IConfigureOptions<TOptions>>(new ConfigureNamedOptions<TOptions>(name ?? string.Empty, configureOptions));
            return services;
        }

        /// <summary>
        /// Add service factory to register/resolve services of type <typeparamref name="TService"/> by string key
        /// e.g. services.AddServiceFactory&lt;IMyService&gt;((x, sp) => x == "key1" ? sp.GetService&lt;MyServiceA&gt;() : sp.GetService&lt;MyServiceB&gt;()); 
        /// </summary>
        /// <typeparam name="TService">an interface/abstraction</typeparam>
        /// <param name="services">IServiceCollection</param>
        /// <param name="func">factory to resolve TService</param>
        public static IServiceCollection AddServiceFactory<TService>(this IServiceCollection services, Func<string, IServiceProvider, TService> func)
        {
            return services.AddSingleton<Func<string, TService>>(sp => x => func(x, sp));
        }

        /// <summary>
        /// Register the last registration as its own type.
        /// </summary>
        /// <returns>The original <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.</returns>
        public static IServiceCollection AsSelf(this IServiceCollection services)
        {
            var lastRegistration = services.LastOrDefault();
            if (lastRegistration == null)
                return services;

            var implementationType = GetImplementationType(lastRegistration)
                                     ?? throw new InvalidOperationException($"implementation type is null");

            // When the last registration service type was already registered
            // as its implementation type, bail out.
            if (lastRegistration.ServiceType == implementationType)
                return services;

            if (lastRegistration.ImplementationInstance == null)
            {
                // Remove last registration
                services.Remove(lastRegistration);

                if (lastRegistration.ImplementationType == null)
                    return services;

                // Register "self" registration first
                if (lastRegistration.ImplementationFactory != null)
                {
                    // Factory-based
                    services.Add(new ServiceDescriptor(
                        lastRegistration.ImplementationType,
                        lastRegistration.ImplementationFactory,
                        lastRegistration.Lifetime));
                }
                else
                {
                    // Type-based
                    services.Add(new ServiceDescriptor(
                        lastRegistration.ImplementationType,
                        lastRegistration.ImplementationType,
                        lastRegistration.Lifetime));
                }

                // Re-register last registration, proxying our specific registration
                services.Add(new ServiceDescriptor(
                    lastRegistration.ServiceType,
                    provider => provider.GetService(implementationType)!,
                    lastRegistration.Lifetime));
            }
            else
            {
                // Register "self" registration as the same instance
                services.Add(new ServiceDescriptor(
                    implementationType,
                    lastRegistration.ImplementationInstance));
            }

            return services;
        }

        private static Type? GetImplementationType(ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationType != null)
                return descriptor.ImplementationType;

            if (descriptor.ImplementationInstance != null)
                return descriptor.ImplementationInstance.GetType();

            if (descriptor.ImplementationFactory != null)
                return descriptor.ImplementationFactory.GetType().GenericTypeArguments[1];

            return null;
        }
    }
}