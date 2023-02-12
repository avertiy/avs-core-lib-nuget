using System;
using AVS.CoreLib.PowerConsole.Utilities;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.ConsoleLogger;

public static class LogLevelExtensions
{
    public static ColorScheme GetColorScheme(this LogLevel lvl)
    {
        var background = ConsoleColor.Black;
        switch (lvl)
        {
            case LogLevel.Trace:
                return new ColorScheme(ConsoleColor.DarkGray, background);
            case LogLevel.Debug:
                return new ColorScheme(ConsoleColor.Gray, background);
            case LogLevel.Information:
                return new ColorScheme(ConsoleColor.Green, background);
            case LogLevel.Warning:
                return new ColorScheme(ConsoleColor.DarkYellow, background);
            case LogLevel.Error:
                return new ColorScheme(ConsoleColor.Red, background);
            case LogLevel.Critical:
                return new ColorScheme(ConsoleColor.DarkRed, ConsoleColor.White);
            default:
                throw new ArgumentOutOfRangeException(nameof(lvl));
        }
    }
}