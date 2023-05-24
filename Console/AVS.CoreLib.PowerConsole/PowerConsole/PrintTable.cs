using System;
using System.Collections.Generic;
using AVS.CoreLib.Guards;
using AVS.CoreLib.PowerConsole.ConsoleTable;
using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.PowerConsole.Printers;

namespace AVS.CoreLib.PowerConsole
{
    public static partial class PowerConsole
    {
        public static void PrintTable<T>(IEnumerable<T> data, PrintOptions? options = null)
        {
            var builder = new TableBuilder();
            var table = builder.CreateTable(data);
            Printer.PrintTable(table, options ?? DefaultOptions);
        }

        public static void PrintTable<T>(IEnumerable<T> data,
            Action<Table>? configure, PrintOptions? options = null)
        {
            var builder = new TableBuilder();
            var table = builder.CreateTable(data);
            configure?.Invoke(table);
            Printer.PrintTable(table, options ?? DefaultOptions);
        }

        public static void PrintTable(Table table, PrintOptions? options = null)
        {
            Guard.Against.Null(table);
            Printer.PrintTable(table, options ?? DefaultOptions);
        }
    }
}
