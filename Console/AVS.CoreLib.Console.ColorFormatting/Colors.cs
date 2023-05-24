using System;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Console.ColorFormatting
{
    /// <summary>
    /// Foreground/Background colors
    /// used for colorizing text (either ansi codes or color tags markup) 
    /// {arg:-Red -bgGray} means Red - foreground color, Gray - background color
    /// </summary>
    public readonly struct Colors
    {
        public ConsoleColor? Foreground { get; }
        public ConsoleColor? Background { get; }
        public bool HasValue => Foreground.HasValue || Background.HasValue;
        public Colors(ConsoleColor? foreground, ConsoleColor? background)
        {
            Foreground = foreground;
            Background = background;
        }

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

        public static Colors Empty { get; } = new Colors(null, null);

        /// <summary>
        /// supported formats:
        /// foreground: -Color or Color
        /// background: --Color or bgColor
        /// both: -Color --Color or -Color bgColor
        /// </summary>
        public static Colors Parse(string str)
        {
            if (!TryGetColors(str, out var color, out var bgColor))
                return Colors.Empty;

            return new Colors(color, bgColor);
        }

        public static bool TryParse(string str, out Colors colors)
        {
            if (!TryGetColors(str, out var color, out var bgColor))
            {
                colors = Colors.Empty;
                return false;
            }

            colors = new Colors(color, bgColor);
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

                if ((str.ContainsAt("--", index: i) || str.ContainsAt("bg", index: i)) && char.IsUpper(str[i + 2]))
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

        /// <summary>
        /// converts strings in format -Color or --Color into ColorScheme
        /// </summary>
        public static implicit operator Colors(ConsoleColor color)
        {
            return new Colors(color, null);
        }

        #region Equality operators 
        public static bool operator ==(Colors a, Colors b)
        {
            return a.Foreground == b.Foreground && a.Background == b.Background;
        }

        public static bool operator !=(Colors a, Colors b)
        {
            return !(a == b);
        }

        public bool Equals(Colors other)
        {
            return Background == other.Background && Foreground == other.Foreground;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is Colors other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (397 * (int)Background.GetValueOrDefault()) ^ (int)Foreground.GetValueOrDefault();
            }
        }
        #endregion
    }

    public static class ColorsExtensions
    {
        public static string Colorize(this Colors colors, string text)
        {
            if (colors.Foreground.HasValue && colors.Background.HasValue)
                return $"{AnsiCodes.Color(colors.Foreground.Value)}{AnsiCodes.BgColor(colors.Background.Value)}{text}{AnsiCodes.RESET}";

            if (colors.Foreground.HasValue)
                return $"{AnsiCodes.Color(colors.Foreground.Value)}{text}{AnsiCodes.RESET}";

            if (colors.Background.HasValue)
                return $"{AnsiCodes.BgColor(colors.Background.Value)}{text}{AnsiCodes.RESET}";

            return text;
        }
    }
}