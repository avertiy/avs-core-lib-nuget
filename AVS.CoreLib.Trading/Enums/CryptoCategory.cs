using System;

namespace AVS.CoreLib.Trading.Enums
{
    [Flags]
    public enum CryptoCategory
    {
        Top = 1,
        Fiat = 2,
        StableCoin = 4,
        DeFi = 8,
        Gen2d = 16,
        LeveragedToken = 32,
        ShitCoin = 64,
        AllCrypto = Top | StableCoin | Gen2d | LeveragedToken | DeFi | ShitCoin,
        All = Top | Fiat | StableCoin | Gen2d | LeveragedToken | DeFi | ShitCoin
    }

    public enum Fiat
    {
        Main = 1,
        All = 2
    }

    //public enum Exchange
    //{
    //    Binance = 0,
    //    Exmo = 1, 
    //    HitBtc,
    //    Kuna,
    //    Poloniex,
    //    Bitfinex,
    //    Bitmex,
    //    Bitstamp,
    //    Bittrex,
    //    Coinbase,
    //    Huobi,
    //    Kraken,
    //    KuCoin,
    //    Liquid,
    //    OKEx
    //}
}