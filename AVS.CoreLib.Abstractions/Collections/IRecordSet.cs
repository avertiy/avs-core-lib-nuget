#nullable enable
using System.Collections.Generic;

namespace AVS.CoreLib.Abstractions.Collections;

/// <summary>
/// Represents a wrapper over <see cref="List{T}"/> collection, helps to merge items, preserving their uniqueness
/// </summary>
public interface IRecordSet<T> : ICollection<T>
{
    int AddRange(IList<T> items);
    T this[int index] { get; }
    int IndexOf(T item);
    int Compare(T item, T other);
    void RemoveAt(int index);
    T[] ToArray();
}