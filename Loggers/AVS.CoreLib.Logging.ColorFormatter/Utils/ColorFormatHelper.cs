using System.Text;
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
                argFormat = Format.Substring(ind, closeArgInd - ind);
                var ii = ind + key.Length + 1;
                var frmt = Format.Substring(ii, closeArgInd - ii);
                valueStr = string.Format($"{{0:{frmt}}}", val);
            }

            Keys[i] = argFormat;
            Values[i] = valueStr;
            i++;
            startInd = closeArgInd;
        }

        //Keys = state.Select(x => "{" + x.Key + "}").Take(state.Count - 1).ToArray();
        
        //var parts = Format.Split(Keys, StringSplitOptions.None);
        //Values = message.Split(parts.Where(x => x.Length > 1).ToArray(), StringSplitOptions.RemoveEmptyEntries);
        
        //if (Values.Length < Keys.Length)
        //{
        //    var k = state[0].Key;
        //    var v = state[1].Value;
        //}

       
    }

    public string FormatMessage()
    {
        var colorProvider = ColorsProvider ?? new ColorsProvider();
        var sb = new StringBuilder(Format);

        for (var i = 0; i < Keys.Length; i++)
        {
            var key = Keys[i];
            var value = Values[i];
            var type = GetArgumentType(value);
            var colors = colorProvider.GetColorsForArgument(type);

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

            var coloredStr = colors.Format(value);
            sb.Replace($"{{{key}}}", coloredStr);
        }

        Output = sb.ToString();
        return Output;
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

/*
public static class ColorFormatHelper
{
    /// <summary>
    /// colorize arguments within the formatted message
    /// </summary>
    public static string Format(string message, string originalFormat, string[] keys, Func<ArgumentType, ConsoleColors> getColors)
    {
        //message: "<Red>red text #1 text 0.123456789</Red>"
        //originalFormat:  "<Red>some text #{arg1} text {arg2}</Red>"
        //expected output: "<Red>some text <Blue>1</Blue> text <Green>0.123456789</Green></Red>"
        var parts = originalFormat.Split(keys, StringSplitOptions.None);

        //parts[0]: "<Red>some text "
        //parts[1]: " text "
        //parts[2]: 
        //parts[3]: "</Red>"

        var values = message.Split(parts.Where(x => x.Length > 1).ToArray(), StringSplitOptions.RemoveEmptyEntries);
        //values: [] {"1", "0.123456789"}
        if (keys.Length != values.Length)
            throw new Exception("keys.Length != values.Length");

        var sb = new StringBuilder(originalFormat);

        for (var i = 0; i < keys.Length; i++)
        {
            var key = keys[i];
            var value = values[i];
            var type = GetArgumentType(value);
            var colors = getColors(type);

            if (type == ArgumentType.Numeric)
            {
                var ind = sb.IndexOf(key) - 2;
                //highlight # symbol as well
                if (sb[ind] == '#')
                {
                    sb.Remove(ind, 1);
                    value = "#" + value;
                }
            }

            var coloredStr = colors.Format(value);
            sb.Replace($"{{{key}}}", coloredStr);
        }

        return sb.ToString();
    }

    
}
*/