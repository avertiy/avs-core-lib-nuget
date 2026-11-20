#nullable enable
using System;
using System.Reflection.Metadata;
using AVS.CoreLib.Abstractions.Json;
using AVS.CoreLib.REST.Json.Newtonsoft;

namespace AVS.CoreLib.REST.Json
{
    /// <summary>
    /// JsonHelper represent an abstraction layer to decouple from direct dependency on Newtonsoft/System.Text.Json 
    /// (<see cref="IJsonSerializer"/>)
    /// </summary>
    public static class JsonHelper
    {
        private static IJsonSerializer? _serializer;

        public static IJsonSerializer Serializer
        {
            //get => _serializer ??= new NewtonsoftJsonSerializer();
            get => _serializer ??= new SystemTextJsonSerializer();
            set => _serializer = value;
        }

        public static string Serialize(object obj, Type? type = null)
        {
            return Serializer.SerializeObject(obj, type);
        }

        /// <summary>
        /// Parses json text into <typeparamref name="T"/> value
        /// </summary>
        public static T? Deserialize<T>(string? json)
        {
            return Serializer.Deserialize<T>(json);
        }

        /// <summary>
        /// Parses json text into <typeparamref name="T"/>[] array
        /// </summary>
        public static T[]? DeserializeArray<T>(string? json)
        {
            if (json == null)
                return null;

            if (json.StartsWith('{'))
            {
                var obj = JsonHelper.Deserialize<T>(json);

                if (obj == null)
                    return null;

                return [obj];
            }

            return JsonHelper.Deserialize<T[]>(json);
        }
    }
}