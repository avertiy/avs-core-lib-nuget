#nullable enable
using System.Collections.ObjectModel;
using System.Diagnostics;
using AVS.CoreLib.Abstractions.Responses;
using AVS.CoreLib.REST.Attributes;

namespace AVS.CoreLib.REST.Responses
{
    [DebuggerDisplay("Count = {Count}, Error = {Error}")]
    public class MultiResponse<T> : KeyedCollection<string, T>, IResponse
        where T : IResponse
    {
        #region Properties

        //[JsonProperty(Order = -10)]
        [ArrayProperty(-10, true)]
        public string? Error { get; set; }

        public virtual bool ShouldSerializeError()
        {
            return Error != null;
        }

        public bool Success => string.IsNullOrEmpty(Error);

        public string Source { get; set; } = null!;

        public virtual bool ShouldSerializeSource()
        {
            return Source != null;
        }

        public object? Request { get; set; }

        public virtual bool ShouldSerializeRequest()
        {
            return false;
        }

        #endregion

        protected override string GetKeyForItem(T item)
        {
            return item.Source;
        }
    }
}