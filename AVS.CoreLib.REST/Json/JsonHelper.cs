#nullable enable
using System;
using AVS.CoreLib.Abstractions.Json;
using AVS.CoreLib.REST.Json.Newtonsoft;

namespace AVS.CoreLib.REST.Json
{
    /// <summary>
    /// JsonHelper cleared from static dependency on Newtonsoft, 
    /// instead it realies on an abstraction layer (<see cref="IJsonSerializer"/>)
    /// </summary>
    public static class JsonHelper
    {
        public static IJsonSerializer Serializer { get; set; } = new NewtonsoftJsonSerializer();

        public static string SerializeObject(object obj, Type? type = null, params Type[] converters)
        {
            return Serializer.SerializeObject(obj, type, converters);
        }

        public static string SerializeObject(object obj, Type? type = null)
        {
            return Serializer.SerializeObject(obj, type);
        }

        public static object? DeserializeObject(string json, Type type)
        {
            return Serializer.DeserializeObject(json, type);
        }

        public static T? DeserializeObject<T>(string? json)
        {
            return (T?)Serializer.DeserializeObject(json, typeof(T));
        }
    }
}