#nullable enable
using System;
using System.Diagnostics;
using AVS.CoreLib.Abstractions.Responses;
using AVS.CoreLib.Attributes;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.REST.Responses
{
    [DebuggerDisplay("{ToString()}")]
    public abstract class ResponseBase : IResponse
    {
        /// <summary>
        /// the source that produced the response
        /// </summary>
        public string Source { get; set; } = null!;

        [ArrayProperty(-10, true)]
        public string? Error { get; set; }

        public virtual bool ShouldSerializeError()
        {
            return Error != null;
        }

        public string RawContent { get; set; } = string.Empty;

        public bool Success => Error == null;

        public virtual bool ShouldSerializeSuccess()
        {
            return false;
        }

        public object? Request { get; set; }

        public virtual bool ShouldSerializeRequestedUrl()
        {
            return false;
        }

        public override string ToString()
        {
            var str = Success ? "Response - OK" : $"Response - Fail [{Error}]";
            return str.AppendIf(Source != null, $" [Source: {Source}]");
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