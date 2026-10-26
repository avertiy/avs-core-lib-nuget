#nullable enable
using System;
using System.Diagnostics;
using System.Net;
using System.Text.Json.Serialization;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.Collections;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.REST.Helpers;
using AVS.CoreLib.REST.Json;
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

        public string? Content { get; set; }


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

                if (Content != null)
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
            if (Content == null)
                return default;

            try
            {
                return JsonHelper.Deserialize<T>(Content);
            }
            catch(Exception ex)
            {
                Error = ex.ToString();
                throw;
            }
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