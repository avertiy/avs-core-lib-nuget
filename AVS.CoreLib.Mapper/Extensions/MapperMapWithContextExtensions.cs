using System;
using System.Collections.Generic;
using System.Linq;

namespace AVS.CoreLib.Mapper.Extensions
{
    public static class MapperMapWithContextExtensions
    {
        /// <summary>
        /// Register type mapping delegate to PRODUCE NEW <see cref="TDestination"/>
        /// <seealso cref="Map{TSource, TDestination, TContext}"/>
        /// </summary>
        /// <typeparam name="TSource">source type</typeparam>
        /// <typeparam name="TDestination">destination type</typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="mapper">mapper</param>
        /// <param name="delegate">delegate to do the mapping</param>
        public static void Register<TSource, TContext, TDestination>(this IMapper mapper, Func<TSource, TContext, TDestination> @delegate)
        {
            var mappingKey = $"({typeof(TSource).Name},{typeof(TContext).Name})->{typeof(TDestination).Name}";
            mapper.RegisterDelegate(mappingKey, @delegate);

            //var wrapper = new Func<TSource, TContext, TDestination>((x, arg) =>
            //{
            //    try
            //    {
            //        return @delegate(x, arg);
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new MapException($"Map with {typeof(TContext).Name} arg {mappingKey} failed", ex);
            //    }
            //});

            //mapper.RegisterDelegate(mappingKey, wrapper);
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
        public static TDestination Map<TSource, TContext, TDestination>(this IMapper mapper, TSource source, TContext context, Action<TDestination>? modify = null, string? delegateRef = null)
        {
            var mappingKey = $"({typeof(TSource).Name},{typeof(TContext).Name})->{typeof(TDestination).Name}";
            var del = mapper[mappingKey];
            var func = (Func<TSource, TContext, TDestination>)del;

            try
            {
                var result = func(source, context);
                modify?.Invoke(result);
                return result;
            }
            catch (Exception ex)
            {
                throw new MapException($"Map {mappingKey} Failed", ex, delegateRef);
            }
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
            TContext context, Action<TDestination>? action = null, string? delegateRef = null)
        {
            var mappingKey = $"({typeof(TSource).Name},{typeof(TContext).Name})->{typeof(TDestination).Name}";
            var del = mapper[mappingKey];
            var func = (Func<TSource, TContext, TDestination>)del;

            try
            {
                if (action == null)
                    return source.Select(x => func(x, context));

                return source.Select(x =>
                {
                    var model = func(x, context);
                    action.Invoke(model);
                    return model;
                });
            }
            catch (Exception ex)
            {
                throw new MapException($"MapAll {mappingKey} Failed", ex, delegateRef);
            }
        }

    }
}
