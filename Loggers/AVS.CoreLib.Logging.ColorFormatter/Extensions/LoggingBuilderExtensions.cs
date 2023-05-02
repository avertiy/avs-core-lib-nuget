using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.Logging.ColorFormatter.Extensions;

public static class LoggingBuilderExtensions
{
    /// <summary>
    /// Adds Console logging with ColorFormatter.
    /// Usage:
    ///  <code>
    ///     services.AddLogging(builder => { builder.AddConsoleWithColorFormatter(x =>
    ///         {
    ///             x.IncludeScopes = true; // to print scope as well
    ///             x.SingleLine = true;    // the entire message written in one line
    ///             x.TimestampFormat = "T";// add timestamp label to log messages
    ///             x.UseUtcTimestamp = true;
    ///         }
    ///     );});
    /// </code>
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configure">A delegate to configure options 'TOptions' for custom formatter 'TFormatter'.</param>
    public static ILoggingBuilder AddConsoleWithColorFormatter(
        this ILoggingBuilder builder,
        Action<ColorFormatterOptions> configure)
    {
        builder.AddConsole(options => options.FormatterName = nameof(Logging.ColorFormatter));
        builder.AddConsoleFormatter<ColorFormatter, ColorFormatterOptions>(configure);
        return builder;
    }

    public static ILoggingBuilder AddConsoleWithColorFormatter(
        this ILoggingBuilder builder, string timestampFormat = "HH:mm:ss")
    {
        return builder.AddConsole(options => options.FormatterName = nameof(Logging.ColorFormatter))
            .AddConsoleFormatter<ColorFormatter, ColorFormatterOptions>(x =>
            {
                x.TimestampFormat = timestampFormat;
            });
    }
}