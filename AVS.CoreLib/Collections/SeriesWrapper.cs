using System.Collections;
using System.Collections.Generic;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.Collections;

/// <summary>
/// Series provide reverse enumerator to enumerate in direct order (0,1,2,...) use Items property
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISeries<out T> : IEnumerable<T>
{
    int Count { get; }
    T this[int offset] { get; }
    T Offset(int offset);
}

/// <summary>
/// Represent a wrapper (hat) over list collection, helps to access items in reverse order aka series collection
/// </summary>
public class SeriesWrapper<T> : ISeries<T>, IEnumerable<T>
{
    public IList<T> Items { get; }

    public int Count => Items.Count;

    public SeriesWrapper(IList<T> source)
    {
        Items = source;
    }

    /// <summary>
    /// reverse indexer i.e. series[0] returns the last item: Items[^1]
    /// </summary>
    public T this[int offset]
    {
        get
        {
            Guard.MustBe.WithinRange(offset, 0, Items.Count - 1);
            return Items[(Items.Count - 1) - offset];
        }

        set
        {
            Guard.MustBe.WithinRange(offset, 0, Items.Count - 1);
            Items[(Items.Count - 1) - offset] = value;
        }
    }

    public T Offset(int offset)
    {
        Guard.MustBe.WithinRange(offset, 0, Items.Count - 1);
        return Items[(Items.Count - 1) - offset];
    }

    public IEnumerator<T> GetEnumerator()
    {
        return new ReverseEnumerator<T>(Items);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public static class ListExtensions
{
    public static ISeries<T> AsSeries<T>(this IList<T> source)
    {
        return new SeriesWrapper<T>(source);
    }
}