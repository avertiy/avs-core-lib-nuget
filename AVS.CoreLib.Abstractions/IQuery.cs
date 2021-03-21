using System.Linq;

namespace AVS.CoreLib.Abstractions
{
    public interface IQuery<T>
    {
        IQueryable<T> Apply(IQueryable<T> queryable);
    }
}