#nullable enable
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Guards;
using AVS.CoreLib.REST.Extensions;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.REST
{   
    [DebuggerDisplay("{ToString()}")]
    public class RestResponse
    {
        public string Source { get; set; }
        public string? Content { get; set; }
        public string? Error { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess => Error == null && IsSuccessStatusCode;
        public bool IsSuccessStatusCode => ((int)StatusCode) >= 200 && (int)StatusCode < 300;

        public RestResponse(string source, HttpStatusCode code)
        {
            Source = source;
            StatusCode = code;
        }

        public override string ToString()
        {
            var content = Content == null ? string.Empty : Content.Truncate(90, $".. (Length={Content.Length})");
            return IsSuccess ? $"RestResponse - {StatusCode} => {content}" : $"RestResponse - Failed ({StatusCode}) - {Error}";
        }        

        public static RestResponse OK(string source, HttpStatusCode code, string content)
        {
            return new RestResponse(source, code) { Content = content };
        }

        public static RestResponse FromResponse(HttpWebResponse response, string source)
        {
            var result = new RestResponse(source, response.StatusCode);

            if(response.StatusCode == HttpStatusCode.OK)
                result.Content = response.GetContent();
            else
                result.Error = response.StatusDescription;

            return result;
        }
    }

    public static class RestResponseExtensions
    {
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
                ? $"\"{name}\":(?<data>{{.+?}})"
                : $"\"{name}\":(?<data>\\[.+?\\])";

            var re = new Regex(regex);
            var match = re.Match(result.Content);

            if (match.Success)
            {
                var text = match.Groups["data"].Success ? match.Groups["data"].Value : match.Value;
                return RestResponse.OK(result.Source, result.StatusCode, text);
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
                return RestResponse.OK(result.Source, result.StatusCode, text);
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
                return RestResponse.OK(result.Source, result.StatusCode, text);
            }

            result.Error = errorText ?? result.Error ?? $"Invalid format [json text must match a regex pattern: {regexPattern}]";
            return result;
        }
    }
}
