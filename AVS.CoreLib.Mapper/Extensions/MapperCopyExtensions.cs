using System;
using System.Collections.Generic;

namespace AVS.CoreLib.Mapper.Extensions
{
    public static class MapperCopyExtensions
    {
        /// <summary>
        /// Register one-to-one type mapping delegate to PRODUCE COPY of the  <see cref="TSource"/> object
        /// <seealso cref="Copy{TSource}"/>
        /// </summary>
        /// <typeparam name="TSource">source type</typeparam>
        /// <param name="func">delegate to do the mapping</param>
        public static void Register<TSource>(this IMapper mapper, Func<TSource, TSource> func)
        {
            var mappingKey = $"{typeof(TSource).Name}->{typeof(TSource).Name}";
            mapper.RegisterDelegate(mappingKey, func);
        }

        /// <summary>
        /// Create a copy of the source object
        /// </summary>
        /// <param name="mapper">mapper instance</param>
        /// <param name="source">source object</param>
        /// <param name="modify">optional parameter in case you need to modify the copy</param>
        /// <param name="delegateRef">dummy parameter to track delegate(s) usages</param>
        /// <returns>copy of the source object</returns>
        public static TSource Copy<TSource>(this IMapper mapper, TSource source, Action<TSource>? modify = null, string? delegateRef = null)
        {
            var copy = mapper.Map<TSource, TSource>(source, delegateRef);
            modify?.Invoke(copy);
            return copy;
        }

        /// <summary>
        /// Execute many-to-many mapping to PRODUCE COPIES of <see cref="TSource"/> objects        
        /// <code>
        /// var copies = mapper.CopyAll(items, modify: x => ... );
        /// </code>
        /// </summary>
        public static IEnumerable<TSource> CopyAll<TSource>(this IMapper mapper, IEnumerable<TSource> source, Action<TSource>? modify = null, string? delegateRef = null)
        {
            return mapper.MapAll(source, delegateRef, modify);
        }

    }
}
