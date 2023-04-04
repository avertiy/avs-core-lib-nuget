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
        public Combination[] Combinations { get; set; } = null!;

        public class Combination
        {
            public string Quote { get; set; } = null!;
            public string BaseAssets { get; set; } = null!;
            public string? Default { get; set; }
            public string? Preset1 { get; set; }
            public string? Preset2 { get; set; }
        }
    }

    /// <summary>
    /// Define base asset(s) list <see cref="Combination"/> that will be combined with quote asset(s) to make trading pairs list
    /// <see cref="MarketConfigExtensions.GetTradingPairs"/>
    /// Preset helps to reduce the number of available instruments and focus on a certain list of symbols
    /// </summary>
    public enum Preset
    {
        Default = 0,
        Preset1 = 1,
        Preset2 = 2,
        All =3
    }

    public static class MarketConfigExtensions
    {
        public static string[] GetTradingPairs(this MarketConfig config, Preset preset = Preset.Default)
        {
            var result = new List<string>(15);
            foreach (var combination in config.Combinations)
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
            var str = preset switch
            {
                Preset.Default => combination.Default,
                Preset.Preset1 => combination.Preset1,
                Preset.Preset2 => combination.Preset2,
                _ => combination.BaseAssets
            };
            
            return str == null ? Array.Empty<string>() : str.Split(',');
        }
    }
}
