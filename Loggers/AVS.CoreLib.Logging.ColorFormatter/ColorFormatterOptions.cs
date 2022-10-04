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

    public ScopeBehavior ScopeBehavior { get; set; }

    public bool IncludeCategory { get; set; }
    public bool IncludeLogLevel { get; set; } = true;
    
}

public enum ScopeBehavior
{
    Inline = 0,
    Header = 1,
}