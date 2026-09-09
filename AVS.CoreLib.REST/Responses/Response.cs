#nullable enable
using System;
using System.Diagnostics;
using System.Text.Json.Serialization;
using AVS.CoreLib.Abstractions.Responses;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Dynamic;
using AVS.CoreLib.Json;

namespace AVS.CoreLib.REST.Responses
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public sealed class Response<T> : IResponse<T>
    {
        /// <summary>
        /// source of the response
        /// </summary>
        public required string Source { get; set; }

        /// <summary>
        /// data
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// error when response failed
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Error { get; set; }

        /// <summary>
        /// request payload
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public object? Request { get; set; }

        /// <summary>
        /// raw content
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string? RawContent { get; set; }

        /// <summary>
        /// indicates whether the response is succesfull or not
        /// </summary>
        public bool Success => Error == null;

        /// <summary>
        /// 
        /// </summary>
        public override string ToString()
        {
            var typeName = typeof(T).Name;

            if (Error != null)
                return $"Response<{typeName}> - Failed ({Error})";

            if (Data == null)
                return $"Response<{typeName}> - OK (NO DATA)";

            if (Data.TryGetCount(out var count))
                return $"Response<{typeName}> - OK (#{count})";

            var content = RawContent == null 
                ? Data.ToBriefJson().Truncate(180, TruncateOptions.CutOffTheMiddle)
                : RawContent.Truncate(180, TruncateOptions.CutOffTheMiddle);

            return $"Response<{typeName}> - OK ({content})";
        }

        /// <summary>
        /// 
        /// </summary>
        public static implicit operator T?(Response<T> response) => response.Data;

        /// <summary>
        /// 
        /// </summary>
        public static implicit operator bool(Response<T> response) => response.Success;
    }

    /// <summary>
    /// 
    /// </summary>
    [DebuggerNonUserCode]
    public static class Response
    {
        /// <summary>
        /// 
        /// </summary>
        public static Response<T> Create<T>(string source, string? content, string? error, object? request = null)
        {
            return new Response<T>()
            {
                Source = source,
                RawContent = content,
                Error = error,
                Request = request
            };
        }      
        
        /// <summary>
        /// Creates a succesfull response
        /// </summary>
        public static Response<T> OK<T>(T data, string source, string? content, object? request = null)
        {
            return new Response<T>()
            {
                Source = source,
                Data = data,
                RawContent = content,                
                Request = request
            };
        }

        /// <summary>
        /// Creates response with error
        /// </summary>
        public static Response<T> Failed<T>(Exception ex, string source, string? content, object? request = null)
        {
            return new Response<T>()
            {
                Source = source,                
                RawContent = content,
                Request = request,
                Error = $"{ex.GetType().Name}:{ex.Message}"
            };
        }
    }
}

