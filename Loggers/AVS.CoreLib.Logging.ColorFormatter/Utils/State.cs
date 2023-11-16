namespace AVS.CoreLib.Logging.ColorFormatter.Utils;

/// <summary>
/// Wrapper for LogEntry.State helps to extract message format, argument keys and string values
/// </summary>
public class State
{
    private IReadOnlyList<KeyValuePair<string, object>> _state;
    public string Format { get; set; }
    public string[] Keys { get; set; }
    public string[] Values { get; set; }

    public State(IReadOnlyList<KeyValuePair<string, object>> state)
    {
        _state = state;
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
    }

    public (string key, object value, string str) this[int index] => (Keys[index], _state[index].Value, Values[index]);

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

    public static bool TryParse<T>(T state, out State args)
    {
        args = default;
        if (state is IReadOnlyList<KeyValuePair<string, object>> s)
        {
            args = new State(s);            
            return true;
        }

        return false;
    }
}
