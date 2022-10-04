using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using AVS.CoreLib.Logging.ColorFormatter.Extensions;
using AVS.CoreLib.Text.Extensions;
using AVS.CoreLib.Text.Formatters.ColorMarkup;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace AVS.CoreLib.Logging.ColorFormatter;

public class LogMessageBuilder
{
    private const string LOGLEVEL_PADDING = ": ";
    private static readonly string _messagePadding = new string(' ', LogLevel.Information.GetLogLevelText().Length + LOGLEVEL_PADDING.Length);
    private static readonly string _newLineWithMessagePadding = Environment.NewLine + _messagePadding;
    private readonly StringBuilder _sb = new StringBuilder();
    public LoggerColorBehavior ColorBehavior { get; set; }
    public ScopeBehavior ScopeBehavior { get; set; }
    public bool SingleLine { get; set; }
    public bool IncludeScopes { get; set; }
    public bool IncludeLogLevel { get; set; }

    public void AddPrefix(string prefix)
    {
        if (prefix == null)
            return;
        _sb.Append(prefix);
        _sb.EnsureWhitespace();
    }

    public void AddTimestamp(DateTimeOffset dateTimeOffset, string format = null)
    {
        if (string.IsNullOrEmpty(format))
            return;
        var timestamp = dateTimeOffset.ToString(format);
        _sb.Append(timestamp);
        _sb.EnsureWhitespace();
    }

    public void AddLogLevel(LogLevel logLevel)
    {
        if (!IncludeLogLevel && logLevel < LogLevel.Error)
            return;

        var logLevelString = logLevel.GetLogLevelText() + LOGLEVEL_PADDING;
        var colors = logLevel.GetConsoleColors(ColorBehavior);
        Append(logLevelString, colors);
        _sb.EnsureWhitespace();
    }

    public void AddCategory<TState>(bool includeCategory, in LogEntry<TState> logEntry)
    {
        if (!includeCategory) 
            return;

        // Example:
        // 12:05:00 info: ConsoleApp.Program[10]

        var eventId = logEntry.EventId.Id;
        var str = $"{logEntry.Category}[{eventId}]";
        Append(str, ConsoleColors.DarkGray);
        _sb.EnsureWhitespace();
    }

    public void AddScopeInformation(IExternalScopeProvider scopeProvider)
    {
        if (!IncludeScopes || scopeProvider == null)
            return;

        var scopes = scopeProvider.GetAllScopes();
        if (scopes.Count == 0)
            return;

        var json = scopes.ToJsonString();
        Append(json, ConsoleColors.Cyan);
        _sb.Append(SingleLine ? " " : Environment.NewLine);
    }

    public void AddMessageText(string message, LogLevel logLevel)
    {
        if (string.IsNullOrEmpty(message))
            return;

        var text = SingleLine
            ? message.Replace(Environment.NewLine, " ")
            : $"{_messagePadding}{message.Replace(Environment.NewLine, _newLineWithMessagePadding)}{Environment.NewLine}";

        var colors = logLevel == LogLevel.Debug
                ? ConsoleColors.DarkGray
                : ConsoleColors.Gray;

        if (SingleLine)
            Append2(text, colors);
        else
            Append2(text, colors);
    }

    public void AddError(Exception error)
    {
        if (error == null)
            return;

        var text = error.ToString();
        Append(text, ConsoleColors.Error);
    }

    private void Append(string text, ConsoleColors colors)
    {
        if (ColorBehavior == LoggerColorBehavior.Disabled)
            _sb.Append(text);
        else
            _sb.Append(colors.FormatWithColors(text));
    }

    private void Append2(string text, ConsoleColors colors)
    {
        var colorMarkup2 = text.HasColorMarkup2(out var colors2, out var index);
        var colorMarkup = text.HasColorMarkup();


        if (ColorBehavior == LoggerColorBehavior.Disabled)
        {
            text = colorMarkup ? text.StripColorMarkup() : text;
            text = colorMarkup2 ? text.StripColorMarkup2() : text;
            _sb.Append(text);
            return;
        }

        if (colorMarkup2)
        {
            text = text.Substring(0, index);
            colors = ConsoleColors.Parse(colors2);
        }

        if (colorMarkup)
        {
            _sb.Append(ColorMarkupHelper.ColorizePlainText(text, colors.Foreground));
        }
        else
        {
            // highlight in green json like text 
            //todo make it as helper and get all matches not first one
            var jsonRegex = new Regex("(?<json>{.*})");
            var match = jsonRegex.Match(text);
            if (match.Success)
            {
                var plainText = text.Substring(0, match.Index);
                var json = match.Groups["json"].Value;
                var restText = text.Substring(match.Index + json.Length);

                text = $"{colors.FormatWithColors(plainText)}{ConsoleColors.Cyan.FormatWithColors(json)}{restText}";
                _sb.Append(text);
            }
            else
                _sb.Append(colors.FormatWithColors(text));
        }
    }

    public override string ToString()
    {
        return _sb.ToString();
    }
}