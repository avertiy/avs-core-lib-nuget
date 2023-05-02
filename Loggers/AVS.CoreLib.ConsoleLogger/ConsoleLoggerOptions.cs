namespace AVS.CoreLib.ConsoleLogger
{
    public class ConsoleLoggerOptions
    {
        public bool UseCurlyBracketsForScope { get; set; }
        public bool PrintLoggerName { get; set; } = true;

        /// <summary>
        /// Gets or sets format string used to format timestamp in logging messages.
        /// Defaults to <c>null</c>.
        /// </summary>
        public string TimestampFormat { get; set; }

        /// <summary>
        /// Gets or sets indication whether or not UTC timezone should be used to for timestamps in logging messages. Defaults to <c>false</c>.
        /// </summary>
        public bool UseUtcTimestamp { get; set; }
        public bool SingleLine { get; set; } = true;
    }
}