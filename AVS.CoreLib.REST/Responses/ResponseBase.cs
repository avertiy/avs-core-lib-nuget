using System.Diagnostics;
using AVS.CoreLib.Abstractions.Responses;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.REST.Attributes;

namespace AVS.CoreLib.REST.Responses
{
    [DebuggerDisplay("{ToString()}")]
    public abstract class ResponseBase : IResponse
    {
        [ArrayProperty(-10, true)]
        public string Error { get; set; }

        public virtual bool ShouldSerializeError()
        {
            return Error != null;
        }

        public bool Success => Error == null;

        public virtual bool ShouldSerializeSuccess()
        {
            return false;
        }

        /// <summary>
        /// the source that produced the response
        /// </summary>
        public string Source { get; set; }

        public virtual bool ShouldSerializeSource()
        {
            return Source != null;
        }

        public override string ToString()
        {
            return (Success ? "Response - OK " : $"Response - Fail [{Error}]").Append(Source, "[Source: {0}]");
        }

        /// <summary>
        /// Overwrite bool check so you can use if(response) instead of if(response.Success)
        /// </summary>
        public static implicit operator bool(ResponseBase response)
        {
            return response is { Success: true };
        }
    }
}