using System.Text;

namespace AVS.CoreLib.Logging.ColorFormatter.Utils
{
    public sealed class AnsiCodesHelper
    {
        public const string DEFAULT_FOREGROUND_COLOR = "\x1B[39m\x1B[22m"; // reset to default foreground color
        public const string DEFAULT_BACKGROUND_COLOR = "\x1B[49m"; // reset to the background color

        public static string GetForegroundColorEscapeCode(ConsoleColor color)
        {
            return color switch
            {
                ConsoleColor.Black => "\x1B[30m",
                ConsoleColor.DarkRed => "\x1B[31m",
                ConsoleColor.DarkGreen => "\x1B[32m",
                ConsoleColor.DarkYellow => "\x1B[33m",
                ConsoleColor.DarkBlue => "\x1B[34m",
                ConsoleColor.DarkMagenta => "\x1B[35m",
                ConsoleColor.DarkCyan => "\x1B[36m",
                ConsoleColor.Gray => "\x1B[37m",
                ConsoleColor.DarkGray => "\x1B[1m\x1B[30m",
                ConsoleColor.Red => "\x1B[1m\x1B[31m",
                ConsoleColor.Green => "\x1B[1m\x1B[32m",
                ConsoleColor.Yellow => "\x1B[1m\x1B[33m",
                ConsoleColor.Blue => "\x1B[1m\x1B[34m",
                ConsoleColor.Magenta => "\x1B[1m\x1B[35m",
                ConsoleColor.Cyan => "\x1B[1m\x1B[36m",
                ConsoleColor.White => "\x1B[1m\x1B[37m",
                _ => DEFAULT_FOREGROUND_COLOR // default foreground color
            };
        }

        public static string GetColorAnsiCode(ConsoleColor color, bool foreground = true)
        {
            return foreground
                ? GetForegroundColorEscapeCode(color)
                : GetBackgroundColorEscapeCode(color);
        }

        public static string Colorize(string text, string colorScheme)
        {
            var cc = ConsoleColors.Parse(colorScheme);
            var sb = new StringBuilder();

            if (cc.Background.HasValue)
                sb.Append(AnsiCodesHelper.GetBackgroundColorEscapeCode(cc.Background.Value));

            if (cc.Foreground.HasValue)
                sb.Append(AnsiCodesHelper.GetForegroundColorEscapeCode(cc.Foreground.Value));

            sb.Append(text);

            // restore console colors
            if (cc.Background.HasValue)
                sb.Append(AnsiCodesHelper.GetBackgroundColorEscapeCode(Console.BackgroundColor));

            if (cc.Foreground.HasValue)
                sb.Append(AnsiCodesHelper.GetForegroundColorEscapeCode(Console.ForegroundColor));

            return sb.ToString();
        }

        public static string GetBackgroundColorEscapeCode(ConsoleColor color)
        {
            return color switch
            {
                ConsoleColor.Black => "\x1B[40m",
                ConsoleColor.DarkRed => "\x1B[41m",
                ConsoleColor.DarkGreen => "\x1B[42m",
                ConsoleColor.DarkYellow => "\x1B[43m",
                ConsoleColor.DarkBlue => "\x1B[44m",
                ConsoleColor.DarkMagenta => "\x1B[45m",
                ConsoleColor.DarkCyan => "\x1B[46m",
                ConsoleColor.Gray => "\x1B[47m",
                ConsoleColor.DarkGray => "\x1B[100m",
                ConsoleColor.Red => "\x1B[101m",
                ConsoleColor.Green => "\x1B[102m",
                ConsoleColor.Yellow => "\x1B[103m",
                ConsoleColor.Blue => "\x1B[104m",
                ConsoleColor.Magenta => "\x1B[105m",
                ConsoleColor.Cyan => "\x1B[106m",
                ConsoleColor.White => "\x1B[107m",
                _ => DEFAULT_BACKGROUND_COLOR // Use default background color
            };
        }

        public static void PrintAnsiColors(int from = 30, int to =255)
        {
            for (var i = from; i < to; i++)
            {
                var c = $"\x1B[{i}m";
                Console.WriteLine($"{c} test - \\x1B[{i}m {DEFAULT_FOREGROUND_COLOR}");
            }

            for (var i = from; i < to; i++)
            {
                var c = $"\x1B[1m\x1B[{i}m";
                Console.WriteLine($"{c} test- \\x1B[1m\\x1B[{i}m {DEFAULT_FOREGROUND_COLOR}");
            }

        }

        public static void PrintPalete(bool foregroundColors = true, bool backgroundColors = true)
        {
            Console.WriteLine("Ansi color palete");
            var colors = Enum.GetValues<ConsoleColor>();

            if (foregroundColors)
            {
                Console.WriteLine("Foreground colors:\r\n");
                foreach (var color in colors)
                {
                    var foreground = GetForegroundColorEscapeCode(color);
                    var suffix = foreground == DEFAULT_FOREGROUND_COLOR ? "[DEFAULT_FOREGROUND_COLOR]" : "";

                    Console.Write("Color: ");
                    Write(color.ToString(), color);
                    Console.Write($" => {foreground.Replace("\x1B", "\\x1B")}");
                    Console.Write(suffix + $" {foreground}test color" + DEFAULT_FOREGROUND_COLOR);
                    Console.WriteLine();
                }
            }

            if (backgroundColors)
            {
                Console.WriteLine("Background colors:\r\n");
                foreach (var color in colors)
                {
                    var background = GetBackgroundColorEscapeCode(color);
                    var suffix = background == DEFAULT_BACKGROUND_COLOR ? "[DEFAULT_FOREGROUND_COLOR]" : "";

                    Console.Write("Color: ");
                    WriteB(color.ToString(), color);
                    Console.Write($" => {background.Replace("\x1B", "\\x1B")}");
                    Console.Write(suffix + $" {background}test color" + DEFAULT_BACKGROUND_COLOR);
                    Console.WriteLine();
                }
            }
        }

        private static void Write(string value, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            if (color == ConsoleColor.Black)
            {
                Console.BackgroundColor = ConsoleColor.DarkYellow;
            }
            Console.Write(value);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        private static void WriteB(string value, ConsoleColor color)
        {
            Console.BackgroundColor = color;
            Console.Write(value);
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
