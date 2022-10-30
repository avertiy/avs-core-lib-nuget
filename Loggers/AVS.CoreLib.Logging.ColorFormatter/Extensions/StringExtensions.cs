namespace AVS.CoreLib.Logging.ColorFormatter.Extensions;

public static class StringExtensions
{
    public static string Colorize(this string str, string colorScheme)
    {
        if (!ConsoleColors.TryParse(colorScheme, out var colors))
            return str;

        return colors.Colorize(str);
    }

    public static bool Contains(this string str, string value, int fromIndex = 0)
    {
        if(str.Length < fromIndex + value.Length)
            return false;
        var i = 0;
        while ((i < value.Length) && (str[fromIndex+i] == value[i]))
            i++;

        return i == value.Length;
    }
    
    public static int IndexOfEndOfWord(this string str, int fromIndex = 0)
    {
        if (str.Length <= fromIndex)
            throw new ArgumentOutOfRangeException($"{nameof(fromIndex)} {fromIndex} exceeds {nameof(str)} length {str.Length}");

        var end = str.Length;
        for (var i = fromIndex+1; i < str.Length; i++)
        {
            if (char.IsWhiteSpace(str[i]))
            {
                end = i;
                break;
            }
        }

        return end;
    }

    public static string ReadWord(this string str, int fromIndex = 0)
    {
        if (str.Length <= fromIndex)
            throw new ArgumentOutOfRangeException($"{nameof(fromIndex)} {fromIndex} exceeds {nameof(str)} length {str.Length}");

        var end = str.IndexOfEndOfWord(fromIndex);
        return str.Substring(fromIndex, end - fromIndex);
    }
}