using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AVS.CoreLib.Collections;

[DebuggerDisplay("{ToString()}")]
public sealed class MultiDictionary<TKey, TValue> : BaseDictionary<TKey, IList<TValue>> where TKey : notnull
{
    public int TotalCount { get; private set; }

    public MultiDictionary()
    {
    }

    public void Add(TKey key, TValue item)
    {
        TotalCount++;
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

    public override bool Remove(TKey key)
    {
        if (Data.TryGetValue(key, out var list))
        {
            TotalCount -= list.Count;
            Data.Remove(key);
            return true;
        }

        return false;
    }

    public override string ToString()
    {
        var sb = new StringBuilder(TotalCount * 10 + 30);
        sb.Append($"MultiDictionary<{typeof(TKey).Name},{typeof(TValue).Name}> ");

        foreach (var kp in Data)
        {
            if (kp.Value.Count > 0)
            {
                sb.Append($"\"{kp.Key}\":[");

                foreach (var value in kp.Value)
                {
                    sb.Append($"{value},");
                }

                sb.Length--;
                sb.Append("] ");
            }
            else
            {
                sb.Append($"{kp.Key}:[] ");
            }
        }

        return sb.ToString();
    }
}