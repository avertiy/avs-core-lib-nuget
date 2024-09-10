using AVS.CoreLib.Logging.ColorFormatter.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace AVS.CoreLib.Logging.ColorFormatter;

public class ColorFormatter : ConsoleFormatter, IDisposable
{
    public static IColorProvider ColorProvider = new ColorProvider();
    private readonly IDisposable _optionsReloadToken;
    private ColorFormatterOptions _options;

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
        var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);

        var builder = new OutputBuilder()
        {
            FormatterOptions = _options,
            ColorProvider = ColorProvider
        };

        builder
            .WithTimestamp(_options.TimestampFormat)
            .WithPrefix(_options.CustomPrefix)
            .WithCategory(logEntry.Category, logEntry.EventId.Id)
            .WithLogLevel(logEntry.LogLevel)
            .WithScope(scopeProvider);

        builder
            .WithMessage(message)
            .WithError(logEntry.Exception);

        if (_options.ArgsColorFormat == ArgsColorFormat.Auto && State.TryParse(logEntry.State, out var state))
            builder = builder.WithHighlightedArgs(state);

        var logMessage = builder.Build();

        // write message to memory buffer (if profiling enabled)
        ConsoleLogProfiler.Write(logMessage);

        // write output to console stream
        textWriter.Write(logMessage);
        return;
    }

    private void ReloadLoggerOptions(ColorFormatterOptions formatterOptions)
    {
        _options = formatterOptions;
    }

    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
    }
}

