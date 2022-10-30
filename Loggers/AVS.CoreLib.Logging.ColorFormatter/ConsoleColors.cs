using AVS.CoreLib.Logging.ColorFormatter.Extensions;
using AVS.CoreLib.Logging.ColorFormatter.Utils;

namespace AVS.CoreLib.Logging.ColorFormatter;

public readonly struct ConsoleColors
{
    public ConsoleColors(ConsoleColor? foreground, ConsoleColor? background)
    {
        Foreground = foreground;
        Background = background;
    }

    public ConsoleColor? Foreground { get; }

    public ConsoleColor? Background { get; }

    public override string ToString()
    {
        if(Foreground.HasValue && Background.HasValue)
            return $"-{Foreground} --{Background}";

        if (Foreground.HasValue)
            return $"-{Foreground}";

        if (Background.HasValue)
            return $"--{Foreground}";

        return string.Empty;
    }

    public string FormatWithTags(string text)
    {
        if (Foreground.HasValue && Background.HasValue)
            return $"<{Foreground}><bg{Background}>{text}</bg{Background}></{Foreground}>";

        if (Foreground.HasValue)
            return $"<{Foreground}>{text}</{Foreground}>";

        if (Background.HasValue)
            return $"<bg{Background}>{text}</bg{Background}>";

        return text;
    }

    public string Colorize(string text)
    {
        if (Foreground.HasValue && Background.HasValue)
            return $"{AnsiCodes.Color(Foreground.Value)}{AnsiCodes.BgColor(Background.Value)}{text}{AnsiCodes.RESET}";

        if (Foreground.HasValue)
            return $"{AnsiCodes.Color(Foreground.Value)}{text}{AnsiCodes.RESET}";

        if (Background.HasValue)
            return $"{AnsiCodes.BgColor(Background.Value)}{text}{AnsiCodes.RESET}";

        return text;
    }    

    public bool HasValue => Foreground.HasValue || Background.HasValue;

    public static ConsoleColors Empty => new ConsoleColors(null, null);

    /// <summary>
    /// supported formats:
    /// foreground: -Color or Color
    /// background: --Color or bgColor
    /// both: -Color --Color or -Color bgColor
    /// </summary>
    public static ConsoleColors Parse(string str)
    {
        if (!TryGetColors(str, out var color, out var bgColor))
            return ConsoleColors.Empty;

        return new ConsoleColors(color, bgColor);
        //var parts = str.Split(new []{ ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
        //return parts.Length switch
        //{
        //    1 when ConsoleColorHelper.TryParseConsoleColor(parts[0], out var color) => new ConsoleColors(color, null),
        //    2 when ConsoleColorHelper.TryParseConsoleColor(parts[0], out var foreground) &&
        //           ConsoleColorHelper.TryParseConsoleColor(parts[1], out var background) => new ConsoleColors(
        //        foreground, background),
        //    _ => Empty
        //};
    }

    public static bool TryParse(string str, out ConsoleColors colors)
    {
        if (!TryGetColors(str, out var color, out var bgColor))
        {
            colors = ConsoleColors.Empty;
            return false;
        }

        colors = new ConsoleColors(color, bgColor);
        return true;
    }

    private static bool TryGetColors(string str, out ConsoleColor? color, out ConsoleColor? bgColor)
    {
        color = null;
        bgColor = null;
        for (var i = 0; i < str.Length; i++)
        {
            var fromInd = -1;
            if (char.IsUpper(str[i]))
                fromInd = i;

            if (str[i] == '-' && str[i + 1] != '-' && str[i + 1] != 'b' && char.IsUpper(str[i + 1]))
                fromInd = i + 1;
                
            if (fromInd > 0)
            {
                var colorStr = str.ReadWord(fromInd);
                i += colorStr.Length;
                if (Enum.TryParse(colorStr, out ConsoleColor c))
                    color = c;
                continue;
            }

            if ((str.Contains("--", fromIndex: i) || str.Contains("bg", fromIndex: i)) && char.IsUpper(str[i + 2]))
            {
                var colorStr = str.ReadWord(fromIndex: i + 2);
                i += colorStr.Length;
                if (Enum.TryParse(colorStr, out ConsoleColor c))
                    bgColor = c;
                break;
            }
        }

        return color.HasValue || bgColor.HasValue;
    }
}

/*
public static class ConsoleColorHelper
{
    public static bool TryParseConsoleColor(string value, out ConsoleColor color)
    {
        if (Enum.TryParse(value, out color))
        {
            return true;
        }

        color = ConsoleColor.Black;
        return false;
    }
}
*/