#nullable enable
using System;
using System.Text.Json;
using AVS.CoreLib.Dates;
using System.Text.Json.Serialization;

namespace AVS.CoreLib.Json
{
    /// <summary>
    /// <see cref="System.Text.Json.JsonSerializer"/> helper 
    /// </summary>
    public static class JSON
    {
        public static JsonSerializerOptions? DefaultOptions { get; set; }

        /// <summary>
        /// Setup <see cref="DefaultOptions"/> 
        /// (i) I prefer to have by default enum string values converter, PropertyNameCaseInsensitive and AllowTrailingCommas
        /// (ii) add core lib converters such as <see cref="DateRangeJsonConverter"/>
        /// </summary>
        public static void Setup(Action<JsonSerializerOptions>? setup = null)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, AllowTrailingCommas = true };
            options.Converters.Add(new JsonStringEnumConverter());
            options.Converters.Add(new DateRangeJsonConverter());

            setup?.Invoke(options);
            DefaultOptions = options;
        }

        public static string ToJson(this object obj, JsonSerializerOptions? options = null)
        {
            var json = JsonSerializer.Serialize(obj, options ?? DefaultOptions);
            return json;
        }

        public static string ToJsonSafe(this object obj, JsonSerializerOptions? options = null)
        {
            try
            {
                return JsonSerializer.Serialize(obj, options ?? DefaultOptions);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static T? Deserialize<T>(this string json, JsonSerializerOptions? options = null) where T : new()
        {
            return JsonSerializer.Deserialize<T>(json, options ?? DefaultOptions);
        }
    }
}