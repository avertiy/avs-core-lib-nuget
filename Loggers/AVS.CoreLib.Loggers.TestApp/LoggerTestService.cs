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
            //AnsiCodesHelper.PrintPalete(false);

            var obj = new
            {
                List = new List<object>() { new { A = 1 }, new { B = "abc" } },
                Prop1 = 25,
                Prop2 = 0.001m,
                Prop3 = "prop value"
            };

            using (var scope = _logger.BeginScope($"test logger scope"))
            {
                //_logger.LogInformation("test info log {colored message:-Cyan} {100500:-Red}");
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
            _logger.LogInformation($"{nameof(NestedService)} info log  {{colored message:-Blue}} {{100500:-Green}}");
            _logger.LogWarning($"{nameof(NestedService)} warning log {{colored message:-Yellow}}");
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