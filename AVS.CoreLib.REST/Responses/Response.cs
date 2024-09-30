#nullable enable
using System;
using System.Diagnostics;
using System.Text.Json.Serialization;
using AVS.CoreLib.Abstractions.Responses;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Dynamic;
using AVS.CoreLib.Json;
//using Newtonsoft.Json;

namespace AVS.CoreLib.REST.Responses
{
    //TODO this stuff with Response looks too complicated, this needs to be simplified to basic stuff without props etc. things

    public class Response : ResponseBase
    {
        #region Create static methods
        public static Response<T> Create<T>(string source, string? error, object? request = null)
        {
            return new Response<T>(source, error: error, request: request);
        }

        internal static Response<T> Create<T>(T? data, string source, string? error, object? request = null)
        {
            return new Response<T>(source, data, error, request);
        }

        public static Response<T> OK<T>(T data, string source, object? request = null)
        {
            return new Response<T>(source, data, request: request);
        }

        public static IResponse<T> Failed<T>(Exception ex, string source, object? request = null)
        {
            return new Response<T>(source, error: ex.Message, request: request);
        }

        #endregion
    }

    [DebuggerDisplay("{ToString()}")]
    public class Response<T> : IResponse<T>
    {
        public string Source { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Request { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Error { get; set; }
        public T? Data { get; set; }
        public virtual bool Success => Error == null;

        public Response(string source, T? data = default, string? error = null, object? request = null)
        {
            Source = source;
            Error = error;
            Data = data;
            Request = request;
        }

        public override string ToString()
        {
            if (Error != null)
                return $"Response<{typeof(T).Name}> - Failed ({Error})";

            if (Data == null)
                return $"Response<{typeof(T).Name}> - OK (NO DATA)";

            if (Data.TryGetCount(out var count))
                return $"Response<{typeof(T).Name}> - OK (#{count})";

            var data = Data.ToBriefJson().Truncate(600, TruncateOptions.CutOffTheMiddle);
            return $"Response<{typeof(T).Name}> - OK  {data}";
        }
    }
}

