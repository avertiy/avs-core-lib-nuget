#nullable enable
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AVS.CoreLib.Guards;
using AVS.CoreLib.REST.Json;
using Newtonsoft.Json.Linq;

namespace AVS.CoreLib.REST.Extensions;

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
        return new RestResponse(response.StatusCode, response.Source)
        {
            Request = response.Request,
            Content = content,
            Error = response.Error,
        };
    }

    /// <summary>
    /// Match JsonText with a regex pattern to select property value which is either json object or json array
    /// <code>
    /// result.Select("data",JsonType.Object):
    /// JsonText = { "data": { Item1 = "..."} } you might pick data property value i.e. { Item1 = "..."}
    ///
    /// result.Select("data", JsonType.Array):
    /// JsonText = { "data": [{...},{...},...] } => [{...},{...},...]
    /// </code>
    /// Returns new JsonResult object unless the result contains an error
    /// </summary>
    public static RestResponse Select(this RestResponse result, string name, JsonType tokenType)
    {
        // if has error do nothing
        if (result.Error != null || string.IsNullOrEmpty(result.Content))
            return result;

        Guard.MustBe.OneOf(tokenType, JsonType.Object, JsonType.Array);

        var regex = tokenType == JsonType.Object
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