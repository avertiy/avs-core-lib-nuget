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
        ///// <summary>
        ///// Print array
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="enumerable"></param>
        ///// <param name="color">printed text color [optional]</param>
        ///// <param name="message">message [optional]</param>
        ///// <param name="options">stringify options [optional], when null <see cref="StringifyOptions.Default"/> is applied</param>
        ///// <param name="formatter">formatter to format array items</param>
        ///// <param name="endLine"></param>
        ///// <param name="colorTags"></param>
        //public static void PrintArray<T>(
        //    IEnumerable<T> enumerable,
        //    ConsoleColor? color = null,
        //    string? message = null,
        //    StringifyOptions? options = null,
        //    Func<T, string>? formatter = null,
        //    bool endLine = true,
        //    bool colorTags = false)
        //{
        //    var arrayPrintOptions = new ArrayPrintOptions<T>()
        //    {
        //        StringifyOptions = options ?? StringifyOptions.Default,
        //        Formatter = formatter,
        //        EndLine = endLine,
        //        ColorTags = colorTags
        //    };

        //    Printer.PrintArray(enumerable, message, arrayPrintOptions);
        //}

        /// <summary>
        /// Print array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="message">message [optional]</param>
        /// <param name="options"><see cref="ArrayPrintOptions{T}"/></param>
        /// <param name="formatter"></param>
        public static void PrintArray<T>(
            IEnumerable<T> enumerable,
            string? message = null,
            ArrayPrintOptions<T>? options = null,
            Func<T, string>? formatter = null)
        {
            options = options ?? new ArrayPrintOptions<T>() { StringifyOptions = StringifyOptions.Default };
            options.Formatter = formatter;
            Printer.PrintArray(enumerable, message, options);
        }


    }

    public class ArrayPrintOptions<T> : PrintOptions
    {
        /// <summary>
        /// format array items
        /// </summary>
        public Func<T, string>? Formatter { get; set; }

        public StringifyOptions StringifyOptions { get; set; } = StringifyOptions.Default;

        public static implicit operator ArrayPrintOptions<T>(Func<T, string> formatter)
        {
            return new ArrayPrintOptions<T>()
            {
                Formatter = formatter, 
                StringifyOptions = StringifyOptions.Default
            };
        }

        public static implicit operator ArrayPrintOptions<T>(StringifyOptions options)
        {
            return new ArrayPrintOptions<T>()
            {
                StringifyOptions = options
            };
        }

        public static implicit operator ArrayPrintOptions<T>(ConsoleColor color)
        {
            return new ArrayPrintOptions<T>()
            {
                Color = color,
                StringifyOptions = StringifyOptions.Default
            };
        }

        public static implicit operator ArrayPrintOptions<T>(Colors colors)
        {
            return new ArrayPrintOptions<T>()
            {
                Colors = colors,
                StringifyOptions = StringifyOptions.Default
            };
        }
    }
}
