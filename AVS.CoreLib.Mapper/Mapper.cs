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
        private readonly Dictionary<string, Delegate> _mappings = new();

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
}
