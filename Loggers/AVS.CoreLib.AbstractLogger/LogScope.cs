using System;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.AbstractLogger
{
    public class LogScope : IDisposable
    {
        public bool UseCurlyBrackets { get; set; }
        public bool PrintLoggerName { get; set; }
        public IExternalScopeProvider ScopeProvider { get; set; }
        public ILogWriter Writer { get; set; }
        private int HashCode { get; set; }
        private string Logger { get; set; }

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

        protected void Begin(object scope)
        {
            var hashCode = scope.GetHashCode();
            if (HashCode == hashCode)
                return;

            Close();
            Writer.WriteLine(false);
            Writer.Write(UseCurlyBrackets ? $"\t{scope}\r\n {{" : $"\t{scope}");
            HashCode = hashCode;
        }

        protected void Close()
        {
            if (HashCode != 0)
            {
                if (UseCurlyBrackets)
                    Writer.Write(" }");
                else
                    Writer.WriteLine();

                HashCode = 0;
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}