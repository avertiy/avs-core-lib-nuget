using System.Diagnostics;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.BootstrapTools
{
    //3 ways of use Bootstrap:
    //Bootstrap.ConfigureServices(...);
    //Bootstrap.StartWith<StartupService>();
    //Bootstrap.UseStartup<Startup>().StartWith<StartupService>();

    /// <summary>
    /// Console utility to quickly bootstrap your console app with DI
    /// (Microsoft.Extensions.DependencyInjection) without getting into burden of the .NET core Host/WebHost builder complexities. 
    /// <code>
    ///     //usages: 
    ///     // if you just need to build a service provider: 
    ///     Bootstrap.ConfigureServices(services => {..register your services..});
    /// 
    ///     // if want to start with a startup service:
    ///     Bootstrap.Default().StartWith&lt;StartupService&gt;(services => {..register your services..});
    /// 
    ///     // if want to use a Startup class to configure services (like in .net core 3.1 style)
    ///     Bootstrap.Default().UseStartup&lt;Startup&gt;().StartWith(..);
    /// </code>
    /// StartupService <seealso cref="StartupServiceBase"/>.
    /// Startup <seealso cref="StartupServiceBase"/>.
    /// </summary>
    public class Bootstrap
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
        ///     Bootstrap.Start&lt;StartupService&gt;(services =>
        ///     {
        ///         services.AddXXX(..)
        ///     });
        /// </code>
        /// </summary>
        public static void StartWith<TStartupService>(Action<IServiceCollection> register, Action<IServiceProvider>? configure = null, string culture = "en-US")
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
                PrintError(ex, $"Bootstrap::Run{typeof(TStartupService).Name}>() configure services failed");
                throw;
            }

            try
            {
                startupService.Start();
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
        ///    Bootstrap.Run(sp=> {Console.WriteLine("Hello World"); sp.GetService();})
        /// };
        /// </code>
        public Bootstrap UseStartup<TStartup>(Action<IServiceProvider>? action = null) where TStartup : IStartup, new()
        {
            SetDefaultColorScheme();
            SetupCulture("en-US");
            try
            {
                var startup = new TStartup();
                var services = new ServiceCollection();
                startup.RegisterServices(services);
                var serviceProvider = services.BuildServiceProvider();
                startup.ConfigureServices(serviceProvider);
                action?.Invoke(serviceProvider);
                return this;
            }
            catch (Exception ex)
            {
                PrintError(ex, $"Bootstrap::{nameof(UseStartup)}<{typeof(TStartup).Name}>() failed");
                throw;
            }
        }

        [DebuggerStepThrough]
        public static Bootstrap Default(string culture = "en-US")
        {
            SetupCulture(culture);
            SetDefaultColorScheme();
            var bootstrap = new Bootstrap();
            return bootstrap;
        }

        private static void SetupCulture(string culture = "en-US")
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
        }

        private static void SetDefaultColorScheme()
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

    //may be in future version can make Bootstrap overrideable
    //Bootstrap.PowerConsoleSetup().ConfigureServices();
    //Bootstrap.Default().StartWith<StartupService>();
    //Bootstrap.Default().UseStartup<Startup>().StartWith<StartupService>();
}