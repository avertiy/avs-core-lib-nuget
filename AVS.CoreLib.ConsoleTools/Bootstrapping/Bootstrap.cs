using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions;
using AVS.CoreLib.PowerConsole.Utilities;
using Microsoft.Extensions.Hosting;
using Console = AVS.CoreLib.PowerConsole.PowerConsole;
using BootstrapBase = AVS.CoreLib.PowerConsole.Bootstrapping.Bootstrap;

namespace AVS.CoreLib.ConsoleTools.Bootstrapping
{
    public class Bootstrap : BootstrapBase
    {
        /// <summary>
        /// To run program as windows service
        /// https://www.stevejgordon.co.uk/running-net-core-generic-host-applications-as-a-windows-service
        /// </summary>
        public static void RunAsService<TStartup>(string[] args)
            where TStartup : IStartup, new()
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));
            var host = CreateHost<TStartup>(args, isService);
            host.Run();
        }

        /// <summary>
        /// To run program as windows service
        /// </summary>
        public static Task RunAsServiceAsync<TStartup>(string[] args)
            where TStartup : IStartup, new()
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));
            var host = CreateHost<TStartup>(args, isService);
            return host.RunAsync();
        }

        /// <summary>
        /// to execute service when host is started
        /// implement IHostedService interface and register it as AddHostedService
        /// </summary>
        public static void RunAsHost<TStartup>(string[] args)
            where TStartup : IStartup, new()
        {
            var host = CreateHost<TStartup>(args, false);
            host.Run();
        }

        /// <summary>
        /// to execute service when host is started
        /// implement IHostedService interface and register it as AddHostedService
        /// </summary>
        public static Task RunAsHostAsync<TStartup>(string[] args)
            where TStartup : IStartup, new()
        {
            var host = CreateHost<TStartup>(args, false);
            return host.RunAsync();
        }

        private static IHost CreateHost<TStartup>(string[] args, bool windowsService = false)
            where TStartup : IStartup, new()
        {
            try
            {
                Bootstrap.SetCulture("en-US");
                var hostBuilder = (HostBuilder)Host.CreateDefaultBuilder(args);
                var startup = new TStartup();
                hostBuilder.ConfigureServices(startup.RegisterServices);

                if (windowsService)
                {
                    hostBuilder.UseWindowsService();
                }
                else
                {
                    hostBuilder.UseConsoleLifetime();
                    Console.SetDefaultColorScheme(ColorScheme.DarkGray);
                    Console.ApplyColorScheme(ColorScheme.DarkGray);
                }

                var host = hostBuilder.Build();

                startup.ConfigureServices(host.Services);

                if (!windowsService)
                    Console.WriteDebug("host is built");
                return host;
            }
            catch (Exception ex)
            {
                Console.WriteError(ex);
                return null;
            }
        }
    }
}