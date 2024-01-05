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
        protected Dictionary<string, Delegate> Delegates { get; set; } = new();

        public string[] Keys => Delegates.Keys.ToArray();

        public void RegisterDelegate(string mappingKey, Delegate del)
        {
            Delegates.Add(mappingKey, del);
        }

        public Delegate this[string mappingKey]
        {
            get 
            {
                if (!Delegates.ContainsKey(mappingKey))
                    throw new MappingNotFoundException(mappingKey);

                return Delegates[mappingKey]; 
            }
        }

        #region Map
        /// <summary>
        /// Register type mapping delegate to PRODUCE NEW <see cref="TDestination"/>
        /// <seealso cref="Map{TSource, TDestination}(TSource)"/>
        /// </summary>
        /// <typeparam name="TSource">source type</typeparam>
        /// <typeparam name="TDestination">destination type</typeparam>
        /// <param name="func">delegate to do the mapping</param>
        public void Register<TSource, TDestination>(Func<TSource, TDestination> func)
        {
            var mappingKey = $"{typeof(TSource).Name}->{typeof(TDestination).Name}";
            RegisterDelegate(mappingKey, func);
        }

        /// <summary>        
        /// Execute one-to-one mapping to PRODUCE NEW <see cref="TDestination"/> object
        /// <code>
        ///	var model = mapper.Map&lt;Source,Model&gt;(source)
        /// </code>
        /// </summary>        
        /// <typeparam name="TSource">source type</typeparam>
        /// <typeparam name="TDestination">model (destination) type</typeparam>
        public TDestination Map<TSource, TDestination>(TSource source)
        {
            var mappingKey = $"{typeof(TSource).Name}->{typeof(TDestination).Name}";
            var del = this[mappingKey];
            var func = (Func<TSource, TDestination>)del;
            return func(source);
        } 
        #endregion

        #region Update
        /// <summary>
        /// Register type mapping delegate to update existing <see cref="TDestination"/> object
        /// <seealso cref="Update{TDestination, TSource}(TDestination, TSource)"/>
        /// </summary>
        public void RegisterUpdate<TDestination, TSource>(Action<TDestination, TSource> func)
        {
            var mappingKey = $"({typeof(TDestination).Name},{typeof(TSource).Name})";
            Delegates.Add(mappingKey, func);
        }

        /// <summary>        
        /// Execute one-to-one mapping to UPDATE EXISTING <see cref="TDestination"/> object
        /// <code>
        ///	var updatedEntity = mapper.Update(existingEntity, source);
        /// </code>
        /// </summary>        
        /// <typeparam name="TSource">source type</typeparam>
        /// <typeparam name="TDestination">destination type</typeparam>
        public TDestination Update<TDestination, TSource>(TDestination destination, TSource source)
        {
            var mappingKey = $"({typeof(TDestination).Name},{typeof(TSource).Name})";
            var del = this[mappingKey];
            var func = (Action<TDestination, TSource>)del;
            func(destination,source);
            return destination;
        }
        
        #endregion
    }
}
