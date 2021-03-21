using AVS.CoreLib.Abstractions.Rest;

namespace AVS.CoreLib.REST.Filters
{
    public abstract class Filter<T, TData> : IFilter<TData>
    {
        public T Value { get; set; }

        protected Filter()
        {
        }

        protected Filter(T value)
        {
            Value = value;
        }

        public abstract TData Process(TData obj);

        public static implicit operator T(Filter<T, TData> filter)
        {
            return filter.Value;
        }
    }
}
