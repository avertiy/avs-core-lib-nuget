using System;

namespace AVS.CoreLib.Trading.Enums
{
    [Flags]
    public enum CryptoCategory
    {
        CryptoCurrency =0,
        Fiat =1,
        SmartContract = 2,
        StableCoin = 4,
        Token = 8,
        DeFi =16,
        Top20 = 32,
        ShitCoin =64,
        AllCrypto = CryptoCurrency | SmartContract | StableCoin | Token  | DeFi | ShitCoin,
        All = CryptoCurrency | Fiat | SmartContract | StableCoin | Token | DeFi | ShitCoin,
    }
}