#nullable enable
using System;
using AVS.CoreLib.Abstractions.Json;
using AVS.CoreLib.Json;

namespace AVS.CoreLib.REST.Json
{
    /// <summary>
    /// Wrapper for <see cref="System.Text.Json.JsonSerializer"/> 
    /// </summary>
    public class SystemTextJsonSerializer : IJsonSerializer
    {
        /// <summary>
        /// Parses the text representing a single JSON value into a <typeparamref name="T"/> value.
        /// </summary>
        public T? Deserialize<T>(string? json)
        {
            if (json == null)
                return default;

            return JSON.Deserialize<T>(json);
        }

        /// <summary>
        /// Serializes the provided value into JSON string
        /// </summary>
        public string SerializeObject(object obj, Type? type = null)
        {
            return JSON.ToJson(obj, type ?? obj.GetType());
        }
    }

}