using System;
using AVS.CoreLib.PowerConsole.Utilities;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.ConsoleLogger
{
    static class LogLevelExtensions
    {
        public static ColorScheme GetColorScheme(this LogLevel lvl)
        {
            var background = ConsoleColor.Black;
            switch (lvl)
            {
                case LogLevel.Trace:
                    return new ColorScheme(background, ConsoleColor.DarkGray);
                case LogLevel.Debug:
                    return new ColorScheme(background, ConsoleColor.Cyan);
                case LogLevel.Information:
                    return new ColorScheme(background, ConsoleColor.White);
                case LogLevel.Warning:
                    return new ColorScheme(background, ConsoleColor.Yellow);
                case LogLevel.Error:
                    return new ColorScheme(background, ConsoleColor.Red);
                case LogLevel.Critical:
                    return new ColorScheme(ConsoleColor.DarkRed, ConsoleColor.White);
                default:
                    throw new ArgumentOutOfRangeException(nameof(lvl));
            }
        }

        //public static string GetLogLevelText(this LogLevel logLevel, string append = " ")
        //{
        //    string text;
        //    switch (logLevel)
        //    {
        //        case LogLevel.Trace:
        //            text = "trce";
        //            break;
        //        case LogLevel.Debug:
        //            text = "dbug";
        //            break;
        //        case LogLevel.Information:
        //            return "";
        //        //text ="info";
        //        //break;
        //        case LogLevel.Warning:
        //            text = "warn";
        //            break;
        //        case LogLevel.Error:
        //            text = "fail";
        //            break;
        //        case LogLevel.Critical:
        //            text = "crit";
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException(nameof(logLevel));
        //    }
        //    return text + append;
        //}

    }
}