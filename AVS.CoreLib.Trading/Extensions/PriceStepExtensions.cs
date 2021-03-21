namespace AVS.CoreLib.Trading.Extensions
{
    public static class PriceStepExtensions
    {
        public static decimal GetPriceStep(this decimal price, decimal customValue = -1)
        {
            if (customValue > 0)
                return customValue;

            var round = 0.00000001m;
            if (price > 10000) round = 50;
            else if (price > 1000) round = 20;
            else if (price > 100) round = 2;
            else if (price > 10) round = 0.1m;
            else if (price > 1) round = 0.01m;
            else if (price > 0.1m) round = 0.001m;
            else if (price > 0.01m) round = 0.0001m;
            else if (price > 0.001m) round = 0.00001m;
            else if (price > 0.0001m) round = 0.000001m;
            return round / 2;
        }
    }
}