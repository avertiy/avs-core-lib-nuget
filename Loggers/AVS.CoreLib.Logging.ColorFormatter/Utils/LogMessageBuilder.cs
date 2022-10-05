using System.Text;
using System.Text.RegularExpressions;
using AVS.CoreLib.Logging.ColorFormatter.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace AVS.CoreLib.Logging.ColorFormatter.Utils;

public class LogMessageBuilder
{
    private const string LOGLEVEL_PADDING = ": ";
    private static readonly string _messagePadding = new string(' ', LogLevel.Information.GetLogLevelText().Length + LOGLEVEL_PADDING.Length);
    private static readonly string _newLineWithMessagePadding = Environment.NewLine + _messagePadding;
    private readonly StringBuilder _sb = new StringBuilder();
    public LoggerColorBehavior ColorBehavior { get; set; }
    public ScopeBehavior ScopeBehavior { get; set; }
    public CategoryFormat CategoryFormat { get; set; }
    public ArgsColorFormat ArgsColorFormat { get; set; }
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

    public void AddCategory<TState>(in LogEntry<TState> logEntry)
    {
        if (CategoryFormat == CategoryFormat.None || string.IsNullOrEmpty(logEntry.Category))
            return;

        var eventId = logEntry.EventId.Id;
        // Example:
        // 12:05:00 info: ConsoleApp.Program[10]

        var category = logEntry.Category;
        if (CategoryFormat == CategoryFormat.Name)
        {
            var ind = logEntry.Category.LastIndexOf('.');
            if (ind + 1 < logEntry.Category.Length)
                category = logEntry.Category.Substring(ind + 1);
        }

        if (eventId > 0)
            category = $"{category}[{eventId}]";

        Append(category, ConsoleColors.Category);
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
        Append(json, ConsoleColors.Scope);
        _sb.Append(SingleLine ? " " : Environment.NewLine);
    }

    public void AddMessageText<T>(string message, LogEntry<T> logEntry, Func<ArgumentType, ConsoleColors> colorizeArgument)
    {
        if (string.IsNullOrEmpty(message))
            return;

        var text = SingleLine
            ? message.Replace(Environment.NewLine, " ")
            : $"{_messagePadding}{message.Replace(Environment.NewLine, _newLineWithMessagePadding)}{Environment.NewLine}";


        var colors = logEntry.LogLevel == LogLevel.Debug
            ? ConsoleColors.DarkGray
            : ConsoleColors.Gray;

        var colorMarkup2 = ColorMarkup2Helper.HasEndLineColorMarkup(text, out var colors2, out var index);
        if (colorMarkup2)
        {
            text = text.Substring(0, index);
            colors = ConsoleColors.Parse(colors2);
        }
        else if (ArgsColorFormat == ArgsColorFormat.Auto && logEntry.State is IReadOnlyList<KeyValuePair<string, object>> { Count: > 1 } list)
        {
            var keys = list.Select(x => "{" + x.Key + "}").ToArray();
            var format = (string)list[^1].Value;
            text = ColorMarkup2Helper.AddColorMarkup(text, format, keys, colorizeArgument);
        }

        Append2(text, colors);
    }



    public void AddError(Exception error)
    {
        if (error == null)
            return;

        var text = error.ToString();
        Append(text, ConsoleColors.Error);
    }


    private void Append(string text, ConsoleColor foreground)
    {
        if (ColorBehavior == LoggerColorBehavior.Disabled)
            _sb.Append(text);
        else
            _sb.Append(text.FormatWithColorMarkup(foreground));
    }

    private void Append(string text, ConsoleColors colors)
    {
        if (ColorBehavior == LoggerColorBehavior.Disabled)
            _sb.Append(text);
        else
            _sb.Append(colors.FormatWithColorMarkup(text));
    }

    private void Append2(string text, ConsoleColors colors)
    {
        var colorMarkup = ColorMarkup2Helper.HasColorMarkup(text);

        if (ColorBehavior == LoggerColorBehavior.Disabled)
        {
            text = colorMarkup ? text.StripColorMarkup() : text;
            _sb.Append(text);
            return;
        }

        if (colorMarkup)
            _sb.Append(ColorMarkup2Helper.ColorizePlainText(text, colors.Foreground));
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

                text = $"{colors.FormatWithColorMarkup(plainText)}{ConsoleColors.Cyan.FormatWithColorMarkup(json)}{restText}";
                _sb.Append(text);
            }
            else
                _sb.Append(colors.FormatWithColorMarkup(text));
        }
    }

    public override string ToString()
    {
        return _sb.ToString();
    }
}