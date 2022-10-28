using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace AVS.CoreLib.Logging.ColorFormatter.Extensions
{
    public static class LogLevelExtensions
    {
        public static ConsoleColors GetLogLevelColors(this LogLevel logLevel)
        {
            // We must explicitly set the background color if we are setting the foreground color,
            // since just setting one can look bad on the users console.
            return logLevel switch
            {
                LogLevel.Trace => new ConsoleColors(ConsoleColor.DarkGray, ConsoleColor.Black),
                LogLevel.Debug => new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black),
                LogLevel.Information => new ConsoleColors(ConsoleColor.DarkGreen, ConsoleColor.Black),
                LogLevel.Warning => new ConsoleColors(ConsoleColor.Yellow, ConsoleColor.Black),
                LogLevel.Error => new ConsoleColors(ConsoleColor.DarkRed, ConsoleColor.Black),
                LogLevel.Critical => new ConsoleColors(ConsoleColor.White, ConsoleColor.DarkRed),
                _ => new ConsoleColors(null, null)
            };
        }


        public static ConsoleColors GetLogLevelConsoleColors(this LogLevel logLevel, LoggerColorBehavior colorBehavior)
        {
            // We shouldn't be outputting color codes for Android/Apple mobile platforms,
            // they have no shell (adb shell is not meant for running apps) and all the output gets redirected to some log file.
            bool disableColors = colorBehavior == LoggerColorBehavior.Disabled;
            if (disableColors)
            {
                return new ConsoleColors(null, null);
            }

            // We must explicitly set the background color if we are setting the foreground color,
            // since just setting one can look bad on the users console.
            return logLevel switch
            {
                LogLevel.Trace => new ConsoleColors(ConsoleColor.DarkGray, ConsoleColor.Black),
                LogLevel.Debug => new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black),
                LogLevel.Information => new ConsoleColors(ConsoleColor.DarkGreen, ConsoleColor.Black),
                LogLevel.Warning => new ConsoleColors(ConsoleColor.Yellow, ConsoleColor.Black),
                LogLevel.Error => new ConsoleColors(ConsoleColor.DarkRed, ConsoleColor.Black),
                LogLevel.Critical => new ConsoleColors(ConsoleColor.White, ConsoleColor.DarkRed),
                _ => new ConsoleColors(null, null)
            };
        }

        public static ConsoleColors GetMessageConsoleColors(this LogLevel logLevel, LoggerColorBehavior colorBehavior)
        {
            // We shouldn't be outputting color codes for Android/Apple mobile platforms,
            // they have no shell (adb shell is not meant for running apps) and all the output gets redirected to some log file.
            bool disableColors = colorBehavior == LoggerColorBehavior.Disabled;
            if (disableColors)
            {
                return new ConsoleColors(null, null);
            }

            // We must explicitly set the background color if we are setting the foreground color,
            // since just setting one can look bad on the users console.
            return logLevel switch
            {
                LogLevel.Trace => new ConsoleColors(ConsoleColor.DarkGray, ConsoleColor.Black),
                LogLevel.Debug => new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black),
                LogLevel.Information => new ConsoleColors(ConsoleColor.White, ConsoleColor.Black),
                LogLevel.Warning => new ConsoleColors(ConsoleColor.Yellow, ConsoleColor.Black),
                LogLevel.Error => new ConsoleColors(ConsoleColor.DarkRed, ConsoleColor.Black),
                LogLevel.Critical => new ConsoleColors(ConsoleColor.White, ConsoleColor.DarkRed),
                _ => new ConsoleColors(null, null)
            };
        }


        public static string GetLogLevelText(this LogLevel logLevel)
        {
            string text;
            switch (logLevel)
            {
                case LogLevel.Trace:
                    text = "trce";
                    break;
                case LogLevel.Debug:
                    text = "dbug";
                    break;
                case LogLevel.Information:
                    text = "info";
                    break;
                case LogLevel.Warning:
                    text = "warn";
                    break;
                case LogLevel.Error:
                    text = "fail";
                    break;
                case LogLevel.Critical:
                    text = "crit";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
            return text;
        }
    }
}