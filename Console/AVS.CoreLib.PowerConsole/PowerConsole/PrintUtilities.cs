using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using AVS.CoreLib.Extensions;
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
        public static void PrintAllColors()
        {
            Printer.PrintConsoleColors();
        }

        public static void PrintHeader(string header, HeaderPrintOptions? options = null)
        {
            Printer.PrintHeader(header, options ?? HeaderPrintOptions.Options);
        }

        public static void PrintTest(bool test, string message, int padRight, PrintOptions? options = null)
        {
            Printer.PrintTest(message, test, padRight, options ?? DefaultOptions);
        }

        public static void PrintTimeElapsed(
            DateTime dateTime,
            string? message = null,
            PrintOptions? options = null)
        {
            Printer.PrintTimeElapsed(message, dateTime, options ?? DefaultOptions);
        }

        [Conditional("DEBUG")]
        public static void PrintDebug(string message, PrintOptions? options = null)
        {
            options = options ?? PrintOptions.Debug;
            WriteLine(message, options);
        }

        public static void PrintError(Exception ex, string? message = null, bool printStackTrace = true, PrintOptions? options = null)
        {
            options = options ?? PrintOptions.Error;
            Printer.PrintError(ex, message, printStackTrace, options);
            if (BeepOnMessageLevels != null && BeepOnMessageLevels.Any(x => x == options.Level))
                Console.Beep(2500, 1000);
        }
    }
}
