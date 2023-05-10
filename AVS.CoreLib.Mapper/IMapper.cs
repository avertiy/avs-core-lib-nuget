using System.Collections.Generic;

namespace AVS.CoreLib.Mapper
{
    /// <summary>
    /// simple & easy to use mapper 
    /// <code>
    /// //register mappings:
    /// mapper.Register&lt;Model,Source&gt;(x =&gt; new Model { ... });
    ///
    /// //map one
    ///	var model = mapper.Map&lt;Model,Source&gt;(Source source)
    ///
    /// //map many
    /// var items = mapper.MapAll&lt;Model,Source&gt;(IEnumerable&lt;Source&gt; source)
    /// </code>
    /// </summary>
    public interface IMapper
    {
        TModel Map<TSource, TModel>(TSource source);
        IEnumerable<TModel> MapAll<TSource, TModel>(IEnumerable<TSource> source);
    }
}