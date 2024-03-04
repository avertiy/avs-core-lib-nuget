using System.Collections;
using System.Text.Json.Serialization;
using AVS.CoreLib.DLinq;

namespace AVS.CoreLib.DLinq;
/*
/// <summary>
/// Represent an untyped wrapper over <see cref="IEnumerable{T}"/> 
/// </summary>
public interface IBox
{
    IList ToList();
    IEnumerable GetData();
    Type GetDataType();
    Type GetItemType();
    IEnumerable<TResult> Select<TResult>(Func<dynamic, TResult> selector);
    bool Is<TData>(Action<TData>? action = null, Action? @else = null);
}

public class EnumerableBox<T> : IBox, IEnumerable<T>
{
    [JsonIgnore]
    public IEnumerable<T> Data { get; private set; }

    public EnumerableBox(IEnumerable<T> data)
    {
        Data = data;
    }

    public IEnumerable GetData() => Data;

    public IList ToList() => Data.ToList();

    public IEnumerator GetEnumerator()
    {
        return Data.GetEnumerator();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return Data.GetEnumerator();
    }

    public Type GetDataType()
    {
        return Data.GetType();
    }

    public Type GetItemType()
    {
        return typeof(T);
    }

    public IEnumerable<TResult> Select<TResult>(Func<dynamic, TResult> selector)
    {
        return Data.Select(x => selector(x!));
    }

    public bool Is<TData>(Action<TData>? action = null, Action? @else = null)
    {
        if (Data is TData data)
        {
            action?.Invoke(data!);
            return true;
        }
        @else?.Invoke();
        return false;
    }
}

public static class BoxExtensions
{
    public static IBox Box<T>(this IEnumerable<T> source) => new EnumerableBox<T>(source);
}*/