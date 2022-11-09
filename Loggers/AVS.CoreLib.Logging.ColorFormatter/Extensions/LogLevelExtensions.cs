using AVS.CoreLib.ConsoleColors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
namespace AVS.CoreLib.Logging.ColorFormatter.Extensions
{
    public static class LogLevelExtensions
    {
        public static Colors GetLogLevelColors(this LogLevel logLevel)
        {
            // We must explicitly set the background color if we are setting the foreground color,
            // since just setting one can look bad on the users console.
            return logLevel switch
            {
                LogLevel.Trace => new Colors(ConsoleColor.DarkGray, null),
                LogLevel.Debug => new Colors(ConsoleColor.Gray, null),
                LogLevel.Information => new Colors(ConsoleColor.DarkGreen, null),
                LogLevel.Warning => new Colors(ConsoleColor.Yellow, null),
                LogLevel.Error => new Colors(ConsoleColor.DarkRed, null),
                LogLevel.Critical => new Colors(ConsoleColor.White, ConsoleColor.DarkRed),
                _ => new Colors(null, null)
            };
        }


        public static Colors GetLogLevelColors(this LogLevel logLevel, LoggerColorBehavior colorBehavior)
        {
            // We shouldn't be outputting color codes for Android/Apple mobile platforms,
            // they have no shell (adb shell is not meant for running apps) and all the output gets redirected to some log file.
            bool disableColors = colorBehavior == LoggerColorBehavior.Disabled;
            if (disableColors)
            {
                return new Colors(null, null);
            }

            // We must explicitly set the background color if we are setting the foreground color,
            // since just setting one can look bad on the users console.
            return logLevel switch
            {
                LogLevel.Trace => new Colors(ConsoleColor.DarkGray, ConsoleColor.Black),
                LogLevel.Debug => new Colors(ConsoleColor.Gray, ConsoleColor.Black),
                LogLevel.Information => new Colors(ConsoleColor.DarkGreen, ConsoleColor.Black),
                LogLevel.Warning => new Colors(ConsoleColor.Yellow, ConsoleColor.Black),
                LogLevel.Error => new Colors(ConsoleColor.DarkRed, ConsoleColor.Black),
                LogLevel.Critical => new Colors(ConsoleColor.White, ConsoleColor.DarkRed),
                _ => new Colors(null, null)
            };
        }

        public static Colors GetMessageColors(this LogLevel logLevel, LoggerColorBehavior colorBehavior)
        {
            // We shouldn't be outputting color codes for Android/Apple mobile platforms,
            // they have no shell (adb shell is not meant for running apps) and all the output gets redirected to some log file.
            bool disableColors = colorBehavior == LoggerColorBehavior.Disabled;
            if (disableColors)
            {
                return new Colors(null, null);
            }

            // We must explicitly set the background color if we are setting the foreground color,
            // since just setting one can look bad on the users console.
            return logLevel switch
            {
                LogLevel.Trace => new Colors(ConsoleColor.DarkGray, ConsoleColor.Black),
                LogLevel.Debug => new Colors(ConsoleColor.Gray, ConsoleColor.Black),
                LogLevel.Information => new Colors(ConsoleColor.White, ConsoleColor.Black),
                LogLevel.Warning => new Colors(ConsoleColor.Yellow, ConsoleColor.Black),
                LogLevel.Error => new Colors(ConsoleColor.DarkRed, ConsoleColor.Black),
                LogLevel.Critical => new Colors(ConsoleColor.White, ConsoleColor.DarkRed),
                _ => new Colors(null, null)
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