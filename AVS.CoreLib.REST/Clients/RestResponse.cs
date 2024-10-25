#nullable enable
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.Collections;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.REST.Extensions;
using AVS.CoreLib.REST.Json;

namespace AVS.CoreLib.REST
{
    [DebuggerDisplay("RestResponse {ToString()}")]
    public class RestResponse
    {
        public string Source { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Content { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Error { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public IRequest? Request { get; set; }
        public MultiDictionary<string, string>? Headers { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? RequestId => Request?.RequestId;

        public bool IsSuccess => Error == null && IsSuccessStatusCode;

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public bool IsSuccessStatusCode => ((int)StatusCode) >= 200 && (int)StatusCode < 300;
        

        public RestResponse(string source, HttpStatusCode code)
        {
            Source = source;
            StatusCode = code;
            Content = string.Empty;
        }

        public override string ToString()
        {
            var content = Content.Truncate(550, TruncateOptions.CutOffTheMiddle);

            var reqId = RequestId != null ? $"(Req.Id #{RequestId})" : null;

            return IsSuccess
                ? $"{StatusCode}: {content} {reqId}"
                : $"Failed ({StatusCode}) - {Error} {reqId}";
        }

        public T? Deserialize<T>()
        {
            try
            {
                return JsonHelper.DeserializeObject<T>(Content);
            }
            catch (Exception ex)
            {
                throw new Exception($"RestResponse.Deserialize<{typeof(T).Name}>() failed", ex);
            }
        }

        internal static RestResponse FromHttpWebResponse(HttpWebResponse response, string source, IRequest request)
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

        internal static RestResponse Timeout(string source)
        {
            return new RestResponse(source, HttpStatusCode.RequestTimeout) { Error = "The request timed out." };
        }
    }
}
