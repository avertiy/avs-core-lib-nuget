using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AVS.CoreLib.ConsoleTools.Bootstrapping
{
    public static class ServiceCollectionExtension
    {
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
            string name = null,
            Action<TOptions> configure = null)
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

        public static IServiceCollection AddOptions<TOptions>(this IServiceCollection services, Action<TOptions> configureOptions, string name = null)
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
    }
}