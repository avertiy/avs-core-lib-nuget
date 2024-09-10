using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Logging.ColorFormatter.Enums;
using AVS.CoreLib.Logging.ColorFormatter.Extensions;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.Logging.ColorFormatter.Utils;
public interface IColorProvider
{
    Colors GetColors(ObjType type);
    Colors GetColorsFor(LogPart part, LogLevel logLevel = LogLevel.None);
    Colors GetColors(TextKind flag);
    Colors GetColors(NumberFlags flag);
}

public class ColorProvider : IColorProvider
{
    public Colors GetColors(ObjType type)
    {
        switch (type)
        {
            case ObjType.Array:
            case ObjType.List:
                return new Colors(ConsoleColor.Yellow, null);
            case ObjType.Dictionary:
                return new Colors(ConsoleColor.Cyan, null);
            case ObjType.Boolean:
                return new Colors(ConsoleColor.Blue, null);
            case ObjType.Enum:
                return new Colors(ConsoleColor.DarkYellow, null);
            case ObjType.DateTime:
            case ObjType.Time:
            case ObjType.String:
                return new Colors(ConsoleColor.White, null);
            default:
                return new Colors(null, null);
        }
    }

    public Colors GetColorsFor(LogPart part, LogLevel logLevel)
    {
        var colors = new Colors();
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
                colors = new Colors(ConsoleColor.DarkGray, null);
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

    public Colors GetColors(TextKind flag)
    {
        switch (flag)
        {
            case TextKind.Text:
                return new Colors(ConsoleColor.DarkGray, null);
            case TextKind.Json:
                return new Colors(ConsoleColor.Cyan, null);
            case TextKind.Array:
                return new Colors(ConsoleColor.Yellow, null);
            case TextKind.Brackets:
                return new Colors(ConsoleColor.DarkCyan, null);
            case TextKind.Url:
                return new Colors(ConsoleColor.Green, null);
            case TextKind.HttpVerb:
                return new Colors(ConsoleColor.Blue, null);
            case TextKind.Symbol:
                return new Colors(ConsoleColor.Blue, null);
            case TextKind.Keyword:
                return new Colors(ConsoleColor.Yellow, null);
            case TextKind.SpecialKeyword:
                return new Colors(ConsoleColor.Magenta, null);
            case TextKind.OK:
                return new Colors(ConsoleColor.DarkGreen, null);
            case TextKind.Error:
                return new Colors(ConsoleColor.DarkRed, null);
            case TextKind.Percentage:
                return new Colors(ConsoleColor.Blue, null);
            case TextKind.Quotes:
                return new Colors(ConsoleColor.White, null);
            case TextKind.DoubleQuotes:
                return new Colors(ConsoleColor.Magenta, null);
            case TextKind.FilePath:
                return new Colors(ConsoleColor.DarkGray, null);
            default:
                return new Colors(ConsoleColor.Gray, null);
        }
    }

    public Colors GetColors(NumberFlags flags)
    {
        if (flags.HasFlag(NumberFlags.Percentage))
        {
            return flags.PickColorBySignFlags(@default: ConsoleColor.Cyan, negative: ConsoleColor.Red, zero: ConsoleColor.Yellow);
        }

        if (flags.HasFlag(NumberFlags.Currency))
        {
            return flags.PickColorBySignFlags(@default: ConsoleColor.Green, negative: ConsoleColor.Red, zero: ConsoleColor.Yellow);
        }

        if (flags.HasFlag(NumberFlags.Float))
        {
            return flags.PickColorBySignFlags(@default: ConsoleColor.Magenta, ConsoleColor.Red, ConsoleColor.Yellow);
        }

        // regular integers
        return flags.PickColorBySignFlags(@default: ConsoleColor.Blue, ConsoleColor.Red, ConsoleColor.DarkYellow);
    }

    ConsoleColor GetFloatColor(NumberFlags flags)
    {
        if (flags.HasFlag(NumberFlags.Percentage))
        {
            return flags.PickColorBySignFlags(ConsoleColor.Cyan, ConsoleColor.Red, ConsoleColor.Yellow);
        }

        if (flags.HasFlag(NumberFlags.Currency))
        {
            return flags.PickColorBySignFlags(ConsoleColor.Green, ConsoleColor.DarkRed, ConsoleColor.Yellow);
        }

        return flags.PickColorBySignFlags(ConsoleColor.Blue, ConsoleColor.Red, ConsoleColor.DarkYellow);
    }
}

public static class FormatFlagsExtensions
{
    internal static ConsoleColor PickColorBySignFlags(this NumberFlags flags, ConsoleColor @default, ConsoleColor negative, ConsoleColor zero)
    {
        if (flags.HasFlag(NumberFlags.Zero))
            return zero;
        return flags.HasFlag(NumberFlags.Negative) ? negative : @default;
    }
}