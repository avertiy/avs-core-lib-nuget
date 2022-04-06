using System;
using AVS.CoreLib.Text.Formatters;
using AVS.CoreLib.Trading.Abstractions;
using AVS.CoreLib.Trading.Extensions;

namespace AVS.CoreLib.Trading.FormatProviders
{
    /// <summary>
    /// 
    /// </summary>
    public class OhlcFormatter : CustomFormatter
    {
        /// <summary>
        /// qualifiers: "q|quote; b|base; p|pair; Q|B|symbol"
        /// </summary>
        public static string GetQualifiers => "c|color;s|size; h|height; t|type; H|high; L|low; O|open; C:close";
        protected override string CustomFormat(string format, object arg, IFormatProvider formatProvider)
        {
            switch (arg)
            {
                case IOhlc ohlc:
                    switch (format)
                    {
                        case "H":
                        case "high":
                            return ohlc.High.FormatAsPrice();
                        case "L":
                        case "low":
                            return ohlc.Low.FormatAsPrice();
                        case "O":
                        case "open":
                            return ohlc.Open.FormatAsPrice();
                        case "C":
                        case "close":
                            return ohlc.Close.FormatAsPrice();
                        case "h":
                        case "height":
                            return $"{ohlc.CandleHeight():P}";
                        case "t":
                        case "type":
                            return ohlc.GetCandleType().ToString();
                        case "c":
                        case "color":
                            return ohlc.GetCandleColor().ToString();
                        case "s":
                        case "size":
                            return ohlc.GetCandleSize().ToString();
                        case "ohlc":
                        case "OHLC":
                            return string.Format("{0} {1} {2} {3}", 
                                ohlc.Open.FormatAsPrice(),
                                ohlc.High.FormatAsPrice(), 
                                ohlc.Low.FormatAsPrice(), 
                                ohlc.Close.FormatAsPrice()
                                // ohlc.IsGreenCandle() ? "^" : ""
                            );
                        default:
                            return ohlc.ToString();
                    }
                default:
                    return arg?.ToString();
            }
        }

        protected override bool Match(string format)
        {
            switch (format)
            {
                case "O":
                case "H":
                case "L":
                case "C":
                case "OHLC":
                case "ohlc":
                case "open":
                case "high":
                case "low":
                case "close":
                case "h":
                case "height":
                case "t":
                case "type":
                case "s":
                case "size":
                case "c":
                case "color":
                    return true;
                default:
                    return false;
            }
        }
    }
}