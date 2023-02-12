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
    public static class PrintDictionaryExtensions
    {
        public static void PrintDictionary<TKey, TValue>(this IPowerConsolePrinter printer,
            IDictionary<TKey, TValue> dictionary,
            string? message,
            StringifyOptions? options,
            Func<TKey, TValue, string>? formatter,
            ConsoleColor? color = null,
            bool endLine = true,
            bool colorTags = false)
        {
            string str;
            if (options == null)
            {
                str = dictionary.Stringify(StringifyOptions.Default, formatter);
            }
            else
            {
                str = dictionary.Stringify(options, formatter);
            }

            var text = message == null ? str : $"{message}{str}";
            var printOptions = PrintOptions.Default.Clone();
            printOptions.EndLine = endLine;
            printOptions.ColorTags = colorTags;
            printOptions.Color = color;
            printer.Print(text, printOptions);
        }

        public static void PrintDictionary<TKey, TValue>(this IPowerConsolePrinter printer,
            IDictionary<TKey, TValue> dictionary,
            string? message,
            KeyValuePrintOptions<TKey,TValue> options)
        {
            var str = dictionary.Stringify(options.StringifyOptions, options.Formatter);
            var text = message == null ? str : $"{message}{str}";
            printer.Print(text, options);
        }

    }
}