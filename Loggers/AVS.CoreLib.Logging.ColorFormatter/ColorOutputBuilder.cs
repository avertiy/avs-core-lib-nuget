using System.Text;
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
        var colors = ColorsProvider.GetColorsFor(LogParts.Timestamp);
        Output.Append(colors.Format(timestamp));
        Output.EnsureWhitespace();
    }

    protected override void AddLogLevel()
    {
        if (!Options.IncludeLogLevel && LogLevel < LogLevel.Error)
            return;

        var logLevel = LogLevel.GetLogLevelText() + LOGLEVEL_PADDING;
        PadLength += logLevel.Length;

        if (Options.TagsBehavior == TagsBehavior.Enabled)
        {
            var colors = LogLevel.GetLogLevelColors();
            Output.Append(colors.Format(logLevel));
        }
        else
        {
            Output.Append(logLevel);
        }
    }

    protected override void AddCategory()
    {
        if (Options.CategoryFormat == CategoryFormat.None || string.IsNullOrEmpty(Category))
            return;

        PadLength += Category.Length;
        var category = Format(LogParts.Category, Category);
        Output.Append(category);
        Output.EnsureWhitespace();
    }

    protected override void AddScopeInformation(IExternalScopeProvider scopeProvider)
    {
        if (string.IsNullOrEmpty(Scopes))
            return;

        var scope = Format(LogParts.Scope, Scopes);
        Output.Append(scope);
        Output.Append(Options.SingleLine ? " " : Environment.NewLine);
    }

    protected override void AddMessageText()
    {
        if (ProcessStamps())
            return;

        var text = FormatLines();
        text = Format(LogParts.Message, text);    
        //var colorMarkup2 = ColorMarkup2Helper.HasEndLineColorMarkup(text, out var colors2, out var index);
        //if (colorMarkup2)
        //{
        //    text = text.Substring(0, index);
        //    colors = ConsoleColors.Parse(colors2);
        //}
       
        Output.Append(text);
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

        var tagProcessor = new TagProcessor(Output);
        tagProcessor.ProcessHeaderTags(Options.HeaderPadding);
        tagProcessor.ProcessColorTags();
        tagProcessor.ProcessRgbTags();
    }

    private string Format(LogParts part, string text)
    {
        if (Options.TagsBehavior != TagsBehavior.Enabled)
            return text;

        var colors = ColorsProvider.GetColorsFor(part);
        return colors.Format(text);
    }
}