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
    
}

public enum ScopeBehavior
{
    Inline = 0,
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