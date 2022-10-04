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

        //loggers share scope so once you begin scope and drill down into children classes with their respective loggers they all will write into the same scope 
        private readonly LogScope _sharedScope;
        public ConsoleLoggerProvider(IOptionsMonitor<ConsoleLoggerOptions> options)
        {
            _loggers = new ConcurrentDictionary<string, AbstractLogger.AbstractLogger>(StringComparer.Ordinal);
            var optionsValue = options.CurrentValue;

            _sharedScope = new LogScope()
            {
                PrintLoggerName = optionsValue.PrintLoggerName,
                UseCurlyBrackets = optionsValue.UseCurlyBracketsForScope,
                Writer = new ConsoleLogWriter(options)
            };
        }

        /// <summary>
        /// Creates a new <see cref="ILogger"/> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>The instance of <see cref="ILogger"/> that was created.</returns>
        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, category => new AbstractLogger.AbstractLogger(categoryName, _sharedScope));
        }

        /// <summary>
        /// Sets external scope information source for logger provider.
        /// </summary>
        /// <param name="scopeProvider">The provider of scope data.</param>
        public void SetScopeProvider(IExternalScopeProvider scopeProvider)
        {
            _sharedScope.ScopeProvider = scopeProvider;
        }

        public void Dispose()
        {
        }
    }
}