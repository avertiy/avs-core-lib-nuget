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
            ArrayPrintOptions<T> options)
        {
            var stringifyOptions = options.StringifyOptions;
            var str = enumerable.Stringify(stringifyOptions.Format, stringifyOptions.Separator, options.Formatter);
            var text = message == null ? str : $"{message}{str}";
            printer.Print(text, options);
        }

        //public static void PrintArray<T>(this IPowerConsolePrinter printer, IEnumerable<T> enumerable,
        //    string? message,
        //    PrintOptions printOptions,
        //    StringifyOptions? options,
        //    Func<T, string>? formatter)
        //{
        //    string str;
        //    if (options == null)
        //    {
        //        str = enumerable.Stringify(StringifyFormat.Default, ",", formatter);
        //    }
        //    else
        //    {
        //        str = enumerable.Stringify(options.Format, options.Separator, formatter);
        //    }

        //    var text = message == null ? str : $"{message}{str}";
        //    printer.Print(text, printOptions);
        //}

    }
}