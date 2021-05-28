using System;
using System.Globalization;
using AVS.CoreLib.AbstractLogger;
using AVS.CoreLib.PowerConsole.Utilities;
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
            var options = Options.CurrentValue;
            var scheme = logLevel.GetColorScheme();
            var color = ColorHelper.ExtractColor(ref message, scheme.Foreground);
            using (var locker = ConsoleLocker.Create())
            {
                Console.WriteLine();
                if (options.IncludeTimeStamp)
                {
                    var timestamp = DateTimeOffset.Now.ToLocalTime().ToString(options.DateFormat, CultureInfo.InvariantCulture);
                    Console.Print($"{logLevel.GetLogLevelText()} {timestamp}", scheme);
                }
                else
                {
                    Console.Print(logLevel.GetLogLevelText(), scheme);
                }
                scheme.Foreground = color;
                Console.Print("     " + message, scheme);
                if (exception != null)
                {
                    Console.WriteError(exception, true);
                    //Console.WriteLine(exception.StackTrace, ConsoleColor.DarkGray, null);
                }
            }
        }
    }
}