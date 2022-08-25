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
            switch (value.ToUpper())
            {
                case "M":
                case "MARKET":
                    return OrderType.Market;
                case "L":
                case "LIMIT":
                    return OrderType.Limit;
                case "I":
                case "IMMEDIATE":
                    return OrderType.Immediate;

                case "FOK":
                case "FILL_OR_KILL":
                    return OrderType.FillOrKill;
                case "GTC":
                case "GOOD_TILL_CANCEL":
                    return OrderType.GoodTillCanceled;
                case "SL":
                case "STOP_LIMIT":
                    return OrderType.StopLimit;
                case "S":
                case "STOP":
                    return OrderType.Stop;
                case "OCTO":
                    return OrderType.OneCancelsTheOther;
            }

            throw new ArgumentOutOfRangeException($"{value} unknown kind of {nameof(OrderType)}");
        }

        public static AccountType ParseAccountType(string value)
        {
            switch (value.ToUpper()[0])
            {
                case 'S': return AccountType.Spot;
                case 'M': return AccountType.Margin;
                case 'L': return AccountType.Lending;
                case 'F': return AccountType.Margin;
                default:
                    throw new NotSupportedException($"Unknown {nameof(AccountType)} '{value}'");
            }
        }

        public static OrderState ParseOrderState(string value)
        {
            switch (value.ToUpper()[0])
            {
                case 'N':
                    return OrderState.New;
                case 'P':
                    return OrderState.PartiallyFilled;
                case 'F':
                    return OrderState.Filled;
                default:
                    throw new NotSupportedException($"Unknown {nameof(OrderState)} '{value}'");
            }
        }
    }
}