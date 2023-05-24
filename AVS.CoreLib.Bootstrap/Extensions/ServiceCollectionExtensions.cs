using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AVS.CoreLib.BootstrapTools.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// if you don't use <see cref="StartupBase"/> you can add all common services:
    ///     - build & register <see cref="IConfiguration"/>
    ///     - add services required to use <seealso cref="IOptions{TOptions}"/>
    ///     - add logging services 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configureLogging"></param>
    /// <param name="environment"></param>
    /// <param name="addCustomUserSecrets"></param>
    public static void RegisterCommonServices(this IServiceCollection services, Action<ILoggingBuilder>? configureLogging = null,  string environment = "dev",
        bool addCustomUserSecrets = true)
    {
        var configuration = services.AddConfiguration(environment, addCustomUserSecrets);
        services.AddSingleton(configuration);
        services.AddOptions();
        services.AddLogging(builder =>
        {
            builder.AddConfiguration(configuration.GetSection("Logging"));
            configureLogging?.Invoke(builder);
        });
    }

    /// <summary>
    /// build <see cref="IConfiguration"/> adding environment variables, appsettings and custom user secrets
    /// than register it as singleton
    /// </summary>
    public static IConfiguration AddConfiguration(this IServiceCollection services, string environment = "dev", bool addCustomUserSecrets = true)
    {
        var builder = new ConfigurationBuilder();
        builder.AddInMemoryCollection();
        builder.AddEnvironmentVariables();
        var env = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT") ?? environment;
        builder.AddAppSettings(env);
        if (addCustomUserSecrets)
            builder.AddCustomUserSecrets();
        var configuration = builder.Build();
        services.AddSingleton(configuration);
        return configuration;
    }

    /// <summary>
    /// register singleton TOptions type
    /// <code>
    ///     //usage example:
    ///     services.AddOptions&lt;MyOptions&gt;(config);
    ///     //DI injection
    ///     MyService(IOptions&lt;MyOptions&gt; options);
    ///     MyService(IOptionsMonitor&lt;MyOptions&gt; options);
    /// </code> 
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> to get config section.</param>
    /// <param name="name">Name to register named options [optional], if null the options type name will be used</param>
    /// <param name="configure">The configure options action [optional]</param>
    public static IServiceCollection AddOptions<TOptions>(this IServiceCollection services,
        IConfiguration configuration,
        string name = null,
        Action<TOptions> configure = null)
        where TOptions : class
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

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

    /// <summary>
    /// register singleton TOptions type
    /// <code>
    ///     //usage example:
    ///     services.AddOptions&lt;MyOptions&gt;(config);
    ///     //DI injection
    ///     MyService(IOptions&lt;MyOptions&gt; options);
    ///     MyService(IOptionsMonitor&lt;MyOptions&gt; options);
    /// </code> 
    /// </summary>
    public static IServiceCollection AddOptions<TOptions>(this IServiceCollection services, Action<TOptions> configureOptions,
        string name = null)
        where TOptions : class
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configureOptions == null)
            throw new ArgumentNullException(nameof(configureOptions));

        services.AddSingleton<IConfigureOptions<TOptions>>(new ConfigureNamedOptions<TOptions>(name ?? string.Empty, configureOptions));
        return services;
    }
}