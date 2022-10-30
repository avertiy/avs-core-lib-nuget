using AVS.CoreLib.Logging.ColorFormatter.Enums;
using AVS.CoreLib.Logging.ColorFormatter.Extensions;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.Logging.ColorFormatter.Utils;
public interface IColorsProvider
{
    ConsoleColors GetColorsForArgument(ArgType kind);
    ConsoleColors GetColorsFor(LogPart part, LogLevel logLevel = LogLevel.None);
}

public class ColorsProvider : IColorsProvider
{
    public ConsoleColors GetColorsForArgument(ArgType kind)
    {
        return kind switch
        {
            ArgType.Array => new ConsoleColors(ConsoleColor.Cyan, null),
            ArgType.Date => new ConsoleColors(ConsoleColor.Cyan, null),
            ArgType.TextJson => new ConsoleColors(ConsoleColor.Cyan, null),

            ArgType.Numeric => new ConsoleColors(ConsoleColor.Yellow, null),
            ArgType.NumericNegative => new ConsoleColors(ConsoleColor.Red, null),
            
            ArgType.Percentage => new ConsoleColors(ConsoleColor.Magenta, null),
            ArgType.Cash => new ConsoleColors(ConsoleColor.DarkGreen, null),
            ArgType.CashNegative => new ConsoleColors(ConsoleColor.DarkRed, null),
            ArgType.String => new ConsoleColors(ConsoleColor.DarkCyan, null),
            ArgType.Text => new ConsoleColors(ConsoleColor.DarkGray, null),
            
            ArgType.Default => new ConsoleColors(ConsoleColor.Gray, null),
            _ => new ConsoleColors(ConsoleColor.Gray, null)
        };
    }

    public ConsoleColors GetColorsFor(LogPart part, LogLevel logLevel)
    {
        var colors = ConsoleColors.Empty;
        switch (part)
        {
            case LogPart.Error:
                colors = new ConsoleColors(ConsoleColor.Red, null);
                break;
            case LogPart.Scope:
                colors = new ConsoleColors(ConsoleColor.Cyan, null);
                break;
            case LogPart.Timestamp:
                colors = new ConsoleColors(ConsoleColor.DarkGray, null);
                break;
            case LogPart.Category:
                colors = new ConsoleColors(ConsoleColor.DarkYellow, null);
                break;
            case LogPart.LogLevel:
                colors = logLevel.GetLogLevelColors();
                break;
            case LogPart.Message:
            {
                switch (logLevel)
                {
                    case LogLevel.Debug:
                        colors = new ConsoleColors(ConsoleColor.Gray, null);
                        break;
                    case LogLevel.Information:
                        colors = ConsoleColors.Empty;
                        break;
                    case LogLevel.Warning:
                        colors = new ConsoleColors(ConsoleColor.Yellow, null);
                        break;
                    case LogLevel.Error:
                        colors = new ConsoleColors(ConsoleColor.DarkRed, null);
                        break;
                    case LogLevel.Critical:
                        colors = new ConsoleColors(ConsoleColor.White, ConsoleColor.DarkRed);
                        break;
                }
                break;
            }
            default:
            {
                colors = new ConsoleColors(ConsoleColor.Gray, null);
                break;
            }
        }
        return colors;
    }
}