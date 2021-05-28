using System;

namespace AVS.CoreLib.FileLogger
{
    public class FileLogEntry
    {
        public string Text { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}