using System;

namespace AVS.CoreLib.ConsoleColors.Extensions
{
    public static class StringExtensions
    {
        public static string Colorize(this string str, string colorScheme)
        {
            if (!Colors.TryParse(colorScheme, out var colors))
                return str;

            return colors.Colorize(str);
        }
    }
}
