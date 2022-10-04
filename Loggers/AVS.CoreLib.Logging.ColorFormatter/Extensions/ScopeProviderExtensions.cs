using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.Logging.ColorFormatter.Extensions
{
    public static class ScopeProviderExtensions
    {
        public static List<object> GetAllScopes(this IExternalScopeProvider scopeProvider)
        {
            var list = new List<object>();
            scopeProvider.ForEachScope((scope, _) =>
            {
                list.Add(scope);
            }, list);

            return list;
        }
    }
}