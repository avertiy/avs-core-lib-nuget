using System.Text;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Logging.ColorFormatter.Enums;
using AVS.CoreLib.Logging.ColorFormatter.Extensions;

namespace AVS.CoreLib.Logging.ColorFormatter.Utils;

/// <summary>
/// extracts 
/// format arguments and wrap them in color tags (highlight arguments feature)
/// </summary>
/// <code>
/// logger.LogInformation("{arg:C}", 1.022); => "<Green>$1.02</Green>"
/// i.e. arg=1.022 is formatted as currency with `C` modifier and wrapped into color tags
/// </code>
public class ArgsColorFormatter
{
    public string Message { get; set; }
    public IColorProvider ColorProvider { get; set; }
    public IReadOnlyList<KeyValuePair<string, object>> State { get; private set; }
    
    public string Format { get; private set; }
    public string[] Keys { get; private set; }
    public string[] Values { get; private set; }
    public string Output { get; private set; }

    public void Init(IReadOnlyList<KeyValuePair<string, object>> state)
    {
        State = state;
        Keys = new string[state.Count - 1];
        Values = new string[state.Count - 1];
        Format = (string)state[^1].Value;
        var startInd = 0;
        var i = 0;

        foreach (var kp in state.Take(state.Count - 1))
        {
            var key = kp.Key;
            var val = kp.Value;

            var ind = Format.IndexOf('{' + key, startInd, StringComparison.Ordinal) + 1;
            var closeArgInd = Format.IndexOf('}', ind);
            var len = closeArgInd - ind;

            var argFormat = key;
            string valueStr;
            if (key.Length == len)
            {
                valueStr = val?.ToString();
            }
            else
            {
                // allows to use with logger string format approach {arg:C} or {arg:C -Yellow} would be OK as well! 
                argFormat = Format.Substring(ind, closeArgInd - ind);
                var ii = ind + key.Length + 1;
                var frmt = Format.Substring(ii, closeArgInd - ii);
                valueStr = CustomFormat(val, frmt);
            }

            Keys[i] = argFormat;
            Values[i] = valueStr;
            i++;
            startInd = closeArgInd;
        }

        InitKeywords();
        InitVerbs();
    }

    private string CustomFormat(object val, string format)
    {
        if (val is string str)
            return str;
        
        str = string.Format($"{{0:{format}}}", val);
        if (str != format)
            return str;

        var parts = format.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Format($"{{0:{parts[0]}}}", val);
    }

    public string FormatMessage()
    {
        var sb = new StringBuilder(Format);

        for (var i = 0; i < Keys.Length; i++)
        {
            var key = Keys[i];
            var keyInBrackets = '{' + key + '}';
            var valueStr = Values[i];
            var valueObj = State[i].Value;

            if (string.IsNullOrEmpty(valueStr) || valueObj == null)
            {
                sb.Replace(keyInBrackets, valueStr);
                continue;
            }

            var colonInd = key.IndexOf(':');

            if (colonInd > 0 && Colors.TryParse(key.Substring(colonInd), out var colors))
            {
                var formattedStr = colors.FormatWithTags(valueStr);
                sb.Replace(keyInBrackets, formattedStr);
                continue;
            }

            if (valueObj is string)
            {
                var formattedStr = FormatString(valueStr);
                sb.Replace(keyInBrackets, formattedStr);
                continue;
            }

            if (valueObj.IsNumeric())
            {
                var flags = valueObj.GetNumberFlags(valueStr);
                var ind = sb.IndexOf(keyInBrackets) - 1;
                //highlight # symbol as well
                if (sb[ind] == '#' || (ind > 1 && sb[ind - 1] == '#'))
                {
                    sb.Remove(ind, 1);
                    valueStr = "#" + valueStr;
                }

                colors = ColorProvider.GetColors(flags);
                var formattedStr = colors.FormatWithTags(valueStr);
                sb.Replace(keyInBrackets, formattedStr);
                continue;
            }

            var objType = valueObj.GetObjType();

            if (objType == ObjType.Object)
            {
                var formattedStr = FormatObject(valueStr);
                sb.Replace(keyInBrackets, formattedStr);
                continue;
            }
            else
            {
                colors = ColorProvider.GetColors(objType);                
                var coloredStr = colors.FormatWithTags(valueStr);
                sb.Replace(keyInBrackets, coloredStr);
            }
        }

        Output = FrmtStartKeywords(sb.ToString());        

        return Output;
    }

    private string FormatObject(string str)
    {
        // obj
        if (str.StartsWith('{') && str.EndsWithEither('}','.','"',','))
        {
            if (str.Contains('"') && str.Contains(':'))
                return Frmt(str, TextKind.Json);

            return Frmt(str, TextKind.Brackets);
        }

        // arr
        if (str.StartsWith('[') && str.EndsWithEither(']','.','"', ','))
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
        str = FrmtEndKeywords(str);
        return Frmt(str, TextKind.None);
    }

    private string FormatString(string str)
    {
        // json obj
        if(str.StartsWith('{') && str.EndsWith('}'))
        {
            if(str.Contains('"') && str.Contains(':'))
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
        if (str.Length > ind2 && str[ind2].Either('[','{'))
        {
            var substr = str.Substring(ind2);
            str = str.Replace(substr, Frmt(substr, TextKind.Json));
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
        Colors colors = ColorProvider.GetColors(flag);
        return colors.FormatWithTags(str);
    }

    public static Dictionary<string, string> StartKeywords = new(2);
    public static Dictionary<string, string> EndKeywords = new(1);
    public static Dictionary<string, string> Verbs = new(5);

    private void InitKeywords()
    {
        if (StartKeywords.Any())
            return;

        var keywords = new[] { "Start ", "End " };

        foreach (var keyword in keywords)
            StartKeywords[keyword] = Frmt(keyword, TextKind.Keyword);

        keywords = new[] { "(FROM CACHE)" };

        foreach (var keyword in keywords)
            EndKeywords[keyword] = Frmt(keyword, TextKind.Keyword);
    }

    private void InitVerbs()
    {
        if (Verbs.Any())
            return;

        var httpVerbs = new[] { "GET", "POST", "PUT", "DELETE", "PATCH" };

        foreach (var keyword in httpVerbs)
            Verbs[keyword] = Frmt(keyword, TextKind.HttpVerb);
    }
}
