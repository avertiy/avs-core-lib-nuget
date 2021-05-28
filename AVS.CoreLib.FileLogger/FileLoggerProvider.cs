using System;
using System.Collections.Concurrent;
using AVS.CoreLib.FileLogger.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AVS.CoreLib.FileLogger
{
    [ProviderAlias("File")]
    public class FileLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly ConcurrentDictionary<string, AbstractLogger> _loggers;
        private readonly LogScope _currentScope;
        public FileLoggerProvider(IOptionsMonitor<FileLoggerOptions> options)
        {
            _loggers = new ConcurrentDictionary<string, AbstractLogger>(StringComparer.Ordinal);
            var optionsValue = options.CurrentValue;
            if (string.IsNullOrEmpty(optionsValue.DateFormat))
            {
                optionsValue.DateFormat = "G";
            }

            _currentScope = new LogScope()
            {
                PrintLoggerName = optionsValue.PrintLoggerName,
                UseCurlyBrackets = optionsValue.UseCurlyBracketsForScope,
                Writer = new FileLogWriter(options)
            };
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, category => new AbstractLogger(categoryName, _currentScope));
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            _currentScope.ScopeProvider = scopeProvider;
        }

        public void Dispose()
        {
        }
    }
}