#nullable enable
using System;

namespace AVS.CoreLib.Abstractions.Json
{
    /// <summary>
    /// An abstraction layer to avoid implicit Newtonsoft dependency by means of 
    /// JsonConvert.SerializeObject(obj) / JsonConvert.DeserializeObject(obj) methods
    /// </summary>
    public interface IJsonSerializer
    {
        string SerializeObject(object obj, Type? type = null);
        string SerializeObject(object obj, Type? type = null, params Type[] converters);
        object? DeserializeObject(string? json, Type type);
        void Populate(object target, string json);
    }

    public static class JsonSerializerExtensions
    {
        public static T? DeserializeObject<T>(this IJsonSerializer jsonService, string json)
        {
            return (T?)jsonService.DeserializeObject(json, typeof(T));
        }
    }
}