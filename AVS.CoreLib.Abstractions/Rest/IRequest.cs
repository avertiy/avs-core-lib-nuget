using System.Collections.Generic;

namespace AVS.CoreLib.Abstractions.Rest
{
    public interface IRequest
    {
        public AuthType AuthType { get; set; }
        public string Method { get; set; }
        public string BaseUrl { get; set; }
        public string Path { get; set; }
        public Dictionary<string, object> Data { get; set; }
        string GetFullUrl(bool orderParameters = true);
    }
}