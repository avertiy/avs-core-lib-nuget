using System.Collections;
using AVS.CoreLib.Extensions.Linq;

namespace AVS.CoreLib.Extensions.Dynamic;

public static class CountExtensions
{
    /// <summary>
    /// if object either is countable i.e. contains Count property or it is IEnumerable or it contains Data property with Count like Response.Data.Count or Response.Data.Items.Count 
    /// </summary>
    public static int? GetCount<T>(this T? obj)
    {
        if (obj == null)
            return null;

        // test Count property
        if (TryGetCountProperty(obj, out var count))
            return count;

        if (obj is IEnumerable col)
            return col.Count();

        // test Data property
        var data = TryGetData(obj);
        if (data == null)
            return null;

        // test Data?.Count 
        if (TryGetCountProperty(data, out count))
            return count;

        // test Data.Items?.Count 
        var items = TryGetItems(data);

        if (TryGetCountProperty(items, out count))
            return count;

        return null;
    }

    public static bool TryGetCount<T>(this T? obj, out int count)
    {
        count = 0;
        if (obj == null)
            return false;

        // test Count property
        if (TryGetCountProperty(obj, out count))
            return true;

        if (obj is IEnumerable col)
        {
            count = col.Count();
            return true;
        }

        // test Data property
        var data = TryGetData(obj);
        if (data == null)
            return false;

        // test Data?.Count 
        if (TryGetCountProperty(data, out count))
            return true;

        // test Data.Items?.Count 
        var items = TryGetItems(data);
        return TryGetCountProperty(items, out count);
    }

    public static bool TryGetLength(dynamic obj, out int length)
    {
        // Attempt to get the "Count" property from the dynamic object
        try
        {
            length = obj.Length;
            return true;
        }
        catch
        {
            length = 0;
            return false;
        }
    }

    private static bool TryGetCountProperty(dynamic obj, out int count)
    {
        // Attempt to get the "Count" property from the dynamic object
        try
        {
            count = obj.Count;
            return true;
        }
        catch
        {
            count = 0;
            return false;
        }
    }

    private static dynamic? TryGetData(dynamic obj)
    {
        try
        {
            return obj.Data;
        }
        catch
        {
            return null;
        }
    }

    private static dynamic? TryGetItems(dynamic obj)
    {
        try
        {
            return obj.Items;
        }
        catch
        {
            return null;
        }
    }
}