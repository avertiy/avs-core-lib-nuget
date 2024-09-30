using System;
using System.Collections;
using System.Collections.Generic;

namespace AVS.CoreLib.Extensions.Linq;

public class CastIterator<T, TResult>(IEnumerable<T> items) : IEnumerable<TResult>
{
    public IEnumerator<TResult> GetEnumerator()
    {
        if (items is IEnumerable<TResult> result)
            return result.GetEnumerator();

        return GetEnumeratorInternal();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private IEnumerator<TResult> GetEnumeratorInternal()
    {
        foreach (var item in items)
        {
            if (item is TResult res)
                yield return res;
        }
    }
}

public class CastIterator<TResult>(IEnumerable items, Func<object, TResult> cast) : IEnumerable<TResult>
{
    public IEnumerator<TResult> GetEnumerator()
    {
        if (items is IEnumerable<TResult> result)
            return result.GetEnumerator();

        return GetEnumeratorInternal();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private IEnumerator<TResult> GetEnumeratorInternal()
    {
        foreach (var item in items)
            yield return cast(item);
    }
}