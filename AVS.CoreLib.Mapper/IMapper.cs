using System;

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
    public interface IMapper
    {
        string[] Keys { get; }
        void RegisterDelegate(string mappingKey, Delegate @delegate);
        Delegate this[string mappingKey] { get; }

        /// <summary>
        /// Registers one-to-one mapping to PRODUCE NEW <see cref="TDestination"/> object <seealso cref="Map{TSource, TDestination}(TSource, string)"/>
        /// </summary>
        void Register<TSource, TDestination>(Func<TSource, TDestination> @delegate);

        /// <summary>
        /// Executes one-to-one mapping to PRODUCE NEW <see cref="TDestination"/> object
        /// </summary>
        /// <typeparam name="TSource">source type</typeparam>
        /// <typeparam name="TDestination">destination type</typeparam>
        /// <param name="source">source object</param>
        /// <param name="delegateRef">dummy parameter helps to track delegate(s) usages</param>
        TDestination Map<TSource, TDestination>(TSource source, string delegateRef);

        /// <summary>
        /// Registers type mapping delegate to update existing <see cref="TTarget"/> object
        /// <seealso cref="Update{TTarget, TSource}(TTarget, TSource, string)"/>
        /// </summary>
        void RegisterUpdate<TTarget, TSource>(Action<TTarget, TSource> @delegate);

        /// <summary>
        /// Executes one-to-one mapping to UPDATE EXISTING <see cref="TTarget"/> object
        /// </summary>
        /// <param name="target">target object</param>
        /// <param name="source">source object</param>
        /// <param name="delegateRef">dummy parameter helps to track delegate(s) usages</param>
        TTarget Update<TTarget, TSource>(TTarget target, TSource source, string? delegateRef = null);
    }
}