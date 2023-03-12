#nullable enable
using System.Linq;
using AVS.CoreLib.Abstractions;
using AVS.CoreLib.Trading.Enums;

namespace AVS.CoreLib.Trading.Symbols
{
    public class SymbolDescriptorsQuery : IQuery<SymbolDescriptor>
    {
        public string? Market { get; set; }
        public int? Rank { get; set; }
        public CoinType? Type { get; set; }
        public CoinCap? Cap { get; set; }
        public AssetClass? Class { get; set; }
        public string? Blockchain { get; set; }

        public IQueryable<SymbolDescriptor> Filter(IQueryable<SymbolDescriptor> query)
        {
            if (Cap.HasValue)
                query = query.Where(x => x.Cap == Cap.Value);

            if (Rank.HasValue)
                query = query.Where(x => x.Rank <= Rank.Value);

            if (Class.HasValue)
                query = query.Where(x => x.Class <= Class.Value);

            if (Type.HasValue)
                query = query.Where(x => x.Type <= Type.Value);

            if (!string.IsNullOrEmpty(Blockchain))
                query = query.Where(x => x.Blockchain == Blockchain);

            if (!string.IsNullOrEmpty(Market))
                query = query.Where(x => (x.Markets == "*" || x.Markets.Contains(Market)));

            return query;
        }
    }
}