using System;
using System.Collections.Generic;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Extensions.Stringify;
using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.PowerConsole.Printers;
using AVS.CoreLib.PowerConsole.Utilities;

namespace AVS.CoreLib.PowerConsole
{
    public static partial class PowerConsole
    {
        public static void PrintKeyValue<TKey, TValue>(IDictionary<TKey, TValue> dictionary,
            string? message = null,
            KeyValuePrintOptions<TKey,TValue>? options = null,
            Func<TKey, TValue, string>? formatter = null)
        {
            var opt = options ?? new KeyValuePrintOptions<TKey, TValue>();
            opt.Formatter = formatter;
            Printer.PrintDictionary(dictionary, message, opt);
        }
    }

    public class KeyValuePrintOptions<TKey, TValue> : PrintOptions
    {
        public StringifyOptions StringifyOptions { get; set; } = StringifyOptions.Default;
        public Func<TKey, TValue, string>? Formatter { get; set; }

        public static implicit operator KeyValuePrintOptions<TKey, TValue>(ConsoleColor color)
        {
            return new KeyValuePrintOptions<TKey, TValue>() { Color = color };
        }

        //public static implicit operator KeyValuePrintOptions<TKey, TValue>(PrintOptions options)
        //{
        //    return new KeyValuePrintOptions<TKey, TValue>()
        //    {
        //        Scheme = options.Scheme,
        //        EndLine = options.EndLine,
        //        ColorTags = options.ColorTags,
        //        CTag = options.CTag,
        //        Colors = options.Colors,
        //        Level = options.Level,
        //        TimeFormat = options.TimeFormat,
        //        VoidEmptyLines = options.VoidEmptyLines,
        //        Color = options.Color
        //    };
        //}

        public static implicit operator KeyValuePrintOptions<TKey, TValue>(ColorScheme scheme)
        {
            return new KeyValuePrintOptions<TKey, TValue>() {Scheme = scheme};
        }

        public static implicit operator KeyValuePrintOptions<TKey, TValue>(Colors colors)
        {
            return new KeyValuePrintOptions<TKey, TValue>() {Colors = colors };
        }
    }
}
