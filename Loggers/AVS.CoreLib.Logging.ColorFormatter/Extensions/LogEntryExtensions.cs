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
            // formatter not only adds color tags to highlight arguments but also makes extended args formatting:  
            // e.g. LogInformation("{arg:C}", 1.022); => <Green>$1.02</Green>
            var formatter = new ArgsColorFormatter() { Message = message };
            formatter.Init(state);
            message = formatter.FormatMessage(colorProvider);
        }

        return message;
    }
}