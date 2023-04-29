#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Guards;
using AVS.CoreLib.Trading.Enums;

namespace AVS.CoreLib.Trading.Configuration.Markets
{
    public interface IMarketsConfigManager
    {
        bool Contains(string name);
        string[] GetTradingPairs(string name, AccountType account, Preset preset);
        string[] GetQuoteAssets(string name, AccountType accountType);
        string[] GetBaseAssets(string name, AccountType account, Preset preset, string? quote = null);
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

        public bool Contains(string name)
        {
            return _items.ContainsKey(name) || _items.Keys.Any(x => x.ToLower().Contains(name.ToLower()));
        }

        /// <summary>
        /// trading pairs for spot
        /// </summary>
        public string[] GetTradingPairs(string name, AccountType accountType = AccountType.Spot, Preset preset = Preset.Default)
        {
            var combinations = Get(name, accountType);
            return combinations?.GetPairs(preset) ?? Array.Empty<string>();
        }

        public string[] GetQuoteAssets(string name, AccountType accountType = AccountType.Spot)
        {
            var combinations = Get(name, accountType);
            return combinations?.Select(x => x.Quote).ToArray() ?? Array.Empty<string>();
        }

        public string[] GetBaseAssets(string name, AccountType accountType, Preset preset, string? quote = null)
        {
            var combinations = Get(name, accountType);

            var assets = quote == null ?
                combinations?.SelectMany(x => x.GetBaseAssets(preset)).ToArray() :
                combinations?.FirstOrDefault(x => x.Quote == quote)?.GetBaseAssets(preset);

            return assets ?? Array.Empty<string>();
        }

        private MarketConfig.Combination[]? Get(string name, AccountType account)
        {
            var config = Get(name);
            var combinations = account == AccountType.Futures ? config.Futures : config.Spot;
            return combinations;
        }

        /*
        public string[] GetFuturesPairs(string name, Preset preset = Preset.Default)
        {
            var combinations = Get(name).Futures;
            return combinations?.GetPairs(preset) ?? Array.Empty<string>();
        }

        public string[] GetFuturesQuoteAssets(string name)
        {
            return Get(name).Futures?.Select(x => x.Quote).ToArray() ?? Array.Empty<string>();
        }*/
    }
}