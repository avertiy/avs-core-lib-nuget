using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.Logging.ColorFormatter.Extensions;
/*
public static class LoggerExtensions
{
    /// <summary>
    /// Formats and writes an informational log message colorized into <see cref="LogColors.H1Color"/>
    /// </summary>
    public static void H1(this ILogger logger, string message, params object[] args)
    {
        var log = message.Colorize(LogColors.Instance.H1Color);
        logger.LogInformation(log, args);
    }

    /// <summary>
    /// Formats and writes an informational log message colorized into <see cref="LogColors.H2Color"/>
    /// </summary>
    public static void H2(this ILogger logger, string message, params object[] args)
    {
        var log = message.Colorize(LogColors.Instance.H2Color);
        logger.LogInformation(log, args);
    }

    /// <summary>
    /// Formats and writes an informational log message colorized into <see cref="LogColors.H3Color"/>
    /// </summary>
    public static void H3(this ILogger logger, string message, params object[] args)
    {
        var log = message.Colorize(LogColors.Instance.H3Color);
        logger.LogInformation(log, args);
    }

    /// <summary>
    /// Formats and writes an informational log message colorized into <see cref="LogColors.H4Color"/>
    /// </summary>
    public static void H4(this ILogger logger, string message, params object[] args)
    {
        var log = message.Colorize(LogColors.Instance.H4Color);
        logger.LogInformation(log, args);
    }
    /// <summary>
    /// Formats and writes an informational log message colorized into <see cref="LogColors.H5Color"/>
    /// </summary>
    public static void H5(this ILogger logger, string message, params object[] args)
    {
        var log = message.Colorize(LogColors.Instance.H5Color);
        logger.LogInformation(log, args);
    }
}

public class LogColors
{
    private static LogColors _instance;
    public static LogColors Instance => _instance ??= new LogColors();

    public ConsoleColor H1Color { get; set; } = ConsoleColor.Yellow;
    public ConsoleColor H2Color { get; set; } = ConsoleColor.Cyan;
    public ConsoleColor H3Color { get; set; } = ConsoleColor.Green;
    public ConsoleColor H4Color { get; set; } = ConsoleColor.Blue;
    public ConsoleColor H5Color { get; set; } = ConsoleColor.DarkGray;
}
*/
/*
public class LoggerWrapper : ILogger
{
    private readonly ILogger _logger;
    public ConsoleColor Foreground { get; set; }   

    public LoggerWrapper(ILogger logger)
    {
        _logger = logger;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        _logger.Log(logLevel, eventId, state, exception, formatter);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _logger.IsEnabled(logLevel);
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return _logger.BeginScope(state);
    }

    public LoggerWrapper Yellow
}
*/