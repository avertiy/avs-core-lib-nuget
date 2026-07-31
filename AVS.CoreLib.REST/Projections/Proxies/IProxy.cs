using System;

namespace AVS.CoreLib.REST.Projections
{
    /// <summary>
    /// Proxy{in T,out TResult} provides an interface to add T item(s) and produce the TResult     
    /// </summary>
    public interface IProxy<in T, out TResult> : IContainer<T>, IProxy<TResult>
    {
    }
    // TO-DO swap Iproxty generic args
    //public interface IProxy2<out TResult, in T> : IContainer<T>, IProxy<TResult>
    //{
    //}


    /// <summary>
    /// Proxy{T} is used with  <see cref="Projection{T}.MapWith{TProxy}"/>
    /// In cases when json can't be deserialized into a certain object model directly, 
    /// a proxy (or builder) object is used for deserialization, the proxy then creates target type (TResult)
    /// </summary>
    public interface IProxy<out TResult>
    {
        TResult Create();
    }


    /// <summary>
    /// It used by KeyedProjection{T,TItem}
    /// in case json represents an object with key values pairs but the target object model can't be deserialized directly
    /// the builder object is used like a proxy to deserialize json into it and than build the target T object i.e. call <see cref="Create"/>
    /// </summary>
    public interface IKeyedCollectionProxy<out T, in TItem> : IProxy<T>
    {
        void Add(string key, TItem item);
    }

    public interface IContainer<in T>
    {
        void Add(T item);
    }

    public interface IContainer<in TKey, in TValue>
    {
        void Add(TKey key, TValue value);
    }

    /// <summary>
    /// Maps an object of one type to another
    /// <code>
    ///     // use case: deserialize and convert deserialized concrete type to generic DTO
    ///     var binanceTrade = json.Deserialize&lt;BinanceTrade&gt;();
    ///     var futuresTrade = mapper.Map(binanceTrade); // mapper converts BinanceTrade to FuturesTrade
    /// </code>
    /// </summary>
    public interface IMapper<in TSource, out TResult>
    {
        /// <summary>
        /// Maps the specified source object to the result type.
        /// </summary>
        TResult Map(TSource obj);
    }

}