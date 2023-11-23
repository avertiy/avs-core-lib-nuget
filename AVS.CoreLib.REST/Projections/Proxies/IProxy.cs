namespace AVS.CoreLib.REST.Projections
{
    /// <summary>
    /// Proxy{in T,out TResult} provides an interface to add T item(s) and produce the TResult     
    /// </summary>
    public interface IProxy<in T, out TResult> : IContainer<T>, IProxy<TResult>
    {
    }


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
    /// It used by <see cref="KeyedProjection{T,TItem}"/>
    /// in case json represents an object with key values pairs but the target object model can't be deserialized directly
    /// the builder object is used like a proxy to deserialize json into it and than build the target T object i.e. call <see cref="Create"/>
    /// </summary>
    public interface IKeyedCollectionProxy<out T, in TItem>
    {
        void Add(string key, TItem item);
        T Create();
    }

    public interface IContainer<in T>
    {
        void Add(T item);
    }

    public interface IContainer<in TKey, in TValue>
    {
        void Add(TKey key, TValue value);
    }
}