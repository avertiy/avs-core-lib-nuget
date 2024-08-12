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

            //var wrapper = new Func<TSource,object, TDestination>((x, arg) =>
            //{
            //    try
            //    {
            //        return @delegate(x, arg);
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new MapException($"Map with arg {mappingKey} failed", ex);
            //    }
            //});

            //mapper.RegisterDelegate(mappingKey, wrapper);
            mapper.RegisterDelegate(mappingKey, @delegate);
        }

        public static void Register<TSource, TDestination>(this IMapper mapper, Func<TSource, object[], TDestination> @delegate)
        {
            var mappingKey = $"({typeof(TSource).Name}, args)->{typeof(TDestination).Name}";
            mapper.RegisterDelegate(mappingKey, @delegate);
            //var wrapper = new Func<TSource, object[], TDestination>((x, args) =>
            //{
            //    try
            //    {
            //        return @delegate(x, args);
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new MapException($"Map with args[] {mappingKey} failed", ex);
            //    }
            //});

            //mapper.RegisterDelegate(mappingKey, wrapper);
        }

        public static TDestination Map<TSource, TDestination>(this IMapper mapper, TSource source, string? delegateRef, params object[] args)
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
                throw new MapException($"Map {mappingKey} failed", ex, delegateRef);
            }
        }

        public static IEnumerable<TDestination> MapAll<TSource, TDestination>(this IMapper mapper, IEnumerable<TSource> source, string? delegateRef)
        {
            var mappingKey = $"{typeof(TSource).Name}->{typeof(TDestination).Name}";
            var del = mapper[mappingKey];
            var func = (Func<TSource, TDestination>)del;
            try
            {
                return source.Select(x => func(x));
            }
            catch (Exception ex)
            {
                throw new MapException($"MapAll {mappingKey} failed", ex, delegateRef);
            }
        }

        public static IEnumerable<TDestination> MapAll<TSource, TDestination>(this IMapper mapper, IEnumerable<TSource> source, string? delegateRef, params object[] args)
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
                    return source.Select(x => func(x, args[0]));
                }
                else
                {
                    var func = (Func<TSource, object[], TDestination>)del;
                    return source.Select(x => func(x, args));
                }
            }
            catch (Exception ex)
            {
                throw new MapException($"MapAll {mappingKey} failed", ex, delegateRef);
            }
        }
    }
}
