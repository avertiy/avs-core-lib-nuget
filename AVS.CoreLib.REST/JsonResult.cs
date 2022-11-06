using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using AVS.CoreLib.REST.Extensions;
using AVS.CoreLib.Utilities;

namespace AVS.CoreLib.REST
{
    public class JsonResult
    {
        public string JsonText { get; set; }

        public string Error { get; set; }

        public string Source { get; set; }

        public JsonResult()
        {
        }

        //public JsonResult(string jsonText)
        //{
        //    JsonText = jsonText;
        //}

        /// <summary>
        /// Match JsonText to regex pattern and cut it to match data group
        /// </summary>
        /// <param name="regex_pattern">pattern should contain a named group "data"</param>
        /// <param name="errorText">when pattern does not match and errorText is provided the JsonText will be set as the following json object { "error": "errorText"}</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public bool Take(string regex_pattern, string errorText = null, RegexOptions options = RegexOptions.None)
        {
            var re = new Regex(regex_pattern, options);
            var match = re.Match(JsonText);

            if (match.Success)
            {
                JsonText = match.Groups["data"].Success ? match.Groups["data"].Value : match.Value;
            }
            else if (!string.IsNullOrEmpty(errorText))
            {
                JsonText = $"{{\"error\":\"{errorText}\"}}";
            }
            return match.Success;
        }

        public JsonResult TakeMany(string regex_pattern, string errorText = null, RegexOptions options = RegexOptions.None)
        {
            var re = new Regex(regex_pattern, options);
            var match = re.Match(JsonText);
            if (match.Success)
            {
                var items = new List<string>();
                while (match.Success)
                {
                    items.Add(match.Groups["data"].Success ? match.Groups["data"].Value : match.Value);
                    match = match.NextMatch();
                }
                JsonText = $"[{string.Join(",", items)}]";
            }
            else if (!string.IsNullOrEmpty(errorText))
            {
                JsonText = $"{{\"error\":\"{errorText}\"}}";
            }
            return this;
        }

        public static implicit operator string(JsonResult result)
        {
            return result?.JsonText;
        }

        public bool HasError
        {
            get
            {
                var re = new Regex("(error|err-msg|error-message)[\"']?:[\"']?(?<error>.*?)[\"',}]", RegexOptions.IgnoreCase);
                var match = re.Match(JsonText);

                if (match.Success)
                {
                    Error = match.Groups["error"].Value;
                }

                return match.Success;
            }
        }

        public override string ToString()
        {
            return $"JsonResult: {Source} => {JsonText}";
        }

        public static JsonResult FromResponse(HttpWebResponse response, string source = null)
        {
            var json = response.GetContent();
            var result = new JsonResult() { JsonText = json, Source = source };
            
            if (response.StatusCode == HttpStatusCode.OK)
                return result;

            result.Error = response.StatusDescription;
            return result;
        }
    }
}
