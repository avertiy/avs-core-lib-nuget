namespace AVS.CoreLib.REST.Projections
{
    /// <summary>
    /// Object Proxy is used by <see cref="ObjectProjection{T}"/>
    /// in case json can't be deserialized into a certain object model directly, the proxy object (in this case ObjectBuilder)
    /// could help with the deserialization, the corresponding ObjectProjection will call <see cref="Create"/> method to build the target object
    /// </summary>
    public interface IObjectProxy<out T>
    {
        T Create();
    }

    /// <summary>
    /// used by <see cref="ObjectProjection{T,TItem}"/> to setup builder
    /// </summary>
    public interface IObjectProxy<out T, in TData>
    {
        T Create();
        void Add(TData data);
    }

    /// <summary>
    /// Array Proxy is used by <see cref="ArrayProjection{T,TItem}"/>
    /// in case json represents an array but the target object model can't be deserialized directly
    /// the builder object is used like a proxy to deserialize json into it and than build the target T object 
    /// </summary>
    public interface IArrayProxy<out T, in TItem>
    {
        void Add(TItem item);
        T Create();
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
}