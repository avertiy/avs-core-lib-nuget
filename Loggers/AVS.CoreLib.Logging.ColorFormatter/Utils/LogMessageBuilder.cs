using System.Text;
using System.Text.RegularExpressions;
using AVS.CoreLib.Logging.ColorFormatter.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace AVS.CoreLib.Logging.ColorFormatter.Utils;
/*
public class LogMessageBuilder
{
    private const string LOGLEVEL_PADDING = ": ";
    private static readonly string _messagePadding = new string(' ', LogLevel.Information.GetLogLevelText().Length + LOGLEVEL_PADDING.Length);
    private static readonly string _newLineWithMessagePadding = Environment.NewLine + _messagePadding;
    private readonly StringBuilder _sb = new StringBuilder();
    private readonly TagProcessor _tagProcessor = new TagProcessor();
    private ColorFormatterOptions _options;

    private LoggerColorBehavior ColorBehavior => _options.ColorBehavior;

    public void AddPrefix()
    {
        if (string.IsNullOrEmpty(_options.CustomPrefix))
            return;

        _sb.Append(_options.CustomPrefix);
        _sb.EnsureWhitespace();
    }

    public void AddTimestamp()
    {
        if (string.IsNullOrEmpty(_options.TimestampFormat))
            return;

        var timestamp = GetCurrentDateTime().ToString(_options.TimestampFormat);
        _sb.Append(timestamp);
        _sb.EnsureWhitespace();
    }

    private DateTimeOffset GetCurrentDateTime()
    {
        return _options.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;
    }

    public void AddLogLevel(LogLevel logLevel)
    {
        if (!_options.IncludeLogLevel && logLevel < LogLevel.Error)
            return;

        var logLevelString = logLevel.GetLogLevelText() + LOGLEVEL_PADDING;
        var colors = logLevel.GetLogLevelConsoleColors(ColorBehavior);
        Append(logLevelString, colors);
        _sb.EnsureWhitespace();
    }

    public void AddCategory<TState>(in LogEntry<TState> logEntry)
    {
        if (_options.CategoryFormat == CategoryFormat.None || string.IsNullOrEmpty(logEntry.Category))
            return;

        var eventId = logEntry.EventId.Id;
        // Example:
        // 12:05:00 info: ConsoleApp.Program[10]

        var category = logEntry.Category;
        if (_options.CategoryFormat == CategoryFormat.Name)
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
        if (!_options.IncludeScopes || scopeProvider == null)
            return;

        var scopes = scopeProvider.GetAllScopes();
        if (scopes.Count == 0)
            return;

        var json = scopes.ToJsonString();
        Append(json, ConsoleColors.Scope);
        _sb.Append(_options.SingleLine ? " " : Environment.NewLine);
    }

    public void AddMessageText<T>(string message, LogEntry<T> logEntry, Func<ArgumentType, ConsoleColors> colorizeArgument)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            _sb.Append(message);
            return;
        }

        var text = message;

        var colors = logEntry.LogLevel.GetMessageConsoleColors(ColorBehavior);

        var colorMarkup2 = ColorMarkup2Helper.HasEndLineColorMarkup(text, out var colors2, out var index);
        if (colorMarkup2)
        {
            text = text.Substring(0, index);
            colors = ConsoleColors.Parse(colors2);
        }
        else if (_options.ArgsColorFormat == ArgsColorFormat.Auto && 
                 ColorBehavior != LoggerColorBehavior.Disabled && 
                 logEntry.State is IReadOnlyList<KeyValuePair<string, object>> { Count: > 1 } list)
        {
            var keys = list.Select(x => "{" + x.Key + "}").Take(list.Count-1).ToArray();
            var format = (string)list[^1].Value;
            format = format.TrimStart('\r', '\n');
            var textWithMarkup = ColorMarkup2Helper.AddColorMarkup(text, format, keys, colorizeArgument);
            text = textWithMarkup;
        }

        text = _options.SingleLine
            ? text.Replace(Environment.NewLine, " ")
            : $"{_messagePadding}{text.Replace(Environment.NewLine, _newLineWithMessagePadding)}{Environment.NewLine}";

        
        message = _tagProcessor.Process(message, TagFormat.All);


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

    

    public void Init(ColorFormatterOptions options)
    {
        _options = options;
        _tagProcessor.UseUtcTimestamp = _options.UseUtcTimestamp;
        _tagProcessor.HeaderPadding = _options.HeaderPadding;

        //IncludeScopes = options.IncludeScopes,
        //CategoryFormat = options.CategoryFormat,
        //ArgsColorFormat = options.ArgsColorFormat,
        //ScopeBehavior = options.ScopeBehavior,
        //ColorBehavior = options.ColorBehavior,
        //SingleLine = options.SingleLine,
        //IncludeLogLevel = options.IncludeLogLevel
    }
}

*/