using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AVS.CoreLib.Trading.Abstractions;

namespace AVS.CoreLib.Trading.Collections
{
    public interface IDataCollection<TData> : ICollection<TData>
        where TData : IExchange, IPair
    {
        IEnumerable<TData> GetByExchange(string exchange);
        IEnumerable<TData> GetByPair(string pair);
        bool Contains(string exchange, string pair);
        TData this[string exchange, string pair] { get; set; }

        IEnumerable<string> GetAllPairs(string exchange = null);

        IEnumerable<string> GetAllExchanges();
    }

    public class DataCollection<TData> : KeyedCollection<string, TData> where TData : IExchange, IPair
    {
        public IEnumerable<TData> GetByExchange(string exchange)
        {
            return Items.Where(x => x.Exchange == exchange);
        }

        public IEnumerable<TData> GetByPair(string pair)
        {
            return Items.Where(x => x.Pair == pair);
        }

        public bool Contains(string exchange, string pair)
        {
            return Items.Any(x => x.Exchange == exchange && x.Pair == pair);
        }

        public TData this[string exchange, string pair]
        {
            get
            {
                return Items.FirstOrDefault(x => x.Exchange == exchange && x.Pair == pair);
            }
            set
            {
                var item = Items.FirstOrDefault(x => x.Exchange == exchange && x.Pair == pair);
                if (item != null)
                {
                    Items.Remove(item);
                }
                Items.Add(value);
            }
        }

        public IEnumerable<string> GetAllPairs(string exchange = null)
        {
            var source = Items.AsEnumerable();
            if(exchange != null)
                source = source.Where(x => x.Exchange == exchange);

            return source.Select(x => x.Pair);
        }

        public IEnumerable<string> GetAllExchanges()
        {
            return Items.Select(x => x.Exchange).Distinct();
        }

        protected override string GetKeyForItem(TData item)
        {
            return $"{item.Exchange}-{item.Pair}";
        }
    }
}