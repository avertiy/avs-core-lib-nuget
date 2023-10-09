using System;
using System.Collections.Generic;
using System.Linq;

namespace AVS.CoreLib.Mapper
{
    /// <summary>
    /// simple & easy to use mapper  
    /// </summary> 
    /// <code>
    /// // configure mappings:
    /// mapper.Register&lt;Model,Source&gt;(x =&gt; new Model { ... });
    ///
    /// // do mapping:
    ///	var model = mapper.Map&lt;Model,Source&gt;(Source source)
    /// var items = mapper.MapAll&lt;Model,Source&gt;(IEnumerable&lt;Source&gt; source)
    /// </code>
    public class Mapper : IMapper
    {
        protected Dictionary<string, Delegate> Mappings { get; set; } = new();

        /// <summary>
        /// Register type mapping  delegate
        /// <code>
        /// usage:
        /// mapper.Register&lt;Model,Source&gt;(x =&gt; new Model { ... });
        /// </code>
        /// </summary>
        /// <typeparam name="TSource">source type</typeparam>
        /// <typeparam name="TModel">model (destination) type</typeparam>
        /// <param name="func">delegate to do the mapping</param>
        public void Register<TSource, TModel>(Func<TSource, TModel> func)
        {
            var mappingKey = $"{typeof(TSource).Name}->{typeof(TModel).Name}";
            Mappings.Add(mappingKey, func);
        }

        public void Register<TSource, TContext, TModel>(Func<TSource, TContext, TModel> func)
        {
            var mappingKey = $"({typeof(TSource).Name},{typeof(TContext).Name})->{typeof(TModel).Name}";
            Mappings.Add(mappingKey, func);
        }

        public void Register<TSource, TModel>(Action<TSource, TModel> func)
        {
            var mappingKey = $"({typeof(TSource).Name},{typeof(TModel).Name})";
            Mappings.Add(mappingKey, func);
        }

        public void Register<TSource, TModel, TContext>(Action<TSource, TModel, TContext> func)
        {
            var mappingKey = $"({typeof(TSource).Name},{typeof(TModel).Name},{typeof(TContext).Name})";
            Mappings.Add(mappingKey, func);
        }

        /// <summary>
        /// Map one-to-one <see cref="TSource"/> to <see cref="TModel"/>
        /// <code>
        ///	var model = mapper.Map&lt;Model,Source&gt;(Source source)
        /// </code>
        /// </summary>
        /// <typeparam name="TSource">source type</typeparam>
        /// <typeparam name="TModel">model (destination) type</typeparam>
        public TModel Map<TSource, TModel>(TSource source)
        {
            var mappingKey = $"{typeof(TSource).Name}->{typeof(TModel).Name}";

            if (!Mappings.ContainsKey(mappingKey))
                throw new MappingNotFoundException(mappingKey);

            var del = Mappings[mappingKey];
            var func = (Func<TSource, TModel>)del;
            return func(source);
        }        

        public TModel Map<TSource, TContext, TModel>(TSource source, TContext context)
        {
            var mappingKey = $"({typeof(TSource).Name},{typeof(TContext).Name})->{typeof(TModel).Name}";

            if (!Mappings.ContainsKey(mappingKey))
                throw new MappingNotFoundException(mappingKey);

            var del = Mappings[mappingKey];
            var func = (Func<TSource, TContext, TModel>)del;
            return func(source, context);
        }

        /// <summary>
        /// Map many <see cref="TSource"/> to many <see cref="TModel"/>
        /// <code>
        /// mapper.Register&lt;Model,Source&gt;(x =&gt; new Model { ... });
        /// </code>
        /// </summary>
        /// <typeparam name="TSource">source type</typeparam>
        /// <typeparam name="TModel">model (destination) type</typeparam>
        public IEnumerable<TModel> MapAll<TSource, TModel>(IEnumerable<TSource> source)
        {
            var mappingKey = $"{typeof(TSource).Name}->{typeof(TModel).Name}";

            if (!Mappings.ContainsKey(mappingKey))
                throw new MappingNotFoundException(mappingKey);

            var del = Mappings[mappingKey];
            var func = (Func<TSource, TModel>)del;
            return source.Select(x => func(x));
        }

        public Func<TSource, TModel> GetMapper<TSource, TModel>()
        {
            var mappingKey = $"{typeof(TSource).Name}->{typeof(TModel).Name}";

            if (!Mappings.ContainsKey(mappingKey))
                throw new MappingNotFoundException(mappingKey);

            var del = Mappings[mappingKey];
            var func = (Func<TSource, TModel>)del;
            return func;
        }

        public Func<TSource, TContext, TModel> GetMapper<TSource, TContext, TModel>()
        {
            var mappingKey = $"({typeof(TSource).Name},{typeof(TContext).Name})->{typeof(TModel).Name}";

            if (!Mappings.ContainsKey(mappingKey))
                throw new MappingNotFoundException(mappingKey);

            var del = Mappings[mappingKey];
            var func = (Func<TSource, TContext, TModel >)del;
            return func;
        }

        public void Fill<TSource, TModel>(TSource source, TModel model)
        {
            var mappingKey = $"({typeof(TSource).Name},{typeof(TModel).Name})";

            if (!Mappings.ContainsKey(mappingKey))
                throw new MappingNotFoundException(mappingKey);

            var del = Mappings[mappingKey];
            var func = (Action<TSource, TModel>)del;
            func(source, model);
        }

        public void Fill<TSource, TModel, TContext>(TSource source, TModel model, TContext context)
        {
            var mappingKey = $"({typeof(TSource).Name},{typeof(TModel).Name},{typeof(TContext).Name})";

            if (!Mappings.ContainsKey(mappingKey))
                throw new MappingNotFoundException(mappingKey);

            var del = Mappings[mappingKey];
            var func = (Action<TSource, TModel, TContext>)del;
            func(source, model, context);
        }

        public Action<TSource, TModel> GetFiller<TSource, TModel>()
        {
            var mappingKey = $"({typeof(TSource).Name},{typeof(TModel).Name})";

            if (!Mappings.ContainsKey(mappingKey))
                throw new MappingNotFoundException(mappingKey);

            var del = Mappings[mappingKey];
            var func = (Action<TSource, TModel>)del;
            return func;
        }

        public Action<TSource, TModel, TContext> GetFiller<TSource, TModel, TContext>()
        {
            var mappingKey = $"({typeof(TSource).Name},{typeof(TModel).Name},{typeof(TContext).Name})";

            if (!Mappings.ContainsKey(mappingKey))
                throw new MappingNotFoundException(mappingKey);

            var del = Mappings[mappingKey];
            var func = (Action<TSource, TModel, TContext>)del;
            return func;
        }
    }
}
