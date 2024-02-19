using System.Collections;
using System.Collections.Generic;

namespace AVS.CoreLib.Collections;

public class ReverseEnumerator<TItem> : IEnumerator<TItem>
{
    private readonly List<TItem> _items;
    private int _currentIndex;

    public ReverseEnumerator(List<TItem> items)
    {
        _items = items;
        Reset();
    }

    public TItem Current => _items[_currentIndex];

    object IEnumerator.Current => Current!;

    public void Dispose()
    {
        _items.Clear();
    }

    public bool MoveNext()
    {
        return --_currentIndex >= 0;
    }

    public void Reset()
    {
        _currentIndex = _items.Count;
    }
}