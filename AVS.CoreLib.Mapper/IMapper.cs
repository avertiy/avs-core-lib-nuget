using System;

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
        void Register<TSource, TModel>(Func<TSource, TModel> func);
        void Register<TSource,TContext, TModel>(Func<TSource, TContext, TModel> func);
        void Register<TSource, TModel>(Action<TSource, TModel> action);
        void Register<TSource, TModel, TContext>(Action<TSource, TModel, TContext> action);
        TModel Map<TSource, TModel>(TSource source);
        TModel Map<TSource, TContext, TModel>(TSource source, TContext context);
        Func<TSource, TModel> GetMapper<TSource, TModel>();
        Func<TSource, TContext, TModel> GetMapper<TSource, TContext, TModel>();
        void Fill<TSource, TModel>(TSource source, TModel model);
        void Fill<TSource, TModel,TContext>(TSource source, TModel model, TContext context);
        
    }
}