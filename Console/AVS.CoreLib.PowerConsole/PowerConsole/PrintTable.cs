using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using AVS.CoreLib.Extensions;
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
            options = options ?? DefaultOptions;
            Printer.PrintTable(table, options);
        }
    }
}
