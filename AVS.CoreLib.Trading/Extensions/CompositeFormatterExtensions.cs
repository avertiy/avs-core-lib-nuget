using AVS.CoreLib.Text.Formatters;
using AVS.CoreLib.Trading.Enums;

namespace AVS.CoreLib.Trading.Extensions
{
    /// <summary>
    /// GenericFormatter extensions
    /// </summary>
    public static class CompositeFormatterExtensions
    {
        /// <summary>
        /// Register <see cref="TradeType"/> enum formatting
        /// </summary>
        public static CompositeFormatter AddTradeTypeFormatter(this CompositeFormatter formatter)
        {
            formatter.AddTypeFormatter(new[] { "+", "c", "character", "n", "number" }, (string format, TradeType x) =>
             {
                 switch (format)
                 {
                     case "+":
                         return x == TradeType.Buy ? "+" : "-";
                     case "c":
                     case "character":
                         return x == TradeType.Buy ? "buy" : "sell";
                     case "n":
                     case "number":
                         return ((int)x).ToString();
                     default:
                         return x.ToString();
                 }
             });
            return formatter;
        }

        /// <summary>
        /// Register <see cref="OrderSide"/> enum formatting
        /// </summary>
        public static CompositeFormatter AddOrderSideFormatter(this CompositeFormatter formatter)
        {
            formatter.AddTypeFormatter(new[] { "+", "c", "character", "n", "number" }, (string format, OrderSide x) =>
            {
                switch (format)
                {
                    case "+":
                        return x == OrderSide.Buy ? "+" : "-";
                    case "c":
                    case "character":
                        return x == OrderSide.Buy ? "buy" : "sell";
                    case "n":
                    case "number":
                        return ((int)x).ToString();
                    default:
                        return x.ToString();
                }
            });
            return formatter;
        }

        /// <summary>
        /// Register <see cref="PositionType"/> enum formatting
        /// </summary>
        public static CompositeFormatter AddPositionTypeFormatter(this CompositeFormatter formatter)
        {
            formatter.AddTypeFormatter(new[] { "+", "c", "character", "n", "number" }, (string format, PositionType x) =>
            {
                switch (format)
                {
                    case "+":
                        return x == PositionType.Long ? "+" : "-";
                    case "c":
                    case "character":
                        return x == PositionType.Long ? "long" : "short";
                    case "n":
                    case "number":
                        return ((int)x).ToString();
                    default:
                        return x.ToString();
                }
            });
            return formatter;
        }
    }
}