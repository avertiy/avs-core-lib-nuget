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
        {
            textWriter.WriteLine(message);
            return;
        }

        NewLineCheck:
        if (message.StartsWith(Environment.NewLine))
        {
            message = message.Remove(0, Environment.NewLine.Length);
            textWriter.WriteLine();
            goto NewLineCheck;
        }

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
        var timeStampFormat = _formatterFormatterOptions.TimestampFormat;
        messageBuilder.AddTimestamp(GetCurrentDateTime(), _formatterFormatterOptions.TimestampFormat);

        if (AnyTimeTags(message, messageBuilder))
        {
            WriteLogMessage(textWriter, messageBuilder.ToString());
            return;
        }

        messageBuilder.AddLogLevel(logEntry.LogLevel);
        
        if (message == "[level]")
        {
            WriteLogMessage(textWriter, messageBuilder.ToString());
            return;
        }

        messageBuilder.AddCategory(logEntry);

        messageBuilder.AddScopeInformation(scopeProvider);

        messageBuilder.AddMessageText(message, logEntry, x => ColorsProvider.GetColorsForArgument(x));

        messageBuilder.AddError(logEntry.Exception);

        var logMessage = messageBuilder.ToString();

        WriteLogMessage(textWriter, logMessage);
    }

    private bool AnyTimeTags(string message, LogMessageBuilder messageBuilder)
    {
        if (message == "<time/>" || message == "[time]")
        {
            messageBuilder.AddTimestamp(GetCurrentDateTime(), "T");
            return true;
        }

        if (message == "<timestamp/>" || message == "[timestamp]")
        {
            messageBuilder.AddTimestamp(GetCurrentDateTime(), "G");
            return true;
        }

        if (message == "<date/>" || message == "[date]")
        {
            messageBuilder.AddTimestamp(GetCurrentDateTime(), "d");
            return true;
        }

        return false;
    }

    //private bool AnyTags(string message, LogMessageBuilder messageBuilder)
    //{
    //    if (message == "<header>" || message == "[time]")
    //    {
    //        messageBuilder.AddTimestamp(GetCurrentDateTime(), "T");
    //        return true;
    //    }

    //    if (message == "<timestamp/>" || message == "[timestamp]")
    //    {
    //        messageBuilder.AddTimestamp(GetCurrentDateTime(), "G");
    //        return true;
    //    }

    //    if (message == "<date/>" || message == "[date]")
    //    {
    //        messageBuilder.AddTimestamp(GetCurrentDateTime(), "d");
    //        return true;
    //    }

    //    return false;
    //}

    private void WriteLogMessage(TextWriter textWriter, string message)
    {
        if (_formatterFormatterOptions.ColorBehavior == LoggerColorBehavior.Disabled)
            textWriter.WriteLine(message);
        else
        {
            var colorMarkupString = new ColorMarkupString2(message);
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