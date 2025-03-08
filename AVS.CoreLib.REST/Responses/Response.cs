#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using AVS.CoreLib.Abstractions.Responses;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Dynamic;
using AVS.CoreLib.Json;
using AVS.CoreLib.REST.Projections;

namespace AVS.CoreLib.REST.Responses
{
    [DebuggerDisplay("{ToString()}")]
    public sealed class Response<T> : IResponse<T>
    {
        public string Source { get; set; }
        public bool Success => Error == null;
        public T? Data { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Error { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public object? Request { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
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
                ? Data.ToBriefJson().Truncate(180, TruncateOptions.CutOffTheMiddle)
                : RawContent.Truncate(180, TruncateOptions.CutOffTheMiddle);

            return $"Response<{typeName}> - OK ({content})";
        }
    }

    [DebuggerNonUserCode]
    public static class Response
    {
        public static Response<T> Create<T>(string source, string? error = null, object? request = null)
        {
            return new Response<T>(source, string.Empty, error: error, request: request);
        }

        public static Response<T> Create<T>(string source, string content, string? error, object? request = null)
        {
            return new Response<T>(source, content, error: error, request: request);
        }

        internal static Response<T> Create<T>(T? data, string source, string content, string? error, object? request = null)
        {
            return new Response<T>(source, content, data, error, request);
        }

        public static Response<T> Create<T>(string source, string? error, Func<T> getData)
        {
            var response = new Response<T>(source, string.Empty, error: error);

            if (error == null)
                response.Data = getData.Invoke();

            return response;
        }

        public static Response<T> OK<T>(T data, string source, string content, object? request = null)
        {
            return new Response<T>(source, content, data, request: request);
        }

        public static Response<T> Failed<T>(Exception ex, string source, string content, object? request = null)
        {
            return new Response<T>(source, content, error: ex.Message, request: request);
        }
    }
}

