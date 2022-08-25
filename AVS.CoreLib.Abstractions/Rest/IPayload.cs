using System.Collections.Generic;

namespace AVS.CoreLib.Abstractions.Rest
{
    public interface IPayload : IEnumerable<KeyValuePair<string, string>>
    {
        string RelativeUrl { get; set; }
        bool ContainsKey(string key);
        bool Remove(string key);
        string ToHttpQueryString();
        string this[string key] { get; set; }
        IPayload Add(string key, object value);
        IPayload Add(object[] parameters);
        IPayload Add(string[] arguments);
        IPayload AddParameter(string input);
        IPayload Join(string queryString);

        byte[] GetBytes();
    }
}