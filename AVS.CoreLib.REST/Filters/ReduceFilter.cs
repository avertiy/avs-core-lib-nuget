using AVS.CoreLib.REST.Reduce;

namespace AVS.CoreLib.REST.Filters
{
    public class ReduceFilter<T> : Filter<ReduceOptions, T> where T : IReduceable
    {
        public ReduceFilter(ReduceOptions value) : base(value)
        {
        }

        public override T Process(T obj)
        {
            obj.Reduce(Value);
            return obj;
        }
    }
}