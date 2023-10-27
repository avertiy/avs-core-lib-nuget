#nullable enable
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using AVS.CoreLib.Guards;
using AVS.CoreLib.REST.Extensions;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.REST
{
    public class JsonResult
    {
        public string JsonText { get; private set; }

        public string Source { get; private set; }

        public string? Error { get; private set; }

        public JsonResult(string source, string? text = null, string? error = null)
        {
            Source = source;
            JsonText = text ?? string.Empty;
            Error = error;
        }

        public bool HasError
        {
            get
            {
                if (!string.IsNullOrEmpty(Error))
                    return true;

                var re = new Regex("(error|err-msg|error-message)[\"']?:[\"']?(?<error>.*?)[\"',}]", RegexOptions.IgnoreCase);
                var match = re.Match(JsonText);

                if (match.Success)
                {
                    Error = match.Groups["error"].Value;
                }

                return match.Success;
            }
        }

        /// <summary>
        /// <see cref="JsonText"/> might contain
        /// </summary>
        [Obsolete("seems no usages, use HasError prop")]
        public bool ContainsError()
        {
            if (Error != null)
                return true;

            var re = new Regex("(error|err-msg|error-message)[\"']?:[\"']?(?<error>.*?)[\"',}]", RegexOptions.IgnoreCase);
            var match = re.Match(JsonText);

            if (match.Success)
            {
                Error = match.Groups["error"].Value;
            }

            return match.Success;
        }

        

        public override string ToString()
        {
            return $"JsonResult: {Source} => {(HasError ? Error : JsonText)}";
        }

        public static implicit operator string(JsonResult result)
        {
            return result?.JsonText!;
        }

        public static JsonResult Success(string source, string text)
        {
            return new JsonResult(source, text);
        }

        public static JsonResult Failed(string source, string error, string? text = null)
        {
            return new JsonResult(source, text: text, error: error);
        }

        public static JsonResult FromResponse(HttpWebResponse response, string source)
        {
            var json = response.GetContent();
            var error = response.StatusCode == HttpStatusCode.OK ? null : response.StatusDescription;
            var result = new JsonResult(source, json, error);
            return result;
        }
        
    }

    public static class JsonResultExtensions
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
        public static JsonResult Select(this JsonResult result, string name, JTokenType tokenType)
        {
            // if has error do nothing
            if (result.Error != null)
                return result;

            Guard.MustBe.OneOf(tokenType, JTokenType.Object, JTokenType.Array);

            var regex = tokenType == JTokenType.Object
                ? $"\"{name}\":(?<data>{{.+?}})"
                : $"\"{name}\":(?<data>\\[.+?\\])";

            var re = new Regex(regex);
            var match = re.Match(result.JsonText);

            if (match.Success)
            {
                var text = match.Groups["data"].Success ? match.Groups["data"].Value : match.Value;
                return JsonResult.Success(result.Source, text);
            }

            return JsonResult.Failed(result.Source, $"Invalid format [json text must match a regex pattern: {regex}]", result.JsonText);
        }

        /// <summary>
        /// Match JsonText with a regex pattern and select json matched the regex, regex should contain a data group value
        /// if matches returns new <see cref="JsonResult"/> object with JsonText set to matched "data" group value
        /// </summary>
        /// <param name="result"><see cref="JsonResult"/></param>
        /// <param name="regexPattern">pattern should contain a named group "data"</param>
        /// <param name="errorText">when pattern does not match and errorText is provided the JsonText will be set as the following json object { "error": "errorText"}</param>
        /// <param name="options"></param>
        public static JsonResult Select(this JsonResult result, string regexPattern, string? errorText = null, RegexOptions options = RegexOptions.None)
        {
            // if has error do nothing
            if (result.Error != null || string.IsNullOrEmpty(result.JsonText))
                return result;


            var re = new Regex(regexPattern, options);
            var match = re.Match(result.JsonText);

            if (match.Success)
            {
                var text = match.Groups["data"].Success ? match.Groups["data"].Value : match.Value;
                return JsonResult.Success(result.Source, text);
            }

            var error = errorText ?? result.Error ?? $"Invalid format [json text must match a regex pattern: {regexPattern}]";
            return JsonResult.Failed(result.Source, error, result.JsonText);
        }

        public static JsonResult SelectMany(this JsonResult result, string regexPattern, string? errorText = null, RegexOptions options = RegexOptions.None)
        {
            // if has error do nothing
            if (result.Error != null)
                return result;

            var re = new Regex(regexPattern, options);
            var match = re.Match(result.JsonText);
            if (match.Success)
            {
                var items = new List<string>();
                while (match.Success)
                {
                    items.Add(match.Groups["data"].Success ? match.Groups["data"].Value : match.Value);
                    match = match.NextMatch();
                }

                var text = $"[{string.Join(",", items)}]";
                return JsonResult.Success(result.Source, text);
            }

            var error = errorText ?? result.Error ?? $"Invalid format [json text must match a regex pattern: {regexPattern}]";
            return JsonResult.Failed(result.Source, error, result.JsonText);
        }
    }
}
