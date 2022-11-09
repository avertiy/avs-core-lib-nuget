using System.Text;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Logging.ColorFormatter.Extensions;
using AVS.CoreLib.Logging.ColorFormatter.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace AVS.CoreLib.Logging.ColorFormatter;


public interface IOutputBuilder
{
    void Init<T>(in LogEntry<T> logEntry, IExternalScopeProvider scopeProvider);
    string Build();
}


public class OutputBuilder : IOutputBuilder
{
    protected const string LOGLEVEL_PADDING = ": ";
    protected int PadLength { get; set; }
    protected StringBuilder Output { get; set; }
    protected string Message { get; set; }
    protected LogLevel LogLevel { get; set; }
    protected string Category { get; set; }
    protected string Scopes { get; set; }
    protected Exception Error { get; set; }

    public ColorFormatterOptions Options { get; set; }

    public void Init<T>(in LogEntry<T> logEntry, IExternalScopeProvider scopeProvider)
    {
        PadLength = 0;
        Output = new StringBuilder();
        LogLevel = logEntry.LogLevel;
        Error = logEntry.Exception;
        InitCategory(logEntry);
        InitScopes(scopeProvider);
        InitMessage(logEntry);
    }

    protected virtual void InitMessage<T>(in LogEntry<T> logEntry)
    {
        Message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);
        //Message = Message.StripColorMarkup().StripEndLineColorMarkup();
    }

    public override string ToString()
    {
        return Output.ToString();
    }

    public string Build()
    {
        if (string.IsNullOrWhiteSpace(Message))
        {
            Output.AppendLine(Message);
        }
        else
        {
            PadLength = 0;
            AddTimestamp();
            AddLogLevel();
            AddPrefix();
            AddCategory();
            AddMessageText();
            AddError();
            ProcessTags();
        }

        return Output.ToString();
    }

    protected virtual void AddTimestamp()
    {
        if (string.IsNullOrEmpty(Options.TimestampFormat))
            return;

        var timestamp = GetCurrentDateTime().ToString(Options.TimestampFormat);
        PadLength += timestamp.Length+1;
        Output.Append(timestamp);
        Output.EnsureWhitespace();
    }

    protected virtual void ProcessTags()
    {
        if(Options.TagsBehavior == TagsBehavior.Disabled)
            return;

        var removedTagsCount = Output.StripTags();
    }
    
    protected virtual void AddMessageText()
    {
        if (ProcessStamps())
            return;

        var text = FormatLines();
        Output.AppendLine(text);
    }

    protected virtual string FormatLines()
    {
        var startIndex = 0;
        NewLineCheck:

        if (Message.IndexOf(Environment.NewLine, startIndex, StringComparison.Ordinal) == 0)
        {
            startIndex += Environment.NewLine.Length;
            Output.Insert(0, Environment.NewLine);
            goto NewLineCheck;
        }

        var text = Message.Substring(startIndex);

        if (Options.SingleLine)
            text = text.Replace(Environment.NewLine, " ");
        else
        {
            var padding = new string(' ', PadLength);
            var newLineWithPadding = Environment.NewLine + padding;
            text = $"{text.Replace(Environment.NewLine, newLineWithPadding)}{Environment.NewLine}";
        }

        return text;
    }

    protected virtual void AddError()
    {
        if (Error == null)
            return;

        Output.AppendLine(Error.ToString());
    }


    protected virtual void AddScopeInformation(IExternalScopeProvider scopeProvider)
    {
        if(string.IsNullOrEmpty(Scopes))
            return;

        Output.Append(Scopes);
        //_sb.EnsureWhitespace();
        Output.Append(Options.SingleLine ? " " : Environment.NewLine);
    }

    protected virtual void AddCategory()
    {
        if (Options.CategoryFormat == CategoryFormat.None || string.IsNullOrEmpty(Category))
            return;

        PadLength += Category.Length;
        Output.Append(Category);
        Output.EnsureWhitespace();
    }

    protected virtual bool ProcessStamps()
    {
        if (StampHelper.MatchTimeStamp(Message, GetCurrentDateTime(), out var stamp))
        {
            Output.AppendLine(stamp);
            return true;
        }
        return false;
    }

    protected virtual void AddPrefix()
    {
        if (string.IsNullOrEmpty(Options.CustomPrefix))
            return;

        Output.Append(Options.CustomPrefix);
        PadLength += Options.CustomPrefix.Length;
        Output.EnsureWhitespace();
    }

    protected virtual void AddLogLevel()
    {
        if (!Options.IncludeLogLevel && LogLevel < LogLevel.Error)
            return;

        var logLevel = LogLevel.GetLogLevelText() + LOGLEVEL_PADDING;
        PadLength+=logLevel.Length;
        Output.Append(logLevel);
        Output.EnsureWhitespace();
    }



    protected DateTimeOffset GetCurrentDateTime()
    {
        return Options.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;
    }

    protected void InitScopes(IExternalScopeProvider scopeProvider)
    {
        if (!Options.IncludeScopes || scopeProvider == null)
            return;

        var scopes = scopeProvider.GetAllScopes();
        if (scopes.Count == 0)
            return;

        Scopes = scopes.ToJsonString();
    }

    protected void InitCategory<T>(LogEntry<T> logEntry)
    {
        if (Options.CategoryFormat == CategoryFormat.None)
            return;

        var eventId = logEntry.EventId.Id;
        // Example:
        // 12:05:00 info: ConsoleApp.Program[10]

        Category = logEntry.Category;
        if (Options.CategoryFormat == CategoryFormat.Name)
        {
            var ind = logEntry.Category.LastIndexOf('.');
            if (ind + 1 < logEntry.Category.Length)
                Category = logEntry.Category.Substring(ind + 1);
        }

        if (eventId > 0)
            Category = $"{Category}[{eventId}]";

    }
}