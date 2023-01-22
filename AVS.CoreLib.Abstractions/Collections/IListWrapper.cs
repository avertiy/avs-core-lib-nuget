using System.Collections.Generic;

namespace AVS.CoreLib.Abstractions.Collections
{
    /// <summary>
    /// <see cref="ICollection{T}"/> interface is rarely implemented, but in most cases we are ok to have Add(T item) method
    /// </summary>
    public interface IListWrapper<in T>
    {
        void Add(T item);
    }
}