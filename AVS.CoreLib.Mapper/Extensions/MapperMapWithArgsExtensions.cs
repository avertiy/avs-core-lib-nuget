using System;
using System.Collections.Generic;
using System.Linq;

namespace AVS.CoreLib.Mapper.Extensions
{
    public static class MapperMapWithArgsExtensions
    {
        /// <summary>
        /// Register one-to-one type mapping delegate to PRODUCE NEW <see cref="TDestination"/> object
        /// </summary>
        /// <typeparam name="TSource">source type</typeparam>
        /// <typeparam name="TDestination">destination type</typeparam>
        /// <param name="mapper">mapper</param>
        /// <param name="delegate">delegate to do the mapping</param>
        public static void Register<TSource, TDestination>(this IMapper mapper, Func<TSource, object, TDestination> @delegate)
        {
            var mappingKey = $"({typeof(TSource).Name}, arg)->{typeof(TDestination).Name}";
            mapper.RegisterDelegate(mappingKey, @delegate);
        }

        public static void Register<TSource, TDestination>(this IMapper mapper, Func<TSource, object[], TDestination> @delegate)
        {
            var mappingKey = $"({typeof(TSource).Name}, args)->{typeof(TDestination).Name}";
            mapper.RegisterDelegate(mappingKey, @delegate);
        }

        public static TDestination Map<TSource, TDestination>(this IMapper mapper, TSource source, string delegateRef, object[] args)
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
                    return func(source, args[0]);
                }
                else
                {
                    var func = (Func<TSource, object[], TDestination>)del;
                    return func(source, args);
                }
            }
            catch (Exception ex)
            {
                throw new MapException($"Map<{typeof(TSource).Name},{typeof(TDestination).Name}> {mappingKey} failed", ex, delegateRef);
            }
        }
    }
}
