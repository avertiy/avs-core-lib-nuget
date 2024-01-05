using System;

namespace AVS.CoreLib.Mapper
{
    /// <summary>
    /// simple and easy to use mapper     
    /// <code>
    /// //register CREATE NEW mappings:    
    /// mapper.Register&lt;Model,Source&gt;(x =&gt; new Model { ... });
    /// //map one
    ///	var model = mapper.Map&lt;Model,Source&gt;(Source source)
    /// //map many
    /// var items = mapper.MapAll&lt;Model,Source&gt;(IEnumerable&lt;Source&gt; source)
    /// 
    /// //register UPDATE EXISTING mappings:
    /// mapper.RegisterUpdate&lt;Entity,Source&gt;((x, src) =&gt; { x.Prop = src.Prop; ...});
    /// //update one
    ///	var updatedEntity = mapper.Update&lt;Entity,Source&gt;(existingEntity, src)    
    /// </code>
    /// </summary>
    public interface IMapper //: ICreateMapper, IUpdateMapper
    {
        string[] Keys { get; }
        void RegisterDelegate(string mappingKey, Delegate del);
        Delegate this[string mappingKey] { get; }

        /// <summary>
        /// Execute one-to-one mapping to PRODUCE NEW <see cref="TDestination"/> object
        /// <seealso cref="Map{TSource, TDestination}(TSource)"/>
        /// </summary>
        void Register<TSource, TDestination>(Func<TSource, TDestination> func);

        /// <summary>        
        /// Execute one-to-one mapping to PRODUCE NEW <see cref="TDestination"/> object
        /// </summary>
        TDestination Map<TSource, TDestination>(TSource source);

        /// <summary>
        /// Register type mapping delegate to update existing <see cref="TDestination"/> object
        /// <seealso cref="Update{TDestination, TSource}(TDestination, TSource)"/>
        /// </summary>
        void RegisterUpdate<TDestination, TSource>(Action<TDestination, TSource> action);

        /// <summary>
        /// Execute one-to-one mapping to UPDATE EXISTING <see cref="TTarget"/> object        
        /// </summary>
        TTarget Update<TTarget, TSource>(TTarget target, TSource source);
    }
}