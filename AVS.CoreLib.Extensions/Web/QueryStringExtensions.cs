using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace AVS.CoreLib.Extensions.Web
{
    public static class QueryStringExtensions
    {
        //[DebuggerStepThrough]
        public static string ToHttpQueryString(this IDictionary<string, object> data, bool orderBy = true)
        {
            if (data.Count == 0)
                return string.Empty;

            var sb = new StringBuilder();

            var entries = data.AsEnumerable();

            if (orderBy)
                entries = entries.OrderBy(x => x.Key);

            foreach (var entry in entries)
            {
                if (entry.Value == null)
                    continue;

                var value = HttpUtility.UrlEncode(entry.Value.ToString());
                if (value.Length == 0)
                    continue;

                var str = "&" + HttpUtility.UrlEncode(entry.Key) + "=" + value;
                sb.Append(str);
            }

            if (sb.Length > 0)
                sb.Remove(0, 1);

            return sb.ToString();
        }

        public static string GetQueryStringValue(this string queryString, string key)
        {
            var ind = queryString.IndexOf(key, StringComparison.Ordinal);
            if (ind == -1)
                return string.Empty;

            ind += key.Length + 1;
            var endInd = queryString.IndexOf('&', ind);
            return endInd == -1 ? queryString.Substring(ind) : queryString.Substring(ind, endInd - ind);
        }

        public static string QueryStringCombine(this string queryString, string otherPart)
        {
            if (queryString.Length > 0 && otherPart.Length > 0)
                return $"{queryString}&{otherPart}";
            return queryString + otherPart;
        }
    }
}