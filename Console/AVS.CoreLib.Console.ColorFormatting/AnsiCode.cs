namespace AVS.CoreLib.Console.ColorFormatting
{
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

        //Black = 30,
        //DarkRed = 31,
        //DarkGreen = 32,
        //DarkYellow = 33,
        //DarkBlue = 34,
        //DarkMagenta = 35,
        //DarkCyan = 36,
        //Gray = 37,
    }
}