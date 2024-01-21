#nullable enable
using System;

namespace AVS.CoreLib.Abstractions.Json
{
    /// <summary>
    /// An absraction layer to avoid implicit Newtonsoft dependency by means of 
    /// JsonConvert.SerializeObject(obj) / JsonConvert.DeserializeObject(obj) methods
    /// </summary>
    public interface IJsonService
    {
        string SerializeObject(object obj, Type? type = null);
        string SerializeObject(object obj, Type? type = null, params Type[] converters);
        object? DeserializeObject(string? json, Type type);
        void Populate(object target, string json);
    }

    public static class JsonServiceExtensions
    {
        public static T? DeserializeObject<T>(this IJsonService jsonService, string json)
        {
            return (T?)jsonService.DeserializeObject(json, typeof(T));
        }
    }
}