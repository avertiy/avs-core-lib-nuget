using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AVS.CoreLib.ConsoleTools.Logging
{
    [ProviderAlias("Console")]
    public partial class ConsoleLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly ConcurrentDictionary<string, ConsoleLogger> _loggers;
        private IExternalScopeProvider _scopeProvider;
        private readonly IOptionsMonitor<ConsoleLoggerOptions> _options;

        public ConsoleLoggerProvider(IOptionsMonitor<ConsoleLoggerOptions> options)
        {
            _options = options;
            _loggers = new ConcurrentDictionary<string, ConsoleLogger>(StringComparer.Ordinal);
            ConsoleLogger.Scope.UseCurlyBrackets = _options.CurrentValue.UseCurlyBracketsForScope;
            ConsoleLogger.Scope.PrintLoggerName = _options.CurrentValue.PrintLoggerName;
        }

        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, category => new ConsoleLogger(categoryName, _scopeProvider, _options.CurrentValue));
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }
    }
}
