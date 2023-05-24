using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using AVS.CoreLib.Abstractions.Bootstrap;
using AVS.CoreLib.PowerConsole.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.PowerConsole.Bootstrapping
{
    /// <summary>
    /// Console utility to quickly bootstrap your console app with DI
    /// (Microsoft.Extensions.DependencyInjection) without getting into burden of the .NET core Host/WebHost builder complexities. 
    /// <code>
    ///    //simple usage:
    ///     Bootstrap.Start&lt;StartupService&gt;(services => {..register your services..});
    /// </code>
    /// <seealso cref="StartupServiceBase"/>
    /// </summary>
    public class Bootstrap
    {
        /// <summary>
        /// Create <see cref="ServiceCollection"/> container, invoke register services than build service provider
        /// usage:
        /// <code>    
        ///     Bootstrap.ConfigureServices(services =>
        ///     {
        ///         services.AddSingleton&lt;StartupService&gt;();
        ///         services.AddScoped&lt;IAnotherService&gt;();
        ///     });
        /// </code>
        /// </summary>
        public static IServiceProvider ConfigureServices(Action<IServiceCollection> configure)
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

        /// <summary>
        /// Bootstrap set for you DarkGray console color scheme, a default en-US current culture, build <see cref="IServiceProvider"/>
        /// and run <see cref="IStartupService.Start"/> method
        /// Also configure DI and run Main() are wrapped in try catch blocks for you
        /// </summary>
        public static void Start<TStartupService>(Action<IServiceCollection> register, Action<IServiceProvider>? configure = null, string culture = "en-US")
            where TStartupService : class, IStartupService, new()
        {
            TStartupService startupService;
            try
            {
                PowerConsole.SetDefaultColorScheme(ColorScheme.DarkGray);
                PowerConsole.ApplyColorScheme(ColorScheme.DarkGray);
                SetCulture(culture);
                var services = new ServiceCollection();
                services.AddSingleton<TStartupService>();
                register.Invoke(services);
                IServiceProvider serviceProvider = services.BuildServiceProvider();
                configure?.Invoke(serviceProvider);
                startupService = serviceProvider.GetRequiredService<TStartupService>();
            }
            catch (Exception ex)
            {
                PowerConsole.PrintError(ex, $"Bootstrap::Run{typeof(TStartupService).Name}>() configure services failed", printStackTrace: true);
                throw;
            }

            try
            {
                startupService.Start();
                PowerConsole.PressEnterToExit();
            }   
            catch (Exception ex)
            {
                PowerConsole.PrintError(ex, $"{typeof(TStartupService).Name}.{nameof(startupService.Start)}() failed", printStackTrace: true);
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
        /// <code>
        /// Main(string[] args){
        ///    Bootstrap.Run(sp=> {Console.WriteLine("Hello World"); sp.GetService();})
        /// };
        /// </code>
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
        /// <code>
        /// Main(string[] args){
        ///    Bootstrap.RunAsync(sp=> {Console.WriteLine("Hello World"); sp.GetService();})
        /// };
        /// </code>
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

        protected static void SetCulture(string culture = "en-US")
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
        }
    }
}