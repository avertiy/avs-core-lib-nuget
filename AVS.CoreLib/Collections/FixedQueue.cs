#nullable enable
using System;
using System.Collections.Generic;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.Collections;

public class FixedQueue<T> : Queue<T>
{
    public int Capacity { get; }
    public bool IsFull => Count >= Capacity;
    public FixedQueue(int capacity) : base(capacity)
    {
        Guard.MustBe.Positive(capacity);
        Capacity = capacity;
    }

    public new void Enqueue(T item)
    {
        if (Count < Capacity)
        {
            base.Enqueue(item);
            return;
        }

        throw new InvalidOperationException($"Queue reached a max number of elements ({Capacity})");
    }

    public bool TryEnqueue(T item)
    {
        if (Count < Capacity)
        {
            base.Enqueue(item);
            return true;
        }

        return false;
    }

    public void Enqueue(T item, bool force)
    {
        if (Count < Capacity)
        {
            base.Enqueue(item);
        }
        else if (force)
        {
            base.Dequeue();
            base.Enqueue(item);
        }
        else
        {
            throw new InvalidOperationException($"Queue reached a max number of elements ({Capacity})");
        }
    }
}