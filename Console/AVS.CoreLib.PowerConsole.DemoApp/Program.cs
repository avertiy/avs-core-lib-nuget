using AVS.CoreLib.Abstractions.Bootstrap;
using AVS.CoreLib.PowerConsole.Bootstrapping;
using AVS.CoreLib.PowerConsole.DemoApp.Services;
using AVS.CoreLib.Trading;
using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.PowerConsole.DemoApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //ColorScheme.ApplyScheme(ColorScheme.Classic);
            Bootstrap.ConfigureServices(delegate (ServiceCollection services)
                {
                    services
                        //.AddLogging(x => x.AddConsoleLogger())
                        .AddTradingCore()
                        //.AddTransient<ITestService, FileLoggerTestService>()
                        .AddTransient<IDemoService, XFormatDemoService>()
                        .AddTransient<IDemoService, ConsoleFeaturesDemoService>();
                })
                .RunAllTest()
                .RunAllDemo();
            PowerConsole.PressEnterToExit();
        }
    }
}
