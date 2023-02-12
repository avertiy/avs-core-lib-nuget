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
        public static void Write(string str)
        {
            Printer.Write(str);
        }
     
        public static void Write(string str, ConsoleColor color, bool endLine = false)
        {
            Printer.Write(str, color, endLine);
        }

        public static void Write(string str, ColorScheme scheme, bool endLine = false)
        {
            var options = PrintOptions.FromColorScheme(scheme, endLine: endLine);
            Printer.Write(str, options);
        }

        public static void Write(string str, PrintOptions options)
        {
            Printer.Write(str, options);
        }
    }
}
