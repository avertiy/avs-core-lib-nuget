using System;

namespace AVS.CoreLib.ConsoleColors
{
    /// <summary>
    /// https://learn.microsoft.com/en-us/windows/console/console-virtual-terminal-sequences
    /// </summary>
    public static class AnsiCodes
    {
        //public const string DEFAULT_FOREGROUND_COLOR = "\x1B[39m\x1B[22m";
        public const string RESET = "\u001b[0m";
        //public const string RESET = "RESET";//"\u001b[0m";
        public static string Code(int code)
        {
            //return $"\x1B[{code}m";
            return $"\u001b[{code}m";
        }

        public static string Code(AnsiCode code)
        {
            return $"\u001b[{(int)code}m";
        }

        public static string Bright(AnsiCode code)
        {
            return $"\u001b[{(int)code};1m";
        }

        public static string Bright(int code)
        {
            //return $"\x1B[1m\x1B[{code}m"; - the same
            return $"\u001b[{code};1m";
        }

        public static string Esc(this string str)
        {
            return str.Replace("\u001b", "\\u001b").Replace("\x1B", "\\x1B");
        }

        public static string Rgb(byte r, byte g, byte b)
        {
            return $"\u001b[38;2;{r};{g};{b}m";
        }

        public static string Rgb((byte R, byte G, byte B) rgb)
        {
            return $"\u001b[38;2;{rgb.R};{rgb.G};{rgb.B}m";
        }

        public static string BgRgb(byte r, byte g, byte b)
        {
            return $"\u001b[48;2;{r};{g};{b}m";
        }

        public static string Color(ConsoleColor color)
        {
            return color switch
            {
                ConsoleColor.Black => Code(AnsiCode.Black),
                ConsoleColor.DarkRed => Code(AnsiCode.DarkRed),
                ConsoleColor.DarkGreen => Code(AnsiCode.DarkGreen),
                ConsoleColor.DarkYellow => Code(AnsiCode.DarkYellow),
                ConsoleColor.DarkBlue => Code(AnsiCode.DarkBlue),
                ConsoleColor.DarkMagenta => Code(AnsiCode.DarkMagenta),
                ConsoleColor.DarkCyan => Code(AnsiCode.DarkCyan),
                ConsoleColor.Gray => Code(AnsiCode.Gray),
                ConsoleColor.DarkGray => Code(AnsiCode.DarkGray),
                ConsoleColor.Red => Code(AnsiCode.Red),
                ConsoleColor.Green => Code(AnsiCode.Green),
                ConsoleColor.Yellow => Code(AnsiCode.Yellow),
                ConsoleColor.Blue => Code(AnsiCode.Blue),
                ConsoleColor.Magenta => Code(AnsiCode.Magenta),
                ConsoleColor.Cyan => Code(AnsiCode.Cyan),
                ConsoleColor.White => Code(AnsiCode.White),
                _ => RESET
            };
        }

        public static string BgColor(ConsoleColor color)
        {
            return color switch
            {
                ConsoleColor.Black => Code(AnsiCode.Black +10),
                ConsoleColor.DarkRed => Code(AnsiCode.DarkRed +10),
                ConsoleColor.DarkGreen => Code(AnsiCode.DarkGreen+10),
                ConsoleColor.DarkYellow => Code(AnsiCode.DarkYellow + 10),
                ConsoleColor.DarkBlue => Code(AnsiCode.DarkBlue + 10),
                ConsoleColor.DarkMagenta => Code(AnsiCode.DarkMagenta + 10),
                ConsoleColor.DarkCyan => Code(AnsiCode.DarkCyan + 10),
                ConsoleColor.Gray => Code(AnsiCode.Gray + 10),
                ConsoleColor.DarkGray => "\x1B[100m",
                ConsoleColor.Red => "\x1B[101m",
                ConsoleColor.Green => "\x1B[102m",
                ConsoleColor.Yellow => "\x1B[103m",
                ConsoleColor.Blue => "\x1B[104m",
                ConsoleColor.Magenta => "\x1B[105m",
                ConsoleColor.Cyan => "\x1B[106m",
                ConsoleColor.White => "\x1B[107m",
                _ => RESET
            };
        }

    }
}