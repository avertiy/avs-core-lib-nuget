using System;
using System.Reflection;
using AVS.CoreLib.Extensions.Reflection;

namespace AVS.CoreLib.DLinq;

/// <summary>
/// represent selector helper
/// </summary>
public static class DynamicSelector
{
    /// <summary>
    /// resolve properties that match selector expression
    /// fow now only basic syntax & capabilities are supported
    /// <code>
    /// //selector expression capabilities:
    /// //1. ignore case
    ///     close or Close // => Close property 
    /// //2. x.Close is ok
    ///     x.close // => Close property
    /// //3. multi properties are OK
    ///     close,high,time // => 3 properties: Close, High, Time
    ///     x.close,x.high,x.time // => 3 properties: Close, High, Time
    ///     close,high,time,prop1 // => 3 properties: Close, High, Time (not existing props are ignored)
    /// // *
    ///   "" or * // => all public properties e.g. Open,High,Low,Close,Time 
    /// </code>
    /// - case in-sensitive (ignore case)
    /// </summary>
    public static PropertyInfo[] LookupProperties(Type type, string? selectExpression)
    {
        var expr = selectExpression == null ? string.Empty : selectExpression.Replace("x.", "");
        return type.SearchProperties(expr, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
    }
}