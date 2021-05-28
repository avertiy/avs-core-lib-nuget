using AVS.CoreLib.ConsoleLogger;
using AVS.CoreLib.ConsoleTools.Bootstrapping;
using AVS.CoreLib.FileLogger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.Loggers.TestApp
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
                                //.AddConsole()
                                .AddFileLogger()
                                .AddConsoleLogger()
                                ;
                        })
                        .AddTransient<ITestService, FileLoggerTestService>();
                })
                .RunAllTest()
                .PressEnterToExit();
        }
    }
}
