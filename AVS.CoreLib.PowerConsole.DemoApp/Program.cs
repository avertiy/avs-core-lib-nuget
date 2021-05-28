using System;
using System.Threading.Tasks;
using AVS.CoreLib.ConsoleTools.Bootstrapping;
using AVS.CoreLib.FileLogger;
using AVS.CoreLib.PowerConsole.DemoApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AVS.CoreLib.PowerConsole.DemoApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Bootstrap.ConfigureServices(services =>
                    {
                        services
                            .AddLogging(configure => configure.AddFileLogger(x => x.BasePath = "."))
                            .AddTransient<FileLoggerTestService>();
                        //.AddTransient<XFormatDemoService>()
                        //.AddTransient<ConsoleFeaturesDemoService>();
                    })
                    .RunTest<FileLoggerTestService>();
                //.RunAllDemo();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
