using System;
using System.Collections;
using System.Collections.Generic;

namespace AVS.CoreLib.Extensions.Collections;

/// <summary>
/// Helps to format values within a collection (list/dictionary)
/// <code>
/// var data = GetData();
/// //if data is collection contains values of decimal or DateTime e.g. data is List{Dictionary{string,object}}
/// items.FormatValues{decimal, DateTime}(dec => dec.Round(), dateTime => dateTime.Round());
/// </code>
/// </summary>
public static class FormatValuesExtensions
{
    public static void FormatValues<T>(this IEnumerable enumerable, Func<T, T> fn)
    {
        switch (enumerable)
        {
            //e.g. List<decimal> [0]: [1,2,3..]
            case IList<T> list1:
                list1.FormatListValues(fn);
                break;
            case IList<IList<T>> listOfListValues:
                foreach (var list in listOfListValues)
                    list.FormatListValues(fn);
                break;
            //e.g. Dict<string, decimal> [0]: {close: 100, ...}
            case IList<IDictionary<string, T>> listOfDict:
                foreach (var dict in listOfDict)
                    dict.FormatDictValues(fn);
                break;
            //e.g. Dict<string, object> [0]: {close: 100, type: Sell,...}
            case IList<IDictionary<string, object>> listOfDict:
                foreach (var dict in listOfDict)
                    dict.FormatDictValues(fn);
                break;
            case IList<object> list:
                list.FormatListValues(fn);
                break;
            case IDictionary<string, T> dict:
                dict.FormatDictValues(fn);
                break;
            case IDictionary<string, object> dict:
                dict.FormatDictValues(fn);
                break;
            default:
                foreach (var item in enumerable)
                {
                    if (item is IEnumerable enumerable2)
                    {
                        enumerable2.FormatValues(fn);
                        continue;
                    }
                    break;
                }
                break;
        }
    }

    public static void FormatValues<T1, T2>(this IEnumerable enumerable, Func<T1, T1> fn1, Func<T2, T2> fn2)
    {
        switch (enumerable)
        {
            //e.g. List<decimal> [0]: [1,2,3..]
            case IList<T1> list1:
                list1.FormatListValues(fn1);
                break;
            case IList<T2> list1:
                list1.FormatListValues(fn2);
                break;
            case IList<IList<T1>> listOfListValues:
                foreach (var list in listOfListValues)
                    list.FormatListValues(fn1);
                break;
            case IList<IList<T2>> listOfListValues:
                foreach (var list in listOfListValues)
                    list.FormatListValues(fn2);
                break;
            //e.g. Dict<string, decimal> [0]: {close: 100, ...}
            case IList<IDictionary<string, T1>> listOfDict:
                foreach (var dict in listOfDict)
                    dict.FormatDictValues(fn1);
                break;
            case IList<IDictionary<string, T2>> listOfDict:
                foreach (var dict in listOfDict)
                    dict.FormatDictValues(fn2);
                break;
            //e.g. Dict<string, object> [0]: {close: 100, type: Sell,...}
            case IList<IDictionary<string, object>> listOfDict:
                foreach (var dict in listOfDict)
                    dict.FormatDictValues(o => o switch
                    {
                        T1 v1 => fn1(v1)!,
                        T2 v2 => fn2(v2)!,
                        _ => o!
                    });
                break;
            case IList<object> list:
                list.FormatListValues(o => o switch
                {
                    T1 v1 => fn1(v1)!,
                    T2 v2 => fn2(v2)!,
                    _ => o!
                });
                break;
            case IDictionary<string, T1> dict:
                dict.FormatDictValues(fn1);
                break;
            case IDictionary<string, T2> dict:
                dict.FormatDictValues(fn2);
                break;
            case IDictionary<string, object> dict:
                dict.FormatDictValues(o => o switch
                {
                    T1 v1 => fn1(v1)!,
                    T2 v2 => fn2(v2)!,
                    _ => o!
                });
                break;
            default:
                foreach (var item in enumerable)
                {
                    if (item is IEnumerable enumerable2)
                    {
                        enumerable2.FormatValues(fn1, fn2);
                        continue;
                    }
                    break;
                }
                break;
        }
    }

    public static void FormatValues<T1, T2, T3>(this IEnumerable enumerable, Func<T1, T1> fn1, Func<T2, T2> fn2, Func<T3, T3> fn3)
    {
        switch (enumerable)
        {
            //e.g. List<decimal> [0]: [1,2,3..]
            case IList<T1> list1:
                list1.FormatListValues(fn1);
                break;
            case IList<T2> list1:
                list1.FormatListValues(fn2);
                break;
            case IList<T3> list1:
                list1.FormatListValues(fn3);
                break;
            case IList<IList<T1>> listOfListValues:
                foreach (var list in listOfListValues)
                    list.FormatListValues(fn1);
                break;
            case IList<IList<T2>> listOfListValues:
                foreach (var list in listOfListValues)
                    list.FormatListValues(fn2);
                break;
            case IList<IList<T3>> listOfListValues:
                foreach (var list in listOfListValues)
                    list.FormatListValues(fn3);
                break;
            //e.g. Dict<string, decimal> [0]: {close: 100, ...}
            case IList<IDictionary<string, T1>> listOfDict:
                foreach (var dict in listOfDict)
                    dict.FormatDictValues(fn1);
                break;
            case IList<IDictionary<string, T2>> listOfDict:
                foreach (var dict in listOfDict)
                    dict.FormatDictValues(fn2);
                break;
            case IList<IDictionary<string, T3>> listOfDict:
                foreach (var dict in listOfDict)
                    dict.FormatDictValues(fn3);
                break;
            //e.g. Dict<string, object> [0]: {close: 100, type: Sell,...}
            case IList<IDictionary<string, object>> listOfDict:
                foreach (var dict in listOfDict)
                    dict.FormatDictValues(o => o switch
                    {
                        T1 v1 => fn1(v1)!,
                        T2 v2 => fn2(v2)!,
                        T3 v3 => fn3(v3)!,
                        _ => o!
                    });
                break;
            case IList<object> list:
                list.FormatListValues(o => o switch
                {
                    T1 v1 => fn1(v1)!,
                    T2 v2 => fn2(v2)!,
                    T3 v3 => fn3(v3)!,
                    _ => o!
                });
                break;
            case IDictionary<string, T1> dict:
                dict.FormatDictValues(fn1);
                break;
            case IDictionary<string, T2> dict:
                dict.FormatDictValues(fn2);
                break;
            case IDictionary<string, T3> dict:
                dict.FormatDictValues(fn3);
                break;
            case IDictionary<string, object> dict:
                dict.FormatDictValues(o => o switch
                {
                    T1 v1 => fn1(v1)!,
                    T2 v2 => fn2(v2)!,
                    T3 v3 => fn3(v3)!,
                    _ => o!
                });
                break;
            default:
                foreach (var item in enumerable)
                {
                    if (item is IEnumerable enumerable2)
                    {
                        enumerable2.FormatValues(fn1, fn2, fn3);
                        continue;
                    }
                    break;
                }
                break;
        }
    }
}

public static class FormatListValuesExtensions
{
    public static IList<T> FormatListValues<T>(this IList<T> list, Func<T, T> fn)
    {
        for (var i = 0; i < list.Count; i++)
            list[i] = fn(list[i]);
        return list;
    }

    internal static IList FormatListValues(this IList list, Func<object, object> fn)
    {
        for (var i = 0; i < list.Count; i++)
            list[i] = fn(list[i]!);
        return list;
    }

    public static IList<object> FormatListValues<T>(this IList<object> list, Func<T, T> fn)
    {
        for (var i = 0; i < list.Count; i++)
            if (list[i] is T val)
                list[i] = fn(val)!;
        return list;
    }

    internal static IList<object> FormatListValues(this IList<object> list, Func<object, object> fn)
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

public static class FormatDictValuesExtensions
{
    public static IDictionary<string, T> FormatDictValues<T>(this IDictionary<string, T> dict, Func<T, T> fn)
    {
        foreach (var kp in dict)
            dict[kp.Key] = fn(kp.Value)!;
        return dict;
    }

    public static IDictionary<string, object> FormatDictValues<T>(this IDictionary<string, object> dict, Func<T, T> fn)
    {
        foreach (var kp in dict)
        {
            if (kp.Value is T val)
                dict[kp.Key] = fn(val)!;
        }
        return dict;
    }

    internal static IDictionary<string, object> FormatDictValues(this IDictionary<string, object> dict, Func<object, object> fn)
    {
        foreach (var kp in dict)
        {
            dict[kp.Key] = fn(kp.Value)!;
        }
        return dict;
    }
}