using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Trading.Abstractions;
using AVS.CoreLib.Trading.Collections;
using AVS.CoreLib.Trading.Enums;

namespace AVS.CoreLib.Trading.Extensions
{
    public static class EnumerableDataExtensions
    {
        public static IEnumerable<TData> MatchQuoteCurrency<TData>(
            this IEnumerable<TData> source,
            CurrencyCollection currencies)
            where TData : ISymbol
        {
            if (currencies == null || !currencies.Any())
                return source;
            return source.Where(x => currencies.MatchQuoteCurrency(x.Symbol));
        }

        public static IEnumerable<TData> MatchBaseCurrency<TData>(
            this IEnumerable<TData> source,
            CurrencyCollection currencies)
            where TData : ISymbol
        {
            if (currencies == null || !currencies.Any())
                return source;
            return source.Where(x => currencies.MatchBaseCurrency(x.Symbol));
        }

        public static IEnumerable<TData> MatchBaseCurrency<TData>(this IEnumerable<TData> source,
            string currency) where TData : ISymbol
        {
            return source.Where(x => x.Symbol.EndsWith("_" + currency));
        }

        public static IEnumerable<TData> MatchQuoteCurrency<TData>(this IEnumerable<TData> source,
            string currency) where TData : ISymbol
        {
            return source.Where(x => x.Symbol.StartsWith(currency + "_"));
        }

        public static IEnumerable<TData> MatchAnyExchange<TData>(
            this IEnumerable<TData> source, params string[] exchanges)
            where TData : IExchange
        {
            if (exchanges == null)
                throw new ArgumentNullException(nameof(exchanges));
            return source.Where(x => exchanges.Contains(x.Exchange));
        }

        public static IEnumerable<TData> MatchExchange<TData>(
            this IEnumerable<TData> source,
            MatchType type,
            params string[] exchanges)
            where TData : IExchange, ISymbol
        {
            if (exchanges == null)
                throw new ArgumentNullException(nameof(exchanges));

            if (type == MatchType.Any || exchanges.Length == 1)
                return source.Where(x => exchanges.Contains(x.Exchange));

            var result = new List<TData>();
            foreach (var grouping in source.GroupBy(x => x.Symbol))
            {
                if (exchanges.All(x => grouping.Any(t => t.Exchange == x)))
                    result.AddRange(grouping);
            }
            return result;
        }
    }
}