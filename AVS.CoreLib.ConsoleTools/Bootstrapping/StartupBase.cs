using System;
using System.Reflection;
using AVS.CoreLib.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.ConsoleTools.Bootstrapping
{
    public abstract class StartupBase : IStartup
    {
        protected virtual string ContentRootPath => AppContext.BaseDirectory;
        protected virtual string AppName => Assembly.GetEntryAssembly()?.GetName().Name;
        protected virtual string ENVIRONMENT => Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT") ?? "dev";
        protected virtual bool IsDevelopment => ENVIRONMENT == "dev" || ENVIRONMENT == "development";

        protected bool UseCustomUserSecrets = true;

        private IConfiguration _configuration;
        protected IConfiguration Configuration => _configuration ??= SetupConfiguration(new ConfigurationBuilder());

        public virtual void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            AddOptions(services);
            AddLogging(services);
            AddHostedServices(services);
            // coravel things
            AddInvocables(services);
            AddEventConsumers(services);
        }

        protected virtual IConfigurationRoot SetupConfiguration(IConfigurationBuilder builder)
        {
            builder.AddInMemoryCollection();
            builder.AddEnvironmentVariables();
            builder.AddAppSettings(ENVIRONMENT);
            if (UseCustomUserSecrets)
                builder.AddCustomUserSecrets();
            return builder.Build();
        }

        public virtual void ConfigureServices(IServiceProvider services)
        {
        }

        protected virtual void ConfigureLogging(ILoggingBuilder builder)
        {
        }

        protected void AddLogging(IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(Configuration.GetSection("Logging"));
                ConfigureLogging(builder);
            });
        }

        protected virtual void AddHostedServices(IServiceCollection services)
        {
        }

        protected virtual void AddOptions(IServiceCollection services)
        {
            services.AddOptions();
        }

        protected virtual void AddEventConsumers(IServiceCollection services)
        {
            // handler should implement IListener<TEvent>
            // services.AddScoped<EventHandler>(); 
        }

        protected virtual void AddInvocables(IServiceCollection services)
        {
            // services.AddScoped<IInvocable>();
        }
    }
}