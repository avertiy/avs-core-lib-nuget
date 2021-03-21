namespace AVS.CoreLib.REST.Projections
{
    public interface IObjectBuilder<out T>
    {
        T Create();
    }

    public interface IObjectBuilder<out T, in TData>
    {
        T Create();
        void Add(TData data);
    }

    public interface IArrayBuilder<out T, in TItem>
    {
        void Add(TItem item);
        T Create();
    }

    public interface IKeyedCollectionBuilder<out T, in TItem>
    {
        void Add(string key, TItem item);
        T Create();
    }
}