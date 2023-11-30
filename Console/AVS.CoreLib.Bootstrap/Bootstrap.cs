using System.Globalization;
using AVS.CoreLib.BootstrapTools.Schedule;
using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.BootstrapTools
{
    /// <summary>
    /// Console utility to quickly bootstrap your console app with DI
    /// (Microsoft.Extensions.DependencyInjection) without getting into burden of the .NET core Host/WebHost builder complexities.
    /// <seealso cref="Startup{T}"/> and <see cref="StartupServiceBase"/>
    /// <code>
    ///     // usages: 
    ///     // if you just need to build a service provider: 
    ///     Bootstrap.ConfigureServices(services => {..register your services..});
    /// 
    ///     // full configuration using Startup class and StartupService 
    ///     Bootstrap.UseStartup&lt;TStartup&gt;(); 
    ///     
    ///     // TStartup could either implement IStartup or you can use Startup&lt;TStartupService&gt;    
    ///     Bootstrap.UseStartup&lt;Startup&lt;StartupService&gt;&gt;(); 
    ///     StartupService : StartupServiceBase {...}
    /// </code>
    /// </summary>
    public static class Bootstrap
    {
        private const string ANSI_RESET = "\u001b[0m";
        private const string ANSI_RED = "\u001b[91m";
        private const string ANSI_DARK_RED = "\u001b[31m";

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
                PrintError(ex, $"Bootstrap::{nameof(ConfigureServices)} failed");
                throw;
            }
        }

        /// <summary>
        /// Bootstrap set for you DarkGray console color scheme, a default en-US current culture, build <see cref="IServiceProvider"/>
        /// and run <see cref="IStartupService.Start"/> method
        /// Also configure DI and run Main() are wrapped in try catch blocks for you
        /// <code>    
        ///     Bootstrap.StartWith&lt;StartupService&gt;(services =>
        ///     {
        ///         services.AddXXX(..)
        ///     });
        /// </code>
        /// </summary>
        public static void StartWith<TStartupService>(Action<IServiceCollection> register, string[]? args = null,
            Action<IServiceProvider>? configure = null, string culture = "en-US")
            where TStartupService : class, IStartupService, new()
        {
            TStartupService startupService;
            try
            {
                SetDefaultColorScheme();
                SetupCulture(culture);
                var services = new ServiceCollection();
                services.AddSingleton<TStartupService>();
                register.Invoke(services);
                IServiceProvider serviceProvider = services.BuildServiceProvider();
                configure?.Invoke(serviceProvider);
                startupService = serviceProvider.GetRequiredService<TStartupService>();
            }
            catch (Exception ex)
            {
                PrintError(ex, $"Bootstrap::StartWith{typeof(TStartupService).Name}>() configure services failed");
                throw;
            }

            try
            {
                startupService.Start(args ?? Array.Empty<string>());
                PressEnterToExit();
            }
            catch (Exception ex)
            {
                PrintError(ex, $"{typeof(TStartupService).Name}.{nameof(startupService.Start)}() failed");
                throw;
            }
        }

        /// <summary>
        /// Builds service provider <see cref="IServiceProvider"/>
        /// </summary>
        /// <code>
        /// Main(string[] args){
        ///    Bootstrap.UseStartup{Startup}()
        /// };
        /// </code>
        public static IServiceProvider UseStartup<TStartup>(string[]? args = null,
            Action<IServiceProvider>? action = null) where TStartup : IStartup, new()
        {
            IServiceProvider serviceProvider;
            var startup = new TStartup();
            try
            {

                var services = new ServiceCollection();
                startup.RegisterServices(services);
                serviceProvider = services.BuildServiceProvider();
                startup.ConfigureServices(serviceProvider);
                action?.Invoke(serviceProvider);

            }
            catch (Exception ex)
            {
                PrintError(ex,
                    $"Bootstrap::{nameof(UseStartup)}<{typeof(TStartup).Name}>() services configuration failed");
                throw;
            }

            var scheduler = serviceProvider.GetService<IScheduler>();
            scheduler?.Start();

            Start(startup.GetStartupService(serviceProvider), args);

            scheduler?.Stop();
            return serviceProvider;
        }

        private static void Start(IStartupService? startupService, string[]? args)
        {
            if (startupService == null)
                return;
            try
            {
                startupService.Start(args ?? Array.Empty<string>());
                PressEnterToExit();
            }
            catch (Exception ex)
            {
                PrintError(ex, $"Bootstrap::{startupService.GetType().Name}.{nameof(Start)}() failed");
                throw;
            }
        }

        internal static void SetupCulture(string culture = "en-US")
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
        }

        internal static void SetDefaultColorScheme()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }

        internal static void PrintError(Exception ex, string message)
        {
            var type = ex.GetType().Name;
            var str = $"{message} [{type}:{ex.Message}]";
            Console.WriteLine($"\r\n{ANSI_RED}{str}{ANSI_RESET}");
            Console.WriteLine($"{ANSI_DARK_RED}{ex.StackTrace}{ANSI_RESET}\r\n");
        }

        private static void PressEnterToExit()
        {
            Console.Write("Press enter to quit.");
            Console.ReadLine();
        }
    }
}