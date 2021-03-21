using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.PowerConsole.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Console = AVS.CoreLib.PowerConsole.PowerConsole;

namespace AVS.CoreLib.ConsoleTools.Bootstrapping
{
    //https://www.stevejgordon.co.uk/running-net-core-generic-host-applications-as-a-windows-service
    public class Bootstrap
    {
        /// <summary>
        /// To run program as windows service
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
            var host = CreateHost<TStartup>(args);
            host.Run();
        }

        /// <summary>
        /// to execute service when host is started
        /// implement IHostedService interface and register it as AddHostedService
        /// </summary>
        public static Task RunAsHostAsync<TStartup>(string[] args)
            where TStartup : IStartup, new()
        {
            var host = CreateHost<TStartup>(args);
            return host.RunAsync();
        }

        /// <summary>
        /// Builds service provider <see cref="IServiceProvider"/>
        /// </summary>
        /// <example>
        /// Main(string[] args){
        ///    Run(sp=> {Console.WriteLine("Hello World"); sp.GetService();})
        /// };
        /// </example>
        public static void Run<TStartup>(Action<IServiceProvider> main) where TStartup : IStartup, new()
        {
            ColorScheme.DarkGray.Apply();
            SetCurrentCulture("en-US");
            var startup = new TStartup();
            IServiceProvider sp =
                ServiceProviderBuilder.BuildServiceProvider(startup.RegisterServices, startup.ConfigureLogging);
            startup.ConfigureServices(sp);

            var hostEnv = sp.GetService<IHostEnvironment>();
            try
            {
                main(sp);
            }
            catch (Exception ex)
            {
                Console.Print($"{hostEnv.ApplicationName} unhandled error", ConsoleColor.Red);
                Console.WriteError(ex, true);
            }

            //Console.Print("Press enter to quit.");
            //Console.ReadLine();
        }
        /// <summary>
        /// Builds service provider <see cref="IServiceProvider"/>
        /// </summary>
        /// <example>
        /// Main(string[] args){
        ///    Run(sp=> {Console.WriteLine("Hello World"); sp.GetService();})
        /// };
        /// </example>
        public static async Task RunAsync<TStartup>(Func<IServiceProvider, Task> main) where TStartup : IStartup, new()
        {
            ColorScheme.DarkGray.Apply();
            SetCurrentCulture("en-US");
            var startup = new TStartup();
            var sp = ServiceProviderBuilder.BuildServiceProvider(startup.RegisterServices, startup.ConfigureLogging);
            startup.ConfigureServices(sp);

            var hostEnv = sp.GetService<IHostEnvironment>();
            try
            {
                await main(sp);
            }
            catch (Exception ex)
            {
                Console.Print($"{hostEnv.ApplicationName} unhandled error", ConsoleColor.Red);
                Console.WriteError(ex, true);
            }

            //Console.Print("Press enter to quit.");
            //Console.ReadLine();
        }

        public static void SetCurrentCulture(string culture = "en-US")
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
        }

        private static IHost CreateHost<TStartup>(string[] args, bool windowsService = false)
            where TStartup : IStartup, new()
        {
            try
            {
                var hostBuilder = (HostBuilder)Host.CreateDefaultBuilder(args);
                var startup = new TStartup();
                hostBuilder.ConfigureServices(startup.RegisterServices);
                hostBuilder.ConfigureLogging(startup.ConfigureLogging);

                if (windowsService)
                {
                    hostBuilder.UseWindowsService();
                }
                else
                {
                    hostBuilder.UseConsoleLifetime();
                    ColorScheme.DarkGray.Apply();
                }

                SetCurrentCulture("en-US");
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