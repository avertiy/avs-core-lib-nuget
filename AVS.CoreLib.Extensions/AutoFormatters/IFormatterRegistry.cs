using System;
using System.Linq.Expressions;

namespace AVS.CoreLib.Extensions.AutoFormatters;

/// <summary>
/// The formatter purpose is to help with auto-formatting values
/// Pick formatter by key (the key could be a property name, or type name, or whatever)
/// </summary>
/// <remarks>
/// the idea is completely different from other formatters it is mainly needed when deal with reflection when printing object 
/// </remarks>
public interface IFormatterRegistry
{
    /// <summary>
    /// register format function by key (use property name or `type:propertyName` in case you need similar properties format differently depending on the type owning the property)
    /// </summary>
    void Register(string key, Func<object, string> format);

    /// <summary>
    /// Determines whether the specified key was registered 
    /// </summary>
    bool ContainsKey(string key);

    /// <summary>
    /// get formatter by key
    /// </summary>
    Func<object, string> GetFormatter(string key);
    /// <summary>
    /// get formatter by key
    /// </summary>
    Func<object, string> this[string key] { get; set; }

    void Remove(string key);
    void Clear();
}

/// <summary>
/// <see cref="IFormatterRegistry"/> extensions
/// </summary>
public static class FormatterRegistryExtensions
{
    public static Func<object, string> GetFormatterOrDefault(this IFormatterRegistry registry, string key, string @default)
    {
        return registry.GetFormatter(registry.ContainsKey(key) ? key : @default);
    }

    /// <summary>
    /// Try get formatter
    /// </summary>
    public static bool TryGetFormatter(this IFormatterRegistry registry, string key, out Func<object, string> format)
    {
        if (registry.ContainsKey(key))
        {
            format = registry.GetFormatter(key);
            return true;
        }

        format = default!;
        return false;
    }

    /// <summary>
    /// register format function by key
    /// </summary>
    public static void Register<TProperty>(this IFormatterRegistry registry, string key, Func<TProperty, string> format)
    {
        registry.Register(key, x => format((TProperty)x));
    }

    /// <summary>
    /// register format function by key=`{type}:{propName}` and by key=`{propName}`  
    /// </summary>
    public static void Register<T, TProperty>(this IFormatterRegistry registry, Expression<Func<T, TProperty>> expression, Func<TProperty, string> format)
    {
        var propName = ((MemberExpression)expression.Body).Member.Name;
        var type = typeof(T).Name;

        if (!registry.ContainsKey(propName))
            registry.Register(propName, x => format((TProperty)x));

        var key = $"{type}:{propName}";
        registry.Register(key, x => format((TProperty)x));
    }
}