#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Guards;

namespace AVS.CoreLib.Trading.Configuration.Markets
{
    public interface IMarketsConfigManager
    {
        string[] GetQuoteAssets(string name);
        string[] GetTradingPairs(string name, Preset preset = Preset.Default);
        string[] GetBaseAssets(string name, Preset preset = Preset.Default, string? quote = null);
    }

    public class MarketsConfigManager : IMarketsConfigManager
    {
        private readonly IDictionary<string, MarketConfig> _items = new Dictionary<string, MarketConfig>();

        public MarketsConfigManager(MarketConfig[] markets)
        {
            //Guard.AgainstNull(markets, "`markets` config is missing");
            Init(markets);
        }

        protected void Init(MarketConfig[] marketConfigs)
        {
            foreach (var marketConfig in marketConfigs.OrderBy(x => x.Name))
                Add(marketConfig);
        }

        protected void Add(MarketConfig config)
        {
            Guard.AgainstNullOrEmpty(config.Name, "Market name must be not empty [e.g. Binance or Binance:main]");
            _items.Add(config.Name, config);
        }

        public MarketConfig Get(string name)
        {
            if (_items.ContainsKey(name))
                return _items[name];

            var key = _items.Keys.FirstOrDefault(x => x.ToLower().Contains(name.ToLower()));

            if (key != null)
                return _items[key];

            throw new KeyNotFoundException($"{nameof(MarketConfig)} for `{name}` does not exist");
        }

        public MarketConfig this[string name] => Get(name);

        public ICollection<MarketConfig> GetAll()
        {
            return _items.Values;
        }

        public string[] GetTradingPairs(string name, Preset preset = Preset.Default)
        {
            return Get(name).GetTradingPairs(preset);
        }

        public string[] GetQuoteAssets(string name)
        {
            return Get(name).Combinations.Select(x => x.Quote).ToArray();
        }

        public string[] GetBaseAssets(string name, Preset preset = Preset.Default, string? quote = null)
        {
            var config = Get(name);
            if (quote == null)
                return config.Combinations.SelectMany(x => x.GetBaseAssets(preset)).ToArray();

            var assets = config.Combinations.FirstOrDefault(x => x.Quote == quote)?.GetBaseAssets(preset);
            return assets ?? Array.Empty<string>();
        }
    }
}