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
        /// <summary>
        /// Print array
        /// </summary>
        public static void PrintArray<T>(
            string message,
            IEnumerable<T> enumerable,
            Func<T, string>? formatter = null,
            StringifyOptions? stringifyOptions = null,
            PrintOptions2 options = PrintOptions2.Default,
            Colors? colors = null)
        {
            Printer2.Print(message, options | PrintOptions2.Inline);
            Printer2.PrintArray(enumerable, options, stringifyOptions, colors);
        }

        /// <summary>
        /// Print array
        /// </summary>
        public static void PrintArray<T>(
            IEnumerable<T> enumerable,
            Func<T, string>? formatter = null,
            StringifyOptions? stringifyOptions = null,
            PrintOptions2 options = PrintOptions2.Default,
            Colors? colors = null)
        {
            Printer2.PrintArray(enumerable, options, stringifyOptions, colors);
        }
    }

    //[Obsolete]
    //public class ArrayPrintOptions<T> : PrintOptions
    //{
    //    /// <summary>
    //    /// format array items
    //    /// </summary>
    //    public Func<T, string>? Formatter { get; set; }

    //    public StringifyOptions StringifyOptions { get; set; } = StringifyOptions.Default;

    //    public static implicit operator ArrayPrintOptions<T>(Func<T, string> formatter)
    //    {
    //        return new ArrayPrintOptions<T>()
    //        {
    //            Formatter = formatter, 
    //            StringifyOptions = StringifyOptions.Default
    //        };
    //    }

    //    public static implicit operator ArrayPrintOptions<T>(StringifyOptions options)
    //    {
    //        return new ArrayPrintOptions<T>()
    //        {
    //            StringifyOptions = options
    //        };
    //    }

    //    public static implicit operator ArrayPrintOptions<T>(ConsoleColor color)
    //    {
    //        return new ArrayPrintOptions<T>()
    //        {
    //            Color = color,
    //            StringifyOptions = StringifyOptions.Default
    //        };
    //    }

    //    public static implicit operator ArrayPrintOptions<T>(Colors colors)
    //    {
    //        return new ArrayPrintOptions<T>()
    //        {
    //            Colors = colors,
    //            StringifyOptions = StringifyOptions.Default
    //        };
    //    }
    //}
}
