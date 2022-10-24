using System;
using AVS.CoreLib.Trading.Enums;

namespace AVS.CoreLib.Trading.Extensions
{
    public static class TimeFrameExtensions
    {
        public static TimeFrame GetSeniorTimeFrame(this TimeFrame timeframe)
        {
            switch (timeframe)
            {
                case TimeFrame.M1:
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
                    throw new ArgumentOutOfRangeException(nameof(timeframe));
            }
        }
    }
}