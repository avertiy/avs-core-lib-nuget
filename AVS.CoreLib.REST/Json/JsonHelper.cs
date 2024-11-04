#nullable enable
using System;
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
            get => _serializer ??= new NewtonsoftJsonSerializer();
            set => _serializer = value;
        }

        public static string Serialize(object obj, Type? type = null, params Type[] converters)
        {
            return Serializer.SerializeObject(obj, type, converters);
        }

        public static string Serialize(object obj, Type? type = null)
        {
            return Serializer.SerializeObject(obj, type);
        }

        public static object? Deserialize(string json, Type type)
        {
            return Serializer.DeserializeObject(json, type);
        }

        public static T? Deserialize<T>(string? json)
        {
            return (T?)Serializer.DeserializeObject(json, typeof(T));
        }

        public static void Populate(object target, string json)
        {
            Serializer.Populate(target, json);
        }
    }

    public enum JsonType
    {
        Object = 0,
        Array
    }
}