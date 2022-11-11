using System.Text;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Logging.ColorFormatter.Enums;
using AVS.CoreLib.Logging.ColorFormatter.Extensions;
using Microsoft.VisualBasic.CompilerServices;

namespace AVS.CoreLib.Logging.ColorFormatter.Utils;

public class ArgsColorFormatter
{
    public IColorsProvider ColorsProvider { get; set; }
    public string Message { get; set; }
    public IReadOnlyList<KeyValuePair<string, object>> State { get; set; }
    public string Format { get; set; }
    public string[] Keys { get; set; }
    public string[] Values { get; set; }
    public string Output { get; set; }

    public ArgsColorFormatter(string message, object state)
    {
        Init(message, state as IReadOnlyList<KeyValuePair<string, object>>);
    }

    private void Init(string message, IReadOnlyList<KeyValuePair<string, object>> state)
    {
        Message = message;
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

            var ind = Format.IndexOf('{'+key, startInd, StringComparison.Ordinal)+1;
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
            var value = Values[i];

            if (string.IsNullOrEmpty(value))
            {
                sb.Replace(keyInBrackets, value);
                continue;
            }

            var colonInd = key.IndexOf(':');
            if (colonInd <= 0 || !Colors.TryParse(key.Substring(colonInd), out var colors))
            {
                var obj = State[i].Value;
                var (type, flags) = obj.GetTypeAndFlags(value);
                colors = GetColors(key, type, flags);

                if (type == ObjType.Integer)
                {
                    var ind = sb.IndexOf(keyInBrackets) - 1;
                    //highlight # symbol as well
                    if (sb[ind] == '#' || (ind > 1 && sb[ind - 1] == '#'))
                    {
                        sb.Remove(ind, 1);
                        value = "#" + value;
                    }
                }
            }
            
            var coloredStr = colors.FormatWithTags(value);
            sb.Replace(keyInBrackets, coloredStr);
        }

        Output = sb.ToString();
        return Output;
    }

    private Colors GetColors(string argKey, ObjType type, FormatFlags flags)
    {
        var colorProvider = ColorsProvider ?? new ColorsProvider();
        var colors = colorProvider.GetColorsForArgument(type, flags);
        return colors;
    }
}