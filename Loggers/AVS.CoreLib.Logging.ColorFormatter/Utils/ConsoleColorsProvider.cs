using AVS.CoreLib.Logging.ColorFormatter.Extensions;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.Logging.ColorFormatter.Utils;
public interface IColorsProvider
{
    ConsoleColors GetColorsForArgument(ArgType kind);
    ConsoleColors GetColorsFor(LogParts part, LogLevel logLevel = LogLevel.None);
}

/// <summary>
/// bad naming i know think on better name...
/// </summary>
public enum ArgType
{
    Default = 0,
    Array,
    Numeric,
    NumericNegative,
    Cash,
    CashNegative,
    Percentage,
    /// <summary>
    /// short string
    /// </summary>
    String,
    /// <summary>
    /// long string
    /// </summary>
    Text,
    Date,
    TextJson
}

public enum LogParts
{
    Timestamp,
    LogLevel,
    Scope,
    Category,
    Message,
    Error
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

    public ConsoleColors GetColorsFor(LogParts part, LogLevel logLevel)
    {
        var colors = ConsoleColors.Empty;
        switch (part)
        {
            case LogParts.Error:
                colors = new ConsoleColors(ConsoleColor.Red, null);
                break;
            case LogParts.Scope:
                colors = new ConsoleColors(ConsoleColor.Cyan, null);
                break;
            case LogParts.Timestamp:
                colors = new ConsoleColors(ConsoleColor.DarkGray, null);
                break;
            case LogParts.Category:
                colors = new ConsoleColors(ConsoleColor.DarkYellow, null);
                break;
            case LogParts.LogLevel:
                colors = logLevel.GetLogLevelColors();
                break;
            case LogParts.Message:
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