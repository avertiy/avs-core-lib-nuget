namespace AVS.CoreLib.ConsoleLogger
{
    public class ConsoleLoggerOptions
    {
        public bool IncludeTimeStamp { get; set; } = true;
        public string DateFormat { get; set; } = "HH:mm:ss.fff";
        public bool UseCurlyBracketsForScope { get; set; }
        public bool PrintLoggerName { get; set; } = true;
    }
}