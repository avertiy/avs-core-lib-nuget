using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.BootstrapTools;
using AVS.CoreLib.Debugging;
using AVS.CoreLib.Extensions.Stringify;
using AVS.CoreLib.Logging.ColorFormatter.Extensions;
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

            // basic tests
            LogLevelTests();
            ScopeTests();
            NewLineHandlingTests();
            TestDumpObject();

            return;

            FeaturesTests();
            TestArgsColorFormatter();
            ArgsFormatTests();

            TagFormattingTests();

            ArgHighlightTests();

            var log = ConsoleLogProfiler.Flush();
        }

        #region Basic tests

        private void LogLevelTests()
        {
            using (_logger.BeginScope("LogLevelTests"))
            {
                _logger.LogDebug("debug log");
                _logger.LogInformation("info log");
                _logger.LogWarning("warning log");
                _logger.LogError("error log without exception");

                try
                {
                    _service.Throw("test error");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "error log");
                }

                try
                {
                    _service.ThrowWithInnerException("test error");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "error log");
                }

                _logger.LogCritical("critical log");
            }
        }

        private void NewLineHandlingTests()
        {
            using (_logger.BeginScope("NewLineHandlingTests"))
            {
                // should handle new line at the beginning of the message nicely
                _logger.LogInformation("regular log message");
                _logger.LogInformation("\r\n\\r\\n there should be 1 empty line above");
                _logger.LogInformation("the next log message will be string.Empty");
                _logger.LogInformation("");
                _logger.LogInformation("there should be 1 empty line above");
                _logger.LogInformation("\r\n\r\n\\r\\n\\r\\n there should be 2 empty lines above");
            }
        }

        private void ScopeTests()
        {
            // scope
            using (var scope = _logger.BeginScope("simple scope test"))
            {
                _logger.LogInformation("log under simple scope {arg:N2}", 200.222);
            }

            _logger.LogInformation("log outside of simple scope");

            using (var scope = _logger.Context(("nested scope", "test")))
            {
                _logger.LogInformation("log under parent scope {arg:N2}", 44.4444);
                //nested scope
                _service.ScopeTest();
                _logger.LogError("error log under parent scope");
            }

            _logger.LogInformation("log outside of scopes");

            using (var scope = _logger.BeginScope("scope with context args test\r\n{arg1}\r\n{arg2}", "arg1", new { Prop1 = 25.5, Prop2 = "abc" }))
            {
                _logger.LogInformation("log under scope with args {arg:N2}", 30.333);
            }

            _logger.LogInformation("info outside of scope with args");


        }

        #endregion

        private void TestDumpObject()
        {
            using (var scope = _logger.BeginScope("TestDumpObject"))
            {
                var arr = new object[] { "1", 2, 3.0m, 54, 6 };
                var list = new List<object>() { "|", "123213", new object(), new { RRR = 11 } };
                var obj = new { Prop1 = "abc", Price = 252.2, Prop3 = arr, Prop4 = list };
                _logger.LogInformation("object dump:\r\n{obj}", obj.Dump());
            }
        }

        private void TestArgsColorFormatter()
        {
            _logger.LogInformation("{req} => {response}", "GetTradesFuturesRequest", "OK: [{\"symbol\":\"ALGOUSDT\",\"id\":360651546,\"orderId\":11000955971, ... yer\":false,\"maker\":false}](Length=106803)");
            _logger.LogInformation("Sending.. {request}", "GetTradesFuturesRequest GET https://fapi.binance.com/fapi/v1/userTrades?recvWindow=5000&signature=2601A6DD817F00ADD7263C0E9410B52DBC00F2262B4B35174C3FFB4EBC8ED936&symbol=ALGOUSDT&timestamp=1724857781154 (SIGNED)");


            _logger.LogInformation("enum value:{arg1:D}; enum value with custom format:{arg2:G}", ConsoleColor.Cyan, ConsoleColor.DarkYellow);
            //_logger.LogInformation("enum value:{arg1:D}; enum value with custom format:{arg2:G}", ConsoleColor.Cyan, ConsoleColor.DarkYellow);
            _logger.LogInformation("string:{arg1}; currency:{arg2:C}", "my-str", 10.21m);
            var dict = new Dictionary<string, object>();

            var str = "string:my-str; currency:$10.21";
            if (State.TryParse(dict.ToList(), out var state))
            {
                var formatter = new ArgsColorFormatter(new ColorProvider());
                var msg = formatter.Format(state);

                _logger.LogInformation("{str} => {msg}", str, msg);
            }
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





        private void FeaturesTests()
        {


            //stamps
            _logger.LogInformation("[time]");


        }



        private void ArgHighlightTests()
        {
            _logger.LogInformation("\r\n\r\nArgument auto highlight tests:");

            _logger.LogInformation("string:{arg1}; bool:{arg2}/{arg3}; null: {arg4}; empty:{arg5}", "string value", true, false, null, "");
            _logger.LogInformation("integers and count:  short {arg1}/{arg2}; int #{arg3}/{arg4}; long {arg5}/{arg6}",
                (short)10, (short)-1,
                100, -10,
                500L, -2000L);
            _logger.LogInformation("integers/percentage: {arg1:P}; {arg2:P}; {arg3:P}", 2, -1, 0);
            _logger.LogInformation("floating: double {arg1}/{arg2}; decimal {arg3}/{arg4}; float {arg5}/{arg6}",
                1.01, -1.22,
                100.01m, -10.22m,
                (float)1.33, (float)-2.50);
            _logger.LogInformation("float/currency:  {arg1:C}; {arg2:C}; {arg3:C};{arg4:C}", 501.01, -10.22m, (float)-1.33, 0.00);
            _logger.LogInformation("float/percentage:  {arg1:P}/{arg2:P}; {arg3:P}/{arg4:P}; {arg5:P}", 0.25, -0.33, 0.22m, (float)-1.33, 0.00);
            _logger.LogInformation("date: {arg1:g}; time {arg2:t}; timespan {arg3}", DateTime.Now, DateTime.Now, (DateTime.Now - DateTime.Today));
            _logger.LogInformation("array: {arg1}; arr2: {arg2}", new[] { 1, 2, 3 }, new[] { 1.11, 2.1, -3.333 });
            _logger.LogInformation("array: {arg1}; arr2: {arg2}", (new[] { 1, 2, 3 }).Stringify(), (new[] { 1.11, 2.1, -3.333 }).Stringify());
            _logger.LogInformation("array: {arg1}; arr2: {arg2}", new[] { "a", "bb", "blah-blah" }, new object[] { DateTime.Now, 250, "abc" });


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

        public void Throw(string message)
        {
            throw new ArgumentException(message);
        }

        public void ThrowWithInnerException(string message)
        {
            try
            {
                Throw("inner exception");
            }
            catch (Exception ex)
            {
                throw new Exception(message, ex);
            }
        }

        public void ScopeTest()
        {
            _logger.LogInformation("ScopeTest: log outside of nested scope, arg:{arg:g}", DateTime.Today);

            using (var scope = _logger.BeginScope(("@request", new TestRequest())))
            {
                _logger.LogInformation("log under nested scope arg:{arg:N2}", 0.002);
            }
        }

        public void Execute()
        {
            _logger.LogInformation($"{nameof(NestedService)} nested info log {{arg}}", "string argument");
            _logger.LogWarning($"{nameof(NestedService)} nested warning log {{arg:C}}", 1);
            _logger.LogError($"{nameof(NestedService)} error log {{arg123:C --Gray}}", 123);

            using (var scope = _logger.BeginScope($"{nameof(NestedService)} scope"))
            {
                _logger.LogInformation($"{nameof(NestedService)} info log");
                _logger.LogWarning($"{nameof(NestedService)} warning log");
                _logger.LogError($"{nameof(NestedService)} error log");
            }
        }
    }

    public class TestRequest
    {
        public string RequestId { get; set; } = Guid.NewGuid().ToString();
        public object Data { get; set; } = new[] { 11, 123, 224.0 };
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}