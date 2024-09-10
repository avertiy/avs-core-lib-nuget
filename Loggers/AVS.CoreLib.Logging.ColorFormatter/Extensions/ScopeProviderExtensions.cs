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
                var sb = new StringBuilder(scope.ToString());
                sb.Replace("\r\n", "\r\n => ");
                foreach (var kp in list)
                {
                    var val = kp.Value?.ToString();

                    if (val == null)
                        continue;

                    var ind = sb.IndexOf(val);
                    if (ind == -1)
                        continue;

                    if (sb.IndexOf(kp.Key + ":") > -1)
                        continue;

                    sb.Replace(val, $"{{\"{kp.Key}\": {val}}}", ind, val.Length);
                }

                return sb.ToString();
            }

            if (scope is ITuple tuple)
            {
                return tuple.Length == 2 && tuple[0] is string key && key.StartsWith("@") && tuple[1] != null
                    ? $"{key}:{tuple[1].ToJsonString()}"
                    : tuple.ToString()!;
                //switch (tuple)
                //{
                //    case ValueTuple<string, string>:
                //    case ValueTuple<string, int>:
                //    case ValueTuple<string, double>:
                //    case ValueTuple<string, decimal>:
                //        return tuple.ToString()!;
                //    case ValueTuple<string, object> objTuple:
                //    {
                //        return objTuple.Item1.StartsWith("@") 
                //            ? objTuple.ToJsonString() // $"{objTuple.Item1}:{objTuple.Item2.ToJsonString()}"
                //            : objTuple.ToString();
                //    }
                //    default:
                //    {

                //    }
                //}
            }



            return scope.ToString()!;
        }
    }
}