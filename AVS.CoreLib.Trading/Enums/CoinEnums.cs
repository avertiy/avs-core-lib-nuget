namespace AVS.CoreLib.Trading.Enums
{
    public enum CoinType
    {
        None = 0,
        Fiat = 1,
        Blockchain = 2,
        StableCoin = 3,
        Token = 4,
    }

    public enum TokenCategory
    {
        None = 0,
        DeFi,
        NFT,
        Gaming,
        Wrapped,
        Web3,
        AMM,
        IoT,
        Rollups,
        Wallet,
        Other
    }

    public enum CoinCap
    {
        None = 0,
        /// <summary>
        /// 1+ billion ~ top 50 coins
        /// </summary>
        BigCap,
        /// <summary>
        /// 250M+ (rank 50 - 125)
        /// </summary>
        MidCap,
        /// <summary>
        /// 50M - 250M (rank 125-375)
        /// </summary>
        SmallCap,
        /// <summary>
        /// 2M - 50M (rank 375-1200)
        /// </summary>
        MicroCap,
        /// <summary>
        /// 200k - 2M (rank 1200 - 2000)
        /// </summary>
        SeedCap,
    }

    public enum AssetClass
    {
        None,
        /// <summary>
        /// Top BTC;ETH;BNB;XRP
        /// </summary>
        Top,

        /// <summary>
        /// Ecosystem around Top asset for example: ETH-> UNI,AAVE, MKR; TRX -> SUN,JST,APENFT
        /// </summary>
        TopRelated,
        /// <summary>
        /// DASH, XMR, EOS, BCH,FIL etc.
        /// </summary>
        SecondTier,

        /// <summary>
        /// 3rd tier or other coins with not prominent even weak perspectives like XEM etc.
        /// </summary>
        Other,
        /// <summary>
        /// DOGE, SHIB, FLOKI etc.
        /// </summary>
        Blud,
        /// <summary>
        /// things like PARA, THETA etc.
        /// </summary>
        DarkHorse,
        /// <summary>
        /// dangerous things that has no or very poor fundamental, and might scam
        /// </summary>
        Shit
    }

    //public enum InvestType
    //{
    //    /// <summary>
    //    /// SOLID fundamental  + top 100 by coin market cap + like BNB, TRX etc.
    //    /// </summary>
    //    LongTerm,
    //    /// <summary>
    //    /// secondary tokens bound to TOP asset things like SUN, JST etc. 
    //    /// </summary>
    //    MidTerm,
    //    /// <summary>
    //    /// Top 100 by coin market cap but WEAK fundamental like EOS, XMR, DASH, XEM etc.
    //    /// </summary>
    //    ShortTerm,
    //    Risky,
    //}


    ///// <summary>
    ///// Coin rating
    ///// AAA - BigCap + Top Rank, at the moment only BTC, ETH + USDT/USDC 
    ///// AA  - BigCap + Top 20 Rank, assets like BNB, TRX, XRP, ADA etc.
    ///// A - BigCap (or close to 1B cap) assets like APT, EOS, XMR, AAVE, XTZ etc.
    ///// BBB - MidCap + Top 100 Rank
    ///// BB - MidCap + Top 300 Rank
    ///// B - SmallCap + Top 500 Rank 
    ///// etc.
    ///// DarkHorse - new token/project that might give Xs profit or nothing like FLOKI 
    ///// </summary>
    //public enum CoinRating
    //{
    //    None = 0,
    //    AAA =1,
    //    AA = 2,
    //    A =3,
    //    BBB = 4,
    //    BB =5,
    //    B = 6,
    //    CCC = 7,
    //}
}