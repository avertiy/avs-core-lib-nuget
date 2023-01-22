using System;
using System.Collections.Generic;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Stringify;
using AVS.CoreLib.PowerConsole.ConsoleTable;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.PowerConsole.Printers;
using AVS.CoreLib.PowerConsole.Structs;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.Extensions
{
    public static class PrinterExtensions
    {
        public static void PrintJson<T>(this IPowerConsolePrinter printer, T obj, JsonPrintOptions options)
        {
            var str = obj.ToJsonString(options.Indented);
            printer.Print(str, options);
        }

        public static void PrintTable(this IPowerConsolePrinter printer, Table table, PrintOptions options)
        {
            var str = table.ToString();
            printer.Print(str, options);
        }

        public static void PrintHeader(this IPowerConsolePrinter printer, string header, HeaderPrintOptions options)
        {
            var str = $"{options.LineIndentation}{options.Template} {header} {options.Template}{options.LineIndentation}";
            printer.Print(str, options);
        }

        public static void PrintTest(this IPowerConsolePrinter printer, string message, bool test, int padRight, 
            PrintOptions options)
        {
            var str = message.PadRight(padRight) + (test ? "OK" : "Fail");
            options.Color = test ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed;
            printer.Print(str, options);
        }

        public static void PrintTimeElapsed(this IPowerConsolePrinter printer, string? message, DateTime from, PrintOptions options)
        {
            var ms = (DateTime.Now - @from).TotalMilliseconds;
            if (ms < 0.5)
                return;

            var text = message == null ? $"[elapsed:{ms:N1} ms]" : $"{message}   [elapsed:{ms:N1} ms]";
            printer.Print(text, options);
        }

        public static void PrintConsoleColors(this IPowerConsolePrinter printer)
        {
            var values = Enum.GetNames(typeof(ConsoleColor));
            foreach (var colorName in values)
            {
                var options = PrintOptions.Default.Clone();
                options.Color = Enum.Parse<ConsoleColor>(colorName);
                options.EndLine = true;
                options.ColorTags = false;

                printer.Print(colorName, options);
            }
        }
        
        public static void PrintError(this IPowerConsolePrinter printer,
            Exception ex,
            string? message,
            bool printStackTrace,
            PrintOptions options)
        {
            var type = ex.GetType().Name;

            var str = message == null ? $"{ex.Message} ({type})" : $"{message} - {ex.Message} ({type})";
            printer.Print(str, options);

            if (printStackTrace)
                printer.Print(ex.StackTrace, ColorScheme.GetColorScheme(MessageLevel.Debug));
        }
    }
}