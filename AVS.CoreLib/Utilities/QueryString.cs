using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace AVS.CoreLib.Utilities
{
    public class QueryString
    {
        public string BaseUrl { get; set; }
        public IDictionary<string, object> Params { get; } = new Dictionary<string, object>();

        public QueryString()
        {
        }

        public QueryString(IDictionary<string, object> dict)
        {
            Params = dict;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(BaseUrl ?? "?");

            foreach (var kp in Params.OrderBy(x => x.Key))
            {
                sb.Append(HttpUtility.UrlEncode(kp.Key));
                sb.Append('=');
                sb.Append(HttpUtility.UrlEncode(kp.Value?.ToString() ?? ""));
                sb.Append('&');
            }

            if (Params.Any())
                sb.Length--;

            return sb.ToString();
        }

        public static implicit operator string(QueryString qs)
        {
            return qs.ToString();
        }

        public QueryString Add(string key, string value)
        {
            Params.Add(key, value);
            return this;
        }

        public QueryString Add(string key, object value)
        {
            Params.Add(key, value);
            return this;
        }

        public QueryString AddIfNotEmpty(string key, string value)
        {
            if (string.IsNullOrEmpty(value))
                return this;

            Params.Add(key, value);
            return this;
        }

        public QueryString AddIfNotEmpty(string key, object value)
        {
            if (value == null)
                return this;

            var parameterValue = value.ToString();
            if (string.IsNullOrEmpty(parameterValue))
                return this;

            Params.Add(key, parameterValue);
            return this;
        }

        public byte[] GetBytes()
        {
            return Encoding.ASCII.GetBytes(ToString());
        }

        public static QueryString From(string baseUrl)
        {
            return new QueryString() { BaseUrl = baseUrl };
        }

        public static QueryString From(IDictionary<string, object> dict)
        {
            var qs = new QueryString(dict);
            return qs;
        }

        public static QueryString Create(string key, object value)
        {
            return new QueryString().Add(key, value);
        }
    }
}