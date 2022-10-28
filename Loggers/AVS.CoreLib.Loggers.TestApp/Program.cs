using System;
using AVS.CoreLib.Abstractions;
using AVS.CoreLib.ConsoleLogger;
using AVS.CoreLib.ConsoleTools.Bootstrapping;
using AVS.CoreLib.Logging.ColorFormatter;
using AVS.CoreLib.Logging.ColorFormatter.Extensions;
using AVS.CoreLib.Logging.ColorFormatter.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AVS.CoreLib.Loggers.TestApp;

internal class Program
{
    private static void Main()
    {
        Bootstrap.ConfigureServices(services =>
            {
                AddConsoleFormatter(services)
                //AddConsoleLogger(services)
                    .AddTransient<NestedService, NestedService>()
                    .AddTransient<ITestService, LoggerTestService>();
            })
            .RunAllTest();
        Console.ReadLine();
    }

    private static IServiceCollection AddConsoleLogger(IServiceCollection services)
    {
        return services
            // .AddFileLogger()
            .AddLogging(builder =>
            {
                builder.AddConsoleLogger(x =>
                {
                    x.TimestampFormat = "T";
                    x.SingleLine = true;
                    //x.PrintLoggerName = false;
                    x.UseCurlyBracketsForScope = true;
                });
            });
    }

    private static IServiceCollection AddConsoleFormatter(IServiceCollection services)
    {
        return services
            .AddLogging(builder =>
            {
                builder
                    .AddConsoleWithColorFormatter(x =>
                    {
                        x.IncludeScopes = true;
                        x.SingleLine = true;
                        x.TimestampFormat = "T";
                        x.UseUtcTimestamp = true;
                    });
            });
    }

    private static IServiceCollection AddSimpleConsole(IServiceCollection services)
    {
        return services
            .AddLogging(builder =>
            {
                builder
                    .AddSimpleConsole(x =>
                    {
                        x.IncludeScopes = true;
                        x.SingleLine = true;
                        x.TimestampFormat = "G";
                        x.UseUtcTimestamp = true;
                    });
            });
    }

    private static void DefaultConsoleLoggerTest()
    {
        using ILoggerFactory loggerFactory =
            LoggerFactory.Create(builder =>
                builder.AddSimpleConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.SingleLine = true;
                    options.UseUtcTimestamp = true;
                    options.TimestampFormat = "dd/MM hh:mm:ss ";
                })
            );




        ILogger<Program> logger = loggerFactory.CreateLogger<Program>();
        ILogger logger2 = loggerFactory.CreateLogger<LoggerTestService>();
        using (logger.BeginScope("[my scope]"))
        {
            logger.LogInformation("Hello World!");
            logger.LogInformation("Logs contain timestamp and log level.");
            logger.LogInformation("Each log message is fit in a single line.");

            logger2.LogError("logger2 error test");
            logger2.LogWarning("logger2 warning test");
            logger2.LogInformation("\u001b[31mtest color output\u001b[0m");

            //Console.WriteLine("\u001b[31mtest color output\u001b[0m");
            //Console.WriteLine("\x1b[34mTEST\x1b[0m");
        }

    }
}