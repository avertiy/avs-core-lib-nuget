using AVS.CoreLib.Console.ColorFormatting.Tags;

namespace AVS.CoreLib.Console.ColorFormatting.Extensions
{
    public static class StringExtensions
    {
        public static string Colorize(this string str, string colorScheme)
        {
            if (!Colors.TryParse(colorScheme, out var colors))
                return str;

            return colors.Colorize(str);
        }

        public static string WrapInTag(this string str, CTag tag)
        {
            return $"<{tag}>{str}</{tag}>";
        }
    }
}
