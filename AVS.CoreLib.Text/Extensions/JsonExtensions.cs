/*using System.Text.Json;

namespace AVS.CoreLib.Text.Extensions
{
    /// <summary>
    /// Json extensions
    /// </summary>
    public static class JsonExtensions
    {
        /// <summary>
        /// Converts the value of a type specified into a JSON string through <see cref="JsonSerializer"/>
        /// </summary>
        public static string ToJsonString<T>(this T obj, bool indented = false)
        {
            JsonSerializerOptions options = null;
            if (indented)
            {
                options = new JsonSerializerOptions { WriteIndented = true };
            }
            return JsonSerializer.Serialize<T>(obj, options);
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
*/