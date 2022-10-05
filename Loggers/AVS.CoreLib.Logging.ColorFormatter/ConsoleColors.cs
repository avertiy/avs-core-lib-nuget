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

    public bool HasValue => Foreground.HasValue || Background.HasValue;

    public static ConsoleColors Gray => new ConsoleColors(ConsoleColor.Gray, null);
    public static ConsoleColors DarkGray => new ConsoleColors(ConsoleColor.DarkGray, null);
    public static ConsoleColors Cyan => new ConsoleColors(ConsoleColor.Cyan, null);
    public static ConsoleColors Error = new ConsoleColors(ConsoleColor.DarkRed, null);
    public static ConsoleColors Category = new ConsoleColors(ConsoleColor.DarkYellow, null);
    public static ConsoleColors Scope = new ConsoleColors(ConsoleColor.Cyan, null);
    public static ConsoleColors Args = new ConsoleColors(ConsoleColor.Blue, null);

    public static ConsoleColors Parse(string str)
    {
        var parts = str.Split(new []{ ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length switch
        {
            1 when ConsoleColorHelper.TryParseConsoleColor(parts[0], out var color) => new ConsoleColors(color, null),
            2 when ConsoleColorHelper.TryParseConsoleColor(parts[0], out var foreground) &&
                   ConsoleColorHelper.TryParseConsoleColor(parts[1], out var background) => new ConsoleColors(
                foreground, background),
            _ => new ConsoleColors()
        };
    }

   
}

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
