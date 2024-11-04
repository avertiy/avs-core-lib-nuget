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
        
        public RestResponse(string source, HttpStatusCode code)
        {
            Source = source;
            StatusCode = code;
            Content = string.Empty;
        }

        public override string ToString()
        {
            var content = GetBriefContent(550);

            var reqId = RequestId != null ? $" RequestId={RequestId}" : null;

            return IsSuccess
                ? $"{StatusCode}: Content={content} (Length={Content.Length}){reqId}"
                : $"Failed ({StatusCode}) - {Error}{reqId}";
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
            return new RestResponse(source, HttpStatusCode.RequestTimeout) { Error = "The request timed out." };
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
            var result = new RestResponse(source, response.StatusCode) { Request = request };

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
