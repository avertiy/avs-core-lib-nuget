namespace AVS.CoreLib.Trading.Enums
{
    /// <summary>Represents a time frame of a market.</summary>
    public enum TimeFrame
    {
        None = 0,
        /// <summary> interval of 1 second </summary>
        S1 = 1,
        /// <summary> interval of 30 seconds </summary>
        S30 = 30,
        /// <summary>A time interval of 1 minute</summary>
        M1 = 60,
        /// <summary>A time interval of 3 minutes</summary>
        M3 = 180,
        /// <summary>A time interval of 5 minutes</summary>
        M5 = 300,

        /// <summary>A time interval of 15 minutes</summary>
        M15 = 900,

        /// <summary>A time interval of 30 minutes</summary>
        M30 = 1800,
        /// <summary>A time interval of 1 hour</summary>
        H1 = 3600,
        /// <summary>A time interval of 2 hours</summary>
        H2 = 7200,

        /// <summary>A time interval of 3 hours</summary>
        H3 = 10800,

        /// <summary>A time interval of 4 hours</summary>
        H4 = 14400,
        /// <summary>A time interval of 12 hours</summary>
        H12 = 43200,

        /// <summary>A time interval of a day</summary>
        D = 86400,

        /// <summary>A time interval of 7 days</summary>
        Week = 86400 * 7,

        /// <summary>A time interval of 30 days</summary>
        Month = 86400 * 30
    }

    public enum TimeFrameType
    {
        /// <summary>
        /// less then 5 minutes
        /// </summary>
        Micro,
        /// <summary>
        /// 5M / 15M / 30M
        /// </summary>
        Small,
        /// <summary>
        /// 1H / 4H  (from 1 hour but less than 1D)
        /// </summary>
        Intraday,
        /// <summary>
        /// 1D
        /// </summary>
        Day,
        /// <summary>
        /// Week
        /// </summary>
        Week,
    }
}
