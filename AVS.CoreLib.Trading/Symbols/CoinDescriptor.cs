#nullable enable
using AVS.CoreLib.Trading.Enums;
using AVS.CoreLib.Trading.Extensions;

namespace AVS.CoreLib.Trading.Symbols
{
    /// <summary>
    /// descriptors should help to classify coin-zoo 
    /// </summary>
    public class SymbolDescriptor
    {
        public string Symbol { get; set; } = null!;
        public string? Blockchain { get; set; }
        /// <summary>
        /// what exchanges have trading pairs with this symbol possible values
        /// - `*` - widely accepted listed on many exchanges e.g. LTC
        /// - `Binance+` - listed on Binance and other markets
        /// - `Binance!` - listed on Binance but not listed on other big markets usually new listings on Binance 
        /// - `Gate` - listed on gate.io i.e. not listed on Binance
        /// - `DEX` - listed on DEX only
        /// </summary>
        public string Markets { get; set; } = null!;
        public CoinType Type { get; set; }
        public CoinCap Cap { get; set; }
        public AssetClass Class { get; set; }
        public int Rank { get; set; }

        public SymbolDescriptor()
        {
        }

        public SymbolDescriptor(string symbol, string? blockchain, string markets, CoinType type, CoinCap cap, AssetClass @class, int rank)
        {
            Symbol = symbol;
            Blockchain = blockchain;
            Markets = markets;
            Type = type;
            Cap = cap;
            Class = @class;
            Rank = rank;
        }

        public static SymbolDescriptor Fiat(string symbol)
        {
            return new SymbolDescriptor() { Symbol = symbol, Type = CoinType.Fiat, };
        }

        public static SymbolDescriptor StableCoin(string symbol, int rank)
        {
            return new SymbolDescriptor() { Symbol = symbol, Type = CoinType.StableCoin, Rank = rank, Cap = rank.GetCoinCapByRank() };
        }

        public static SymbolDescriptor Top(string symbol, int rank, string blockchain, CoinType type = CoinType.Blockchain)
        {
            return new SymbolDescriptor() { Symbol = symbol, Rank = rank, Blockchain = blockchain, Type = type, Class = AssetClass.Top, Cap = rank.GetCoinCapByRank() };
        }

        public static SymbolDescriptor TopRelated(string symbol, int rank, string blockchain, CoinType type = CoinType.Token)
        {
            return new SymbolDescriptor() { Symbol = symbol, Rank = rank, Blockchain = blockchain, Type = type, Class = AssetClass.TopRelated, Cap = rank.GetCoinCapByRank() };
        }

        /// <summary>
        /// Блудняк like DOGE, FLOKI etc.
        /// </summary>
        public static SymbolDescriptor Blud(string symbol, int rank, string blockchain, CoinType type = CoinType.Blockchain)
        {
            return new SymbolDescriptor() { Symbol = symbol, Blockchain = blockchain, Type = type, Rank = rank, Class = AssetClass.Blud, Cap = rank.GetCoinCapByRank() };
        }

        public static SymbolDescriptor SecondTier(string symbol, int rank, string blockchain, CoinType type = CoinType.Blockchain)
        {
            return new SymbolDescriptor() { Symbol = symbol, Blockchain = blockchain, Type = type, Rank = rank, Class = AssetClass.SecondTier, Cap = rank.GetCoinCapByRank() };
        }

        public static SymbolDescriptor Other(string symbol, int rank, string? blockchain = null, CoinType type = CoinType.None)
        {
            return new SymbolDescriptor() { Symbol = symbol, Blockchain = blockchain, Type = type, Rank = rank, Class = AssetClass.Other, Cap = rank.GetCoinCapByRank() };
        }

        public static SymbolDescriptor ShitCoin(string symbol, int rank, string? blockchain = null)
        {
            return new SymbolDescriptor() { Symbol = symbol, Rank = rank, Blockchain = blockchain, Class = AssetClass.Shit, Cap = rank.GetCoinCapByRank() };
        }

        //public static SymbolDescriptor Token(string symbol, int rank, string blockchain, AssetClass @class)
        //{
        //    return new SymbolDescriptor() { Symbol = symbol, Blockchain = blockchain, Type = CoinType.Token, Rank = rank, Class = @class, Cap = rank.GetCoinCapByRank() };
        //}

        //public static SymbolDescriptor NFT(string symbol, int rank, string blockchain, AssetClass @class)
        //{
        //    return new SymbolDescriptor() { Symbol = symbol, Blockchain = blockchain, Type = CoinType.Token, Rank = rank, Class = @class, Cap = rank.GetCoinCapByRank() };
        //}

        //public static SymbolDescriptor DeFi(string symbol, int rank, string blockchain, AssetClass @class)
        //{
        //    return new SymbolDescriptor() { Symbol = symbol, Blockchain = blockchain, Type = CoinType.Token, Rank = rank, Class = @class, Cap = rank.GetCoinCapByRank() };
        //}


    }
}