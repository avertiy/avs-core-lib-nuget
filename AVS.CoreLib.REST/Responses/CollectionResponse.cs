#nullable enable
using System;
using System.Collections.ObjectModel;
using AVS.CoreLib.Abstractions.Responses;
using AVS.CoreLib.REST.Attributes;

namespace AVS.CoreLib.REST.Responses
{
    [Obsolete("Obsolete don't use")]
    public abstract class CollectionResponse<T> : KeyedCollection<string, T>, IResponse
    {
        #region Properties

        [ArrayProperty(-10, true)]
        public string? Error { get; set; }

        public virtual bool ShouldSerializeError()
        {
            return Error != null;
        }

        public string? RawContent { get; set; }

        public bool Success => string.IsNullOrEmpty(Error);

        public virtual bool ShouldSerializeSuccess()
        {
            return false;
        }

        public string Source { get; set; } = null!;
        public object? Request { get; set; }

        public virtual bool ShouldSerializeSource()
        {
            return Source != null;
        }

        public virtual bool ShouldSerializeCount() => false;

        #endregion
    }
}