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
    public static class PrintArrayExtensions
    {
        public static void PrintArray<T>(this IPowerConsolePrinter printer, IEnumerable<T> enumerable,
            string? message,
            StringifyOptions? options,
            Func<T, string>? formatter,
            ConsoleColor? color = null,
            bool endLine = true,
            bool colorTags = false)
        {
            string str;
            if (options == null)
            {
                str = enumerable.Stringify(StringifyFormat.Default, ",", formatter);
            }
            else
            {
                str = enumerable.Stringify(options.Format, options.Separator, formatter);
            }

            var text = message == null ? str : $"{message}{str}";
            printer.Print(text, endLine, colorTags, color);
        }

        public static void PrintArray<T>(this IPowerConsolePrinter printer, IEnumerable<T> enumerable,
            string? message,
            StringifyOptions? options,
            Func<T, string>? formatter,
            Colors colors,
            bool endLine = true,
            bool colorTags = false)
        {
            string str;
            if (options == null)
            {
                str = enumerable.Stringify(StringifyFormat.Default, ",", formatter);
            }
            else
            {
                str = enumerable.Stringify(options.Format, options.Separator, formatter);
                str = colors.Colorize(str);
            }

            var text = message == null ? str : $"{message}{str}";
            printer.Print(text, endLine, colorTags, colors);
        }

    }
}