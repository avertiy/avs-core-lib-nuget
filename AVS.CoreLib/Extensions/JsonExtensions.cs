using System;
using System.Text.Json;

namespace AVS.CoreLib.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJson(this object obj)
        {
            var json = JsonSerializer.Serialize(obj);
            return json;
        }
        public static string ToJson<T>(this T value)
        {
            var json = JsonSerializer.Serialize(value);
            return json;
        }

        public static string ToJsonSafe(this object obj)
        {
            try
            {
                return JsonSerializer.Serialize(obj);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static T Deserialize<T>(this string json) where T : new()
        {
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}