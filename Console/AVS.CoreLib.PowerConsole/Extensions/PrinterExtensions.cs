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
        public static void PrintJson<T>(this IPowerConsolePrinter printer, T obj, bool indented, ConsoleColor? color = null, bool endLine = true)
        {
            var str = obj.ToJsonString(indented);
            printer.Print(str, endLine, false, color);
        }

        public static void PrintDictionary<TKey, TValue>(this IPowerConsolePrinter printer,
            string? message, IDictionary<TKey, TValue> dictionary,
            PrintArrayOptions? options,
            Func<TKey, TValue, string>? formatter,
            bool endLine)
        {
            string str;
            var tags = false;
            if (options == null)
                str = dictionary.Stringify(StringifyFormat.Default, ",", ":", formatter);
            else
            {
                str = dictionary.Stringify(options.Format, options.Separator, options.KeyValueSeparator, formatter, options.MaxLength);
                str = options.Colors.Colorize(str);
                tags = options.ContainsCTags;
            }

            var text = message == null ? str : $"{message}{str}";
            printer.Print(text, endLine, tags);
        }


        public static void PrintTable(this IPowerConsolePrinter printer, Table table, bool endLine, bool? containsCTags, ConsoleColor? color)
        {
            var str = table.ToString();
            printer.Print(str, endLine, containsCTags, color);
        }

        public static void PrintHeader(this IPowerConsolePrinter printer, string header, string template, string lineIndentation, bool? containsCTags, ConsoleColor? color)
        {
            var str = $"{template} {header} {template}{lineIndentation}";
            printer.Print(str, false, containsCTags, color);
        }

        public static void PrintTest(this IPowerConsolePrinter printer, string message, bool test, int padRight, bool endLine, bool? containsCTags = null)
        {
            var str = message.PadRight(padRight) + (test ? "OK" : "Fail");
            printer.Print(str, endLine, containsCTags, test ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed);
        }

        public static void PrintTimeElapsed(this IPowerConsolePrinter printer, string? message, DateTime from, ConsoleColor? color, bool endLine)
        {
            var ms = (DateTime.Now - @from).TotalMilliseconds;
            if (ms < 0.5)
                return;

            var text = message == null ? $"[elapsed:{ms:N3} ms]" : $"{message}   [elapsed:{ms:N3} ms]";
            printer.Print(text, endLine, false, color);
        }

        public static void PrintConsoleColors(this IPowerConsolePrinter printer)
        {
            var values = Enum.GetNames(typeof(ConsoleColor));
            foreach (var colorName in values)
            {
                var color = Enum.Parse<ConsoleColor>(colorName);
                printer.Print(colorName, true, false, color);
            }
        }
        public static void Print(this IPowerConsolePrinter printer, 
            string str, 
            MessageStatus status,
            string? timeFormat = "yyyy-MM-dd hh:mm:ss.ff", bool endLine = true, bool? containsCTags = null)
        {
            if (!string.IsNullOrEmpty(timeFormat))
                str = $"{DateTime.Now.ToString(timeFormat)} {str}";
            var color = ColorScheme.GetStatusColorScheme(status);
            printer.Print(str, endLine, containsCTags, color);
        }

        public static void PrintError(this IPowerConsolePrinter printer, Exception ex,
            bool printStackTrace = true,
            string? timeFormat = "yyyy-MM-dd hh:mm:ss.ff")
        {
             printer.Print($"\r\n{ex.GetType().Name}: ", false, containsCTags: false, ConsoleColor.DarkRed);
             printer.Print(ex.Message, MessageStatus.Error, timeFormat, true, containsCTags: false);
            if (printStackTrace)
                printer.Print(ex.StackTrace, MessageStatus.Debug, null, containsCTags: false);
        }
    }
}