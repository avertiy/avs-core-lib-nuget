using AVS.CoreLib.Trading.Exceptions;

namespace AVS.CoreLib.Trading.Structs
{
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