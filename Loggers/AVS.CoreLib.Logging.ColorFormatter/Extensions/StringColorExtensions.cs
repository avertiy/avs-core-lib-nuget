namespace AVS.CoreLib.Logging.ColorFormatter.Extensions;

public static class StringColorExtensions
{
    
    public static string Colorize(this string str, ConsoleColor foreground = ConsoleColor.DarkGreen)
    {
        //end line color markup
        return $"{str}$$:{foreground}";
    }

    public static string Yellow(this string str)
    {
        //end line color markup
        return $"{str}$$:Yellow";
    }

    public static string Blue(this string str)
    {
        //end line color markup
        return $"{str}$$:Blue";
    }

    public static string Cyan(this string str)
    {
        //end line color markup
        return $"{str}$$:Cyan";
    }

    public static string FormatWithColorMarkup(this string text, ConsoleColor foreground)
    {
        return string.IsNullOrEmpty(text) ? text: $"${text}:-{foreground}$";
    }
}