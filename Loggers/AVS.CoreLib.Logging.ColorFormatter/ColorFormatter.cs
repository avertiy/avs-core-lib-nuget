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
    protected ColorFormatterOptions _formatterFormatterOptions;
    public static IColorsProvider ColorsProvider = new ColorsProvider();
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

    protected virtual string FormatMessage<T>(in LogEntry<T> logEntry)
    {
        var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);
        if (_formatterFormatterOptions.ColorBehavior == LoggerColorBehavior.Disabled)
        {
            message = message.StripColorMarkup().StripEndLineColorMarkup();
            return message;
        }

        return message;
    }

    public override void Write<T>(
        in LogEntry<T> logEntry,
        IExternalScopeProvider scopeProvider,
        TextWriter textWriter)
    {
        var message = FormatMessage(logEntry);

        if (string.IsNullOrEmpty(message))
            return;

        var messageBuilder = new LogMessageBuilder()
        {
            IncludeScopes = _formatterFormatterOptions.IncludeScopes,
            CategoryFormat = _formatterFormatterOptions.CategoryFormat,
            ArgsColorFormat = _formatterFormatterOptions.ArgsColorFormat,
            ScopeBehavior = _formatterFormatterOptions.ScopeBehavior,
            ColorBehavior = _formatterFormatterOptions.ColorBehavior,
            SingleLine = _formatterFormatterOptions.SingleLine,
            IncludeLogLevel = _formatterFormatterOptions.IncludeLogLevel
        };

        messageBuilder.AddPrefix(_formatterFormatterOptions.CustomPrefix);
        messageBuilder.AddTimestamp(GetCurrentDateTime(), _formatterFormatterOptions.TimestampFormat);
        messageBuilder.AddLogLevel(logEntry.LogLevel);
        messageBuilder.AddCategory(logEntry);

        messageBuilder.AddScopeInformation(scopeProvider);

        messageBuilder.AddMessageText(message, logEntry, x => ColorsProvider.GetColorsForArgument(x));

        messageBuilder.AddError(logEntry.Exception);

        var logMessage = messageBuilder.ToString();

        if (_formatterFormatterOptions.ColorBehavior == LoggerColorBehavior.Disabled)
            textWriter.WriteLine(logMessage);
        else
        {
            var colorMarkupString = new ColorMarkupString2(logMessage);
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