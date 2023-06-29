using System;
using System.Reflection;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Guards;
using AVS.CoreLib.PowerConsole.ConsoleTable;
using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.PowerConsole.Printers2;
using AVS.CoreLib.PowerConsole.Printers2.Extensions;

namespace AVS.CoreLib.PowerConsole
{
    public static partial class PowerConsole
    {
        public static void PrintObject(object? obj, ObjectFormat format = ObjectFormat.Default, PrintOptions2 options = PrintOptions2.Default, Colors? colors = null)
        {
            Printer2.PrintObject(obj, string.Empty, format, options, colors);
        }

        public static void PrintObject(string message, object? obj, ObjectFormat format = ObjectFormat.Default, PrintOptions2 options = PrintOptions2.Default, Colors? colors = null)
        {
            Printer2.PrintObject(obj, message, format, options, colors);
        }

        public static void PrintJson(object obj, bool indented = false, PrintOptions2 options = PrintOptions2.Default, Colors? colors = null)
        {
            Printer2.PrintJson(obj, indented, options, colors);
        }

        public static void PrintTable(object? obj, TableOrientation options = TableOrientation.Auto,
            Colors? colors = null,
            params string[] excludeProps)
        {
            if (obj == null)
            {
                Printer2.Print("null");
                return;
            }

            var tbl = obj.ToTable(options, excludeProps);
            Printer2.PrintTable(tbl, PrintOptions2.NoTimestamp, colors);
        }
    }

    public enum ObjectFormat
    {
        /// <summary>
        /// by default object will be printed as json
        /// </summary>
        Default = 0,
        /// <summary>
        /// Use obj.ToJsonString() (<see cref="System.Text.Json.JsonSerializer"/>)
        /// </summary>
        Json = 1,
        /// <summary>
        /// Use obj.ToJsonString() (<see cref="System.Text.Json.JsonSerializer"/>) 
        /// </summary>
        JsonIndented = 2,
        /// <summary>
        /// Use obj.ToTableString() (<see cref="AVS.CoreLib.PowerConsole.ConsoleTable.Table"/>) 
        /// </summary>
        Table = 3,
        /// <summary>
        /// Use obj.ToTableString() (<see cref="AVS.CoreLib.PowerConsole.ConsoleTable.Table"/>)
        /// Horizontal instructs <see cref="TableBuilder"/> to build table into width 
        /// </summary>
        TableHorizontal = 4,
        /// <summary>
        /// Use obj.ToTableString() (<see cref="AVS.CoreLib.PowerConsole.ConsoleTable.Table"/>)
        /// Vertical instructs <see cref="TableBuilder"/> to build table in two 2 columns: Property Name, Property Value
        /// </summary>
        TableVertical = 5,
        ToString = 6,
    }
}
