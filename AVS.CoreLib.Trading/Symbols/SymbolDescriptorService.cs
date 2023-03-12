#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Abstractions;

namespace AVS.CoreLib.Trading.Symbols
{
    public interface ISymbolDescriptorService
    {
        void UpdateRanks(IDictionary<string, int> coins);
        SymbolDescriptor GetDescriptor(string symbol);
        SymbolDescriptor[] GetAllDescriptors();
        IEnumerable<SymbolDescriptor> GetDescriptors(Func<SymbolDescriptor, bool> predicate);
    }

    public class SymbolDescriptorService : ISymbolDescriptorService
    {
        private readonly SymbolRegistry _registry = new SymbolRegistry();

        public SymbolDescriptorService()
        {
            Init();
        }

        private void Init()
        {
            var descriptors = new List<SymbolDescriptor>(200)
            {
                SymbolDescriptor.Fiat("USD"),
                SymbolDescriptor.Fiat("UAH"),
                SymbolDescriptor.StableCoin("USDT", 0),
                SymbolDescriptor.StableCoin("USDC", 0),
                SymbolDescriptor.StableCoin("BUSD", 0),
                SymbolDescriptor.StableCoin("DAI", 0),
                SymbolDescriptor.StableCoin("USDJ", 0),
                SymbolDescriptor.StableCoin("USDD", 0),
                SymbolDescriptor.StableCoin("TUSD", 0),
                SymbolDescriptor.StableCoin("GUSD", 0),
                SymbolDescriptor.StableCoin("USDP", 0),

                SymbolDescriptor.Top("BTC", 1, "Bitcoin"),
                SymbolDescriptor.Top("ETH", 2, "Etherium"),
                SymbolDescriptor.Top("BNB", 3, "BSC"),
                SymbolDescriptor.Top("XRP", 6, "Ripple"),
                SymbolDescriptor.Top("ADA", 7, "Cardano"),
                SymbolDescriptor.Top("MATIC", 8, "Polygon"),
                SymbolDescriptor.Top("DOGE", 9, "Dogecoin"),
                SymbolDescriptor.Top("SOL", 11, "Solana"),
                SymbolDescriptor.Top("DOT", 12, "Polkadot"),
                SymbolDescriptor.Top("SHIB", 13, "ShibaInu"),
                SymbolDescriptor.Top("TRX", 15, "Tron"),
                SymbolDescriptor.Top("LTC", 16, "Litecoin"),
                SymbolDescriptor.Top("AVAX", 17, "Avax"),
                SymbolDescriptor.TopRelated("UNI", 18, "ETH"),
                SymbolDescriptor.Top("LINK", 20, "Chainlink"),
                SymbolDescriptor.Top("ATOM", 21, "Cosmos"),
                SymbolDescriptor.TopRelated("WBTC",22, "ETH"),

                SymbolDescriptor.SecondTier("TON", 23, "TON"),
                SymbolDescriptor.SecondTier("XMR", 24, "Monero"),
                SymbolDescriptor.Blud("ETC",  25, "Etherium Classic"),
                SymbolDescriptor.TopRelated("OKB", 26, "ETH"),
                SymbolDescriptor.SecondTier("BCH", 27, "Bitcoin Cash"),
                SymbolDescriptor.SecondTier("FIL", 28, "Filecoin"),
                SymbolDescriptor.SecondTier("XLM", 29, "Stellar"),
                //LDO
                SymbolDescriptor.SecondTier("APT", 31, "Aptos"),
                SymbolDescriptor.SecondTier("HBAR",32, "Hedera"),
                SymbolDescriptor.SecondTier("CRO", 33, "Cronos"),//layer-one blockchain
                SymbolDescriptor.SecondTier("NEAR", 34, "NEAR Protocol"),//layer-one blockchain
                SymbolDescriptor.SecondTier("VET", 35, "VeChain"),
                SymbolDescriptor.SecondTier("QNT", 36, "Quant"),
                SymbolDescriptor.TopRelated("APE", 37, "ETH"),
                SymbolDescriptor.SecondTier("ICP", 38, "Internet Computer"),
                SymbolDescriptor.SecondTier("ALGO",3, "Algorand"),
                //TS
                SymbolDescriptor.SecondTier("EOS", 41, "Eos"),
                SymbolDescriptor.SecondTier("AAVE",46, "ETH"),
                SymbolDescriptor.SecondTier("XTZ", 47, "Tezos"),//smart-contract blockchain with voting to avoid forking
                SymbolDescriptor.SecondTier("FTM", 48, "Fantom"),//alt to etherium smart-contract platform for DApps
                SymbolDescriptor.SecondTier("THETA", 49, "Theta Network"),//blockchain for video-streaming
                SymbolDescriptor.TopRelated("OP", 64, "ETH"),//layer-two blockchain 
                SymbolDescriptor.TopRelated("CAKE",65, "BSC"),//AMM DeFi
                SymbolDescriptor.Blud("BSV", 66, "BSV"),//BCH hard-fork
                SymbolDescriptor.TopRelated("HT", 67, "ETH"),
                SymbolDescriptor.TopRelated("MKR", 68, "ETH"),
                //GUSD
                SymbolDescriptor.SecondTier("BTT", 70, "BTT"),
                SymbolDescriptor.SecondTier("MINA", 71, "Mina protocol"), // succinct blockchain
                SymbolDescriptor.SecondTier("DASH", 73, "Dash"),
                SymbolDescriptor.SecondTier("IOTA", 75, "Dash"),
                SymbolDescriptor.SecondTier("ZEC", 76, "Zcash"),//ptional anonymity blockchain
                SymbolDescriptor.SecondTier("KAVA", 86, "Kava network"),//layer-one blockchain
                SymbolDescriptor.SecondTier("CSPR", 91, "Casper network"),//layer-one blockchain 
                SymbolDescriptor.SecondTier("FLR", 92, "Flare network"),//layer-one EVM-based blockchain 
                SymbolDescriptor.SecondTier("1INCH", 94, "*"),//DEX on multiple networks ETH,BSC,Polygon,Fantom etc.
                SymbolDescriptor.Blud("ETHW", 99, "EtheriumPow"),//fork Etherium
                SymbolDescriptor.SecondTier("BAT", 101, "BAT"),
                //SSV
                SymbolDescriptor.SecondTier("XEM", 103, "XEM"),
                SymbolDescriptor.SecondTier("DYDX", 104, "DYDX"),
                SymbolDescriptor.Blud("LUNA", 105, "LUNA"),
                SymbolDescriptor.TopRelated("FLOKI", 106, "ETH,BSC"),//NFT Gaming
                SymbolDescriptor.TopRelated("COMP", 113, "ETH"),//DeFi
                SymbolDescriptor.SecondTier("DCR", 115, "Decred"),
                SymbolDescriptor.SecondTier("KSM", 120, "Polkadot"),//Like DOT with on-chain governance capabilities, multichain, nominated proof-of-stake (NPoS)
                SymbolDescriptor.SecondTier("XCH", 130, "Chia network"),//layer 1 blockchain with proof-of-space-and-time (PoST)
                SymbolDescriptor.SecondTier("WAVES", 140, "Waves"),
                SymbolDescriptor.TopRelated("OCEAN", 143, "ETH"),
                SymbolDescriptor.TopRelated("JST", 147, "Tron"),//DeFi
                SymbolDescriptor.TopRelated("BAND", 153, "*"),//cross-chain data oracle platform
                SymbolDescriptor.SecondTier("FLUX", 157, "Flux"),//DApp web3 
                SymbolDescriptor.TopRelated("OMG", 158, "ETH"),// layer-2 scaling solution built for the Ethereum
                SymbolDescriptor.TopRelated("AMP", 159, "ETH"),// layer-2 scaling solution built for the Ethereum
                SymbolDescriptor.SecondTier("ZRX", 164, "0x"),
                SymbolDescriptor.SecondTier("ONT", 166, "Ontology"),//identity and data solutions to Web3
                SymbolDescriptor.TopRelated("UMA", 187, ""),
                SymbolDescriptor.SecondTier("LSK", 192, ""),
                SymbolDescriptor.TopRelated("APENFT", 194, ""),
                SymbolDescriptor.TopRelated("DFI", 219, "BTC"), //DeFi for BTC
            };
            _registry.AddRange(descriptors);
        }

        public void UpdateRanks(IDictionary<string, int> coins)
        {
            foreach (var kp in coins)
            {
                if (_registry.Contains(kp.Key))
                {
                    _registry[kp.Key].Rank = kp.Value;
                }
                else
                {
                    _registry.Add(SymbolDescriptor.Other(kp.Key, kp.Value));
                }
            }
        }

        public SymbolDescriptor GetDescriptor(string symbol)
        {
            return _registry[symbol];
        }

        public SymbolDescriptor[] GetAllDescriptors()
        {
            return _registry.GetAll();
        }

        public IEnumerable<SymbolDescriptor> GetDescriptors(Func<SymbolDescriptor, bool> predicate)
        {
            return _registry.Where(predicate);
        }

        /// <summary>
        /// Get descriptors filtered by <see cref="IQuery{T}"/>
        /// you can use <see cref="SymbolDescriptorsQuery"/> or your own implementation
        /// </summary>
        public IEnumerable<SymbolDescriptor> GetDescriptors(IQuery<SymbolDescriptor> query)
        {
            var descriptors = _registry.GetAll().AsQueryable();
            return query.Filter(descriptors);
        }
    }
}