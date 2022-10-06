using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using AVS.CoreLib.Extensions;

namespace AVS.CoreLib.Trading.Collections
{
    /// <summary>
    /// base type to make bind-able string[] parameter type <see cref="Exchanges"/>
    /// </summary>
    public abstract class StringCollection : Collection<string>, ICollection<string>
    {
        internal abstract string[] AllItems { get; }

        protected StringCollection()
        {
        }

        protected StringCollection(IList<string> items) : base(items)
        {
        }

        public void Add(params string[] items)
        {
            foreach (var item in items)
                Items.Add(item);
        }

        /// <summary>
        /// the Add method implemented explicitly due to model binding mechanics 
        /// </summary>
        void ICollection<string>.Add(string str)
        {
            if (string.IsNullOrEmpty(str))
                return;

            if (str.Either("all", "*") && AllItems.Length > 0)
                foreach (var exchange in AllItems)
                    base.Add(exchange);
            else if (str.Contains(","))
                foreach (var exchange in str.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    base.Add(exchange);
            else
                base.Add(str);
        }

        public override string ToString()
        {
            return string.Join(',', Items);
        }

        [DebuggerStepThrough]
        public static implicit operator string[](StringCollection value)
        {
            if (value == null || value.Count == 0)
                return new string[] { };
            return value.Items.ToArray();
        }

        public static T Parse<T>(string str) where T : StringCollection, new()
        {
            var res = new T();
            if (string.IsNullOrEmpty(str))
                return res;
            res.Add(str.Either("all", "*") ? res.AllItems : str.Split(',', StringSplitOptions.RemoveEmptyEntries));
            return res;
        }
    }
}