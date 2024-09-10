#nullable enable
using System;
using System.Text.RegularExpressions;

namespace AVS.CoreLib.REST.Helpers;

internal static class ResponseHelper
{
    private const string REGEX_PATTERN = "\"(?<msg>message|error|err-msg|error-message)\"[\\s:]+\"(?<err>.+)\"";
    public static Regex ErrorRegex = new Regex(REGEX_PATTERN, RegexOptions.IgnoreCase);

    public static bool ContainsError(string? content, out string? error)
    {
        error = null;
        if (string.IsNullOrEmpty(content) || content.Length > 500)
            return false;

        var match = ErrorRegex.Match(content);

        if (!match.Success)
            return false;

        if (match.Groups["msg"].Value == "message")
        {
            var ind = content.IndexOf("code", StringComparison.Ordinal);
            if (ind == -1 || content.IndexOf("200", ind, StringComparison.Ordinal) > 0)
                return false;
        }

        error = match.Groups["err"].Value;
        error = string.IsNullOrEmpty(error) ? content : error;
        return true;
    }
}