using System;
using System.Globalization;
using AVS.CoreLib.AbstractLogger;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.Text.Formatters.ColorMarkup;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Console = AVS.CoreLib.PowerConsole.PowerConsole;
namespace AVS.CoreLib.ConsoleLogger
{
    public class ConsoleLogWriter : ILogWriter
    {
        public IOptionsMonitor<ConsoleLoggerOptions> Options { get; }
        public ConsoleLogWriter(IOptionsMonitor<ConsoleLoggerOptions> options)
        {
            Options = options;
        }

        public void WriteLine(bool combineEmptyLines = true)
        {
            Console.WriteLine(combineEmptyLines);
        }

        public void Write(string str, bool endLine = true)
        {
            Console.Print(str, endLine);
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
                Console.Print(str);
                if (exception != null)
                {
                    Console.WriteError(exception, true);
                }

                ColorScheme.ApplyScheme(schemeBackup);
            }
        }

        private void PrintLoggerType(string logger)
        {
            if (Options.CurrentValue.PrintLoggerName)
            {
                Console.Print($" {logger} ", ConsoleColor.DarkGray, false);
            }
        }

        private void PrintTimestampAndLogLevel(LogLevel logLevel)
        {
            var scheme = logLevel.GetColorScheme();
            var options = Options.CurrentValue;
            Console.WriteLine();
            if (string.IsNullOrEmpty(options.TimestampFormat))
            {
                Console.Print(logLevel.GetLogLevelText(), scheme, !options.SingleLine);
            }
            else
            {
                var timestamp = DateTimeOffset.Now.ToLocalTime()
                    .ToString(options.TimestampFormat, CultureInfo.InvariantCulture);
                Console.Print($"{logLevel.GetLogLevelText()} {timestamp}", scheme, !options.SingleLine);
            }
        }

        public void BeginScope(object scope, bool addCurlyBraces)
        {
            WriteLine(false);
            if (addCurlyBraces)
            {
                Console.Print($"{scope}\r\n {{", ConsoleColor.Cyan);
            }
            else
            {
                Console.Print($" ===== begin scope: {scope} =====\r\n", ConsoleColor.Cyan);
            }
            WriteLine(false);
        }

        public void EndScope(bool addCurlyBraces)
        {
            WriteLine(false);
            if (addCurlyBraces)
            {
                Console.Print($" }}", ConsoleColor.Cyan);
            }
            else
            {
                Console.Print($" ===== end scope =====\r\n", ConsoleColor.Cyan);
            }
            WriteLine(false);
        }
    }
}