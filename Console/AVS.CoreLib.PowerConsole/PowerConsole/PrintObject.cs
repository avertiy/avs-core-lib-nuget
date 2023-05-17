using System;
using AVS.CoreLib.Guards;
using AVS.CoreLib.PowerConsole.ConsoleTable;
using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.PowerConsole.Printers;

namespace AVS.CoreLib.PowerConsole
{
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

        /// <summary>
        /// print object as table 
        /// </summary>
        public static void PrintObject<T>(T obj, ColumnOptions options = ColumnOptions.Auto, params string[] excludeProps)
        {
            Guard.Against.Null(obj);
            var builder = new TableBuilder { ColumnOptions = options, ExcludeProperties = excludeProps }; ;
            var tbl = builder.CreateTable(obj);
            Printer.PrintTable(tbl, PrintOptions.NoTimestamp);
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
