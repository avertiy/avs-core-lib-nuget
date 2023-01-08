using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions;
using AVS.CoreLib.Abstractions.Bootstrap;
using AVS.CoreLib.PowerConsole.Utilities;
using Microsoft.Extensions.DependencyInjection;

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
                PowerConsole.PrintError(ex);
                throw;
            }
        }

        public static IServiceProvider Run<TStartup>() where TStartup : IStartup, new()
        {
            PowerConsole.SetDefaultColorScheme(ColorScheme.DarkGray);
            PowerConsole.ApplyColorScheme(ColorScheme.DarkGray);
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
                PowerConsole.PrintError(ex, $"Bootstrap.Run<{typeof(TStartup).Name}>() failed", printStackTrace: true);
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
            PowerConsole.SetDefaultColorScheme(ColorScheme.DarkGray);
            PowerConsole.ApplyColorScheme(ColorScheme.DarkGray);
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
                PowerConsole.Print($"Bootstrap.Run<{typeof(TStartup).Name}>() failed", ConsoleColor.Red);
                PowerConsole.PrintError(ex);
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
            PowerConsole.SetDefaultColorScheme(ColorScheme.DarkGray);
            PowerConsole.ApplyColorScheme(ColorScheme.DarkGray);
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
                PowerConsole.Print($"Bootstrap.Run<{typeof(TStartup).Name}>() failed", ConsoleColor.Red);
                PowerConsole.PrintError(ex);
                throw;
            }
        }

        public static void SetCulture(string culture = "en-US")
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
        }
    }
}