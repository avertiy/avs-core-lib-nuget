using System;
using AVS.CoreLib.Abstractions;
using AVS.CoreLib.ConsoleLogger;
using AVS.CoreLib.ConsoleTools.Bootstrapping;
using AVS.CoreLib.FileLogger;
using Microsoft.Extensions.DependencyInjection;

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
                                .AddFileLogger()
                                .AddConsoleLogger()
                                ;
                        })
                        .AddTransient<ITestService, FileLoggerTestService>();
                })
                .RunAllTest();
            Console.ReadLine();
        }
    }
}
