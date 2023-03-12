using System;
using System.Linq;

namespace AVS.CoreLib.Abstractions
{
    public interface IQuery<T>
    {
        IQueryable<T> Filter(IQueryable<T> queryable);
    }
}