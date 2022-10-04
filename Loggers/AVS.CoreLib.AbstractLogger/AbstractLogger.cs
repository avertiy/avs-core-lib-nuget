using System;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.AbstractLogger
{
    //todo rename to ScopeLogger
    public class AbstractLogger : ILogger
    {
        private LogScope Scope { get; }
        public string Name { get; }

        public AbstractLogger(string name, LogScope scope)
        {
            Scope = scope;
            Name = name;
        }

        /// <summary>Begins a logical operation scope.</summary>
        /// <param name="state">The identifier for the scope.</param>
        /// <typeparam name="TState">The type of the object to be written.</typeparam>
        public IDisposable BeginScope<TState>(TState state)
        {
            //this method is called by only when ScopeLogger.ExternalScopeProvider is null
            //but framework initializes it for us so this method is never called
            return Scope.ScopeProvider?.Push(state) ?? NullScope.Instance;
        }

        /// <summary>
        /// Checks if the given <paramref name="logLevel" /> is enabled.
        /// </summary>
        public virtual bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public virtual void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));

            var message = FormatState(state, exception, formatter);

            Scope.OpenScope(Name, state);
            var writer = Scope.Writer;

            writer.Write(Name, eventId, logLevel, message, exception);
        }

        protected virtual string FormatState<TState>(TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            return formatter(state, exception);
        }
    }
}

