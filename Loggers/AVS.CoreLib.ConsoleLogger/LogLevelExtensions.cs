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
                return new ColorScheme(background, ConsoleColor.DarkGray);
            case LogLevel.Debug:
                return new ColorScheme(background, ConsoleColor.Gray);
            case LogLevel.Information:
                return new ColorScheme(background, ConsoleColor.Green);
            case LogLevel.Warning:
                return new ColorScheme(background, ConsoleColor.DarkYellow);
            case LogLevel.Error:
                return new ColorScheme(background, ConsoleColor.Red);
            case LogLevel.Critical:
                return new ColorScheme(ConsoleColor.DarkRed, ConsoleColor.White);
            default:
                throw new ArgumentOutOfRangeException(nameof(lvl));
        }
    }
}