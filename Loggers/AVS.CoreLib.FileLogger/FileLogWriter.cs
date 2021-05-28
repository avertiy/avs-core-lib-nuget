using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AVS.CoreLib.AbstractLogger;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AVS.CoreLib.FileLogger
{
    public class FileLogWriter : ILogWriter
    {
        private readonly StringBuilder _sb = new StringBuilder();
        private static readonly object _lock = new object();
        private bool _newLineFlag = false;
        public IOptionsMonitor<FileLoggerOptions> Options { get; }
        private string LogsPath { get; }
        public FileLogWriter(IOptionsMonitor<FileLoggerOptions> options)
        {
            Options = options;
            var optionsValue = Options.CurrentValue;
            if (optionsValue.RootPath != null)
            {
                LogsPath = Path.Combine(optionsValue.RootPath, optionsValue.BasePath);
            }
            else
            {
                LogsPath = optionsValue.BasePath ?? "Logs";
            }
        }

        public void WriteLine(bool combineEmptyLines = true)
        {
            if (!_newLineFlag)
            {
                _sb.AppendLine();
                _newLineFlag = true;
            }
        }

        public void Write(string str, bool endLine = true)
        {
            if (endLine)
            {
                _sb.AppendLine(str);
                _newLineFlag = true;
            }
            else
            {
                _sb.Append(str);
                _newLineFlag = false;
            }
        }

        public void Write(string logger, EventId eventId, LogLevel logLevel, string message, Exception exception = null)
        {
            var options = Options.CurrentValue;
            var sb = new StringBuilder();
            string timestamp = null, eventIdStr= null;
            if (options.IncludeTimeStamp)
            {
                timestamp = DateTimeOffset.Now.ToLocalTime().ToString(options.DateFormat, CultureInfo.InvariantCulture);
            }

            sb.Append($"{logLevel.GetLogLevelText()} {timestamp}");
            if (eventId.Id > 0 || eventId.Name != null)
            {
                sb.Append(" [").Append(eventId.ToString()).Append(']');
            }

            sb.AppendLine();
            sb.Append("     ");
            sb.Append(message);
            if (exception != null)
            {
                sb.Append("     ");
                sb.Append(exception);
                sb.AppendLine();
            }
            sb.AppendLine();
            lock (_lock)
            {
                _sb.Append(sb.ToString());
                _newLineFlag = true;
            }

            Task.Run(Flush);
        }

        protected void Flush()
        {
            if(_sb.Length == 0)
                return;

            string content = null;
            lock (_lock)
            {
                content = _sb.ToString();
                _sb.Clear();
            }

            if (content.Length == 0)
                return;

            var logFilePath = $"{LogsPath}\\{DateTime.Now:yyyy.MM.dd}\\";
            try
            {
                if (!Directory.Exists(logFilePath))
                {
                    Directory.CreateDirectory(logFilePath);
                }
                //open or create file
                using (var sw = File.AppendText($"{logFilePath}log-{DateTime.Now:HH}.log"))
                {
                    sw.Write(content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}