using System;
using System.Collections.Generic;
using System.Linq;

namespace AVS.CoreLib.Mapper
{
    /// <summary>
    /// simple and easy to use mapper     
    /// <code>
    /// //register CREATE mappers:    
    /// mapper.Register&lt;Source,Model&gt;(MapSourceToModel);
    /// //where MapSourceToModel(Source x) => x => new Model { ... };
    /// 
    /// //map one
    ///	var model = mapper.Map&lt;Source,Model&gt;(source, delegateRef: nameof(MapperProfile.MapSourceToModel));
    /// //map many
    /// var items = mapper.MapAll&lt;Source,Model&gt;(IEnumerable&lt;Source&gt; source, delegateRef: nameof(MapperProfile.MapSourceToModel))
    /// 
    /// //register UPDATE mappers:
    /// mapper.RegisterUpdate&lt;Entity,Model&gt;(UpdateEntityFromModel);
    /// where UpdateEntityFromModel(Entity x, Model src) => { x.Prop = src.Prop; ...});
    /// //update one
    ///	var updatedEntity = mapper.Update&lt;Entity,Model&gt;(existingEntity, model, delegateRef: nameof(MapperProfile.UpdateEntityFromModel))    
    /// </code>
    /// </summary>
    public class Mapper : IMapper
    {
        protected Dictionary<string, Delegate> Delegates { get; set; } = new();

        public string[] Keys => Delegates.Keys.ToArray();

        public void RegisterDelegate(string mappingKey, Delegate @delegate)
        {
            Delegates.Add(mappingKey, @delegate);
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
        /// Register type mapping delegate to PRODUCE NEW <see cref="TResult"/>
        /// <seealso cref="Map{TSource, TDestination}(TSource, string)"/>
        /// </summary>
        /// <typeparam name="TSource">source type</typeparam>
        /// <typeparam name="TResult">result type</typeparam>
        /// <param name="delegate">delegate to do the mapping</param>
        public void Register<TSource, TResult>(Func<TSource, TResult> @delegate)
        {
            var mappingKey = $"{typeof(TSource).Name}->{typeof(TResult).Name}";

            //var wrapper = new Func<TSource, TResult>(x =>
            //{
            //    try
            //    {
            //        return @delegate(x);
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new MapException($"Map {mappingKey} failed", ex);
            //    }
            //});

            //RegisterDelegate(mappingKey, wrapper);
            RegisterDelegate(mappingKey, @delegate);
        }
        
        /// <summary>        
        /// Execute one-to-one mapping to PRODUCE NEW <see cref="TDestination"/> object
        /// <code>
        ///	var model = mapper.Map&lt;Source,Model&gt;(source, delegateRef:nameof(MapperProfile.MapSourceToModel))
        /// </code>
        /// </summary>        
        /// <typeparam name="TSource">source type</typeparam>
        /// <typeparam name="TDestination">model (destination) type</typeparam>
        /// <param name="source">mapping source</param>
        /// <param name="delegateRef">helps to track mapping delegate(s) usages</param>
        public TDestination Map<TSource, TDestination>(TSource source, string? delegateRef = null)
        {
            var mappingKey = $"{typeof(TSource).Name}->{typeof(TDestination).Name}";
            var del = this[mappingKey];
            var func = (Func<TSource, TDestination>)del;
            try
            {
                return func(source);
            }
            catch (Exception ex)
            {
                throw new MapException($"Map {typeof(TSource).Name}->{typeof(TDestination).Name} Failed", ex, delegateRef);
            }
        }
        #endregion

        #region Update
        /// <summary>
        /// Register type mapping delegate to update existing <see cref="TTarget"/> object
        /// <seealso cref="Update{TTarget, TSource}(TTarget, TSource, string)"/>
        /// <code>
        ///  mapper.RegisterUpdate&lt;Entity,Model&gt;(entity, model, key: UpdateEntityFromModel);
        /// </code>
        /// </summary>
        public void RegisterUpdate<TTarget, TSource>(Action<TTarget, TSource> @delegate)
        {
            var mappingKey = $"({typeof(TTarget).Name},{typeof(TSource).Name})";

            //var wrapper = new Action<TTarget, TSource>((x,y) =>
            //{
            //    try
            //    {
            //        @delegate(x,y);
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new MapException($"Update {mappingKey} failed", ex);
            //    }
            //});

            RegisterDelegate(mappingKey, @delegate);
        }

        /// <summary>        
        /// Execute one-to-one mapping to UPDATE EXISTING <see cref="TTarget"/> object
        /// <code>
        ///	var updatedEntity = mapper.Update&lt;Entity,Model&gt;(existingEntity, model, delegateRef: nameof(MapperProfile.UpdateEntityFromModel));
        /// </code>
        /// </summary>        
        /// <typeparam name="TSource">source type</typeparam>
        /// <typeparam name="TTarget">destination type</typeparam>
        /// <param name="target">mapping target object</param>
        /// <param name="source">mapping source object</param>
        /// <param name="delegateRef">dummy parameter helps to track mapping delegate(s) usages</param>
        public TTarget Update<TTarget, TSource>(TTarget target, TSource source, string? delegateRef = null)
        {
            var mappingKey = $"({typeof(TTarget).Name},{typeof(TSource).Name})";
            var del = this[mappingKey];
            var func = (Action<TTarget, TSource>)del;

            try
            {
                func(target, source);
                return target;
            }
            catch (Exception ex)
            {
                throw new MapException($"Update{mappingKey} Failed", ex, delegateRef);
            }
        }
        
        #endregion
    }
}
