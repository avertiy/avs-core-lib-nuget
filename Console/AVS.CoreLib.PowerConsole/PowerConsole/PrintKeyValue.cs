using System;
using System.Collections.Generic;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Extensions.Stringify;
using AVS.CoreLib.PowerConsole.Printers2;
using AVS.CoreLib.PowerConsole.Printers2.Extensions;

namespace AVS.CoreLib.PowerConsole
{
    public static partial class PowerConsole
    {
        public static void PrintKeyValue<TKey, TValue>(IDictionary<TKey, TValue> dictionary,
            PrintOptions2 options = PrintOptions2.Default,
            Colors? colors = null,
            StringifyOptions? stringifyOptions = null,
            Func<TKey, TValue, string>? formatter = null)
        {
            Printer2.PrintDictionary(dictionary, formatter, options, stringifyOptions, colors);
        }

        public static void PrintKeyValue<TKey, TValue>(IDictionary<TKey, TValue> dictionary,
            string message,
            PrintOptions2 options = PrintOptions2.Default,
            Colors? colors = null,
            StringifyOptions? stringifyOptions = null,
            Func<TKey, TValue, string>? formatter = null)
        {
            Printer2.Print(message, options | PrintOptions2.Inline);
            Printer2.PrintDictionary(dictionary, formatter, options, stringifyOptions, colors);
        }
    }
}
