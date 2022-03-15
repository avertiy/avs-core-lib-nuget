using System;
using AVS.CoreLib.Trading.Exceptions;
using AVS.CoreLib.Trading.Helpers;

namespace AVS.CoreLib.Trading.Structs
{
    [Obsolete("Use CurrencyValue instead")]
    public struct CurrencyBalance
    {
        public string Currency;
        public decimal Balance;

        public CurrencyBalance(string currency)
        {
            Currency = currency;
            Balance = 0;
        }

        public CurrencyBalance(string currency, decimal balance)
        {
            Currency = currency;
            Balance = balance;
        }

        public void Debit(decimal amount)
        {
            if (Balance < amount)
                throw new NotEnoughBalanceException($"Debit of {amount:N3} {Currency} is not possible");
            Balance -= amount;
        }

        public void Credit(decimal amount)
        {
            Balance += amount;
        }

        public override string ToString()
        {
            return $"{Balance:0.00} {Currency}";
        }

        /// <summary>
        /// parses values like 10.00 UAH, 2.2 XRP into currency balance 
        /// </summary>
        /// <param name="str">"100.00 UAH"</param>
        public static CurrencyValue Parse(string str)
        {
            var parts = str.Split(' ');
            if (parts.Length != 2)
                throw new ArgumentException($"String '{str}' is not recognized as a valid currency value");

            if (NumericHelper.TryParseDecimal(parts[0], out var value))
            {
                return new CurrencyValue(parts[1], value);
            }

            throw new ArgumentException($"Unable to parse '{str}' into currency value");
        }

        public static CurrencyBalance operator +(CurrencyBalance b, decimal balance)
        {
            b.Balance += balance;
            return b;
        }

        public static CurrencyBalance operator -(CurrencyBalance b, decimal balance)
        {
            b.Balance -= balance;
            return b;
        }

        public static implicit operator decimal(CurrencyBalance b)
        {
            return b.Balance;
        }
    }
}