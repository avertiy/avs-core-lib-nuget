using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Trading.Structs;

namespace AVS.CoreLib.Trading.Helpers
{
    public static class PairHelper
    {
        public static PairString[] CreateCombinations(string baseCurrencies, string quoteCurrencies, bool isBaseCurrencyFirst = true)
        {
            var pairs = new List<PairString>();
            foreach (var baseCur in baseCurrencies.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).OrderBy(c => c))
            {
                foreach (var quoteCur in quoteCurrencies.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).OrderBy(c => c))
                {
                    if (isBaseCurrencyFirst)
                        pairs.Add(baseCur + "_" + quoteCur);
                    else
                        pairs.Add(quoteCur + "_" + baseCur);
                }
            }
            return pairs.ToArray();
        }

        public static string[] GeneratePairs(string[] baseCurrencies, string[] quoteCurrencies)
        {
            var pairs = new List<string>();
            foreach (var baseCurrency in baseCurrencies)
            {
                foreach (var quoteCur in quoteCurrencies)
                {
                    if (quoteCur == baseCurrency)
                        continue;
                    pairs.Add(baseCurrency + "_" + quoteCur);
                }
            }
            return pairs.ToArray();
        }

        public static string[] GeneratePairs(string baseCurrency, params string[] quoteCurrencies)
        {
            var pairs = new List<string>();
            foreach (var quoteCur in quoteCurrencies)
            {
                if (quoteCur == baseCurrency)
                    continue;
                pairs.Add(baseCurrency + "_" + quoteCur);
            }
            return pairs.ToArray();
        }

        public static string[] GenerateSymbols(string quoteCurrency, params string[] baseCurrencies)
        {
            var pairs = new List<string>();
            foreach (var baseCurr in baseCurrencies)
            {
                if (baseCurr == quoteCurrency)
                    continue;
                pairs.Add(baseCurr + "_" + quoteCurrency);
            }
            return pairs.ToArray();
        }

        public static PairString[] CreateCombinations(string baseCurrency, bool isBaseCurrencyFirst,
            params string[] currencies)
        {
            var pairs = new List<PairString>();

            foreach (var quoteCur in currencies)
            {
                if (quoteCur == baseCurrency)
                    continue;
                if (isBaseCurrencyFirst)
                    pairs.Add(baseCurrency + "_" + quoteCur);
                else
                    pairs.Add(quoteCur + "_" + baseCurrency);
            }

            return pairs.ToArray();

        }

        public static string Normalize(string pair, bool isBaseCurrencyFirst)
        {
            return isBaseCurrencyFirst ? pair : pair.Swap('_');
        }
    }
}