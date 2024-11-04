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
    [DebuggerDisplay("{ToString()}")]
    public sealed class Response<T> : IResponse<T>
    {
        public string Source { get; set; }
        public bool Success => Error == null;


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Request { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Error { get; set; }
        public T? Data { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RawContent { get; set; }

        public Response(string source, string rawContent, T? data = default, string? error = null, object? request = null)
        {
            Source = source;
            RawContent = rawContent;
            Error = error;
            Data = data;
            Request = request;
        }

        public Response(string source, T? data = default, string? error = null, object? request = null)
        {
            Source = source;
            Error = error;
            Data = data;
            Request = request;
        }

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
                ? Data.ToBriefJson().Truncate(550, TruncateOptions.CutOffTheMiddle)
                : RawContent.Truncate(550, TruncateOptions.CutOffTheMiddle);

            return $"Response<{typeName}> - OK ({content})";
        }
    }

    [DebuggerNonUserCode]
    public static class Response
    {
        public static Response<T> Create<T>(string source, string content, string? error, object? request = null)
        {
            return new Response<T>(source, content, error: error, request: request);
        }

        internal static Response<T> Create<T>(T? data, string source, string content, string? error, object? request = null)
        {
            return new Response<T>(source, content, data, error, request);
        }

        public static Response<T> OK<T>(T data, string source, string content, object? request = null)
        {
            return new Response<T>(source, content, data, request: request);
        }

        public static Response<T> Failed<T>(Exception ex, string source, string content, object? request = null)
        {
            return new Response<T>(source, content, error: ex.Message, request: request);
        }

        //public static Response<T> Create<T>(string source, string? error, object? request = null)
        //{
        //    return new Response<T>(source, error: error, request: request);
        //}

        //internal static Response<T> Create<T>(T? data, string source, string? error, object? request = null)
        //{
        //    return new Response<T>(source, data, error, request);
        //}
    }
}

