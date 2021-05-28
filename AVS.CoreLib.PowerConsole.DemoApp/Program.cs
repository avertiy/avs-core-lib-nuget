using AVS.CoreLib.Abstractions;
using AVS.CoreLib.ConsoleTools.Bootstrapping;
using AVS.CoreLib.PowerConsole.DemoApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.PowerConsole.DemoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrap.ConfigureServices(services =>
                {
                    services
                        .AddLogging(configure =>
                        {
                            configure
                                .AddConsole();
                        })
                        .AddTransient<ITestService, FileLoggerTestService>()
                        .AddTransient<IDemoService, XFormatDemoService>()
                        .AddTransient<IDemoService, ConsoleFeaturesDemoService>();
                })
                .RunAllTest()
                .RunAllDemo();
            PowerConsole.PressEnterToExit();
        }
    }
}
