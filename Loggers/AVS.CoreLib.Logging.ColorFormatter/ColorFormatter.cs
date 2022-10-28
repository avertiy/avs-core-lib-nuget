using AVS.CoreLib.Logging.ColorFormatter.ColorMakup;
using AVS.CoreLib.Logging.ColorFormatter.Extensions;
using AVS.CoreLib.Logging.ColorFormatter.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace AVS.CoreLib.Logging.ColorFormatter;

public class ColorFormatter : ConsoleFormatter, IDisposable
{
    private readonly IDisposable _optionsReloadToken;
    protected ColorFormatterOptions _options;
    public static IColorsProvider ColorsProvider = new ColorsProvider();
    public ColorFormatter(IOptionsMonitor<ColorFormatterOptions> options)
        : base(nameof(ColorFormatter))
    {
        (_optionsReloadToken, _options) = (options.OnChange(ReloadLoggerOptions), options.CurrentValue);
    }

    public override void Write<T>(
        in LogEntry<T> logEntry,
        IExternalScopeProvider scopeProvider,
        TextWriter textWriter)
    {
        var outputBuilder = GetOutputBuilder();
        outputBuilder.Init(logEntry, scopeProvider);
        var message = outputBuilder.Build();
        ConsoleLogProfiler.Write(message);
        textWriter.WriteLine(message);
        return;
    }
    private void ReloadLoggerOptions(ColorFormatterOptions formatterOptions)
    {
        _options = formatterOptions;
    }
    private IOutputBuilder GetOutputBuilder()
    {
        IOutputBuilder builder;
        if (_options.ColorBehavior == LoggerColorBehavior.Disabled)
        {
            builder = new OutputBuilder() { Options = _options };
        }
        else
        {
            builder = new ColorOutputBuilder() { ColorsProvider = ColorsProvider, Options = _options };
        }
        return builder;
    }

    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
    }
}

