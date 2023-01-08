using System;
using System.Diagnostics;
using System.Linq;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.PowerConsole.Printers;
using AVS.CoreLib.PowerConsole.Utilities;

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

        public static void WriteLine(PrintOptions? options = null)
        {
            Printer.WriteLine(null, options ?? DefaultOptions);
        }

        ///// <summary>
        ///// Writes the specified string value, followed by the current line terminator, to console output stream.
        ///// </summary>
        /// <param name="str">string value to write</param>
        /// <param name="options"><see cref="PrintOptions"/></param>
        public static void WriteLine(string str, PrintOptions? options = null)
        {
            options = options ?? DefaultOptions;
            Printer.WriteLine(str, options);

            if (BeepOnMessageLevels != null && BeepOnMessageLevels.Contains(options.Level))
                Console.Beep();
        }

        public static void WriteLine(string str, Action<PrintOptions> configure)
        {
            var options = DefaultOptions.Clone();
            configure(options);
            WriteLine(str, options);
        }


        public static void WriteLine(int posX, int posY, params string[] arr)
        {
            ClearRegion(posX, posY, arr.Length);
            foreach (var text in arr)
            {
                Console.WriteLine(text);
            }
        }

        #endregion
    }
}
