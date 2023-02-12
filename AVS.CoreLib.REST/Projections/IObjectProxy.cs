namespace AVS.CoreLib.REST.Projections
{
    /// <summary>
    /// ObjectProxy comes to the rescue when you deal with <see cref="ObjectProjection{T}"/>
    /// In cases when json can't be deserialized into a certain object model directly, the proxy object (or object builder)
    /// used for deserialization and then it produces (constructs) the target type (projection)
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
    /// ArrayProxy comes to the rescue when you deal with <see cref="ArrayProjection{T,TItem}"/>
    /// In cases when json can't be deserialized into a certain model directly, the proxy object is used to deserialize json into it, and then it produces
    /// the target type (projection) 
    /// </summary>
    /// <remarks>proxy might help to eliminate routine and memory consumption for large arrays when you do filtering or processing items without need to keep them as objects</remarks>
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