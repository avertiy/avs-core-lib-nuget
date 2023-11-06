using AVS.CoreLib.Logging.ColorFormatter.Utils;
using Microsoft.Extensions.Logging.Abstractions;

namespace AVS.CoreLib.Logging.ColorFormatter.Extensions;

public static class LogEntryExtensions
{
    public static string GetMessage<T>(this LogEntry<T> logEntry, ArgsColorFormat format, IColorProvider colorProvider)
    {
        //to-do feature:pipe formatters e.g. {arg:|U} - upper case, {arg:|L} - lower case
        //note default Formatter throws FormatException when adding custom formatters like {arg:|U}
        //so it needs to be replace with a PipeFormatter that will handle pipe format modifiers
        //state = PipeFormatter.Invoke(state)
        var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);

        if (format == ArgsColorFormat.Auto && logEntry.State is IReadOnlyList<KeyValuePair<string, object>> state)
        {
            // formatter highlight arguments wrapping them in color tags
            // (i) based on color modifier e.g. "{arg:Red}" => <Red>...</Red>
            // (ii) analizing argument e.g. "{1.022:C}" => $1.02 is a currency value we want such values to be a green color: "<Green>$1.02</Green>"
            // or "{arg.ToJson()}" we want json to be Cyan color  => <Cyan>...json..</Cyan>

            var formatter = new ArgsColorFormatter() { Message = message, ColorProvider = colorProvider };
            formatter.Init(state);
            message = formatter.FormatMessage();
        }

        return message;
    }
}