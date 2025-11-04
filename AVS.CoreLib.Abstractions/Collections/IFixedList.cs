#nullable enable
using System.Collections.Generic;

namespace AVS.CoreLib.Abstractions.Collections;

/// <summary>
/// Represents a fixed size list collection
/// </summary>
public interface IFixedList<T> : IList<T>
{
    int Capacity { get; }
    bool IsFull { get; }

    void EnsureCapacity(int capacity);

    T? ForceAdd(T item);
    bool TryAdd(T item);

    /// <summary>
    /// Put an item on top of the list, if item already exists, moves it to the end of the list
    /// </summary>
    void Put(T item);

    T[] ToArray();

    string AsString();
}