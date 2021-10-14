using AVS.CoreLib.Abstractions;
using AVS.CoreLib.PowerConsole.Bootstrapping;
using AVS.CoreLib.PowerConsole.DemoApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.PowerConsole.DemoApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Bootstrap.ConfigureServices(services => services
                    .AddLogging(x => x.AddConsole())
                    .AddTransient<ITestService, FileLoggerTestService>()
                    .AddTransient<IDemoService, XFormatDemoService>()
                    .AddTransient<IDemoService, ConsoleFeaturesDemoService>())
                .RunAllTest()
                .RunAllDemo();
            PowerConsole.PressEnterToExit();
        }
    }
}
