#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using AVS.CoreLib.Trading.Enums;

namespace AVS.CoreLib.Trading.Symbols
{
    internal class SymbolRegistry
    {
        private readonly List<SymbolDescriptor> _descriptors = new List<SymbolDescriptor>();


        public void AddRange(IEnumerable<SymbolDescriptor> descriptors) 
        {
            _descriptors.AddRange(descriptors);
        }


        public void Add(SymbolDescriptor descriptor)
        {
            _descriptors.Add(descriptor);
        }

        public bool Contains(string symbol)
        {
            return _descriptors.Any(x => x.Symbol == symbol);
        }

        public SymbolDescriptor GetDescriptor(string symbol)
        {
            return _descriptors.First(x => x.Symbol == symbol);
        }

        public SymbolDescriptor this[string symbol] => GetDescriptor(symbol);

        public IEnumerable<SymbolDescriptor> GetByAssetType(AssetClass @class)
        {
            return _descriptors.Where(x => x.Class == @class);
        }

        public IEnumerable<SymbolDescriptor> Where(Func<SymbolDescriptor, bool> predicate)
        {
            return _descriptors.Where(predicate);
        }

        public SymbolDescriptor[] GetAll()
        {
            return _descriptors.ToArray();
        }
    }
}