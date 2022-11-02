using AVS.CoreLib.Logging.ColorFormatter.Enums;
using AVS.CoreLib.Logging.ColorFormatter.Extensions;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.Logging.ColorFormatter.Utils;
public interface IColorsProvider
{
    ConsoleColors GetColorsForArgument(ArgType kind);
    ConsoleColors GetColorsForArgument(ObjType type, FormatFlags flags);
    ConsoleColors GetColorsFor(LogPart part, LogLevel logLevel = LogLevel.None);
}

public class ColorsProvider : IColorsProvider
{
    public ConsoleColors GetColorsForArgument(ArgType kind)
    {
        return kind switch
        {
            ArgType.Array => new ConsoleColors(ConsoleColor.Cyan, null),
            ArgType.DateTime => new ConsoleColors(ConsoleColor.Cyan, null),
            ArgType.TextJson => new ConsoleColors(ConsoleColor.Cyan, null),

            ArgType.Percentage => new ConsoleColors(ConsoleColor.Magenta, null),

            ArgType.Numeric => new ConsoleColors(ConsoleColor.Green, null),
            ArgType.NumericNegative => new ConsoleColors(ConsoleColor.Red, null),
            
            ArgType.Cash => new ConsoleColors(ConsoleColor.DarkGreen, null),
            ArgType.CashNegative => new ConsoleColors(ConsoleColor.DarkRed, null),
            
            ArgType.Enum => new ConsoleColors(ConsoleColor.DarkYellow, null),

            ArgType.String => new ConsoleColors(ConsoleColor.DarkCyan, null),
            ArgType.Text => new ConsoleColors(ConsoleColor.DarkGray, null),
            
            ArgType.Default => new ConsoleColors(ConsoleColor.Gray, null),
            _ => new ConsoleColors(ConsoleColor.Gray, null)
        };
    }

    public ConsoleColors GetColorsForArgument(ObjType type, FormatFlags flags)
    {
        switch (type)
        {
            case ObjType.Array:
            case ObjType.List:
            case ObjType.Dictionary:
            case ObjType.DateTime:
            case ObjType.Time:
                return new ConsoleColors(ConsoleColor.Cyan, null);
            case ObjType.Boolean:
                return new ConsoleColors(ConsoleColor.Blue, null);
            case ObjType.Enum:
                return new ConsoleColors(ConsoleColor.DarkYellow, null);
            case ObjType.String:
                return new ConsoleColors(GetStringColor(flags), null);
            case ObjType.Float:
                return new ConsoleColors(GetFloatColor(flags), null);
            case ObjType.Integer:
                return new ConsoleColors(GetIntColor(flags), null);
            case ObjType.Object:
                return new ConsoleColors(GetObjectColor(flags), null);
            default:
                return new ConsoleColors(null, null);
        }
    }

    private ConsoleColor? GetObjectColor(FormatFlags flags)
    {
        return flags.PickColorByBracketFlags(ConsoleColor.Cyan, ConsoleColor.DarkYellow, ConsoleColor.Blue);
    }

    ConsoleColor GetIntColor(FormatFlags flags)
    {
        return flags.PickColorBySignFlags(ConsoleColor.Blue, ConsoleColor.Red, ConsoleColor.Yellow);
    }

    ConsoleColor GetStringColor(FormatFlags flags)
    {
        if(flags.HasFlag(FormatFlags.ShortString))
            return ConsoleColor.Yellow;

        if (flags.HasFlag(FormatFlags.Percentage))
            return ConsoleColor.Cyan;

        if (flags.HasFlag(FormatFlags.SquareBrackets))
            return ConsoleColor.Blue;
        
        if (flags.HasFlag(FormatFlags.CurlyBrackets))
            return ConsoleColor.DarkYellow;
        
        if (flags.HasFlag(FormatFlags.Text))
            return ConsoleColor.DarkGray;
        
        if (flags.HasFlag(FormatFlags.Json))
            return ConsoleColor.Cyan;

        return ConsoleColor.DarkYellow;
    }

    ConsoleColor GetFloatColor(FormatFlags flags)
    {
        if (flags.HasFlag(FormatFlags.Percentage))
        {
            return flags.PickColorBySignFlags(ConsoleColor.Cyan, ConsoleColor.Red, ConsoleColor.Yellow);
        }else if (flags.HasFlag(FormatFlags.Currency))
        {
            return flags.PickColorBySignFlags(ConsoleColor.Green, ConsoleColor.DarkRed, ConsoleColor.Yellow);
        }

        return flags.PickColorBySignFlags(ConsoleColor.Blue, ConsoleColor.Red, ConsoleColor.DarkYellow);
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

public static class FormatFlagsExtensions
{
    public static ConsoleColor PickColorBySignFlags(this FormatFlags flags, ConsoleColor @default, ConsoleColor negative, ConsoleColor zero)
    {
        if (flags.HasFlag(FormatFlags.Zero))
            return zero;
        return flags.HasFlag(FormatFlags.Negative) ? negative : @default;
    }

    public static ConsoleColor PickColorByBracketFlags(this FormatFlags flags, ConsoleColor @default, ConsoleColor curlyBrackets, ConsoleColor squareBrackets)
    {
        if (flags.HasFlag(FormatFlags.CurlyBrackets))
            return curlyBrackets;
        return flags.HasFlag(FormatFlags.SquareBrackets) ? squareBrackets : @default;
    }
}