using System;
using System.Collections.Generic;
using System.Threading;
using AVS.CoreLib.Abstractions;
using AVS.CoreLib.Logging.ColorFormatter;
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

        public void Test()
        {
            ConsoleLogProfiler.Enabled = true;
            _logger.LogCritical("simple critical log");

            //Console.ForegroundColor = ConsoleColor.DarkGray;
            //AnsiCodesHelper.PrintAnsiColors();
            //AnsiCodesHelper.PrintPalete();

            ConsoleFormatterFeaturesTests();
            TagFormattingTests();
            
            ArgHighlightTests();

            var log = ConsoleLogProfiler.Flush();
        }

        private void TagFormattingTests()
        {
            // header tags
            _logger.LogInformation("<H1>simple header test arg:{arg1}</H1>", "string value");
            _logger.LogInformation("\r\n<H1>new lines header test arg:{arg1}</H1>\r\n", 1.12345);
            _logger.LogInformation("<H1>multiline header test: {arg1:C}\r\n some text\r\n</H1>", 1.12345);
            _logger.LogInformation("<H1>header test double args: {arg1:C}{arg2:N3}\r\n some text\r\n</H1>", 99.123, 1.12345);

            //color tags
            _logger.LogInformation("<Green>simple color tag test arg:{arg1}</Green>", "string value");
            _logger.LogInformation("<bgYellow>bg yellow test arg:{arg1}</bgYellow>", "string value");
            _logger.LogInformation("nested tags test: <Blue><bgGray>blue + bg gray arg:{arg1:C}</Blue></bgGray>", 1.12345);
            _logger.LogInformation("nested tags test: <Red>red <Green>green<bgBlue>blue</bgBlue></Green></Red>");
            _logger.LogInformation("<RGB:120,100,200>rgb text here {arg:N3}</RGB>", 1.12345);
            _logger.LogInformation("<RGB:120,100,200>rgb <Yellow>yellow color tag {arg}</Yellow></RGB>", "string value");
            //
        }

        private void ConsoleFormatterFeaturesTests()
        {
            _logger.LogDebug("simple debug log");
            _logger.LogInformation("simple info log");
            _logger.LogWarning("simple warning log");
            _logger.LogError("simple error log");
            _logger.LogError(new ArgumentException("test error"),"error log with error");
            _logger.LogCritical("simple critical log");

            //new line
            _logger.LogInformation("");
            _logger.LogInformation("new line test");
            _logger.LogInformation("\r\nanother line test");
            _logger.LogInformation("\r\n\r\ndouble new lines test at the begging");

            //stamps
            _logger.LogInformation("[time]");

            // scope
            using (var scope = _logger.BeginScope($"test logger scope"))
            {
                _logger.LogWarning("log inside outer scope {arg:P}", 1.117);
                //nested scope
                _service.Print();
                _logger.LogError("test error log inside outer scope");
            }

            _logger.LogInformation("info log out of scope");
        }

        

        private void ArgHighlightTests()
        {
            var obj = new
            {
                List = new List<object>() { new { A = 1 }, new { B = "abc" } },
                Prop1 = 25,
                Prop2 = 0.001m,
                Prop3 = "prop value"
            };

            _logger.LogInformation("test {arg1:C} some text {arg2}{arg3:N2}", 1, "string value", 3.3333);
            _logger.LogInformation("\r\n\r\nTesting {strategy}, timeframe: {timeframe}; parameters: {parameters}", "name", "H1", "Pool Fees: 10%");
            _logger.LogWarning("simple argument: #{arg}", 15);
            _logger.LogInformation("simple $colored argument: {arg}:--Yellow$", (-5).ToString("C"));
            _logger.LogInformation(" - processed $bars #{bars}; trades count #{trades};:--Yellow$", 100, 25);
            _logger.LogInformation("info log with simple arg1: {arg} arg2: {arg2}", 1.25, (-22).ToString());
            _logger.LogInformation("info log with arguments: {arg1} some text {arg2}{arg3}", 1.ToString("P"), obj, "argument long text asdad asd asd asdasdadadsd");
            _logger.LogInformation($"info log with json text: {obj.ToJsonString()}");
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