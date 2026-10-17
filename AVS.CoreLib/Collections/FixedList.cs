#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AVS.CoreLib.Abstractions.Collections;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Extensions.Reflection;
using AVS.CoreLib.Extensions.Stringify;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.Collections;


/// <summary>
/// Represent a fixed size list
/// implement capabilities of <see cref="List{T}"/>, <see cref="Queue{T}"/> and <see cref="Stack{T}"/>
/// </summary>
[DebuggerDisplay("{ToString()}")]
public class FixedList<T> : IList<T>, IQueue<T>, IStack<T>, IEnumerable<T>, IEnumerable
{
    private T[] _items;
    /// <summary>
    /// when non-zero indicates queue / stack behavior has been applied, that shifted Head pointer
    /// </summary>
    public int Head { get; private set; }
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

    public FixedList(T[] items)
    {
        _items = items;
        Capacity = items.Length;
        Count = items.Length;
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

    #region Add / ForceAdd / TryAdd / Insert

    public void Add(T item)
    {
        if (Count >= Capacity)
            throw new InvalidOperationException($"FixedList reached max number of elements (capacity:{Capacity})");

        _items[(Head + Count) % Capacity] = item;
        Count++;
    }

    public T? ForceAdd(T item)
    {
        var ind = (Head + Count) % Capacity;
        T? removed = default;
        
        if (Count == Capacity)
            removed = _items[ind];
        else
            Count++;

        _items[ind] = item;
        return removed;
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
    /// Inserts an element into the list at a given index.
    /// </summary>
    /// <exception cref="InvalidOperationException">Throws an exception if count reached capacity</exception>
    public void Insert(int index, T item)
    {
        if (index < 0 || index > Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Index must be within the bounds of the list.");

        if (Count == Capacity)
            throw new InvalidOperationException($"FixedList reached max number of elements ({Capacity})");

        // Head = 0 - simple (list) behaviour
        if (Head == 0)
        {
            Array.Copy(_items, index, _items, index + 1, Count - index);
            _items[index] = item;
            Count++;
            return;
        }

        // Head > 0 - Q/Stack behaviour i.e. either Push/Dequeue/Put methods being called  
        if (index == 0)
        {
            _items[Head-1] = item;
            Count++;
            Head--;
            return;
        }

        if (Count == 1)
        {
            _items[0] = _items[Head];
            _items[index] = item;
            Head = 0;
            Count++;
            return;
        }

        // [0,0,1,3,4] Head =2 => {1,3,4}.Insert(index:2, item:10) => [1,3,_10_,4,0] Head = 0
        var items = new T[Capacity];
        items[index] = item;

        for (var i = 0; i < Count; i++)
        {
            if (i < index)
            {
                items[i] = _items[(Head + i) % Capacity];
                continue;
            }

            items[i+1] = _items[(Head + i) % Capacity];
        }

        Head = 0;
        Count++;
        _items = items;
    }

    #endregion

    #region Put

    /// <summary>
    /// Put an item on top of the list, if item already exists, moves it to the end of the list
    /// NOTE! helps to maintain unique items in the list
    /// </summary>
    public void Put(T item)
    {
        var ind = IndexOf(item);

        // if item is already at the end of the list (queue) do nothing
        if (ind == Count - 1 && IsFull)
            return;

        // if item is on top:  head = 1, item=2 [1,*2,3,4,5,6]  Queue=[2,3,4,5,6,1]
        // move it to the end:  => [1,2,*3,4,5,6];
        if (ind == 0 && IsFull)
        {
            Head = (Head + 1) % Capacity;
            return;
        }

        //head = 0 [*1,2,(3),4,5,6] item=3 (ind:2) => [*1,2,|4,5,6|,(3)];
        if (Head == 0 && ind > -1)
        {
            Array.Copy(_items, ind + 1, _items, ind, Count - ind - 1);
            _items[^1] = item;
            return;
        }

        if (ind > -1 && IsFull)
            RemoveAt(ind);

        if (Count < Capacity)
        {
            _items[(Head + Count) % Capacity] = item;
            Count++;
        }
        else
        {
            _items[Head] = item;
            Head = (Head + 1) % Capacity;
        }
    }

    #endregion

    #region IndexOf / Contains

    public int IndexOf(T item) 
    {
        var ind = Array.IndexOf(_items, item, Head, Count - Head);

        if (ind == -1 && Count > Capacity - Head) // check second range if applicable
            ind = Array.IndexOf(_items, item, 0, Count - (Capacity - Head)); 

        if (ind >= 0)
        {
            var index = (Head + ind) % Capacity;
            return index;
        }

        //    for (var i = 0; i < Count; i++)
        //    {
        //        var currentItem = _items[(Head + i) % Capacity];
        //        if (EqualityComparer<T>.Default.Equals(currentItem, item))
        //        {
        //            return i;
        //        }
        //    }

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

    public bool ContainsAny(params T[] items)
    {
        if (Count == 0)
            return false;

        if (Head == 0)
        {
            foreach (var item in items)
                if (Array.IndexOf(_items, item, Head, Count) >= 0)
                    return true;
        }
        else
        {
            foreach (var item in items)
            {
                if (Array.IndexOf(_items, item, Head, _items.Length - Head) >= 0)
                    return true;
                if (Array.IndexOf(_items, item, 0, Capacity - Count) >= 0)
                    return true;
            }
        }

        return false;
    }

    #endregion

    #region IStack<T>

    public T? Push(T item)
    {
        T? removed = default;
        if (Count == Capacity)
            removed = Pop();

        PushStrict(item);
        return removed;
    }

    public void PushStrict(T item)
    {
        if (Count == Capacity)
            throw new InvalidOperationException($"FixedList reached a max number of elements ({Capacity})");

        if (Head == 0)
        {
            Head = Capacity - 1;
            _items[Head] = item;
            Count++;
            return;
        }

        //[_,_,_]; 1. [_,_,10]; 2. [_,10,20]; 3. [10,20,30]
        // H:0          H=2          H=1            H=0

        Head--;
        _items[Head] = item;
        Count++;
    }

    public T Pop()
    {
        if (Count == 0)
            throw new InvalidOperationException("FixedList has no items");

        // stack (LIFO): Push([4,3,2,1]) => _items[1,2,3,4];  output => 4,3,2,1
        // queue (FIFO): Enqueue([1,2,3,4]); _items[1,2,3,4]; output => 1,2,3,4

        // NOTE! Head points to the top element in the stack: 1 was pushed the latest to the stack, Head=0, _items[0] => 1;

        //1) C=4/4
        // H=0:_items[1,2,3,4];  H=1: _items[4,1,2,3]
        // stack output => 4,3,2,1; queue output: 1,2,3,4

        //2) C=3/4
        // H=1:_items[_,2,3,4];  H=2: _items[4,_,2,3]   
        // stack output => 4,3,2; queue output: 2,3,4

        //3) C=2/4
        // H=2:_items[_,_,3,4];  H=3: _items[4,_,_,3]   
        // stack output => 4,3; queue output: 3,4

        //i = 4-4+1 = 1; 4-3+1=2
        var ind = Head;
        var item = _items[ind];
        _items[ind] = default!;
        Count--;
        Head = (Head + 1) % Capacity;
        return item;
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

    #region IQueue<T>

    /// <summary>
    /// Adds an item to the end of the list (queue).
    /// If the list is full, removes and returns the oldest item.
    /// Otherwise, returns default(T).
    /// </summary>
    public T? Enqueue(T item)
    {
        T? removed = default;

        if (Count == Capacity)
            removed = Dequeue();

        Add(item);
        return removed;
    }

    /// <summary>
    /// Adds item to the end of the queue,
    /// </summary>
    /// <exception cref="InvalidOperationException">if queue <see cref="IsFull"/> throws</exception>
    public void EnqueueStrict(T item)
    {
        if (Count == Capacity)
            throw new InvalidOperationException($"FixedList reached max number of elements (capacity:{Capacity})");

        Add(item);
    }

    /// <summary>
    /// Removes and returns element from the beginning of the list (queue)
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
    /// <summary>
    /// Returns the object at the head of the queue. The object remains in the queue. 
    /// </summary>
    /// <exception cref="InvalidOperationException">If the queue is empty throws an exception</exception>
    public T Peek()
    {
        if (Count == 0)
            throw new InvalidOperationException("FixedList has no items");

        return _items[Head];
    }

    /// <summary>
    /// Returns element by offset. The element remains in the queue.
    /// e.g. Q:{1,2,3,4}; Q.Peek(offset:1) => 3  
    /// </summary>
    /// <exception cref="InvalidOperationException">If the queue is empty throws an exception</exception>
    public T Peek(int offset)
    {
        Guard.MustBe.LessThan(offset, Count);
        if (Count == 0)
            throw new InvalidOperationException("FixedList has no items");

        return _items[(Head + Count - 1 - offset) % Capacity];
    }

    public T PeekFromEnd(int offset)
    {
        Guard.MustBe.LessThan(offset, Count);
        var index = Count - (offset+1);
        return this[index];
    }

    //Stack behaviour
    public T PeekLast()
    {
        if (Count == 0)
            throw new InvalidOperationException("FixedList has no items");

        var lastIndex = (Head + Count - 1) % Capacity;
        return _items[lastIndex];
    }

    #endregion

    /// <summary>
    /// <code>
    ///  var fixedList = new FixedList(3);
    ///  fixedList.Add(1); // [*1]       Head=0
    ///  fixedList.Add(2); // [*1,2]     Head=0
    ///  fixedList.Add(3); // [*1,2,3]   Head=0
    ///
    ///  fixedList[0] => 1
    ///  fixedList[2] => 3
    /// 
    ///  fixedList.Put(4); // [4,*2,3]   Head=1
    ///  
    ///  fixedList[0] => 2
    ///  fixedList[2] => 4
    /// </code>
    /// </summary>
    public T this[int index]
    {
        get
        {
            Guard.MustBe.WithinRange(index, 0, Count-1);
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

    /// <summary>
    /// sorts in ascending order (from smallest to largest)
    /// </summary>
    public void Sort()
    {
        if (Count < 2)
            return;

        if (Head == 0)
        {
            Array.Sort(_items, 0, Count);
            return;
        }

        var arr = ToArray();
        Array.Sort(arr, 0, arr.Length);
        Head = 0;
        Array.Copy(arr, 0, _items, 0, arr.Length);
    }

    /// <summary>
    /// sorts in descending order (from largest to smallest)
    /// </summary>
    public void Reverse()
    {
        if (Count < 2)
            return;

        if (Head == 0)
        {
            Array.Reverse(_items, 0, Count);
            return;
        }

        var arr = ToArray();
        Array.Reverse(arr, 0, arr.Length);
        Head = 0;
        Array.Copy(arr, 0, _items, 0, arr.Length);
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
        if (Count == 0)
            return Array.Empty<T>();

        var arr = new T[Count];
        
        if (Count == 1)
        {
            arr[0] = _items[Head];
            return arr;
        }

        //regardles stack / queue behaviour ToArray() output will copy the internal representation 

        //1) H=0
        // _items[1,2,3,4,5,6];  => [1,2,3,4,5,6]

        //2a) H=2, Count=4
        // _items[1,2,*3,4,5,6] => [3,4,5,6,_,_] // because Count =4

        //2b) H=2, Count=5
        // _items[1,2,*3,4,5,6] => [3,4,5,6,1,_] // because Count =5

        // copy the first part 3,4,5,6 => [3,4,5,6,_,_] 
        Array.Copy(_items, Head, arr, 0, length: _items.Length - Head);

        // copy the rest 1,2 if applicable
        if (Count > Capacity - Head)
            Array.Copy(_items, 0, arr, _items.Length - Head, Count - (Capacity - Head));

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


    public override string ToString()
    {
        var str = AsString().Truncate();
        return $"FixedList<{typeof(T).GetReadableName()}> #{Count} of {Capacity} {str}";
    }

    public string AsString()
    {
        var arr = ToArray();
        return arr.Stringify();
    }
}