namespace AVS.CoreLib.Logging.ColorFormatter.Extensions;

public static class StringColorExtensions
{
    public static string Colorize(this string str, ConsoleColor foreground = ConsoleColor.DarkGreen)
    {
        return $"{str}$$:{foreground}";
    }

    public static string Yellow(this string str)
    {
        return $"{str}$$:Yellow";
    }

    public static string Blue(this string str)
    {
        return $"{str}$$:Blue";
    }

    public static string Cyan(this string str)
    {
        return $"{str}$$:Cyan";
    }

    public static string FormatWithColorMarkup(this string text, ConsoleColor foreground)
    {
        return string.IsNullOrEmpty(text) ? text: $"${text}:-{foreground}$";
    }
}