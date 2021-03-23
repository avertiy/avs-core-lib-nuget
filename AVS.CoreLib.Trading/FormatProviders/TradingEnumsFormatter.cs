using System;
using AVS.CoreLib.Text.Formatters;
using AVS.CoreLib.Trading.Enums;

namespace AVS.CoreLib.Trading.FormatProviders
{
    public class TradingEnumsFormatter : CustomFormatter
    {
        /// <summary>
        /// qualifiers: +; c|character; n|number
        /// </summary>
        public static string GetQualifiers => "+; c|character; n|number";
        protected override string CustomFormat(string format, object arg, IFormatProvider formatProvider)
        {
            switch (arg)
            {
                case TradeType tradeType:
                    switch (format)
                    {
                        case "+":
                            return tradeType == TradeType.Buy ? "+" : "-";
                        case "c":
                        case "character":
                            return tradeType == TradeType.Buy ? "buy" : "sell";
                        case "n":
                        case "number":
                            return ((int)tradeType).ToString();
                        default:
                            return tradeType.ToString();
                    }
                case OrderSide orderSide:
                    switch (format)
                    {
                        case "+":
                            return orderSide == OrderSide.Buy ? "+" : "-";
                        case "c":
                        case "character":
                            return orderSide == OrderSide.Buy ? "buy" : "sell";
                        case "n":
                        case "number":
                            return ((int)orderSide).ToString();
                        default:
                            return orderSide.ToString();
                    }
                case PositionType positionType:
                    switch (format)
                    {
                        case "+":
                            return positionType == PositionType.Long ? "+" : "-";
                        case "c":
                        case "character":
                            return positionType == PositionType.Long ? "long" : "short";
                        case "n":
                        case "number":
                            return ((int)positionType).ToString();
                        default:
                            return positionType.ToString();
                    }
                default:
                    return arg?.ToString();
            }
        }

        protected override bool Match(string format)
        {
            switch (format)
            {
                case "+"://e.g. TradeType.Buy will be as '+', Sell as '-'
                case "c"://character representation -> buy / sell
                case "n"://number representation -> 1 / 2
                case "character"://character representation -> buy / sell
                case "number"://number representation -> 1 / 2
                    return true;
                default:
                    return false;
            }
        }
    }
}