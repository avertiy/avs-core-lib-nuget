using System;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.PowerConsole.Enums;

namespace AVS.CoreLib.PowerConsole.Utilities
{
    using Console = System.Console;

    public struct ColorScheme
    {
        public static ColorScheme Default { get; internal set; }

        /// <summary>
        /// classic console colors: black / dark gray
        /// </summary>
        public static ColorScheme Classic => new ColorScheme(ConsoleColor.Black, ConsoleColor.DarkGray);

        public static ColorScheme GetCurrentScheme()
        {
            return new ColorScheme(Console.BackgroundColor, Console.ForegroundColor);
        }

        public ConsoleColor Background { get; set; }
        public ConsoleColor Foreground { get; set; }

        public ColorScheme(ConsoleColor foreground)
        {
            Foreground = foreground;
            Background = Console.BackgroundColor;
        }

        public ColorScheme(ConsoleColor background, ConsoleColor foreground)
        {
            Background = background;
            Foreground = foreground;
        }

        public override string ToString()
        {
            return $"-{Foreground} --{Background}";
        }

        public string Colorize(string text)
        {
            return $"{AnsiCodes.Color(Foreground)}{AnsiCodes.BgColor(Background)}{text}{AnsiCodes.RESET}";
        }

        /// <summary>
        /// converts strings in format -Color or --Color into ColorScheme
        /// </summary>
        public static explicit operator ColorScheme(string str)
        {
            if (ColorSchemeHelper.TryParse(str, out ColorScheme scheme))
                return scheme;
            throw new ArgumentException($"Invalid color scheme: {str}");
        }

        public static void ApplyScheme(ColorScheme scheme)
        {
            Console.ForegroundColor = scheme.Foreground;
            Console.BackgroundColor = scheme.Background;
        }

        public static void Reset()
        {
            ApplyScheme(Default);
        }

        #region Popular schemes
        public static ColorScheme Blue => new ColorScheme(ConsoleColor.Blue);
        public static ColorScheme DarkBlue => new ColorScheme(ConsoleColor.DarkBlue);
        public static ColorScheme Red => new ColorScheme(ConsoleColor.Red);
        public static ColorScheme DarkRed => new ColorScheme(ConsoleColor.DarkRed);
        public static ColorScheme Green => new ColorScheme(ConsoleColor.Green);
        public static ColorScheme DarkGreen => new ColorScheme(ConsoleColor.DarkGreen);
        public static ColorScheme Yellow => new ColorScheme(ConsoleColor.Yellow);
        public static ColorScheme DarkYellow => new ColorScheme(ConsoleColor.DarkYellow);
        public static ColorScheme Gray => new ColorScheme(ConsoleColor.Gray);
        public static ColorScheme DarkGray => new ColorScheme(ConsoleColor.DarkGray);
        public static ColorScheme Magenta => new ColorScheme(ConsoleColor.Magenta);
        public static ColorScheme Cyan => new ColorScheme(ConsoleColor.Cyan);
        #endregion

        public static ColorScheme Error { get; set; } = new ColorScheme(ConsoleColor.Red);
        public static ColorScheme Warning { get; set; } = new ColorScheme(ConsoleColor.DarkYellow);
        public static ColorScheme Info { get; set; } = new ColorScheme(ConsoleColor.Green);
        public static ColorScheme Debug { get; set; } = new ColorScheme(ConsoleColor.DarkGray);

        /// <summary>
        /// Gets the color for specific message status enumeration value
        /// </summary>
        /// <param name="status">Status of the message</param>
        /// <returns>Console color for the passed message status enumeration value</returns>
        internal static ColorScheme GetStatusColorScheme(MessageStatus status)
        {
            switch (status)
            {
                case MessageStatus.Debug:
                    return Debug;
                case MessageStatus.Info:
                    return Info;
                case MessageStatus.Warning:
                    return Warning;
                case MessageStatus.Error:
                    return Error;
                default:
                    return Default;
            }
        }

        #region Equality operators 
        public static bool operator ==(ColorScheme a, ColorScheme b)
        {
            return a.Foreground == b.Foreground && a.Background == b.Background;
        }

        public static bool operator !=(ColorScheme a, ColorScheme b)
        {
            return !(a == b);
        }

        public bool Equals(ColorScheme other)
        {
            return Background == other.Background && Foreground == other.Foreground;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is ColorScheme other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Background * 397) ^ (int)Foreground;
            }
        }
        #endregion
    }
    
}