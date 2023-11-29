using System;
using System.Linq;
using System.Linq.Expressions;
using AVS.CoreLib.Extensions.Enums;

namespace AVS.CoreLib.Extensions.Linq
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Limit<T>(this IQueryable<T> source, int limit)
        {
            return limit > 0 ? source.Take(limit) : source;
        }

        public static IOrderedQueryable<T> OrderBy<T, Key>(this IQueryable<T> source, Expression<Func<T, Key>> keySelector, Sort sort)
        {
            return sort == Sort.Desc ? source.OrderByDescending(keySelector) : source.OrderBy(keySelector);            
        }

        public static IQueryable<T> ThenBy<T, Key>(this IOrderedQueryable<T> source, Expression<Func<T, Key>> keySelector, Sort sort)
        {
            return sort == Sort.Desc ? source.ThenByDescending(keySelector) : source.ThenBy(keySelector);
        }
    }
}