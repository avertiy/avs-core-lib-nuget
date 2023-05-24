using System.Reflection;
using AVS.CoreLib.Guards;
using AVS.CoreLib.PowerConsole.ConsoleTable;
using AVS.CoreLib.PowerConsole.Extensions;
using AVS.CoreLib.PowerConsole.Printers;

namespace AVS.CoreLib.PowerConsole
{
    public static partial class PowerConsole
    {
        public static void PrintObject(string message, object obj, ObjectFormat format = ObjectFormat.Default, PrintOptions? options = null)
        {
            Guard.Against.Null(obj);
            Printer.PrintObject(obj, message, format, options ?? DefaultOptions);
        }

        public static void PrintJson(object obj, bool indented = false, PrintOptions? options = null)
        {
            Guard.Against.Null(obj);
            Printer.PrintJson(obj, indented, options ?? DefaultOptions);
        }

        public static void PrintTable(object obj, ColumnOptions options = ColumnOptions.Auto, params string[] excludeProps)
        {
            Guard.Against.Null(obj);
            var tbl = obj.ToTable(options, excludeProps);
            Printer.PrintTable(tbl, PrintOptions.NoTimestamp);
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
