using System;
using System.Collections.Generic;
using System.Threading;
using AVS.CoreLib.Abstractions;
using AVS.CoreLib.Logging.ColorFormatter.Utils;
using AVS.CoreLib.Text.Extensions;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.Loggers.TestApp
{
    public class LoggerTestService : ITestService
    {
        private readonly ILogger _logger;
        private readonly NestedService _service;

        public LoggerTestService(ILogger<LoggerTestService> logger, NestedService service)
        {
            _logger = logger;
            _service = service;
        }

        public void Demo()
        {

        }

        public void Test()
        {
           Console.ForegroundColor = ConsoleColor.DarkGray;
            //AnsiCodesHelper.PrintAnsiColors();
            //AnsiCodesHelper.PrintPalete();

            var obj = new
            {
                List = new List<object>() { new { A = 1 }, new { B = "abc" } },
                Prop1 = 25,
                Prop2 = 0.001m,
                Prop3 = "prop value"
            };

            _logger.LogInformation("simple info log");
            _logger.LogInformation("\r\n\r\nTesting {strategy}, timeframe: {timeframe}; parameters: {parameters}",
                "name", "H1", "Pool Fees: 10%");
            _logger.LogInformation("");
            _logger.LogCritical("simple critical log\r\n");
            _logger.LogInformation("[time]");
            _logger.LogWarning("simple argument: #{arg}", 15);
            _logger.LogInformation("simple $colored argument: {arg}:--Yellow$", (-5).ToString("C"));
            _logger.LogInformation(" - processed $bars #{bars}; trades count #{trades};:--Yellow$", 100, 25);
            _logger.LogInformation("info log with simple arg1: {arg} arg2: {arg2}", 1.25, (-22).ToString());
            _logger.LogInformation("info log with arguments: {arg1} some text {arg2}{arg3}", 1.ToString("P"), obj, "argument long text asdad asd asd asdasdadadsd");
            _logger.LogInformation($"info log with json text: {obj.ToJsonString()}");


            using (var scope = _logger.BeginScope($"test logger scope"))
            {
                _logger.LogInformation("test info log $colored message:-Cyan$ ${arg}:-Red$", 500.ToString("C"));
                _logger.LogInformation("test info {arg1} log ${arg2}:-Cyan$ ${arg3}:-Red$", 150.ToString("C"), 500.ToString("C"), 1500.ToString("C"));
                //_logger.LogInformation("\u001b[31mtest ANSI colors output\u001b[0m");
               
                _logger.LogWarning("test warning log {obj}",obj);
                _logger.LogWarning($"test warning log {obj.ToJsonString()}");
                _service.Print();
                _logger.LogError("test error log");
            }

            _logger.LogInformation("out of scope");
        }

    }

    public class NestedService
    {
        private readonly ILogger _logger;

        public NestedService(ILogger<NestedService> logger)
        {
            _logger = logger;
        }

        public void Print()
        {
            _logger.LogInformation($"{nameof(NestedService)} info log  $colored message:-Blue$ $100500:-Green$");
            _logger.LogWarning($"{nameof(NestedService)} warning log $colored message:-Yellow$");
            _logger.LogError($"{nameof(NestedService)} error log");

            using (var scope = _logger.BeginScope($"{nameof(NestedService)} scope"))
            {
                _logger.LogInformation($"{nameof(NestedService)} info log");
                _logger.LogWarning($"{nameof(NestedService)} warning log");
                _logger.LogError($"{nameof(NestedService)} error log");
            }
        }
    }
}