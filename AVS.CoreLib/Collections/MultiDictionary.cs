using System.Collections.Generic;

namespace AVS.CoreLib.Collections;

public sealed class MultiDictionary<TKey, TValue> : BaseDictionary<TKey, IList<TValue>> where TKey : notnull
{
    public MultiDictionary()
    {
    }

    public void Add(TKey key, TValue item)
    {
        if (ContainsKey(key))
        {
            this[key].Add(item);
        }
        else
        {
            this[key] = new List<TValue>() { item };
        }
    }

    public bool TryGetItem(TKey key, out TValue? item, int index = 0)
    {
        item = default;
        if (!ContainsKey(key))
            return false;

        var list = this[key];

        if (list.Count <= index)
            return false;

        item = list[index];
        return true;
    }
}