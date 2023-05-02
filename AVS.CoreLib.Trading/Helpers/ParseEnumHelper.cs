using System;
using AVS.CoreLib.Trading.Enums;
using AVS.CoreLib.Trading.Enums.Futures;

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

                case nameof(PositionSide):
                {
                    obj = ParseEnumHelper.ParsePositionSide(value);
                    return true;
                }
                case nameof(FuturesOrderType):
                {
                    obj = ParseEnumHelper.ParseFuturesOrderType(value);
                    return true;
                }
                case nameof(WorkingType):
                {
                    obj = ParseEnumHelper.ParseWorkingType(value);
                    return true;
                }
                case nameof(MarginType):
                {
                    obj = ParseEnumHelper.ParseMarginType(value);
                    return true;
                }
                case nameof(AccountUpdateReason):
                {
                    obj = ParseEnumHelper.ParseAccountUpdateReason(value);
                    return true;
                }
                case nameof(ExecutionType):
                {
                    obj = ParseEnumHelper.ParseExecutionType(value);
                    return true;
                }

                default:
                {
                    obj = null;
                    return false;
                }
            }
        }

        private static ExecutionType ParseExecutionType(string value)
        {
            switch (value.ToUpper())
            {
                case "NEW":
                    return ExecutionType.New;
                case "CANCELED":
                    return ExecutionType.Canceled;
                case "REPLACED":
                    return ExecutionType.Replaced;
                case "REJECTED":
                    return ExecutionType.Rejected;
                case "TRADE":
                    return ExecutionType.Trade;
                case "EXPIRED":
                    return ExecutionType.Expired;
                case "AMENDMENT":
                    return ExecutionType.Amendment;
                default:
                    throw new NotSupportedException($"Unknown {nameof(ExecutionType)} '{value}'");
            }
        }

        private static AccountUpdateReason ParseAccountUpdateReason(string value)
        {
            switch (value.ToUpper())
            {
                case "DEPOSIT":
                    return AccountUpdateReason.Deposit;
                case "WITHDRAW":
                    return AccountUpdateReason.Withdraw;
                case "COIN_SWAP_DEPOSIT":
                    return AccountUpdateReason.CoinSwapDeposit;
                case "COIN_SWAP_WITHDRAW":
                    return AccountUpdateReason.CoinSwapWithdraw;
                case "ADMIN_DEPOSIT":
                    return AccountUpdateReason.AdminDeposit;
                case "ADMIN_WITHDRAW":
                    return AccountUpdateReason.AdminWithdraw;
                case "ORDER":
                    return AccountUpdateReason.Order;
                case "FUNDING_FEE":
                    return AccountUpdateReason.FundingFee;
                case "ADJUSTMENT":
                    return AccountUpdateReason.Adjustment;
                case "WITHDRAW_REJECT":
                    return AccountUpdateReason.WithdrawReject;

                case "INSURANCE_CLEAR":
                    return AccountUpdateReason.InsuranceClear;
                case "MARGIN_TRANSFER":
                    return AccountUpdateReason.MarginTransfer;
                case "MARGIN_TYPE_CHANGE":
                    return AccountUpdateReason.MarginTypeChange;
                case "ASSET_TRANSFER":
                    return AccountUpdateReason.AssetTransfer;
                case "OPTIONS_PREMIUM_FEE":
                    return AccountUpdateReason.OptionsPremiumFee;
                case "OPTIONS_SETTLE_PROFIT":
                    return AccountUpdateReason.OptionsSettleProfit;
                //case "AUTO_EXCHANGE":
                //    return AccountUpdateReason.CoinSwapWithdraw;
                default:
                    throw new NotSupportedException($"Unknown {nameof(AccountUpdateReason)} '{value}'");
            }
        }


        private static MarginType ParseMarginType(string value)
        {
            switch (value.ToUpper())
            {
                case "ISOLATED":
                    return MarginType.Isolated;
                case "CROSS":
                    return MarginType.Cross;
                default:
                    throw new NotSupportedException($"Unknown {nameof(MarginType)} '{value}'");
            }
        }

        private static WorkingType ParseWorkingType(string value)
        {
            switch (value.ToUpper())
            {
                case "CONTRACT":
                case "CONTRACT_PRICE":
                    return WorkingType.Contract;
                case "MARK":
                case "MARK_PRICE":
                    return WorkingType.Mark;
                default:
                    throw new NotSupportedException($"Unknown {nameof(WorkingType)} '{value}'");
            }
        }

        public static TimeInForce ParseTimeInForce(string value)
        {
            switch (value.ToUpper())
            {
                case "GTC":
                    return TimeInForce.GTC;
                case "GTX":
                    return TimeInForce.GTX;
                case "FOK":
                    return TimeInForce.FOK;
                case "IOC":
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

        public static PositionSide ParsePositionSide(string value)
        {
            switch (value[0])
            {
                case 'l':
                case 'L':
                    return PositionSide.Long;
                case 's':
                case 'S':
                    return PositionSide.Short;
                case 'b':
                case 'B':
                    return PositionSide.Both;
            }
            throw new ArgumentOutOfRangeException($"{value} unknown OrderSide");
        }

        public static FuturesOrderType ParseFuturesOrderType(string value)
        {
            switch (value.ToUpper())
            {
                case "M":
                case "MARKET":
                    return FuturesOrderType.Market;
                case "L":
                case "LIMIT":
                    return FuturesOrderType.Limit;
                case "S":
                case "STOP":
                    return FuturesOrderType.Stop;
                case "SM":
                case "STOP_MARKET":
                    return FuturesOrderType.StopMarket;
                case "TP":
                case "TAKE_PROFIT":
                    return FuturesOrderType.TakeProfit;
                case "TPM":
                case "TAKE_PROFIT_MARKET":
                    return FuturesOrderType.TakeProfitMarket;
                case "TSM":
                case "TRAILING_STOP_MARKET":
                    return FuturesOrderType.TrailingStopMarket;
                case "LIQUIDATION":
                    return FuturesOrderType.Liquidation;
            }

            throw new ArgumentOutOfRangeException($"{value} unknown kind of {nameof(OrderType)}");
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
                    if (str is "C" or "CANCELED")
                        return OrderState.Canceled;

                    throw new NotSupportedException($"Unknown {nameof(OrderState)} '{value}'");
                }
            }
        }
    }
}