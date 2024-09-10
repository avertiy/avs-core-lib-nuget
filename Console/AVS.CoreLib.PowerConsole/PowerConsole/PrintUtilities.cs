using System;
using System.Diagnostics;
using System.Linq;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.PowerConsole.Printers2;
using AVS.CoreLib.PowerConsole.Printers2.Extensions;

namespace AVS.CoreLib.PowerConsole
{
    using Console = System.Console;
    public static partial class PowerConsole
    {
        public static void PrintAllColors()
        {
            Printer2.PrintConsoleColors();
        }

        public static void PrintHeader(string header, Colors? colors = null, string template = "============", string indentation = "\r\n")
        {
            Printer2.PrintHeader(header, template, indentation, colors);
        }

        public static void PrintTest(bool test, string message, int padRight, PrintOptions2 options = PrintOptions2.Default)
        {
            Printer2.PrintTest(message, test, padRight, options);
        }

        public static void PrintTimeElapsed(
            DateTime dateTime,
            string? message = null,
            PrintOptions2 options = PrintOptions2.Default, Colors? colors = null)
        {
            Printer2.PrintTimeElapsed(message, dateTime, options, colors);
        }

        [Conditional("DEBUG")]
        public static void PrintDebug(string message, PrintOptions2 options = PrintOptions2.Default, Colors? colors = null)
        {
            Printer2.Print(MessageLevel.Debug, message, options, colors);
        }

        public static void PrintError(Exception ex, string? message = null, bool printStackTrace = true, PrintOptions2 options = PrintOptions2.Default)
        {
            Printer2.PrintError(ex, message, printStackTrace, options);
            if (BeepOnMessageLevels != null && BeepOnMessageLevels.Any(x => x == MessageLevel.Error))
                Console.Beep(2500, 1000);
        }
    }
}
