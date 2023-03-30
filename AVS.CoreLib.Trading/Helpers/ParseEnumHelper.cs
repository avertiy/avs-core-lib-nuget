using System;
using AVS.CoreLib.Trading.Enums;
using AVS.CoreLib.Trading.FormatProviders;

namespace AVS.CoreLib.Trading.Helpers
{
    public static class ParseEnumHelper
    {
        public static bool TryParse(string value, Type enumType, out object obj)
        {
            switch (enumType.Name)
            {
                case nameof(OrderSide):
                {
                    obj = ParseEnumHelper.ParseOrderSide(value);
                    return true;
                }
                
                case nameof(OrderState):
                {
                    obj = ParseEnumHelper.ParseOrderState(value);
                    return true;
                }

                case nameof(OrderType):
                {
                    obj = ParseEnumHelper.ParseOrderType(value);
                    return true;
                }

                case nameof(TimeInForce):
                {
                    obj = ParseEnumHelper.ParseTimeInForce(value);
                    return true;
                }
                case nameof(TradeType):
                {
                    obj = ParseEnumHelper.ParseTradeType(value);
                    return true;
                }

                case nameof(TradeCategory):
                {
                    obj = ParseEnumHelper.ParseTradeCategory(value);
                    return true;
                }
                case nameof(AccountType):
                {
                    obj = ParseEnumHelper.ParseAccountType(value);
                    return true;
                }

                default:
                {
                    obj = null;
                    return false;
                }
            }
        }

        public static TimeInForce ParseTimeInForce(string value)
        {
            switch (value.ToUpper()[0])
            {
                case 'G':
                    return TimeInForce.GTC;
                case 'F':
                    return TimeInForce.FOK;
                case 'I':
                    return TimeInForce.IOC;
                default:
                    throw new NotSupportedException($"Unknown {nameof(TimeInForce)} '{value}'");
            }
        }

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

        public static OrderType ParseOrderType(string value)
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
                    return OrderType.FOK;
                case "GTC":
                case "GOOD_TILL_CANCEL":
                    return OrderType.GTC;
                case "SL":
                case "STOP_LIMIT":
                    return OrderType.StopLimit;
                case "S":
                case "STOP":
                    return OrderType.Stop;
                case "OCO":
                case "OCTO":
                    return OrderType.OCO;
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
            var str = value.ToUpper();
            switch (str[0])
            {
                case 'N':
                    return OrderState.New;
                case 'P':
                    return OrderState.PartiallyFilled;
                case 'F':
                    return OrderState.Filled;
                default:
                {
                    if(str == "CLOSED")
                        return OrderState.Filled;
                    if (str is "C" or "CANCELLED")
                        return OrderState.Canceled;

                    throw new NotSupportedException($"Unknown {nameof(OrderState)} '{value}'");
                }
            }
        }
    }
}