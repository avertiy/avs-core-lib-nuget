using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.PowerConsole.Bootstrapping;
using AVS.CoreLib.PowerConsole.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Console = AVS.CoreLib.PowerConsole.PowerConsole;

namespace AVS.CoreLib.ConsoleTools.Bootstrapping
{
    public static class Bootstrap
    {
        public static IBootstrap Bootstrapper = new Bootstrapper();
        public static IServiceProvider ConfigureServices(Action<ServiceCollection> configure)
        {
            try
            {
                var services = new ServiceCollection();
                configure(services);
                return services.BuildServiceProvider();
            }
            catch (Exception ex)
            {
                Console.WriteError(ex);
                throw;
            }
        }

        public static void Run<TStartup>() where TStartup : IStartup, new()
        {
            Console.SetDefaultColorScheme(ColorScheme.DarkGray);
            Console.ApplyColorScheme(ColorScheme.DarkGray);
            SetCulture("en-US");
            Bootstrapper.Run<TStartup>();
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
            Console.SetDefaultColorScheme(ColorScheme.DarkGray);
            Console.ApplyColorScheme(ColorScheme.DarkGray);
            SetCulture("en-US");
            Bootstrapper.Run<TStartup>(main);
        }

        /// <summary>
        /// Builds service provider <see cref="IServiceProvider"/>
        /// </summary>
        /// <example>
        /// Main(string[] args){
        ///    Run(sp=> {Console.WriteLine("Hello World"); sp.GetService();})
        /// };
        /// </example>
        public static Task RunAsync<TStartup>(Func<IServiceProvider, Task> main) where TStartup : IStartup, new()
        {
            Console.SetDefaultColorScheme(ColorScheme.DarkGray);
            Console.ApplyColorScheme(ColorScheme.DarkGray);
            SetCulture("en-US");
            return Bootstrapper.RunAsync<TStartup>(main);
        }

        public static void SetCulture(string culture = "en-US")
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
        }

        /// <summary>
        /// To run program as windows service
        /// https://www.stevejgordon.co.uk/running-net-core-generic-host-applications-as-a-windows-service
        /// </summary>
        public static void RunAsService<TStartup>(string[] args)
            where TStartup : IStartup, new()
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));
            Bootstrapper.Run<TStartup>(args, isService);
        }

        /// <summary>
        /// To run program as windows service
        /// </summary>
        public static Task RunAsServiceAsync<TStartup>(string[] args)
            where TStartup : IStartup, new()
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));
            return Bootstrapper.RunAsync<TStartup>(args, isService);
        }

        /// <summary>
        /// to execute service when host is started
        /// implement IHostedService interface and register it as AddHostedService
        /// </summary>
        public static void RunAsHost<TStartup>(string[] args)
            where TStartup : IStartup, new()
        {
            Bootstrapper.Run<TStartup>(args);
        }

        /// <summary>
        /// to execute service when host is started
        /// implement IHostedService interface and register it as AddHostedService
        /// </summary>
        public static Task RunAsHostAsync<TStartup>(string[] args)
            where TStartup : IStartup, new()
        {
            return Bootstrapper.RunAsync<TStartup>(args);
        }
    }
}