namespace AVS.CoreLib.Console.ColorFormatting.Tags
{
    /// <summary>
    /// CTags (color tags) or simply tags correlate with ansi codes, the purpose of tags is to make string color formatting human-readable
    /// when string contains ansi codes it's difficult to understand what's there
    /// e.g. "my string contains <Red>red text</Red>" is easier for human than
    /// the string with ansi codes "my string contains \u001b[91m;red text\u001b[0m;"
    /// but these are no equivalents to print string on console with colors tags needs to be translated (interpreted) as ansi codes
    /// so only the second string will produce a colorized output on console
    /// </summary>
    public enum CTag
    {
        #region decorations
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
        #endregion

        #region colors
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
        #endregion

        #region bright colors
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
        #endregion

        #region bg colors
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
        #endregion
    }

    public static class TagExtensions
    {
        public static string ToAnsiCode(this CTag tag)
        {
            var val = (int)tag;
            var ansiCode = val > 1000 ? AnsiCodes.Bright(val - 1000) : AnsiCodes.Code(val);
            return ansiCode;
        }

        public static string Wrap(this CTag tag, string str)
        {
            return $"<{tag}>{str}</{tag}>";
        }
    }
}