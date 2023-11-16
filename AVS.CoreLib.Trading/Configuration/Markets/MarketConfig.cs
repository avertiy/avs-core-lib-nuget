#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Trading.Helpers;
using static AVS.CoreLib.Trading.Configuration.Markets.MarketConfig;

namespace AVS.CoreLib.Trading.Configuration.Markets
{
    public class MarketConfig
    {
        public string Name { get; set; } = null!;
        public Combination[]? Spot { get; set; }
        public Combination[]? Futures { get; set; }

        public class Combination
        {
            public string Quote { get; set; } = null!;
            public string BaseAssets { get; set; } = null!;
            public string? All { get; set; }
            public string? Default { get; set; }
            public string? Preset1 { get; set; }
            public string? Preset2 { get; set; }
            public string? Preset3 { get; set; }

            public string? this[Preset preset]
            {
                get
                {
                    return preset switch
                    {
                        Preset.Default => Default,
                        Preset.Preset1 => Preset1,
                        Preset.Preset2 => Preset2,
                        Preset.Preset3 => Preset3,
                        Preset.All => All ?? BaseAssets,
                        _ => Default
                    };
                }
            }
        }
    }

    /// <summary>
    /// Define base asset(s) list <see cref="Combination"/> that will be combined with quote asset(s) to make trading pairs list
    /// <see cref="MarketConfigExtensions.GetPairs"/>
    /// Preset helps to reduce the number of available instruments and focus on a certain list of symbols
    /// </summary>
    public enum Preset
    {
        Default = 0,
        Preset1 = 1,
        Preset2 = 2,
        Preset3 = 3,
        All =4
    }

    public static class MarketConfigExtensions
    {
        public static string[] GetPairs(this Combination[]? combinations, Preset preset = Preset.Default)
        {
            if(combinations == null || combinations.Length == 0)
                return Array.Empty<string>();

            var result = new List<string>(15);

            foreach (var combination in combinations)
            {
                var assets = combination.GetBaseAssets(preset);
                if (assets.Any())
                {
                    var pairs = TradingPairHelper.Combine(combination.Quote, assets);
                    result.AddRange(pairs);
                }
            }

            return result.ToArray();
        }        

        public static string[] GetBaseAssets(this Combination combination, Preset preset)
        {
            var str = combination[preset];

            if (str == "*")
                str = combination.BaseAssets;

            return str == null ? Array.Empty<string>() : str.Split(',');
        }
    }
}
