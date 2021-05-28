using System;
using System.Collections.Generic;
using System.IO;
//using AVS.CoreLib.ConsoleTools.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.ConsoleTools.Bootstrapping
{
    internal class ServiceProviderBuilder
    {
        /// <summary>
        /// setup DI
        /// </summary>
        public static IServiceProvider BuildServiceProvider(
            Action<HostBuilderContext, IServiceCollection> registerServices,
            Action<HostBuilderContext, ILoggingBuilder> configureLogging = null
        )
        {
            var services = new ServiceCollection();
            var ctx = CreateHostBuilderContext();

            if (configureLogging == null)
                services.AddLogging(builder =>
                {
                    builder.AddConfiguration(ctx.Configuration.GetSection("Logging"));
                    //builder.AddConsoleLogger();
                });
            else
                services.AddLogging(builder =>
                {
                    builder.AddConfiguration(ctx.Configuration.GetSection("Logging"));
                    configureLogging(ctx, builder);
                });


            services.AddSingleton(ctx.HostingEnvironment);
            services.AddSingleton(ctx.Configuration);
            services.AddSingleton(ctx);
            services.AddOptions();

            registerServices(ctx, services);
            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }

        private static IConfiguration GetConfiguration()
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder().AddInMemoryCollection();
            configurationBuilder.AddJsonFile("appsettings.json", true, true);
            configurationBuilder.AddEnvironmentVariables();
            return configurationBuilder.Build();
        }

        private static string ResolveContentRootPath(string contentRootPath, string basePath)
        {
            if (string.IsNullOrEmpty(contentRootPath))
                return basePath;
            if (Path.IsPathRooted(contentRootPath))
                return contentRootPath;
            return Path.Combine(Path.GetFullPath(basePath), contentRootPath);
        }

        private static HostingEnvironment CreateHostingEnvironment(IConfiguration config)
        {
            return new HostingEnvironment()
            {
                ApplicationName = config[HostDefaults.ApplicationKey],
                EnvironmentName = config[HostDefaults.EnvironmentKey] ?? Environments.Production,
                ContentRootPath = ResolveContentRootPath(config[HostDefaults.ContentRootKey], AppContext.BaseDirectory)
            };
        }

        private static HostBuilderContext CreateHostBuilderContext()
        {
            var config = GetConfiguration();
            var hostingEnvironment = CreateHostingEnvironment(config);

            return new HostBuilderContext(new Dictionary<object, object>())
            {
                HostingEnvironment = hostingEnvironment,
                Configuration = config
            };
        }
    }
}