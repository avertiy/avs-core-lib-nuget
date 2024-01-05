using System;
using System.Collections.Generic;
using System.Linq;

namespace AVS.CoreLib.Mapper.Extensions
{
    public static class MapperMapWithArgsExtensions
    {
        /// <summary>
        /// Register one-to-one type mapping delegate to PRODUCE NEW <see cref="TDestination"/> object
        /// <seealso cref="Map{TSource, TDestination}(TSource, params object[])"/>
        /// </summary>
        /// <typeparam name="TSource">source type</typeparam>
        /// <typeparam name="TDestination">destination type</typeparam>
        /// <param name="func">delegate to do the mapping</param>
        public static void Register<TSource, TDestination>(this IMapper mapper, Func<TSource, object, TDestination> func)
        {
            var mappingKey = $"({typeof(TSource).Name}, arg)->{typeof(TDestination).Name}";
            mapper.RegisterDelegate(mappingKey, func);
        }

        public static void Register<TSource, TDestination>(this IMapper mapper, Func<TSource, object[], TDestination> func)
        {
            var mappingKey = $"({typeof(TSource).Name}, args)->{typeof(TDestination).Name}";
            mapper.RegisterDelegate(mappingKey, func);
        }

        public static TDestination Map<TSource, TDestination>(this IMapper mapper, TSource source, params object[] args)
        {
            var mappingKey = args.Length > 1
                ? $"({typeof(TSource).Name}, args)->{typeof(TDestination).Name}"
                : $"({typeof(TSource).Name}, arg)->{typeof(TDestination).Name}";

            var del = mapper[mappingKey];

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

        public static IEnumerable<TDestination> MapAll<TSource, TDestination>(this IMapper mapper, IEnumerable<TSource> source, params object[] args)
        {
            var mappingKey = args.Length > 1
                ? $"({typeof(TSource).Name}, args)->{typeof(TDestination).Name}"
                : $"({typeof(TSource).Name}, arg)->{typeof(TDestination).Name}";

            var del = mapper[mappingKey];
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
    }
}
