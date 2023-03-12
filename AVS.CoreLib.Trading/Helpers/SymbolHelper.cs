using System.Collections.Generic;

namespace AVS.CoreLib.Trading.Helpers
{
    public static class SymbolHelper
    {
        public static string[] Generate(string[] baseCurrencies, params string[] quoteCurrencies)
        {
            var symbols = new List<string>();
            foreach (var baseCurrency in baseCurrencies)
            {
                foreach (var quoteCur in quoteCurrencies)
                {
                    if (quoteCur == baseCurrency)
                        continue;
                    symbols.Add(baseCurrency + "_" + quoteCur);
                }
            }
            return symbols.ToArray();
        }

        public static string[] Generate(string baseCurrency, params string[] quoteCurrencies)
        {
            var symbols = new List<string>();
            foreach (var quoteCur in quoteCurrencies)
            {
                if (quoteCur == baseCurrency)
                    continue;
                symbols.Add(baseCurrency + "_" + quoteCur);
            }
            return symbols.ToArray();
        }

        public static string[] GenerateSymbols(string quoteCurrency, params string[] baseCurrencies)
        {
            var symbols = new List<string>();
            foreach (var baseCurr in baseCurrencies)
            {
                if (baseCurr == quoteCurrency)
                    continue;
                symbols.Add(baseCurr + "_" + quoteCurrency);
            }
            return symbols.ToArray();
        }
    }
}