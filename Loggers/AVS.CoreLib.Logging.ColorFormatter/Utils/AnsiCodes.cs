namespace AVS.CoreLib.Logging.ColorFormatter.Utils;

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

    /// <summary>
    /// Returns a new string in which all occurrences of a <see cref="RESET"/> escape code are replaced with RESET+format.
    /// this needed when nesting formatting e.g. [red] text in red [bold]bold text[/bold] continue red text[/red]  
    /// </summary>
    public static string? ReformatAfterReset(this string? input, string format)
    {
        var text = input?.Replace(RESET, $"{RESET}{format}");
        return text;
    }

    public static string Color(ConsoleColor color)
    {
        return color switch
        {
            ConsoleColor.Black => Code(AnsiCode.Black),
            ConsoleColor.DarkRed => Code(AnsiCode.Red),
            ConsoleColor.DarkGreen => Code(AnsiCode.Green),
            ConsoleColor.DarkYellow => Code(AnsiCode.Yellow),
            ConsoleColor.DarkBlue => Code(AnsiCode.Blue),
            ConsoleColor.DarkMagenta => Code(AnsiCode.Magenta),
            ConsoleColor.DarkCyan => Code(AnsiCode.Cyan),
            ConsoleColor.Gray => Code(AnsiCode.White),
            ConsoleColor.DarkGray => Bright(AnsiCode.Black),
            ConsoleColor.Red => Bright(AnsiCode.Red),
            ConsoleColor.Green => Bright(AnsiCode.Green),
            ConsoleColor.Yellow => Bright(AnsiCode.Yellow),
            ConsoleColor.Blue => Bright(AnsiCode.Blue),
            ConsoleColor.Magenta => Bright(AnsiCode.Magenta),
            ConsoleColor.Cyan => Bright(AnsiCode.Cyan),
            ConsoleColor.White => Code(AnsiCode.White),
            _ => RESET
        };
    }

    public static string BgColor(ConsoleColor color)
    {
        return color switch
        {
            ConsoleColor.Black => Code(AnsiCode.Black +10),
            ConsoleColor.DarkRed => Code(AnsiCode.Red +10),
            ConsoleColor.DarkGreen => Code(AnsiCode.Green+10),
            ConsoleColor.DarkYellow => Code(AnsiCode.Yellow + 10),
            ConsoleColor.DarkBlue => Code(AnsiCode.Blue + 10),
            ConsoleColor.DarkMagenta => Code(AnsiCode.Magenta + 10),
            ConsoleColor.DarkCyan => Code(AnsiCode.Cyan + 10),
            ConsoleColor.Gray => Code(AnsiCode.White + 10),
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


    public static string FromTag(Tag tag)
    {
        var val = (int)tag;
        var ansiCode = val > 1000 ? Bright(val - 1000) : Code(val);
        return ansiCode;
    }
}

public enum AnsiCode
{
    Reset = 0,
    /// <summary>
    /// Applies brightness/intensity flag to foreground color
    /// </summary>
    Bold = 1,
    Dim = 2,
    Underline = 4,
    /// <summary>
    /// Removes underline
    /// </summary>
    NoUnderline = 24,
    /// <summary>
    /// Swaps foreground and background colors
    /// </summary>
    Reversed = 7,
    /// <summary>
    /// Returns foreground/background to normal
    /// </summary>
    NoReversed = 27,
    Black = 30,
    Red = 31,
    Green = 32,
    Yellow = 33,
    Blue = 34,
    Magenta = 35,
    Cyan = 36,
    White = 37,
}
