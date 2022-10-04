using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.Logging.ColorFormatter.Extensions;

public static class LoggingBuilderExtensions
{
    public static ILoggingBuilder AddConsoleWithColorFormatter(
        this ILoggingBuilder builder,
        Action<ColorFormatterOptions> configure)
    {
        return builder.AddConsole(options => options.FormatterName = nameof(Logging.ColorFormatter))
            .AddConsoleFormatter<Logging.ColorFormatter.ColorFormatter, ColorFormatterOptions>(configure);
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