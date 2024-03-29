﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AVS.CoreLib.Mapper.Extensions
{
    public static class MapperUpdateWithContextExtensions
    {
        /// <summary>
        /// Register type mapping delegate to update existing <see cref="TDestination"/> object
        /// <seealso cref="Update{TDestination, TSource,TContext}(TDestination, TSource, TContext)"/>
        /// </summary>
        public static void RegisterUpdate<TDestination, TSource, TContext>(this IMapper mapper, Action<TDestination, TSource, TContext> func)
        {
            var mappingKey = $"({typeof(TDestination).Name},{typeof(TSource).Name},{typeof(TContext).Name})";
            mapper.RegisterDelegate(mappingKey, func);
        }

        public static Action<TDestination, TSource, TContext> GetUpdateMapper<TDestination, TSource, TContext>(this IMapper mapper)
        {
            var mappingKey = $"({typeof(TDestination).Name},{typeof(TSource).Name},{typeof(TContext).Name})";
            var del = mapper[mappingKey];
            var func = (Action<TDestination, TSource, TContext>)del;
            return func;
        }

        /// <summary>        
        /// Execute one-to-one mapping to UPDATE EXISTING <see cref="TDestination"/> object
        /// <code>
        ///	var updatedEntity = mapper.Update(existingEntity, source, ctx, postUpdate: x=> ...);
        /// </code>
        /// </summary>        
        /// <typeparam name="TSource">source type</typeparam>
        /// <typeparam name="TDestination">destination type</typeparam>
        /// <typeparam name="TContext">additional argument to update the destination object</typeparam>
        public static TDestination Update<TDestination, TSource, TContext>(this IMapper mapper, TDestination destination, TSource source,
            TContext context, Action<TDestination>? postUpdate = null)
        {
            var mappingKey = $"({typeof(TDestination).Name},{typeof(TSource).Name},{typeof(TContext).Name})";
            var func = mapper.GetUpdateMapper<TDestination, TSource, TContext>();
            mapper.Update(destination, source, context);
            postUpdate?.Invoke(destination);
            return destination;
        }

        /// <summary>
        /// Execute many-to-many mapping to UPDATE EXISTING <see cref="TDestination"/> objects collection       
        /// <code>
        /// mapper.UpdateAll(entities, models, ctx);
        /// </code>
        /// </summary>
        /// <typeparam name="TSource">source type</typeparam>
        /// <typeparam name="TDestination">destination type</typeparam>
        public static void UpdateAll<TDestination, TSource, TContext>(this IMapper mapper, IEnumerable<TDestination> destination,
            IEnumerable<TSource> source, TContext context, Action<TDestination>? postUpdate = null)
        {
            var func = mapper.GetUpdateMapper<TDestination, TSource, TContext>();
            foreach (var zip in destination.Zip(source))
            {
                func(zip.First, zip.Second, context);
                postUpdate?.Invoke(zip.First);
            }
        }
    }
}
