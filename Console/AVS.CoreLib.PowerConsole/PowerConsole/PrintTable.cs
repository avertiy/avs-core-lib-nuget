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
            var table = Table.Create(data);
            PrintTable(table, options);
        }

        public static void PrintTable<T>(IEnumerable<T> data,
            Action<Table>? configure, PrintOptions? options = null)
        {
            var table = Table.Create(data);
            configure?.Invoke(table);
            PrintTable(table, options);
        }

        public static void PrintTable(Table table, PrintOptions? options = null)
        {
            Guard.Against.Null(table);
            var op = options ?? PrintOptions.NoTimestamp;
            Printer.PrintTable(table, op);
        }
    }
}
