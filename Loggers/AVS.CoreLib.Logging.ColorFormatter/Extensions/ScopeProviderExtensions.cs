#nullable enable
using System.Runtime.CompilerServices;
using System.Text;
using AVS.CoreLib.Extensions;
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
                if (scope == null)
                    return;
                list.Add(scope);
            }, list);

            return list;
        }

        public static string? GetScope(this IExternalScopeProvider scopeProvider)
        {
            //var list = new Stack<string>();
            //scopeProvider.ForEachScope((scope, _) =>
            //{
            //    if (scope == null)
            //        return;
            //    list.Push(FormatScope(scope));
            //}, list);
            //var scope = "\r\n" + string.Join(" <= ", list) + "\r\n";

            var sb = new StringBuilder();
            scopeProvider.ForEachScope((scope, _) =>
            {
                if (sb.Length > 0)
                {
                    sb.Append(" => ");
                }

                sb.Append(FormatScope(scope));
            }, sb);

            return sb.Length == 0 ? null : sb.ToString();
        }

        private static string FormatScope(object? scope)
        {
            if (scope == null)
                return string.Empty;

            if (scope is string str)
                return str;

            if (scope is IDictionary<string, object> dict)
                return dict.ToJsonString();

            if (scope is IReadOnlyList<KeyValuePair<string, object?>> list && list.Count > 0)
            {
                var sb = new StringBuilder(list[^1].Value?.ToString() ?? scope.ToString());
                sb.Replace("\r\n", "\r\n => ");
                foreach (var kp in list)
                {
                    var keyInBrackets = '{' + kp.Key + '}';

                    var ind = sb.IndexOf(keyInBrackets);
                    if (ind == -1)
                        continue;

                    var val = kp.Value == null 
                        ? "(null)"
                        : kp.Key.StartsWith('@') 
                            ? kp.Value.ToJsonString()
                            : kp.Value.ToString();

                    if (kp.Key.EndsWith('$'))
                    {
                        var key = kp.Key.TrimStart('@').TrimEnd('$');
                        sb.Replace(keyInBrackets, $"{{\"{key}\": \"{val}\"}}", ind, keyInBrackets.Length);
                    }
                    else
                    {
                        sb.Replace(keyInBrackets, val, ind, keyInBrackets.Length);
                    }
                        
                }

                return sb.ToString();
            }

            if (scope is ITuple tuple)
            {
                return tuple.Length == 2 && tuple[0] is string key && key.StartsWith("@") && tuple[1] != null
                    ? $"{key}:{tuple[1].ToJsonString()}"
                    : tuple.ToString()!;
            }

            return scope.ToString()!;
        }
    }
}