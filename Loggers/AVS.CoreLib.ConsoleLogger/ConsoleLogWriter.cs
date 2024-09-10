using System;
using System.Globalization;
using AVS.CoreLib.AbstractLogger;
using AVS.CoreLib.PowerConsole.Printers2;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.Text.Formatters.ColorMarkup;
using AVS.CoreLib.Utilities;
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
            Console1.WriteLine(voidEmptyLines);
        }

        public void Write(string str, bool endLine = true)
        {
            if (endLine)
                Console1.WriteLine(str);
            else
                Console1.Write(str);
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
                Console1.Print($" {logger} ", PrintOptions2.Inline, ConsoleColor.DarkGray);
            }
        }

        private void PrintTimestampAndLogLevel(LogLevel logLevel)
        {
            var options = Options.CurrentValue;
            Console1.WriteLine();
            if (string.IsNullOrEmpty(options.TimestampFormat))
            {
                var printOptions = AdjustPrintOptions(PrintOptions2.NoTimestamp | PrintOptions2.NoCTags);
                Console1.Print(logLevel.GetLogLevelText(), printOptions, colors: logLevel.GetColors());
            }
            else
            {
                var timestamp = DateTimeOffset.Now.ToLocalTime()
                    .ToString(options.TimestampFormat, CultureInfo.InvariantCulture);
                var printOptions = AdjustPrintOptions(PrintOptions2.NoCTags);
                Console1.Print($"{logLevel.GetLogLevelText()} {timestamp}", printOptions, logLevel.GetColors());
            }
        }

        private PrintOptions2 AdjustPrintOptions(PrintOptions2 options)
        {
            return options.InLine(Options.CurrentValue.SingleLine);
        }

        public void BeginScope(object scope, bool addCurlyBraces)
        {
            WriteLine(false);
            if (addCurlyBraces)
            {
                Console1.WriteLine($"{scope}\r\n {{", colors: ConsoleColor.Cyan);
            }
            else
            {
                Console1.WriteLine($" ===== begin scope: {scope} =====\r\n", colors: ConsoleColor.Cyan);
            }
            WriteLine(false);
        }

        public void EndScope(bool addCurlyBraces)
        {
            WriteLine(false);
            if (addCurlyBraces)
            {
                Console1.WriteLine($" }}", colors: ConsoleColor.Cyan);
            }
            else
            {
                Console1.WriteLine($" ===== end scope =====\r\n", colors: ConsoleColor.Cyan);
            }
            WriteLine(false);
        }
    }
}