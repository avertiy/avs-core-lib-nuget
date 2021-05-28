using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.PowerConsole.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Console = AVS.CoreLib.PowerConsole.PowerConsole;

namespace AVS.CoreLib.PowerConsole.Bootstrapping
{
    public class Bootstrap
    {
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

        public static IServiceProvider Run<TStartup>() where TStartup : IStartup, new()
        {
            Console.SetDefaultColorScheme(ColorScheme.DarkGray);
            Console.ApplyColorScheme(ColorScheme.DarkGray);
            SetCulture("en-US");
            try
            {
                var startup = new TStartup();
                var services = new ServiceCollection();
                startup.RegisterServices(services);
                var serviceProvider = services.BuildServiceProvider();
                startup.ConfigureServices(serviceProvider);
                return serviceProvider;
            }
            catch (Exception ex)
            {
                Console.Print($"Bootstrap.Run<{typeof(TStartup).Name}>() failed", ConsoleColor.Red);
                Console.WriteError(ex);
                throw;
            }
        }

        /// <summary>
        /// Builds service provider <see cref="IServiceProvider"/>
        /// </summary>
        /// <example>
        /// Main(string[] args){
        ///    Run(sp=> {Console.WriteLine("Hello World"); sp.GetService();})
        /// };
        /// </example>
        public static void Run<TStartup>(Action<IServiceProvider> action) where TStartup : IStartup, new()
        {
            Console.SetDefaultColorScheme(ColorScheme.DarkGray);
            Console.ApplyColorScheme(ColorScheme.DarkGray);
            SetCulture("en-US");
            try
            {
                var startup = new TStartup();
                var services = new ServiceCollection();
                startup.RegisterServices(services);
                var serviceProvider = services.BuildServiceProvider();
                startup.ConfigureServices(serviceProvider);
                action(serviceProvider);
            }
            catch (Exception ex)
            {
                Console.Print($"Bootstrap.Run<{typeof(TStartup).Name}>() failed", ConsoleColor.Red);
                Console.WriteError(ex);
                throw;
            }
        }

        /// <summary>
        /// Builds service provider <see cref="IServiceProvider"/>
        /// </summary>
        /// <example>
        /// Main(string[] args){
        ///    Run(sp=> {Console.WriteLine("Hello World"); sp.GetService();})
        /// };
        /// </example>
        public static Task RunAsync<TStartup>(Func<IServiceProvider, Task> func) where TStartup : IStartup, new()
        {
            Console.SetDefaultColorScheme(ColorScheme.DarkGray);
            Console.ApplyColorScheme(ColorScheme.DarkGray);
            SetCulture("en-US");
            try
            {
                var startup = new TStartup();
                var services = new ServiceCollection();
                startup.RegisterServices(services);
                var serviceProvider = services.BuildServiceProvider();
                startup.ConfigureServices(serviceProvider);
                return func(serviceProvider);
            }
            catch (Exception ex)
            {
                Console.Print($"Bootstrap.Run<{typeof(TStartup).Name}>() failed", ConsoleColor.Red);
                Console.WriteError(ex);
                throw;
            }
        }

        public static void SetCulture(string culture = "en-US")
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
        }
    }
}