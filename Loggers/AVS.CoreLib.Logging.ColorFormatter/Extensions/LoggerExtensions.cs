using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.Logging.ColorFormatter.Extensions;

public static class LoggerExtensions
{
    public static void LogInformation(this ILogger logger, EventId eventId, string message, ConsoleColor foreground, params object[] args)
    {
        var log = message.Colorize(foreground);
        logger.LogInformation(eventId, log, args);
    }

    public static void LogInformation(this ILogger logger, string message, ConsoleColor foreground, params object[] args)
    {
        var log = message.Colorize(foreground);
        logger.LogInformation(log, args);
    }

    public static void LogInformation(this ILogger logger, string message, ConsoleColor foreground)
    {
        var log = message.Colorize(foreground);
        logger.LogInformation(log);
    }

    public static void LogInfoHeader(this ILogger logger, string message, params object[] args)
    {
        var log = message.Colorize(LogColors.Instance.Header);
        logger.LogInformation(log, args);
    }
}

public class LogColors
{
    private static LogColors _instance;
    public static LogColors Instance => _instance ??= new LogColors();

    public ConsoleColor Header { get; set; } = ConsoleColor.Yellow;
}