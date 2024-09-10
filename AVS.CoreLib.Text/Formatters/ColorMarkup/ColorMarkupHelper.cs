using System;
using System.Text;

namespace AVS.CoreLib.Text.Formatters.ColorMarkup
{
    /// <summary>
    /// provides color markup text utilities
    /// </summary>
    public static class ColorMarkupHelper
    {
        /// <summary>
        /// match text with any color markup (i) text {arg:-Red}  (ii) some text@:Color
        /// returns true if match success, false otherwise
        /// </summary>
        public static bool HasAnyColorMarkup(this string text)
        {
            var match = ColorMarkupString.regex.Match(text);
            var match2 = ColorMarkupString.regex2.Match(text);
            return match.Success || match2.Success;
        }

        /// <summary>
        /// match text with color markup regex e.g. text {arg:-Red --Blue}
        /// returns true if match success, false otherwise
        /// </summary>
        public static bool HasColorMarkup(this string text)
        {
            var match = ColorMarkupString.regex.Match(text);
            return match.Success;
        }

        /// <summary>
        /// match text with color markup2 regex e.g. some text@:-Red
        /// returns true if match success, false otherwise
        /// </summary>
        public static bool HasColorMarkup2(this string text, out string color, out int index)
        {
            var match = ColorMarkupString.regex2.Match(text);
            color = match.Success ? match.Groups["color"].Value : null;
            index = match.Success ? match.Index : text.Length;
            return match.Success;
        }



        /// <summary>
        /// remove color markup from the text
        /// </summary>
        public static string StripColorMarkup(this string text)
        {
            var match = ColorMarkupString.regex.Match(text);

            if (!match.Success)
                return text;

            var sb = new StringBuilder();
            var pos = 0;

            while (match.Success)
            {
                var pos2 = match.Index - pos;
                if (pos2 > 0)
                {
                    //plain text
                    sb.Append(text.Substring(pos, pos2));
                }

                var coloredText = match.Groups["text"].Value;
                sb.Append(coloredText);

                match = match.NextMatch();
                pos = match.Index + match.Length;
            }

            if (pos < text.Length)
            {
                var restText = text.Substring(pos);
                sb.Append(restText);
            }

            return sb.ToString();
        }

        /// <summary>
        /// remove color markup2 from the text e.g. "text@:Red" => "text"
        /// </summary>
        public static string StripColorMarkup2(this string text)
        {
            var match2 = ColorMarkupString.regex2.Match(text);

            // strip @:Color i.e. markup at the end of the string  
            if (match2.Success)
                text = text.Substring(0, match2.Index);

            return text;
        }

        /// <summary>
        /// format plain text into color markup text e.g. FormatColor("str", Green) => {"str":-Green}; 
        /// </summary>
        public static string FormatWithColor(string str, ConsoleColor foreground)
        {
            return $"{{{str}:-{foreground}}}";
        }

        /// <summary>
        /// Colorize plain text within a text that might contain color markup
        /// </summary>
        /// <remarks>
        /// if text does not contain a color markup, use <see cref="FormatWithColor"/>
        /// </remarks>
        /// <returns></returns>
        public static string ColorizePlainText(string text, ConsoleColor? foreground)
        {
            if (!foreground.HasValue)
                return text;

            var sb = new StringBuilder();
            var match = ColorMarkupString.regex.Match(text);
            var pos = 0;

            while (match.Success)
            {
                var pos2 = match.Index - pos;
                if (pos2 > 0)
                {
                    var plainText = text.Substring(pos, pos2);
                    sb.Append(FormatWithColor(plainText, foreground.Value));
                }

                var coloredText = match.Value;
                sb.Append(coloredText);
                pos = match.Index + match.Length;
                match = match.NextMatch();
            }

            if (pos < text.Length)
            {
                var restText = text.Substring(pos);
                sb.Append(FormatWithColor(restText, foreground.Value));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Parse color markup e.g. "{arg:-Reg --Black}", also short representation "{arg:-Reg}" or "{arg:--Reg}" 
        /// </summary>
        public static bool TryParse(string str, out ConsoleColor? foreground, out ConsoleColor? background)
        {
            foreground = null;
            background = null;
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            ConsoleColor color;
            var ind = str.LastIndexOf('-');

            //no `-` symbol just Color
            if (ind < 0)
            {
                //e.g. Red
                if (!TryParseConsoleColor(str, out color))
                    return false;
                foreground = color;
                return true;
            }

            //foreground color: -Color e.g. -Red
            if (ind == 0 && TryParseConsoleColor(str.Substring(1), out color))
            {
                foreground = color;
            }
            //background color: --Color e.g. --Red  
            else if (ind == 1 && TryParseConsoleColor(str.Substring(2), out color))
            {
                background = color;
            }
            else
            {
                //e.g. -Red --Gray
                var parts = str.Split(new[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                    return false;

                foreground = TryParseConsoleColor(parts[0], out color)
                    ? color
                    : (ConsoleColor?)null;
                background = TryParseConsoleColor(parts[1], out color)
                    ? color
                    : (ConsoleColor?)null;
            }

            return true;
        }

        private static bool TryParseConsoleColor(string value, out ConsoleColor color)
        {
            if (Enum.TryParse(value, out color))
            {
                return true;
            }
            color = ConsoleColor.Black;
            return false;
        }
    }
}