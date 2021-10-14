using System;
using System.IO;
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

        protected virtual IConfiguration LoadAppSettings(bool reloadOnChange = false)
        {
            var builder = new ConfigurationBuilder().AddInMemoryCollection();
            CreateSecretsDirectory();
            var configuration = builder
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange)
                .AddJsonFile($"appsettings.{ENVIRONMENT}.json", optional: true, reloadOnChange)
                .AddEnvironmentVariables()
                .AddJsonFile(UserSecretsPath, optional: true, reloadOnChange)
                .Build();
            return configuration;
        }

        /// <summary>
        /// avoid using common path for secrets due to common location like microsoft/UserSecrets/.. kind of dangerous
        /// malicious software will target that path to get all user secrets for all apps at once
        /// </summary>
        protected virtual string UserSecretsPath
        {
            get
            {
                var userFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
                return Path.Combine(userFolder, ".secrets", AppName, "secrets.json");
            }
        }

        protected virtual string AppName => Assembly.GetEntryAssembly()?.GetName().Name;

        private void CreateSecretsDirectory()
        {
            if (IsDevelopment)
            {
                var path = UserSecretsPath;
                var dirPath = Path.GetDirectoryName(path);
                if (dirPath != null && !Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                if (!File.Exists(path))
                {
                    File.Create(path);
                }
            }
        }

        public virtual void ConfigureServices(IServiceProvider services)
        {
        }

        protected virtual string ENVIRONMENT => Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT") ?? "dev";
        protected virtual bool IsDevelopment => ENVIRONMENT == "dev" || ENVIRONMENT == "development";
    }
}