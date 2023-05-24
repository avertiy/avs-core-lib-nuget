using System.Text.Json;
using AVS.CoreLib.PowerConsole.ConsoleTable;

namespace AVS.CoreLib.PowerConsole.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Converts the value of a type specified into a JSON string through <see cref="JsonSerializer"/>
        /// </summary>
        public static string ToJsonString(this object obj, bool indented = false)
        {
            return indented ? JsonSerializer.Serialize(obj, new JsonSerializerOptions() { WriteIndented = true }) : JsonSerializer.Serialize(obj);
        }

        /// <summary>
        /// Converts the value of a type specified into a JSON string through <see cref="JsonSerializer"/>
        /// </summary>
        public static string ToJsonString(this object obj, JsonSerializerOptions options)
        {
            return JsonSerializer.Serialize(obj, options);
        }

        public static Table ToTable(this object obj, ColumnOptions options, params string[] excludeProperties)
        {
            var builder = new TableBuilder { ColumnOptions = options, ExcludeProperties = excludeProperties };
            return builder.CreateTable(obj);
        }
    }
}