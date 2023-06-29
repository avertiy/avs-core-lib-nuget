using AVS.CoreLib.BootstrapTools;
using AVS.CoreLib.BootstrapTools.Extensions;
using AVS.CoreLib.PowerConsole.DemoApp.Services;
using AVS.CoreLib.Trading;
using Microsoft.Extensions.DependencyInjection;

namespace AVS.CoreLib.PowerConsole.DemoApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Bootstrap.ConfigureServices(services =>
                {
                    services
                        .AddTradingCore()
                        .AddTransient<ITestService, PrinterTestService>()
                        .AddTransient<ITestService, PrintTableTestService>()
                        .AddTransient<ITestService, XFormatTestService>()
                        .AddTransient<ITestService, ConsoleFeaturesTestService>();
                })
                .RunAllTest(10_000);
            PowerConsole.PressEnterToExit();
        }
    }
}
