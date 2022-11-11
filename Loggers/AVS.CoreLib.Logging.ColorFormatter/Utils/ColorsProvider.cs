using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Logging.ColorFormatter.Enums;
using AVS.CoreLib.Logging.ColorFormatter.Extensions;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.Logging.ColorFormatter.Utils;
public interface IColorsProvider
{
    Colors GetColorsForArgument(ArgType kind);
    Colors GetColorsForArgument(ObjType type, FormatFlags flags);
    Colors GetColorsFor(LogPart part, LogLevel logLevel = LogLevel.None);
}

public class ColorsProvider : IColorsProvider
{
    public Colors GetColorsForArgument(ArgType kind)
    {
        return kind switch
        {
            ArgType.Array => new Colors(ConsoleColor.Cyan, null),
            ArgType.DateTime => new Colors(ConsoleColor.Cyan, null),
            ArgType.TextJson => new Colors(ConsoleColor.Cyan, null),

            ArgType.Percentage => new Colors(ConsoleColor.Magenta, null),

            ArgType.Numeric => new Colors(ConsoleColor.Green, null),
            ArgType.NumericNegative => new Colors(ConsoleColor.Red, null),
            
            ArgType.Cash => new Colors(ConsoleColor.DarkGreen, null),
            ArgType.CashNegative => new Colors(ConsoleColor.DarkRed, null),
            
            ArgType.Enum => new Colors(ConsoleColor.DarkYellow, null),

            ArgType.String => new Colors(ConsoleColor.DarkCyan, null),
            ArgType.Text => new Colors(ConsoleColor.DarkGray, null),
            
            ArgType.Default => new Colors(ConsoleColor.Gray, null),
            _ => new Colors(ConsoleColor.Gray, null)
        };
    }

    public Colors GetColorsForArgument(ObjType type, FormatFlags flags)
    {
        switch (type)
        {
            case ObjType.Array:
            case ObjType.List:
            case ObjType.Dictionary:
            case ObjType.DateTime:
            case ObjType.Time:
                return new Colors(ConsoleColor.Cyan, null);
            case ObjType.Boolean:
                return new Colors(ConsoleColor.Blue, null);
            case ObjType.Enum:
                return new Colors(ConsoleColor.DarkYellow, null);
            case ObjType.String:
                return new Colors(GetStringColor(flags), null);
            case ObjType.Float:
                return new Colors(GetFloatColor(flags), null);
            case ObjType.Integer:
                return new Colors(GetIntColor(flags), null);
            case ObjType.Object:
                return new Colors(GetObjectColor(flags), null);
            default:
                return new Colors(null, null);
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

    public Colors GetColorsFor(LogPart part, LogLevel logLevel)
    {
        var colors = Colors.Empty;
        switch (part)
        {
            case LogPart.Error:
                colors = new Colors(ConsoleColor.Red, null);
                break;
            case LogPart.Scope:
                colors = new Colors(ConsoleColor.Cyan, null);
                break;
            case LogPart.Timestamp:
                colors = new Colors(ConsoleColor.DarkGray, null);
                break;
            case LogPart.Category:
                colors = new Colors(ConsoleColor.DarkYellow, null);
                break;
            case LogPart.LogLevel:
                colors = logLevel.GetLogLevelColors();
                break;
            case LogPart.Message:
            {
                switch (logLevel)
                {
                    case LogLevel.Debug:
                        colors = new Colors(ConsoleColor.Gray, null);
                        break;
                    case LogLevel.Information:
                        colors = Colors.Empty;
                        break;
                    case LogLevel.Warning:
                        colors = new Colors(ConsoleColor.Yellow, null);
                        break;
                    case LogLevel.Error:
                        colors = new Colors(ConsoleColor.DarkRed, null);
                        break;
                    case LogLevel.Critical:
                        colors = new Colors(ConsoleColor.White, ConsoleColor.DarkRed);
                        break;
                }
                break;
            }
            default:
            {
                colors = new Colors(ConsoleColor.Gray, null);
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