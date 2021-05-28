using System;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.FileLogger.Common
{
    public static class LogLevelExtensions
    {
        public static string GetLogLevelText(this LogLevel logLevel)
        {
            string text;
            switch (logLevel)
            {
                case LogLevel.Trace:
                    text = "trce";
                    break;
                case LogLevel.Debug:
                    text = "dbug";
                    break;
                case LogLevel.Information:
                    text ="info";
                    break;
                case LogLevel.Warning:
                    text = "warn";
                    break;
                case LogLevel.Error:
                    text = "fail";
                    break;
                case LogLevel.Critical:
                    text = "crit";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
            return text;
        }

    }
}