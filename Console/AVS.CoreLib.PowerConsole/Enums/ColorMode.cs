namespace AVS.CoreLib.PowerConsole.Enums
{
    /// <summary>
    /// Defines console color mode whether it is colorful or plain, 
    /// There are 2 color modes:
    /// 1) based on switching console color through Console.ForegroundColor
    /// 2) inject into message ansi codes that System.Console recognize, but that does not work when app is run from command line
    /// </summary>
    public enum ColorMode
    {
        /// <summary>
        /// The default mode coloring based on ansi-color codes
        /// </summary>
        Default = 0,
        /// <summary>
        /// print colored text using ansi color codes <see cref="Console.ColorFormatting.AnsiCodes"/>
        /// </summary>
        AnsiCodes = 1,
        /// <summary>
        /// print plain text, i.e. no colors removing color tags and color-markup 
        /// </summary>
        PlainText = 2,
        /// <summary>
        /// print colored text coloring is based on switch console foreground & background colors 
        /// </summary>
        SwitchColors = 3
    }
}