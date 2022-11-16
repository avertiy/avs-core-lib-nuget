using System.Text.Json;

namespace AVS.CoreLib.PowerConsole.Extensions
{
    public static class JsonExtensions
    {
        /// <summary>
        /// Converts the value of a type specified into a JSON string through <see cref="JsonSerializer"/>
        /// </summary>
        public static string ToJsonString<T>(this T obj, bool indented = false)
        {
            if (indented)
            {
                return JsonSerializer.Serialize<T>(obj, new JsonSerializerOptions() { WriteIndented = true });
            }
            return JsonSerializer.Serialize<T>(obj);
        }

        /// <summary>
        /// Converts the value of a type specified into a JSON string through <see cref="JsonSerializer"/>
        /// </summary>
        public static string ToJsonString<T>(this T obj, JsonSerializerOptions options)
        {
            return JsonSerializer.Serialize<T>(obj, options);
        }
    }
}