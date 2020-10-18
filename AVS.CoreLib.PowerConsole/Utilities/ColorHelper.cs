using System;

namespace AVS.CoreLib.PowerConsole.Utilities
{
    public class ColorHelper
    {
        /// <summary>
        /// Parse <see cref="ColorScheme"/> by its
        /// short representation: `-ForegroundColor` or `--BackgroundColor`
        /// or
        /// full representation: `-ForegroundColor --BackgroundColor`
        /// </summary>
        public static bool TryParse(string str, out ColorScheme scheme)
        {
            scheme = null;
            ConsoleColor color;
            var ind = str.LastIndexOf('-');
            
            //no `-` symbol just Color
            if (ind < 0)
            {
                //e.g. Red
                if (TryParseConsoleColor(str, out color))
                {
                    scheme = new ColorScheme(color);
                    return true;
                }
                return false;
            }
            
            scheme = new ColorScheme();

            //foreground color: -Color e.g. -Red
            if (ind == 0)
            {
                scheme.Foreground = TryParseConsoleColor(str.Substring(1), out color) 
                    ? color 
                    : ColorScheme.Default.Foreground;
                scheme.Background = ColorScheme.Default.Background;
            }
            //background color: --Color e.g. --Red  
            else if (ind == 1)
            {
                scheme.Foreground = ColorScheme.Default.Foreground;
                scheme.Background = TryParseConsoleColor(str.Substring(2), out color) 
                    ? color 
                    : ColorScheme.Default.Background;
            }
            else
            {
                //e.g. -Red --Gray
                var parts = str.Split(new[] {' ', '-'}, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                    return false;

                scheme.Foreground = TryParseConsoleColor(parts[0], out color)
                    ? color
                    : ColorScheme.Default.Foreground;
                scheme.Background = TryParseConsoleColor(parts[1], out color)
                    ? color
                    : ColorScheme.Default.Background;
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
/*
        public static bool TryExtractColor(ref string input, out ConsoleColor color)
        {
            var matches = input.GetMatches("@\\w+");
            if (matches.Length == 1 && Enum.TryParse(matches[0], out color))
            {
                input = input.Replace(matches[0], "");
                return true;
            }
            color = ConsoleColor.Gray;
            return false;
        }

        public static ConsoleColor ExtractColor(ref string input, ConsoleColor defaultColor)
        {
            var re = new Regex("@(?<value>\\w+)");
            var matches = re.Replace(ref input, "");
            if (matches.Length == 1 && Enum.TryParse(matches[0], out ConsoleColor color))
                return color;
            return defaultColor;
        }*/
    }
}