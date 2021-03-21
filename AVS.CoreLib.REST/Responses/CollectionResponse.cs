using System.Collections.ObjectModel;
using AVS.CoreLib.Abstractions.Responses;
using AVS.CoreLib.REST.Attributes;

namespace AVS.CoreLib.REST.Responses
{
    public abstract class CollectionResponse<T> : KeyedCollection<string, T>, IResponse
    {
        #region Properties

        //[JsonProperty(Order = -10)]
        [ArrayProperty(-10, true)]
        public string Error { get; set; }

        public virtual bool ShouldSerializeError()
        {
            return Error != null;
        }

        public bool Success => string.IsNullOrEmpty(Error);

        public virtual bool ShouldSerializeSuccess()
        {
            return false;
        }

        public string Source { get; set; }

        public virtual bool ShouldSerializeSource()
        {
            return Source != null;
        }

        public virtual bool ShouldSerializeCount() => false;

        #endregion

        //public static implicit operator T(CollectionResponse<T> response)
        //{
        //    if (response.Count == 1)
        //        return response.First();

        //    throw new InvalidCastException("Response must contain exactly one item.");
        //}
    }
}