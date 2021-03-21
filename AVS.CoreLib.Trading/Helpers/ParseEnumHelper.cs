using System;
using AVS.CoreLib.Trading.Enums;

namespace AVS.CoreLib.Trading.Helpers
{
    public static class ParseEnumHelper
    {
        public static TradeType ParseTradeType(string value)
        {
            switch (value[0])
            {
                case 'b':
                case 'B':
                    return TradeType.Buy;
                case 'a'://ask
                case 's':
                case 'S':
                    return TradeType.Sell;
            }
            throw new ArgumentOutOfRangeException($"{value} unknown TradeType");
        }

        public static TradeCategory ParseTradeCategory(string value)
        {
            switch (value)
            {
                case "exchange":
                    return TradeCategory.Exchange;
                case "marginTrade":
                    return TradeCategory.MarginTrade;
                case "settlement":
                    return TradeCategory.Settlement;
                case "lendingFees":
                    return TradeCategory.LendingFees;
            }
            throw new ArgumentOutOfRangeException($"{value} unknown TradeCategory");
        }

        public static OrderSide ParseOrderSide(string value)
        {
            switch (value[0])
            {
                case 'b':
                case 'B':
                    return OrderSide.Buy;
                case 's':
                case 'S':
                    return OrderSide.Sell;
            }
            throw new ArgumentOutOfRangeException($"{value} unknown OrderSide");
        }

        public static OrderType ParseOrderKind(string value)
        {
            switch (value[0])
            {
                case 'm':
                case 'M':
                    return OrderType.Market;
                case 'l':
                case 'L':
                    return OrderType.Limit;
                case 'i':
                case 'I':
                    return OrderType.Immediate;
                case 'f':
                case 'F':
                    return OrderType.FillOrKill;
                case 'g':
                case 'G':
                    return OrderType.GoodTillCanceled;
                case 's':
                case 'S':
                    return OrderType.StopLimit;
            }
            throw new ArgumentOutOfRangeException($"{value} unknown OrderSide");
        }
    }
}