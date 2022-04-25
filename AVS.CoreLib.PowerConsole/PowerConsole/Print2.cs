using System;
using System.Collections.Generic;
using System.Globalization;
using AVS.CoreLib.PowerConsole.ConsoleTable;
using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.PowerConsole.Printers;
using AVS.CoreLib.PowerConsole.Structs;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.Text.Extensions;
using AVS.CoreLib.Text.Formatters.ColorMarkup;

namespace AVS.CoreLib.PowerConsole
{

    /// <summary>
    /// PowerConsole represents simple extensions over standard .NET Console functionality
    /// If you need more rich and extensive console frameworks check out links below  
    /// </summary>
    /// <seealso>https://github.com/Athari/CsConsoleFormat - advanced formatting of console output for .NET</seealso>
    /// <seealso>https://github.com/migueldeicaza/gui.cs - Terminal GUI toolkit for .NET</seealso>
    /// <seealso>http://elw00d.github.io/consoleframework/- cross-platform toolkit that allows to develop TUI applications using C# and based on WPF-like concepts</seealso>
    public static partial class PowerConsole
    {
        /// <summary>
        /// Format delegate is used to convert <see cref="FormattableString"/> to string used by <see cref="Print(System.FormattableString,bool)"/>
        /// </summary>
        internal static Func<FormattableString, string> Format { get; set; } = str => str.ToString(CultureInfo.CurrentCulture);
        public static IPrinter DefaultPrinter { get; set; } = new Printer();
        
        /// <summary>
        /// Format string by <see cref="Format"/> delegate and print it
        /// </summary>
        public static void Print(FormattableString str, bool endLine = true)
        {
            var formattedString = Format(str);
            Print(formattedString, endLine);
        }

        public static void Print<T>(string message, IEnumerable<T> arr, ConsoleColor color = ConsoleColor.Gray)
        {
            Print($"{message} {arr.ToArrayString()}", color);
        }

        public static void PrintAllColors()
        {
            var values = Enum.GetNames(typeof(ConsoleColor));
            foreach (var colorName in values)
            {
                var color = Enum.Parse<ConsoleColor>(colorName);
                Print(colorName, color);
            }
        }

        public static void PrintArray<T>(IEnumerable<T> enumerable, Func<T, string> formatter = null, bool endLine = true)
        {
            Write(enumerable.ToArrayString(formatter));
            if (endLine && NewLineFlag == false)
            {
                Console.WriteLine();
                NewLineFlag = true;
            }
        }

        public static void PrintKeyValue<TKey,TValue>(IDictionary<TKey, TValue> dictionary, ConsoleColor color = ConsoleColor.White, string keyValueSeparator = " => ", string separator = "\r\n", bool endLine = true)
        {
            Write(dictionary.ToKeyValueString(keyValueSeparator, separator), color);
            WriteEndLine(endLine);
        }

        public static void PrintJson<T>(T obj, bool indented = false, ConsoleColor color = ConsoleColor.White, bool endLine = true)
        {
            Write(obj.ToJsonString(indented), color);
            WriteEndLine(endLine);
        }

        public static void PrintHeader(string header, ConsoleColor color = ConsoleColor.Cyan, string template = "============", string lineIndentation = "\r\n\r\n")
        {
            WriteLine();
            var str = $"{template} {header} {template}{lineIndentation}";
            Print(str, color, false);
        }

        public static void PrintTable<T>(IEnumerable<T> data, ConsoleColor color = ConsoleColor.White, bool endLine = true)
        {
            Print(Table.Create(data).ToString(), color, endLine);
        }

        public static void PrintTable<T>(IEnumerable<T> data, Action<Table> configure = null, bool endLine = true)
        {
            var table = Table.Create(data);
            var colorFormattedString = table.ToColorFormattedString();
            Print(colorFormattedString, endLine);
        }

        public static void PrintTable(Table table, bool endLine = true)
        {
            var colorFormattedString = table.ToColorFormattedString();
            Print(colorFormattedString, endLine);
        }

        public static void PrintTest(string message, bool test, int padRight, bool endLine = true)
        {
            Write(message.PadRight(padRight));
            if (test)
            {
                Print("OK", ConsoleColor.Green, endLine);
            }
            else
            {
                Print("Fail", ConsoleColor.DarkRed, endLine);
            }
        }

        public static void PrintTimeElapsed(DateTime @from, string comment)
        {
            var ms = (DateTime.Now - @from).TotalMilliseconds;
            if (ms < 0.5)
                return;

            Print($"{comment} => elapsed:{ms:N3} ms", ConsoleColor.Green);
        }

        public static void Print(params ColorString[] messages)
        {
            foreach (var coloredText in messages)
                Write(coloredText.Text, coloredText.Color);
        }

        public static void Print(IEnumerable<ColorString> messages)
        {
            foreach (var coloredText in messages)
                Write(coloredText.Text, coloredText.Color);
        }

        public static void Print(FormattableString str, ColorPalette palette, bool endLine = true)
        {
            DefaultPrinter.Print(str, palette, endLine);
        }

        public static void Print(FormattableString str, ConsoleColor[] colors, bool endLine = true)
        {
            DefaultPrinter.Print(str, colors, endLine);
        }

        public static void Print(ColorMarkupString str, bool endLine = true)
        {
            var currentScheme = ColorScheme.GetCurrentScheme();

            foreach ((string plainText, string scheme, string coloredText) tuple in str)
            {
                // write plain text
                if (!string.IsNullOrEmpty(tuple.plainText))
                {
                    ColorScheme.ApplyScheme(currentScheme);
                    Write(tuple.Item1);
                }

                // if scheme valid apply it
                if (ColorHelper.TryParse(tuple.scheme, out var scheme))
                {
                    ColorScheme.ApplyScheme(scheme);
                }

                if (!string.IsNullOrEmpty(tuple.coloredText))
                {
                    Write(tuple.coloredText);
                }
            }

            if (endLine)
                WriteLine();
            ColorScheme.ApplyScheme(currentScheme);
        }
    }
}
