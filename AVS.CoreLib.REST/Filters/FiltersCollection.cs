using System.Collections.ObjectModel;
using AVS.CoreLib.Abstractions.Rest;

namespace AVS.CoreLib.REST.Filters
{
    public class FiltersCollection<T> : Collection<IFilter<T>>, IFilter<T>
    {
        public T Process(T obj)
        {
            foreach (var filter in Items)
                obj = filter.Process(obj);
            return obj;
        }
    }
}