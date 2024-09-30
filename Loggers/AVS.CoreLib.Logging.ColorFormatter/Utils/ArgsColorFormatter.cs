using System.Collections;
using System.Text;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Linq;
using AVS.CoreLib.Extensions.Reflection;
using AVS.CoreLib.Json;
using AVS.CoreLib.Logging.ColorFormatter.Enums;
using AVS.CoreLib.Logging.ColorFormatter.Extensions;

namespace AVS.CoreLib.Logging.ColorFormatter.Utils;

/// <summary>
/// Format 
/// </summary>
/// <code>
/// logger.LogInformation("{arg:C}", 1.022); => "<Green>$1.02</Green>"
/// i.e. arg=1.022 is formatted as currency with `C` modifier and wrapped into color tags
/// </code>
public class ArgsColorFormatter
{
    public string OriginalMessage { get; init; }
    private readonly IColorProvider _colorProvider;
    public ArgsColorFormatter(IColorProvider colorProvider)
    {
        _colorProvider = colorProvider;
        InitKeywords();
    }

    public string Format(State state)
    {
        var sb = new StringBuilder(state.Format);

        for (var i = 0; i < state.Keys.Length; i++)
        {
            var (key, obj, str) = state[i];

            var keyInBrackets = '{' + key + '}';
            if (string.IsNullOrEmpty(str) || obj == null)
            {
                sb.Replace(keyInBrackets, str);
                continue;
            }

            var colonInd = key.IndexOf(':');

            if (colonInd > 0 && Colors.TryParse(key.Substring(colonInd), out var colors))
            {
                var formattedStr = colors.FormatWithTags(str);
                sb.Replace(keyInBrackets, formattedStr);
                continue;
            }

            if (obj is string)
            {
                var formattedStr = FormatString(str);
                sb.Replace(keyInBrackets, formattedStr);
                continue;
            }

            if (obj is IEnumerable enumerable)
            {
                var formattedStr = FormatEnumerable(key, enumerable);
                sb.Replace(keyInBrackets, formattedStr);
                continue;
            }

            if (key.StartsWith("@"))
            {
                var json = key.StartsWith("@@")
                    ? obj.ToJson().Truncate(1000, TruncateOptions.CutOffTheMiddle)
                    : obj.ToBriefJson().Truncate(100, TruncateOptions.CutOffTheMiddle);

                var formattedStr = Frmt(obj.GetType().GetReadableName(), TextKind.Keyword) + " " + Frmt(json, TextKind.Json);
                sb.Replace(keyInBrackets, formattedStr);
                continue;
            }

            if (obj.IsNumeric())
            {
                var flags = obj.GetNumberFlags(str);
                var ind = sb.IndexOf(keyInBrackets) - 1;
                //highlight # symbol as well
                if (ind >= 0 && sb[ind] == '#' || (ind > 1 && sb[ind - 1] == '#'))
                {
                    sb.Remove(ind, 1);
                    str = "#" + str;
                }

                colors = _colorProvider.GetColors(flags);
                var formattedStr = colors.FormatWithTags(str);
                sb.Replace(keyInBrackets, formattedStr);
                continue;
            }

            var objType = obj.GetObjType();

            if (objType == ObjType.Object)
            {
                var formattedStr = FormatObject(str);
                sb.Replace(keyInBrackets, formattedStr);
                continue;
            }

            colors = _colorProvider.GetColors(objType);
            var coloredStr = colors.FormatWithTags(str);
            sb.Replace(keyInBrackets, coloredStr);

        }

        FrmtStartKeywords(sb);
        return sb.ToString();
    }

    private string FormatEnumerable(string key, IEnumerable enumerable)
    {
        string json;
        if (key.StartsWith("@@"))
        {
            json = enumerable.ToJson();
            json = json.Truncate(1000, TruncateOptions.CutOffTheMiddle);
        }
        else
        {
            json = enumerable.ToBriefJson();
            json = json.Truncate(100, TruncateOptions.CutOffTheMiddle);
        }
        
        var formattedStr = Frmt(json,  TextKind.Array);
        return formattedStr;
    }

    private string FormatObject(string str)
    {
        // obj
        if (str.StartsWith('{') && str.EndsWithAny('}', '.', '"', ','))
        {
            return Frmt(str, str.Contains(':') ? TextKind.Json : TextKind.Brackets);
        }

        // arr
        if (str.StartsWith('[') && str.EndsWithAny(']', '.', '"', ','))
        {
            return Frmt(str, str.Contains(':') ? TextKind.Json : TextKind.Array);
        }

        var sb = new StringBuilder(str);
        //limit str scanning with 50 symbols
        var count = str.Length < 50 ? str.Length : 50;
        FrmtJson(sb, count);
        FrmtStringWithUrl(sb, count);
        FrmtHttpVerbs(sb, count);
        FrmtResponse(sb, count);
        FrmtKeywords(sb, count);
        FrmtEndKeywords(sb);

        return Frmt(sb.ToString(), str.Length > 255 ? TextKind.Text : TextKind.None);
    }

    private string FormatString(string str)
    {
        // json obj or curly brackets?
        if (str.StartsWith('{') && str.EndsWith('}'))
        {
            return Frmt(str, str.Contains(':') ? TextKind.Json : TextKind.Brackets);
        }

        // json arr or brackets?
        if (str.StartsWith('[') && str.EndsWith(']'))
        {
            return Frmt(str, str.Contains(':') ? TextKind.Json : TextKind.Array);
        }

        // value in square brackets
        if (str.StartsWith('(') && str.EndsWith(')'))
            return Frmt(str, TextKind.Brackets);

        if (str.Length <= 9 && str.Contains('_'))
            return Frmt(str, TextKind.Symbol);

        // url
        if (str.StartsWith("https://") && !str.Contains(' '))
            return Frmt(str, TextKind.Url);

        var pathInd = str.IndexOf(":\\", StringComparison.Ordinal);

        if (pathInd == 1 && str.Length < 255)
            return Frmt(str, TextKind.FilePath);

        if (str.StartsWith('"') && str.EndsWith('"'))
            return Frmt(str, TextKind.DoubleQuotes);

        if (str.StartsWith('\'') && str.EndsWith('\''))
            return Frmt(str, TextKind.Quotes);

        var sb = new StringBuilder(str);
        //limit str scanning with 50 symbols
        var count = str.Length < 50 ? str.Length : 50;

        FrmtJson(sb, count);
        FrmtStringWithUrl(sb, count);
        FrmtHttpVerbs(sb, count);
        FrmtResponse(sb, count);
        FrmtKeywords(sb, count);
        FrmtEndKeywords(sb);

        return Frmt(sb.ToString(), str.Length > 255 ? TextKind.Text : TextKind.None);
    }

    private void FrmtJson(StringBuilder sb, int count)
    {
        // contains json-like object inside of string?
        var openBracketInd = sb.IndexOf('{', 1, count);

        if (openBracketInd > -1)
        {
            var closeBracketInd = sb.LastIndexOf('}');

            if (closeBracketInd > openBracketInd)
            {
                // if json represents an array [{...}, {...}]
                if (openBracketInd > 0 && sb[openBracketInd - 1] == '[' && closeBracketInd + 1 < sb.Length &&
                    sb[closeBracketInd + 1] == ']')
                {
                    openBracketInd--;
                    closeBracketInd++;
                }

                var textKind = sb.IndexOf(':', openBracketInd) > openBracketInd ? TextKind.Json : TextKind.Brackets;
                var length = closeBracketInd - openBracketInd + 1;
                var subStr = sb.ToString(openBracketInd, length);
                sb.Replace(subStr, Frmt(subStr, textKind), openBracketInd, length);
            }
        }
    }

    private void FrmtStartKeywords(StringBuilder sb)
    {
        foreach (var keyword in StartKeywords)
        {
            if (sb.StartsWith(keyword.Key))
            {
                sb.Replace(keyword.Key, keyword.Value);
                return;
            }
        }

    }

    private void FrmtEndKeywords(StringBuilder sb)
    {
        if (sb.Length > 200)
            return;

        foreach (var keyword in EndKeywords)
        {
            if (sb.EndsWith(keyword.Key))
            {
                sb = sb.Replace(keyword.Key, keyword.Value);
                break;
            }
        }
    }

    private void FrmtKeywords(StringBuilder sb, int count)
    {
        foreach (var keyword in Keywords)
        {
            var ind = sb.IndexOf(keyword.Key, 0, count);

            if (ind < 0)
                continue;

            var startInd = sb.IndexOf(' ', 0, ind);
            if (startInd < 0)
            {
                startInd = 0;
            }
            else
            {
                startInd++;
            }

            var textToReplace = sb.ToString(startInd, ind - startInd + keyword.Key.Length);
            var value = keyword.Value.Replace(keyword.Key, textToReplace);
            sb.Replace(textToReplace, value);
        }
    }

    private void FrmtHttpVerbs(StringBuilder sb, int count)
    {
        foreach (var verb in Verbs)
        {
            var ind = sb.IndexOf(verb.Key, 0, count);
            if (ind < 0)
                continue;

            sb.Replace(verb.Key, verb.Value, ind, verb.Key.Length);
            break;
        }
    }

    private void FrmtStringWithUrl(StringBuilder sb, int count)
    {
        var ind = sb.IndexOf("https://", 0, count);

        if (ind > 0)
        {
            var endInd = sb.IndexOf(" ", ind);
            var length = endInd > -1 ? endInd - ind : sb.Length - ind;
            var substr = sb.ToString(ind, length);
            sb.Replace(substr, Frmt(substr, TextKind.Url), ind, length);
        }
    }

    private void FrmtResponse(StringBuilder sb, int count)
    {
        var ind = sb.IndexOf(" => ", 0, count);

        if (ind < 0)
            return;

        var ind2 = ind + 4;
        if (sb.Length > ind2 && sb[ind2].Either('[', '{'))
        {
            var substr = sb.ToString(ind2, sb.Length - ind2);
            sb.Replace(substr, Frmt(substr, TextKind.Json), ind2, sb.Length - ind2);
        }
    }


    private string Frmt(string str, TextKind flag)
    {
        Colors colors = _colorProvider.GetColors(flag);
        return colors.FormatWithTags(str);
    }

    public static Dictionary<string, string> StartKeywords = new(11);
    public static Dictionary<string, string> Keywords = new(6);
    public static Dictionary<string, string> EndKeywords = new(3);
    public static Dictionary<string, string> Verbs = new(5);

    private void InitKeywords()
    {
        if (StartKeywords.Any())
            return;

        var keywords = new[]
        {
            "Start handling",
            "Finished handling",
            "Handling",
            "Start processing",
            "Finished processing",
            "Start loading",
            "Finished loading",
            "Start ",
            "End ",
            "Sending..",
            "Received:",
        };

        foreach (var keyword in keywords)
            StartKeywords[keyword] = Frmt(keyword, TextKind.Keyword);

        Keywords["Request"] = Frmt("Request", TextKind.Keyword);
        Keywords["Response"] = Frmt("Response", TextKind.Keyword);
        Keywords["(OK)"] = Frmt("(OK)", TextKind.OK);
        Keywords[" OK"] = Frmt(" OK", TextKind.OK);
        Keywords[" Failed"] = Frmt(" Failed", TextKind.Error);
        Keywords["BadRequest"] = Frmt("BadRequest", TextKind.Error);

        EndKeywords["(FROM CACHE)"] = Frmt("(FROM CACHE)", TextKind.Keyword);
        EndKeywords[" OK"] = Frmt(" OK", TextKind.OK);
        EndKeywords["Failed"] = Frmt("Failed", TextKind.Error);

        var httpVerbs = new[] { "GET", "POST", "PUT", "DELETE", "PATCH" };

        foreach (var keyword in httpVerbs)
            Verbs[keyword] = Frmt(keyword, TextKind.HttpVerb);
    }
}