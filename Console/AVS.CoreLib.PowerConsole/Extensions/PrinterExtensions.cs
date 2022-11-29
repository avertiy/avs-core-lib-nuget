using System;
using System.Collections.Generic;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.PowerConsole.ConsoleTable;
using AVS.CoreLib.PowerConsole.Printers;
using AVS.CoreLib.PowerConsole.Structs;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole.Extensions
{
    public static class PrinterExtensions
    {
        public static void PrintJson<T>(this IPrinter printer, T obj, bool indented, ConsoleColor? color, bool endLine)
        {
            var str = obj.ToJsonString(indented);
            printer.Print(str, color, endLine);
        }

        public static void PrintArray<T>(this IPrinter printer, IEnumerable<T> enumerable,
            string? message,
            StringifyOptions? options,
            Func<T, string>? formatter,
            bool endLine)
        {
            string str;
            var tags = false;
            if (options == null)
                str = enumerable.Stringify(StringifyFormat.Default, ",", formatter);
            else
            {
                str = enumerable.Stringify(options.Format, options.Separator, formatter);
                str = options.Colors.Colorize(str);
                tags = options.ContainsCTags;
            }

            var text = message == null ? str : $"{message}{str}";
            printer.Print(text, endLine, tags);
        }

        public static void PrintDictionary<TKey, TValue>(this IPrinter printer,
            string? message, IDictionary<TKey, TValue> dictionary,
            StringifyOptions? options,
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


        public static void PrintTable(this IPrinter printer, Table table, ConsoleColor? color, bool endLine, bool containsCTags)
        {
            var str = table.ToString();
            printer.Print(str, color, endLine, containsCTags);
        }

        public static void PrintHeader(this IPrinter printer, string header, string template, string lineIndentation, ConsoleColor? color)
        {
            printer.WriteLine();
            var str = $"{template} {header} {template}{lineIndentation}";
            printer.Print(str, color, false);
        }

        public static void PrintTest(this IPrinter printer, string message, bool test, int padRight, bool endLine)
        {
            var str = message.PadRight(padRight) + (test ? "OK" : "Fail");
            printer.Print(str, test ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed, endLine);
        }

        public static void PrintTimeElapsed(this IPrinter printer, string? message, DateTime from, ConsoleColor? color, bool endLine)
        {
            var ms = (DateTime.Now - @from).TotalMilliseconds;
            if (ms < 0.5)
                return;

            printer.Print(message == null ? $"[elapsed:{ms:N3} ms]" : $"{message}   [elapsed:{ms:N3} ms]", color, endLine);
        }

        public static void PrintConsoleColors(this IPrinter printer)
        {
            var values = Enum.GetNames(typeof(ConsoleColor));
            foreach (var colorName in values)
            {
                var color = Enum.Parse<ConsoleColor>(colorName);
                printer.Print(colorName, color, true);
            }
        }
        public static void Print(this IPrinter printer, 
            string str, 
            MessageStatus status,
            string? timeFormat = "yyyy-MM-dd hh:mm:ss.ff", bool endLine = true)
        {
            if (!string.IsNullOrEmpty(timeFormat))
                str = $"{DateTime.Now.ToString(timeFormat)} {str}";
            var color = ColorScheme.GetStatusColorScheme(status);
            printer.Print(str, color, endLine);
        }

        public static void PrintError(this IPrinter printer, Exception ex,
            bool printStackTrace = true,
            string? timeFormat = "yyyy-MM-dd hh:mm:ss.ff")
        {
             printer.Print($"\r\n{ex.GetType().Name}: ", ConsoleColor.DarkRed, false);
             printer.Print(ex.Message, MessageStatus.Error, timeFormat);
            if (printStackTrace)
                printer.Print(ex.StackTrace, MessageStatus.Debug, null);
        }
    }
}