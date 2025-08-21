#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.Collections;

/// <summary>
/// Represent a fixed size list
/// implement capabilities of <see cref="List{T}"/>, <see cref="Queue{T}"/> and <see cref="Stack{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public class FixedList<T> : IList<T>, IEnumerable<T>, IEnumerable
{
    private T[] _items;
    private int Head { get; set; }
    public int Capacity { get; protected set; }
    public int Count { get; protected set; }
    public bool IsFull => Count == Capacity;
    public bool IsReadOnly => false;

    public FixedList(int capacity)
    {
        Guard.MustBe.GreaterThanOrEqual(capacity, 0);
        Capacity = capacity;
        _items = new T[capacity];
        Count = 0;
        Head = 0;
    }

    public void EnsureCapacity(int capacity)
    {
        if (Capacity >= capacity)
            return;

        Capacity = capacity;
        var arr = new T[capacity];

        for (var i = 0; i < _items.Length; i++)
            arr[i] = _items[i];
            
        _items = arr;
    }

    #region Add / Insert / Put
    public void Add(T item)
    {
        if (Count >= Capacity)
            throw new ExceedCapacityException($"FixedList reached a max number of elements ({Capacity})");

        _items[(Head + Count) % Capacity] = item;
        Count++;
    }

    private void Add(T item, bool force)
    {
        if (Count < Capacity)
        {
            _items[(Head + Count) % Capacity] = item;
            Count++;
            return;
        }

        if (!force)
            throw new ExceedCapacityException($"FixedList reached a max number of elements ({Capacity})");

        _items[Head] = item;
        Head = (Head + 1) % Capacity;
    }

    public bool TryAdd(T item)
    {
        if (Count < Capacity)
        {
            _items[(Head + Count) % Capacity] = item;
            Count++;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Put an item on top of the list (if item already exists, moves it to the end of the list (queue))
    /// </summary>
    public void Put(T item)
    {
        var ind = IndexOf(item);

        if (ind == -1 || !IsFull)
        {
            Add(item, force: true);
            return;
        }

        if (IsFull)
        {
            // if item is already at the end of the list (queue) do nothing
            if (ind == Count - 1)
                return;

            if (ind == 0)
            {
                //e.g. head = 1, item=2 [1,*2,3,4,5,6] => [1,2,*3,4,5,6];
                Head = (Head + 1) % Capacity;
                return;
            }
        }

        //head = 0 [*1,2,(3),4,5,6] item=3 (ind:2) => [*1,2,|4,5,6|,(3)];
        if (Head == 0)
        {
            Array.Copy(_items, ind + 1, _items, ind, Count - ind - 1);
            _items[^1] = item;
            return;
        }

        RemoveAt(ind);
        Add(item, true);
    }

    public void Insert(int index, T item)
    {
        if (Count >= Capacity)
            throw new ExceedCapacityException($"FixedList reached a max number of elements ({Capacity})");

        if (Count < Capacity && Head == 0)
        {
            Array.Copy(_items, index, _items, index + 1, Count - index);
            Count++;
        }
        else if (index == 0)
        {
            _items[Head] = item;
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
                items[i + shift] = _items[ii];
            }
            Array.Copy(items, 0, _items, 0, Count);
            Head = 0;
        }
    }
    #endregion

    #region IndexOf / Contains

    public int IndexOf(T item)
    {
        for (var i = 0; i < Count; i++)
        {
            var currentItem = _items[(Head + i) % Capacity];
            if (EqualityComparer<T>.Default.Equals(currentItem, item))
            {
                return i;
            }
        }
        return -1;
    }

    public bool Contains(T item)
    {
        if (Count == 0)
            return false;

        if (Head == 0)
            return Array.IndexOf(_items, item, Head, Count) >= 0;

        return
            Array.IndexOf(_items, item, Head, _items.Length - Head) >= 0 ||
            Array.IndexOf(_items, item, 0, Capacity - Count) >= 0;
    } 
    #endregion

    #region Push/Pop
    public void Push(T item)
    {
        if (Count >= Capacity)
            throw new ExceedCapacityException($"FixedList reached a max number of elements ({Capacity})");

        _items[Head] = item;
        Head = (Head + 1) % Capacity;
        Count++;
    }

    public T Pop()
    {
        if (Count == 0)
            throw new InvalidOperationException("List has no items");

        //stack Add: 1,2,3 => [1,2,3]; Pop()=> 3,2,1
        //queue Add: 1,2,3 => [1,2,3]; Dequeue() =>1,2,3
        Count--;
        var index = (Head + Count - 1) % Capacity;
        var result = _items[index];
        _items[index] = default!;
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
    #endregion

    #region Enqueue/Dequeue

    /// <summary>
    /// Adds item to the end of the list (queue)
    /// </summary>
    public void Enqueue(T item)
    {
        Add(item);
    }

    /// <summary>
    /// Removes and returns object from the beginning of the list (queue)
    /// </summary>
    public T Dequeue()
    {
        if (Count == 0)
            throw new InvalidOperationException("List has no items");

        var result = _items[Head];
        _items[Head] = default!;

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
    #endregion

    #region Peek
    public T Peek()
    {
        if (Count == 0)
            throw new InvalidOperationException("Queue has no items");

        return _items[Head];
    }

    public T Peek(int index)
    {
        Guard.MustBe.LessThanOrEqual(index, Count);
        return this[index];
    }

    public T PeekFromEnd(int offset)
    {
        Guard.MustBe.LessThanOrEqual(offset, Count);
        var index = Count - offset;
        return this[index];
    }

    //Stack behaviour
    public T PeekLast()
    {
        var lastIndex = (Head + Count - 1) % Capacity;
        return _items[lastIndex];
    }

    #endregion

    public T this[int index]
    {
        get
        {
            Guard.MustBe.WithinRange(index, 0, Capacity - 1);
            return _items[(Head + index) % Capacity];
        }
        set => _items[(Head + index) % Capacity] = value;
    }

    #region Remove / Clear
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
            Array.Copy(_items, index + 1, _items, index, Count - index);
        }
        else
        {
            var items = new T[Capacity];

            for (var i = 0; i < Count - 1; i++)
            {
                if (i < index)
                {
                    var ind = (Head + i) % Capacity;
                    items[i] = _items[ind];
                }
                else
                {
                    var ind = (Head + i + 1) % Capacity;
                    items[i] = _items[ind];
                }
            }

            Count--;
            Array.Copy(items, 0, _items, 0, Count);
            Head = 0;
        }
    }

    public void Clear()
    {
        Array.Clear(_items, 0, _items.Length);
        Count = 0;
        Head = 0;
    }
    #endregion

    #region GetEnumerator
    public IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < Count; i++)
        {
            yield return _items[(Head + i) % Capacity];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    #endregion

    public T[] ToArray()
    {
        var arr = new T[Count];

        if (Count == 0)
            return arr;

        //if (Head < _tail)
        //{
        //    Array.Copy(_array, _head, arr, 0, _size);
        //}

        //[1,2,*3,4,5,6]  6-2=3, Head=2
        //[3,4,5,6,..] // copy [1,2]
        Array.Copy(_items, Head, arr, 0, _items.Length - Head);
        Array.Copy(_items, 0, arr, _items.Length - Head, Head);
        return arr;
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
}

