using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Abstractions.Collections;

namespace AVS.CoreLib.Collections;

public class Accumulator<T> : IAccumulator<T>
{
    public RecordSet<T> Records { get; }

    public Func<T, bool>? Filter { get; set; }

    public Accumulator(int capacity, IComparer<T> comparer)
    {
        Records = new RecordSet<T>(capacity, comparer)
        {
            OnDuplicate = HandleDuplicate
        };
    }

    protected virtual int HandleDuplicate(T item, T other)
    {
        // do nothing, i.e. it's OK to omit the duplicate
        return 0;
    }

    public void Add(T item)
    {
        Records.Add(item);
    }

    public int AddRecords(IList<T> records)
    {
        return Records.AddRange(records);
    }

    public List<T> ToList()
    {
        var records = Filter == null
            ? Records.ToList()
            : Records.Where(Filter).ToList();

        return records;
    }

    public int Count => Records.Count;

    public IEnumerator<T> GetEnumerator()
    {
        return Records.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public T[] ToArray()
    {
        var records = Filter == null
            ? Records.ToArray()
            : Records.Where(Filter).ToArray();
        return records;
    }
}