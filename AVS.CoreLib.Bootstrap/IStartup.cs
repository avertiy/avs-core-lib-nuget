using System;
using System.Reflection;
using AVS.CoreLib.BootstrapTools.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.BootstrapTools;

public interface IStartup
{
    void RegisterServices(IServiceCollection services);
    void ConfigureServices(IServiceProvider sp);
}

/// <summary>
/// Startup base class provides basic startup stuff
/// </summary>
public abstract class StartupBase : IStartup
{
    protected virtual string ContentRootPath => AppContext.BaseDirectory;
    protected virtual string? AppName => Assembly.GetEntryAssembly()?.GetName().Name;
    protected virtual string ENVIRONMENT { get; set; } = "dev";
    protected virtual bool IsDevelopment => ENVIRONMENT is "dev" or "development";
    protected bool CustomUserSecretsEnabled { get; set; } = true;

    private IConfiguration? _configuration;
    protected IConfiguration Configuration => _configuration ??= SetupConfiguration(new ConfigurationBuilder());

    public virtual void RegisterServices(IServiceCollection services)
    {
        services.AddSingleton(Configuration);
        services.AddOptions();
        AddLogging(services);
    }

    public virtual void ConfigureServices(IServiceProvider sp)
    {

    }

    // override to configure logging
    protected virtual void ConfigureLogging(ILoggingBuilder builder)
    {
    }

    protected virtual IConfigurationRoot SetupConfiguration(IConfigurationBuilder builder)
    {
        builder.AddInMemoryCollection();
        builder.AddEnvironmentVariables();
        builder.AddAppSettings(ENVIRONMENT);
        if (CustomUserSecretsEnabled)
            builder.AddCustomUserSecrets();
        return builder.Build();
    }

    protected void AddLogging(IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.AddConfiguration(Configuration.GetSection("Logging"));
            ConfigureLogging(builder);
        });
    }
}