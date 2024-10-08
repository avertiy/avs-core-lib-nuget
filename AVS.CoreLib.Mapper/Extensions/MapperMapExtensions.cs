﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AVS.CoreLib.Mapper.Extensions
{
    public static class MapperMapExtensions
    {
        /*
        /// <summary>
        /// returns a type mapping delegate that PRODUCE NEW <see cref="TDestination"/> object
        /// </summary>
        public static Func<TSource, TDestination> GetMapper<TSource, TDestination>(this IMapper mapper)
        {
            var mappingKey = $"{typeof(TSource).Name}->{typeof(TDestination).Name}";
            var del = mapper[mappingKey];
            var func = (Func<TSource, TDestination>)del;
            return func;
        }
        */

        /// <summary>
        /// Execute many-to-many mapping to PRODUCE NEW <see cref="TDestination"/> objects        
        /// <code>
        /// var models = mapper.MapAll&lt;Source,Model&gt;(items, nameof(MapperProfile.MapSourceToModel));
        /// </code>
        /// </summary>
        /// <typeparam name="TSource">source type</typeparam>
        /// <typeparam name="TDestination">destination type</typeparam>
        public static IEnumerable<TDestination> MapAll<TSource, TDestination>(this IMapper mapper, IEnumerable<TSource> source, Action<TDestination>? action = null, string? delegateRef = null)
        {
            var mappingKey = $"{typeof(TSource).Name}->{typeof(TDestination).Name}";
            var del = mapper[mappingKey];
            var func = (Func<TSource, TDestination>)del;

            try
            {

                if (action == null)
                    return source.Select(x => func(x));

                return source.Select(x =>
                {
                    var model = func(x);
                    action.Invoke(model);
                    return model;
                });
            }
            catch (Exception ex)
            {
                throw new MapException($"MapAll {mappingKey} failed", ex, delegateRef);
            }
        }

        /// <summary>
        /// Execute mapping to PRODUCE NEW <see cref="TDestination"/> object
        /// </summary>
        public static TDestination Map<TSource, TDestination>(this IMapper mapper, TSource source, Action<TDestination> modify, string? delegateRef = null)
        {
            var model = mapper.Map<TSource, TDestination>(source);
            modify.Invoke(model);
            return model;
        }
    }
}
