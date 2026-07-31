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
        /// <summary>
        /// 
        /// </summary>
        T? Deserialize<T>(string? json);
    }
}