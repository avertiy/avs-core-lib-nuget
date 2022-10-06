using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using AVS.CoreLib.Trading.Extensions;
using AVS.CoreLib.Trading.Helpers;
using AVS.CoreLib.Trading.Types;

namespace AVS.CoreLib.Trading.Models.Balance
{
    public class BalanceBook
    {
        [JsonInclude]
        private Dictionary<string, decimal> Balances { get; }

        [JsonInclude]
        private List<UpdateBalanceCommand> Commands { get; }

        public BalanceBook()
        {
            Balances = new Dictionary<string, decimal>();
            Commands = new List<UpdateBalanceCommand>();
        }

        public BalanceBook(params CurrencyValue[] balances)
        {
            Balances = new Dictionary<string, decimal>();
            Commands = new List<UpdateBalanceCommand>();
            foreach (var currencyValue in balances)
            {
                Credit(currencyValue.Currency, currencyValue.Value);
            }
        }

        [JsonConstructor]
        public BalanceBook(Dictionary<string, decimal> balances, List<UpdateBalanceCommand> commands)
        {
            Balances = balances;
            Commands = commands;
        }

        public Dictionary<string, decimal>.KeyCollection Currencies => Balances.Keys;

        /// <summary>
        /// Gets the number of currencies in balance book 
        /// </summary>
        public int Count => Balances.Count;

        public void Update(UpdateBalanceCommand command)
        {
            if (command.Currency == null)
            {
                throw new ArgumentException("Currency is required");
            }

            Commands.Add(command);
            if (!Balances.ContainsKey(command.Currency))
            {
                Balances.Add(command.Currency, 0);
            }

            if (command.Type == UpdateBalanceCommandType.Credit)
            {
                Balances[command.Currency] += command.Amount;
            }
            else
            {
                Balances[command.Currency] -= command.Amount;
            }
        }

        public IList<UpdateBalanceCommand> GetBalanceHistory(string currency)
        {
            return Commands.Where(x => x.Currency == currency).ToList();
        }

        public bool Contains(string currency)
        {
            return Balances.ContainsKey(currency);
        }

        public void Credit(string currency, in decimal amount)
        {
            Update(new UpdateBalanceCommand() { Currency = currency, Amount = amount, Type = UpdateBalanceCommandType.Credit });
        }

        public void Debit(string currency, in decimal amount)
        {
            Update(new UpdateBalanceCommand() { Currency = currency, Amount = amount, Type = UpdateBalanceCommandType.Debit });
        }

        public CurrencyValue GetCurrencyBalance(string currency)
        {
            return new CurrencyValue(currency, Balances[currency]);
        }

        #region operators overloading

        public static BalanceBook operator +(BalanceBook book, CurrencyValue balance)
        {
            book.Credit(balance.Currency, balance.Value);
            return book;
        }

        public static BalanceBook operator -(BalanceBook book, CurrencyValue balance)
        {
            book.Debit(balance.Currency, balance.Value);
            return book;
        }

        public static implicit operator BalanceBook(string str)
        {
            return Parse(str);
        }

        #endregion

        public decimal this[string currency] => Balances[currency];

        public decimal GetBalance(string currency)
        {
            return this[currency];
        }

        public IEnumerable<CurrencyValue> GetAllBalances()
        {
            return Balances.Select(x => new CurrencyValue(x.Key, x.Value));
        }

        public void Clear()
        {
            Balances.Clear();
            Commands.Clear();
        }

        public void RollbackLastCommand()
        {
            var cmd = Commands.Last();
            Commands.Remove(cmd);
            if (cmd.Type == UpdateBalanceCommandType.Credit)
            {
                Balances[cmd.Currency] -= cmd.Amount;
            }
            else
            {
                Balances[cmd.Currency] += cmd.Amount;
            }
        }

        public override string ToString()
        {
            return string.Join(";", Balances.Select(x => $"{x.Value.FormatNumber()} {x.Key}"));
        }

        /// <summary>
        /// Can parse BalanceBook from strings like "0.2 BTC;10 USD; 25 TRX;0.05 ETH" 
        /// </summary>
        public static BalanceBook Parse(string str)
        {
            var parts = str.Split(';', ' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length % 2 != 0)
                throw new ArgumentException($"String '{str}' is not recognized as a valid {nameof(BalanceBook)} string (e.g. 0.2 BTC; 10 USD)");

            var book = new BalanceBook();

            for (var i = 0; i < parts.Length; i += 2)
            {
                if (!NumericHelper.TryParseDecimal(parts[0], out var amount))
                {
                    throw new ArgumentException($"{nameof(BalanceBook)} unable to parse amount from '{parts[0]}' of '{str}'");
                }

                book.Credit(parts[i + 1], amount);
            }

            return book;
        }
    }
}