#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.Collections;

/// <summary>
/// Represent a fixed size list
/// implement capabilities of <see cref="List{T}"/>, <see cref="Queue{T}"/> and <see cref="Stack{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public class FixedList<T> : IList<T>, IEnumerable<T>, IEnumerable
{
    protected T[] Items;
    protected int Head { get; set; }
    public int Capacity { get; protected set; }
    public int Count { get; protected set; }
    public bool IsFull => Count == Capacity;
    public bool IsReadOnly => false;

    public FixedList(int capacity)
    {
        Guard.MustBe.GreaterThanOrEqual(capacity, 0);
        Capacity = capacity;
        Items = new T[capacity];
        Count = 0;
        Head = 0;
    }

    public void EnsureCapacity(int capacity)
    {
        if (Capacity >= capacity)
            return;

        Capacity = capacity;
        var arr = new T[capacity];

        for (var i = 0; i < Items.Length; i++)
            arr[i] = Items[i];
            
        Items = arr;
    }

    public T this[int index]
    {
        get
        {
            Guard.MustBe.WithinRange(index, 0, Capacity - 1);
            return Items[(Head + index) % Capacity];
        }
        set => Items[(Head + index) % Capacity] = value;
    }

    public void Add(T item)
    {
        if (Count >= Capacity)
            throw new ExceedCapacityException($"List reached a max number of elements ({Capacity})");

        Items[(Head + Count) % Capacity] = item;
        Count++;
    }

    public void Add(T item, bool force)
    {
        if (Count < Capacity)
        {
            Items[(Head + Count) % Capacity] = item;
            Count++;
            return;
        }

        if (!force)
            throw new ExceedCapacityException($"List reached a max number of elements ({Capacity})");

        Items[Head] = item;
        Head = (Head + 1) % Capacity;
    }

    public bool TryAdd(T item)
    {
        if (Count < Capacity)
        {
            Items[(Head + Count) % Capacity] = item;
            Count++;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Put item on top of the list (if item already exists, removes it and add to the end)
    /// </summary>
    public void Put(T item)
    {
        var ind = IndexOf(item);

        if (ind == -1)
        {
            Add(item, force: true);
            return;
        }

        if (ind == Count - 1)
            return;

        if (ind == 0 && IsFull)
        {
            //e.g. head = 1, item=2 [1,*2,3,4,5,6] => [1,2,*3,4,5,6];
            Head++;
            return;
        }

        //head = 0 [*1,2,(3),4,5,6] item=3 (ind:2) => [*1,2,(4,5,6)];
        if (Head == 0)
        {
            Array.Copy(Items, ind + 1, Items, ind, Count - ind - 1);
            Items[^1] = item;
            return;
        }

        RemoveAt(ind);
        Add(item, true);
    }


    public IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < Count; i++)
        {
            yield return Items[(Head + i) % Capacity];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Clear()
    {
        Array.Clear(Items, 0, Items.Length);
        Count = 0;
        Head = 0;
    }

    public bool Contains(T item)
    {
        if (Count == 0)
            return false;

        if (Head == 0)
            return Array.IndexOf(Items, item, Head, Count) >= 0;

        return
            Array.IndexOf(Items, item, Head, Items.Length - Head) >= 0 ||
            Array.IndexOf(Items, item, 0, Capacity - Count) >= 0;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        if (arrayIndex < 0 || arrayIndex + Count > array.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        }

        for (var i = 0; i < Count; i++)
        {
            array[arrayIndex + i] = this[i];
        }
    }

    public int IndexOf(T item)
    {
        for (var i = 0; i < Count; i++)
        {
            var currentItem = Items[(Head + i) % Capacity];
            if (EqualityComparer<T>.Default.Equals(currentItem, item))
            {
                return i;
            }
        }
        return -1;
    }

    public void Insert(int index, T item)
    {
        if (Count < Capacity && Head == 0)
        {
            Array.Copy(Items, index, Items, index + 1, Count - index);
            Count++;
        }
        else if (index == 0)
        {
            Items[Head] = item;
            Head = (Head + 1) % Capacity;
        }
        else
        {
            var items = new T[Capacity];
            items[index] = item;
            for (var i = 0; i < Count - 1; i++)
            {
                var ii = (Head + i + 1) % Capacity;
                var shift = i < index ? 0 : 1;
                items[i + shift] = Items[ii];
            }
            Array.Copy(items, 0, Items, 0, Count);
            Head = 0;
        }
    }

    public bool Remove(T item)
    {
        var index = IndexOf(item);
        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }

        return false;
    }

    public void RemoveAt(int index)
    {
        Guard.MustBe.WithinRange(index, 0, Count - 1);
        if (Head == 0)
        {
            Count--;
            Array.Copy(Items, index + 1, Items, index, Count - index);
        }
        else
        {
            var items = new T[Capacity];

            for (var i = 0; i < Count - 1; i++)
            {
                if (i < index)
                {
                    var ind = (Head + i) % Capacity;
                    items[i] = Items[ind];
                }
                else
                {
                    var ind = (Head + i + 1) % Capacity;
                    items[i] = Items[ind];
                }
            }

            Count--;
            Array.Copy(items, 0, Items, 0, Count);
            Head = 0;
        }
    }

    public T Pop()
    {
        if (Count == 0)
            throw new InvalidOperationException("List has no items");

        //stack Add: 1,2,3 => [1,2,3]; Pop()=> 3,2,1
        //queue Add: 1,2,3 => [1,2,3]; Dequeue() =>1,2,3
        Count--;
        var index = (Head + Count - 1) % Capacity;
        var result = Items[index];
        Items[index] = default!;
        return result;
    }

    public bool TryPop(out T? result)
    {
        if (Count == 0)
        {
            result = default!;
            return false;
        }

        result = Pop();
        return true;
    }

    public T Dequeue()
    {
        if (Count == 0)
            throw new InvalidOperationException("List has no items");

        var result = Items[Head];
        Items[Head] = default!;

        if (Head + 1 < Capacity)
            Head++;
        else
            Head = 0;

        Count--;

        return result;
    }

    public bool TryDequeue(out T? item)
    {
        if (Count == 0)
        {
            item = default;
            return false;
        }

        item = Dequeue();
        return true;
    }

    public T Peek()
    {
        if (Count == 0)
            throw new InvalidOperationException("Queue has no items");

        return Items[Head];
    }

    public T Peek(int index)
    {
        return this[index];
    }

    //Stack behaviour
    public T PeekLast()
    {
        var lastIndex = (Head + Count - 1) % Capacity;
        return Items[lastIndex];
    }
}

