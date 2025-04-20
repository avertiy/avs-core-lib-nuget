#nullable enable
using System.Text;
using AVS.CoreLib.Console.ColorFormatting;
using AVS.CoreLib.Console.ColorFormatting.Extensions;
using AVS.CoreLib.Console.ColorFormatting.Tags;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Logging.ColorFormatter.Enums;
using AVS.CoreLib.Logging.ColorFormatter.Extensions;
using AVS.CoreLib.Logging.ColorFormatter.Utils;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.Logging.ColorFormatter;

/// <summary>
/// Builds output string
/// Log message is formatted with color tags e.g. &lt;Red&gt;colored text&lt;/Red&gt; than tags are replaced with a corresponding ANSI-codes
/// Coloring is based on .NET ANSI color codes feature
/// 
/// </summary>
public class OutputBuilder
{
    protected const string LOG_LEVEL_PADDING = ": ";
    public required ColorFormatterOptions FormatterOptions { get; set; }
    public required IColorProvider ColorProvider { get; set; }

    protected int PadLength { get; set; }
    protected string? NewLines { get; set; }
    protected string? Scope { get; set; }
    protected string? Timestamp { get; set; }
    protected string? LogLevelText { get; set; }
    protected LogLevel LogLevel { get; set; }
    public string? Prefix { get; set; }
    protected string? Category { get; set; }
    protected string? Message { get; set; }
    protected string? Error { get; set; }

    protected StringBuilder Output { get; set; } = new();
    protected static string? PrevScope { get; set; }

    private static TagProcessor? _tagProcessor;
    public static TagProcessor TagProcessor
    {
        get
        {
            if (_tagProcessor == null)
            {
                var processor = new CompositeTagProcessor();
                processor.AddTagProcessor(new FormattingTagProcessor());
                processor.AddTagProcessor(new CTagProcessor());
                processor.AddTagProcessor(new RgbTagProcessor());
                _tagProcessor = processor;
            }

            return _tagProcessor;
        }

        set => _tagProcessor = value;
    }

    #region With methods
    public OutputBuilder WithTimestamp(string? timestampFormat)
    {
        if (string.IsNullOrEmpty(timestampFormat))
            return this;

        var timestamp = GetCurrentDateTime().ToString(timestampFormat);
        Timestamp = Format(LogPart.Timestamp, timestamp);
        PadLength += Timestamp!.Length + 1;
        return this;
    }

    public OutputBuilder WithCategory(string category, int eventId)
    {
        var format = FormatterOptions.CategoryFormat;
        if (format == CategoryFormat.None)
            return this;

        // Example:
        // 12:05:00 info: ConsoleApp.Program[10]

        Category = category;
        if (format == CategoryFormat.Name)
        {
            var ind = category.LastIndexOf('.');
            if (ind + 1 < category.Length)
                Category = category.Substring(ind + 1) + ':';
        }

        if (eventId > 0)
            Category = $"{Category}[{eventId}]:";

        Category = Format(LogPart.Category, Category);

        return this;
    }

    public OutputBuilder WithLogLevel(LogLevel level)
    {
        LogLevel = level;
        var text = level.GetLogLevelText();
        PadLength += text.Length;

        text = Category == null ? text : text + LOG_LEVEL_PADDING;
        LogLevelText = Format(LogPart.LogLevel, text);

        return this;
    }

    public OutputBuilder WithPrefix(string? prefix)
    {
        if (string.IsNullOrEmpty(prefix))
            return this;

        Prefix = prefix;
        PadLength += prefix.Length;
        return this;
    }

    public OutputBuilder WithScope(IExternalScopeProvider? scopeProvider)
    {
        if (scopeProvider == null || FormatterOptions.IncludeScopes == false)
            return this;

        Scope = Format(LogPart.Scope, scopeProvider.GetScope());
        return this;
    }

    public OutputBuilder WithMessage(string? message, LogLevel logLevel)
    {
        if (message == null)
            return this;

        if (logLevel < LogLevel.Error && message.StartsWith(Environment.NewLine))
        {
            Message = FormatMessage(message.TrimStart());
            NewLines = message.Substring(0, message.Length - Message.Length);
        }
        else
            Message = FormatMessage(message);

        return this;
    }

    public OutputBuilder WithError(Exception? error)
    {
        if (error == null)
            return this;

        var padding = new string(' ', 3); // padding =3 to align with stack trace 
        var errorText = error.ToString(ErrorFormat.Console, padding);
        Error = Format(LogPart.Error, errorText);

        return this;
    }

    public OutputBuilder WithHighlightedArgs(State state)
    {
        if (FormatterOptions.TagsBehavior != TagsBehavior.Enabled)
            return this;

        // args color formatter highlights arguments wrapping them in color tags
        // (i) based on color modifier e.g. "{arg:Red}" => <Red>...</Red>
        // (ii) analizing argument e.g. "{1.022:C}" => $1.02 is a currency value we want such values to be a green color: "<Green>$1.02</Green>"
        // or "{arg.ToJson()}" we want json to be Cyan color  => <Cyan>...json..</Cyan>
        var formatter = new ArgsColorFormatter(ColorProvider) { OriginalMessage = Message };
        Message = formatter.Format(state);
        return this;

    }

    private string FormatMessage(string message)
    {
        var text = message;

        if (FormatterOptions.SingleLine)
            text = text.Replace(Environment.NewLine, " ");
        else
        {
            var padding = new string(' ', PadLength);
            var newLineWithPadding = Environment.NewLine + padding;
            text = $"{text.Replace(Environment.NewLine, newLineWithPadding)}{Environment.NewLine}";
        }

        var colors = ColorProvider.GetColorsFor(LogPart.Message, LogLevel);
        return colors.FormatWithTags(text);
    }

    private string? Format(LogPart part, string? text)
    {
        if (text == null || FormatterOptions.TagsBehavior != TagsBehavior.Enabled)
            return text;

        var colors = ColorProvider.GetColorsFor(part, LogLevel);
        return colors.FormatWithTags(text);
    }

    #endregion

    public string Build()
    {
        if (string.IsNullOrWhiteSpace(Message))
            Output.AppendLine(Message);
        else
        {
            PadLength = 0;
            // build output message
            WriteNewLines();
            WriteScope();
            Write(Timestamp);
            Write(LogLevelText);
            Write(Prefix);
            Write(Category);
            WriteMessage();
            WriteError();
            // output might contain tags like color tags process them
            // process means replace them with ansi-codes or strip tags when TagsBehavior is Disabled
            ProcessTags();
        }

        return Output.ToString();
    }

    #region Write methods
    private void Write(string? str)
    {
        if (str == null)
            return;

        Output.Append(str);
        Output.EnsureWhitespace();
    }

    private void WriteScope()
    {
        if (Scope == null || Scope == PrevScope)
            return;

        Output.AppendLine();
        Output.AppendLine(Scope);
        PrevScope = Scope;
    }

    private void WriteError()
    {
        if (Error == null)
            return;

        Output.AppendLine(Error);
    }

    private void WriteNewLines()
    {
        if (NewLines != null)
            Output.Append(NewLines);
    }

    private void WriteMessage()
    {
        if (StampHelper.MatchTimeStamp(Message, GetCurrentDateTime(), out var stamp))
        {
            Output.Append(stamp);
            return;
        }

        Output.AppendLine(Message);
    }

    #endregion

    private void ProcessTags()
    {
        if (FormatterOptions.TagsBehavior == TagsBehavior.Disabled)
            return;

        if (FormatterOptions.TagsBehavior == TagsBehavior.StripTags)
        {
            var removedTags = Output.StripTags();
            return;
        }

        TagProcessor.Process(Output);


        // plain output builder strip tags
        // color output builder will replace color tags with ansi-codes
        var removedTagsCount = Output.StripTags();
    }

    private DateTimeOffset GetCurrentDateTime()
    {
        return FormatterOptions.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now;
    }
}