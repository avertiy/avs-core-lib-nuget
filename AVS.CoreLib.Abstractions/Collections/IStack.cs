#nullable enable
using System;
using System.Collections.Generic;

namespace AVS.CoreLib.Abstractions.Collections;

/// <summary>
/// Represents a fixed size LIFO collection
/// </summary>
public interface IStack<T> : IEnumerable<T>
{
    int Count { get; }
    int Capacity { get; }
    bool IsFull { get; }
    /// <summary>
    /// Adds item on top of the list (stack)
    /// If the stack is full, pop the last element and returns it.
    /// Otherwise, returns default(T).
    /// </summary>
    T? Push(T item);

    /// <summary>
    /// Adds item on top of the stack
    /// </summary>
    /// <exception cref="InvalidOperationException">If stack reached capacity throws an exception</exception>
    void PushStrict(T item);
    /// <summary>
    /// Removes item at the top of the stack and returns it.
    /// If Stack is empty throws an InvalidOperationException.
    /// </summary>
    T Pop();

    bool TryPop(out T? result);
    /// <summary>
    /// Returns element at the top of the Stack without removing it.
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
    /// Get element at the specified index (direct order)
    /// e.g. {1,2,3,4}; stack[2] => 3   
    /// </summary>
    T this[int index] { get; }

    /// <summary>
    /// Returns a string that represents the current stack.
    /// </summary>
    string AsString();

    void Clear();
}