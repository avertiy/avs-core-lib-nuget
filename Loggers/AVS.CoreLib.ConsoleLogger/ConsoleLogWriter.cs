using System;
using System.Globalization;
using AVS.CoreLib.AbstractLogger;
using AVS.CoreLib.PowerConsole.Printers;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.Text.Formatters.ColorMarkup;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Console1 = AVS.CoreLib.PowerConsole.PowerConsole;
namespace AVS.CoreLib.ConsoleLogger
{
    public class ConsoleLogWriter : ILogWriter
    {
        public IOptionsMonitor<ConsoleLoggerOptions> Options { get; }
        public ConsoleLogWriter(IOptionsMonitor<ConsoleLoggerOptions> options)
        {
            Options = options;
        }

        public void WriteLine(bool voidEmptyLines = true)
        {
            Console1.WriteLine(voidEmptyLines ? PrintOptions.EmptyLinesVoided : PrintOptions.EmptyLinesAllowed);
        }

        public void Write(string str, bool endLine = true)
        {
            Console1.Write(str, endLine ? PrintOptions.Default : PrintOptions.Inline);
        }

        public void Write(string logger, EventId eventId, LogLevel logLevel, string message, Exception exception = null)
        {
            
            //this looks wierd message starting from @Color will be colorized.. not good solution
            //var color = ColorSchemeHelper.ExtractColor(ref message, scheme.Foreground);
            
            using (var locker = ConsoleLocker.Create())
            {
                var schemeBackup = ColorScheme.GetCurrentScheme();
                PrintTimestampAndLogLevel(logLevel);
                PrintLoggerType(logger);

                var str = new ColorMarkupString(message);
                Console1.Print(str);
                if (exception != null)
                {
                    Console1.PrintError(exception, printStackTrace: true);
                }

                ColorScheme.ApplyScheme(schemeBackup);
            }
        }

        private void PrintLoggerType(string logger)
        {
            if (Options.CurrentValue.PrintLoggerName)
            {
                Console1.Print($" {logger} ", PrintOptions.FromColor(ConsoleColor.DarkGray, endLine: false));
            }
        }

        private void PrintTimestampAndLogLevel(LogLevel logLevel)
        {
            var scheme = logLevel.GetColorScheme();
            var options = Options.CurrentValue;
            Console1.WriteLine();
            if (string.IsNullOrEmpty(options.TimestampFormat))
            {
                Console1.Print(logLevel.GetLogLevelText(), PrintOptions.FromColorScheme(scheme, endLine: !options.SingleLine));
            }
            else
            {
                var timestamp = DateTimeOffset.Now.ToLocalTime()
                    .ToString(options.TimestampFormat, CultureInfo.InvariantCulture);
                Console1.Print($"{logLevel.GetLogLevelText()} {timestamp}", PrintOptions.FromColorScheme(scheme, endLine: !options.SingleLine));
            }
        }

        public void BeginScope(object scope, bool addCurlyBraces)
        {
            WriteLine(false);
            if (addCurlyBraces)
            {
                Console1.Print($"{scope}\r\n {{", ConsoleColor.Cyan);
            }
            else
            {
                Console1.Print($" ===== begin scope: {scope} =====\r\n", ConsoleColor.Cyan);
            }
            WriteLine(false);
        }

        public void EndScope(bool addCurlyBraces)
        {
            WriteLine(false);
            if (addCurlyBraces)
            {
                Console1.Print($" }}", ConsoleColor.Cyan);
            }
            else
            {
                Console1.Print($" ===== end scope =====\r\n", ConsoleColor.Cyan);
            }
            WriteLine(false);
        }
    }
}