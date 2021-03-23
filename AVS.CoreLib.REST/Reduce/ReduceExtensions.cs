using System;
using System.Collections.Generic;
using System.Linq;

namespace AVS.CoreLib.REST.Reduce
{
    public static class ReduceExtensions
    {
        /// <summary>
        /// Reduce number of items in list by applying reducer function to ones that less than threashold
        /// </summary>
        /// <param name="items">items collection</param>
        /// <param name="selector">key property selector</param>
        /// <param name="reducer">reducer is a function that accepts many items and returns one</param>
        /// <param name="options"><see cref="ReduceOptions"/> sets threshold, items that has lower value than the threshold is a subject for reducer function</param>
        public static IList<T> Reduce<T>(
            this IList<T> items,
            Func<T, decimal> selector,
            Func<T[], T> reducer,
            ReduceOptions options = default)
        {
            if (items.Count < 5 || !options.HasValue)
                return items;

            decimal threshold = options.Value;

            if (options.IsDynamic)
            {
                decimal maxValue = 0;
                decimal sum = 0;
                //calculate avereage
                //but to eliminate reduce issue when 1 big order is much greater than sum of others
                //so the average value will set an unreacheable threshold for all orders except the big one
                for (int i = 0; i < items.Count; i++)
                {
                    var value = selector(items[i]);
                    if (value > maxValue)
                        maxValue = value;
                    sum += value;
                }

                var avg = (sum - maxValue) / (items.Count - 1);
                threshold = avg * options.Factor;
            }

            var list = new List<T>();
            var tmpSum = 0m;
            var tmpList = new List<T>();

            foreach (var item in items)
            {
                var value = selector(item);
                if (value >= threshold)
                {
                    if (tmpList.Any())
                    {
                        if (tmpSum < threshold / 2)
                        {
                            tmpList.Add(item);
                            var combinedItem = reducer(tmpList.ToArray());
                            list.Add(combinedItem);
                        }
                        else
                        {
                            var combinedItem = reducer(tmpList.ToArray());
                            list.Add(combinedItem);
                            list.Add(item);
                        }
                        tmpList.Clear();
                        tmpSum = 0m;
                    }
                    else
                    {
                        list.Add(item);
                    }
                    continue;
                }

                tmpList.Add(item);
                tmpSum += value;
                if (tmpSum >= threshold)
                {
                    var combinedItem = reducer(tmpList.ToArray());
                    list.Add(combinedItem);
                    tmpList.Clear();
                    tmpSum = 0m;
                }
            }

            if (tmpList.Count > 0)
            {
                var combinedItem = reducer(tmpList.ToArray());
                list.Add(combinedItem);
            }

            return list;
        }

        public static IList<T> Reduce<T>(
            this IList<T> items,
            Func<T, decimal> selector,
            Func<T[], T[]> reducer,
            ReduceOptions options = default)
        {
            if (items.Count < 5 || !options.HasValue)
                return items;

            decimal threshold = options.Value;

            if (options.IsDynamic)
            {
                decimal maxValue = 0;
                decimal sum = 0;
                //calculate avereage
                //but to eliminate reduce issue when 1 big order is much greater than sum of others
                //so the average value will set an unreacheable threshold for all orders except the big one
                for (int i = 0; i < items.Count; i++)
                {
                    var value = selector(items[i]);
                    if (value > maxValue)
                        maxValue = value;
                    sum += value;
                }

                var avg = (sum - maxValue) / (items.Count - 1);
                threshold = avg * options.Factor;
            }

            var list = new List<T>();
            var tmpSum = 0m;
            var tmpList = new List<T>();


            foreach (var item in items)
            {
                var value = selector(item);
                if (value >= threshold)
                {
                    list.Add(item);
                    continue;
                }

                tmpList.Add(item);
                tmpSum += value;
                if (tmpSum >= threshold)
                {
                    var combinedItem = reducer(tmpList.ToArray());
                    list.AddRange(combinedItem);
                    tmpList.Clear();
                    tmpSum = 0m;
                }
            }

            if (tmpList.Count > 0)
            {
                var combinedItem = reducer(tmpList.ToArray());
                list.AddRange(combinedItem);
            }

            return list;
        }
    }
}