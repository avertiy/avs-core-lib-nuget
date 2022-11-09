using AVS.CoreLib.ConsoleColors;
using AVS.CoreLib.Logging.ColorFormatter.Utils;

namespace AVS.CoreLib.Logging.ColorFormatter.Enums;
/// <summary>
/// tags to control console color behaviour by means of using ansi codes,
/// tag values are convertible to ansi codes
/// </summary>
public enum Tag
{
    /// <summary>
    /// Bold/bright
    /// </summary>
    B = 1,
    /// <summary>
    /// Dim
    /// </summary>
    D = 2,
    /// <summary>
    /// underline
    /// </summary>
    U = 4,
    /// <summary>
    /// Reversed
    /// </summary>
    R = 7,

    Black = 30,
    DarkRed = 31,
    DarkGreen = 32,
    DarkYellow = 33,
    DarkBlue = 34,
    DarkMagenta = 35,
    DarkCyan = 36,
    Gray = 37,
    DarkGray = 90,
    Red = 91,
    Green = 92,
    Yellow = 93,
    Blue = 94,
    Magenta = 95,
    Cyan = 96,
    White = 97,

    //bright codes
    /// <summary>
    /// Bright Reversed
    /// </summary>
    BR = 1007,
    BrightRed = 1031,
    BrightGreen = 1032,
    BrightYellow = 1033,
    BrightBlue = 1034,
    BrightMagenta = 1035,
    BrightCyan = 1036,
    BrightWhite = 1037,

    bgBlack = 40,
    bgDarkBlue = 44,
    bgDarkGreen = 42,
    bgDarkCyan = 46,
    bgDarkRed = 41,
    bgDarkMagenta = 45,
    bgDarkYellow = 43,
    bgGray = 47,
    bgDarkGray = 100,
    bgBlue = 104,
    bgGreen = 1102,
    bgCyan = 1106,
    bgRed = 1101,
    bgMagenta = 1105,
    bgYellow = 103,
    bgWhite = 107,

    //non ansi related tags
    H1,
    H2,
    H3
}

public static class TagExtensions
{
    public static string ToAnsiCode(this Tag tag)
    {
        var val = (int)tag;
        var ansiCode = val > 1000 ? AnsiCodes.Bright(val - 1000) : AnsiCodes.Code(val);
        return ansiCode;
    }
}