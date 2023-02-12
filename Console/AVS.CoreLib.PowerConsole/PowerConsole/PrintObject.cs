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
        public static void PrintJson<T>(T obj, JsonPrintOptions? options = null)
        {
            Printer.PrintJson<T>(obj, options ?? JsonPrintOptions.Json);
        }

        /// <summary>
        /// synonym of <see cref="PrintJson{T}"/>
        /// </summary>
        public static void Dump<T>(T obj, JsonPrintOptions? options = null)
        {
            Printer.PrintJson<T>(obj, options ?? JsonPrintOptions.JsonIndented);
        }
    }

    public class JsonPrintOptions : PrintOptions
    {
        public bool Indented { get; set; }

        public static JsonPrintOptions Json { get; set; } = new JsonPrintOptions()
        {
            ColorTags = false,
            EndLine = true,
            Color = ConsoleColor.Cyan,
        };

        public static JsonPrintOptions JsonIndented { get; set; } = new JsonPrintOptions()
        {
            ColorTags = false,
            EndLine = true,
            Color = ConsoleColor.Cyan,
            Indented = true
        };

        public static JsonPrintOptions Create(bool indented = true, ConsoleColor? color = ConsoleColor.Cyan,  bool endLine = true, bool? colorTags = false)
        {
            return new JsonPrintOptions()
            {
                ColorTags = colorTags,
                EndLine = endLine,
                Color = color,
                Indented = indented
            };
        }
        public static implicit operator JsonPrintOptions(ConsoleColor color)
        {
            return new JsonPrintOptions()
            {
                ColorTags = false,
                EndLine = true,
                Color = color,
            };
        }
    }
}
