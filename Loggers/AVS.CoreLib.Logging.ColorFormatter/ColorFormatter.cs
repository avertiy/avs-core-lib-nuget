using AVS.CoreLib.Logging.ColorFormatter.Extensions;
using AVS.CoreLib.Logging.ColorFormatter.Utils;
using AVS.CoreLib.Text.Formatters.ColorMarkup;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace AVS.CoreLib.Logging.ColorFormatter;

/*
public class ColorSchemeHelper
{
    public ConsoleColors Default { get; set; } = new ConsoleColors(System.Console.ForegroundColor, System.Console.BackgroundColor);

}
*/
public class ColorFormatter : ConsoleFormatter, IDisposable
{
    private readonly IDisposable _optionsReloadToken;
    protected ColorFormatterOptions _formatterFormatterOptions;
    

    public ColorFormatter(IOptionsMonitor<ColorFormatterOptions> options)
        // case insensitive
        : base(nameof(ColorFormatter))
    {
        (_optionsReloadToken, _formatterFormatterOptions) = (options.OnChange(ReloadLoggerOptions), options.CurrentValue);
    }

    private void ReloadLoggerOptions(ColorFormatterOptions formatterOptions)
    {
        _formatterFormatterOptions = formatterOptions;
    }

    public override void Write<TState>(
        in LogEntry<TState> logEntry,
        IExternalScopeProvider scopeProvider,
        TextWriter textWriter)
    {
        var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);

        if (message is null)
            return;
        var messageBuilder = new LogMessageBuilder()
        {
            IncludeScopes = _formatterFormatterOptions.IncludeScopes,
            ScopeBehavior = _formatterFormatterOptions.ScopeBehavior,
            ColorBehavior = _formatterFormatterOptions.ColorBehavior,
            SingleLine = _formatterFormatterOptions.SingleLine,
            IncludeLogLevel = _formatterFormatterOptions.IncludeLogLevel
        };

        messageBuilder.AddPrefix(_formatterFormatterOptions.CustomPrefix);
        messageBuilder.AddTimestamp(GetCurrentDateTime(), _formatterFormatterOptions.TimestampFormat);
        messageBuilder.AddLogLevel(logEntry.LogLevel);
        messageBuilder.AddCategory(_formatterFormatterOptions.IncludeCategory, logEntry);

        messageBuilder.AddScopeInformation(scopeProvider);
        
        messageBuilder.AddMessageText(message, logEntry.LogLevel);
        messageBuilder.AddError(logEntry.Exception);

        var logMessage = messageBuilder.ToString();

        if (_formatterFormatterOptions.ColorBehavior == LoggerColorBehavior.Disabled)
            textWriter.WriteLine(logMessage);
        else
        {
            var colorMarkupString = new ColorMarkupString(logMessage);
            textWriter.WriteColorMarkupString(colorMarkupString);
            textWriter.WriteLine();
        }
    }

    private DateTimeOffset GetCurrentDateTime()
    {
        return _formatterFormatterOptions.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;
    }

    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
    }
}