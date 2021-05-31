using System;
using AVS.CoreLib.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.ConsoleTools.Bootstrapping
{
    public abstract class StartupBase : IStartup
    {
        protected virtual string ContentRootPath => AppContext.BaseDirectory;
        private IConfiguration _configuration;

        protected IConfiguration Configuration => _configuration ??= LoadAppSettings();

        public virtual void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            AddOptions(services);
            services.AddLogging(builder =>
            {
                builder.AddConfiguration(Configuration.GetSection("Logging"));
                ConfigureLogging(builder);
            });

            // coravel things
            AddInvocables(services);
            AddEventConsumers(services);
        }

        protected virtual void AddOptions(IServiceCollection services)
        {
            services.AddOptions();
        }


        protected void AddOptions<TOptions>(IServiceCollection services, string configSectionKey) where TOptions : class
        {
            var section = Configuration.GetSection(configSectionKey);
            services.Configure<TOptions>(section.Bind);
        }

        protected virtual void AddEventConsumers(IServiceCollection services)
        {
            //services.AddScoped<EventHandler>(); // handler implement IListener<TEvent>
        }

        protected virtual void AddInvocables(IServiceCollection services)
        {
            //services.AddScoped<IInvocable>();
        }

        protected virtual void ConfigureLogging(ILoggingBuilder builder)
        {
        }

        protected virtual IConfiguration LoadAppSettings(string appSettings = "appsettings.json")
        {
            var configurationBuilder = new ConfigurationBuilder().AddInMemoryCollection();
            configurationBuilder.AddJsonFile(appSettings, true, true);
            configurationBuilder.AddEnvironmentVariables();
            return configurationBuilder.Build();
        }

        public virtual void ConfigureServices(IServiceProvider services)
        {

        }

    }
}