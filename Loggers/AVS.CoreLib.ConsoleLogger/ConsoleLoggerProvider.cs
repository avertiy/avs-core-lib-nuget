using System;
using System.Collections.Concurrent;
using AVS.CoreLib.AbstractLogger;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AVS.CoreLib.ConsoleLogger
{
    [ProviderAlias("Console")]
    public class ConsoleLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly ConcurrentDictionary<string, AbstractLogger.AbstractLogger> _loggers;
        private readonly LogScope _currentScope;
        public ConsoleLoggerProvider(IOptionsMonitor<ConsoleLoggerOptions> options)
        {
            _loggers = new ConcurrentDictionary<string, AbstractLogger.AbstractLogger>(StringComparer.Ordinal);
            var optionsValue = options.CurrentValue;
            if (string.IsNullOrEmpty(optionsValue.DateFormat))
            {
                optionsValue.DateFormat = "G";
            }

            _currentScope = new LogScope()
            {
                PrintLoggerName = optionsValue.PrintLoggerName,
                UseCurlyBrackets = optionsValue.UseCurlyBracketsForScope,
                Writer = new ConsoleLogWriter(options)
            };
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, category => new AbstractLogger.AbstractLogger(categoryName, _currentScope));
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