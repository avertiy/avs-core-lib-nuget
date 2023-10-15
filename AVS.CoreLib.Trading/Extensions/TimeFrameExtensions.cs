using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AVS.CoreLib.Trading.Enums;
using AVS.CoreLib.Trading.Enums.TA;

namespace AVS.CoreLib.Trading.Extensions
{
    public static class TimeFrameExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsIntraday(this TimeFrame timeframe)
        {
            return (int)timeframe < (int)TimeFrame.D;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMinutes(this TimeFrame timeframe)
        {
            return (int)timeframe is < (int)TimeFrame.H1 and >= (int)TimeFrame.M1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        public static int ToSeconds(this TimeFrame timeframe)
        {
            return (int)timeframe;
        }

        /// <summary>
        /// Calculate timespan = (int)timeframe x count 
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalcTimespan(this TimeFrame timeframe, int count)
        {
            return (int)timeframe * count;
        }

        /// <summary>
        /// Calculate bar count = period / (int)timeframe
        /// i.e. how many bars (count) cover the specified period
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalcBarCount(this TimeFrame timeframe, double periodInSeconds)
        {
            return (int) (periodInSeconds / (int)timeframe);
        }

        /// <summary>
        /// Calculate bar count from <see cref="fromDate"/> till <see cref="DateTime.Now"/>
        /// </summary>
        public static int CalcBarCountFrom(this TimeFrame timeframe, DateTime fromDate)
        {
            var ts = DateTime.Now - fromDate;
            var count = (int)(ts.TotalSeconds / (int)timeframe);
            return count;
        }
        public static TimeFrameType GetTimeFrameType(this TimeFrame timeframe)
        {
            switch (timeframe)
            {
                case TimeFrame.S1:
                case TimeFrame.S30:
                case TimeFrame.M1:
                    return TimeFrameType.Micro;
                case TimeFrame.M3:
                case TimeFrame.M5:
                case TimeFrame.M15:
                case TimeFrame.M30:
                    return TimeFrameType.Small;

                case TimeFrame.H1:
                case TimeFrame.H2:
                case TimeFrame.H3:
                case TimeFrame.H4:
                case TimeFrame.H12:
                    return TimeFrameType.Intraday;
                case TimeFrame.D:
                    return TimeFrameType.Day;
                case TimeFrame.Week:
                case TimeFrame.Month:
                    return TimeFrameType.Week;
                default:
                    throw new ArgumentOutOfRangeException(nameof(timeframe));
            }
        }

        public static TimeFrame? GetSeniorTimeFrame(this TimeFrame timeframe)
        {
            switch (timeframe)
            {
                case TimeFrame.M1:
                case TimeFrame.M3:
                case TimeFrame.M5:
                    return TimeFrame.H1;
                case TimeFrame.M15:
                case TimeFrame.M30:
                    return TimeFrame.H4;
                case TimeFrame.H1:
                case TimeFrame.H2:
                case TimeFrame.H4:
                    return TimeFrame.D;
                case TimeFrame.H12:
                case TimeFrame.D:
                    return TimeFrame.Week;
                default:
                    return null;
            }
        }

        public static TimeFrame? GetHTF1(this TimeFrame timeframe)
        {
            switch (timeframe)
            {
                case TimeFrame.M1:
                case TimeFrame.M3:
                case TimeFrame.M5:
                    return TimeFrame.M15;
                case TimeFrame.M15:                
                    return TimeFrame.H1;
                case TimeFrame.M30:
                case TimeFrame.H1:                
                    return TimeFrame.H4;
                case TimeFrame.H2:
                case TimeFrame.H4:
                    return TimeFrame.D;
                case TimeFrame.H12:
                case TimeFrame.D:
                    return TimeFrame.Week;
                default:
                    return null;
            }
        }

        public static TimeFrame? GetHTF2(this TimeFrame timeframe)
        {
            switch (timeframe)
            {
                case TimeFrame.M1:
                case TimeFrame.M3:
                case TimeFrame.M5:
                    return TimeFrame.H1;
                case TimeFrame.M15:
                    return TimeFrame.H4;
                case TimeFrame.M30:
                case TimeFrame.H1:
                case TimeFrame.H2:
                    return TimeFrame.D;                
                case TimeFrame.H4:
                    return TimeFrame.Week;                
                default:
                    return null;
            }
        }

        public static TimeFrame? GetHTF3(this TimeFrame timeframe)
        {
            switch (timeframe)
            {
                case TimeFrame.M1:
                case TimeFrame.M3:
                case TimeFrame.M5:
                    return TimeFrame.H4;
                case TimeFrame.M15:
                    return TimeFrame.D;
                case TimeFrame.M30:
                case TimeFrame.H1:
                    return TimeFrame.Week;                
                default:
                    return null;
            }
        }

        public static TimeFrame[] GetHTF(this TimeFrame timeframe, HTFEnum htf)
        {
            var list = new List<TimeFrame>(3);

            if (htf.HasFlag(HTFEnum.HTF1))
            {
                var tf = timeframe.GetHTF1();
                if(tf.HasValue)
                    list.Add(tf.Value);
            }

            if (htf.HasFlag(HTFEnum.HTF2))
            {
                var tf = timeframe.GetHTF2();
                if (tf.HasValue)
                    list.Add(tf.Value);
            }

            if (htf.HasFlag(HTFEnum.HTF3))
            {
                var tf = timeframe.GetHTF3();
                if (tf.HasValue)
                    list.Add(tf.Value);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Volatility is kind of average* bar length at a given scale (timeframe)
        /// *average calculated taking into account only normal-size bars, similar princip as for ATR (short, long and paranormal bars are ignored as they are unusual)
        /// e.g.. tf:D ~7-8%, tf:H1 ~ 1.2-1.8%
        /// for some assets it could differ but in most cases ATR bar will 
        /// </summary>
        public static decimal GetVolatilityByTimeFrame(this TimeFrame timeframe)
        {
            // for BTC and most other big cap coins we can assume avg volatility i.e.
            // tf:D ~7-8%
            // tf:1H ~ 1.5-2% 
            // tf:M5 ~0.21%
            switch (timeframe)
            {
                case TimeFrame.M1:
                    return 0.12m;
                case TimeFrame.M3:
                case TimeFrame.M5:
                    return 0.22m;
                case TimeFrame.M15:
                    return 0.5m;
                case TimeFrame.M30:
                    return 0.8m;
                case TimeFrame.H1:
                    return 1.5m;
                case TimeFrame.H2:
                    return 2.5m;
                case TimeFrame.H4:
                    return 4m;
                case TimeFrame.D:
                    return 8;
                case TimeFrame.Week:
                    return 16;
                case TimeFrame.Month:
                    return 30;
                default:
                    throw new NotImplementedException();
            }
        }

    }
}