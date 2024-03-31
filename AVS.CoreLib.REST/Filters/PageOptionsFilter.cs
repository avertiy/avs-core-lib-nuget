using AVS.CoreLib.REST.Pagination;

namespace AVS.CoreLib.REST.Filters
{
    public class PageOptionsFilter<T> : Filter<PageOptions, T> where T : IPageable
    {
        public PageOptionsFilter(PageOptions value) : base(value)
        {
        }

        public override T Process(T obj)
        {
            obj.ApplyPageOptions(Value);
            return obj;
        }
    }
}