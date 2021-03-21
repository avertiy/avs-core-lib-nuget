namespace AVS.CoreLib.Trading.Structs
{
    public readonly struct ExchangeFees
    {
        public ExchangeFees(decimal makerFee, decimal takerFee)
        {
            MakerFee = makerFee;
            TakerFee = takerFee;
        }

        public decimal MakerFee { get; }
        public decimal TakerFee { get; }

        public decimal SellAsMaker(decimal price, decimal amount)
        {
            return price * amount * (1 - MakerFee);
        }

        public decimal BuyAsMaker(decimal price, decimal amount)
        {
            return price * amount * (1 + MakerFee);
        }

        public decimal SellAsTaker(decimal price, decimal amount)
        {
            return price * amount * (1 - TakerFee);
        }

        public decimal BuyAsTaker(decimal price, decimal amount)
        {
            return price * amount * (1 + TakerFee);
        }
    }
}