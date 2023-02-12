using Microsoft.Extensions.Logging.Console;

namespace AVS.CoreLib.Logging.ColorFormatter;

public class ColorFormatterOptions : ConsoleFormatterOptions
{
    public string CustomPrefix { get; set; }

    /// <summary>
    /// Determines when to use color when logging messages.
    /// </summary>
    public LoggerColorBehavior ColorBehavior { get; set; }

    /// <summary>
    /// When <see langword="true" />, the entire message gets logged in a single line.
    /// </summary>
    public bool SingleLine { get; set; } = true;
    public ScopeBehavior ScopeBehavior { get; set; } = ScopeBehavior.Inline;
    public CategoryFormat CategoryFormat { get; set; } = CategoryFormat.Name;
    public ArgsColorFormat ArgsColorFormat { get; set; } = ArgsColorFormat.Auto;
    public bool IncludeLogLevel { get; set; } = true;
    public TagsBehavior TagsBehavior { get; set; }

    //public string HeaderPadding { get; set; }

    public static implicit operator ColorFormatterOptions((bool singleLine, string timeFormat)
            options)
    {
        return new ColorFormatterOptions()
        {
            SingleLine = options.singleLine,
            TimestampFormat = options.timeFormat,
        };
    }

    public static implicit operator ColorFormatterOptions((bool singleLine, string timeFormat, bool logLevel) options)
    {
        return new ColorFormatterOptions()
        {
            SingleLine = options.singleLine,
            TimestampFormat = options.timeFormat,
            IncludeLogLevel = options.logLevel,
        };
    }

    public static implicit operator ColorFormatterOptions(
        (bool singleLine, bool logLevel, string timeFormat, bool utcTime)
            options)
    {
        return new ColorFormatterOptions()
        {
            SingleLine = options.singleLine,
            TimestampFormat = options.timeFormat,
            UseUtcTimestamp = options.utcTime,
            IncludeLogLevel = options.logLevel,
        };
    }

    public static implicit operator ColorFormatterOptions(
        (bool singleLine, bool logLevel, string timeFormat, bool utcTime, bool scopes, ScopeBehavior scopeBehavior)
            options)
    {
        return new ColorFormatterOptions()
        {
            IncludeScopes = options.scopes,
            SingleLine = options.singleLine,
            TimestampFormat = options.timeFormat,
            UseUtcTimestamp = options.utcTime,
            ScopeBehavior = options.scopeBehavior,
            IncludeLogLevel = options.logLevel,
        };
    }
}

public enum TagsBehavior
{
    Enabled = 0,
    Disabled = 1,
    StripTags = 2,
}

public enum ScopeBehavior
{
    /// <summary>
    /// sets scope to be printed inline with the log 
    /// </summary>
    Inline = 0,
    /// <summary>
    /// sets scope to be printed like a header for a log or a group of logs
    /// </summary>
    Header = 1,
}

public enum ArgsColorFormat
{
    None = 0,
    Auto = 1
}

public enum CategoryFormat
{
    None = 0,
    /// <summary>
    /// logger name without namespace e.g. instead of Namespace.Service1[0] => Service1 or if eventId not 0 Service1[10] 
    /// </summary>
    Name = 1,
    /// <summary>
    /// full logger name Namespace.Service1[0]
    /// </summary>
    FullName = 2
}