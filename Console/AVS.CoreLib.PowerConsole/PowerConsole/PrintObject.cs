using System;
using System.Collections.Generic;
using System.Globalization;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Stringify;
using AVS.CoreLib.PowerConsole.ConsoleTable;
using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.PowerConsole.Printers;
using AVS.CoreLib.PowerConsole.Structs;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.Text.Extensions;
using AVS.CoreLib.Text.Formatters.ColorMarkup;

namespace AVS.CoreLib.PowerConsole
{
    using Console = System.Console;
    public static partial class PowerConsole
    {
        public static void PrintJson<T>(T obj,
            bool indented = false,
            ConsoleColor? color = null,
            bool endLine = true)
        {
            Printer.PrintJson<T>(obj, indented, color, endLine);
            
        }

        public static void Dump<T>(T obj, ConsoleColor? color = null, bool endLine = true)
        {
            Printer.PrintJson<T>(obj, true, color, endLine);
        }
    }
}
