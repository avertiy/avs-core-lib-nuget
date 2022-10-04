using System;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.AbstractLogger
{
    public interface ILogScope : IDisposable
    {
        bool UseCurlyBrackets { get; set; }
        bool PrintLoggerName { get; set; }
        IExternalScopeProvider ScopeProvider { get; set; }

        //ILogWriter Writer { get; set; }

        ILogWriter OpenScope<TState>(string loggerName, TState state);
    }

    public class LogScope : IDisposable
    {
        public bool UseCurlyBrackets { get; set; }
        public bool PrintLoggerName { get; set; }
        public IExternalScopeProvider ScopeProvider { get; set; }
        public ILogWriter Writer { get; set; }
        private int HashCode { get; set; }
        private string Logger { get; set; }

        public void OpenScope<TState>(string loggerName, TState state)
        {
            SetLogger(loggerName);
            var openScopes = 0;
            ScopeProvider?.ForEachScope((scopeObj, x) =>
            {
                openScopes++;
                if (openScopes > 1)
                    return;

                Begin(scopeObj);

            }, state);

            if (openScopes < 1)
            {
                Close();
            }
        }

        private void SetLogger(string name)
        {
            if (Logger != name)
            {
                Close();
                Logger = name;
                if (PrintLoggerName && Logger != null)
                {
                    Writer.WriteLine(false);
                    Writer.Write(Logger);
                }
            }
        }

        private void Begin(object scope)
        {
            var hashCode = scope.GetHashCode();
            if (HashCode == hashCode)
                return;

            Close();
            Writer.BeginScope(scope, UseCurlyBrackets);
            HashCode = hashCode;
        }

        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            if (HashCode != 0)
            {
                Writer.EndScope(UseCurlyBrackets);
                HashCode = 0;
            }
        }
    }
}