using System;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.PowerConsole.Enums;

namespace AVS.CoreLib.PowerConsole.Utilities
{
    using Console = System.Console;

    public struct ColorScheme
    {
        public static ColorScheme Default { get; internal set; }

        public static ColorScheme GetCurrentScheme()
        {
            return new ColorScheme(Console.ForegroundColor, Console.BackgroundColor);
        }

        public ConsoleColor Background { get; set; }
        public ConsoleColor Foreground { get; set; }

        public ColorScheme(ConsoleColor foreground)
        {
            Foreground = foreground;
            Background = Console.BackgroundColor;
        }

        public ColorScheme(ConsoleColor foreground, ConsoleColor background)
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

        /// <summary>
        /// classic console colors: gray / black
        /// </summary>
        public static ColorScheme Classic => new ColorScheme(ConsoleColor.Gray, ConsoleColor.Black);
        public static ColorScheme DarkGray => new ColorScheme(ConsoleColor.DarkGray);


        public static ColorScheme Critical { get; set; } = new ColorScheme(ConsoleColor.White, ConsoleColor.DarkRed);
        public static ColorScheme Error { get; set; } = new ColorScheme(ConsoleColor.Red);
        public static ColorScheme Warning { get; set; } = new ColorScheme(ConsoleColor.Yellow);
        public static ColorScheme Info { get; set; } = new ColorScheme(ConsoleColor.White);
        public static ColorScheme Success { get; set; } = new ColorScheme(ConsoleColor.Green);
        public static ColorScheme Important { get; set; } = new ColorScheme(ConsoleColor.White, ConsoleColor.DarkGray);
        public static ColorScheme Debug { get; set; } = new ColorScheme(ConsoleColor.DarkGray);

        /// <summary>
        /// Gets the color for specific message status enumeration value
        /// </summary>
        /// <param name="level">Status of the message</param>
        /// <returns>Console color for the passed message status enumeration value</returns>
        internal static ColorScheme GetColorScheme(MessageLevel level)
        {
            switch (level)
            {
                case MessageLevel.Debug:
                    return Debug;
                case MessageLevel.Info:
                    return Info;
                case MessageLevel.Success:
                    return Success;
                case MessageLevel.Important:
                    return Important;
                case MessageLevel.Warning:
                    return Warning;
                case MessageLevel.Error:
                    return Error;
                case MessageLevel.Critical:
                    return Critical;
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