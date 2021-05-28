using System;
using AVS.CoreLib.PowerConsole.Utilities;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.ConsoleTools.Logging
{
    class ScopeHelper
    {
        public static void OpenScope<TState>(IExternalScopeProvider scopeProvider, TState state, string logger)
        {
            ConsoleLogger.Scope.SetLogger(logger);
            var openScopes = 0;
            if (scopeProvider != null)
            {
                scopeProvider.ForEachScope((scopeObj, x) =>
                {
                    openScopes++;
                    if (openScopes > 1)
                        return;

                    var hashCode = scopeObj.GetHashCode();
                    if (ConsoleLogger.Scope.HashCode == hashCode)
                        return;

                    string scope = scopeObj.ToString();
                    var color = ColorHelper.ExtractColor(ref scope, ConsoleColor.DarkMagenta);
                    ConsoleLogger.Scope.Begin(hashCode, scope, color);

                }, state);
            }

            if (openScopes < 1)
            {
                ConsoleLogger.Scope.Close();
            }
        }
    }
}