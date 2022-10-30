using System.Text;
using AVS.CoreLib.Logging.ColorFormatter.Enums;
using AVS.CoreLib.Logging.ColorFormatter.Extensions;

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
                valueStr = val.ToString();
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
            var value = Values[i];
            var type = GetArgumentType(value);

            

            if (type == ArgType.Numeric)
            {
                var ind = sb.IndexOf(key) - 2;
                //highlight # symbol as well
                if (sb[ind] == '#')
                {
                    sb.Remove(ind, 1);
                    value = "#" + value;
                }
            }

            var colors = GetColors(key, type);
            var coloredStr = colors.FormatWithTags(value);
            sb.Replace($"{{{key}}}", coloredStr);
        }

        Output = sb.ToString();
        return Output;
    }

    private ConsoleColors GetColors(string key, ArgType type)
    {
        var colonInd = key.IndexOf(':');

        if (colonInd > 0 && ConsoleColors.TryParse(key.Substring(colonInd), out var markupColors))
        {
            return markupColors;
        }

        var colorProvider = ColorsProvider ?? new ColorsProvider();
        var colors = colorProvider.GetColorsForArgument(type);
        return colors;
    }

    public static ArgType GetArgumentType(string arg)
    {
        if (arg == null)
            throw new ArgumentNullException();

        if (arg.StartsWith("[") && arg.EndsWith("]"))
        {
            return ArgType.Array;
        }

        if (arg.StartsWith("{") && arg.EndsWith("}"))
            return ArgType.TextJson;

        if (arg.Contains("%"))
            return ArgType.Percentage;

        if (arg.Contains("$") || arg.Contains("USD") || arg.Contains("EUR") || arg.Contains("UAH"))
        {
            return arg[0] == '(' && arg[^1] == ')' ? ArgType.CashNegative : ArgType.Cash;
        }

        if (double.TryParse(arg, out var d))
        {
            return d >= 0 ? ArgType.Numeric : ArgType.NumericNegative;
        }

        if (DateTime.TryParse(arg, out var date))
            return ArgType.Date;

        if (arg.Length < 25)
            return ArgType.String;

        return ArgType.Text;
    }
}