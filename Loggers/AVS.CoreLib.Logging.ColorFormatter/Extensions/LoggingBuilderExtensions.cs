using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.Logging.ColorFormatter.Extensions;

public static class LoggingBuilderExtensions
{
    /// <summary>
    /// Adds Console logging with ColorFormatter
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
    /// <param name="configure">A delegate to configure options 'TOptions' for custom formatter 'TFormatter'.</param>
    public static ILoggingBuilder AddConsoleWithColorFormatter(
        this ILoggingBuilder builder,
        Action<ColorFormatterOptions> configure)
    {
        builder.AddConsole(options => options.FormatterName = nameof(Logging.ColorFormatter));
        builder.AddConsoleFormatter<Logging.ColorFormatter.ColorFormatter, ColorFormatterOptions>(configure);
        return builder;
    }

    public static ILoggingBuilder AddConsoleWithColorFormatter(
        this ILoggingBuilder builder, string timestampFormat = "HH:mm:ss")
    {
        return builder.AddConsole(options => options.FormatterName = nameof(Logging.ColorFormatter))
            .AddConsoleFormatter<Logging.ColorFormatter.ColorFormatter, ColorFormatterOptions>(x =>
            {
                x.TimestampFormat = timestampFormat;
            });
    }
}