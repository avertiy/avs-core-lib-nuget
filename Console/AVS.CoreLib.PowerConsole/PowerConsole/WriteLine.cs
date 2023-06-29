using System;
using System.Linq;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.PowerConsole.Enums;

namespace AVS.CoreLib.PowerConsole
{
    using Console = System.Console;

    /// <remarks>
    /// Write/WriteLine methods provide pure behavior i.e. no c-tags, ansi-codes etc.
    /// The message argument is written to output stream <see cref="Out"/>
    /// </remarks>
    public static partial class PowerConsole
    {
        #region WriteLine

        public static void WriteLine(bool voidEmptyLines = true)
        {
            Printer2.WriteLine(voidEmptyLines);
        }

        ///// <summary>
        ///// Writes the specified string value, followed by the current line terminator, to console output stream.
        ///// </summary>
        public static void WriteLine(string str, Colors? colors = null)
        {
            Printer2.WriteLine(str, colors);

            if (BeepOnMessageLevels != null && BeepOnMessageLevels.Contains(MessageLevel.Default))
                Console.Beep();
        }

        #endregion

        public static void WriteLine(int posX, int posY, params string[] arr)
        {
            ClearRegion(posX, posY, arr.Length);
            foreach (var text in arr)
            {
                Console.WriteLine(text);
            }
        }
    }
}
