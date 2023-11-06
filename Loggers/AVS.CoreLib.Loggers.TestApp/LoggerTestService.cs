using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.BootstrapTools;
using AVS.CoreLib.Debugging;
using AVS.CoreLib.Extensions.Stringify;
using AVS.CoreLib.Logging.ColorFormatter.Utils;
using AVS.CoreLib.PowerConsole.Extensions;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.Loggers.TestApp
{
    public class LoggerTestService : TestService
    {
        private readonly ILogger _logger;
        private readonly NestedService _service;

        public LoggerTestService(ILogger<LoggerTestService> logger, NestedService service)
        {
            _logger = logger;
            _service = service;
        }

        public override void Test(string[] args)
        {
            ConsoleLogProfiler.Enabled = true;
            TestArgsColorFormatter();
            ArgsFormatTests();
            ConsoleFormatterFeaturesTests();
            TagFormattingTests();
            
            ArgHighlightTests();
            TestDumpObject();
            var log = ConsoleLogProfiler.Flush();
        }

        private void TestDumpObject()
        {
            var arr = new object[] { "1", 2, 3.0m, 54, 6 };
            var list = new List<object>() { "|", "123213", new object(), new { RRR = 11 } };
            var obj = new { Prop1 = "abc", Price = 252.2, Prop3 = arr, Prop4 = list };
            _logger.LogInformation("Dump object test {obj}", obj.Dump());
        }

        private void TestArgsColorFormatter()
        {
            _logger.LogInformation("enum value:{arg1:D}; enum value with custom format:{arg2:G}", ConsoleColor.Cyan, ConsoleColor.DarkYellow);
            //_logger.LogInformation("enum value:{arg1:D}; enum value with custom format:{arg2:G}", ConsoleColor.Cyan, ConsoleColor.DarkYellow);
            _logger.LogInformation("string:{arg1}; currency:{arg2:C}", "my-str", 10.21m);
            var dict = new Dictionary<string, object>();

            var str = "string:my-str; currency:$10.21";
            var state = dict.ToList();
            var formatter = new ArgsColorFormatter() { Message = str, ColorProvider = new ColorProvider() };
            formatter.Init(state);
            var msg = formatter.FormatMessage();

            //var msg2 = str.FormatMessage(state, new ColorsProvider());
            //Assert.True(msg,msg2);

        }

        private void TagFormattingTests()
        {
            // header tags
            _logger.LogInformation("<H1>simple header test arg:{arg1}</H1>", "string value");
            _logger.LogInformation("\r\n<H1>new lines header test arg:{arg1}</H1>\r\n", 1.12345);
            _logger.LogInformation("<H1>multiline header test: {arg1:C}\r\n some text\r\n</H1>", 1.12345);
            _logger.LogInformation("<H1>header test double args: {arg1:C}{arg2:N3}\r\n some text\r\n</H1>", 99.123, 1.12345);

            _logger.LogInformation("<H2>header2 test double args: {arg1:C}{arg2:N3}\r\n some text\r\n</H2>", 99.123, 1.12345);

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
            _logger.LogInformation("\r\n\r\nArgument auto highlight tests:");

            _logger.LogInformation("string:{arg1}; bool:{arg2}/{arg3}; null: {arg4}; empty:{arg5}", "string value", true, false, null,"");
            _logger.LogInformation("integers and count:  short {arg1}/{arg2}; int #{arg3}/{arg4}; long {arg5}/{arg6}",
                (short)10, (short)-1,
                100,-10, 
                500L,-2000L);
            _logger.LogInformation("integers/percentage: {arg1:P}; {arg2:P}; {arg3:P}", 2, -1, 0);
            _logger.LogInformation("floating: double {arg1}/{arg2}; decimal {arg3}/{arg4}; float {arg5}/{arg6}",
                1.01, -1.22, 
                100.01m, -10.22m, 
                (float)1.33, (float)-2.50);
            _logger.LogInformation("float/currency:  {arg1:C}; {arg2:C}; {arg3:C};{arg4:C}", 501.01, -10.22m, (float)-1.33, 0.00);
            _logger.LogInformation("float/percentage:  {arg1:P}/{arg2:P}; {arg3:P}/{arg4:P}; {arg5:P}",0.25,-0.33, 0.22m, (float)-1.33, 0.00);
            _logger.LogInformation("date: {arg1:g}; time {arg2:t}; timespan {arg3}", DateTime.Now, DateTime.Now, (DateTime.Now-DateTime.Today));
            _logger.LogInformation("array: {arg1}; arr2: {arg2}", new []{1,2,3}, new[] { 1.11, 2.1, -3.333 });
            _logger.LogInformation("array: {arg1}; arr2: {arg2}", (new []{1,2,3}).Stringify(), (new[] { 1.11, 2.1, -3.333 }).Stringify());
            _logger.LogInformation("array: {arg1}; arr2: {arg2}", new []{"a", "bb","blah-blah"}, new object[] { DateTime.Now, 250,"abc"});


            var obj = new
            {
                List = new List<object>() { new { A = 1 }, new { B = "abc" } },
                Prop1 = 25,
                Prop2 = 0.001m,
                Prop3 = "prop value"
            };
            _logger.LogInformation("object: {arg1}; json: {arg2}", obj, obj.ToJsonString());
            _logger.LogInformation("\r\n\r\nTesting {strategy}, timeframe: {timeframe}; parameters: {parameters}", "name", "H1", "Pool Fees: 10%");
            _logger.LogInformation("test argument count: #{arg}", 150);
            _logger.LogInformation("test negative values arg1:{arg}; arg2:{arg2:C}", -1.25, -22);
            _logger.LogInformation("info log with arguments: {arg1} some text {arg2}{arg3}", 1.ToString("P"), obj, "argument long text asdad asd asd asdasdadadsd");
            _logger.LogInformation($"info log with json text: {obj.ToJsonString()}");
        }

        private void ArgsFormatTests()
        {
            _logger.LogInformation("\r\n\r\nArgument color format tests:");
            _logger.LogInformation("test colored argument: {arg:-Yellow}", (-5).ToString("C"));
            _logger.LogInformation("test colored argument: {arg:C -Red --DarkYellow}", 10.125);
            _logger.LogInformation("test colored argument: {arg:C Yellow bgBlue}", 10.123);
            _logger.LogInformation("test colored argument: {arg:C -Cyan --Yellow}", 22.55);
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
            _logger.LogInformation($"{nameof(NestedService)} nested info log {{arg}}", "string argument");
            _logger.LogWarning($"{nameof(NestedService)} nested warning log {{arg:C}}", 1);
            _logger.LogError($"{nameof(NestedService)} error log {{arg123:C --Gray}}",123);

            using (var scope = _logger.BeginScope($"{nameof(NestedService)} scope"))
            {
                _logger.LogInformation($"{nameof(NestedService)} info log");
                _logger.LogWarning($"{nameof(NestedService)} warning log");
                _logger.LogError($"{nameof(NestedService)} error log");
            }
        }
    }
}