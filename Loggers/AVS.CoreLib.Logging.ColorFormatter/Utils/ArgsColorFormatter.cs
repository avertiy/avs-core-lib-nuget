using System.Text;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Extensions;
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
    private IColorProvider _colorProvider;
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
            else
            {
                colors = _colorProvider.GetColors(objType);
                var coloredStr = colors.FormatWithTags(str);
                sb.Replace(keyInBrackets, coloredStr);
            }

        }

        var text = sb.ToString();
        return FrmtStartKeywords(text);
    }

    private string FormatObject(string str)
    {
        // obj
        if (str.StartsWith('{') && str.EndsWithEither('}', '.', '"', ','))
        {
            if (str.Contains('"') && str.Contains(':'))
                return Frmt(str, TextKind.Json);

            return Frmt(str, TextKind.Brackets);
        }

        // arr
        if (str.StartsWith('[') && str.EndsWithEither(']', '.', '"', ','))
        {
            if (str.Contains('"') && str.Contains(':'))
                return Frmt(str, TextKind.Json);

            return Frmt(str, TextKind.Array);
        }

        //limit str scanning with 30 symbols
        var count = str.Length < 30 ? str.Length : 30;

        str = FrmtStringWithUrl(str, count);
        str = FrmtHttpVerbs(str, count);
        str = FrmtResponse(str, count);
        str = FrmtKeywords(str, count);
        str = FrmtEndKeywords(str);
        return Frmt(str, TextKind.None);
    }

    private string FormatString(string str)
    {
        // json obj
        if (str.StartsWith('{') && str.EndsWith('}'))
        {
            if (str.Contains('"') && str.Contains(':'))
                return Frmt(str, TextKind.Json);

            return Frmt(str, TextKind.Brackets);
        }

        // json arr
        if (str.StartsWith('[') && str.EndsWith(']'))
        {
            if (str.Contains('"') && str.Contains(':'))
                return Frmt(str, TextKind.Json);

            return Frmt(str, TextKind.Array);
        }

        // value in square brackets
        if (str.StartsWith('(') && str.EndsWith(')'))
            return Frmt(str, TextKind.Brackets);

        if (str.Length <= 9 && str.Contains('_'))
            return Frmt(str, TextKind.Symbol);

        // url
        if (str.StartsWith("https://") && !str.Contains(' '))
            return Frmt(str, TextKind.Url);

        if (str.StartsWith('"') && str.EndsWith('"'))
            return Frmt(str, TextKind.DoubleQuotes);

        if (str.StartsWith('\'') && str.EndsWith('\''))
            return Frmt(str, TextKind.Quotes);

        return Frmt(str, str.Length > 255 ? TextKind.Text : TextKind.None);
    }

    private string FrmtStartKeywords(string str)
    {
        foreach (var keyword in StartKeywords)
        {
            if (str.StartsWith(keyword.Key))
            {
                return str.Replace(keyword.Key, keyword.Value);
            }
        }

        return str;
    }

    private string FrmtResponse(string str, int count)
    {
        var ind = str.IndexOf(" => ", 0, count);

        if (ind < 0)
            return str;

        var ind2 = ind + 4;
        if (str.Length > ind2 && str[ind2].Either('[', '{'))
        {
            var substr = str.Substring(ind2);
            str = str.Replace(substr, Frmt(substr, TextKind.Json));
        }

        return str;
    }

    private string FrmtKeywords(string str, int count)
    {
        foreach (var keyword in Keywords)
        {
            var ind = str.IndexOf(keyword.Key, 0, count);

            if (ind <= 0)
                continue;

            var startInd = str.IndexOf(' ', 0, ind);
            if (startInd < 0)
            {
                startInd = 0;
            }
            else
            {
                startInd++;
            }

            var textToReplace = str.Substring(startInd, ind - startInd + keyword.Key.Length);
            var value = keyword.Value.Replace(keyword.Key, textToReplace);
            return str.Replace(textToReplace, value);
        }

        return str;
    }

    private string FrmtEndKeywords(string str)
    {
        if (str.Length > 200)
            return str;

        foreach (var keyword in EndKeywords)
        {
            if (str.EndsWith(keyword.Key))
            {
                str = str.Replace(keyword.Key, keyword.Value);
                break;
            }
        }

        return str;
    }

    private string FrmtHttpVerbs(string str, int count)
    {
        foreach (var verb in Verbs)
        {
            var ind = str.IndexOf(verb.Key, 0, count);
            if (ind < 0)
                continue;

            str = str.Replace(verb.Key, verb.Value);
            break;
        }

        return str;
    }

    private string FrmtStringWithUrl(string str, int count)
    {
        var ind = str.IndexOf("https://", 0, count);
        if (ind > 0)
        {
            var substr = str.Substring(ind);
            str = str.Replace(substr, Frmt(substr, TextKind.Url));
        }
        return str;
    }

    private string Frmt(string str, TextKind flag)
    {
        Colors colors = _colorProvider.GetColors(flag);
        return colors.FormatWithTags(str);
    }

    public static Dictionary<string, string> StartKeywords = new(9);
    public static Dictionary<string, string> Keywords = new(2);
    public static Dictionary<string, string> EndKeywords = new(1);
    public static Dictionary<string, string> Verbs = new(5);

    private void InitKeywords()
    {
        if (StartKeywords.Any())
            return;

        var keywords = new[] { "Start handling", "Finished handling", "Handling", "Start processing", "Finished processing", "Start loading", "Finished loading", "Start ", "End " };

        foreach (var keyword in keywords)
            StartKeywords[keyword] = Frmt(keyword, TextKind.Keyword);

        keywords = new[] { "Request", "Response" };

        foreach (var keyword in keywords)
            Keywords[keyword] = Frmt(keyword, TextKind.SpecialKeyword);

        keywords = new[] { "(FROM CACHE)" };

        foreach (var keyword in keywords)
            EndKeywords[keyword] = Frmt(keyword, TextKind.Keyword);

        var httpVerbs = new[] { "GET", "POST", "PUT", "DELETE", "PATCH" };

        foreach (var keyword in httpVerbs)
            Verbs[keyword] = Frmt(keyword, TextKind.HttpVerb);
    }    
}