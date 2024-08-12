using System;
using System.Collections.Generic;
using System.Linq;

namespace AVS.CoreLib.Mapper.Extensions
{
    public static class MapperUpdateExtensions
    {
        //public static void RegisterUpdate<TSource>(this IMapper mapper, Action<TSource, TSource> update)
        //{
        //    mapper.RegisterUpdate(update);
        //}

        public static TDestination Update<TDestination, TSource>(this IMapper mapper, TDestination destination, TSource source,
            Action<TDestination> modify, string? delegateRef = null)
        {
            mapper.Update(destination, source);
            modify.Invoke(destination);
            return destination;
        }

        /// <summary>
        /// Execute many-to-many mapping to UPDATE EXISTING <see cref="TDestination"/> objects collection       
        /// <code>
        /// mapper.UpdateAll(entities, models);
        /// </code>
        /// </summary>
        /// <typeparam name="TSource">source type</typeparam>
        /// <typeparam name="TDestination">destination type</typeparam>
        public static void UpdateAll<TDestination, TSource>(this IMapper mapper, IEnumerable<TDestination> destination,
            IEnumerable<TSource> source, Action<TDestination>? postUpdate = null, string? delegateRef = null)
        {
            var mappingKey = $"({typeof(TDestination).Name},{typeof(TSource).Name})";
            var del = mapper[mappingKey];
            var func = (Action<TDestination, TSource>)del;

            foreach (var zip in destination.Zip(source))
            {
                try
                {
                    func(zip.First, zip.Second);
                    postUpdate?.Invoke(zip.First);
                }
                catch (Exception ex)
                {
                    throw new MapException($"UpdateAll {mappingKey} Failed", ex, delegateRef);
                }
            }
        }
    }
}
