#nullable enable
using System.Diagnostics;
using System.Net;
using System.Text.Json.Serialization;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.Collections;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.REST.Extensions;
using AVS.CoreLib.REST.Helpers;
using AVS.CoreLib.REST.Json;

namespace AVS.CoreLib.REST
{
    [DebuggerDisplay("RestResponse {ToString()}")]
    public class RestResponse
    {
        public string Source { get; set; }
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

                if (Content.Length <= 360)
                    return $"({StatusCode}){reqId} Content={Content}";

                var content = GetBriefContent(360);
                return $"({StatusCode}){reqId} Content={content} (Length={content.Length})";
            }

            return $"Failed ({StatusCode}){reqId} Error={Error}";
        }

        public T? Deserialize<T>()
        {
            return JsonHelper.Deserialize<T>(Content);
        }

        public string GetBriefContent(int maxLength, TruncateOptions options = TruncateOptions.CutOffTheMiddle)
        {
            return Content.Truncate(maxLength, options);
        }

        internal static RestResponse Timeout(string source)
        {
            return new RestResponse(HttpStatusCode.RequestTimeout, source) { Error = "The request timed out." };
        }
    }

    internal static class RestResponseExtensions
    {
        internal static bool IsSuccessful(this RestResponse response)
        {
            if (!response.IsSuccess)
                return false;

            if (ResponseHelper.ContainsError(response.Content, out var error))
            {
                response.Error = error;
                return false;
            }

            return true;
        }

        internal static RestResponse ToRestResponse(this HttpWebResponse response, IRequest request, string source)
        {
            var result = new RestResponse(response.StatusCode, source) { Request = request };

            if (response.Headers.Count > 0)
            {
                result.Headers = new MultiDictionary<string, string>();

                foreach (string key in response.Headers)
                {
                    var values = response.Headers.GetValues(key);

                    if (values == null)
                        continue;

                    result.Headers.Add(key, values);
                }
            }

            if (response.StatusCode == HttpStatusCode.OK)
                result.Content = response.GetContent();
            else
                result.Error = response.StatusDescription;

            return result;
        }
    }
}
