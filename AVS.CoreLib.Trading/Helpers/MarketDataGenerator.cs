using System;
using System.Collections.Generic;
using AVS.CoreLib.Extensions;
using AVS.CoreLib.Trading.Enums;
using AVS.CoreLib.Trading.Extensions;
using AVS.CoreLib.Trading.Models;

namespace AVS.CoreLib.Trading.Helpers
{
    public class MarketDataGenerator
    {
        private static Random Rand { get; } = new Random();
        private static Bar GetBar(decimal open, DateTime timestamp, decimal volatility)
        {
            var d1 = Rand.NextDecimal(0, volatility)*2;
            var d2 = Rand.NextDecimal(0, volatility)*2;
            var high = open* (1 + d1/100); // Generate a random high price slightly higher than open
            var low = open* (1 - d2/100); // Generate a random low price slightly lower than open
            var close = Rand.NextDecimal() * (high - low) + low; // Generate a random close price within the high-low range            
            var volume = Rand.NextDecimal() * 1000;
            var total = volume * close;

            return new Bar
            {
                Open = open.Round(4),
                High = high.Round(4),
                Low = low.Round(4),
                Close = close.Round(4),
                Total = total.Round(4),
                Volume = volume.Round(4),
                Time = timestamp
            };
        }

        public static Bar[] Generate(decimal atr, int length =10, TimeFrame timeframe = TimeFrame.H1)
        {
            var bars = new List<Bar>();
            var time = DateTime.Now.AddSeconds(-((int)timeframe * length)).RoundDown(timeframe);
            var open = 100.0m;
            var volatility = timeframe.GetVolatilityByTimeFrame();
            var ATR = atr.Abs();
            var atr_passed = 0m;            
            while (atr_passed <= ATR && bars.Count < length)
            {
                var rest = ATR - atr_passed;
                var n = length - bars.Count;
                var bar = GetBar(open, time, volatility);
                var len = bar.GetBodyLength();
                var direction = atr > 0 ? bar.IsBullish() : bar.IsBearish();

                if (direction)
                {
                    bars.Add(bar);
                    atr_passed += len;
                    open = bar.Close;                    
                }
                else if (rest / n < len * 0.9m && atr_passed > len)
                {
                    bars.Add(bar);
                    atr_passed -= len;
                    open = bar.Close;                    
                }                
                //if (atr > 0)
                //{
                //    // if direction match than add bar
                //    if (bar.IsBullish())
                //    {
                //        bars.Add(bar);
                //        atr_passed += len;
                //        open = bar.Close;
                //        continue;
                //    }
                //    // filter bars of opposite direction if atr distance is too long relative to number of bars   
                //    else if(rest/n < len *0.9m)
                //    {
                //        bars.Add(bar);
                //        atr_passed -= len;
                //        open = bar.Close;
                //        continue;
                //    }
                //}else if (atr < 0)
                //{
                //    if (bar.IsBearish())
                //    {
                //        bars.Add(bar);
                //        atr_passed += len;
                //        open = bar.Close;
                //        continue;
                //    }
                //    else if (rest/n < len*0.9m)
                //    {
                //        bars.Add(bar);
                //        atr_passed -= len;
                //        open = bar.Close;
                //        continue;
                //    }
                //}                
            }

            return bars.ToArray();
        }

        public static Bar[] GenerateAsc(int length = 100, int bullFactor = 5, TimeFrame timeframe = TimeFrame.H1)
        {
            var bars = new List<Bar>();
            var time = DateTime.Now.AddSeconds(-((int)timeframe * length)).RoundDown(timeframe);
            var open = 100.0m;

            var volatility = timeframe > TimeFrame.H1 ? 0.1m : 0.04m;

            var bullCounter = 0;

            for (int i = 0; i < length; i++)
            {
                var bar = GetBar(open, time, volatility);
                if (bar.IsBearish())
                {
                    if (bullFactor > 0 && bullCounter >= bullFactor)
                        bullCounter = 0;
                    else
                    {
                        i--;
                        continue;
                    }
                }
                else
                {
                    bullCounter++;
                }

                bars.Add(bar);
                open = bar.Close;
                time = time.AddSeconds((int)timeframe);
            }

            return bars.ToArray();
        }

        public static Bar[] GenerateDesc(int length = 100, int bearFactor = 5, TimeFrame timeframe = TimeFrame.H1)
        {
            var bars = new List<Bar>();
            var time = DateTime.Now.AddSeconds(-((int)timeframe * length)).RoundDown(timeframe);
            var open = 100.0m;

            var volatility = timeframe > TimeFrame.H1 ? 0.1m : 0.04m;
            var bearCounter = 0;
            for (int i = 0; i < length; i++)
            {
                var bar = GetBar(open, time, volatility);
                if (bar.IsBullish())
                {
                    if (bearFactor > 0 && bearCounter >= bearFactor)
                        bearCounter = 0;
                    else
                    {
                        i--;
                        continue;
                    }
                }
                else
                {
                    bearCounter++;
                }

                bars.Add(bar);
                open = bar.Close;
                time = time.AddSeconds((int)timeframe);
            }

            return bars.ToArray();
        }

        public static Bar[] Generate(int length = 100, TimeFrame timeframe = TimeFrame.H1)
        {
            var bars = new List<Bar>();
            var time = DateTime.Now.AddSeconds(-((int)timeframe * length)).RoundDown(timeframe);            
            var open = 100.0m;

            var volatility = timeframe > TimeFrame.H1 ? 0.1m : 0.04m;

            for (int i = 0; i < length; i++)
            {
                var bar = GetBar(open, time, volatility);
                bars.Add(bar);
                open = bar.Close;
                time = time.AddSeconds((int)timeframe);
            }

            return bars.ToArray();
        }
    }
}