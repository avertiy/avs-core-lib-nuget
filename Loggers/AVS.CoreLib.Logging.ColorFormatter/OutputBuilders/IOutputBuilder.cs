using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace AVS.CoreLib.Logging.ColorFormatter.OutputBuilders;

public interface IOutputBuilder
{
    void Init<T>(in LogEntry<T> logEntry, IExternalScopeProvider scopeProvider);
    string Build();
}