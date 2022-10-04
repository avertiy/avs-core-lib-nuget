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

    public string FormatWithColors(string text)
    {
        return HasValue && !string.IsNullOrEmpty(text) ? $"{{{text}:{ToString()}}}" : text;
    }

    public static ConsoleColors Gray => new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black);
    public static ConsoleColors DarkGray => new ConsoleColors(ConsoleColor.DarkGray, ConsoleColor.Black);
    public static ConsoleColors Cyan => new ConsoleColors(ConsoleColor.Cyan, ConsoleColor.Black);
    public static ConsoleColors Error => new ConsoleColors(ConsoleColor.DarkRed, ConsoleColor.Black);
    public static ConsoleColors Current => new ConsoleColors(System.Console.ForegroundColor, System.Console.BackgroundColor);

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
