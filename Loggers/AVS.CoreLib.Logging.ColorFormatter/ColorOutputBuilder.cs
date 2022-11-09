using System.Text;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Logging.ColorFormatter.Enums;
using AVS.CoreLib.Logging.ColorFormatter.Extensions;
using AVS.CoreLib.Logging.ColorFormatter.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace AVS.CoreLib.Logging.ColorFormatter;

public class ColorOutputBuilder : OutputBuilder
{
    public IColorsProvider ColorsProvider { get; set; }

    protected override void InitMessage<T>(in LogEntry<T> logEntry)
    {
        var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);
        
        if (Options.ArgsColorFormat == ArgsColorFormat.Auto)
        {
            // formatter not only adds color tags to highlight arguments but also makes extended args formatting:  
            // e.g. LogInformation("{arg:C}", 1.022); => <Green>$1.02</Green>
            var formatter = new ArgsColorFormatter(message, logEntry.State) { ColorsProvider = ColorsProvider };
            message = formatter.FormatMessage();
        }

        Message = message;
    }

    protected override void AddTimestamp()
    {
        if (string.IsNullOrEmpty(Options.TimestampFormat))
            return;

        var timestamp = GetCurrentDateTime().ToString(Options.TimestampFormat);
        PadLength += timestamp.Length+1;
        var str = Format(LogPart.Timestamp, timestamp);
        Output.Append(str);
        Output.EnsureWhitespace();
    }

    protected override void AddLogLevel()
    {
        if (!Options.IncludeLogLevel && LogLevel < LogLevel.Error)
            return;

        var logLevel = LogLevel.GetLogLevelText() + LOGLEVEL_PADDING;
        PadLength += logLevel.Length;

        var str = Format(LogPart.LogLevel, logLevel);
        Output.Append(str);
    }

    protected override void AddCategory()
    {
        if (Options.CategoryFormat == CategoryFormat.None || string.IsNullOrEmpty(Category))
            return;

        PadLength += Category.Length;
        var category = Format(LogPart.Category, Category);
        Output.Append(category);
        Output.EnsureWhitespace();
    }

    protected override void AddScopeInformation(IExternalScopeProvider scopeProvider)
    {
        if (string.IsNullOrEmpty(Scopes))
            return;

        var scope = Format(LogPart.Scope, Scopes);
        Output.Append(scope);
        Output.Append(Options.SingleLine ? " " : Environment.NewLine);
    }

    protected override void AddMessageText()
    {
        if (ProcessStamps())
            return;

        var text = FormatLines();
        text = Format(LogPart.Message, text);    
       
        Output.AppendLine(text);
    }

    protected override void AddError()
    {
        if (Error == null)
            return;

        var error = Format(LogPart.Error, Error.ToString());
        Output.AppendLine(error);
    }

    protected override void ProcessTags()
    {
        if (Options.TagsBehavior == TagsBehavior.Disabled)
            return;

        if (Options.TagsBehavior == TagsBehavior.StripTags)
        {
            var removedTags = Output.StripTags();
            return;
        }

        var tagProcessor = new TagProcessor(Output, Message);
        tagProcessor.ProcessHeaderTags(Options.HeaderPadding);
        tagProcessor.ProcessColorTags();
        tagProcessor.ProcessRgbTags();
    }

    private string Format(LogPart part, string text)
    {
        if (Options.TagsBehavior != TagsBehavior.Enabled)
            return text;

        var colors = ColorsProvider.GetColorsFor(part, LogLevel);
        return colors.FormatWithTags(text);
    }
}