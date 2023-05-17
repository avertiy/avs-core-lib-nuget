using System;
using System.Collections.Generic;
using System.Linq;

namespace AVS.CoreLib.Mapper
{
    /// <summary>
    /// simple & easy to use mapper  
    /// </summary> 
    /// <code>
    /// // configure mappings:
    /// mapper.Register&lt;Model,Source&gt;(x =&gt; new Model { ... });
    ///
    /// // do mapping:
    ///	var model = mapper.Map&lt;Model,Source&gt;(Source source)
    /// var items = mapper.MapAll&lt;Model,Source&gt;(IEnumerable&lt;Source&gt; source)
    /// </code>
    public class Mapper : IMapper
    {
        protected readonly Dictionary<string, Delegate> _mappings = new();

        /// <summary>
        /// Register type mapping  delegate
        /// <code>
        /// usage:
        /// mapper.Register&lt;Model,Source&gt;(x =&gt; new Model { ... });
        /// </code>
        /// </summary>
        /// <typeparam name="TSource">source type</typeparam>
        /// <typeparam name="TModel">model (destination) type</typeparam>
        /// <param name="func">delegate to do the mapping</param>
        public void Register<TSource, TModel>(Func<TSource, TModel> func)
        {
            var mappingKey = $"{typeof(TSource).Name}->{typeof(TModel).Name}";
            _mappings.Add(mappingKey, func);
        }

        /// <summary>
        /// Map one-to-one <see cref="TSource"/> to <see cref="TModel"/>
        /// <code>
        ///	var model = mapper.Map&lt;Model,Source&gt;(Source source)
        /// </code>
        /// </summary>
        /// <typeparam name="TSource">source type</typeparam>
        /// <typeparam name="TModel">model (destination) type</typeparam>
        public TModel Map<TSource, TModel>(TSource source)
        {
            var mappingKey = $"{typeof(TSource).Name}->{typeof(TModel).Name}";
            var del = _mappings[mappingKey];
            var func = (Func<TSource, TModel>)del;
            return func(source);
        }

        /// <summary>
        /// Map many <see cref="TSource"/> to many <see cref="TModel"/>
        /// <code>
        /// mapper.Register&lt;Model,Source&gt;(x =&gt; new Model { ... });
        /// </code>
        /// </summary>
        /// <typeparam name="TSource">source type</typeparam>
        /// <typeparam name="TModel">model (destination) type</typeparam>
        public IEnumerable<TModel> MapAll<TSource, TModel>(IEnumerable<TSource> source)
        {
            var mappingKey = $"{typeof(TSource).Name}->{typeof(TModel).Name}";
            var del = _mappings[mappingKey];
            var func = (Func<TSource, TModel>)del;
            return source.Select(x => func(x));
        }

        
    }

    public static class MapperExtensions
    {
        public static void Register<TSource>(this IMapper mapper, Func<TSource, TSource> func)
        {
            mapper.Register(func);
        }

        /// <summary>
        /// Create a copy of the source object
        /// </summary>
        /// <param name="mapper">mapper instance</param>
        /// <param name="source">source object</param>
        /// <param name="modify">optional parameter in case you need to modify the copy</param>
        /// <returns>copy of the source object</returns>
        public static TSource Copy<TSource>(this IMapper mapper, TSource source, Action<TSource>? modify = null)
        {
            var copy = mapper.Map<TSource, TSource>(source);
            modify?.Invoke(copy);
            return copy;
        }

        public static TModel Map<TSource, TModel>(this IMapper mapper, TSource source, Action<TModel>? modify = null)
        {
            var model = mapper.Map<TSource, TModel>(source);
            modify?.Invoke(model);
            return model;
        }
    }
}
