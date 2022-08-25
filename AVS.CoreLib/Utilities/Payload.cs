using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using AVS.CoreLib.Abstractions.Rest;

namespace AVS.CoreLib.Utilities
{
    public class Payload : IPayload
    {
        public string RelativeUrl { get; set; }

        private readonly Dictionary<string, string> _items = new Dictionary<string, string>();
        public Payload()
        {
        }

        public Payload(string queryStringParameters)
        {
            Join(queryStringParameters);
        }

        public bool ContainsKey(string key) => _items.ContainsKey(key);

        public bool Remove(string key) => _items.Remove(key);

        /// <summary>
        /// creates query string with ordered key values 
        /// </summary>
        public string ToHttpQueryString()
        {
            if (_items.Count == 0)
                return string.Empty;
            var sb = new StringBuilder();
            foreach (var kp in _items.OrderBy(i => i.Key))
            {
                if (kp.Value != null)
                {
                    sb.Append($"{HttpUtility.UrlEncode(kp.Key)}={HttpUtility.UrlEncode(kp.Value)}&");
                }
            }

            if (sb.Length > 0)
            {
                sb.Length--;
                if (_items.Count == 1 && sb[^1] == '=')
                    sb.Length--;
            }

            return sb.ToString();
        }

        public string this[string key]
        {
            get => _items[key];
            set => _items[key] = value;
        }

        public IPayload Add(string key, object value)
        {
            _items[key] = value?.ToString();
            return this;
        }

        public IPayload Add(QueryString queryString)
        {
            foreach (var kp in queryString.Params)
            {
                _items.Add(kp.Key, kp.Value?.ToString() ?? "");
            } 
             
            return this;
        }

        public IPayload Add(object[] parameters)
        {
            foreach (var parameter in parameters)
                AddParameter(parameter.ToString());
            return this;
        }

        public IPayload Add(string[] arguments)
        {
            try
            {
                for (int i = 0; i < arguments.Length; i++)
                {
                    if (arguments[i].Contains("="))
                    {
                        AddParameter(arguments[i]);
                    }
                    else
                    {
                        Add(arguments[i], arguments[i + 1]);
                        i++;
                    }
                }
                return this;
            }
            catch (Exception ex) { throw new ArgumentException("Invalid request data arguments", ex); }
        }

        public IPayload AddParameter(string input)
        {
            //possible cases 1. parameter1=value 2. /url?parameter1=value
            if (input.StartsWith("/"))
            {
                var ind = input.IndexOf("?", StringComparison.Ordinal);
                if (ind == -1)
                {
                    RelativeUrl = input;
                    return this;
                }
                else
                {
                    RelativeUrl = input.Substring(0, ind);
                    input = input.Substring(ind);
                }
            }
            else if (input.Contains("?"))
            {
                var arr = input.Split('?');
                RelativeUrl = arr[0] + "?";
                input = arr[1];
            }

            var parts = input.Split('=');
            if (parts.Length == 2)
            {
                Add(parts[0], parts[1]);
                return this;
            }

            if (parts.Length == 1)
            {
                Add(parts[0], string.Empty);
                return this;
            }
            throw new ArgumentException($"invalid request data parameter: {input}");
        }

        public IPayload Join(string queryString)
        {
            if (string.IsNullOrEmpty(queryString))
                return this;
            var parameters = queryString.Split('&');
            foreach (var parameter in parameters)
            {
                AddParameter(parameter);
            }
            return this;
        }

        public byte[] GetBytes()
        {
            return Encoding.ASCII.GetBytes(ToHttpQueryString());
        }

        public override string ToString()
        {
            return ToHttpQueryString();
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            foreach (var kp in _items.OrderBy(i => i.Key))
            {
                if (kp.Value == null)
                    continue;
                yield return kp;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static Payload Empty => new Payload();

        #region implicit operators
        public static implicit operator Payload(string str)
        {
            return new Payload(str);
        }

        public static implicit operator Payload(Dictionary<string, string> dict)
        {
            var data = new Payload();
            foreach (var kp in dict)
                data.Add(kp.Key, kp.Value);
            return data;
        }

        public static implicit operator Payload(Dictionary<string, object> dict)
        {
            return Payload.From(dict);
        }

        public static implicit operator Payload(object[] parameters)
        {
            var data = new Payload();
            data.Add(parameters);
            return data;
        }

        public static implicit operator Payload(QueryString qs)
        {
            var payload = new Payload();
            payload.Add(qs);
            return payload;
        }

        #endregion

        public static Payload Create(string key, object value)
        {
            var data = new Payload { { key, value } };
            return data;
        }

        public static Payload Create(string queryStringParameters)
        {
            return new Payload(queryStringParameters);
        }

        public static Payload Create(object[] parameters)
        {
            var data = new Payload();
            data.Add(parameters);
            return data;
        }

        public static Payload Create(string[] parameters)
        {
            var data = new Payload();
            data.Add(parameters);
            return data;
        }

        public static Payload From(IDictionary<string, object> dict)
        {
            var data = new Payload();
            foreach (var kp in dict)
                data.Add(kp.Key, kp.Value);
            return data;
        }
    }
}
