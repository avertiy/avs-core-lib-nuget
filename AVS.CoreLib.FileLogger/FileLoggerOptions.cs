namespace AVS.CoreLib.FileLogger
{
    public class FileLoggerOptions
    {
        public string RootPath { get; set; }
        public string BasePath { get; set; } = "Logs";
        public bool IncludeTimeStamp { get; set; } = true;
        public string DateFormat { get; set; } = "HH:mm:ss.fff";
        public bool UseCurlyBracketsForScope { get; set; }
        public bool PrintLoggerName { get; set; } = true;
    }
}