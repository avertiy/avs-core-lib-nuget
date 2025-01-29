using System;
using System.Collections.Generic;
using System.Linq;

namespace AVS.CoreLib.Mapper.Extensions;

public static class MapperMapAllExtensions
{
    /// <summary>
    /// Execute many-to-many mapping to PRODUCE NEW <see cref="TDestination"/> objects        
    /// <code>
    /// var models = mapper.MapAll&lt;Source,Model&gt;(items, nameof(MapperProfile.MapSourceToModel));
    /// </code>
    /// </summary>
    /// <typeparam name="TSource">source type</typeparam>
    /// <typeparam name="TDestination">destination type</typeparam>
    public static IEnumerable<TDestination> MapAll<TSource, TDestination>(this IMapper mapper, IEnumerable<TSource> source, string delegateRef)
    {
        var mappingKey = $"{typeof(TSource).Name}->{typeof(TDestination).Name}";
        var del = mapper[mappingKey];
        var func = (Func<TSource, TDestination>)del;

        try
        {
            return source.Select(x => func(x));
        }
        catch (Exception ex)
        {
            throw new MapException($"MapAll<{typeof(TSource).Name},{typeof(TDestination).Name}> {mappingKey} failed", ex, delegateRef);
        }
    }

    /// <summary>
    /// Execute many-to-many mapping to PRODUCE NEW <see cref="TDestination"/> objects        
    /// <code>
    /// var models = mapper.MapAll&lt;Source,Model&gt;(items, nameof(MapperProfile.MapSourceToModel));
    /// </code>
    /// </summary>
    /// <typeparam name="TSource">source type</typeparam>
    /// <typeparam name="TDestination">destination type</typeparam>
    public static TDestination[] MapAll<TSource, TDestination>(this IMapper mapper, IList<TSource> source, string delegateRef)
    {
        if (source.Count == 0)
            return Array.Empty<TDestination>();

        var mappingKey = $"{typeof(TSource).Name}->{typeof(TDestination).Name}";
        var del = mapper[mappingKey];
        var func = (Func<TSource, TDestination>)del;

        try
        {
            return source.Select(x => func(x)).ToArray();
        }
        catch (Exception ex)
        {
            throw new MapException($"MapAll<{typeof(TSource).Name},{typeof(TDestination).Name}> {mappingKey} failed", ex, delegateRef);
        }
    }

    /// <summary>
    /// Execute many-to-many mapping to PRODUCE NEW <see cref="TDestination"/> objects        
    /// <code>
    /// var models = mapper.MapAll&lt;Source,Model&gt;(items, action, nameof(MapperProfile.MapSourceToModel));
    /// </code>
    /// </summary>
    /// <typeparam name="TSource">source type</typeparam>
    /// <typeparam name="TDestination">destination type</typeparam>
    public static IEnumerable<TDestination> MapAll<TSource, TDestination>(this IMapper mapper, IEnumerable<TSource> source, Action<TDestination> action, string delegateRef)
    {
        var mappingKey = $"{typeof(TSource).Name}->{typeof(TDestination).Name}";
        var del = mapper[mappingKey];
        var func = (Func<TSource, TDestination>)del;

        try
        {
            return source.Select(x =>
            {
                var model = func(x);
                action.Invoke(model);
                return model;
            });
        }
        catch (Exception ex)
        {
            throw new MapException($"MapAll<{typeof(TSource).Name},{typeof(TDestination).Name}> {mappingKey} failed", ex, delegateRef);
        }
    }

    #region MapAll with args

    public static IEnumerable<TDestination> MapAllWithArgs<TSource, TDestination>(this IMapper mapper, IEnumerable<TSource> source, string delegateRef, params object[] args)
    {
        var mappingKey = args.Length > 1
            ? $"({typeof(TSource).Name}, args)->{typeof(TDestination).Name}"
            : $"({typeof(TSource).Name}, arg)->{typeof(TDestination).Name}";

        var del = mapper[mappingKey];

        try
        {
            if (args.Length == 1)
            {
                var func = (Func<TSource, object, TDestination>)del;
                return source.Select(x => func(x, args[0]));
            }
            else
            {
                var func = (Func<TSource, object[], TDestination>)del;
                return source.Select(x => func(x, args));
            }
        }
        catch (Exception ex)
        {
            throw new MapException($"MapAll<{typeof(TSource).Name},{typeof(TDestination).Name}> {mappingKey} failed", ex, delegateRef);
        }
    } 
    #endregion
}