﻿using System;
using AVS.CoreLib.ConsoleColors.Extensions;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.ConsoleColors
{
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

        public string Colorize(string text)
        {
            if (Foreground.HasValue && Background.HasValue)
                return $"{AnsiCodes.Color(Foreground.Value)}{AnsiCodes.BgColor(Background.Value)}{text}{AnsiCodes.RESET}";

            if (Foreground.HasValue)
                return $"{AnsiCodes.Color(Foreground.Value)}{text}{AnsiCodes.RESET}";

            if (Background.HasValue)
                return $"{AnsiCodes.BgColor(Background.Value)}{text}{AnsiCodes.RESET}";

            return text;
        }    

        public static Colors Empty => new Colors(null, null);

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

                if ((str.Contains("--", fromIndex: i) || str.Contains("bg", fromIndex: i)) && char.IsUpper(str[i + 2]))
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
    }
}