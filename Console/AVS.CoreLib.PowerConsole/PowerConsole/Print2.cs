using System;
using System.Collections.Generic;
using System.Globalization;
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
        public static void PrintArray<T>(
            IEnumerable<T> enumerable,
            string? message = null,
            StringifyOptions? options = null,
            Func<T, string>? formatter = null,
            bool endLine = true)
        {
            Printer.PrintArray(enumerable, message, options, formatter, endLine);
        }

        public static void PrintAllColors()
        {
            Printer.PrintConsoleColors();
        }

        public static void PrintKeyValue<TKey, TValue>(IDictionary<TKey, TValue> dictionary,
            ConsoleColor color = ConsoleColor.White,
            string keyValueSeparator = " => ",
            string separator = "\r\n",
            bool endLine = true)
        {
            Printer.Print(dictionary.ToKeyValueString(keyValueSeparator, separator), endLine, false, color);
        }

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

        public static void PrintHeader(string header, ConsoleColor color = ConsoleColor.Cyan,
            string template = "============", string lineIndentation = "\r\n\r\n")
        {
            Printer.PrintHeader(header, template, lineIndentation, false, color);
        }

        #region PrintTable
        public static void PrintTable<T>(IEnumerable<T> data,
            ConsoleColor color = ConsoleColor.White,
            bool endLine = true)
        {
            var table = Table.Create(data);
            Printer.PrintTable(table, endLine, false, color);
        }

        public static void PrintTable<T>(IEnumerable<T> data,
            Action<Table>? configure = null,
            bool endLine = true)
        {
            var table = Table.Create(data);
            configure?.Invoke(table);
            Printer.PrintTable(table, endLine, false, null);
        }

        public static void PrintTable(Table table,
            ConsoleColor color = ConsoleColor.White,
            bool endLine = true, bool containsTags = false)
        {
            Printer.PrintTable(table, endLine, containsTags, color);
        }

        #endregion

        public static void PrintTest(bool test, string message, int padRight, bool endLine = true)
        {
            Printer.PrintTest(message, test, padRight, endLine);
        }

        public static void PrintTimeElapsed(DateTime @from,
            string message,
            ConsoleColor color = ConsoleColor.DarkCyan,
            bool endLine = false)
        {
            Printer.PrintTimeElapsed(message, @from, color, endLine);
        }

        public static void Print(params ColorString[] messages)
        {
            Printer.Print(messages);
        }

        public static void Print(IEnumerable<ColorString> messages)
        {
            Printer.Print(messages);
        }

        public static void Print(ColorMarkupString str, bool endLine = true)
        {
            Printer.Print(str, endLine);
        }
    }
}
