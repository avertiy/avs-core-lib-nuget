#nullable enable
using System;
using System.Diagnostics;
using System.Net;
using System.Text.Json.Serialization;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.Collections;
using AVS.CoreLib.Exceptions;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.REST.Helpers;
using AVS.CoreLib.REST.Json;
using AVS.CoreLib.REST.Responses;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace AVS.CoreLib.REST.Models
{
    /// <summary>
    /// Represents REST response object
    /// </summary>
    [DebuggerDisplay("RestResponse {ToString()}")]
    public class RestResponse
    {
        public string Source { get; set; } = null!;
        
        public HttpStatusCode StatusCode { get; set; }

        public string Content { get; set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Error { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public IRequest? Request { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public MultiDictionary<string, string>? Headers { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? RequestId => Request?.RequestId;
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]

        public bool IsSuccess => Error == null && IsSuccessStatusCode;
        public bool IsSuccessStatusCode => ((int)StatusCode) >= 200 && (int)StatusCode < 300;

        public RestResponse(HttpStatusCode code = HttpStatusCode.OK)
        {
            StatusCode = code;
            Source = string.Empty;
            Content = string.Empty;
        }

        public RestResponse(HttpStatusCode code, string source)
        {
            Source = source;
            StatusCode = code;
            Content = string.Empty;
        }

        public override string ToString()
        {
            var reqId = RequestId != null ? $" Req.Id={RequestId}" : null;

            if (IsSuccessStatusCode)
            {
                if (Error != null)
                    return $"({(int)StatusCode}){reqId} with Error={Error}";

                if (!string.IsNullOrEmpty(Content))
                {
                    if (Content.Length <= 360)
                        return $"({StatusCode}){reqId} Content={Content}";

                    var briefContent = Content.Truncate(360, TruncateOptions.CutOffTheMiddle);
                    return $"({StatusCode}){reqId} Content={briefContent} (Length={Content.Length})";
                }

                return $"({StatusCode}){reqId} Content=(null)";
            }

            return $"Failed ({StatusCode}){reqId} Error={Error}";
        }

        public T? Deserialize<T>()
        {
            return JsonHelper.Deserialize<T>(Content);
        }

        public T[]? DeserializeArray<T>()
        {
            return JsonHelper.DeserializeArray<T>(Content);
        }

        /// <summary>
        /// Creates result <see cref="Response{T}"/> giving the caller a full control over deserialization
        /// (deserialize to object or array, use proxy etc.)
        /// </summary>
        /// <remarks>the method is named ToResult although it returns Response{T}, it will be replaced with Result{T}</remarks>
        public Response<TResult> ToResult<TResult>(Func<RestResponse, TResult>? map = null)
        {
            var isSuccess = IsSuccessful();
            var result = Response.Create<TResult>(Source, content: Content, Error, Request);

            if (!isSuccess)
                return result;

            try
            {
                result.Data = map == null 
                    ? Deserialize<TResult>()
                    : map.Invoke(this);

                result.Error = Error;
            }
            catch (Exception ex)
            {
                result.Error ??= $"Map to Response<{typeof(TResult).Name}> failed";
                throw new MapJsonException($"Map to Response<{typeof(TResult).Name}> failed", ex) { JsonText = Content };
            }

            return result;
        } 

        internal static RestResponse Timeout(string source)
        {
            return new RestResponse(HttpStatusCode.RequestTimeout, source) { Error = "The request timed out." };
        }

        internal bool IsSuccessful()
        {
            if (!IsSuccess)
                return false;

            if (ResponseHelper.ContainsError(Content, out var error))
            {
                Error = error;
                return false;
            }

            return true;
        }
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member