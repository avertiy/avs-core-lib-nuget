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
        /// <summary>
        /// Print array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="color">printed text color [optional]</param>
        /// <param name="message">message [optional]</param>
        /// <param name="options">stringify options [optional], when null <see cref="StringifyOptions.Default"/> is applied</param>
        /// <param name="formatter">formatter to format array items</param>
        /// <param name="endLine"></param>
        /// <param name="colorTags"></param>
        public static void PrintArray<T>(
            IEnumerable<T> enumerable,
            ConsoleColor? color = null,
            string? message = null,
            StringifyOptions? options = null,
            Func<T, string>? formatter = null,
            bool endLine = true,
            bool colorTags = false)
        {
            Printer.PrintArray(enumerable, message, options, formatter, color, endLine, colorTags);
        }
        /// <summary>
        /// Print array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="colors">printed text colors</param>
        /// <param name="message">message [optional]</param>
        /// <param name="options">stringify options [optional], when null <see cref="StringifyOptions.Default"/> is applied</param>
        /// <param name="formatter">formatter to format array items</param>
        /// <param name="endLine"></param>
        /// <param name="colorTags"></param>
        public static void PrintArray<T>(
            IEnumerable<T> enumerable,
            Colors colors,
            string? message = null,
            StringifyOptions? options = null,
            Func<T, string>? formatter = null,
            bool endLine = true,
            bool colorTags = false)
        {
            Printer.PrintArray(enumerable, message, options, formatter, colors, endLine, colorTags);
        }
    }
}
