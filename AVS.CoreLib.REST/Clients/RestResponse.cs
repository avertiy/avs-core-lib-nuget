#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using AVS.CoreLib.Abstractions.Rest;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Guards;
using AVS.CoreLib.REST.Extensions;
using AVS.CoreLib.REST.Json;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.REST
{
    [DebuggerDisplay("RestResponse {ToString()}")]
    public class RestResponse
    {
        public string Source { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public IRequest? Request { get; set; }
        public string Content { get; set; }
        public string? Error { get; set; }

        public bool IsSuccess => Error == null && IsSuccessStatusCode;
        public bool IsSuccessStatusCode => ((int)StatusCode) >= 200 && (int)StatusCode < 300;
        public string? RequestType => Request?.RequestId;

        public RestResponse(string source, HttpStatusCode code)
        {
            Source = source;
            StatusCode = code;
            Content = string.Empty;
        }

        public override string ToString()
        {
            var content = Content.Truncate(550, TruncateOptions.CutOffTheMiddle);
            return IsSuccess
                ? $"{StatusCode}: {content}"
                : $"Failed ({StatusCode}) - {Error}";
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

        //public static RestResponse OK(string source, string? content = null, HttpStatusCode code = HttpStatusCode.OK, IRequest? request = null)
        //{
        //    return new RestResponse(source, code) { Content = content, Request = request };
        //}

        internal static RestResponse FromResponse(HttpWebResponse response, string source, IRequest request)
        {
            var result = new RestResponse(source, response.StatusCode) { Request = request };

            if (response.StatusCode == HttpStatusCode.OK)
                result.Content = response.GetContent();
            else
                result.Error = response.StatusDescription;

            return result;
        }
    }

    public static class RestResponseExtensions
    {
        public static bool ContainsError(this RestResponse response, string errorText)
        {
            if (response.Error == null)
                return false;

            return response.Error.Contains(errorText);
        }

        public static bool TryDeserialize<T>(this RestResponse response, out T? value, out string? error)
        {
            try
            {
                value = response.Deserialize<T>();
                error = null;
                return true;
            }
            catch (Exception ex)
            {
                value = default;
                error = ex.ToString();
                return false;
            }
        }

        public static RestResponse Copy(this RestResponse response, string content)
        {
            return new RestResponse(response.Source, response.StatusCode)
            {
                Request = response.Request,
                Content = content,
                Error = response.Error,
            };
        }

        /// <summary>
        /// Match JsonText with a regex pattern to select property value which is either json object or json array
        /// <code>
        /// result.Select("data",JTokenType.Object):
        /// JsonText = { "data": { Item1 = "..."} } you might pick data property value i.e. { Item1 = "..."}
        ///
        /// result.Select("data", JTokenType.Array):
        /// JsonText = { "data": [{...},{...},...] } => [{...},{...},...]
        /// </code>
        /// Returns new JsonResult object unless the result contains an error
        /// </summary>
        public static RestResponse Select(this RestResponse result, string name, JTokenType tokenType)
        {
            // if has error do nothing
            if (result.Error != null || string.IsNullOrEmpty(result.Content))
                return result;

            Guard.MustBe.OneOf(tokenType, JTokenType.Object, JTokenType.Array);

            var regex = tokenType == JTokenType.Object
                ? $"\"{name}\":(?<data>{{.*?}})"
                : $"\"{name}\":(?<data>\\[.*?\\])";

            var re = new Regex(regex);
            var match = re.Match(result.Content);

            if (match.Success)
            {
                var text = match.Groups["data"].Success ? match.Groups["data"].Value : match.Value;
                return result.Copy(text);
            }

            result.Error = $"Invalid format [json text must match a regex pattern: {regex}]";
            return result;
        }

        /// <summary>
        /// Match JsonText with a regex pattern and select json matched the regex, regex should contain a data group value
        /// if matches returns new <see cref="RestResponse"/> object with JsonText set to matched "data" group value
        /// </summary>
        /// <param name="result"><see cref="RestResponse"/></param>
        /// <param name="regexPattern">pattern should contain a named group "data"</param>
        /// <param name="errorText">when pattern does not match and errorText is provided the JsonText will be set as the following json object { "error": "errorText"}</param>
        /// <param name="options"></param>
        public static RestResponse Select(this RestResponse result, string regexPattern, string? errorText = null, RegexOptions options = RegexOptions.None)
        {
            // if has error do nothing
            if (result.Error != null || string.IsNullOrEmpty(result.Content))
                return result;


            var re = new Regex(regexPattern, options);
            var match = re.Match(result.Content);

            if (match.Success)
            {
                var text = match.Groups["data"].Success ? match.Groups["data"].Value : match.Value;
                return result.Copy(text);
            }

            result.Error = errorText ?? result.Error ?? $"Invalid format [json text must match a regex pattern: {regexPattern}]";
            return result;
        }

        public static RestResponse SelectMany(this RestResponse result, string regexPattern, string? errorText = null, RegexOptions options = RegexOptions.None)
        {
            // if has error do nothing
            if (result.Error != null)
                return result;

            var re = new Regex(regexPattern, options);
            var match = re.Match(result.Content);
            if (match.Success)
            {
                var items = new List<string>();
                while (match.Success)
                {
                    items.Add(match.Groups["data"].Success ? match.Groups["data"].Value : match.Value);
                    match = match.NextMatch();
                }

                var text = $"[{string.Join(",", items)}]";
                return result.Copy(text);
            }

            result.Error = errorText ?? result.Error ?? $"Invalid format [json text must match a regex pattern: {regexPattern}]";
            return result;
        }
    }
}
