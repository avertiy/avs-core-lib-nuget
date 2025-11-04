#nullable enable
using System;
using System.Collections.Generic;

namespace AVS.CoreLib.Abstractions.Collections;

/// <summary>
/// Represents a fixed size FIFO collection
/// </summary>
public interface IQueue<T> : IEnumerable<T>
{
    int Count { get; }
    int Capacity { get; }
    bool IsFull { get; }
    /// <summary>
    /// Adds an item to the end of the list (queue).
    /// If the list is full, removes and returns the oldest item.
    /// Otherwise, returns default(T).
    /// </summary>
    T? Enqueue(T item);
    /// <summary>
    /// Adds item to the end of the queue
    /// </summary>
    /// <exception cref="InvalidOperationException">If the queue reached capacity throws an exception</exception>
    void EnqueueStrict(T item);
    /// <summary>
    /// Removes item at the head of the queue and returns it.
    /// If Queue is empty throws an InvalidOperationException.
    /// </summary>
    T Dequeue();

    bool TryDequeue(out T? item);
    /// <summary>
    /// Returns element at the head of the queue. The element remains in the queue. 
    /// </summary>
    /// <exception cref="InvalidOperationException">If the queue is empty throws an exception</exception>
    T Peek();

    /// <summary>
    /// Returns element by offset. The element remains in the queue.
    /// e.g. Q:{1,2,3,4}; Q.Peek(offset:1) => 3  
    /// </summary>
    /// <exception cref="InvalidOperationException">If the queue is empty throws an exception</exception>
    T Peek(int offset);

    /// <summary>
    /// Get Q element at the specified index (direct order)
    /// e.g. Q:{1,2,3,4}; Q[2] => 3   
    /// </summary>
    T this[int index] { get; }

    void Clear();

    /// <summary>
    /// Returns a string that represents the current queue.
    /// </summary>
    string AsString();
}