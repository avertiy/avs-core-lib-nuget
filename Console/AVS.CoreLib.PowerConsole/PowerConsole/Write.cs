using System;
using AVS.CoreLib.Console.ColorFormatting;

namespace AVS.CoreLib.PowerConsole
{
    /// <remarks>
    /// Write/WriteLine methods provide pure behavior i.e. no c-tags, ansi-codes etc.
    /// The message argument is written to output stream <see cref="Out"/>
    /// </remarks>
    public static partial class PowerConsole
    {
        public static void Write(string str)
        {
            Printer2.Write(str);
        }
     
        public static void Write(string str, ConsoleColor color)
        {
            Printer2.Write(str, color);
        }

        public static void Write(string str, Colors colors)
        {
            Printer2.Write(str, colors);
        }
    }
}
