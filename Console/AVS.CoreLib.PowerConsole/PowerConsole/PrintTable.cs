using System;
using System.Collections.Generic;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.PowerConsole.ConsoleTable;
using AVS.CoreLib.PowerConsole.Printers2;
using AVS.CoreLib.PowerConsole.Printers2.Extensions;

namespace AVS.CoreLib.PowerConsole
{
    public static partial class PowerConsole
    {
        public static void PrintTable<T>(IEnumerable<T> data, PrintOptions2 options = PrintOptions2.Default, Colors? colors = null)
        {
            var builder = new TableBuilder();
            var table = builder.CreateTable(data);
            Printer2.PrintTable(table, options, colors);
        }

        public static void PrintTable<T>(IEnumerable<T> data,
            Action<Table>? configure, PrintOptions2 options = PrintOptions2.Default, Colors? colors = null)
        {
            var builder = new TableBuilder();
            var table = builder.CreateTable(data);
            configure?.Invoke(table);
            Printer2.PrintTable(table, options, colors);
        }

        public static void PrintTable(Table table, PrintOptions2 options = PrintOptions2.Default, Colors? colors = null)
        {
            Printer2.PrintTable(table, options, colors);
        }
    }
}
