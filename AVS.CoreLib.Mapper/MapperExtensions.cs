using System;
using System.Collections.Generic;
using System.Linq;

namespace AVS.CoreLib.Mapper
{
    public static class MapperExtensions
    {
        public static IEnumerable<TModel> MapAll<TSource, TModel>(this IMapper mapper, IEnumerable<TSource> source, Action<TModel>? action = null)
        {
            var func = mapper.GetMapper<TSource, TModel>();

            if(action == null)
                return source.Select(x => func(x));

            return source.Select(x =>
            {
                var model = func(x);
                action.Invoke(model);
                return model;
            });
        }

        public static IEnumerable<TModel> MapAll<TSource, TContext, TModel>(this IMapper mapper, IEnumerable<TSource> source, TContext context, Action<TModel>? action = null)
        {
            var func = mapper.GetMapper<TSource, TContext, TModel>();

            if (action == null)
                return source.Select(x => func(x, context));

            return source.Select(x =>
            {
                var model = func(x, context);
                action.Invoke(model);
                return model;
            });
        }

        public static void Register<TSource>(this IMapper mapper, Func<TSource, TSource> func)
        {
            mapper.Register(func);
        }



        /// <summary>
        /// Create a copy of the source object
        /// </summary>
        /// <param name="mapper">mapper instance</param>
        /// <param name="source">source object</param>
        /// <param name="modify">optional parameter in case you need to modify the copy</param>
        /// <returns>copy of the source object</returns>
        public static TSource Copy<TSource>(this IMapper mapper, TSource source, Action<TSource>? modify = null)
        {
            var copy = mapper.Map<TSource, TSource>(source);
            modify?.Invoke(copy);
            return copy;
        }

        public static TModel Map<TSource, TModel>(this IMapper mapper, TSource source, Action<TModel> modify)
        {
            var model = mapper.Map<TSource, TModel>(source);
            modify.Invoke(model);
            return model;
        }

        public static TModel Map<TSource, TContext, TModel>(this IMapper mapper, TSource source, TContext context, Action<TModel> modify)
        {
            var model = mapper.Map<TSource, TContext, TModel>(source, context);
            modify.Invoke(model);
            return model;
        }

        public static void Fill<TSource, TModel>(this IMapper mapper, TSource source, TModel model, Action<TModel> modify)
        {
            mapper.Fill(source, model);
            modify.Invoke(model);            
        }

        public static void Fill<TSource, TModel, TContext>(this IMapper mapper, TSource source, TModel model, TContext context, Action<TModel> modify)
        {
            mapper.Fill(source, model, context);
            modify.Invoke(model);
        }
    }
}
