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

        throw new ExceedCapacityException($"Queue reached a max number of elements ({Capacity})");
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
        }else if (force)
        {
            base.Dequeue();
            base.Enqueue(item);
        }
        else
        {
            throw new ExceedCapacityException($"Queue reached a max number of elements ({Capacity})");
        }   
    }
}

public class ExceedCapacityException : Exception
{
    public ExceedCapacityException(string message) : base(message)
    {
    }
}

//public class FixedQueueWrapper<T>
//{
//    private readonly Queue<T> _queue;

//    public int Capacity { get; }
//    public int Count => _queue.Count;

//    public FixedQueueWrapper(int capacity)
//    {
//        _queue = new(capacity);
//        Capacity = capacity;
//    }

//    public T? Enqueue(T item)
//    {
//        if (_queue.Count < Capacity)
//        {
//            _queue.Enqueue(item);
//            return default;
//        }

//        var dequeItem = _queue.Dequeue();
//        _queue.Enqueue(item);
//        return dequeItem;
//    }

//    public T Dequeue()
//    {
//        return _queue.Dequeue();
//    }

//    public bool TryDequeue(out T item)
//    {
//        return _queue.TryDequeue(out item);
//    }

//    public T Peek()
//    {
//        return _queue.Peek();
//    }
//}