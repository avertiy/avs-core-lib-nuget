using System;
using System.Collections.Generic;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Extensions.Stringify;
using AVS.CoreLib.Guards;
using AVS.CoreLib.PowerConsole.ConsoleTable;
using AVS.CoreLib.PowerConsole.Enums;
using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.PowerConsole.Structs;
using AVS.CoreLib.PowerConsole.Utilities;
using AVS.CoreLib.Text.Formatters.ColorMarkup;

namespace AVS.CoreLib.PowerConsole.Printers2.Extensions
{
    public static class Printer2Extensions
    {
        //for now i just implement same functionality as it was in printer1
        //then will think how to rework that with better &simpler interface
        #region Print Collections (Array, Dictionary)
        public static void PrintArray<T>(this IPowerConsolePrinter2 printer, IEnumerable<T> source,
            PrintOptions2 options = PrintOptions2.Default,
            StringifyOptions? stringifyOptions = null,
            Colors? colors = null)
        {
            printer.Print("", options, colors);
            var strOptions = stringifyOptions ?? StringifyOptions.Default;
            var str = source.Stringify(strOptions.Format, strOptions.Separator, null);
            printer.Print(str, options, colors);
        }

        public static void PrintDictionary<TKey, TValue>(this IPowerConsolePrinter2 printer,
            IDictionary<TKey, TValue> dictionary,
            Func<TKey, TValue, string>? formatter = null,
            PrintOptions2 options = PrintOptions2.Default,
            StringifyOptions? stringifyOptions = null,
            Colors? colors = null)
        {
            var str = dictionary.Stringify(stringifyOptions ?? StringifyOptions.Default, formatter);
            printer.Print(str, options, colors);
        }

        public static void PrintDictionary<TKey, TValue>(this IPowerConsolePrinter2 printer,
            string message,
            IDictionary<TKey, TValue> dictionary,
            Func<TKey, TValue, string> formatter,
            PrintOptions2 options = PrintOptions2.Default,
            StringifyOptions? stringifyOptions = null,
            Colors? colors = null)
        {
            var str = dictionary.Stringify(stringifyOptions ?? StringifyOptions.Default, formatter);
            var text = $"{message}{str}";
            printer.Print(text, options, colors);
        }

        public static void PrintDictionary<TKey, TValue>(this IPowerConsolePrinter2 printer,
            string message,
            IDictionary<TKey, TValue> dictionary,
            PrintOptions2 options = PrintOptions2.Default,
            StringifyOptions? stringifyOptions = null,
            Colors? colors = null)
        {
            var strOptions = stringifyOptions ?? StringifyOptions.Default;
            var str = dictionary.Stringify(strOptions);
            var text = $"{message}{str}";
            printer.Print(text, options, colors);
        }

        #endregion

        //public static void PrintObject(this IPowerConsolePrinter2 printer, object obj, ObjectFormat format, PrintOptions2 options, Colors? colors = null)
        //{
        //    switch (format)
        //    {
        //        case ObjectFormat.ToString:
        //            printer.Print(obj.ToString(), options, colors);
        //            break;
        //        case ObjectFormat.Json:
        //            printer.Print(obj.ToJsonString(), options, colors);
        //            break;
        //        case ObjectFormat.JsonIndented:
        //            printer.Print(obj.ToJsonString(true), options, colors);
        //            break;
        //        case ObjectFormat.Table:
        //        case ObjectFormat.TableVertical:
        //        case ObjectFormat.TableHorizontal:
        //            var table = obj.ToTable(format.GetColumnOptions()).WithTitle(message);
        //            printer.PrintTable(table, options);
        //            break;

        //        default:
        //            printer.Print(message, options);
        //            printer.Print(obj.ToJsonString(), options);
        //            break;
        //    }
        //}

        public static void PrintJson(this IPowerConsolePrinter2 printer, object obj, bool indented,
            PrintOptions2 options = PrintOptions2.Default, Colors? colors = null)
        {
            var str = obj.ToJsonString(indented);
            printer.Print(str, options, colors);
        }

        public static void PrintTable(this IPowerConsolePrinter2 printer, Table table, PrintOptions2 options, Colors? colors = null)
        {
            Guard.Against.Null(table);
            var str = table.ToString();
            printer.Print(str, options, colors);
        }

        public static void PrintHeader(this IPowerConsolePrinter2 printer, string message, string template, string indentation, Colors? colors = null)
        {
            var str = $"{indentation}{template} {message} {template}{indentation}";
            printer.Print(str, PrintOptions2.NoTimestamp, colors ?? ConsoleColor.Cyan);
        }

        public static void PrintTest(this IPowerConsolePrinter2 printer, string message, bool test, int padRight,
            PrintOptions2 options)
        {
            var str = message.PadRight(padRight) + (test ? "OK" : "Fail");
            var color = test ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed;
            printer.Print(str, options, color);
        }

        public static void PrintTimeElapsed(this IPowerConsolePrinter2 printer, string? message, DateTime from, PrintOptions2 options, Colors? colors = null)
        {
            var ms = (DateTime.Now - @from).TotalMilliseconds;
            if (ms < 0.5)
                return;

            var text = message == null ? $"[elapsed:{ms:N1} ms]" : $"{message}   [elapsed:{ms:N1} ms]";
            printer.Print(text, options, colors ?? ConsoleColor.DarkGray);
        }

        public static void PrintConsoleColors(this IPowerConsolePrinter2 printer)
        {
            var values = Enum.GetNames(typeof(ConsoleColor));
            foreach (var colorName in values)
            {
                var color = Enum.Parse<ConsoleColor>(colorName);
                printer.Print(colorName, PrintOptions2.Default, color);
            }
        }

        public static void PrintError(this IPowerConsolePrinter2 printer,
            Exception ex,
            string? message,
            bool printStackTrace,
            PrintOptions2 options = PrintOptions2.Default)
        {
            var type = ex.GetType().Name;
            var str = message == null ? $"[{type}:{ex.Message}]" : $"{message} [{type}:{ex.Message}]";
            printer.Print(MessageLevel.Error, str, options);

            if (printStackTrace)
                printer.Print(ex.StackTrace, PrintOptions2.NoTimestamp, ConsoleColor.DarkGray);
        }


        public static void Print(this IPowerConsolePrinter2 printer, ColorMarkupString str, PrintOptions2 options = PrintOptions2.Default)
        {
            var currentScheme = ColorScheme.GetCurrentScheme();
            var plainTextColors = new Colors(currentScheme.Foreground, currentScheme.Background);

            foreach (var (plainText, colorScheme, coloredText) in str)
            {
                // write plain text
                if (!string.IsNullOrEmpty(plainText))
                    printer.Write(plainText, plainTextColors);

                // if scheme valid apply it
                if (!string.IsNullOrEmpty(coloredText) && ColorSchemeHelper.TryParse(colorScheme, out var scheme))
                    printer.Write(coloredText, new Colors(scheme.Foreground, scheme.Background));
            }

            if (!options.HasFlag(PrintOptions2.Inline))
                printer.WriteLine();
        }

        public static void Print(this IPowerConsolePrinter2 printer, ColorMarkupString str, ConsoleColor? color, bool endLine)
        {
            var colors = new Colors(color, null);

            foreach (var (plainText, colorScheme, coloredText) in str)
            {
                // write plain text
                if (!string.IsNullOrEmpty(plainText))
                    printer.Write(plainText, colors);

                // if scheme valid apply it
                if (!string.IsNullOrEmpty(coloredText) && ColorSchemeHelper.TryParse(colorScheme, out var scheme))
                    printer.Write(coloredText, new Colors(scheme.Foreground, scheme.Background));
            }

            if (endLine)
                printer.WriteLine();
        }

        public static void PrintMessages(this IPowerConsolePrinter2 printer, IEnumerable<ColorString> messages)
        {
            foreach (var coloredText in messages)
                printer.WriteLine(coloredText.Text, new Colors(coloredText.Color.Foreground, coloredText.Color.Background));
        }

        public static void PrintObject(this IPowerConsolePrinter2 printer, object? obj, string message,
            ObjectFormat format,
            PrintOptions2 options = PrintOptions2.Default,
            Colors? colors = null)
        {
            if (obj == null)
            {
                printer.Print(message, options, colors);
                printer.Print("null", options, colors);
                return;
            }

            switch (format)
            {
                case ObjectFormat.ToString:
                    printer.Print(message, options, colors);
                    printer.Print(obj.ToString(), options, colors);
                    break;
                case ObjectFormat.Json:
                    printer.Print(message, options, colors);
                    printer.Print(obj.ToJsonString(), options, colors);
                    break;
                case ObjectFormat.JsonIndented:
                    printer.Print(message, options, colors);
                    printer.Print(obj.ToJsonString(true), options, colors);
                    break;
                case ObjectFormat.Table:
                case ObjectFormat.TableVertical:
                case ObjectFormat.TableHorizontal:
                    var table = obj.ToTable(format.GetColumnOptions()).WithTitle(message);
                    printer.PrintTable(table, options, colors);
                    break;

                default:
                    printer.Print(message, options, colors);
                    printer.Print(obj.ToJsonString(), options, colors);
                    break;
            }
        }

        //public static void PrintConsoleColors(this IPowerConsolePrinter2 printer)
        //{
        //    var values = Enum.GetNames(typeof(ConsoleColor));
        //    foreach (var colorName in values)
        //    {
        //        var options = PrintOptions.Default.Clone();
        //        options.Color = Enum.Parse<ConsoleColor>(colorName);
        //        options.EndLine = true;
        //        options.ColorTags = false;

        //        printer.Print(colorName, options);
        //    }
        //}
    }
}