using System.Linq;

namespace AVS.CoreLib.Abstractions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IQuery<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryable"></param>
        /// <returns></returns>
        IQueryable<T> Filter(IQueryable<T> queryable);
    }
}