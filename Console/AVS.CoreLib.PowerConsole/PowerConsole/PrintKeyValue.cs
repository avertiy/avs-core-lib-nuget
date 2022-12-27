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
        public static void PrintKeyValue<TKey, TValue>(IDictionary<TKey, TValue> dictionary,
            ConsoleColor? color = null,
            string? message = null,
            StringifyOptions? options = null,
            Func<TKey, TValue, string>? formatter = null,
            bool endLine = true,
            bool colorTags = false)
        {
            Printer.PrintDictionary(dictionary, message, options, formatter, color, endLine, colorTags);
        }

        public static void PrintKeyValue<TKey, TValue>(IDictionary<TKey, TValue> dictionary,
            Colors colors,
            string? message = null,
            StringifyOptions? options = null,
            Func<TKey, TValue, string>? formatter = null,
            bool endLine = true,
            bool colorTags = false)
        {
            Printer.PrintDictionary(dictionary, message, options, formatter, colors, endLine, colorTags);
        }
    }
}
