using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AVS.CoreLib.Mapper.Extensions
{
    public static class MapperMapWithContextExtensions
    {
        /// <summary>
        /// Register type mapping delegate to PRODUCE NEW <see cref="TDestination"/>
        /// <seealso cref="Map{TSource, TDestination}(TSource)"/>
        /// </summary>
        /// <typeparam name="TSource">source type</typeparam>
        /// <typeparam name="TDestination">destination type</typeparam>
        /// <param name="func">delegate to do the mapping</param>
        public static void Register<TSource, TContext, TDestination>(this IMapper mapper, Func<TSource, TContext, TDestination> func)
        {
            var mappingKey = $"({typeof(TSource).Name},{typeof(TContext).Name})->{typeof(TDestination).Name}";
            mapper.RegisterDelegate(mappingKey, func);
        }

        /// <summary>
        /// returns a type mapping delegate that PRODUCE NEW <see cref="TDestination"/> object
        /// </summary>
        public static Func<TSource, TContext, TDestination> GetMapper<TSource, TContext, TDestination>(this IMapper mapper)
        {
            var mappingKey = $"({typeof(TSource).Name},{typeof(TContext).Name})->{typeof(TDestination).Name}";
            var del = mapper[mappingKey];
            var func = (Func<TSource, TContext, TDestination>)del;
            return func;
        }

        /// <summary>        
        /// Execute one-to-one mapping to PRODUCE NEW <see cref="TDestination"/> object
        /// <code>
        ///	var model = mapper.Map&lt;Model,Source,Context&gt;(source, ctx);
        /// </code>
        /// </summary>        
        /// <typeparam name="TSource">source type</typeparam>
        /// <typeparam name="TDestination">destination type</typeparam>
        /// <typeparam name="TContext">additional argument to create the destination object</typeparam>
        public static TDestination Map<TSource, TContext, TDestination>(this IMapper mapper, TSource source, TContext context, Action<TDestination>? modify = null)
        {
            var func = mapper.GetMapper<TSource, TContext, TDestination>();
            var result = func(source, context);
            modify?.Invoke(result);
            return result;
        }

        /// <summary>
        /// Execute many-to-many mapping to PRODUCE NEW <see cref="TDestination"/> objects        
        /// <code>
        /// var entities = mapper.MapAll&lt;Source,Context,Enity&gt;(items, ctx, x=> ...(post action) );
        /// </code>
        /// </summary>
        /// <typeparam name="TSource">source type</typeparam>
        /// <typeparam name="TDestination">destination type</typeparam>
        /// <typeparam name="TContext">additional argument to create the destination object</typeparam>
        public static IEnumerable<TDestination> MapAll<TSource, TContext, TDestination>(this IMapper mapper, IEnumerable<TSource> source,
            TContext context, Action<TDestination>? action = null)
        {
            var func = mapper.GetMapper<TSource, TContext, TDestination>();

            if (action == null)
                return source.Select(x => func(x, context));

            return source.Select(x =>
            {
                var model = func(x, context);
                action.Invoke(model);
                return model;
            });
        }

    }
}
