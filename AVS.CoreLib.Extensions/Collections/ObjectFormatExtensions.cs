using System;
using System.Collections;
using System.Collections.Generic;
using AVS.CoreLib.Extensions.Linq;

namespace AVS.CoreLib.Extensions.Collections;

/// <summary>
/// Helps to format values within a collection/enumerable
/// <code>
/// var data = GetData(); the data could be either IList{T} or IDictionary{string, T} or even nested and untyped IEnumerable{IDictionary{string, object}}
/// items.FormatValues(dec => dec.Round(), str=> str.ToString("C"), dateTime => dateTime.Round());
/// </code>
/// </summary>
public static class FormatExtensions
{
    public static void FormatValues<T>(this IEnumerable enumerable, Func<T, T> fn)
    {
        enumerable.ForEach(x =>
        {
            switch (x)
            {
                //List<decimal> [0]: [1,2,3..]
                case IList<T> typedList:
                    typedList.FormatValues(fn);
                    break;
                //Dict<string, decimal> [0]: {close: 100, ...}
                case IDictionary<string, T> typedDict:
                    typedDict.FormatValues(fn);
                    break;
                //Dict<string, object> [0]: {close: 100, type: Sell,...}
                case IDictionary<string, object> dict:
                    dict.FormatDictValues(fn);
                    break;
                //List<object> [0]: {100, Sell,...}
                case IList list:
                    list.FormatListValues(fn);
                    break;
            }
        });
    }

    public static void FormatValues<T1, T2>(this IEnumerable enumerable, Func<T1, T1> fn1, Func<T2, T2> fn2)
    {
        enumerable.ForEach(x =>
        {
            switch (x)
            {
                //List<decimal> [0]: [1,2,3..]
                case IList<T1> typedList:
                    typedList.FormatValues(fn1);
                    break;
                case IList<T2> typedList:
                    typedList.FormatValues(fn2);
                    break;
                //Dict<string, decimal> [0]: {close: 100, ...}
                case IDictionary<string, T1> typedDict:
                    typedDict.FormatValues(fn1);
                    break;
                case IDictionary<string, T2> typedDict:
                    typedDict.FormatValues(fn2);
                    break;
                //Dict<string, object> [0]: {close: 100, type: Sell,...}
                case IDictionary<string, object> dict:
                    dict.FormatDictValues(o => o switch
                    {
                        T1 v1 => fn1(v1)!,
                        T2 v2 => fn2(v2)!,
                        _ => o!
                    });
                    break;
                //List<object> [0]: {100, Sell,...}
                case IList list:
                    list.FormatListValues(o => o switch
                    {
                        T1 v1 => fn1(v1)!,
                        T2 v2 => fn2(v2)!,
                        _ => o!
                    });
                    break;
            }
        });
    }

    public static void FormatValues<T1, T2, T3>(this IEnumerable enumerable, Func<T1, T1> fn1, Func<T2, T2> fn2, Func<T3, T3> fn3)
    {
        enumerable.ForEach(x =>
        {
            switch (x)
            {
                //List<decimal> [0]: [1,2,3..]
                case IList<T1> typedList:
                    typedList.FormatValues(fn1);
                    break;
                case IList<T2> typedList:
                    typedList.FormatValues(fn2);
                    break;
                case IList<T3> typedList:
                    typedList.FormatValues(fn3);
                    break;
                //Dict<string, decimal> [0]: {close: 100, ...}
                case IDictionary<string, T1> typedDict:
                    typedDict.FormatValues(fn1);
                    break;
                case IDictionary<string, T2> typedDict:
                    typedDict.FormatValues(fn2);
                    break;
                case IDictionary<string, T3> typedDict:
                    typedDict.FormatValues(fn3);
                    break;
                //Dict<string, object> [0]: {close: 100, type: Sell,...}
                case IDictionary<string, object> dict:
                    dict.FormatDictValues(o => o switch
                    {
                        T1 v1 => fn1(v1)!,
                        T2 v2 => fn2(v2)!,
                        T3 v3 => fn3(v3)!,
                        _ => o!
                    });
                    break;
                //List<object> [0]: {100, Sell,...}
                case IList list:
                    list.FormatListValues(o => o switch
                    {
                        T1 v1 => fn1(v1)!,
                        T2 v2 => fn2(v2)!,
                        T3 v3 => fn3(v3)!,
                        _ => o!
                    });
                    break;
            }
        });
    }

    public static IList<T> FormatValues<T>(this IList<T> list, Func<T, T> fn)
    {
        for (var i = 0; i < list.Count; i++)
            list[i] = fn(list[i]);
        return list;
    }

    public static IDictionary<string, T> FormatValues<T>(this IDictionary<string, T> dict, Func<T, T> fn)
    {
        foreach (var kp in dict)
            dict[kp.Key] = fn(kp.Value)!;
        return dict;
    }
}

internal static class FormatListValuesExtensions
{
    internal static IList FormatListValues(this IList list, Func<object, object> fn)
    {
        for (var i = 0; i < list.Count; i++)
            list[i] = fn(list[i]!);
        return list;
    }

    internal static IList FormatListValues<T>(this IList list, Func<T, T> fn)
    {
        for (var i = 0; i < list.Count; i++)
            if (list[i] is T val)
                list[i] = fn(val);
        return list;
    }
}

internal static class FormatDictValuesExtensions
{
    internal static IDictionary<string, object> FormatDictValues(this IDictionary<string, object> dict, Func<object, object> fn)
    {
        foreach (var kp in dict)
        {
            dict[kp.Key] = fn(kp.Value)!;
        }
        return dict;
    }

    internal static IDictionary<string, object> FormatDictValues<T>(this IDictionary<string, object> dict, Func<T, T> fn)
    {
        foreach (var kp in dict)
        {
            if (kp.Value is T v1)
                dict[kp.Key] = fn(v1)!;
        }
        return dict;
    }
}